[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$OpenCoverReport,

    [Parameter(Mandatory)]
    [string]$PythonCoverageReport,

    [ValidateRange(0, 100)]
    [double]$MinimumOverallCoverage = 90,

    [ValidateRange(0, 100)]
    [double]$MinimumBranchCoverage = 85
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Read-XmlReport {
    param(
        [Parameter(Mandatory)]
        [string]$Path,

        [Parameter(Mandatory)]
        [string]$Description
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "$Description report does not exist: $Path"
    }

    try {
        return [xml](Get-Content -LiteralPath $Path -Raw -Encoding UTF8)
    }
    catch {
        throw "$Description report is not valid XML: $Path. $($_.Exception.Message)"
    }
}

function Get-OpenCoverMetrics {
    param([xml]$Report)

    $summary = $Report.CoverageSession.Summary
    if ($null -eq $summary) {
        throw "OpenCover report does not contain CoverageSession/Summary."
    }

    $lineVisits = @{}
    foreach ($module in @($Report.CoverageSession.Modules.Module)) {
        $files = @{}
        foreach ($file in @($module.Files.File)) {
            $files[[string]$file.uid] = [string]$file.fullPath
        }

        foreach ($sequencePoint in @($module.SelectNodes(".//SequencePoint"))) {
            $fileId = [string]$sequencePoint.fileid
            if (-not $files.ContainsKey($fileId) -or [int]$sequencePoint.sl -le 0) {
                continue
            }

            $key = "$($files[$fileId])|$([int]$sequencePoint.sl)"
            if (-not $lineVisits.ContainsKey($key)) {
                $lineVisits[$key] = $false
            }
            if ([int]$sequencePoint.vc -gt 0) {
                $lineVisits[$key] = $true
            }
        }
    }

    $coveredLines = @($lineVisits.Values | Where-Object { $_ }).Count
    $totalLines = $lineVisits.Count
    $coveredBranches = [int]$summary.visitedBranchPoints
    $totalBranches = [int]$summary.numBranchPoints
    if ($totalLines -le 0 -or $totalBranches -le 0) {
        throw "OpenCover report does not contain measurable lines and branches."
    }

    return [pscustomobject]@{
        Name = "C#"
        CoveredLines = $coveredLines
        TotalLines = $totalLines
        CoveredBranches = $coveredBranches
        TotalBranches = $totalBranches
    }
}

function Get-PythonCoverageMetrics {
    param([xml]$Report)

    $coverage = $Report.coverage
    if ($null -eq $coverage) {
        throw "Python coverage report does not contain the Cobertura coverage element."
    }

    $metrics = [pscustomobject]@{
        Name = "Python"
        CoveredLines = [int]$coverage.'lines-covered'
        TotalLines = [int]$coverage.'lines-valid'
        CoveredBranches = [int]$coverage.'branches-covered'
        TotalBranches = [int]$coverage.'branches-valid'
    }
    if ($metrics.TotalLines -le 0 -or $metrics.TotalBranches -le 0) {
        throw "Python coverage report does not contain measurable lines and branches."
    }

    return $metrics
}

function Get-Percentage {
    param(
        [int]$Covered,
        [int]$Total
    )

    return 100.0 * $Covered / $Total
}

function Format-Percentage {
    param([double]$Value)

    return $Value.ToString("F2", [Globalization.CultureInfo]::InvariantCulture)
}

$openCoverXml = Read-XmlReport -Path $OpenCoverReport -Description "OpenCover"
$pythonXml = Read-XmlReport -Path $PythonCoverageReport -Description "Python coverage"
$metrics = @(
    Get-OpenCoverMetrics -Report $openCoverXml
    Get-PythonCoverageMetrics -Report $pythonXml
)

foreach ($metric in $metrics) {
    $lineCoverage = Get-Percentage -Covered $metric.CoveredLines -Total $metric.TotalLines
    $branchCoverage = Get-Percentage -Covered $metric.CoveredBranches -Total $metric.TotalBranches
    Write-Host "$($metric.Name): lines $(Format-Percentage $lineCoverage)% ($($metric.CoveredLines)/$($metric.TotalLines)), branches $(Format-Percentage $branchCoverage)% ($($metric.CoveredBranches)/$($metric.TotalBranches))."
}

$coveredLines = ($metrics | Measure-Object -Property CoveredLines -Sum).Sum
$totalLines = ($metrics | Measure-Object -Property TotalLines -Sum).Sum
$coveredBranches = ($metrics | Measure-Object -Property CoveredBranches -Sum).Sum
$totalBranches = ($metrics | Measure-Object -Property TotalBranches -Sum).Sum
$overallCoverage = Get-Percentage -Covered ($coveredLines + $coveredBranches) -Total ($totalLines + $totalBranches)
$branchCoverage = Get-Percentage -Covered $coveredBranches -Total $totalBranches

$failures = [Collections.Generic.List[string]]::new()
if ($overallCoverage -lt $MinimumOverallCoverage) {
    $failures.Add("overall coverage $(Format-Percentage $overallCoverage)% is below $(Format-Percentage $MinimumOverallCoverage)%")
}
if ($branchCoverage -lt $MinimumBranchCoverage) {
    $failures.Add("branch coverage $(Format-Percentage $branchCoverage)% is below $(Format-Percentage $MinimumBranchCoverage)%")
}

if ($failures.Count -gt 0) {
    throw "Coverage gate failed: $($failures -join '; ')."
}

Write-Host "Coverage gate passed: overall $(Format-Percentage $overallCoverage)% and branches $(Format-Percentage $branchCoverage)%."
