[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Tag,

    [switch]$WriteGitHubOutput
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Import-Module (Join-Path $PSScriptRoot "Release.Common.psm1") -Force

$repositoryRoot = Get-RepositoryRoot
$version = Assert-ReleaseTag -Tag $Tag -RepositoryRoot $repositoryRoot

$changelogPath = Join-Path $repositoryRoot "CHANGELOG.md"
if (-not (Test-Path -LiteralPath $changelogPath -PathType Leaf)) {
    throw "Brakuje CHANGELOG.md."
}

$escapedVersion = [regex]::Escape($version)
$changelog = Get-Content -LiteralPath $changelogPath -Raw -Encoding UTF8
if ($changelog -notmatch "(?m)^##\s+(?:\[$escapedVersion\]|$escapedVersion)(?:\s|$)") {
    throw "CHANGELOG.md nie zawiera sekcji wydania $version."
}

$readmePath = Join-Path $repositoryRoot "README.md"
$readme = Get-Content -LiteralPath $readmePath -Raw -Encoding UTF8
if ($readme -notmatch [regex]::Escape("v$version")) {
    throw "README.md nie zawiera numeru wydania v$version."
}

$toolManifestPath = Join-Path $repositoryRoot ".config/dotnet-tools.json"
$toolManifest = Get-Content -LiteralPath $toolManifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
if ($toolManifest.tools.docfx.version -cne "2.78.5") {
    throw "The tool manifest must pin docfx 2.78.5."
}

$sbomTool = $toolManifest.tools.PSObject.Properties["microsoft.sbom.dotnettool"].Value
if ($sbomTool.version -cne "4.1.5") {
    throw "The tool manifest must pin Microsoft.Sbom.DotNetTool 4.1.5."
}

$globalJsonPath = Join-Path $repositoryRoot "global.json"
$globalJson = Get-Content -LiteralPath $globalJsonPath -Raw -Encoding UTF8 | ConvertFrom-Json
if ($globalJson.sdk.version -cne "10.0.302") {
    throw "global.json must pin .NET SDK 10.0.302."
}

if (Get-Command git -ErrorAction SilentlyContinue) {
    Push-Location $repositoryRoot
    try {
        $tagCommit = (& git rev-parse --verify "refs/tags/$Tag^{commit}" 2>$null)
        if ($LASTEXITCODE -ne 0) {
            throw "Tag '$Tag' nie istnieje w pobranej historii Git."
        }

        $headCommit = (& git rev-parse HEAD).Trim()
        if ($LASTEXITCODE -ne 0 -or $tagCommit.Trim() -cne $headCommit) {
            throw "HEAD does not point at tag '$Tag'."
        }
    }
    finally {
        Pop-Location
    }
}

if ($WriteGitHubOutput) {
    if (-not $env:GITHUB_OUTPUT) {
        throw "WriteGitHubOutput requires the GITHUB_OUTPUT environment variable."
    }

    "version=$version" | Out-File -LiteralPath $env:GITHUB_OUTPUT -Encoding utf8 -Append
}

Write-Host "Version and tag are consistent: $Tag"
$version
