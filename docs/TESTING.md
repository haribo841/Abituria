# Plan i raport testów końcowych

Wersja dokumentu: `0.9.0-beta.1`.

Data ostatniego lokalnego wykonania: 5 sierpnia 2026 r., Windows 11 x64, .NET SDK `10.0.302`, Python `3.13.1`, konfiguracja `Release`.

Pełny przebieg `dotnet test Abituria.sln --configuration Release --no-build --no-restore` z raportem OpenCover zakończył się wynikiem `529/529 PASS` w czasie `29 s`. OpenCover wykazał `95,88%` pokrycia linii i `87,70%` pokrycia gałęzi kodu C#. Cztery testy Python generatora PDF przeszły, a `coverage.py` wykazał `99,14%` linii i `93,75%` gałęzi. Wspólna bramka zakończyła się wynikiem `93,60%` pokrycia łącznego i `87,75%` gałęzi, powyżej wymaganych progów `90%` i `85%`.

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
| Systemowe wydania | smoke test wyodrębnionej paczki na natywnych runnerach Windows, Ubuntu i macOS | PASS - [`platform-installation-smoke` dla `e0afeea`](https://github.com/haribo841/Abituria/actions/runs/29646667838) oraz wieloplatformowy workflow publicznego prerelease `v0.9.0-beta.1` |
| Instalacyjne na niezależnych komputerach | workflow na hostowanych natywnych runnerach oraz procedura w `ACCEPTANCE_PROTOCOL.md` | PASS formalny na trzech niezależnych komputerach CI; ręczna sesja człowieka jest opcjonalnym uzupełnieniem |
| Użyteczności i dostępności | automatyczne scenariusze UI, poświadczenie historycznych testów uczestników oraz protokół powtórzenia | PASS RETROSPEKTYWNY dla historycznych uczestników; bieżąca część automatyczna PASS; szczegółowe karty historyczne nie zostały zachowane |
| Akceptacyjne | śledzenie kryteriów w `REQUIREMENTS.md` i protokół końcowego odbioru | historyczny projekt zaakceptowany przez prowadzącego na początku lutego 2022 r.; publiczna publikacja bieżącej wersji ma osobną bramę |

Testy `[AvaloniaFact]` tworzą izolowane aplikacje, dispatchery i renderery Skia. Plik `tests/Abituria.Tests/xunit.runner.json` wyłącza równoległość kolekcji, aby testy UI i bramki wydajności nie konkurowały w tym samym procesie o globalne zasoby renderera. Seryjne wykonanie usuwa zależność wyniku od kolejności klas i zapewnia porównywalne pomiary budżetów.

## Automatyczne zestawy testów

| Obszar | Najważniejsze klasy lub narzędzia | Co chronią |
| --- | --- | --- |
| Kalkulator | `ExpressionCalculatorTests`, `ExpressionCalculatorRobustnessTests`, `CalculatorSessionTests`, `RepeatedEqualsTests`, `QuadraticSolverTests`, `Issue5CalculatorPipTests` | poprawność obliczeń, błędy wejścia, granice, historia, PiP i schowek `Ans` |
| Konta i dane | `AccountServiceTests`, `Issue14RegistrationRegressionTests`, `ReleaseDatabaseCompatibilityTests`, `ExerciseScratchpadSessionTests` | profil gościa, hasła, odzyskiwanie, postęp, preferencję PiP, brudnopis sesji i kompatybilność bazy |
| Treści | `ContentInventoryTests`, `ContentSeparationTests`, `Issue35MathChaptersRegressionTests`, `Formula2023ContentTests`, `MathCourse2023ContentTests`, `Matura2025ContentTests`, `Matura2026ContentTests`, `Matura2026ExtendedContentTests`, `DiagramCatalogTests`, `LegacyImageArchiveTests` | kompletność tablic i kursu Formuły 2023, kontrakty `4/13/73/46/238/357`, matury 2025 `31/35/50` i `12/13/50`, matury 2026 `33/37/50` i `12/13/50`, zachowanie 35 zadań 2021, 76 aktywnych diagramów, zgodność archiwum 75 obrazów, źródła, SHA-256, format JSON i renderowanie treści |
| UI i użyteczność przepływów | `ExerciseAndRoutingCoverageTests`, `GeneralCalculatorViewInteractionTests`, `MainWindowPageCoverageTests`, `ExerciseRandomizerTests`, `AboutViewTests`, `NavigationArchitectureTests`, `Issue4NavigationTests`, `Issue5CalculatorPipTests`, `Matura2026UiTests`, `CompoundAnswerEvaluatorTests` | osiągalne ścieżki użytkownika, pięć arkuszy, agregację 17 tematów, oddzielny postęp, odpowiedzi złożone, wszystkie trasy shella, kontrolowany pojedynczy PiP, wklejanie, losowanie i kontekst zadania |
| Dostępność kontrolek | `AccessibilityRegressionTests` | nazwy pól i symbolicznych przycisków oraz dynamiczne regiony wyników |
| Wizualne | `Discussion10VisualRegressionTests` | renderowanie list matematycznych i zachowanie przy minimalnym rozmiarze okna |
| Styl, motywy i własny chrome | `Discussion49StyleRegressionTests` | Mulish, brak wymuszonego Light i Inter, historyczne emoji `🍓`/`🍋`/`🍏`, tooltipy, cztery ustawienia motywu, stany interakcji, fokus, breakpointy, dialogi, sterowanie i skalowanie okna |
| Koszt renderowania UI | `Discussion49StyleRegressionTests` | rozgrzany render reprezentatywnego widoku w motywie jasnym, ciemnym i wysokiego kontrastu oraz budżet czasu i pamięci |
| Wydanie | `ReleaseRuntimeTests`, `ReleaseContractTests`, `ReleaseValidationScriptTests`, `NuGetLicenseBundleTests` | izolowany smoke test, wersjonowanie, zawartość paczek, dowody licencji i działanie bramki pokrycia |
| Python i PDF | `tests/python/test_new_commission_pdf.py` | generowanie, walidację struktury oraz błędy czcionek i obrazu wejściowego |
| Jakość | `Test-CoverageThreshold.ps1`, `dotnet format`, audyt NuGet, test pochodzenia zasobów, SonarQube Cloud, CodeQL | minimalne pokrycie `90%`/`85%`, formatowanie, podatności, kompletność manifestu, jakość kodu i code scanning |

## Natywny smoke test Issue #5

30 lipca 2026 r. na Windows 11 x64 uruchomiono rzeczywistą aplikację z kompilacji Release. Profil gościa przeszedł migrację, PiP otworzył się jako pojedyncze okno należące do Abiturii, obliczenie `7*6` pokazało `42` i status udanego kopiowania, a `Ctrl+V` wkleił dokładnie `42` do brudnopisu zadania. Zmiana trybu na panel aplikacji i z powrotem do okna zachowała wyrażenie `7*6` i wynik. Po teście przywrócono ustawienie „Nad Abiturią” i zamknięto oba hosty.

31 lipca 2026 r., po refaktorze usuwającym zgłoszenia Sonara, powtórzono na Windows 11 x64 przepływ rzeczywistego schowka. Kalkulator ogólny obliczył `7*6`, wyświetlił `42` i komunikat „Ans skopiowano do schowka.”, a `Ctrl+V` wkleił dokładnie `42` do brudnopisu Zadania 1. Nie wybrano odpowiedzi i nie zmieniono postępu profilu.

Natywne zachowanie schowka oraz właściwości `Owned` i `Topmost` na Ubuntu 24.04 i macOS 15 pozostaje do potwierdzenia na odpowiednich systemach po autoryzowanym pushu. Testy Avalonia Headless sprawdzają wspólną logikę i wszystkie trzy tryby, ale nie są przedstawiane jako zamiennik tej kontroli platformowej.

## Natywny smoke test Issue #6

31 lipca 2026 r. na Windows 11 x64 uruchomiono rzeczywistą aplikację z kompilacji Release. Pasek pokazał po lewej kolorowe `🍓`, `🍋`, `🍏`, wyśrodkowane `🍀 Abituria` i przycisk motywu po prawej. Najechanie na każdą z czterech kontrolek wyświetliło po krótkim opóźnieniu tooltip: „Zamknij”, „Maksymalizuj”, „Minimalizuj” oraz opis zmiany motywu. `🍋` zmaksymalizował okno, zachował symbol i zmienił tooltip oraz nazwę automatyzacji na „Przywróć”, drugie użycie przywróciło zwykły rozmiar, `🍏` zminimalizował okno, a `🍓` zakończył aplikację.

## Regresje stylu i dostępności dyskusji #49

Nowy zestaw testów nie ocenia wyglądu wyłącznie przez obecność nazw klas. Łączy kontrakty źródłowe z uruchomieniem Avalonia Headless:

| Kontrola | Dowód |
| --- | --- |
| Font | projekt produkcyjny i jego lockfile wskazują paczkowany Mulish oraz nie zawierają `Avalonia.Fonts.Inter` ani `WithInterFont`; `Avalonia.Fonts.Inter` pozostaje wyłącznie przechodnią zależnością testowego `Avalonia.Headless` i nie trafia do grafu ani paczki produkcyjnej |
| Motyw domyślny | aplikacja nie wymusza `RequestedThemeVariant="Light"`; wariant systemowy pozostaje rzeczywistym ustawieniem domyślnym |
| Palety | reprezentatywny widok renderuje się w ustawieniu jasnym, ciemnym i wysokiego kontrastu, a obrazy różnią się zgodnie z paletą |
| Kontrast | najważniejsze pary tekst-tło, granica-tło i fokus-tło są liczone według WCAG i mieszczą się w progach `4,5:1` lub `3:1` |
| Znak aplikacji i placeholdery | w trybie ciemnym i wysokiego kontrastu placeholder ma kontrastowy `TextMutedBrush`, a tekstowa koniczyna korzysta z kolorów bieżącego motywu |
| Stany interakcji | style zawierają i stosują `:pointerover`, `:pressed`, `:focus` oraz `:focus-visible` dla kontrolek używanych przez aplikację |
| Widoczny fokus | render kontrolki przed i po fokusie klawiaturowym ma mierzalną zmianę ramki, a fokus nie jest sygnalizowany wyłącznie kolorem tekstu |
| Własny chrome | `WindowDecorations=None`, historyczna kolejność `🍓`/`🍋`/`🍏`, wyśrodkowana marka, tooltipy `250 ms`, niezmienne `🍋` przy maksymalizacji i przywróceniu, zmiana `WindowState` oraz osiem krawędzi `BeginResizeDrag` pozostają obecne; render przy `720x520`, `960x640` i `1280x820` nie przepełnia paska w żadnym motywie |
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
python -m pip install --requirement tools/requirements-test.txt
python -m pip check
dotnet test Abituria.sln --configuration Release --no-build --no-restore `
  --results-directory artifacts/coverage/csharp `
  --collect:"XPlat Code Coverage" `
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
python -m coverage run --branch --source=tools -m unittest discover -s tests/python -p "test_*.py"
python -m coverage xml -o artifacts/coverage/python-coverage.xml
$openCoverReport = Get-ChildItem artifacts/coverage/csharp -Recurse -Filter coverage.opencover.xml
pwsh -NoProfile -File tools/release/Test-CoverageThreshold.ps1 `
  -OpenCoverReport $openCoverReport.FullName `
  -PythonCoverageReport artifacts/coverage/python-coverage.xml
dotnet format Abituria.sln whitespace --verify-no-changes --no-restore
dotnet format Abituria.sln analyzers --verify-no-changes --no-restore --severity info
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

Publiczne wydanie `0.9.0-beta.1` jest osobnym procesem. Zwykły walidator proweniencji i brama `-RequireReleaseEligible` przeszły po rozszerzeniach `ASSET_RIGHTS_DECLARATION.md` z 3 i 5 sierpnia 2026 r.; manifest ma `releaseEligible=true` i nie zawiera grup `blocked`. Oba warianty walidatora ponowiono na dokładnym commicie wydania.

Publiczny prerelease [`v0.9.0-beta.1`](https://github.com/haribo841/Abituria/releases/tag/v0.9.0-beta.1) został opublikowany po wykonaniu następujących czynności:

1. ponowiono pełne bramy na dokładnym commicie wydania;
2. utworzono tag i uruchomiono workflow wydania;
3. skontrolowano wygenerowany draft, sumy, SBOM i atestacje;
4. zweryfikowano artefakty i instalację zgodnie z `RELEASE_PROCESS.md`;
5. opublikowano prerelease.
