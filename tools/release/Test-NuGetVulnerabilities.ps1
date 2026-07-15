[CmdletBinding()]
param(
    [string]$Solution = "Abituria.sln",

    [string]$ReportPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Find-Vulnerabilities {
    param([object]$Node)

    if ($null -eq $Node) {
        return
    }

    if ($Node -is [Collections.IDictionary]) {
        foreach ($key in $Node.Keys) {
            if ($key -eq "vulnerabilities") {
                foreach ($vulnerability in @($Node[$key])) {
                    if ($null -ne $vulnerability) {
                        $vulnerability
                    }
                }
            }
            else {
                Find-Vulnerabilities -Node $Node[$key]
            }
        }
        return
    }

    if ($Node -is [Collections.IEnumerable] -and $Node -isnot [string]) {
        foreach ($item in $Node) {
            Find-Vulnerabilities -Node $item
        }
        return
    }

    if ($Node -isnot [PSCustomObject]) {
        return
    }

    foreach ($property in $Node.PSObject.Properties) {
        if ($property.Name -eq "vulnerabilities") {
            foreach ($vulnerability in @($property.Value)) {
                if ($null -ne $vulnerability) {
                    $vulnerability
                }
            }
        }
        else {
            Find-Vulnerabilities -Node $property.Value
        }
    }
}

$jsonLines = & dotnet list $Solution package `
    --vulnerable `
    --include-transitive `
    --configfile NuGet.Config `
    --format json `
    --output-version 1
if ($LASTEXITCODE -ne 0) {
    throw "NuGet vulnerability audit failed with exit code $LASTEXITCODE."
}

$json = $jsonLines -join [Environment]::NewLine
$report = $json | ConvertFrom-Json
$vulnerabilities = @(Find-Vulnerabilities -Node $report)

if ($ReportPath) {
    $parentDirectory = Split-Path -Parent $ReportPath
    if ($parentDirectory) {
        New-Item -ItemType Directory -Path $parentDirectory -Force | Out-Null
    }
    [IO.File]::WriteAllText($ReportPath, $json + [Environment]::NewLine, [Text.UTF8Encoding]::new($false))
}

if ($vulnerabilities.Count -ne 0) {
    $details = $vulnerabilities | ConvertTo-Json -Depth 10 -Compress
    throw "NuGet audit found $($vulnerabilities.Count) vulnerabilities: $details"
}

Write-Host "NuGet audit found no vulnerable packages."
