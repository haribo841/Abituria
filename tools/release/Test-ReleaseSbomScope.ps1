[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$SbomPath,

    [Parameter(Mandatory)]
    [string]$PublishedDirectory,

    [Parameter(Mandatory)]
    [ValidateSet("win-x64", "linux-x64", "osx-x64")]
    [string]$RuntimeIdentifier,

    [Parameter(Mandatory)]
    [string]$Version
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Import-Module (Join-Path $PSScriptRoot "Release.Common.psm1") -Force

Assert-ReleaseSbomScope `
    -SbomPath $SbomPath `
    -PublishedDirectory $PublishedDirectory `
    -RuntimeIdentifier $RuntimeIdentifier `
    -Version $Version

Write-Host "SBOM scope is correct for $RuntimeIdentifier."
