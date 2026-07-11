# Dokument wymagań projektowych Abituria

Status: aktualny dokument wymagań dla bieżącej implementacji AvaloniaUI.

Źródło uzupełnienia: issue #34 "Uzupełnić: Dokument wymagań projektowych" oraz aktualny stan repozytorium `haribo841/Abituria`.

Dokument zastępuje pusty historyczny szablon wymagań. Historyczne notatki pozostają w `docs/legacy`, ale nie są już jedyną podstawą oceny zakresu projektu.

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

Abituria jest aplikacją desktopową `.NET 9` z interfejsem AvaloniaUI. Aplikacja działa lokalnie i nie wymaga połączenia z internetem do korzystania z podstawowych funkcji.

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
- dział "Wektory" z ilustracjami,
- 35 zadań matury poprawkowej 2021,
- przeglądanie zadań według arkusza i tematów,
- sprawdzanie odpowiedzi w zadaniach zamkniętych,
- ujawnianie odpowiedzi w zadaniach otwartych,
- podpowiedzi krokowe,
- sesyjny brudnopis zadania,
- kalkulator funkcji kwadratowej,
- kalkulator ogólny z nawiasami, potęgami, pierwiastkami, notacją naukową, `Ans`, historią i powtarzanym `=`,
- roadmapę odróżniającą funkcje przeniesione, planowane i zastąpione,
- placeholdery dla funkcji świadomie nieukończonych.

Poza bieżącym zakresem pozostają:

- materiały wideo,
- pełny generator wykresów,
- pełny kalkulator trygonometryczny,
- dystrybucja instalatora,
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
| F-07 | System pokazuje dział "Wektory" z ilustracjami. | Wysoki | Zaimplementowane | `Content/chapters.json`, `ContentInventoryTests` |
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

## 5. Wymagania niefunkcjonalne

| ID | Wymaganie | Priorytet | Status | Dowód weryfikacji |
| --- | --- | --- | --- | --- |
| NF-01 | Aplikacja działa offline. | Wysoki | Zaimplementowane | lokalne JSON, SQLite, brak wymaganych usług sieciowych |
| NF-02 | Aplikacja używa przenośnego UI Avalonia zamiast WPF. | Wysoki | Zaimplementowane | `Abituria.csproj`, `NavigationArchitectureTests` |
| NF-03 | Minimalny obsługiwany rozmiar okna to `960x640`. | Średni | Zaimplementowane | `MainWindow.axaml`, testy wizualne |
| NF-04 | Długie treści edukacyjne i wzory nie są zapisane bezpośrednio w C#. | Wysoki | Zaimplementowane | `ContentSeparationTests`, katalog `Content` |
| NF-05 | Hasła nie są przechowywane jawnie. | Wysoki | Zaimplementowane | `PasswordHasher`, `AccountServiceTests` |
| NF-06 | Błędy kalkulatora są kontrolowane i nie powodują awarii aplikacji. | Wysoki | Zaimplementowane | `ExpressionCalculatorRobustnessTests`, regresje issues #1-#9 |
| NF-07 | Nawigacja nie tworzy nieograniczonej liczby okien. | Wysoki | Zaimplementowane | `NavigationArchitectureTests` |
| NF-08 | Treści matematyczne renderują się bez znanych uszkodzonych komend LaTeX. | Wysoki | Zaimplementowane | `ContentInventoryTests`, `Discussion10VisualRegressionTests` |
| NF-09 | Repozytorium ma bramy jakości: build, testy, format i SonarQube Cloud. | Wysoki | Zaimplementowane | `.github/workflows`, `docs/SONARQUBE.md` |
| NF-10 | Migracja zachowuje oryginalne dokumenty legacy bajt w bajt. | Średni | Zaimplementowane | `docs/legacy/originals`, SHA-256, `ContentInventoryTests` |

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

## 9. Wymagania dotyczące interfejsu użytkownika

- Interfejs musi działać przy minimalnym rozmiarze `960x640`.
- Nawigacja musi być dostępna z jednego głównego okna.
- Strona "Kalkulator" musi pozostawać aktywna także na ekranie kalkulatora ogólnego.
- Widoki treści muszą obsługiwać przewijanie pionowe.
- Długie teksty muszą zawijać się bez wychodzenia poza obszar widoku.
- Wzory inline muszą zachowywać wspólną linię bazową z tekstem.
- Listy matematyczne muszą mieć poprawne wyrównanie myślników, tekstu i wzorów.
- Błędy formularzy i kalkulatora muszą być komunikowane tekstowo, bez nieobsłużonych wyjątków.
- Historia kalkulatora musi umożliwiać przywrócenie wcześniejszego wyrażenia.
- Modalne dialogi kodów odzyskiwania muszą blokować główne okno do czasu zamknięcia.

