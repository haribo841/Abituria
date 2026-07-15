Set-StrictMode -Version Latest

function Get-RepositoryRoot {
    $toolsDirectory = Split-Path -Parent $PSScriptRoot
    return (Resolve-Path (Split-Path -Parent $toolsDirectory)).Path
}

function Get-AbituriaVersion {
    param(
        [string]$RepositoryRoot = (Get-RepositoryRoot)
    )

    $propertiesPath = Join-Path $RepositoryRoot "Directory.Build.props"
    if (-not (Test-Path -LiteralPath $propertiesPath -PathType Leaf)) {
        throw "Version source not found: $propertiesPath"
    }

    [xml]$properties = Get-Content -LiteralPath $propertiesPath -Raw -Encoding UTF8
    $versions = @(
        @(
            $properties.Project.PropertyGroup.Version |
                ForEach-Object { $_.ToString().Trim() } |
                Where-Object { $_ }
        ) | Select-Object -Unique
    )

    if ($versions.Count -ne 1) {
        throw "Directory.Build.props must contain exactly one Version property."
    }

    $version = $versions[0]
    if ($version -notmatch '^\d+\.\d+\.\d+(?:-[0-9A-Za-z]+(?:[.-][0-9A-Za-z]+)*)?$') {
        throw "Version '$version' is not a supported SemVer version."
    }

    return $version
}

function Assert-ReleaseTag {
    param(
        [Parameter(Mandatory)]
        [string]$Tag,

        [string]$RepositoryRoot = (Get-RepositoryRoot)
    )

    $version = Get-AbituriaVersion -RepositoryRoot $RepositoryRoot
    $expectedTag = "v$version"
    if ($Tag -cne $expectedTag) {
        throw "Tag '$Tag' nie odpowiada wersji '$version'. Oczekiwano '$expectedTag'."
    }

    return $version
}

function Invoke-ExternalCommand {
    param(
        [Parameter(Mandatory)]
        [string]$FilePath,

        [Parameter()]
        [string[]]$ArgumentList = @()
    )

    & $FilePath @ArgumentList
    if ($LASTEXITCODE -ne 0) {
        throw "Command '$FilePath' failed with exit code $LASTEXITCODE."
    }
}

function Reset-Directory {
    param(
        [Parameter(Mandatory)]
        [string]$Path
    )

    $repositoryRoot = Get-RepositoryRoot
    $artifactsRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot "artifacts"))
    $artifactsPrefix = $artifactsRoot.TrimEnd(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar
    ) + [IO.Path]::DirectorySeparatorChar
    $fullPath = [IO.Path]::GetFullPath($Path)
    $comparison = if ([Runtime.InteropServices.RuntimeInformation]::IsOSPlatform(
        [Runtime.InteropServices.OSPlatform]::Windows
    )) {
        [StringComparison]::OrdinalIgnoreCase
    }
    else {
        [StringComparison]::Ordinal
    }

    if (-not $fullPath.StartsWith($artifactsPrefix, $comparison)) {
        throw "Refusing to reset a directory outside '$artifactsRoot': '$fullPath'."
    }

    if (Test-Path -LiteralPath $fullPath) {
        Remove-Item -LiteralPath $fullPath -Recurse -Force
    }

    New-Item -ItemType Directory -Path $fullPath -Force | Out-Null
}

function Remove-TemporaryDirectory {
    param(
        [Parameter(Mandatory)]
        [string]$Path
    )

    $temporaryRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())
    $temporaryPrefix = $temporaryRoot.TrimEnd(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar
    ) + [IO.Path]::DirectorySeparatorChar
    $fullPath = [IO.Path]::GetFullPath($Path)
    $comparison = if ([Runtime.InteropServices.RuntimeInformation]::IsOSPlatform(
        [Runtime.InteropServices.OSPlatform]::Windows
    )) {
        [StringComparison]::OrdinalIgnoreCase
    }
    else {
        [StringComparison]::Ordinal
    }

    if (-not $fullPath.StartsWith($temporaryPrefix, $comparison)) {
        throw "Refusing to remove a directory outside '$temporaryRoot': '$fullPath'."
    }

    if (Test-Path -LiteralPath $fullPath) {
        Remove-Item -LiteralPath $fullPath -Recurse -Force
    }
}

