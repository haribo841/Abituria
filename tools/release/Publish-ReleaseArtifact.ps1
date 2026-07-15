[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [ValidateSet("win-x64", "linux-x64", "osx-x64")]
    [string]$RuntimeIdentifier,

    [Parameter(Mandatory)]
    [string]$Version,

    [string]$OutputDirectory
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Import-Module (Join-Path $PSScriptRoot "Release.Common.psm1") -Force

$repositoryRoot = Get-RepositoryRoot
$canonicalVersion = Get-AbituriaVersion -RepositoryRoot $repositoryRoot
if ($Version -cne $canonicalVersion) {
    throw "Requested version '$Version' differs from '$canonicalVersion' in Directory.Build.props."
}

if (-not $OutputDirectory) {
    $OutputDirectory = Join-Path $repositoryRoot "artifacts/release/$RuntimeIdentifier"
}

$requiredFiles = @(
    "LICENSE",
    "docs/legacy/originals/LICENSE-2022-Ich-Troje.txt",
    "fonts/OFL.txt",
    "tools/release/RELEASE-README.md",
    "THIRD-PARTY-NOTICES.md"
)
foreach ($relativePath in $requiredFiles) {
    $path = Join-Path $repositoryRoot $relativePath
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "Release input '$relativePath' is missing."
    }
}

$workDirectory = Join-Path $repositoryRoot "artifacts/release-work/$RuntimeIdentifier"
$publishedDirectory = Join-Path $workDirectory "published"
$stagingDirectory = Join-Path $workDirectory "staging"
$packageName = "Abituria-v$Version-$RuntimeIdentifier"
$packageDirectory = Join-Path $stagingDirectory $packageName

Reset-Directory -Path $workDirectory
New-Item -ItemType Directory -Path $publishedDirectory, $packageDirectory -Force | Out-Null
Reset-Directory -Path $OutputDirectory

$commit = "unknown"
if (Get-Command git -ErrorAction SilentlyContinue) {
    Push-Location $repositoryRoot
    try {
        $candidateCommit = (& git rev-parse HEAD).Trim()
        if ($LASTEXITCODE -eq 0 -and $candidateCommit) {
            $commit = $candidateCommit
        }
    }
    finally {
        Pop-Location
    }
}

$sourceRevisionArgument = if ($commit -eq "unknown") {
    "-p:SourceRevisionId="
}
else {
    "-p:SourceRevisionId=$commit"
}

Push-Location $repositoryRoot
try {
    Invoke-ExternalCommand -FilePath "dotnet" -ArgumentList @(
        "publish",
        "Abituria.csproj",
        "--configuration", "Release",
        "--runtime", $RuntimeIdentifier,
        "--self-contained", "true",
        "--no-restore",
        "--output", $publishedDirectory,
        "-p:Version=$Version",
        $sourceRevisionArgument,
        "-p:PublishTrimmed=false",
        "-p:PublishAot=false",
        "-p:PublishReadyToRun=false",
        "-p:PublishSingleFile=false",
        "-p:DebugSymbols=false",
        "-p:DebugType=None"
    )
}
finally {
    Pop-Location
}

Get-ChildItem -LiteralPath $publishedDirectory -Filter "*.pdb" -File -Recurse |
    Remove-Item -Force
if (Get-ChildItem -LiteralPath $publishedDirectory -Filter "*.pdb" -File -Recurse) {
    throw "Publikacja zawiera pliki PDB."
}

