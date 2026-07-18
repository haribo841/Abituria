# Protokół końcowego odbioru i wdrożenia

Wersja docelowa: `0.9.0-beta.1`.

Data przygotowania: 18 lipca 2026 r.

## Statusy

- `PASS` - kryterium ma aktualny, weryfikowalny dowód pozytywny.
- `PENDING` - wymagane działanie ręczne nie zostało jeszcze udokumentowane.
- `BLOCKED` - spełnienie jest niemożliwe bez rozwiązania wskazanej blokady.

## Odbiór zakresu Issue #41

| Obszar | Kryterium | Dowód | Status |
| --- | --- | --- | --- |
| Przyrost 1 | start, logowanie, nawigacja, kalkulator, dział, zadanie i obsługa błędu działają | testy kont, `NavigationArchitectureTests`, `ExerciseAndRoutingCoverageTests`, testy kalkulatora | PASS - pełny lokalny przebieg 406/406 18 lipca 2026 r. |
| Przyrost 2 | podpowiedzi krokowe, zadania otwarte, postęp, kolejne działy i losowanie zadań są zintegrowane | `ExerciseAndRoutingCoverageTests`, `ExerciseRandomizerTests`, `Issue35MathChaptersRegressionTests` | PASS - pełny lokalny przebieg 406/406 18 lipca 2026 r. |
| Wydajność i pamięć | reprezentatywne obciążenie mieści się w budżetach | `PerformanceMemoryAndLoadTests`, `TESTING.md` | PASS lokalnie 18 lipca 2026 r. |
| Jakość | build, testy, format, audyt zależności i SonarQube są zielone dla commita wydania | workflow `build`, `sonarcloud`, `SONARQUBE.md` | PENDING dla commita po tych zmianach |
| Instalacja lokalna | samowystarczalna publikacja uruchamia izolowany smoke test w nowym katalogu | `dotnet publish --self-contained true` oraz `Abituria.exe --release-smoke-test` | PASS - Windows 11 x64, 18 lipca 2026 r. |
| Instalacja niezależna | każda deklarowana platforma ma udokumentowany test na innym komputerze | tabela poniżej, `platform-installation-smoke.yml` oraz natywny workflow `release` | PENDING - workflow niezależnych runnerów jest gotowy do uruchomienia |
| Publiczne wydanie | tag, paczki, sumy, SBOM, atestacje, draft i publikacja w GitHub Releases | `RELEASE_PROCESS.md`, workflow `release` | BLOCKED przez proweniencję zasobów |

## Odbiór zakresu Issue #42

| Wymagany element dokumentacji | Lokalizacja | Status |
| --- | --- | --- |
| cel, zakres, wymagania, moduły i architektura | `REQUIREMENTS.md`, `ARCHITECTURE.md`, `BUSINESS_ANALYSIS.md` | PASS |
| technologie, zależności i wymagania systemowe | `DEPENDENCIES.md`, `INSTALLATION.md`, `THIRD-PARTY-NOTICES.md` | PASS |
| instrukcja użytkownika i uruchomienia | `USER_GUIDE.md`, `INSTALLATION.md` | PASS |
| testy funkcjonalne, regresyjne, wydajnościowe i pamięciowe | `TESTING.md`, testy projektu | PASS po pełnej bramie automatycznej |
| użyteczność, instalacja niezależna i końcowy odbiór | `USABILITY_TEST_PROTOCOL.md`, ten dokument | PENDING - formularze gotowe, brak rzeczywistych podpisów i niezależnych wykonawców |
| ograniczenia i status prawny zasobów | `KNOWN_LIMITATIONS.md`, `CONTENT_PROVENANCE.md` | PASS jako rzetelny opis blokady |
| autor, licencja i historia zmian | `AUTHORS.md`, `LICENSE`, `CHANGELOG.md` | PASS |
| PDF dla komisji | `output/pdf/Abituria-Technical-Documentation-0.9.0-beta.1.pdf` | PASS po wygenerowaniu i kontroli wizualnej |
| zatwierdzenie i przekazanie komisji | podpisy i potwierdzenie przekazania poniżej | PENDING |

## Test instalacji na niezależnych komputerach

Przed wypełnieniem tabeli należy pobrać paczkę z tego samego wydania co `SHA256SUMS.txt`, porównać pełną sumę SHA-256, rozpakować aplikację do nowego katalogu i wykonać smoke test zgodnie z `INSTALLATION.md`.

| Data | Wersja / hash paczki | System i architektura | Osoba testująca | Instalacja / uruchomienie | Smoke test | Uwagi | Podpis lub identyfikator potwierdzenia |
| --- | --- | --- | --- | --- | --- | --- | --- |
| do wypełnienia | do wypełnienia | Windows 11 x64 | niezależny tester |  |  |  |  |
| do wypełnienia | do wypełnienia | Ubuntu 24.04 x64 | niezależny tester |  |  |  |  |
| do wypełnienia | do wypełnienia | macOS 15 Intel x64 | niezależny tester |  |  |  |  |

## Decyzja odbiorowa

| Decyzja | Data | Osoba odpowiedzialna | Podpis / odnośnik do zatwierdzenia | Uwagi |
| --- | --- | --- | --- | --- |
| do wypełnienia: zaakceptowano / warunkowo / odrzucono | do wypełnienia | do wypełnienia | do wypełnienia | do wypełnienia |

## Warunek publikacji

Nie wolno oznaczać wydania jako publicznie zaakceptowanego, dopóki `tools/Test-ContentProvenance.ps1 -RequireReleaseEligible` nie przejdzie. Obecny manifest wskazuje trzy blokujące grupy: `cke-2021-correction-exam`, `inherited-mathematics-images` i `inherited-application-images`. Wymagane są udokumentowane licencje lub pełne zastąpienie materiałów i grafik własnymi zasobami. Zmiana pola `releaseEligible` bez tego dowodu nie jest dopuszczalnym odbiorem.
