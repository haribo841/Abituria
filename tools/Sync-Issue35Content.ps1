param(
    [string]$ContentRoot = (Join-Path $PSScriptRoot '..\Content')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$generatorPath = Join-Path $PSScriptRoot 'New-MathCourseContent.ps1'
if (-not (Test-Path -LiteralPath $generatorPath -PathType Leaf)) {
    throw "Nie znaleziono generatora kursu: $generatorPath"
}

$canonicalContentRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\Content'))
$requestedContentRoot = [System.IO.Path]::GetFullPath($ContentRoot)
$documentationPath = if ([string]::Equals(
        $canonicalContentRoot,
        $requestedContentRoot,
        [StringComparison]::OrdinalIgnoreCase)) {
    Join-Path $PSScriptRoot '..\docs\MATH_COURSE_2023_COVERAGE.md'
} else {
    Join-Path $requestedContentRoot 'MATH_COURSE_2023_COVERAGE.md'
}

& $generatorPath -ContentRoot $ContentRoot -DocumentationPath $documentationPath

Write-Host 'Zsynchronizowano treści Issue #35 jako część katalogu kursu Formuła 2023.'
