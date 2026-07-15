[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$ArchivePath,

    [Parameter(Mandatory)]
    [ValidateSet("win-x64", "linux-x64", "osx-x64")]
    [string]$RuntimeIdentifier,

    [Parameter(Mandatory)]
    [string]$Version
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Import-Module (Join-Path $PSScriptRoot "Release.Common.psm1") -Force

$canonicalVersion = Get-AbituriaVersion
if ($Version -cne $canonicalVersion) {
    throw "Release smoke test version '$Version' differs from canonical version '$canonicalVersion'."
}

$archive = Get-Item -LiteralPath $ArchivePath -ErrorAction Stop
if ($archive.PSIsContainer -or $archive.Length -eq 0) {
    throw "Release archive '$ArchivePath' is missing or empty."
}

$expectedArchiveName = Get-ReleaseArchiveName -Version $Version -RuntimeIdentifier $RuntimeIdentifier
if ($archive.Name -cne $expectedArchiveName) {
    throw "Release smoke test expected archive '$expectedArchiveName', but received '$($archive.Name)'."
}

function Assert-SafeArchiveEntry {
    param(
        [Parameter(Mandatory)]
        [string]$EntryName
    )

    $normalized = $EntryName.Replace('\', '/')
    if (-not $normalized -or $normalized.StartsWith('/', [StringComparison]::Ordinal) -or
        $normalized -match '^[A-Za-z]:' -or
        @($normalized.Split('/') | Where-Object { $_ -eq '..' }).Count -ne 0) {
        throw "Release archive contains an unsafe entry: '$EntryName'."
    }
}

function Expand-ReleaseArchive {
    param(
        [Parameter(Mandatory)]
        [string]$SourcePath,

        [Parameter(Mandatory)]
        [string]$DestinationPath
    )

    if ($SourcePath.EndsWith(".tar.gz", [StringComparison]::Ordinal)) {
        $entries = @(& tar -tzf $SourcePath)
        if ($LASTEXITCODE -ne 0) {
            throw "Could not inspect release archive '$SourcePath'."
        }
        foreach ($entry in $entries) {
            Assert-SafeArchiveEntry -EntryName $entry
        }
        Invoke-ExternalCommand -FilePath "tar" -ArgumentList @("-xzf", $SourcePath, "-C", $DestinationPath)
        return
    }

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [IO.Compression.ZipFile]::OpenRead($SourcePath)
    try {
        foreach ($entry in $zip.Entries) {
            Assert-SafeArchiveEntry -EntryName $entry.FullName
        }
    }
    finally {
        $zip.Dispose()
    }

    if ($RuntimeIdentifier -eq "osx-x64" -and (Get-Command ditto -ErrorAction SilentlyContinue)) {
        Invoke-ExternalCommand -FilePath "ditto" -ArgumentList @("-x", "-k", $SourcePath, $DestinationPath)
    }
    elseif (Get-Command unzip -ErrorAction SilentlyContinue) {
        Invoke-ExternalCommand -FilePath "unzip" -ArgumentList @("-q", $SourcePath, "-d", $DestinationPath)
    }
    else {
        Expand-Archive -LiteralPath $SourcePath -DestinationPath $DestinationPath
    }
}

$temporaryRoot = Join-Path ([IO.Path]::GetTempPath()) ("abituria-release-smoke-" + [guid]::NewGuid().ToString("N"))
$extractionDirectory = Join-Path $temporaryRoot "extracted"
$dataDirectory = Join-Path $temporaryRoot "data"
try {
    New-Item -ItemType Directory -Path $extractionDirectory, $dataDirectory -Force | Out-Null
    Expand-ReleaseArchive -SourcePath $archive.FullName -DestinationPath $extractionDirectory

    $packageName = "Abituria-v$Version-$RuntimeIdentifier"
    $packageDirectory = Join-Path $extractionDirectory $packageName
    if (-not (Test-Path -LiteralPath $packageDirectory -PathType Container)) {
        throw "Release archive does not contain expected root directory '$packageName'."
    }

    $executableRelativePath = switch ($RuntimeIdentifier) {
        "win-x64" { "Abituria.exe" }
        "linux-x64" { "Abituria" }
        "osx-x64" { "Abituria.app/Contents/MacOS/Abituria" }
    }
    $executablePath = Join-Path $packageDirectory $executableRelativePath
    if (-not (Test-Path -LiteralPath $executablePath -PathType Leaf)) {
        throw "Release package $RuntimeIdentifier does not contain '$executableRelativePath'."
    }

    if ($RuntimeIdentifier -eq "win-x64") {
        $process = Start-Process `
            -FilePath $executablePath `
            -ArgumentList @("--release-smoke-test", "--data-directory", $dataDirectory) `
            -WindowStyle Hidden `
            -Wait `
            -PassThru
        $exitCode = $process.ExitCode
    }
    elseif ($RuntimeIdentifier -eq "linux-x64") {
        if (-not (Get-Command xvfb-run -ErrorAction SilentlyContinue)) {
            throw "xvfb-run is required for the Linux release smoke test."
        }
        & xvfb-run -a $executablePath --release-smoke-test --data-directory $dataDirectory
        $exitCode = $LASTEXITCODE
    }
    else {
        & $executablePath --release-smoke-test --data-directory $dataDirectory
        $exitCode = $LASTEXITCODE
    }
    if ($exitCode -ne 0) {
        throw "Release smoke test for $RuntimeIdentifier failed with exit code $exitCode."
    }

    $databasePath = Join-Path $dataDirectory "abituria-release-smoke.db"
    if (-not (Test-Path -LiteralPath $databasePath -PathType Leaf)) {
        throw "Release smoke test did not create a database in the isolated data directory."
    }
}
finally {
    if (Test-Path -LiteralPath $temporaryRoot) {
        Remove-TemporaryDirectory -Path $temporaryRoot
    }
}

Write-Host "Final archive smoke test succeeded for $RuntimeIdentifier."
