param(
    [switch]$Verify
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$productionLock = Join-Path $root "packages.lock.json"
$testLock = Join-Path $root "tests/Abituria.Tests/packages.lock.json"
[xml]$buildProperties = Get-Content -Raw -Encoding UTF8 (Join-Path $root "Directory.Build.props")
$releaseVersion = [string]$buildProperties.Project.PropertyGroup.Version
if ([string]::IsNullOrWhiteSpace($releaseVersion)) { throw "Directory.Build.props nie zawiera wersji." }
$runtimeFrameworkVersion = [string]$buildProperties.Project.PropertyGroup.RuntimeFrameworkVersion
if ([string]::IsNullOrWhiteSpace($runtimeFrameworkVersion)) {
    throw "Directory.Build.props nie zawiera wersji środowiska uruchomieniowego."
}

$purposes = @{
    "Avalonia" = "Podstawowy framework interfejsu desktopowego."
    "Avalonia.Desktop" = "Klasyczny cykl życia aplikacji desktopowej i backendy systemowe."
    "Avalonia.Fonts.Inter" = "Domyślna czcionka interfejsu Avalonia."
    "Avalonia.Themes.Fluent" = "Motyw i kontrolki Fluent."
    "CommunityToolkit.Mvvm" = "Elementy modelu MVVM."
    "Microsoft.EntityFrameworkCore.Sqlite" = "Trwałe profile i postęp w lokalnej bazie SQLite."
    "Microsoft.Extensions.DependencyInjection" = "Kontener usług aplikacji."
    "SQLitePCLRaw.bundle_e_sqlite3" = "Przypięta, poprawiona biblioteka natywna SQLite."
    "Sylinko.CSharpMath.Avalonia" = "Renderowanie wzorów matematycznych."
    "Avalonia.Headless.XUnit" = "Testy interfejsu bez ekranu."
    "coverlet.collector" = "Pomiar pokrycia testów."
    "Microsoft.NET.Test.Sdk" = "Host i integracja uruchamiania testów .NET."
    "xunit.v3" = "Framework testów automatycznych."
    "xunit.runner.visualstudio" = "Adapter testów dla dotnet test i Visual Studio."
}

function Read-LockPackages([string]$path, [string]$scope) {
    $document = Get-Content -Raw -Encoding UTF8 $path | ConvertFrom-Json
    foreach ($target in $document.dependencies.PSObject.Properties) {
        foreach ($property in $target.Value.PSObject.Properties) {
            $dependency = $property.Value
            if ($dependency.type -eq "Project") { continue }
            [pscustomobject]@{
                Id = $property.Name
                Version = [string]$dependency.resolved
                Type = [string]$dependency.type
                Scope = $scope
            }
        }
    }
}

function Get-License([string]$id, [string]$version) {
    $packageRoot = if ($env:NUGET_PACKAGES) {
        $env:NUGET_PACKAGES
    } else {
        Join-Path ([Environment]::GetFolderPath([Environment+SpecialFolder]::UserProfile)) ".nuget/packages"
    }
    $directory = Join-Path $packageRoot "$($id.ToLowerInvariant())/$version"
    $nuspec = Get-ChildItem -LiteralPath $directory -Filter "*.nuspec" -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $nuspec) { throw "Brak pliku nuspec dla $id $version. Najpierw wykonaj dotnet restore." }
    [xml]$xml = Get-Content -Raw -Encoding UTF8 $nuspec.FullName
    $metadata = $xml.package.metadata
    if ($metadata.license) {
        $value = [string]$metadata.license.InnerText
        if ($metadata.license.type -eq "file") {
            $licensePath = Join-Path $directory $value
            if (-not (Test-Path -LiteralPath $licensePath -PathType Leaf)) {
                throw "Pakiet $id $version wskazuje brakujący plik licencji $value."
            }
            return "$value (plik pakietu)"
        }
        return $value
    }
    if ($metadata.licenseUrl) { return [string]$metadata.licenseUrl }
    throw "Pakiet $id $version nie deklaruje licencji w nuspec."
}

