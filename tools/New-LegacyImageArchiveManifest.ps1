[CmdletBinding()]
param(
    [string]$RepositoryRoot = (Split-Path -Parent $PSScriptRoot)
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$archiveRoot = Join-Path $RepositoryRoot 'docs/legacy/originals/images'
$expectedCount = 75
$files = @(Get-ChildItem -LiteralPath $archiveRoot -File -Recurse |
    Where-Object { $_.Name -notin @('README.md', 'PATH-MAPPING.csv', 'SHA256SUMS') } |
    Sort-Object { [IO.Path]::GetRelativePath($archiveRoot, $_.FullName) })

if ($files.Count -ne $expectedCount) {
    throw "Archiwum powinno zawierać dokładnie $expectedCount obrazów, znaleziono $($files.Count)."
}

$utf8 = [Text.UTF8Encoding]::new($false)
$mappingLines = [Collections.Generic.List[string]]::new()
$mappingLines.Add('oldPath,archivePath')
$checksumLines = [Collections.Generic.List[string]]::new()

foreach ($file in $files) {
    $relative = [IO.Path]::GetRelativePath($archiveRoot, $file.FullName).Replace('\', '/')
    $mappingLines.Add(('"img/{0}","docs/legacy/originals/images/{0}"' -f $relative.Replace('"', '""')))
    $hash = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash.ToUpperInvariant()
    $checksumLines.Add("$hash  $relative")
}

$readme = @'
# Archiwum historycznych obrazów

Ten katalog zawiera 75 obrazów przeniesionych bez zmiany bajtów z aktywnego katalogu `img/`.
Pliki służą wyłącznie jako niepublikowane oryginały historyczne i nie są zasobami działania aplikacji ani witryny DocFX.

- `PATH-MAPPING.csv` mapuje każdą dawną ścieżkę `img/...` na ścieżkę archiwalną.
- `SHA256SUMS` zawiera sumę SHA-256 każdego przeniesionego pliku.
- `img/icon.ico` nie należy do archiwum. Pozostaje jedynym statycznym zasobem graficznym aplikacji i jest używany wyłącznie jako `ApplicationIcon`.
'@

[IO.File]::WriteAllText((Join-Path $archiveRoot 'README.md'), $readme.TrimEnd() + "`n", $utf8)
[IO.File]::WriteAllLines((Join-Path $archiveRoot 'PATH-MAPPING.csv'), $mappingLines, $utf8)
[IO.File]::WriteAllLines((Join-Path $archiveRoot 'SHA256SUMS'), $checksumLines, $utf8)

Write-Host "Zapisano mapowanie i sumy SHA-256 dla $($files.Count) obrazów."