## 10. Ograniczenia technologiczne

- Język i platforma: C# oraz .NET 9.
- UI: AvaloniaUI, bez aktywnego WPF.
- Dane lokalne: SQLite przez Entity Framework Core.
- Treści: JSON jako zasoby Avalonia.
- Renderowanie wzorów: `Sylinko.CSharpMath.Avalonia`.
- Hasła: PBKDF2-HMAC-SHA256, bez jawnego zapisu haseł.
- Testy UI: Avalonia Headless.
- Analiza jakości: SonarQube Cloud przez workflow CI.
- Aplikacja nie wymaga internetu do działania podstawowego.
- Stare snapshoty i prototypy nie są częścią aktywnego kodu i nie powinny być śledzone przez Git.

## 11. Kryteria akceptacji projektu

Projekt można uznać za zaakceptowany w bieżącym zakresie, gdy spełnia poniższe warunki:

1. `dotnet restore Abituria.sln --configfile NuGet.Config` kończy się powodzeniem.
2. `dotnet build Abituria.sln --configuration Release --no-restore` kończy się powodzeniem.
3. `dotnet test Abituria.sln --configuration Release --no-build` kończy się powodzeniem.
4. `dotnet format whitespace Abituria.sln --verify-no-changes --no-restore` nie zgłasza zmian.
5. `git diff --check` nie zgłasza błędów.
6. SonarQube Cloud nie raportuje otwartych problemów po analizie aktualnego commita.
7. Inwentarz treści potwierdza 18 tablic, 7 pozycji działowych, 17 tematów i 35 zadań.
8. Wszystkie obrazy wskazane przez treści istnieją w repozytorium.
9. Każde zadanie ma kompletną umowę odpowiedzi: opcje i klucz dla zadań zamkniętych albo odpowiedź ujawnianą dla zadań otwartych.
10. Kalkulator ogólny przechodzi regresje dla issues #1-#9 oraz powiązanych dyskusji.
11. Widoki architektury nie używają WPF `Page`, `Frame`, `NavigationWindow` ani nie otwierają nieograniczonych niemodalnych okien.
12. Dokumenty `README.md`, `docs/ARCHITECTURE.md`, `docs/REQUIREMENTS.md`, `docs/MIGRATION_INVENTORY.md`, `docs/ROADMAP.md` i `docs/SONARQUBE.md` są dostępne z repozytorium.

## Macierz zgodności wymagań z implementacją

| Obszar | Wymagania | Implementacja | Testy lub dokumenty |
| --- | --- | --- | --- |
| Konta i bezpieczeństwo | F-01 - F-05, NF-05 | `AccountService`, `PasswordHasher`, `LoginView`, `ProfileView` | `AccountServiceTests`, `Issue14RegistrationRegressionTests` |
| Treści edukacyjne | F-06 - F-13, NF-04, NF-08 | `Content/*.json`, `ContentRepository`, `RichContentView`, `ExamViews` | `ContentInventoryTests`, `ContentSeparationTests`, `Discussion10VisualRegressionTests` |
| Kalkulatory | F-14 - F-16, NF-06 | `QuadraticSolver`, `ExpressionCalculator`, `CalculatorSession`, `CalculatorInputState` | `QuadraticSolverTests`, `ExpressionCalculator*Tests`, `CalculatorSessionTests` |
| Nawigacja i UI | F-17, NF-02, NF-03, NF-07 | `MainWindow`, `AppViewModel`, `Views` | `NavigationArchitectureTests`, `ExerciseAndRoutingCoverageTests` |
| Migracja i dokumentacja | F-18, NF-09, NF-10 | `docs`, `.github/workflows`, `docs/legacy/originals` | `docs/MIGRATION_INVENTORY.md`, `docs/ARCHITECTURE.md`, ten dokument |

## Status issue #34

Historyczny problem polegał na tym, że dokument wymagań projektowych był pustym lub prawie pustym szablonem. Ten plik uzupełnia brak jako aktywny dokument wymagań dla aktualnej implementacji i pozwala porównać zakres projektu z kodem, testami oraz dokumentacją.
