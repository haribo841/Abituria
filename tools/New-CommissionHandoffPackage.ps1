param(
    [string]$OutputDirectory = "artifacts/handoff"
)

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$repositoryRoot = [IO.Path]::GetFullPath($repositoryRoot)
$outputRoot = if ([IO.Path]::IsPathRooted($OutputDirectory)) {
    [IO.Path]::GetFullPath($OutputDirectory)
} else {
    [IO.Path]::GetFullPath((Join-Path $repositoryRoot $OutputDirectory))
}

[xml]$buildProperties = Get-Content -LiteralPath (Join-Path $repositoryRoot "Directory.Build.props") -Raw -Encoding UTF8
$version = [string]$buildProperties.Project.PropertyGroup.Version
if ([string]::IsNullOrWhiteSpace($version)) { throw "Nie można odczytać wersji produktu." }
$provenance = Get-Content -LiteralPath (Join-Path $repositoryRoot "Content/provenance.json") -Raw -Encoding UTF8 |
    ConvertFrom-Json
$releaseEligibility = [bool]$provenance.releaseEligible

$baseCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0 -or $baseCommit -notmatch '^[0-9a-f]{40}$') {
    throw "Nie można odczytać bazowego commita Git."
}

$packageName = "Abituria-v$version-commission-documentation-candidate"
$archivePath = Join-Path $outputRoot "$packageName.zip"
$checksumPath = Join-Path $outputRoot "SHA256SUMS.txt"
$temporaryBase = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())
$stagingRoot = [IO.Path]::GetFullPath((Join-Path $temporaryBase "Abituria-handoff-$([Guid]::NewGuid().ToString('N'))"))
if (-not $stagingRoot.StartsWith($temporaryBase, [StringComparison]::OrdinalIgnoreCase) -or
    -not (Split-Path -Leaf $stagingRoot).StartsWith("Abituria-handoff-", [StringComparison]::Ordinal)) {
    throw "Nieprawidłowy katalog tymczasowy pakietu."
}
$packageRoot = Join-Path $stagingRoot $packageName

