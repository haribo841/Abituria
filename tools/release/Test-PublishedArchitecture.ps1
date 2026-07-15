[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$PackageDirectory,

    [Parameter(Mandatory)]
    [ValidateSet("win-x64", "linux-x64", "osx-x64")]
    [string]$RuntimeIdentifier
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$packageDirectory = (Resolve-Path -LiteralPath $PackageDirectory).Path

switch ($RuntimeIdentifier) {
    "win-x64" {
        $executable = Join-Path $packageDirectory "Abituria.exe"
        if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
            throw "Brakuje pliku Abituria.exe."
        }

        $stream = [IO.File]::OpenRead($executable)
        try {
            $reader = [IO.BinaryReader]::new($stream)
            if ($reader.ReadUInt16() -ne 0x5A4D) {
                throw "Abituria.exe does not have a PE header."
            }
            $stream.Position = 0x3C
            $peOffset = $reader.ReadInt32()
            $stream.Position = $peOffset
            if ($reader.ReadUInt32() -ne 0x00004550) {
                throw "Abituria.exe has an invalid PE signature."
            }
            if ($reader.ReadUInt16() -ne 0x8664) {
                throw "Abituria.exe nie jest plikiem PE x64."
            }
        }
        finally {
            $stream.Dispose()
        }
    }
    "linux-x64" {
        $executable = Join-Path $packageDirectory "Abituria"
        $description = (& file -b $executable)
        if ($LASTEXITCODE -ne 0 -or $description -notmatch 'ELF 64-bit.*x86-64') {
            throw "Plik Abituria nie jest plikiem ELF x64: $description"
        }
    }
    "osx-x64" {
        $executable = Join-Path $packageDirectory "Abituria.app/Contents/MacOS/Abituria"
        $description = (& file -b $executable)
        if ($LASTEXITCODE -ne 0 -or $description -notmatch 'Mach-O 64-bit.*x86_64') {
            throw "Plik Abituria nie jest plikiem Mach-O x64: $description"
        }
        if ($description -match 'arm64') {
            throw "The osx-x64 package must not contain an arm64 or universal binary."
        }
    }
}

Write-Host "Architektura publikacji jest poprawna: $RuntimeIdentifier"
