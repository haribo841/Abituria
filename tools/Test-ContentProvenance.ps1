param(
    [switch]$RequireReleaseEligible
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$manifestPath = Join-Path $root "Content/provenance.json"
$projectPath = Join-Path $root "Abituria.csproj"

function Convert-GlobToRegex([string]$pattern) {
    $normalized = $pattern.Replace('\', '/')
    $expression = [Regex]::Escape($normalized)
    $expression = $expression.Replace('\*\*/', '(?:.*/)?')
    $expression = $expression.Replace('\*', '[^/]*')
    $expression = $expression.Replace('\?', '[^/]')
    return '^' + $expression + '$'
}

function Expand-Glob([string]$pattern) {
    $normalized = $pattern.Replace('\', '/')
    $topDirectory = $normalized.Split('/')[0]
    $searchRoot = Join-Path $root $topDirectory
    if (-not (Test-Path -LiteralPath $searchRoot)) { return @() }
    $regex = Convert-GlobToRegex $normalized
    return @(
        Get-ChildItem -LiteralPath $searchRoot -File -Recurse |
            ForEach-Object { $_.FullName.Substring($root.Length + 1).Replace('\', '/') } |
            Where-Object { $_ -match $regex } |
            Sort-Object -Unique
    )
}

[xml]$project = Get-Content -Raw -Encoding UTF8 $projectPath
$resourcePatterns = @(
    $project.Project.ItemGroup.AvaloniaResource |
        ForEach-Object { [string]$_.Include } |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
        ForEach-Object { $_.Split(';', [StringSplitOptions]::RemoveEmptyEntries) }
)
$applicationIcons = @(
    $project.Project.PropertyGroup.ApplicationIcon |
        ForEach-Object { [string]$_.'#text' } |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
)
$packagedResources = @(
    $resourcePatterns + $applicationIcons |
        ForEach-Object { Expand-Glob $_ } |
        Sort-Object -Unique
)

$manifest = Get-Content -Raw -Encoding UTF8 $manifestPath | ConvertFrom-Json
if ($manifest.schemaVersion -ne 1) { throw "Nieobsługiwana wersja schematu manifestu pochodzenia." }
if (-not $manifest.assets) { throw "Manifest pochodzenia nie zawiera zasobów." }

$declaredResources = [System.Collections.Generic.List[string]]::new()
$blockingGroups = [System.Collections.Generic.List[string]]::new()
foreach ($asset in $manifest.assets) {
    foreach ($field in @('id', 'author', 'source', 'license', 'distributionStatus')) {
        if ([string]::IsNullOrWhiteSpace([string]$asset.$field)) {
            throw "Grupa zasobów nie ma wymaganego pola $field."
        }
    }
    if ($asset.distributionStatus -notin @('approved', 'blocked')) {
        throw "Grupa $($asset.id) ma nieznany status $($asset.distributionStatus)."
    }
    if ($asset.distributionStatus -eq 'blocked') {
        if ([string]::IsNullOrWhiteSpace([string]$asset.blockedReason)) {
            throw "Grupa $($asset.id) jest zablokowana bez podania przyczyny."
        }
        $blockingGroups.Add([string]$asset.id)
    }
    if (-not $asset.evidence) { throw "Grupa $($asset.id) nie zawiera dowodów pochodzenia." }
    foreach ($evidence in $asset.evidence) {
        if (-not (Test-Path -LiteralPath (Join-Path $root ([string]$evidence)))) {
            throw "Grupa $($asset.id) odwołuje się do brakującego dowodu: $evidence."
        }
    }
    if (-not $asset.paths) { throw "Grupa $($asset.id) nie zawiera wzorców plików." }
    foreach ($pattern in $asset.paths) {
        $matchedPaths = @(Expand-Glob ([string]$pattern))
        if ($matchedPaths.Count -eq 0) { throw "Wzorzec $pattern z grupy $($asset.id) nie pasuje do żadnego pliku." }
        foreach ($matchedPath in $matchedPaths) {
            if ($declaredResources.Contains($matchedPath)) { throw "Zasób $matchedPath jest zadeklarowany więcej niż raz." }
            $declaredResources.Add($matchedPath)
        }
    }
}

$missing = @($packagedResources | Where-Object { -not $declaredResources.Contains($_) })
$extra = @($declaredResources | Where-Object { $_ -notin $packagedResources })
if ($missing.Count -gt 0) { throw "Brak pochodzenia dla paczkowanych zasobów: $($missing -join ', ')." }
if ($extra.Count -gt 0) { throw "Manifest opisuje zasoby niepakowane przez projekt: $($extra -join ', ')." }

$computedEligibility = $blockingGroups.Count -eq 0
if ([bool]$manifest.releaseEligible -ne $computedEligibility) {
    throw "Pole releaseEligible nie zgadza się ze statusami zasobów."
}
if ($RequireReleaseEligible -and -not $computedEligibility) {
    throw "Publiczne wydanie jest zablokowane przez grupy: $($blockingGroups -join ', ')."
}

Write-Host "Manifest pochodzenia obejmuje $($packagedResources.Count) paczkowanych zasobów. releaseEligible=$computedEligibility."