function Get-RelativePath([string]$path) {
    $fullPath = [IO.Path]::GetFullPath($path)
    $rootPrefix = $repositoryRoot.TrimEnd([IO.Path]::DirectorySeparatorChar, [IO.Path]::AltDirectorySeparatorChar) +
        [IO.Path]::DirectorySeparatorChar
    if (-not $fullPath.StartsWith($rootPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Plik znajduje się poza repozytorium: $fullPath"
    }
    return $fullPath.Substring($rootPrefix.Length).Replace('\', '/')
}

function Get-PackageRelativePath([string]$path) {
    $fullPath = [IO.Path]::GetFullPath($path)
    $packagePrefix = [IO.Path]::GetFullPath($packageRoot).TrimEnd(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    if (-not $fullPath.StartsWith($packagePrefix, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Plik znajduje się poza katalogiem pakietu: $fullPath"
    }
    return $fullPath.Substring($packagePrefix.Length).Replace('\', '/')
}

function Copy-PayloadFile([string]$relativePath) {
    $source = Join-Path $repositoryRoot $relativePath
    if (-not (Test-Path -LiteralPath $source -PathType Leaf)) {
        throw "Brak wymaganego pliku pakietu: $relativePath"
    }
    $destination = Join-Path $packageRoot $relativePath
    $destinationDirectory = Split-Path -Parent $destination
    New-Item -ItemType Directory -Path $destinationDirectory -Force | Out-Null
    Copy-Item -LiteralPath $source -Destination $destination
}

try {
    New-Item -ItemType Directory -Path $packageRoot -Force | Out-Null

    $rootFiles = @(
        "README.md",
        "LICENSE",
        "AUTHORS.md",
        "CHANGELOG.md",
        "THIRD-PARTY-NOTICES.md",
        "SECURITY.md",
        "SUPPORT.md",
        "Content/provenance.json",
        "output/pdf/Abituria-Technical-Documentation-0.9.0-beta.1.pdf"
    )
    foreach ($relativePath in $rootFiles) { Copy-PayloadFile $relativePath }

    $documentationRoot = Join-Path $repositoryRoot "docs"
    $documentationFiles = Get-ChildItem -LiteralPath $documentationRoot -File -Recurse |
        Where-Object { (Get-RelativePath $_.FullName) -notlike "docs/legacy/*" } |
        Sort-Object FullName
    foreach ($file in $documentationFiles) { Copy-PayloadFile (Get-RelativePath $file.FullName) }

    $payloadFiles = Get-ChildItem -LiteralPath $packageRoot -File -Recurse | Sort-Object FullName
    $fileRecords = @(
        foreach ($file in $payloadFiles) {
            [ordered]@{
                path = Get-PackageRelativePath $file.FullName
                bytes = $file.Length
                sha256 = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
            }
        }
    )
    $worktreeDirty = [bool](& git -C $repositoryRoot status --porcelain)
    $manifest = [ordered]@{
        schemaVersion = 1
        packageType = "commission-documentation-candidate"
        isApplicationRelease = $false
        isRunnable = $false
        version = $version
        baseCommit = $baseCommit
        worktreeHadUncommittedChanges = $worktreeDirty
        generatedAtUtc = [DateTime]::UtcNow.ToString("o")
        purpose = "Ocena dokumentacji i dowodów technicznych przez komisję. Pakiet nie jest wydaniem aplikacji."
        releaseEligibilityAtGeneration = $releaseEligibility
        approvedAssetGroupsNotIncludedBecausePackageIsDocumentationOnly = @(
            "cke-2021-correction-exam",
            "inherited-mathematics-images",
            "inherited-application-images"
        )
        excludedScopes = @(
            "aplikacja i biblioteki wykonywalne - poza zakresem pakietu dokumentacyjnego",
            "kod źródłowy i testy - poza zakresem pakietu dokumentacyjnego",
            "Content/exam-2021-correction.json - poza zakresem pakietu dokumentacyjnego",
            "img/ - poza zakresem pakietu dokumentacyjnego",
            "tools/Import-LegacyContent.ps1",
            "docs/legacy/",
            ".git i historia Git"
        )
        payloadFiles = $fileRecords
    }
    $manifestPath = Join-Path $packageRoot "HANDOFF-MANIFEST.json"
    $manifest | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $manifestPath -Encoding UTF8

    New-Item -ItemType Directory -Path $outputRoot -Force | Out-Null
    if (Test-Path -LiteralPath $archivePath) { Remove-Item -LiteralPath $archivePath -Force }
    if (Test-Path -LiteralPath $checksumPath) { Remove-Item -LiteralPath $checksumPath -Force }
    Compress-Archive -LiteralPath $packageRoot -DestinationPath $archivePath -CompressionLevel Optimal

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $archive = [IO.Compression.ZipFile]::OpenRead($archivePath)
    try {
        $entries = @($archive.Entries | Where-Object { $_.Name })
        $entryNames = @($entries | ForEach-Object { $_.FullName.Replace('\', '/') })
        $forbidden = @(
            $entries | Where-Object {
                $normalized = $_.FullName.Replace('\', '/')
                $normalized -match '(^|/)docs/legacy/' -or
                $normalized -match '(^|/)img/' -or
                $normalized -match '(^|/)Content/exam-2021-correction\.json$' -or
                $normalized -match '(^|/)\.git/' -or
                $normalized -match '\.(exe|dll|pdb|ico|icns|png|cs|axaml)$'
            }
        )
        if ($forbidden.Count -gt 0) {
            throw "Pakiet zawiera niedozwolone pliki: $($forbidden.FullName -join ', ')"
        }
        foreach ($required in @("HANDOFF-MANIFEST.json", "docs/DELIVERY_PROTOCOL.md", "docs/DEFENSE_PROTOCOL.md", "docs/EVALUATION_PROTOCOL.md", "Content/provenance.json")) {
            if (-not ($entryNames -contains "$packageName/$required")) {
                throw "Archiwum nie zawiera wymaganego pliku: $required"
            }
        }
    } finally {
        $archive.Dispose()
    }

    $archiveHash = (Get-FileHash -LiteralPath $archivePath -Algorithm SHA256).Hash
    "$archiveHash  $([IO.Path]::GetFileName($archivePath))" |
        Set-Content -LiteralPath $checksumPath -Encoding ASCII
    Write-Host "Utworzono kandydat dokumentacyjny: $archivePath"
    Write-Host "SHA256=$archiveHash"
    Write-Host "Pakiet nie jest uruchamialną aplikacją ani publicznym wydaniem; wyłączenia wynikają z dokumentacyjnego zakresu pakietu, a nie z blokady prawnej zasobów."
} finally {
    if (Test-Path -LiteralPath $stagingRoot) {
        $resolvedStaging = [IO.Path]::GetFullPath($stagingRoot)
        if (-not $resolvedStaging.StartsWith($temporaryBase, [StringComparison]::OrdinalIgnoreCase) -or
            -not (Split-Path -Leaf $resolvedStaging).StartsWith("Abituria-handoff-", [StringComparison]::Ordinal)) {
            throw "Odmowa usunięcia niezweryfikowanego katalogu tymczasowego."
        }
        Remove-Item -LiteralPath $resolvedStaging -Recurse -Force
    }
}