function Escape-Markdown([string]$value) {
    return $value.Replace("|", "\|").Replace("`r", " ").Replace("`n", " ")
}

$all = @(
    Read-LockPackages $productionLock "produkcyjna"
    Read-LockPackages $testLock "testowa"
)

$packages = foreach ($group in ($all | Group-Object Id, Version | Sort-Object { $_.Group[0].Id })) {
    $sample = $group.Group[0]
    $scopes = @($group.Group.Scope | Sort-Object -Unique)
    $types = @($group.Group.Type | Sort-Object -Unique)
    [pscustomobject]@{
        Id = $sample.Id
        Version = $sample.Version
        License = Get-License $sample.Id $sample.Version
        Scope = if ($scopes.Count -eq 2) { "produkcyjna i testowa" } else { $scopes[0] }
        Direct = $types -contains "Direct"
    }
}

$direct = @($packages | Where-Object Direct | Sort-Object Id)
$resolved = @($packages | Sort-Object Id)
$production = @($packages | Where-Object { $_.Scope -ne "testowa" } | Sort-Object Id)

$dependencyLines = [System.Collections.Generic.List[string]]::new()
$dependencyLines.Add("# Zależności Abiturii")
$dependencyLines.Add("")
$dependencyLines.Add("Ten dokument jest generowany przez ``tools/Generate-DependencyDocumentation.ps1`` z przypiętych plików ``packages.lock.json``. Nie należy edytować tabel ręcznie.")
$dependencyLines.Add("")
$dependencyLines.Add("## Zależności bezpośrednie")
$dependencyLines.Add("")
$dependencyLines.Add("| Pakiet | Wersja | Zakres | Zastosowanie | Licencja |")
$dependencyLines.Add("| --- | --- | --- | --- | --- |")
foreach ($package in $direct) {
    $purpose = if ($purposes.ContainsKey($package.Id)) { $purposes[$package.Id] } else { "Bezpośrednia zależność projektu." }
    $dependencyLines.Add("| ``$(Escape-Markdown $package.Id)`` | ``$(Escape-Markdown $package.Version)`` | $(Escape-Markdown $package.Scope) | $(Escape-Markdown $purpose) | $(Escape-Markdown $package.License) |")
}
$dependencyLines.Add("")
$dependencyLines.Add("## Pełne rozwiązanie zależności")
$dependencyLines.Add("")
$dependencyLines.Add("Tabela obejmuje również zależności przechodnie. Dokładne grafy dla każdego targetu pozostają w lockfile, a każde wydanie otrzymuje osobny SBOM SPDX.")
$dependencyLines.Add("")
$dependencyLines.Add("| Pakiet | Wersja | Zakres | Typ | Licencja |")
$dependencyLines.Add("| --- | --- | --- | --- | --- |")
foreach ($package in $resolved) {
    $dependencyLines.Add("| ``$(Escape-Markdown $package.Id)`` | ``$(Escape-Markdown $package.Version)`` | $(Escape-Markdown $package.Scope) | $(if ($package.Direct) { 'bezpośrednia' } else { 'przechodnia' }) | $(Escape-Markdown $package.License) |")
}
$dependencyLines.Add("")
$dependencyLines.Add("## Środowisko dołączane do paczek self-contained")
$dependencyLines.Add("")
$dependencyLines.Add("Każda paczka produkcyjna zawiera ``Microsoft.NETCore.App.Runtime.<rid>`` oraz kod apphost z ``Microsoft.NETCore.App.Host.<rid>`` w wersji ``$runtimeFrameworkVersion``. Konkretny RID i dokładny zbiór komponentów są wyprowadzane z opublikowanego ``Abituria.deps.json`` i rejestrowane w osobnym SBOM SPDX 2.2.")
$dependencyLines.Add("")
$dependencyLines.Add("## Weryfikacja")
$dependencyLines.Add("")
$dependencyLines.Add('```powershell')
$dependencyLines.Add("dotnet restore Abituria.sln --configfile NuGet.Config --locked-mode")
$dependencyLines.Add("dotnet list Abituria.sln package --vulnerable --include-transitive")
$dependencyLines.Add("powershell -File tools/Generate-DependencyDocumentation.ps1 -Verify")
$dependencyLines.Add('```')
$dependencyText = ($dependencyLines -join "`n") + "`n"

