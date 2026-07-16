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
$releaseHeader = [regex]::Match(
    $changelog,
    "(?m)^##\s+(?:\[$escapedVersion\]|$escapedVersion)\s+-\s+(?<date>\d{4}-\d{2}-\d{2})\s*$"
)
if (-not $releaseHeader.Success) {
    throw "Sekcja $version w CHANGELOG.md musi zawierać datę przygotowania tagu w UTC jako YYYY-MM-DD."
}
$releaseDate = [DateTime]::MinValue
if (-not [DateTime]::TryParseExact(
    $releaseHeader.Groups["date"].Value,
    "yyyy-MM-dd",
    [Globalization.CultureInfo]::InvariantCulture,
    [Globalization.DateTimeStyles]::None,
    [ref]$releaseDate
)) {
    throw "Sekcja $version w CHANGELOG.md zawiera nieistniejącą datę kalendarzową."
}

$readmePath = Join-Path $repositoryRoot "README.md"
$readme = Get-Content -LiteralPath $readmePath -Raw -Encoding UTF8
if ($readme -notmatch [regex]::Escape("v$version")) {
    throw "README.md nie zawiera numeru wydania v$version."
}

$provenancePath = Join-Path $repositoryRoot "Content/provenance.json"
$provenance = Get-Content -LiteralPath $provenancePath -Raw -Encoding UTF8 | ConvertFrom-Json
if ($provenance.releaseEligible) {
    foreach ($staleMarker in @(
        @{ Path = "README.md"; Text = "Wersja przygotowywana do pierwszej publicznej publikacji" },
        @{ Path = "README.md"; Text = "Publiczne wydanie jest obecnie zablokowane" },
        @{ Path = "README.md"; Text = "Po zamknięciu checklisty wydawniczej paczki pojawią się" },
        @{ Path = "docs/INSTALLATION.md"; Text = "Paczki nie są jeszcze przeznaczone do publicznego pobierania" },
        @{ Path = "docs/RELEASE_PROCESS.md"; Text = "Aktualny inwentarz ma ``releaseEligible=false``" }
    )) {
        $document = Get-Content `
            -LiteralPath (Join-Path $repositoryRoot $staleMarker.Path) `
            -Raw `
            -Encoding UTF8
        if ($document.Contains($staleMarker.Text, [StringComparison]::Ordinal)) {
            throw "Dokument '$($staleMarker.Path)' nadal zawiera przedwydaniowy komunikat: '$($staleMarker.Text)'."
        }
    }
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

if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    throw "Git jest wymagany do powiązania tagu z commitem origin/main."
}
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

    $mainCommit = (& git rev-parse --verify "refs/remotes/origin/main^{commit}" 2>$null)
    if ($LASTEXITCODE -ne 0) {
        throw "Brakuje pobranej referencji refs/remotes/origin/main."
    }
    if ($tagCommit.Trim() -cne $mainCommit.Trim()) {
        throw "Tag '$Tag' nie wskazuje dokładnie zweryfikowanego commita origin/main."
    }

    $tagType = (& git cat-file -t "refs/tags/$Tag" 2>$null).Trim()
    $tagTimestamp = (& git for-each-ref --format="%(taggerdate:unix)" "refs/tags/$Tag" 2>$null).Trim()
    if ($LASTEXITCODE -ne 0 -or $tagType -cne "tag" -or $tagTimestamp -notmatch '^\d+$') {
        throw "Tag '$Tag' musi być adnotowany i zawierać prawidłową datę taggera."
    }
    $tagDateUtc = [DateTimeOffset]::FromUnixTimeSeconds([long]$tagTimestamp).UtcDateTime.Date
    if ($tagDateUtc -gt [DateTime]::UtcNow.Date) {
        throw "Data adnotowanego tagu nie może leżeć w przyszłości."
    }
    if ($releaseDate.Date -ne $tagDateUtc) {
        throw "Data changelogu musi być datą przygotowania adnotowanego tagu w UTC: $($tagDateUtc.ToString('yyyy-MM-dd'))."
    }
}
finally {
    Pop-Location
}

if ($WriteGitHubOutput) {
    if (-not $env:GITHUB_OUTPUT) {
        throw "WriteGitHubOutput requires the GITHUB_OUTPUT environment variable."
    }

    "version=$version" | Out-File -LiteralPath $env:GITHUB_OUTPUT -Encoding utf8 -Append
}

Write-Host "Version and tag are consistent: $Tag"
$version
