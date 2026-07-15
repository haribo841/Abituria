[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Version,

    [Parameter(Mandatory)]
    [string]$OutputPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Import-Module (Join-Path $PSScriptRoot "Release.Common.psm1") -Force
$repositoryRoot = Get-RepositoryRoot
if ($Version -cne (Get-AbituriaVersion -RepositoryRoot $repositoryRoot)) {
    throw "Cannot generate notes for non-canonical version '$Version'."
}

$lines = Get-Content -LiteralPath (Join-Path $repositoryRoot "CHANGELOG.md") -Encoding UTF8
$escapedVersion = [regex]::Escape($Version)
$headingPattern = "^##\s+(?:\[$escapedVersion\]|$escapedVersion)(?:\s|$)"
$startIndex = -1
for ($index = 0; $index -lt $lines.Count; $index++) {
    if ($lines[$index] -match $headingPattern) {
        $startIndex = $index
        break
    }
}

if ($startIndex -lt 0) {
    throw "Nie znaleziono sekcji $Version w CHANGELOG.md."
}

$endIndex = $lines.Count
for ($index = $startIndex + 1; $index -lt $lines.Count; $index++) {
    if ($lines[$index] -match '^##\s+') {
        $endIndex = $index
        break
    }
}

$releaseLines = @($lines[$startIndex..($endIndex - 1)])
if (($releaseLines -join "").Trim().Length -eq 0) {
    throw "Sekcja wydania $Version jest pusta."
}

$parentDirectory = Split-Path -Parent $OutputPath
if ($parentDirectory) {
    New-Item -ItemType Directory -Path $parentDirectory -Force | Out-Null
}
[IO.File]::WriteAllLines($OutputPath, $releaseLines, [Text.UTF8Encoding]::new($false))
Write-Host "Zapisano notatki wydania w $OutputPath."