$noticeLines = [System.Collections.Generic.List[string]]::new()
$noticeLines.Add("# Informacje o komponentach zewnętrznych")
$noticeLines.Add("")
$noticeLines.Add("Abituria jest udostępniana na licencji MIT. Poniższe komponenty zachowują własne licencje i prawa autorskie. Dokładne wersje odpowiadają ``packages.lock.json`` dla wydania ``$releaseVersion``.")
$noticeLines.Add("")
$noticeLines.Add("## Pakiety dołączane do aplikacji")
$noticeLines.Add("")
$noticeLines.Add("| Pakiet | Wersja | Licencja | Źródło |")
$noticeLines.Add("| --- | --- | --- | --- |")
foreach ($package in $production) {
    $url = "https://www.nuget.org/packages/$($package.Id)/$($package.Version)"
    $noticeLines.Add("| ``$(Escape-Markdown $package.Id)`` | ``$(Escape-Markdown $package.Version)`` | $(Escape-Markdown $package.License) | [NuGet]($url) |")
}
$noticeLines.Add("")
$noticeLines.Add("Pakiety testowe nie są częścią binarnej dystrybucji i są wyszczególnione w ``docs/DEPENDENCIES.md`` oraz testowym lockfile.")
$noticeLines.Add("")
$noticeLines.Add("## .NET Runtime")
$noticeLines.Add("")
$noticeLines.Add("Samowystarczalne paczki zawierają .NET Runtime i apphost w wersji ``$runtimeFrameworkVersion`` na licencji MIT. Każda paczka zawiera pliki ``licenses/dotnet-runtime-LICENSE.txt`` oraz ``licenses/dotnet-runtime-THIRD-PARTY-NOTICES.txt`` skopiowane bezpośrednio z odpowiadającego jej pakietu ``Microsoft.NETCore.App.Runtime.<rid>``.")
$noticeLines.Add("")
$noticeLines.Add("## Czcionka Mulish")
$noticeLines.Add("")
$noticeLines.Add("Copyright 2016 The Mulish Project Authors. Czcionka jest udostępniana na SIL Open Font License 1.1. Pełny tekst licencji znajduje się w ``fonts/OFL.txt`` i jest dołączany do każdego wydania.")
$noticeLines.Add("")
$noticeLines.Add("## Kod i treści odziedziczone")
$noticeLines.Add("")
$noticeLines.Add("Fragmenty historycznego projektu zostały udostępnione na licencji MIT jako Copyright (c) 2022 Ich Troje. Oryginalny tekst tej licencji znajduje się w ``docs/legacy/originals/LICENSE-2022-Ich-Troje.txt`` i jest dołączany do każdego wydania.")
$noticeLines.Add("")
$noticeLines.Add("## Treści edukacyjne i grafiki")
$noticeLines.Add("")
$noticeLines.Add("Pochodzenie oraz status redystrybucji treści, fontów i grafik określa ``Content/provenance.json``. Zasób ze statusem ``blocked`` nie może trafić do publicznego wydania.")
$noticeText = ($noticeLines -join "`n") + "`n"

function Write-Or-Verify([string]$relativePath, [string]$content) {
    $path = Join-Path $root $relativePath
    if ($Verify) {
        if (-not (Test-Path -LiteralPath $path)) { throw "Brak wygenerowanego pliku $relativePath." }
        $existing = (Get-Content -Raw -Encoding UTF8 $path).Replace("`r`n", "`n")
        if ($existing -ne $content) { throw "Plik $relativePath jest nieaktualny. Uruchom generator bez -Verify." }
        return
    }
    [IO.File]::WriteAllText($path, $content, [Text.UTF8Encoding]::new($false))
}

Write-Or-Verify "docs/DEPENDENCIES.md" $dependencyText
Write-Or-Verify "THIRD-PARTY-NOTICES.md" $noticeText
Write-Host "Dokumentacja zależności jest aktualna."