if ($RuntimeIdentifier -eq "osx-x64") {
    & (Join-Path $PSScriptRoot "New-MacAppBundle.ps1") `
        -PublishedDirectory $publishedDirectory `
        -BundleOutputDirectory $packageDirectory `
        -Version $Version | Out-Null
}
else {
    Copy-Item -Path (Join-Path $publishedDirectory "*") -Destination $packageDirectory -Recurse -Force
}

Copy-Item -LiteralPath (Join-Path $repositoryRoot "LICENSE") -Destination $packageDirectory
Copy-Item -LiteralPath (Join-Path $repositoryRoot "tools/release/RELEASE-README.md") `
    -Destination (Join-Path $packageDirectory "RELEASE-README.md")
Copy-Item -LiteralPath (Join-Path $repositoryRoot "THIRD-PARTY-NOTICES.md") -Destination $packageDirectory
$licensesDirectory = Join-Path $packageDirectory "licenses"
New-Item -ItemType Directory -Path $licensesDirectory -Force | Out-Null
Copy-Item -LiteralPath (Join-Path $repositoryRoot "fonts/OFL.txt") `
    -Destination (Join-Path $licensesDirectory "Mulish-OFL.txt")
Copy-Item -LiteralPath (Join-Path $repositoryRoot "docs/legacy/originals/LICENSE-2022-Ich-Troje.txt") `
    -Destination (Join-Path $licensesDirectory "LICENSE-2022-Ich-Troje.txt")

$publishedComponents = Get-PublishedNuGetComponents `
    -PublishedDirectory $publishedDirectory `
    -RuntimeIdentifier $RuntimeIdentifier
$runtimePackId = "Microsoft.NETCore.App.Runtime.$RuntimeIdentifier"
$runtimePack = @($publishedComponents | Where-Object { $_.Id -ceq $runtimePackId })
if ($runtimePack.Count -ne 1) {
    throw "Published application does not identify exactly one '$runtimePackId' component."
}
[xml]$buildProperties = Get-Content `
    -LiteralPath (Join-Path $repositoryRoot "Directory.Build.props") `
    -Raw `
    -Encoding UTF8
$runtimeFrameworkVersion = [string]$buildProperties.Project.PropertyGroup.RuntimeFrameworkVersion
if ($runtimePack[0].Version -cne $runtimeFrameworkVersion) {
    throw "Published runtime version '$($runtimePack[0].Version)' differs from '$runtimeFrameworkVersion'."
}
$nugetPackageRoot = if ($env:NUGET_PACKAGES) {
    $env:NUGET_PACKAGES
}
else {
    Join-Path ([Environment]::GetFolderPath([Environment+SpecialFolder]::UserProfile)) ".nuget/packages"
}
$runtimePackageDirectory = Join-Path `
    $nugetPackageRoot `
    "$($runtimePackId.ToLowerInvariant())/$runtimeFrameworkVersion"
foreach ($runtimeNotice in @(
    @{ Source = "LICENSE.TXT"; Destination = "dotnet-runtime-LICENSE.txt" },
    @{ Source = "THIRD-PARTY-NOTICES.TXT"; Destination = "dotnet-runtime-THIRD-PARTY-NOTICES.txt" }
)) {
    $sourcePath = Join-Path $runtimePackageDirectory $runtimeNotice.Source
    if (-not (Test-Path -LiteralPath $sourcePath -PathType Leaf)) {
        throw "Runtime pack notice is missing: '$sourcePath'."
    }
    Copy-Item `
        -LiteralPath $sourcePath `
        -Destination (Join-Path $licensesDirectory $runtimeNotice.Destination)
}

$releaseMetadata = [ordered]@{
    product = "Abituria"
    version = $Version
    runtimeIdentifier = $RuntimeIdentifier
    commit = $commit
    selfContained = $true
    trimmed = $false
    aot = $false
    readyToRun = $false
    singleFile = $false
}
$metadataJson = $releaseMetadata | ConvertTo-Json
$utf8WithoutBom = [Text.UTF8Encoding]::new($false)
[IO.File]::WriteAllText(
    (Join-Path $packageDirectory "release.json"),
    $metadataJson + [Environment]::NewLine,
    $utf8WithoutBom
)

& (Join-Path $PSScriptRoot "Test-PublishedArchitecture.ps1") `
    -PackageDirectory $packageDirectory `
    -RuntimeIdentifier $RuntimeIdentifier

$manifestOutputDirectory = Join-Path $packageDirectory "_manifest"
$packageSupplier = [string]::Concat("Adam Kubi", [char]0x015B)
$componentDirectory = Join-Path $workDirectory "components"
New-ReleaseNuGetComponentManifest `
    -PublishedDirectory $publishedDirectory `
    -RuntimeIdentifier $RuntimeIdentifier `
    -OutputDirectory $componentDirectory | Out-Null
Push-Location $repositoryRoot
try {
    Invoke-ExternalCommand -FilePath "dotnet" -ArgumentList @(
        "tool", "run", "sbom-tool", "--",
        "generate",
        "-b", $packageDirectory,
        "-bc", $componentDirectory,
        "-m", $packageDirectory,
        "-pn", "Abituria-$RuntimeIdentifier",
        "-pv", $Version,
        "-ps", $packageSupplier,
        "-nsb", "https://github.com/haribo841/Abituria/releases/download/v$Version",
        "-nsu", "Abituria-$Version-$RuntimeIdentifier",
        "-D", "true",
        "-pm", "true",
        "-F", "false",
        "-mi", "SPDX:2.2",
        "-V", "Warning"
    )
}
finally {
    Pop-Location
}

$manifestPath = Join-Path $manifestOutputDirectory "spdx_2.2/manifest.spdx.json"
if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf)) {
    throw "SBOM tool did not create an SPDX 2.2 manifest."
}

$generatedSbom = Get-Content -LiteralPath $manifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
if (@($generatedSbom.packages).Count -le 1) {
    throw "SBOM does not contain detected NuGet dependencies."
}
Assert-ReleaseSbomScope `
    -SbomPath $manifestPath `
    -PublishedDirectory $publishedDirectory `
    -RuntimeIdentifier $RuntimeIdentifier `
    -Version $Version

$sbomName = Get-ReleaseSbomName -Version $Version -RuntimeIdentifier $RuntimeIdentifier
Copy-Item -LiteralPath $manifestPath -Destination (Join-Path $OutputDirectory $sbomName)

if (Get-ChildItem -LiteralPath $packageDirectory -Filter "*.pdb" -File -Recurse) {
    throw "Katalog paczki zawiera pliki PDB."
}

$archiveName = Get-ReleaseArchiveName -Version $Version -RuntimeIdentifier $RuntimeIdentifier
$archivePath = Join-Path $OutputDirectory $archiveName

switch ($RuntimeIdentifier) {
    "win-x64" {
        if (-not $IsWindows) {
            throw "The win-x64 package must be archived on Windows."
        }
        Compress-Archive -LiteralPath $packageDirectory -DestinationPath $archivePath -CompressionLevel Optimal
    }
    "linux-x64" {
        if (-not $IsLinux) {
            throw "The linux-x64 package must be archived on Linux."
        }
        Invoke-ExternalCommand -FilePath "tar" -ArgumentList @(
            "-czf", $archivePath,
            "-C", $stagingDirectory,
            $packageName
        )
    }
    "osx-x64" {
        if (-not $IsMacOS) {
            throw "The osx-x64 package must be archived on macOS."
        }
        Invoke-ExternalCommand -FilePath "ditto" -ArgumentList @(
            "-c", "-k", "--sequesterRsrc", "--keepParent",
            $packageDirectory,
            $archivePath
        )
    }
}

if (-not (Test-Path -LiteralPath $archivePath -PathType Leaf) -or (Get-Item $archivePath).Length -eq 0) {
    throw "Nie utworzono archiwum $archiveName."
}

Write-Host "Utworzono artefakty $RuntimeIdentifier w $OutputDirectory"
