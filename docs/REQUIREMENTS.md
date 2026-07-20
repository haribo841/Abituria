# Dokument wymagań projektowych Abituria

Status: aktualny dokument wymagań dla bieżącej implementacji AvaloniaUI.

Źródło uzupełnienia: issue #34 "Uzupełnić: Dokument wymagań projektowych", issue #35 "Uzupełnić: Treść działów matematyki", issue #36 "Implementacja i wdrożenie", issue #38 "Analiza biznesowa" oraz aktualny stan repozytorium `haribo841/Abituria`.

Dokument zastępuje pusty historyczny szablon wymagań. Historyczne notatki pozostają w `docs/legacy`, ale nie są już jedyną podstawą oceny zakresu projektu. Uzasadnienie biznesowe, interesariusze, ryzyka, model udostępniania i kamienie milowe opisuje [analiza biznesowa](BUSINESS_ANALYSIS.md).

## 1. Cel projektu

Celem Abiturii jest dostarczenie działającej offline aplikacji desktopowej wspierającej naukę matematyki na poziomie szkoły średniej i przygotowanie do matury. System ma łączyć materiały teoretyczne, zadania, podpowiedzi, odpowiedzi, kalkulatory oraz lokalny zapis profilu użytkownika.

Cele szczegółowe:

- skonsolidować zachowane wersje starego projektu WPF w jednym repozytorium AvaloniaUI,
- udostępnić zweryfikowane treści matematyczne i zadania maturalne,
- zapewnić lokalne konta, profil gościa i zapis postępu,
- zapewnić bezpieczne kalkulatory działające bez usług zewnętrznych,
- oddzielić długie treści edukacyjne od kodu produkcyjnego,
- utrzymać możliwość audytu migracji, testów i jakości kodu.

## 2. Opis systemu

Abituria jest aplikacją desktopową `.NET 10 LTS` z interfejsem AvaloniaUI 12. Aplikacja działa lokalnie i nie wymaga połączenia z internetem do korzystania z podstawowych funkcji.

System składa się z:

- jednego głównego okna `MainWindow`,
- widoków Avalonia `UserControl`,
- modelu nawigacji `AppViewModel`,
- usług kont, treści i kalkulatorów,
- lokalnej bazy SQLite,
- zasobów JSON w katalogu `Content`,
- testów jednostkowych, integracyjnych i headless UI.

Szczegółowy diagram komponentów znajduje się w `docs/ARCHITECTURE.md`.

## 3. Zakres funkcjonalny aplikacji

Zakres bieżącej implementacji obejmuje:

- logowanie profilem gościa,
- rejestrację lokalnego konta chronionego hasłem,
- odzyskiwanie i zmianę hasła,
- import historycznych profili gościa z pliku `users.txt`,
- zapis ukończonych zadań osobno dla profilu,
- ekran startowy z kaflami głównych obszarów,
- 18 tablic matematycznych,
- 7 dostępnych działów: Wektory, Liczby naturalne i indukcja, Alfabet grecki, Liczby rzeczywiste, zbiory i logika, Wyrażenia algebraiczne, równania i nierówności, Funkcja kwadratowa i równanie kwadratowe oraz Logarytmy,
- 35 zadań matury poprawkowej 2021,
- przeglądanie zadań według arkusza i tematów,
- sprawdzanie odpowiedzi w zadaniach zamkniętych,
- ujawnianie odpowiedzi w zadaniach otwartych,
- podpowiedzi krokowe,
- losowanie zadań z całego arkusza albo wybranego tematu,
- sesyjny brudnopis zadania,
- kalkulator funkcji kwadratowej,
- kalkulator ogólny z nawiasami, potęgami, pierwiastkami, notacją naukową, `Ans`, historią i powtarzanym `=`,
- roadmapę odróżniającą funkcje przeniesione, planowane i zastąpione,
- ekran "O programie" oraz diagnostyczny tryb wydaniowy bez otwierania UI,
- samowystarczalne, przenośne paczki x64 dla Windows 11, Ubuntu 24.04 i macOS 15 Intel,
- dokumentację GitHub Pages, sumy SHA-256, osobne SBOM i atestacje pochodzenia artefaktów,
- placeholdery dla działów Ciągi liczbowe i Liczby pierwsze,
- placeholdery dla funkcji świadomie nieukończonych.

Poza bieżącym zakresem pozostają:

- materiały wideo,
- pełny generator wykresów,
- pełny kalkulator trygonometryczny,
- natywne instalatory i automatyczne aktualizacje,
- podpisywanie kodu i paczek,
- dystrybucja na macOS dla Apple Silicon,
- synchronizacja kont między urządzeniami,
- zewnętrzne usługi chmurowe.

## 4. Wymagania funkcjonalne

