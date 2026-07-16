[CmdletBinding(DefaultParameterSetName = "Release")]
param(
    [Parameter(Mandatory, ParameterSetName = "Release")]
    [string]$ArtifactsDirectory,

    [Parameter(Mandatory, ParameterSetName = "Release")]
    [string]$Version,

    [Parameter(Mandatory, ParameterSetName = "Release")]
    [string]$Tag,

    [Parameter(Mandatory, ParameterSetName = "LicenseBundle")]
    [string]$LicensePackageDirectory,

    [Parameter(Mandatory, ParameterSetName = "LicenseBundle")]
    [string]$LicensePublishedPayloadDirectory,

    [Parameter(Mandatory, ParameterSetName = "LicenseBundle")]
    [ValidateSet("win-x64", "linux-x64", "osx-x64")]
    [string]$LicenseRuntimeIdentifier
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Import-Module (Join-Path $PSScriptRoot "Release.Common.psm1") -Force
Import-Module (Join-Path $PSScriptRoot "PackageSecurity.psm1") -Force

function Assert-SafeArchiveEntries {
    param(
        [Parameter(Mandatory)]
        [string]$ArchivePath
    )

    $entries = if ($ArchivePath.EndsWith(".tar.gz", [StringComparison]::Ordinal)) {
        $listed = @(& tar -tzf $ArchivePath)
        if ($LASTEXITCODE -ne 0) { throw "Could not inspect archive '$ArchivePath'." }
        $listed
    }
    else {
        Add-Type -AssemblyName System.IO.Compression.FileSystem
        $zip = [IO.Compression.ZipFile]::OpenRead($ArchivePath)
        try { @($zip.Entries | ForEach-Object FullName) }
        finally { $zip.Dispose() }
    }

    $seen = @{}
    foreach ($entry in $entries) {
        $normalized = ([string]$entry).Replace('\', '/')
        if (-not $normalized -or $normalized.StartsWith('/', [StringComparison]::Ordinal) -or
            $normalized -match '^[A-Za-z]:' -or
            @($normalized.Split('/') | Where-Object { $_ -eq '..' }).Count -ne 0) {
            throw "Release archive contains an unsafe entry: '$entry'."
        }
        $identity = $normalized.TrimEnd('/').ToLowerInvariant()
        if ($identity -and $seen.ContainsKey($identity)) {
            throw "Release archive contains a duplicate entry: '$entry'."
        }
        if ($identity) { $seen[$identity] = $true }
    }
}

function Assert-NuGetLicenseBundle {
    param(
        [Parameter(Mandatory)]
        [string]$PackageDirectory,

        [Parameter(Mandatory)]
        [string]$PublishedPayloadDirectory,

        [Parameter(Mandatory)]
        [string]$RuntimeIdentifier
    )

    $bundleRoot = Join-Path $PackageDirectory "licenses/nuget"
    $manifestPath = Join-Path $bundleRoot "manifest.json"
    if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf)) {
        throw "NuGet license evidence manifest is missing for '$RuntimeIdentifier'."
    }
    $manifest = Get-Content -LiteralPath $manifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
    $components = @($manifest.components)
    if ($manifest.schemaVersion -ne 1 -or $manifest.generatedFrom -cne "published-deps" -or
        $manifest.runtimeIdentifier -cne $RuntimeIdentifier -or
        $manifest.componentCount -ne $components.Count) {
        throw "NuGet license evidence manifest has an invalid contract for '$RuntimeIdentifier'."
    }

    $expectedKeys = @(
        Get-PublishedNuGetComponents `
            -PublishedDirectory $PublishedPayloadDirectory `
            -RuntimeIdentifier $RuntimeIdentifier |
            ForEach-Object { "$($_.Id.ToLowerInvariant())|$($_.Version.ToLowerInvariant())" } |
            Sort-Object
    )
    $actualKeys = @(
        $components |
            ForEach-Object {
                "$(([string]$_.id).ToLowerInvariant())|$(([string]$_.version).ToLowerInvariant())"
            } |
            Sort-Object
    )
    if (($expectedKeys -join "`n") -cne ($actualKeys -join "`n")) {
        throw "NuGet license evidence components differ from the published dependency graph for '$RuntimeIdentifier'."
    }

    $bundlePrefix = [IO.Path]::GetFullPath($bundleRoot).TrimEnd(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar
    ) + [IO.Path]::DirectorySeparatorChar
    $listedFiles = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    $null = $listedFiles.Add("manifest.json")
    foreach ($component in $components) {
        $externalPattern = "^Microsoft[.]NETCore[.]App[.](?:Runtime|Host)[.]$([regex]::Escape($RuntimeIdentifier))$"
        if ($component.externallyHandled) {
            if ([string]$component.id -notmatch $externalPattern -or
                $component.externalHandler -cne "dotnet-runtime-host-notices") {
                throw "Component '$($component.id)' uses an invalid external license handler."
            }
        }
        elseif (-not $component.declaredLicense -and @($component.evidence).Count -eq 0) {
            throw "Component '$($component.id)' has no license declaration or preserved evidence."
        }

        $componentFiles = @($component.evidence)
        if ($null -ne $component.nuspec) {
            $componentFiles = @($component.nuspec) + $componentFiles
        }
        elseif (-not $component.externallyHandled) {
            throw "Component '$($component.id)' has no preserved nuspec."
        }

        foreach ($fileEntry in $componentFiles) {
            $bundlePath = [string]$fileEntry.bundlePath
            if (-not $bundlePath -or [IO.Path]::IsPathRooted($bundlePath) -or
                @($bundlePath.Replace('\', '/').Split('/') | Where-Object { $_ -eq '..' }).Count -ne 0) {
                throw "NuGet license manifest contains an unsafe path '$bundlePath'."
            }
            if (-not $listedFiles.Add($bundlePath.Replace('\', '/'))) {
                throw "NuGet license manifest contains a duplicate path '$bundlePath'."
            }
            $fullPath = [IO.Path]::GetFullPath((Join-Path $bundleRoot $bundlePath))
            if (-not $fullPath.StartsWith($bundlePrefix, [StringComparison]::OrdinalIgnoreCase) -or
                -not (Test-Path -LiteralPath $fullPath -PathType Leaf)) {
                throw "NuGet license evidence file is missing or outside the bundle: '$bundlePath'."
            }
            $file = Get-Item -LiteralPath $fullPath
            $hash = (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash.ToLowerInvariant()
            if ($file.Length -ne [long]$fileEntry.length -or $hash -cne [string]$fileEntry.sha256) {
                throw "NuGet license evidence hash or length differs for '$bundlePath'."
            }
        }
    }

    $actualFiles = @(
        Get-ChildItem -LiteralPath $bundleRoot -File -Recurse |
            ForEach-Object { $_.FullName.Substring($bundlePrefix.Length).Replace('\', '/') } |
            Sort-Object
    )
    $expectedFiles = @($listedFiles | Sort-Object)
    if (($actualFiles -join "`n") -cne ($expectedFiles -join "`n")) {
        throw "NuGet license bundle contains unlisted or missing files for '$RuntimeIdentifier'."
    }
}

if ($PSCmdlet.ParameterSetName -eq "LicenseBundle") {
    Assert-NuGetLicenseBundle `
        -PackageDirectory $LicensePackageDirectory `
        -PublishedPayloadDirectory $LicensePublishedPayloadDirectory `
        -RuntimeIdentifier $LicenseRuntimeIdentifier
    Write-Host "NuGet license bundle is complete and matches the published dependency graph."
    return
}

$repositoryRoot = Get-RepositoryRoot
$canonicalVersion = Assert-ReleaseTag -Tag $Tag -RepositoryRoot $repositoryRoot
if ($Version -cne $canonicalVersion) {
    throw "Walidowana wersja '$Version' nie odpowiada wersji '$canonicalVersion'."
}
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    throw "Git is required to validate release source identity."
}
Push-Location $repositoryRoot
try {
    $expectedCommit = (& git rev-parse --verify "refs/tags/$Tag^{commit}" 2>$null).Trim()
    if ($LASTEXITCODE -ne 0 -or $expectedCommit -notmatch '^[0-9a-f]{40}$') {
        throw "Could not resolve the source commit for tag '$Tag'."
    }
}
finally {
    Pop-Location
}

$artifactsDirectory = (Resolve-Path -LiteralPath $ArtifactsDirectory).Path
$runtimeIdentifiers = @("win-x64", "linux-x64", "osx-x64")
$expectedPayloadNames = @(
    foreach ($runtimeIdentifier in $runtimeIdentifiers) {
        Get-ReleaseArchiveName -Version $Version -RuntimeIdentifier $runtimeIdentifier
        Get-ReleaseSbomName -Version $Version -RuntimeIdentifier $runtimeIdentifier
    }
)
$expectedNames = @($expectedPayloadNames + "SHA256SUMS.txt") | Sort-Object
$actualNames = @(Get-ChildItem -LiteralPath $artifactsDirectory -File | ForEach-Object Name | Sort-Object)

if (($expectedNames -join "`n") -cne ($actualNames -join "`n")) {
    throw "Invalid release asset set.`nExpected:`n$($expectedNames -join "`n")`nActual:`n$($actualNames -join "`n")"
}

$checksumsPath = Join-Path $artifactsDirectory "SHA256SUMS.txt"
$checksumEntries = @{}
foreach ($line in Get-Content -LiteralPath $checksumsPath -Encoding UTF8) {
    if ($line -notmatch '^([0-9a-f]{64})  ([^\\/]+)$') {
        throw "Niepoprawny wiersz SHA256SUMS.txt: '$line'."
    }
    if ($checksumEntries.ContainsKey($Matches[2])) {
        throw "Duplicate file in SHA256SUMS.txt: '$($Matches[2])'."
    }
    $checksumEntries[$Matches[2]] = $Matches[1]
}

if ($checksumEntries.Count -ne $expectedPayloadNames.Count) {
    throw "SHA256SUMS.txt must describe all release payloads exactly once."
}

foreach ($name in $expectedPayloadNames) {
    if (-not $checksumEntries.ContainsKey($name)) {
        throw "SHA256SUMS.txt nie zawiera pliku '$name'."
    }
    $actualHash = (Get-FileHash -LiteralPath (Join-Path $artifactsDirectory $name) -Algorithm SHA256).Hash.ToLowerInvariant()
    if ($checksumEntries[$name] -cne $actualHash) {
        throw "Suma SHA-256 pliku '$name' jest niepoprawna."
    }
}

$temporaryRoot = Join-Path ([IO.Path]::GetTempPath()) ("abituria-release-validation-" + [guid]::NewGuid().ToString("N"))
try {
    New-Item -ItemType Directory -Path $temporaryRoot -Force | Out-Null

    foreach ($runtimeIdentifier in $runtimeIdentifiers) {
        $archiveName = Get-ReleaseArchiveName -Version $Version -RuntimeIdentifier $runtimeIdentifier
        $archivePath = Join-Path $artifactsDirectory $archiveName
        $extractionDirectory = Join-Path $temporaryRoot $runtimeIdentifier
        New-Item -ItemType Directory -Path $extractionDirectory -Force | Out-Null
        Assert-SafeArchiveEntries -ArchivePath $archivePath

        if ($archiveName.EndsWith(".tar.gz", [StringComparison]::Ordinal)) {
            Invoke-ExternalCommand -FilePath "tar" -ArgumentList @("-xzf", $archivePath, "-C", $extractionDirectory)
        }
        elseif (Get-Command unzip -ErrorAction SilentlyContinue) {
            Invoke-ExternalCommand -FilePath "unzip" -ArgumentList @("-q", $archivePath, "-d", $extractionDirectory)
        }
        else {
            Expand-Archive -LiteralPath $archivePath -DestinationPath $extractionDirectory
        }

        $packageName = "Abituria-v$Version-$runtimeIdentifier"
        $packageDirectory = Join-Path $extractionDirectory $packageName
        $topLevelEntries = @(Get-ChildItem -LiteralPath $extractionDirectory -Force)
        if ($topLevelEntries.Count -ne 1 -or -not $topLevelEntries[0].PSIsContainer -or
            $topLevelEntries[0].Name -cne $packageName) {
            throw "Archive '$archiveName' must contain exactly one top-level directory named '$packageName'."
        }
        if (-not (Test-Path -LiteralPath $packageDirectory -PathType Container)) {
            throw "Archive '$archiveName' does not contain root directory '$packageName'."
        }

        foreach ($requiredFile in @(
            "LICENSE",
            "RELEASE-README.md",
            "THIRD-PARTY-NOTICES.md",
            "release.json",
            "licenses/Mulish-OFL.txt",
            "licenses/LICENSE-2022-Ich-Troje.txt",
            "licenses/dotnet-runtime-LICENSE.txt",
            "licenses/dotnet-runtime-THIRD-PARTY-NOTICES.txt",
            "licenses/nuget/manifest.json"
        )) {
            $requiredPath = Join-Path $packageDirectory $requiredFile
            if (-not (Test-Path -LiteralPath $requiredPath -PathType Leaf) -or (Get-Item $requiredPath).Length -eq 0) {
                throw "Archiwum '$archiveName' nie zawiera niepustego pliku '$requiredFile'."
            }
        }

        $releaseMetadata = Get-Content -LiteralPath (Join-Path $packageDirectory "release.json") -Raw -Encoding UTF8 |
            ConvertFrom-Json
        if ($releaseMetadata.version -cne $Version -or $releaseMetadata.runtimeIdentifier -cne $runtimeIdentifier) {
            throw "Metadata in '$archiveName' does not match the version or RID."
        }
        if ($releaseMetadata.commit -cne $expectedCommit) {
            throw "Metadata in '$archiveName' is not bound to tag commit '$expectedCommit'."
        }
        if (-not $releaseMetadata.selfContained -or $releaseMetadata.trimmed -or
            $releaseMetadata.aot -or $releaseMetadata.readyToRun -or $releaseMetadata.singleFile) {
            throw "Metadata in '$archiveName' does not match the publication contract."
        }

        $packagedFiles = @(Get-ChildItem -LiteralPath $packageDirectory -File -Recurse)
        $forbiddenFiles = @(
            $packagedFiles | Where-Object {
                $_.Extension -match '^\.(?:pdb|key|pem|pfx|snk|jks|kdbx|publishsettings|user)$' -or
                $_.Name -match '(?i)^(?:\.env(?:\..*)?|secrets?(?:[._-].*)?|credentials?(?:[._-].*)?|id_(?:rsa|dsa|ecdsa|ed25519)|.*token.*)$' -or
                $_.FullName -match '(?i)[\\/](?:Abituria1|BackUp|Projekt-Inzynierski|snapshot|snapshots)[\\/]'
            }
        )
        if ($forbiddenFiles.Count -ne 0) {
            throw "Archiwum '$archiveName' zawiera niedozwolone pliki: $($forbiddenFiles.FullName -join ', ')"
        }

        Test-PackagedSecrets -PackageDirectory $packageDirectory

        $internalSbomPath = Join-Path $packageDirectory "_manifest/spdx_2.2/manifest.spdx.json"
        $externalSbomName = Get-ReleaseSbomName -Version $Version -RuntimeIdentifier $runtimeIdentifier
        $externalSbomPath = Join-Path $artifactsDirectory $externalSbomName
        if (-not (Test-Path -LiteralPath $internalSbomPath -PathType Leaf)) {
            throw "Archiwum '$archiveName' nie zawiera SBOM SPDX 2.2."
        }
        if ((Get-FileHash $internalSbomPath -Algorithm SHA256).Hash -cne
            (Get-FileHash $externalSbomPath -Algorithm SHA256).Hash) {
            throw "Internal and external SBOM differ for '$runtimeIdentifier'."
        }

        $sbom = Get-Content -LiteralPath $externalSbomPath -Raw -Encoding UTF8 | ConvertFrom-Json
        if ($sbom.spdxVersion -cne "SPDX-2.2" -or -not $sbom.documentNamespace) {
            throw "Plik '$externalSbomName' nie jest kompletnym dokumentem SPDX 2.2."
        }
        if (@($sbom.packages).Count -le 1) {
            throw "File '$externalSbomName' does not contain detected NuGet dependencies."
        }
        $publishedPayloadDirectory = if ($runtimeIdentifier -eq "osx-x64") {
            Join-Path $packageDirectory "Abituria.app/Contents/MacOS"
        }
        else {
            $packageDirectory
        }
        Assert-NuGetLicenseBundle `
            -PackageDirectory $packageDirectory `
            -PublishedPayloadDirectory $publishedPayloadDirectory `
            -RuntimeIdentifier $runtimeIdentifier
        Assert-ReleaseSbomScope `
            -SbomPath $externalSbomPath `
            -PublishedDirectory $publishedPayloadDirectory `
            -RuntimeIdentifier $runtimeIdentifier `
            -Version $Version

        switch ($runtimeIdentifier) {
            "win-x64" {
                if (-not (Test-Path -LiteralPath (Join-Path $packageDirectory "Abituria.exe") -PathType Leaf)) {
                    throw "Paczka win-x64 nie zawiera Abituria.exe."
                }
            }
            "linux-x64" {
                $binaryPath = Join-Path $packageDirectory "Abituria"
                if (-not (Test-Path -LiteralPath $binaryPath -PathType Leaf)) {
                    throw "Paczka linux-x64 nie zawiera Abituria."
                }
                if ($IsLinux -and (([IO.File]::GetUnixFileMode($binaryPath) -band [IO.UnixFileMode]::UserExecute) -eq 0)) {
                    throw "Plik Abituria w paczce linux-x64 nie jest wykonywalny."
                }
            }
            "osx-x64" {
                $appDirectory = Join-Path $packageDirectory "Abituria.app"
                $binaryPath = Join-Path $appDirectory "Contents/MacOS/Abituria"
                $plistPath = Join-Path $appDirectory "Contents/Info.plist"
                $iconPath = Join-Path $appDirectory "Contents/Resources/Abituria.icns"
                foreach ($path in @($binaryPath, $plistPath, $iconPath)) {
                    if (-not (Test-Path -LiteralPath $path -PathType Leaf) -or (Get-Item $path).Length -eq 0) {
                        throw "Paczka osx-x64 nie zawiera wymaganego pliku '$path'."
                    }
                }

                $plist = Get-Content -LiteralPath $plistPath -Raw -Encoding UTF8
                if ($plist -notmatch '<key>CFBundleIdentifier</key>\s*<string>io\.github\.haribo841\.abituria</string>' -or
                    $plist -notmatch ("<key>AbituriaReleaseVersion</key>\s*<string>" + [regex]::Escape($Version) + "</string>")) {
                    throw "Info.plist nie zawiera kanonicznego identyfikatora i wersji."
                }
                if ($IsLinux -and (([IO.File]::GetUnixFileMode($binaryPath) -band [IO.UnixFileMode]::UserExecute) -eq 0)) {
                    throw "The Abituria.app binary did not preserve its executable permission."
                }
            }
        }
    }
}
finally {
    if (Test-Path -LiteralPath $temporaryRoot) {
        Remove-TemporaryDirectory -Path $temporaryRoot
    }
}

Write-Host "All release assets for $Tag are complete and consistent."
