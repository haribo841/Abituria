# Plan i raport testów końcowych

Wersja dokumentu: `0.9.0-beta.1`.

Data ostatniego lokalnego wykonania: 20 lipca 2026 r., Windows 11 x64, .NET SDK `10.0.302`, konfiguracja `Release`.

Pełny przebieg `dotnet test Abituria.sln --configuration Release --no-build --no-restore` zakończył się wynikiem `424/424 PASS` w czasie `10,4610 s`. Zestaw obejmuje testy stylu, motywów, widocznego fokusu, breakpointów, własnego chrome, dialogów i kosztu renderowania dyskusji #49.

Dokument rozróżnia wyniki automatyczne, retrospektywne poświadczenie historycznych testów uczestników oraz czynności bieżącego procesu wydawniczego. Brak szczegółowej karty sesji nie jest uzupełniany przez domysł, a poświadczenie historyczne nie jest przedstawiane jako test bieżącej paczki.

## Cel i zakres

Celem testów końcowych jest potwierdzenie, że Abituria spełnia aktualny zakres funkcjonalny, poprawnie reaguje na błędne dane, nie regresuje przy zmianach oraz mieści się w ustalonych, przenośnych budżetach czasu i pamięci. Zakres obejmuje aplikację .NET/Avalonia, lokalną bazę SQLite, treści JSON, proces wydania i dokumentację.

