param(
    [string]$ContentRoot = (Join-Path $PSScriptRoot '..\Content')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$seedPath = Join-Path $PSScriptRoot 'seeds\issue-35-content.json'
$chaptersPath = Join-Path $ContentRoot 'chapters.json'
$roadmapPath = Join-Path $ContentRoot 'roadmap.json'

$seed = Get-Content -Raw -Encoding UTF8 $seedPath | ConvertFrom-Json
$currentChapters = Get-Content -Raw -Encoding UTF8 $chaptersPath | ConvertFrom-Json
$vectors = @($currentChapters.chapters | Where-Object { $_.id -eq 'vectors' })
if ($vectors.Count -ne 1 -or $vectors[0].status -ne 'available') {
    throw 'Aktywny katalog musi zawierac dokladnie jeden dostepny rozdzial vectors.'
}

$chapterCatalog = [ordered]@{
    schemaVersion = 2
    introduction = @($seed.chapterIntroduction)
    chapters = @($vectors[0]) + @($seed.chapters)
}
$roadmapCatalog = [ordered]@{
    schemaVersion = 1
    introduction = @($seed.roadmapIntroduction)
    items = @($seed.roadmapItems)
}

$chapterCatalog | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $chaptersPath -Encoding UTF8
$roadmapCatalog | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $roadmapPath -Encoding UTF8

Write-Host "Zsynchronizowano $($chapterCatalog.chapters.Count) dzialow i $($roadmapCatalog.items.Count) pozycji roadmapy."