function Get-ReleaseArchiveName {
    param(
        [Parameter(Mandatory)]
        [string]$Version,

        [Parameter(Mandatory)]
        [ValidateSet("win-x64", "linux-x64", "osx-x64")]
        [string]$RuntimeIdentifier
    )

    $extension = if ($RuntimeIdentifier -eq "linux-x64") { "tar.gz" } else { "zip" }
    return "Abituria-v$Version-$RuntimeIdentifier.$extension"
}

function Get-ReleaseSbomName {
    param(
        [Parameter(Mandatory)]
        [string]$Version,

        [Parameter(Mandatory)]
        [ValidateSet("win-x64", "linux-x64", "osx-x64")]
        [string]$RuntimeIdentifier
    )

    return "Abituria-v$Version-$RuntimeIdentifier.spdx.json"
}

function Assert-ReleasePackageScope {
    param(
        [Parameter(Mandatory)]
        [object[]]$Components,

        [Parameter(Mandatory)]
        [ValidateSet("win-x64", "linux-x64", "osx-x64")]
        [string]$RuntimeIdentifier
    )

    $testPackagePattern = '(?i)(?:^|[.])(?:xunit|coverlet)(?:[.]|$)|^Microsoft[.]NET[.]Test[.]Sdk$|^Microsoft[.](?:TestPlatform|Testing)(?:[.]|$)|^Microsoft[.]CodeCoverage$|^Avalonia[.]Headless(?:[.]XUnit)?$'
    $foreignRuntimePattern = switch ($RuntimeIdentifier) {
        "win-x64" {
            '(?i)NativeAssets[.](?:Linux|macOS|WebAssembly)(?:[.]|$)|(?:Runtime|Host)[.](?:linux|osx)-x64$'
        }
        "linux-x64" {
            '(?i)NativeAssets[.](?:Win32|macOS|WebAssembly)(?:[.]|$)|Windows[.]Natives(?:[.]|$)|(?:Runtime|Host)[.](?:win|osx)-x64$'
        }
        "osx-x64" {
            '(?i)NativeAssets[.](?:Win32|Linux|WebAssembly)(?:[.]|$)|Windows[.]Natives(?:[.]|$)|(?:Runtime|Host)[.](?:win|linux)-x64$'
        }
    }

    $testPackages = @($Components | Where-Object { $_.Id -match $testPackagePattern })
    if ($testPackages.Count -ne 0) {
        throw "SBOM for '$RuntimeIdentifier' contains test packages: $($testPackages.Id -join ', ')."
    }

    $foreignRuntimePackages = @($Components | Where-Object { $_.Id -match $foreignRuntimePattern })
    if ($foreignRuntimePackages.Count -ne 0) {
        throw "SBOM for '$RuntimeIdentifier' contains packages for another runtime: $($foreignRuntimePackages.Id -join ', ')."
    }

    $expectedRuntimePack = "Microsoft.NETCore.App.Runtime.$RuntimeIdentifier"
    $runtimePacks = @($Components | Where-Object { $_.Id -match '^Microsoft[.]NETCore[.]App[.]Runtime[.]' })
    if ($runtimePacks.Count -ne 1 -or $runtimePacks[0].Id -cne $expectedRuntimePack) {
        throw "SBOM for '$RuntimeIdentifier' must contain exactly the runtime pack '$expectedRuntimePack'."
    }

    $expectedHostPack = "Microsoft.NETCore.App.Host.$RuntimeIdentifier"
    $hostPacks = @($Components | Where-Object { $_.Id -match '^Microsoft[.]NETCore[.]App[.]Host[.]' })
    if ($hostPacks.Count -ne 1 -or $hostPacks[0].Id -cne $expectedHostPack) {
        throw "SBOM for '$RuntimeIdentifier' must contain exactly the host pack '$expectedHostPack'."
    }
}

