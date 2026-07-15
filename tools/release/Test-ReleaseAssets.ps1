[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$ArtifactsDirectory,

    [Parameter(Mandatory)]
    [string]$Version,

    [Parameter(Mandatory)]
    [string]$Tag
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Import-Module (Join-Path $PSScriptRoot "Release.Common.psm1") -Force
Import-Module (Join-Path $PSScriptRoot "PackageSecurity.psm1") -Force

$repositoryRoot = Get-RepositoryRoot
$canonicalVersion = Assert-ReleaseTag -Tag $Tag -RepositoryRoot $repositoryRoot
if ($Version -cne $canonicalVersion) {
    throw "Walidowana wersja '$Version' nie odpowiada wersji '$canonicalVersion'."
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
            "licenses/dotnet-runtime-THIRD-PARTY-NOTICES.txt"
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
        if ($releaseMetadata.commit -notmatch '^(?:[0-9a-f]{40}|unknown)$') {
            throw "Metadata in '$archiveName' contains an invalid commit."
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