| ID | Wymaganie | Priorytet | Status | Dowód implementacji |
| --- | --- | --- | --- | --- |
| F-01 | System umożliwia użycie profilu gościa bez hasła. | Wysoki | Zaimplementowane | `AccountService`, `LoginView`, `AccountServiceTests` |
| F-02 | System umożliwia rejestrację lokalnego konta z walidacją nazwy i hasła. | Wysoki | Zaimplementowane | `AccountService`, `Issue14RegistrationRegressionTests` |
| F-03 | System zapisuje hasła wyłącznie jako skróty z solą. | Wysoki | Zaimplementowane | `PasswordHasher`, `AccountServiceTests` |
| F-04 | System umożliwia odzyskanie i zmianę hasła przez kod odzyskiwania. | Wysoki | Zaimplementowane | `LoginView`, `ProfileView`, `AccountServiceTests` |
| F-05 | System importuje historyczne profile gościa z `users.txt` idempotentnie. | Średni | Zaimplementowane | `AccountService`, `InitialLocalAccounts`, `AccountServiceTests` |
| F-06 | System pokazuje 18 tablic matematycznych z treści JSON. | Wysoki | Zaimplementowane | `Content/formulas.json`, `ContentInventoryTests` |
| F-07 | System pokazuje 7 dostępnych działów matematyki, w tym Wektory oraz pełny zakres treści issue #35. | Wysoki | Zaimplementowane | `Content/chapters.json`, `ContentInventoryTests`, `Issue35MathChaptersRegressionTests` |
| F-08 | System pokazuje 35 zadań matury poprawkowej 2021. | Wysoki | Zaimplementowane | `Content/exam-2021-correction.json`, `ContentInventoryTests` |
| F-09 | System umożliwia wybór zadań według arkusza lub tematu. | Wysoki | Zaimplementowane | `ExerciseListView`, `ContentInventoryTests` |
| F-10 | System sprawdza odpowiedzi A-D w zadaniach zamkniętych i zapisuje postęp po poprawnej odpowiedzi. | Wysoki | Zaimplementowane | `ExerciseView`, `ExerciseAndRoutingCoverageTests` |
| F-11 | System ujawnia odpowiedzi otwarte i zapisuje postęp po ujawnieniu. | Wysoki | Zaimplementowane | `ExerciseView`, `ExerciseAndRoutingCoverageTests` |
| F-12 | System udostępnia podpowiedzi krokowe dla zadań. | Wysoki | Zaimplementowane | `ExerciseView`, `ContentInventoryTests` |
| F-13 | System udostępnia sesyjny brudnopis zadania. | Średni | Zaimplementowane | `ExerciseView` |
| F-14 | System udostępnia kalkulator funkcji kwadratowej z krokami rozwiązania. | Wysoki | Zaimplementowane | `QuadraticSolver`, `QuadraticSolverTests` |
| F-15 | System udostępnia kalkulator ogólny obsługujący działania podstawowe, nawiasy, potęgi i pierwiastki. | Wysoki | Zaimplementowane | `ExpressionCalculator`, `ExpressionCalculator*Tests` |
| F-16 | System obsługuje historię kalkulatora, `Ans`, powtarzane `=` i przyciski `1/x`, `x²`, pierwiastki. | Wysoki | Zaimplementowane | `CalculatorSession`, `CalculatorInputState`, testy kalkulatora |
| F-17 | System prezentuje funkcje planowane jako jawne placeholdery zamiast uszkodzonych ekranów. | Średni | Zaimplementowane | `Content/placeholders.json`, `docs/ROADMAP.md` |
| F-18 | System pozwala śledzić status migracji i zakres funkcji. | Średni | Zaimplementowane | `docs/MIGRATION_INVENTORY.md`, `docs/ROADMAP.md` |
| F-19 | System udostępnia dla działów issue #35 teorię, przykłady, zadania, wskazówki i odpowiedzi, a przypadek ujemnej delty opisuje w zbiorze liczb rzeczywistych. | Wysoki | Zaimplementowane | `Content/chapters.json`, `Issue35MathChaptersRegressionTests` |
| F-20 | System udostępnia ekran "O programie" z wersją, commitem, licencją, autorem i repozytorium. | Średni | Zaimplementowane | `AppBuildInfo`, `AboutView`, `AboutViewTests` |
| F-21 | Opublikowany plik wykonywalny obsługuje `--release-smoke-test --data-directory <katalog>` bez otwierania UI i bez dostępu do prawdziwych danych użytkownika. | Wysoki | Zaimplementowane | `ReleaseSmokeTest`, `ReleaseRuntimeTests` |
| F-22 | System pozwala wylosować zadanie z całego arkusza albo z aktywnego tematu, zachowując kontekst poprzedniego i następnego zadania. | Średni | Zaimplementowane | `ExerciseRandomizer`, `ExamOverviewView`, `ExerciseRandomizerTests` |

## 5. Wymagania niefunkcjonalne

