# Abituria

Abituria to działająca offline aplikacja edukacyjna w AvaloniaUI i .NET 9. Projekt konsoliduje funkcje oraz treści ze starszych wersji WPF w jednym, przenośnym kodzie.

## Funkcje

- lokalne konta chronione hasłem oraz profile gościa importowane ze starego `users.txt`,
- jednorazowe kody odzyskiwania i postęp zapisywany osobno dla każdego profilu,
- 18 pełnych tablic matematycznych,
- rozbudowany dział „Wektory” wraz z ilustracjami,
- 35 zadań z matury poprawkowej 2021,
- zgodność arkusza i klucza odpowiedzi z dokumentami CKE `EMAP-P0-100-2108`,
- przeglądanie zadań według arkusza lub 17 tematów oraz sesyjny brudnopis,
- automatyczne sprawdzanie odpowiedzi A–D w zadaniach 1–28,
- prowadzenie przez podpowiedzi i ujawnienie odpowiedzi w zadaniach 29–35,
- kalkulator ogólny z nawiasami, potęgami, pierwiastkami, notacją naukową, Ans i historią sesji,
- kalkulator funkcji kwadratowej z postacią ogólną, kanoniczną i iloczynową,
- zachowane ekrany informujące o materiałach, które w starym projekcie były tylko placeholderami,
- plan rozwoju rozdzielający elementy przeniesione, zaplanowane i zastąpione.

## Uruchomienie

Wymagany jest .NET SDK 9.

```powershell
dotnet restore Abituria.sln --configfile NuGet.Config
dotnet build Abituria.sln --no-restore
dotnet run --project Abituria.csproj
```

Testy:

```powershell
dotnet test Abituria.sln --no-restore
```

Szczegółowe pokrycie regresji kalkulatora opisuje [macierz testów kalkulatora](docs/CALCULATOR_TEST_MATRIX.md). Testy Avalonia Headless sprawdzają również przewijanie, przepływ inline, linię bazową i obrazy wzorcowe list matematycznych.

## Dane lokalne

Baza SQLite jest tworzona w katalogu zwracanym przez `Environment.SpecialFolder.LocalApplicationData`:

```text
Abituria/abituria.db
```

Hasła nie są przechowywane w postaci jawnej. Aplikacja używa PBKDF2-HMAC-SHA256, osobnej soli dla każdego konta i wersjonowanej liczby iteracji. Kod odzyskiwania jest wyświetlany tylko raz, a w bazie pozostaje jego skrót.

Przy pierwszym uruchomieniu aplikacja odczytuje istniejący plik:

```text
%APPDATA%/Abituria/users.txt
```

Nazwy są importowane jako profile gościa. Plik źródłowy nie jest usuwany, a import jest idempotentny. Gdy po imporcie baza nie zawiera żadnego profilu, aplikacja tworzy domyślnego gościa `Maturzysta`.

## Treści

Treści edukacyjne i dłuższe statyczne opisy interfejsu są wersjonowane w katalogu `Content` jako JSON. Są renderowane przez generyczne widoki Avalonia, a wzory przez `Sylinko.CSharpMath.Avalonia`. Kod produkcyjny nie zawiera długich opisów ani wzorów. Zasady edycji i podglądu opisuje [dokumentacja treści](docs/CONTENT_AUTHORING.md).

Importer `tools/Import-LegacyContent.ps1` pozwala ponownie wygenerować zasoby z jawnie wskazanego archiwalnego snapshotu. Snapshot nie jest częścią aktywnego repozytorium:

```powershell
powershell -ExecutionPolicy Bypass -File tools/Import-LegacyContent.ps1 `
  -SourceRoot "C:\ścieżka\do\Projekt-Inzynierski-master"
```

Importer poprawia wyłącznie rozpoznane literówki i składnię blokującą renderowanie LaTeX. Wynik jest sprawdzany testami inwentarza.

Końcowy audyt migracji i kryteria pozwalające usunąć stare snapshoty WPF są zapisane w [inwentarzu migracji](docs/MIGRATION_INVENTORY.md).

Opcjonalna analiza SonarQube Cloud jest przygotowana w osobnym workflow. Aktywacja wymaga utworzenia rzeczywistego projektu oraz ustawienia zmiennych i sekretu zgodnie z [instrukcją SonarQube](docs/SONARQUBE.md).

## Architektura

Formalny opis aktualnej architektury i diagram komponentów są w [dokumentacji architektury](docs/ARCHITECTURE.md).

- `AvaloniaApp/Models` - kontrakty treści i profili,
- `AvaloniaApp/Data` - SQLite i migracje EF Core,
- `AvaloniaApp/Services` - konta, bezpieczeństwo, treści i obliczenia,
- `AvaloniaApp/ViewModels` - sesja i nawigacja,
- `AvaloniaApp/Views` - oddzielne ekrany Avalonia,
- `tests/Abituria.Tests` - testy inwentarza, kont i kalkulatora.

Szczegóły konsolidacji starszych wersji opisuje [inwentarz migracji](docs/MIGRATION_INVENTORY.md), a dalsze pozycje [plan rozwoju](docs/ROADMAP.md). Oryginalne dokumenty planistyczne oraz historyczna licencja zostały zachowane bajt w bajt w [archiwum legacy](docs/legacy/README.md).