function Get-PublishedNuGetComponents {
    param(
        [Parameter(Mandatory)]
        [string]$PublishedDirectory,

        [Parameter(Mandatory)]
        [ValidateSet("win-x64", "linux-x64", "osx-x64")]
        [string]$RuntimeIdentifier
    )

    $dependenciesPath = Join-Path $PublishedDirectory "Abituria.deps.json"
    if (-not (Test-Path -LiteralPath $dependenciesPath -PathType Leaf)) {
        throw "Published dependency manifest is missing: '$dependenciesPath'."
    }

    $dependencies = Get-Content -LiteralPath $dependenciesPath -Raw -Encoding UTF8 | ConvertFrom-Json
    $expectedRuntimeTarget = ".NETCoreApp,Version=v10.0/$RuntimeIdentifier"
    if ($dependencies.runtimeTarget.name -cne $expectedRuntimeTarget) {
        throw "Published dependency manifest targets '$($dependencies.runtimeTarget.name)' instead of '$expectedRuntimeTarget'."
    }

    $components = @(
        foreach ($library in $dependencies.libraries.PSObject.Properties) {
            $separatorIndex = $library.Name.LastIndexOf('/')
            if ($separatorIndex -le 0 -or $separatorIndex -eq $library.Name.Length - 1) {
                throw "Published dependency key '$($library.Name)' is invalid."
            }

            $id = $library.Name.Substring(0, $separatorIndex)
            $version = $library.Name.Substring($separatorIndex + 1)
            $type = [string]$library.Value.type
            if ($type -ceq "project") {
                continue
            }
            if ($type -ceq "runtimepack") {
                if (-not $id.StartsWith("runtimepack.", [StringComparison]::Ordinal)) {
                    throw "Runtime pack '$id' does not use the expected prefix."
                }
                $id = $id.Substring("runtimepack.".Length)
            }
            elseif ($type -cne "package") {
                throw "Published dependency '$id' has unsupported type '$type'."
            }

            [pscustomobject]@{
                Id = $id
                Version = $version
            }
        }
    )

    if ($components.Count -eq 0) {
        throw "Published dependency manifest does not contain NuGet components."
    }

    $expectedRuntimePack = "Microsoft.NETCore.App.Runtime.$RuntimeIdentifier"
    $runtimePack = @($components | Where-Object { $_.Id -ceq $expectedRuntimePack })
    if ($runtimePack.Count -ne 1) {
        throw "Published dependency manifest must contain exactly the runtime pack '$expectedRuntimePack'."
    }
    $components += [pscustomobject]@{
        Id = "Microsoft.NETCore.App.Host.$RuntimeIdentifier"
        Version = $runtimePack[0].Version
    }

    $duplicateComponents = @(
        $components |
            Group-Object { "$($_.Id.ToLowerInvariant())|$($_.Version)" } |
            Where-Object Count -gt 1
    )
    if ($duplicateComponents.Count -ne 0) {
        throw "Published dependency manifest contains duplicate components."
    }

    Assert-ReleasePackageScope -Components $components -RuntimeIdentifier $RuntimeIdentifier
    return @($components | Sort-Object Id, Version)
}