| ID | Wymaganie | Priorytet | Status | Dowód weryfikacji |
| --- | --- | --- | --- | --- |
| NF-01 | Aplikacja działa offline. | Wysoki | Zaimplementowane | lokalne JSON, SQLite, brak wymaganych usług sieciowych |
| NF-02 | Aplikacja używa przenośnego UI Avalonia zamiast WPF. | Wysoki | Zaimplementowane | `Abituria.csproj`, `NavigationArchitectureTests` |
| NF-03 | Minimalny obsługiwany rozmiar głównego okna to `720x520`, a kluczowe widoki przechodzą do układu jednokolumnowego przed utratą czytelności. | Wysoki | Zaimplementowane | `MainWindow.axaml`, `AdaptiveLayout`, testy breakpointów |
| NF-04 | Długie treści edukacyjne i wzory nie są zapisane bezpośrednio w C#. | Wysoki | Zaimplementowane | `ContentSeparationTests`, katalog `Content` |
| NF-05 | Hasła nie są przechowywane jawnie. | Wysoki | Zaimplementowane | `PasswordHasher`, `AccountServiceTests` |
| NF-06 | Błędy kalkulatora są kontrolowane i nie powodują awarii aplikacji. | Wysoki | Zaimplementowane | `ExpressionCalculatorRobustnessTests`, regresje issues #1-#9 |
| NF-07 | Nawigacja nie tworzy nieograniczonej liczby okien. | Wysoki | Zaimplementowane | `NavigationArchitectureTests` |
| NF-08 | Treści matematyczne renderują się bez znanych uszkodzonych komend LaTeX. | Wysoki | Zaimplementowane | `ContentInventoryTests`, `Discussion10VisualRegressionTests` |
| NF-09 | Repozytorium ma bramy jakości: build, testy, format i SonarQube Cloud. | Wysoki | Zaimplementowane | `.github/workflows`, `docs/SONARQUBE.md` |
| NF-10 | Migracja zachowuje oryginalne dokumenty legacy bajt w bajt. | Średni | Zaimplementowane | `docs/legacy/originals`, SHA-256, `ContentInventoryTests` |
| NF-11 | Odtworzenie zależności jest deterministyczne, audyt obejmuje zależności przechodnie, a podatności `NU1901`-`NU1904` blokują wydanie. | Wysoki | Zaimplementowane | `Directory.Build.props`, lockfile, workflow wydania |
| NF-12 | Każdy paczkowany zasób treści, obrazu lub fontu ma maszynowo sprawdzane pochodzenie i status redystrybucji. | Wysoki | Zaimplementowane | `Content/provenance.json`, `ContentProvenanceTests`, `Test-ContentProvenance.ps1` |
| NF-13 | Paczki portable nie zawierają PDB, sekretów ani snapshotów, otrzymują sumy SHA-256, SBOM, atestację pochodzenia oraz manifest nuspec i dostępnych dowodów licencyjnych dokładnego grafu NuGet. | Wysoki | Zaimplementowane w automatyzacji | workflow wydania, `New-NuGetLicenseBundle.ps1`, `Test-ReleaseAssets.ps1` |
| NF-14 | Reprezentatywne obciążenie kalkulatora, katalogu treści i zapisu postępu mieści się w przenośnych budżetach czasu oraz pamięci. | Wysoki | Zaimplementowane i mierzone | `PerformanceMemoryAndLoadTests`, `docs/TESTING.md` |
| NF-15 | Interfejs używa paczkowanego kroju Mulish, a zależność od Avalonia Fonts Inter nie jest częścią grafu produkcyjnego. | Średni | Zaimplementowane | `AppStyles.axaml`, `Abituria.csproj`, test zależności i fontu |
| NF-16 | Aplikacja udostępnia ustawienie systemowe oraz motyw jasny, ciemny i wysokiego kontrastu, bez wymuszania wariantu jasnego. | Wysoki | Zaimplementowane | `AppThemeManager`, `AppStyles.axaml`, testy renderowania motywów |
| NF-17 | Interaktywne kontrolki mają rozróżnialne stany najechania, naciśnięcia, fokusu i fokusu klawiaturowego. | Wysoki | Zaimplementowane | selektory `:pointerover`, `:pressed`, `:focus`, `:focus-visible`, test widocznego fokusu |
| NF-18 | Własny pasek tytułu zapewnia minimalizację, maksymalizację, przywrócenie, zamknięcie, przeciąganie oraz zmianę rozmiaru z każdej krawędzi i narożnika. | Wysoki | Zaimplementowane | `MainWindow.axaml`, `MainWindow.axaml.cs`, test kontraktu chrome |
| NF-19 | Login, Start i kalkulator ogólny zmieniają strukturę układu odpowiednio przy szerokościach `860`, `780` i `900`, bez utraty logicznej kolejności kontrolek. | Wysoki | Zaimplementowane | `AdaptiveLayout`, testy breakpointów Avalonia Headless |
| NF-20 | Dialogi aplikacji są skalowalne, mają bezpieczne granice wymiarów i przewijanie dla treści wykraczającej poza obszar klienta. | Wysoki | Zaimplementowane | `AdaptiveLayout.CreateDialog`, test właściwości dialogu |
| NF-21 | Wszystkie kryteria WCAG 2.2 A/AA są przeglądane i śledzone, z jawnym rozdzieleniem dowodów automatycznych, kontroli manualnych i kryteriów nieodpowiednich dla aplikacji desktopowej. | Wysoki | Audyt wykonany, kontrole manualne pozostają jawne | `docs/ACCESSIBILITY_WCAG_AUDIT.md`, `AccessibilityRegressionTests`, `Discussion49StyleRegressionTests` |

