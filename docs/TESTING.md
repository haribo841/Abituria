# Plan i raport testów końcowych

Wersja dokumentu: `0.9.0-beta.1`.

Data ostatniego lokalnego wykonania: 18 lipca 2026 r., Windows 11 x64, .NET SDK `10.0.302`, konfiguracja `Release`.

Ostatni pełny przebieg `dotnet test Abituria.sln --configuration Release`: `406/406` zaliczonych, `0` pominiętych i `0` błędów.

Dokument rozróżnia wyniki automatyczne od czynności wymagających niezależnego testera lub osoby zatwierdzającej. Brak wpisu w protokole ręcznym nie jest interpretowany jako wynik pozytywny.

## Cel i zakres

Celem testów końcowych jest potwierdzenie, że Abituria spełnia aktualny zakres funkcjonalny, poprawnie reaguje na błędne dane, nie regresuje przy zmianach oraz mieści się w ustalonych, przenośnych budżetach czasu i pamięci. Zakres obejmuje aplikację .NET/Avalonia, lokalną bazę SQLite, treści JSON, proces wydania i dokumentację.

| Rodzaj testu | Dowód automatyczny | Status |
| --- | --- | --- |
| Jednostkowe | parser i kalkulator, stan wejścia, historia, funkcja kwadratowa, hasła, losowanie zadań | wykonywane w `dotnet test` |
| Integracyjne | `AccountService` z SQLite, migracja bazy .NET 9 do .NET 10, repozytorium treści | wykonywane w `dotnet test` |
| Funkcjonalne i nawigacyjne | Avalonia Headless: logowanie, routing, zadania, podpowiedzi, odpowiedzi i postęp | wykonywane w `dotnet test` |
| Regresyjne i wizualne | klasy `*RegressionTests`, dwa obrazy wzorcowe dla `960x640` i `1280x820` | wykonywane w `dotnet test` |
| Wydajnościowe, pamięciowe i obciążeniowe | `PerformanceMemoryAndLoadTests` | wykonane lokalnie w konfiguracji Release |
| Systemowe wydania | smoke test wyodrębnionej paczki na natywnych runnerach Windows, Ubuntu i macOS | PASS - [`platform-installation-smoke` dla `4fdecd2`](https://github.com/haribo841/Abituria/actions/runs/29646454681); publiczne wydanie nadal wymaga bramy prawnej |
| Instalacyjne na niezależnych komputerach | procedura i arkusz w `ACCEPTANCE_PROTOCOL.md` | PASS automatyczny na trzech niezależnych runnerach; ręczny odbiór przez niezależną osobę pozostaje wymagany |
| Użyteczności i dostępności | automatyczne scenariusze UI oraz protokół ręczny | automatyczna część wykonana, badanie z użytkownikami oczekuje na wykonanie |
| Akceptacyjne | śledzenie kryteriów w `REQUIREMENTS.md` i protokół końcowego odbioru | techniczne dowody wykonane; podpis i publikacja mają osobne bramy |

## Automatyczne zestawy testów

| Obszar | Najważniejsze klasy lub narzędzia | Co chronią |
| --- | --- | --- |
| Kalkulator | `ExpressionCalculatorTests`, `ExpressionCalculatorRobustnessTests`, `CalculatorSessionTests`, `RepeatedEqualsTests`, `QuadraticSolverTests` | poprawność obliczeń, błędy wejścia, granice i historia |
| Konta i dane | `AccountServiceTests`, `Issue14RegistrationRegressionTests`, `ReleaseDatabaseCompatibilityTests` | profil gościa, hasła, odzyskiwanie, postęp i kompatybilność bazy |
| Treści | `ContentInventoryTests`, `ContentSeparationTests`, `Issue35MathChaptersRegressionTests` | kompletność katalogu, format JSON i renderowanie treści |
| UI i użyteczność przepływów | `ExerciseAndRoutingCoverageTests`, `ExerciseRandomizerTests`, `AboutViewTests`, `NavigationArchitectureTests` | osiągalne ścieżki użytkownika, pojedyncze okno, losowanie i kontekst zadania |
| Wizualne | `Discussion10VisualRegressionTests` | renderowanie list matematycznych i zachowanie przy minimalnym rozmiarze okna |
| Wydanie | `ReleaseRuntimeTests`, `ReleaseContractTests`, `ReleaseValidationScriptTests`, `NuGetLicenseBundleTests` | izolowany smoke test, wersjonowanie, zawartość paczek i dowody licencji |
| Jakość | `dotnet format`, audyt NuGet, test pochodzenia zasobów, SonarQube Cloud | formatowanie, podatności, kompletność manifestu i analiza statyczna |

## Wyniki wydajności, pamięci i obciążenia

Test `PerformanceMemoryAndLoadTests` ma szerokie progi, aby wykrywać regresje rzędu wielkości, a nie różnice między runnerami. Dane z wykonania Release 18 lipca 2026 r.:

| Scenariusz | Dane wejściowe | Wynik | Budżet |
| --- | --- | --- | --- |
| Kalkulator mieszany | 30 000 obliczeń | 101,9 ms, 2 264 B na obliczenie | maks. 15 s, 12 KiB na obliczenie |
| Kalkulator równoległy | 40 000 obliczeń | 149,9 ms, 0 błędów | maks. 20 s, 0 błędów |
| Przeładowanie katalogu treści | 20 odczytów katalogu | 17,0 ms, 4 941 208 B alokacji, 227 096 B pamięci zachowanej po pełnym GC | maks. 15 s, 64 MiB alokacji, 32 MiB pamięci zachowanej |
| Historia postępu SQLite | 5 000 rekordów, 3 odczyty | 56,4 ms, 5 211 488 B alokacji, baza 885 616 B | maks. 15 s, 64 MiB alokacji, baza 16 MiB |

Metryki są wypisywane do wyniku xUnit jako `METRIC ...`, dlatego każdy kolejny przebieg pozostawia porównywalny ślad w logu CI.

## Wykonanie lokalne

Podstawowa brama techniczna:

```powershell
dotnet restore Abituria.sln --configfile NuGet.Config --locked-mode
dotnet build Abituria.sln --configuration Release --no-restore
dotnet test Abituria.sln --configuration Release --no-build --no-restore
dotnet format Abituria.sln whitespace --verify-no-changes --no-restore
git diff --check
```

Weryfikacja samych bramek niefunkcjonalnych:

```powershell
dotnet test tests/Abituria.Tests/Abituria.Tests.csproj `
  --configuration Release `
  --filter "FullyQualifiedName~PerformanceMemoryAndLoadTests"
```

Na Windows testy skryptów wydawniczych mogą działać z wbudowanym `powershell.exe` 5.1. Na macOS i Linux wymagany jest `pwsh`. Workflow GitHub używa PowerShell 7 na wszystkich runnerach wydaniowych.

## Testy systemowe i instalacyjne

Workflow `release` buduje paczki self-contained na trzech natywnych runnerach: Windows 11 x64, Ubuntu 24.04 x64 i macOS 15 Intel x64. Każda paczka jest rozpakowywana do katalogu tymczasowego, a następnie jej rzeczywisty plik wykonywalny uruchamia `--release-smoke-test --data-directory <katalog-tymczasowy>`. Test potwierdza wersję, commit, zasoby, bazę SQLite, profil gościa i kalkulator bez otwierania normalnego okna oraz bez użycia danych użytkownika.

Niezależnie od publikacji workflow `platform-installation-smoke` może zostać uruchomiony ręcznie dla aktualnego commita. Na trzech świeżych, natywnych runnerach buduje samowystarczalną aplikację, archiwizuje ją, rozpakowuje do nowego katalogu instalacyjnego i uruchamia smoke test z tego katalogu. Jest to techniczny test instalacyjny na niezależnych komputerach CI, ale nie zastępuje ręcznego testu przez osobę spoza środowiska budowania.

Przebieg [`29646454681`](https://github.com/haribo841/Abituria/actions/runs/29646454681) z 18 lipca 2026 r. dla commitu `4fdecd29111c1896f9bf48fd2bb4d1d9e26771fc` zakończył się powodzeniem na `windows-2025`, `ubuntu-24.04` i `macos-15-intel`. Każdy job zbudował aplikację self-contained, skopiował ją przez archiwum do nowego katalogu i uruchomił izolowany smoke test.

To jest automatyczny dowód zgodności platformowej, ale nie zastępuje ręcznego uruchomienia przez osobę na komputerze niezależnym od środowiska budowania. Formularze, kryteria i miejsca na wyniki takiego testu znajdują się w `ACCEPTANCE_PROTOCOL.md`.

## Testy użyteczności i dostępności

Automatyczny zakres obejmuje główne przepływy użytkownika, komunikaty błędów kalkulatora, minimalny rozmiar `960x640`, pionowe przewijanie historii, widoczne etykiety działań oraz losowanie z całej puli i z tematu. Nie stanowi to deklaracji pełnego audytu WCAG ani badania z użytkownikami. Wymagany sposób ręcznej weryfikacji opisuje `USABILITY_TEST_PROTOCOL.md`.

## Kryterium zaliczenia

Wynik techniczny jest pozytywny, gdy wszystkie automatyczne bramy przechodzą na aktualnym commicie, a wyniki mieszczą się w budżetach. Końcowy odbiór i publiczne wydanie wymagają dodatkowo:

1. wypełnionych testów niezależnej instalacji i użyteczności,
2. podpisu osoby odpowiedzialnej za odbiór,
3. wyniku `releaseEligible=true` dla każdego paczkowanego zasobu,
4. udanego workflow wydania oraz opublikowanego prerelease.

Ostatnie dwa warunki są obecnie blokowane przez źródła i licencje wskazane w `Content/provenance.json`. Nie należy zastępować ich deklaracją w raporcie.