function New-ReleaseNuGetComponentManifest {
    param(
        [Parameter(Mandatory)]
        [string]$PublishedDirectory,

        [Parameter(Mandatory)]
        [ValidateSet("win-x64", "linux-x64", "osx-x64")]
        [string]$RuntimeIdentifier,

        [Parameter(Mandatory)]
        [string]$OutputDirectory
    )

    $components = Get-PublishedNuGetComponents `
        -PublishedDirectory $PublishedDirectory `
        -RuntimeIdentifier $RuntimeIdentifier
    New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
    $manifestPath = Join-Path $OutputDirectory "packages.config"
    $settings = [Xml.XmlWriterSettings]::new()
    $settings.Encoding = [Text.UTF8Encoding]::new($false)
    $settings.Indent = $true

    $writer = [Xml.XmlWriter]::Create($manifestPath, $settings)
    try {
        $writer.WriteStartDocument()
        $writer.WriteStartElement("packages")
        foreach ($component in $components) {
            $writer.WriteStartElement("package")
            $writer.WriteAttributeString("id", $component.Id)
            $writer.WriteAttributeString("version", $component.Version)
            $writer.WriteAttributeString("targetFramework", "net10.0")
            $writer.WriteEndElement()
        }
        $writer.WriteEndElement()
        $writer.WriteEndDocument()
    }
    finally {
        $writer.Dispose()
    }

    return $manifestPath
}

function Assert-ReleaseSbomScope {
    param(
        [Parameter(Mandatory)]
        [string]$SbomPath,

        [Parameter(Mandatory)]
        [string]$PublishedDirectory,

        [Parameter(Mandatory)]
        [ValidateSet("win-x64", "linux-x64", "osx-x64")]
        [string]$RuntimeIdentifier,

        [Parameter(Mandatory)]
        [string]$Version
    )

    $expectedComponents = Get-PublishedNuGetComponents `
        -PublishedDirectory $PublishedDirectory `
        -RuntimeIdentifier $RuntimeIdentifier
    $sbom = Get-Content -LiteralPath $SbomPath -Raw -Encoding UTF8 | ConvertFrom-Json
    $productName = "Abituria-$RuntimeIdentifier"
    $productPackages = @(
        $sbom.packages | Where-Object { $_.name -ceq $productName -and $_.versionInfo -ceq $Version }
    )
    if ($productPackages.Count -ne 1) {
        throw "SBOM must contain exactly one '$productName' product package at version '$Version'."
    }

    $actualComponents = @(
        $sbom.packages |
            Where-Object { $_.name -cne $productName } |
            ForEach-Object {
                [pscustomobject]@{
                    Id = [string]$_.name
                    Version = [string]$_.versionInfo
                }
            }
    )
    Assert-ReleasePackageScope -Components $actualComponents -RuntimeIdentifier $RuntimeIdentifier

    $expectedKeys = @(
        $expectedComponents |
            ForEach-Object { "$($_.Id.ToLowerInvariant())|$($_.Version)" } |
            Sort-Object
    )
    $actualKeys = @(
        $actualComponents |
            ForEach-Object { "$($_.Id.ToLowerInvariant())|$($_.Version)" } |
            Sort-Object
    )
    if (($expectedKeys -join "`n") -cne ($actualKeys -join "`n")) {
        $missing = @(Compare-Object $actualKeys $expectedKeys | Where-Object SideIndicator -eq '=>' | ForEach-Object InputObject)
        $unexpected = @(Compare-Object $actualKeys $expectedKeys | Where-Object SideIndicator -eq '<=' | ForEach-Object InputObject)
        throw "SBOM package set differs from the published application. Missing: $($missing -join ', '). Unexpected: $($unexpected -join ', ')."
    }
}

Export-ModuleMember -Function @(
    "Get-RepositoryRoot",
    "Get-AbituriaVersion",
    "Assert-ReleaseTag",
    "Invoke-ExternalCommand",
    "Reset-Directory",
    "Remove-TemporaryDirectory",
    "Get-ReleaseArchiveName",
    "Get-ReleaseSbomName",
    "Assert-ReleasePackageScope",
    "Get-PublishedNuGetComponents",
    "New-ReleaseNuGetComponentManifest",
    "Assert-ReleaseSbomScope"
)