## 6. Opis użytkowników systemu

Główne grupy użytkowników:

- Uczeń przygotowujący się do matury - korzysta z tablic, zadań, podpowiedzi i kalkulatorów.
- Maturzysta korzystający z profilu gościa - chce szybko rozpocząć naukę bez zakładania konta.
- Użytkownik z lokalnym kontem - chce zapisywać postęp i chronić profil hasłem.
- Autor treści - aktualizuje pliki JSON w `Content` oraz sprawdza renderowanie wzorów i obrazów.
- Opiekun techniczny - utrzymuje kod, testy, dokumentację, migrację legacy i bramy jakości.

System nie zakłada kont administratora, ról sieciowych ani synchronizacji wielu urządzeń.

## 7. Opis głównych modułów aplikacji

| Moduł | Odpowiedzialność | Główne pliki |
| --- | --- | --- |
| Shell i nawigacja | Główne okno, pasek nawigacji, przełączanie stron | `MainWindow.axaml.cs`, `AppViewModel.cs` |
| Konta i profil | Logowanie, rejestracja, odzyskiwanie hasła, postęp | `AccountService.cs`, `LoginView.cs`, `ProfileView.cs` |
| Dane lokalne | SQLite, encje, migracje i import legacy | `AppDbContext.cs`, `InitialLocalAccounts.cs` |
| Treści edukacyjne | Ładowanie i renderowanie JSON, wzorów i obrazów | `ContentRepository.cs`, `RichContentView.cs`, `UiFactory.cs` |
| Tablice i działy | Lista wzorów, artykuły i placeholdery działów | `ContentViews.cs`, `Content/formulas.json`, `Content/chapters.json` |
| Zadania maturalne | Lista zadań, tematy, podpowiedzi, odpowiedzi, postęp | `ExamViews.cs`, `Content/exam-2021-correction.json` |
| Kalkulator kwadratowy | Delta, miejsca zerowe, wierzchołek, postacie funkcji | `CalculatorView.cs`, `QuadraticSolver.cs` |
| Kalkulator ogólny | Parser wyrażeń, historia, `Ans`, wejście ekranowe | `GeneralCalculatorView.cs`, `ExpressionCalculator.cs`, `CalculatorSession.cs`, `CalculatorInputState.cs` |
| Dokumentacja i jakość | Architektura, wymagania, roadmapa, Sonar, testy | `docs`, `tests/Abituria.Tests`, `.github/workflows` |

## 8. Przypadki użycia

| ID | Nazwa | Aktor | Scenariusz podstawowy | Kryterium powodzenia |
| --- | --- | --- | --- | --- |
| UC-01 | Logowanie jako gość | Uczeń | Użytkownik wybiera profil gościa i przechodzi do ekranu startowego. | Widoczny ekran startowy z nazwą profilu. |
| UC-02 | Rejestracja konta | Użytkownik lokalny | Użytkownik wpisuje nazwę, hasło i potwierdzenie. | Konto powstaje, a system pokazuje kod odzyskiwania. |
| UC-03 | Odzyskanie hasła | Użytkownik lokalny | Użytkownik podaje nazwę, kod odzyskiwania i nowe hasło. | Hasło zostaje zmienione, a system wydaje nowy kod. |
| UC-04 | Przeglądanie tablic | Uczeń | Użytkownik wybiera dział wzorów i otwiera artykuł. | Artykuł renderuje tekst, wzory i obrazy. |
| UC-05 | Rozwiązywanie zadania zamkniętego | Uczeń | Użytkownik wybiera odpowiedź A-D i klika sprawdzenie. | Poprawna odpowiedź zapisuje postęp. |
| UC-06 | Rozwiązywanie zadania otwartego | Uczeń | Użytkownik czyta treść, korzysta z podpowiedzi i ujawnia odpowiedź. | Odpowiedź jest widoczna, a zadanie zapisane jako ukończone. |
| UC-07 | Użycie kalkulatora ogólnego | Uczeń | Użytkownik wpisuje wyrażenie i naciska `=`. | System pokazuje wynik lub kontrolowany błąd. |
| UC-08 | Powtarzanie działania kalkulatora | Uczeń | Użytkownik naciska `=` kilka razy po poprawnym wyniku. | System stosuje semantykę powtórzenia bez utraty historii. |
| UC-09 | Użycie kalkulatora funkcji kwadratowej | Uczeń | Użytkownik wpisuje współczynniki `a`, `b`, `c` i klika obliczenie. | System pokazuje wynik i kroki. |
| UC-10 | Sprawdzenie planu rozwoju | Opiekun lub użytkownik | Użytkownik otwiera roadmapę. | Funkcje są opisane jako przeniesione, planowane lub zastąpione. |
| UC-11 | Przeglądanie działu matematyki | Uczeń | Użytkownik otwiera listę działów i wybiera dostępny materiał. | System otwiera artykuł z teorią, przykładami i zadaniami; tylko ciągi i liczby pierwsze są oznaczone jako treść w przygotowaniu. |