| Rodzaj testu | Dowód automatyczny | Status |
| --- | --- | --- |
| Jednostkowe | parser i kalkulator, stan wejścia, historia, funkcja kwadratowa, hasła, losowanie zadań | wykonywane w `dotnet test` |
| Integracyjne | `AccountService` z SQLite, migracja bazy .NET 9 do .NET 10, repozytorium treści | wykonywane w `dotnet test` |
| Funkcjonalne i nawigacyjne | Avalonia Headless: logowanie, routing, zadania, podpowiedzi, odpowiedzi i postęp | wykonywane w `dotnet test` |
| Regresyjne i wizualne | klasy `*RegressionTests`, obrazy wzorcowe treści oraz render motywów i fokusu | wykonywane w `dotnet test` |
| Wydajnościowe, pamięciowe i obciążeniowe | `PerformanceMemoryAndLoadTests` | wykonane lokalnie w konfiguracji Release |
| Systemowe wydania | smoke test wyodrębnionej paczki na natywnych runnerach Windows, Ubuntu i macOS | PASS - [`platform-installation-smoke` dla `e0afeea`](https://github.com/haribo841/Abituria/actions/runs/29646667838); proweniencja PASS, ale publiczne wydanie nie zostało wykonane |
| Instalacyjne na niezależnych komputerach | workflow na hostowanych natywnych runnerach oraz procedura w `ACCEPTANCE_PROTOCOL.md` | PASS formalny na trzech niezależnych komputerach CI; ręczna sesja człowieka jest opcjonalnym uzupełnieniem |
| Użyteczności i dostępności | automatyczne scenariusze UI, poświadczenie historycznych testów uczestników oraz protokół powtórzenia | PASS RETROSPEKTYWNY dla historycznych uczestników; bieżąca część automatyczna PASS; szczegółowe karty historyczne nie zostały zachowane |
| Akceptacyjne | śledzenie kryteriów w `REQUIREMENTS.md` i protokół końcowego odbioru | historyczny projekt zaakceptowany przez prowadzącego na początku lutego 2022 r.; publiczna publikacja bieżącej wersji ma osobną bramę |

## Automatyczne zestawy testów

| Obszar | Najważniejsze klasy lub narzędzia | Co chronią |
| --- | --- | --- |
| Kalkulator | `ExpressionCalculatorTests`, `ExpressionCalculatorRobustnessTests`, `CalculatorSessionTests`, `RepeatedEqualsTests`, `QuadraticSolverTests` | poprawność obliczeń, błędy wejścia, granice i historia |
| Konta i dane | `AccountServiceTests`, `Issue14RegistrationRegressionTests`, `ReleaseDatabaseCompatibilityTests` | profil gościa, hasła, odzyskiwanie, postęp i kompatybilność bazy |
| Treści | `ContentInventoryTests`, `ContentSeparationTests`, `Issue35MathChaptersRegressionTests` | kompletność katalogu, format JSON i renderowanie treści |
| UI i użyteczność przepływów | `ExerciseAndRoutingCoverageTests`, `ExerciseRandomizerTests`, `AboutViewTests`, `NavigationArchitectureTests` | osiągalne ścieżki użytkownika, pojedyncze okno, losowanie i kontekst zadania |
| Dostępność kontrolek | `AccessibilityRegressionTests` | nazwy pól i symbolicznych przycisków oraz dynamiczne regiony wyników |
| Wizualne | `Discussion10VisualRegressionTests` | renderowanie list matematycznych i zachowanie przy minimalnym rozmiarze okna |
| Styl, motywy i własny chrome | `Discussion49StyleRegressionTests` | Mulish, brak wymuszonego Light i Inter, cztery ustawienia motywu, stany interakcji, fokus, breakpointy, dialogi, sterowanie i skalowanie okna |
| Koszt renderowania UI | `Discussion49StyleRegressionTests` | rozgrzany render reprezentatywnego widoku w motywie jasnym, ciemnym i wysokiego kontrastu oraz budżet czasu i pamięci |
| Wydanie | `ReleaseRuntimeTests`, `ReleaseContractTests`, `ReleaseValidationScriptTests`, `NuGetLicenseBundleTests` | izolowany smoke test, wersjonowanie, zawartość paczek i dowody licencji |
| Jakość | `dotnet format`, audyt NuGet, test pochodzenia zasobów, SonarQube Cloud | formatowanie, podatności, kompletność manifestu i analiza statyczna |

## Regresje stylu i dostępności dyskusji #49

Nowy zestaw testów nie ocenia wyglądu wyłącznie przez obecność nazw klas. Łączy kontrakty źródłowe z uruchomieniem Avalonia Headless:

| Kontrola | Dowód |
| --- | --- |
| Font | projekt produkcyjny i jego lockfile wskazują paczkowany Mulish oraz nie zawierają `Avalonia.Fonts.Inter` ani `WithInterFont`; `Avalonia.Fonts.Inter` pozostaje wyłącznie przechodnią zależnością testowego `Avalonia.Headless` i nie trafia do grafu ani paczki produkcyjnej |
| Motyw domyślny | aplikacja nie wymusza `RequestedThemeVariant="Light"`; wariant systemowy pozostaje rzeczywistym ustawieniem domyślnym |
| Palety | reprezentatywny widok renderuje się w ustawieniu jasnym, ciemnym i wysokiego kontrastu, a obrazy różnią się zgodnie z paletą |
| Kontrast | najważniejsze pary tekst-tło, granica-tło i fokus-tło są liczone według WCAG i mieszczą się w progach `4,5:1` lub `3:1` |
| Logo i placeholdery | w trybie ciemnym i wysokiego kontrastu placeholder ma kontrastowy `TextMutedBrush`, a rastrowe logo z czarnym napisem pozostaje na białej powierzchni |
| Stany interakcji | style zawierają i stosują `:pointerover`, `:pressed`, `:focus` oraz `:focus-visible` dla kontrolek używanych przez aplikację |
| Widoczny fokus | render kontrolki przed i po fokusie klawiaturowym ma mierzalną zmianę ramki, a fokus nie jest sygnalizowany wyłącznie kolorem tekstu |
| Własny chrome | `WindowDecorations=None`, obszar przeciągania, przyciski okna, zmiana `WindowState` oraz osiem krawędzi `BeginResizeDrag` pozostają obecne |
| Breakpointy | Login przy `860`, Start przy `780` i kalkulator przy `900` zmieniają liczbę kolumn oraz położenie paneli bez zmiany drzewa logicznego |
| Dialogi | wspólna fabryka ustawia `CanResize=true`, granice wymiarów, środkowanie względem właściciela i pionowe przewijanie |
| Wpływ stylu | rozgrzany render i nawigacja w trzech paletach mieszczą się w szerokim budżecie regresyjnym czasu i pamięci |

Testy headless nie zastępują natywnego drzewa UI Automation, czytnika ekranu, systemowego skalowania ani ręcznego pomiaru celu wskaźnika. Pełna macierz i osobna checklista manualna znajdują się w `ACCESSIBILITY_WCAG_AUDIT.md`.

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

Przebieg [`29646667838`](https://github.com/haribo841/Abituria/actions/runs/29646667838) z 18 lipca 2026 r. dla commitu `e0afeeaee30ed700fa5b8dc873409c23081106d4` zakończył się powodzeniem na `windows-2025`, `ubuntu-24.04` i `macos-15-intel`. Każdy job zbudował aplikację self-contained, skopiował ją przez archiwum do nowego katalogu i uruchomił izolowany smoke test.

To jest formalny dowód instalacji na komputerach niezależnych od środowiska budowania autora: każdy system działa na osobnym, hostowanym natywnym runnerze i uruchamia plik z ponownie rozpakowanego archiwum. Opcjonalny formularz dodatkowej sesji prowadzonej przez człowieka znajduje się w `ACCEPTANCE_PROTOCOL.md`, ale jego niewypełnienie nie zmienia wyniku trzech niezależnych testów systemowych ani nie blokuje Issue #43.

## Testy użyteczności i dostępności

Właściciel projektu poświadczył 19 lipca 2026 r., że historyczne testy użyteczności z uczestnikami przeprowadzono i zakończyły się powodzeniem. Projekt został następnie zaakceptowany przez prowadzącego na początku lutego 2022 r. Nie zachowano liczby i danych uczestników, terminów sesji, wyników poszczególnych scenariuszy, komentarzy ani powiązania zgłoszeń z poprawkami. `USABILITY_TEST_RESULTS.md` zapisuje wynik zbiorczy jako `PASS RETROSPEKTYWNY` i jawnie ujawnia te ograniczenia.

Automatyczny zakres bieżącej wersji obejmuje główne przepływy użytkownika, komunikaty błędów kalkulatora, minimalny rozmiar `720x520`, breakpointy `860`, `780` i `900`, pionowe przewijanie historii, widoczne etykiety działań, nazwy automatyzacji, dynamiczne regiony wyników oraz losowanie z całej puli i z tematu. Techniczny przegląd z 19 lipca 2026 r. i poprawki H-01-H-03 opisuje `USABILITY_TEST_RESULTS.md`. Poprawki te wynikają z późniejszego przeglądu heurystycznego i nie są przypisywane historycznym uczestnikom.

Pełny przegląd wszystkich kryteriów WCAG 2.2 A/AA bieżącej wersji znajduje się w `ACCESSIBILITY_WCAG_AUDIT.md`. Dokument nie przedstawia wyniku retrospektywnych testów uczestników ani automatycznych testów headless jako certyfikatu WCAG. Powtarzalny sposób nowej sesji użyteczności opisuje `USABILITY_TEST_PROTOCOL.md`, a audyt dostępności zawiera osobną checklistę technologii asystujących, skalowania i alternatywnych metod wejścia.

## Kryterium zaliczenia

Wynik techniczny bieżącej wersji jest pozytywny, gdy wszystkie automatyczne bramy przechodzą na aktualnym commicie, a wyniki mieszczą się w budżetach.

Historyczny odbiór Issue #43 jest zaliczony na podstawie łącznie udokumentowanych faktów:

1. właściciel poświadczył pozytywny wynik testów użyteczności z uczestnikami;
2. prowadzący zaakceptował projekt na początku lutego 2022 r.;
3. historyczna forma przekazania została uzgodniona i przyjęta;
4. brakujące szczegóły sesji, podpisu, kanału i hasha są ujawnione jako ograniczenia archiwalne, a nie uzupełnione fikcyjnymi danymi.

Publiczne wydanie `0.9.0-beta.1` jest osobnym procesem. Warunki praw i proweniencji są obecnie spełnione: deklarację zapisano w `ASSET_RIGHTS_DECLARATION.md`, wszystkie grupy mają status `approved`, `releaseEligible=true`, a oba warianty walidatora przeszły 19 lipca 2026 r.

Do wykonania pozostają:

1. ponowienie pełnych bram na dokładnym commicie wydania;
2. utworzenie tagu i uruchomienie workflow wydania;
3. kontrola wygenerowanego draftu, sum, SBOM i atestacji;
4. ręczna kontrola artefaktów i instalacji zgodnie z `RELEASE_PROCESS.md`;
5. opublikowanie prerelease.

Raport nie twierdzi, że GitHub Release już istnieje.
