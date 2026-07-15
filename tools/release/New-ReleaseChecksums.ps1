[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$ArtifactsDirectory
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$artifactsDirectory = (Resolve-Path -LiteralPath $ArtifactsDirectory).Path
$payloadFiles = @(
    Get-ChildItem -LiteralPath $artifactsDirectory -File |
        Where-Object { $_.Name -ne "SHA256SUMS.txt" } |
        Sort-Object Name
)

if ($payloadFiles.Count -eq 0) {
    throw "No release payloads were found for SHA-256 generation."
}

$lines = foreach ($file in $payloadFiles) {
    $hash = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
    "$hash  $($file.Name)"
}

$checksumsPath = Join-Path $artifactsDirectory "SHA256SUMS.txt"
[IO.File]::WriteAllLines($checksumsPath, $lines, [Text.UTF8Encoding]::new($false))
Write-Host "Created $checksumsPath for $($payloadFiles.Count) files."