## 9. Wymagania dotyczące interfejsu użytkownika

- Interfejs musi działać przy minimalnym rozmiarze `720x520`.
- Nawigacja musi być dostępna z jednego głównego okna.
- Login, Start i kalkulator ogólny muszą przechodzić do układu kompaktowego odpowiednio poniżej `860`, `780` i `900` pikseli.
- Użytkownik musi móc wybrać motyw systemowy, jasny, ciemny lub wysokiego kontrastu.
- Zmiana motywu musi aktualizować otwarte widoki bez ponownego uruchamiania aplikacji.
- Przyciski, pola tekstowe i pola wyboru muszą mieć widoczne stany najechania, naciśnięcia, fokusu i `focus-visible`.
- Własne przyciski paska tytułu muszą być dostępne z klawiatury, mieć opisowe nazwy automatyzacji i zachowywać funkcje natywnego okna.
- Okno musi dać się przenosić za pasek tytułu, maksymalizować dwuklikiem oraz skalować z czterech krawędzi i czterech narożników.
- Dialogi kodów odzyskiwania muszą pozwalać na zmianę rozmiaru i przewijać zawartość zamiast ją ucinać.
- Strona "Kalkulator" musi pozostawać aktywna także na ekranie kalkulatora ogólnego.
- Widoki treści muszą obsługiwać przewijanie pionowe.
- Długie teksty muszą zawijać się bez wychodzenia poza obszar widoku.
- Wzory inline muszą zachowywać wspólną linię bazową z tekstem.
- Listy matematyczne muszą mieć poprawne wyrównanie myślników, tekstu i wzorów.
- Błędy formularzy i kalkulatora muszą być komunikowane tekstowo, bez nieobsłużonych wyjątków.
- Historia kalkulatora musi umożliwiać przywrócenie wcześniejszego wyrażenia.
- Modalne dialogi kodów odzyskiwania muszą blokować główne okno do czasu zamknięcia.

## 10. Ograniczenia technologiczne

- Język i platforma: C# oraz .NET 10 LTS, SDK przypięty do `10.0.302`.
- UI: AvaloniaUI, bez aktywnego WPF.
- Dane lokalne: SQLite przez Entity Framework Core.
- Treści: JSON jako zasoby Avalonia.
- Renderowanie wzorów: `Sylinko.CSharpMath.Avalonia`.
- Hasła: PBKDF2-HMAC-SHA256, bez jawnego zapisu haseł.
- Testy UI: Avalonia Headless.
- Analiza jakości: SonarQube Cloud przez workflow CI.
- Aplikacja nie wymaga internetu do działania podstawowego.
- Wydanie beta używa self-contained, nietrimowanych publikacji bez AOT, ReadyToRun i single-file.
- Deklarowane platformy beta to Windows 11 24H2 x64, Ubuntu 24.04 x64 i macOS 15 x64 na procesorach Intel.
- Paczki są niepodpisane. Dokumentacja nie zaleca globalnego wyłączania SmartScreen ani Gatekeepera.
- Stare snapshoty i prototypy nie są częścią aktywnego kodu i nie powinny być śledzone przez Git.

## 11. Kryteria akceptacji projektu

Kryteria techniczne bieżącej wersji, publiczna obrona 17 stycznia 2022 r., późniejszy historyczny odbiór przez prowadzącego i publiczna publikacja bieżącej migracji są odrębnymi stanami. Obrona zakończyła się pozytywną decyzją komisji i wynikiem bardzo dobrym. Historyczny projekt został również zaakceptowany przez prowadzącego na początku lutego 2022 r. zgodnie z poświadczeniem właściciela zapisanym 19 lipca 2026 r. Kryteria dotyczące tagu, proweniencji i GitHub Release pozostają warunkami osobnej publikacji `0.9.0-beta.1`, a nie warunkami ponownego zatwierdzenia decyzji historycznych.

Zakres techniczny i wydawniczy jest oceniany według poniższych warunków:

1. `dotnet restore Abituria.sln --configfile NuGet.Config --locked-mode` kończy się powodzeniem.
2. `dotnet build Abituria.sln --configuration Release --no-restore` kończy się powodzeniem.
3. `dotnet test Abituria.sln --configuration Release --no-build` kończy się powodzeniem.
4. `dotnet format whitespace Abituria.sln --verify-no-changes --no-restore` nie zgłasza zmian.
5. `git diff --check` nie zgłasza błędów.
6. SonarQube Cloud nie raportuje otwartych problemów po analizie aktualnego commita.
7. Inwentarz treści potwierdza 18 tablic, 9 pozycji działowych, w tym 7 dostępnych i 2 placeholdery, 17 tematów i 35 zadań.
8. Wszystkie obrazy wskazane przez treści istnieją w repozytorium.
9. Każde zadanie ma kompletną umowę odpowiedzi: opcje i klucz dla zadań zamkniętych albo odpowiedź ujawnianą dla zadań otwartych.
10. Kalkulator ogólny przechodzi regresje dla issues #1-#9 oraz powiązanych dyskusji.
11. Widoki architektury nie używają WPF `Page`, `Frame`, `NavigationWindow` ani nie otwierają nieograniczonych niemodalnych okien.
12. Dokumenty `README.md`, `docs/BUSINESS_ANALYSIS.md`, `docs/ARCHITECTURE.md`, `docs/REQUIREMENTS.md`, `docs/ACCESSIBILITY_WCAG_AUDIT.md`, `docs/MIGRATION_INVENTORY.md`, `docs/ROADMAP.md` i `docs/SONARQUBE.md` są dostępne z repozytorium.
13. Bezpośrednia regresja issue #35 potwierdza wymagane sekcje sześciu nowych materiałów, wszystkie 24 litery alfabetu greckiego, przypadek `\(\Delta{}<0\)`, zadania ze wskazówkami i odpowiedziami, statusy roadmapy oraz renderowanie każdego artykułu przy `960x640` i `1280x820`.
14. Importer odtwarza katalog issue #35 z niezależnego seeda do pustego `OutputRoot`, bez odczytywania aktywnych plików wynikowych, a test end-to-end potwierdza zgodność wszystkich działów i pozycji roadmapy.
15. Testy wydania przechodzą na natywnych runnerach Windows, Ubuntu i macOS, a opublikowany artefakt przechodzi diagnostyczny smoke test w katalogu tymczasowym.
16. Tag, assembly, nazwy paczek, ekran "O programie", changelog i strona dokumentacji wskazują tę samą wersję z `Directory.Build.props`.
17. Każda paczka zawiera `LICENSE`, skróconą instrukcję, `THIRD-PARTY-NOTICES.md` oraz `licenses/nuget/manifest.json` zgodny z `Abituria.deps.json`, ale nie zawiera PDB, sekretów ani starych snapshotów.
18. Sumy w `SHA256SUMS.txt` zgadzają się z paczkami, każda paczka ma osobny SBOM SPDX, artefakty mają atestację pochodzenia, a `Generate-DependencyDocumentation.ps1 -Verify` potwierdza aktualność wykazu zależności i narzędzi.
19. `tools/Test-ContentProvenance.ps1 -RequireReleaseEligible` kończy się powodzeniem. Obecność choć jednego statusu `blocked` zabrania publicznego wydania.
20. GitHub Pages buduje się z kanonicznych plików Markdown, a automatyczny test linków nie zgłasza błędów.
21. `PerformanceMemoryAndLoadTests` przechodzi w konfiguracji Release, a metryki nie przekraczają opisanych budżetów.
22. Losowanie z całego arkusza i z tematu otwiera zadanie należące do właściwej puli oraz zachowuje ten kontekst nawigacji.
23. Wynik historycznych testów użyteczności z uczestnikami, ograniczenia zachowanego materiału oraz późniejsze poprawki heurystyczne są rozdzielone i udokumentowane zgodnie z `USABILITY_TEST_PROTOCOL.md`, `USABILITY_TEST_RESULTS.md` oraz `ACCEPTANCE_PROTOCOL.md`. Brakujące liczby, daty, komentarze i poprawki uczestników nie mogą być rekonstruowane przez domysł.
24. Protokoły I-IV rozdzielają daty technicznej rekonstrukcji od poświadczonej końcowej decyzji prowadzącego z początku lutego 2022 r. Historyczna, uzgodniona forma przekazania i odrębny status nieistniejącego jeszcze publicznego GitHub Release są zapisane w `acceptance/README.md` i `DELIVERY_PROTOCOL.md`.
25. `DEFENSE_PROTOCOL.md` zapisuje rzeczywistą datę publicznej obrony, role i nazwiska członków komisji, demonstrację aplikacji, pytania i odpowiedzi, wynik oraz publiczny odnośnik do nagrania, bez utożsamiania historycznej wersji z bieżącą migracją.
26. `EVALUATION_PROTOCOL.md` odwzorowuje wszystkie obszary i warunki Issue #45, rozdziela bezpośrednie dowody od poświadczeń retrospektywnych, nie przypisuje historycznej oceny bieżącej migracji i zawiera decyzję oraz komentarz zamykający.
27. Testy dyskusji #49 potwierdzają własny chrome, pełny zestaw stanów interakcji, widoczny fokus, przełączenie czterech ustawień motywu i różne obrazy palet.
28. Testy breakpointów potwierdzają zmianę Login przy `860`, Start przy `780` i kalkulatora ogólnego przy `900`, a główne okno zachowuje minimum `720x520`.
29. Test wpływu stylowania na renderowanie i nawigację mieści się w zapisanym budżecie czasu i pamięci oraz przechodzi dla wariantu jasnego, ciemnego i wysokiego kontrastu.
30. `ACCESSIBILITY_WCAG_AUDIT.md` zawiera wszystkie 55 obowiązujących kryteriów A/AA WCAG 2.2, nie przedstawia testów headless jako certyfikatu i jawnie wskazuje kontrole wymagające technologii asystującej lub użytkownika.

