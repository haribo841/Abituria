[CmdletBinding(DefaultParameterSetName = "Published")]
param(
    [Parameter(Mandatory, ParameterSetName = "Published")]
    [string]$PublishedDirectory,

    [Parameter(Mandatory, ParameterSetName = "Published")]
    [ValidateSet("win-x64", "linux-x64", "osx-x64")]
    [string]$RuntimeIdentifier,

    [Parameter(Mandatory, ParameterSetName = "Components")]
    [string]$ComponentsPath,

    [Parameter(Mandatory)]
    [string]$OutputDirectory,

    [string]$PackageRoot
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Import-Module (Join-Path $PSScriptRoot "Release.Common.psm1") -Force

$maximumEvidenceLength = 16MB
$maximumComponentCount = 512
$maximumEvidenceFileCount = 64
$evidenceNamePattern =
    '^(?:licen[cs]e|copying|notices?|third[-_. ]?party(?:[-_. ]?notices?)?)(?:[-_. ].*)?$'
$externalComponentPattern =
    '^Microsoft[.]NETCore[.]App[.](?:Runtime|Host)[.](?:win-x64|linux-x64|osx-x64)$'
$pathComparison = if ([Runtime.InteropServices.RuntimeInformation]::IsOSPlatform(
    [Runtime.InteropServices.OSPlatform]::Windows
)) {
    [StringComparison]::OrdinalIgnoreCase
}
else {
    [StringComparison]::Ordinal
}

function Get-FullPathPrefix([string]$Path) {
    return [IO.Path]::GetFullPath($Path).TrimEnd(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar
    ) + [IO.Path]::DirectorySeparatorChar
}

function Assert-PathInside([string]$Path, [string]$Root, [string]$Description) {
    $fullPath = [IO.Path]::GetFullPath($Path)
    $rootPrefix = Get-FullPathPrefix -Path $Root
    if (-not $fullPath.StartsWith($rootPrefix, $pathComparison)) {
        throw "$Description escapes its expected root: '$fullPath'."
    }
    return $fullPath
}

function Get-InputComponents {
    if ($PSCmdlet.ParameterSetName -eq "Published") {
        $script:inputKind = "published-deps"
        return @(
            Get-PublishedNuGetComponents `
                -PublishedDirectory $PublishedDirectory `
                -RuntimeIdentifier $RuntimeIdentifier
        )
    }

    $script:inputKind = "component-manifest"
    $resolvedComponentsPath = (Resolve-Path -LiteralPath $ComponentsPath).Path
    $document = Get-Content -LiteralPath $resolvedComponentsPath -Raw -Encoding UTF8 |
        ConvertFrom-Json
    if ($document -isnot [Array] -and
        $document.PSObject.Properties.Name -contains "components") {
        return @($document.components)
    }
    return @($document)
}

function Get-NormalizedComponents([object[]]$Components) {
    if ($Components.Count -eq 0) {
        throw "The component list is empty."
    }
    if ($Components.Count -gt $maximumComponentCount) {
        throw "The component list exceeds the limit of $maximumComponentCount entries."
    }

    $byKey = @{}
    foreach ($component in $Components) {
        $id = [string]$component.Id
        $version = [string]$component.Version
        if ([string]::IsNullOrWhiteSpace($id) -or
            $id -notmatch '^[A-Za-z0-9][A-Za-z0-9._-]*$') {
            throw "Component id '$id' is unsafe."
        }
        if ([string]::IsNullOrWhiteSpace($version) -or
            $version -notmatch '^[0-9A-Za-z][0-9A-Za-z.+-]*$') {
            throw "Component version '$version' is unsafe."
        }

        $key = "$($id.ToLowerInvariant())|$($version.ToLowerInvariant())"
        if ($byKey.ContainsKey($key)) {
            throw "The component list contains duplicate '$id' '$version'."
        }
        $byKey[$key] = [pscustomobject]@{ Id = $id; Version = $version }
    }

    $keys = [string[]]@($byKey.Keys)
    [Array]::Sort($keys, [StringComparer]::Ordinal)
    return @($keys | ForEach-Object { $byKey[$_] })
}

function Get-PackageDirectory(
    [string]$Id,
    [string]$Version,
    [string]$Root,
    [switch]$AllowMissing
) {
    $current = [IO.Path]::GetFullPath($Root)
    if (((Get-Item -LiteralPath $current -Force).Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
        throw "NuGet package root uses an unsupported reparse point: '$current'."
    }

    foreach ($segment in @($Id.ToLowerInvariant(), $Version.ToLowerInvariant())) {
        $current = Assert-PathInside `
            -Path (Join-Path $current $segment) `
            -Root $Root `
            -Description "Package directory"
        if (-not (Test-Path -LiteralPath $current -PathType Container)) {
            if ($AllowMissing) { return $null }
            throw "NuGet package '$Id' '$Version' is missing from '$Root'."
        }
        if (((Get-Item -LiteralPath $current -Force).Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
            throw "NuGet package '$Id' '$Version' uses an unsupported reparse point at '$current'."
        }
    }
    return $current
}

function Get-PackageFiles([string]$PackageDirectory) {
    $files = [Collections.Generic.List[IO.FileInfo]]::new()
    $pending = [Collections.Generic.Stack[string]]::new()
    $pending.Push($PackageDirectory)
    while ($pending.Count -gt 0) {
        $directory = $pending.Pop()
        foreach ($item in Get-ChildItem -LiteralPath $directory -Force) {
            if (($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
                throw "Package contains an unsupported reparse point: '$($item.FullName)'."
            }
            if ($item.PSIsContainer) {
                $pending.Push($item.FullName)
            }
            elseif ($item.Name -match $evidenceNamePattern) {
                $files.Add($item)
            }
        }
    }
    return @($files)
}

function Assert-EvidenceFile([IO.FileInfo]$File, [string]$PackageDirectory) {
    $fullPath = Assert-PathInside `
        -Path $File.FullName `
        -Root $PackageDirectory `
        -Description "License evidence"
    if (($File.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
        throw "License evidence uses an unsupported reparse point: '$fullPath'."
    }
    if ($File.Length -le 0 -or $File.Length -gt $maximumEvidenceLength) {
        throw "License evidence '$fullPath' must contain between 1 byte and $maximumEvidenceLength bytes."
    }
    return $fullPath
}

function Get-RelativePackagePath([string]$Path, [string]$PackageDirectory) {
    $fullPath = Assert-PathInside -Path $Path -Root $PackageDirectory -Description "Package file"
    return $fullPath.Substring((Get-FullPathPrefix -Path $PackageDirectory).Length).Replace('\', '/')
}

function Get-SafeEvidenceName([string]$Name) {
    $safeName = [Regex]::Replace($Name, '[^A-Za-z0-9._-]', '_')
    if ($safeName.Length -gt 96) {
        $safeName = $safeName.Substring(0, 96)
    }
    if (-not $safeName) {
        return "evidence"
    }
    return $safeName
}

function Copy-BundleFile(
    [string]$SourcePath,
    [string]$DestinationPath,
    [string]$SourceRelativePath,
    [string]$BundleRelativePath
) {
    $destinationParent = Split-Path -Parent $DestinationPath
    New-Item -ItemType Directory -Path $destinationParent -Force | Out-Null
    Copy-Item -LiteralPath $SourcePath -Destination $DestinationPath
    $destination = Get-Item -LiteralPath $DestinationPath
    return [ordered]@{
        sourceRelativePath = $SourceRelativePath
        bundlePath = $BundleRelativePath.Replace('\', '/')
        sha256 = (Get-FileHash -LiteralPath $DestinationPath -Algorithm SHA256).Hash.ToLowerInvariant()
        length = [long]$destination.Length
    }
}

function Get-NuspecMetadata([xml]$Nuspec) {
    $metadata = $Nuspec.SelectSingleNode(
        '/*[local-name()="package"]/*[local-name()="metadata"]'
    )
    if (-not $metadata) {
        throw "Nuspec does not contain package metadata."
    }
    return $metadata
}

function Read-Nuspec([string]$Path) {
    $settings = [Xml.XmlReaderSettings]::new()
    $settings.DtdProcessing = [Xml.DtdProcessing]::Prohibit
    $settings.XmlResolver = $null
    $reader = [Xml.XmlReader]::Create($Path, $settings)
    try {
        $document = [Xml.XmlDocument]::new()
        $document.XmlResolver = $null
        $document.Load($reader)
        return $document
    }
    finally {
        $reader.Dispose()
    }
}

function Get-OptionalXmlText([Xml.XmlNode]$Parent, [string]$LocalName) {
    $node = $Parent.SelectSingleNode("*[local-name()='$LocalName']")
    if (-not $node) {
        return ""
    }
    return [string]$node.InnerText
}

function Get-LicenseNodeType([Xml.XmlNode]$LicenseNode) {
    $licenseTypeAttribute = $LicenseNode.Attributes["type"]
    if (-not $licenseTypeAttribute) {
        return ""
    }
    return [string]$licenseTypeAttribute.Value
}

function New-FileLicenseDeclaration([string]$LicenseValue, [string]$PackageDirectory) {
    if ([IO.Path]::IsPathRooted($LicenseValue)) {
        throw "Nuspec license file path must be relative."
    }

    $licenseFile = Join-Path $PackageDirectory $LicenseValue
    $licenseFile = Assert-PathInside `
        -Path $licenseFile `
        -Root $PackageDirectory `
        -Description "Nuspec license file"
    if (-not (Test-Path -LiteralPath $licenseFile -PathType Leaf)) {
        throw "Nuspec refers to a missing license file '$LicenseValue'."
    }

    return [pscustomobject]@{
        Kind = "file"
        Value = $LicenseValue.Replace('\', '/')
        FilePath = $licenseFile
    }
}

function Get-DeclaredLicenseFromNode(
    [Xml.XmlNode]$LicenseNode,
    [string]$PackageDirectory
) {
    $licenseType = Get-LicenseNodeType -LicenseNode $LicenseNode
    $licenseValue = ([string]$LicenseNode.InnerText).Trim()
    if ([string]::IsNullOrWhiteSpace($licenseValue) -or
        $licenseType -notin @("expression", "file")) {
        throw "Nuspec contains an invalid license declaration."
    }

    if ($licenseType -eq "file") {
        return New-FileLicenseDeclaration `
            -LicenseValue $licenseValue `
            -PackageDirectory $PackageDirectory
    }

    return [pscustomobject]@{
        Kind = "expression"
        Value = $licenseValue
        FilePath = $null
    }
}

function Get-LegacyLicenseUrlDeclaration([Xml.XmlNode]$Metadata) {
    $licenseUrl = (Get-OptionalXmlText -Parent $Metadata -LocalName "licenseUrl").Trim()
    if ([string]::IsNullOrWhiteSpace($licenseUrl)) {
        return $null
    }

    $uri = $null
    if (-not [Uri]::TryCreate($licenseUrl, [UriKind]::Absolute, [ref]$uri) -or
        $uri.Scheme -ne [Uri]::UriSchemeHttps) {
        throw "Nuspec licenseUrl must be an absolute HTTPS URL."
    }

    return [pscustomobject]@{ Kind = "url"; Value = $licenseUrl; FilePath = $null }
}

function Get-DeclaredLicense([xml]$Nuspec, [string]$PackageDirectory) {
    $metadata = Get-NuspecMetadata -Nuspec $Nuspec
    $licenseNode = $metadata.SelectSingleNode('*[local-name()="license"]')
    if ($licenseNode) {
        return Get-DeclaredLicenseFromNode `
            -LicenseNode $licenseNode `
            -PackageDirectory $PackageDirectory
    }

    return Get-LegacyLicenseUrlDeclaration -Metadata $metadata
}

function New-ExternallyHandledComponentEntry([object]$Component) {
    return [ordered]@{
        id = [string]$Component.Id
        version = [string]$Component.Version
        externallyHandled = $true
        externalHandler = "dotnet-runtime-host-notices"
        declaredLicense = $null
        copyright = ""
        nuspec = $null
        evidence = @()
    }
}

function Get-ValidatedComponentNuspec([object]$Component, [string]$PackageDirectory) {
    $nuspecFiles = @(Get-ChildItem -LiteralPath $PackageDirectory -Filter "*.nuspec" -File -Force)
    if ($nuspecFiles.Count -ne 1) {
        throw "NuGet package '$($Component.Id)' '$($Component.Version)' must contain exactly one root nuspec."
    }

    $nuspecPath = Assert-EvidenceFile -File $nuspecFiles[0] -PackageDirectory $PackageDirectory
    [xml]$nuspec = Read-Nuspec -Path $nuspecPath
    $metadata = Get-NuspecMetadata -Nuspec $nuspec
    $metadataId = Get-OptionalXmlText -Parent $metadata -LocalName "id"
    $metadataVersion = Get-OptionalXmlText -Parent $metadata -LocalName "version"
    if (-not [StringComparer]::OrdinalIgnoreCase.Equals($metadataId, [string]$Component.Id) -or
        -not [StringComparer]::OrdinalIgnoreCase.Equals($metadataVersion, [string]$Component.Version)) {
        throw "Nuspec identity '$metadataId' '$metadataVersion' differs from component '$($Component.Id)' '$($Component.Version)'."
    }

    return [pscustomobject]@{
        Path = $nuspecPath
        Document = $nuspec
        Metadata = $metadata
    }
}

function Get-ComponentEvidenceFiles(
    [object]$DeclaredLicense,
    [string]$PackageDirectory,
    [object]$Component
) {
    $evidenceFiles = [Collections.Generic.Dictionary[string, IO.FileInfo]]::new(
        [StringComparer]::Ordinal
    )
    foreach ($file in Get-PackageFiles -PackageDirectory $PackageDirectory) {
        $evidenceFiles[$file.FullName] = $file
    }

    if ($DeclaredLicense -and $DeclaredLicense.FilePath) {
        $licenseFile = Get-Item -LiteralPath $DeclaredLicense.FilePath -Force
        if (($licenseFile.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
            throw "Nuspec license file uses an unsupported reparse point."
        }
        $evidenceFiles[$licenseFile.FullName] = $licenseFile
    }

    if ($evidenceFiles.Count -gt $maximumEvidenceFileCount) {
        throw "NuGet package '$($Component.Id)' '$($Component.Version)' exceeds the limit of $maximumEvidenceFileCount license evidence files."
    }

    return ,$evidenceFiles
}

function Copy-ComponentEvidence(
    [Collections.Generic.Dictionary[string, IO.FileInfo]]$EvidenceFiles,
    [string]$PackageDirectory,
    [string]$BundleRoot,
    [string]$ComponentDirectoryName
) {
    $evidenceByRelativePath = [Collections.Generic.Dictionary[string, string]]::new(
        [StringComparer]::Ordinal
    )
    foreach ($file in $EvidenceFiles.Values) {
        $validatedPath = Assert-EvidenceFile -File $file -PackageDirectory $PackageDirectory
        $relativePath = Get-RelativePackagePath $validatedPath $PackageDirectory
        $evidenceByRelativePath[$relativePath] = $validatedPath
    }

    $relativePaths = [string[]]@($evidenceByRelativePath.Keys)
    [Array]::Sort($relativePaths, [StringComparer]::Ordinal)
    return @(
        for ($index = 0; $index -lt $relativePaths.Count; $index++) {
            $relativePath = $relativePaths[$index]
            $safeName = Get-SafeEvidenceName -Name ([IO.Path]::GetFileName($relativePath))
            $bundlePath = "components/$ComponentDirectoryName/evidence/{0:D4}-$safeName" -f ($index + 1)
            Copy-BundleFile `
                -SourcePath $evidenceByRelativePath[$relativePath] `
                -DestinationPath (Join-Path $BundleRoot $bundlePath) `
                -SourceRelativePath $relativePath `
                -BundleRelativePath $bundlePath
        }
    )
}

function Get-ComponentLicenseEntry([object]$DeclaredLicense, [bool]$ExternallyHandled) {
    if ($DeclaredLicense) {
        return [ordered]@{ kind = $DeclaredLicense.Kind; value = $DeclaredLicense.Value }
    }
    if ($ExternallyHandled) {
        return $null
    }
    return [ordered]@{ kind = "discovered-files"; value = $null }
}

function New-ComponentBundle(
    [object]$Component,
    [string]$NuGetRoot,
    [string]$BundleRoot
) {
    $externallyHandled = [string]$Component.Id -cmatch $externalComponentPattern
    $packageDirectory = Get-PackageDirectory `
        -Id $Component.Id `
        -Version $Component.Version `
        -Root $NuGetRoot `
        -AllowMissing:$externallyHandled
    if (-not $packageDirectory) {
        return New-ExternallyHandledComponentEntry -Component $Component
    }

    $packageNuspec = Get-ValidatedComponentNuspec `
        -Component $Component `
        -PackageDirectory $packageDirectory
    $declaredLicense = Get-DeclaredLicense `
        -Nuspec $packageNuspec.Document `
        -PackageDirectory $packageDirectory
    $evidenceFiles = Get-ComponentEvidenceFiles `
        -DeclaredLicense $declaredLicense `
        -PackageDirectory $packageDirectory `
        -Component $Component

    if (-not $externallyHandled -and -not $declaredLicense -and $evidenceFiles.Count -eq 0) {
        throw "NuGet package '$($Component.Id)' '$($Component.Version)' does not provide license evidence."
    }

    $componentDirectoryName = "$($Component.Id.ToLowerInvariant())/$($Component.Version.ToLowerInvariant())"
    $componentOutput = Join-Path $BundleRoot "components/$componentDirectoryName"
    $nuspecBundlePath = "components/$componentDirectoryName/package.nuspec"
    $nuspecEntry = Copy-BundleFile `
        -SourcePath $packageNuspec.Path `
        -DestinationPath (Join-Path $BundleRoot $nuspecBundlePath) `
        -SourceRelativePath (Get-RelativePackagePath $packageNuspec.Path $packageDirectory) `
        -BundleRelativePath $nuspecBundlePath

    $evidenceEntries = @(
        Copy-ComponentEvidence `
            -EvidenceFiles $evidenceFiles `
            -PackageDirectory $packageDirectory `
            -BundleRoot $BundleRoot `
            -ComponentDirectoryName $componentDirectoryName
    )

    $licenseEntry = Get-ComponentLicenseEntry `
        -DeclaredLicense $declaredLicense `
        -ExternallyHandled $externallyHandled

    return [ordered]@{
        id = [string]$Component.Id
        version = [string]$Component.Version
        externallyHandled = $externallyHandled
        externalHandler = if ($externallyHandled) { "dotnet-runtime-host-notices" } else { $null }
        declaredLicense = $licenseEntry
        copyright = Get-OptionalXmlText -Parent $packageNuspec.Metadata -LocalName "copyright"
        nuspec = $nuspecEntry
        evidence = $evidenceEntries
    }
}

if (-not $PackageRoot) {
    $PackageRoot = if ($env:NUGET_PACKAGES) {
        $env:NUGET_PACKAGES
    }
    else {
        Join-Path ([Environment]::GetFolderPath([Environment+SpecialFolder]::UserProfile)) ".nuget/packages"
    }
}

$PackageRoot = (Resolve-Path -LiteralPath $PackageRoot).Path
if (-not (Test-Path -LiteralPath $PackageRoot -PathType Container)) {
    throw "NuGet package root is not a directory: '$PackageRoot'."
}
$components = @(Get-NormalizedComponents -Components @(Get-InputComponents))
$fullOutput = [IO.Path]::GetFullPath($OutputDirectory)
if (Test-Path -LiteralPath $fullOutput) {
    throw "Output directory already exists: '$fullOutput'."
}
if ($fullOutput.StartsWith((Get-FullPathPrefix -Path $PackageRoot), $pathComparison)) {
    throw "Output directory must not be inside the NuGet package cache."
}
$outputParent = Split-Path -Parent $fullOutput
if (-not (Test-Path -LiteralPath $outputParent -PathType Container)) {
    throw "Output parent directory does not exist: '$outputParent'."
}

$temporaryDirectory = "$fullOutput.tmp-$([Guid]::NewGuid().ToString('N'))"
New-Item -ItemType Directory -Path $temporaryDirectory | Out-Null
try {
    $componentEntries = @(
        foreach ($component in $components) {
            New-ComponentBundle `
                -Component $component `
                -NuGetRoot $PackageRoot `
                -BundleRoot $temporaryDirectory
        }
    )
    $manifest = [ordered]@{
        schemaVersion = 1
        generatedFrom = $inputKind
        runtimeIdentifier = if ($PSCmdlet.ParameterSetName -eq "Published") {
            $RuntimeIdentifier
        }
        else {
            $null
        }
        componentCount = $componentEntries.Count
        components = $componentEntries
    }
    $manifestJson = ($manifest | ConvertTo-Json -Depth 10).Replace("`r`n", "`n") + "`n"
    [IO.File]::WriteAllText(
        (Join-Path $temporaryDirectory "manifest.json"),
        $manifestJson,
        [Text.UTF8Encoding]::new($false)
    )
    Move-Item -LiteralPath $temporaryDirectory -Destination $fullOutput
}
finally {
    if (Test-Path -LiteralPath $temporaryDirectory) {
        Remove-Item -LiteralPath $temporaryDirectory -Recurse -Force
    }
}

Write-Host "NuGet license bundle contains $($components.Count) components: '$fullOutput'."