## Macierz zgodności wymagań z implementacją

| Obszar | Wymagania | Implementacja | Testy lub dokumenty |
| --- | --- | --- | --- |
| Konta i bezpieczeństwo | F-01 - F-05, NF-05 | `AccountService`, `PasswordHasher`, `LoginView`, `ProfileView` | `AccountServiceTests`, `Issue14RegistrationRegressionTests` |
| Treści edukacyjne | F-06 - F-13, F-19, F-22, NF-04, NF-08 | `Content/*.json`, `ContentRepository`, `RichContentView`, `ExamViews`, `ExerciseRandomizer` | `ContentInventoryTests`, `ContentSeparationTests`, `Discussion10VisualRegressionTests`, `Issue35MathChaptersRegressionTests`, `ExerciseRandomizerTests` |
| Kalkulatory | F-14 - F-16, NF-06 | `QuadraticSolver`, `ExpressionCalculator`, `CalculatorSession`, `CalculatorInputState` | `QuadraticSolverTests`, `ExpressionCalculator*Tests`, `CalculatorSessionTests` |
| Nawigacja, styl i dostępność UI | F-17, NF-02, NF-03, NF-07, NF-15 - NF-21 | `MainWindow`, `AppThemeManager`, `AdaptiveLayout`, `AppStyles.axaml`, `Views` | `NavigationArchitectureTests`, `ExerciseAndRoutingCoverageTests`, `AccessibilityRegressionTests`, `Discussion49StyleRegressionTests`, `docs/ACCESSIBILITY_WCAG_AUDIT.md` |
| Migracja i dokumentacja | F-18, NF-09, NF-10 | `docs`, `.github/workflows`, `docs/legacy/originals` | `docs/MIGRATION_INVENTORY.md`, `docs/ARCHITECTURE.md`, `docs/DEFENSE_PROTOCOL.md`, `docs/EVALUATION_PROTOCOL.md`, ten dokument |
| Wydanie i łańcuch dostaw | F-20 - F-21, NF-11 - NF-14 | `Directory.Build.props`, `global.json`, lockfile, `tools/release`, workflow wydania | `ReleaseRuntimeTests`, `ContentProvenanceTests`, `PerformanceMemoryAndLoadTests`, `docs/RELEASE_PROCESS.md` |

## Status issue #34

Historyczny problem polegał na tym, że dokument wymagań projektowych był pustym lub prawie pustym szablonem. Ten plik uzupełnia brak jako aktywny dokument wymagań dla aktualnej implementacji i pozwala porównać zakres projektu z kodem, testami oraz dokumentacją.

## Status issue #35

Historyczne zgłoszenie wymagało uzupełnienia treści działów matematyki. Aktywny katalog `Content/chapters.json` zawiera 9 pozycji: 7 dostępnych artykułów i 2 jawne placeholdery. Kanoniczne dane issue #35 są przechowywane w `tools/seeds/issue-35-content.json` i synchronizowane skryptem `tools/Sync-Issue35Content.ps1`. Bezpośrednie testy sprawdzają aksjomatykę i indukcję, alfabet grecki, liczby rzeczywiste i zbiory, algebrę, równania i nierówności, funkcję kwadratową, logarytmy, przykłady, zadania oraz odtworzenie katalogu do pustego katalogu wyjściowego. Oryginalna notatka pozostaje zachowana bajt w bajt w `docs/legacy/originals`.

## Status issue #36

Repozytorium zawiera implementację procesu przygotowania wersji `0.9.0-beta.1`: jedno źródło wersji, .NET 10 LTS, lockfile, audyt, paczki portable dla trzech systemów, diagnostyczny smoke test, ekran "O programie", dokumentację, sumy, SBOM i atestacje. Zakończenie techniczne nie jest równoznaczne z publicznym wydaniem. Deklaracja praw jest zapisana w `ASSET_RIGHTS_DECLARATION.md`, dowody przypisano do grup w `Content/provenance.json`, manifest ma `releaseEligible=true`, a brama proweniencji przeszła 19 lipca 2026 r. Issue #36 nadal można zamknąć dopiero po utworzeniu tagu, wykonaniu workflow, publicznym prerelease, weryfikacji Pages i komentarzu z linkami oraz checklistą.

## Status issue #38

Historyczne issue #38 wymagało analizy biznesowej obejmującej cele, użytkowników, model udostępniania, zakres, harmonogram, licencję, metodykę wymagań i architekturę. Aktywny dokument [BUSINESS_ANALYSIS.md](BUSINESS_ANALYSIS.md) zastępuje historyczną checklistę i odsyła do weryfikowalnych wymagań, testów oraz bramy pochodzenia zasobów. `releaseEligible=true` i proweniencja jest zatwierdzona, ale dokument nie deklaruje publicznego wydania, dopóki tag, workflow i GitHub Release nie zostaną rzeczywiście wykonane.

## Status issue #43

Issue #43 dotyczyło odbioru czterech przyrostów, prototypowania, testów użyteczności i końcowego przekazania. Właściciel projektu poświadczył 19 lipca 2026 r., że historyczne testy użyteczności z uczestnikami zakończyły się powodzeniem, prowadzący zaakceptował projekt na początku lutego 2022 r., a końcowa wersja została przekazana w uzgodnionej formie. Właściciel poświadczył również posiadanie praw do zasobów projektu.

Nie zachowano dokładnego dnia odbioru, osobnych dat decyzji dla I-III przyrostu, liczby i danych uczestników, kart sesji, komentarzy, powiązania poprawek z uczestnikami, kanału przekazania ani hasha historycznego pakietu. Protokoły ujawniają te braki i nie zastępują ich fikcyjnymi danymi. Późniejsze poprawki H-01-H-03 są wynikiem przeglądu heurystycznego z 2026 r. i nie są przypisywane uczestnikom.

Na tej podstawie historyczny odbiór ma status `ACCEPTED - READY TO CLOSE`. Publiczny GitHub Release `0.9.0-beta.1` nie istnieje i pozostaje osobnym działaniem. Dowody proweniencji oraz brama `-RequireReleaseEligible` są już pozytywne; pozostały tag, workflow, kontrola draftu i publikacja.

## Status issue #44

Publiczna obrona projektu Abituria odbyła się 17 stycznia 2022 r. przed komisją w składzie: przewodniczący dr Paweł M. Owsianny, recenzent prof. UAM dr hab. Jerzy Szymański oraz promotor dr Tomasz Piłka. Pokazano działającą aplikację i przeprowadzono pytania oraz odpowiedzi. Komisja podjęła decyzję pozytywną, a wynik był bardzo dobry.

Nagranie `Projekt Inz final` o widoczności `unlisted` jest dostępne przez link wskazany w [DEFENSE_PROTOCOL.md](DEFENSE_PROTOCOL.md). Metadane potwierdzają transmisję z 17 stycznia 2022 r. i czas 2:10:03. Nagranie bezpośrednio potwierdza prezentację, demonstrację i Q&A, lecz kończy się po zapowiedzi narady. Skład komisji, pozytywna decyzja i wynik są retrospektywnym poświadczeniem właściciela; brak osobnej transkrypcji i podpisanego protokołu jest ujawniony.

Kamień milowy M7 ma status `ACCEPTED - M7 ACHIEVED - READY TO CLOSE AS COMPLETED`. Issue #44 jest zamknięte w GitHub, lecz podczas weryfikacji miało powód `not planned`; merytorycznie właściwy powód to `completed`.

## Status issue #45

Issue #45 definiowało siedem obszarów oceny, dziesięć warunków akceptacji oraz warunki uzyskania wyniku bardzo dobrego. [EVALUATION_PROTOCOL.md](EVALUATION_PROTOCOL.md) zawiera pełną macierz, podstawę dowodową, ograniczenia archiwalne i gotowy komentarz zamykający.

Historyczny produkt WPF został zaprezentowany 17 stycznia 2022 r., miał paczkę `v1.0.0` dostępną przed obroną i końcowe wydanie `v1.0.1` z 18 stycznia 2022 r. Komisja wydała decyzję pozytywną, a projekt i obrona uzyskały wynik bardzo dobry według poświadczenia właściciela. Nagranie dostępne przez link potwierdza prezentację, demonstrację i pytania, ale nie obejmuje ogłoszenia wyniku. Nie zachowano podpisanej karty oceny, pisemnej opinii prowadzącego ani ocen indywidualnych i dokumentacja nie rekonstruuje ich przez domysł.

Bieżąca migracja AvaloniaUI `0.9.0-beta.1` ma osobne dowody techniczne i nie była przedmiotem komisji z 2022 r. Jej publiczny GitHub Release pozostaje niewykonany. Stan historycznego zakresu: `ACCEPTED - HISTORICAL CRITERIA SATISFIED - READY TO CLOSE AS COMPLETED`.
