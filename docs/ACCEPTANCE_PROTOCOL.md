# Protokół końcowego odbioru i wdrożenia

Wersja docelowa: `0.9.0-beta.1`.

Data ostatniej aktualizacji: 19 lipca 2026 r.

## Statusy

- `PASS` - kryterium ma aktualny, weryfikowalny dowód pozytywny.
- `PASS RETROSPEKTYWNY` - właściciel poświadczył historyczny wynik lub decyzję, ale szczegółowy materiał źródłowy nie został zachowany; ograniczenie musi być opisane.
- `PENDING` - wymagane działanie ręczne nie zostało jeszcze udokumentowane.
- `BLOCKED` - spełnienie jest niemożliwe bez rozwiązania wskazanej blokady.

## Odbiór zakresu Issue #41

| Obszar | Kryterium | Dowód | Status |
| --- | --- | --- | --- |
| Przyrost 1 | start, logowanie, nawigacja, kalkulator, dział, zadanie i obsługa błędu działają | testy kont, `NavigationArchitectureTests`, `ExerciseAndRoutingCoverageTests`, testy kalkulatora | PASS - pełny lokalny przebieg 415/415 19 lipca 2026 r. |
| Przyrost 2 | podpowiedzi krokowe, zadania otwarte, postęp, kolejne działy i losowanie zadań są zintegrowane | `ExerciseAndRoutingCoverageTests`, `ExerciseRandomizerTests`, `Issue35MathChaptersRegressionTests` | PASS - pełny lokalny przebieg 415/415 19 lipca 2026 r. |
| Wydajność i pamięć | reprezentatywne obciążenie mieści się w budżetach | `PerformanceMemoryAndLoadTests`, `TESTING.md` | PASS lokalnie 18 lipca 2026 r. |
| Jakość | build, testy, format, audyt zależności i SonarQube są zielone dla commita bazowego | [`build` 29646657707](https://github.com/haribo841/Abituria/actions/runs/29646657707), [`sonarcloud` 29646657701](https://github.com/haribo841/Abituria/actions/runs/29646657701), `SONARQUBE.md` | PASS - commit `e0afeea`, 18 lipca 2026 r.; lokalne zmiany Issue #43 zweryfikowano 19 lipca |
| Instalacja lokalna | samowystarczalna publikacja uruchamia izolowany smoke test w nowym katalogu | `dotnet publish --self-contained true` oraz `Abituria.exe --release-smoke-test` | PASS - Windows 11 x64, 18 lipca 2026 r. |
| Instalacja niezależna automatyczna | każda deklarowana platforma buduje, pakuje, rozpakowuje i uruchamia aplikację na innym, natywnym runnerze | [`platform-installation-smoke` 29646667838](https://github.com/haribo841/Abituria/actions/runs/29646667838) | PASS - Windows, Ubuntu i macOS dla `e0afeea`, 18 lipca 2026 r. |
| Instalacja niezależna ręczna | osoba spoza środowiska budowania instaluje paczkę wydania i zapisuje wynik | opcjonalna tabela ręczna poniżej | NOT PERFORMED - dowód formalny niezależnych komputerów zapewniają trzy natywne, hostowane runnery; ręczna sesja jest uzupełniająca i nie blokuje odbioru |
| Publiczne wydanie | tag, paczki, sumy, SBOM, atestacje, draft i publikacja w GitHub Releases | `RELEASE_PROCESS.md`, workflow `release` | PENDING - proweniencja i jej walidatory są PASS, ale tag, workflow wydania i GitHub Release nie zostały wykonane |

## Odbiór zakresu Issue #42

| Wymagany element dokumentacji | Lokalizacja | Status |
| --- | --- | --- |
| cel, zakres, wymagania, moduły i architektura | `REQUIREMENTS.md`, `ARCHITECTURE.md`, `BUSINESS_ANALYSIS.md` | PASS |
| technologie, zależności i wymagania systemowe | `DEPENDENCIES.md`, `INSTALLATION.md`, `THIRD-PARTY-NOTICES.md` | PASS |
| instrukcja użytkownika i uruchomienia | `USER_GUIDE.md`, `INSTALLATION.md` | PASS |
| testy funkcjonalne, regresyjne, wydajnościowe i pamięciowe | `TESTING.md`, testy projektu | PASS po pełnej bramie automatycznej |
| użyteczność, instalacja niezależna i końcowy odbiór | `USABILITY_TEST_PROTOCOL.md`, `USABILITY_TEST_RESULTS.md`, ten dokument | PASS RETROSPEKTYWNY dla historycznych testów uczestników i odbioru; formalny test instalacji PASS na trzech niezależnych, natywnych runnerach |
| ograniczenia i status prawny zasobów | `KNOWN_LIMITATIONS.md`, `CONTENT_PROVENANCE.md`, `ASSET_RIGHTS_DECLARATION.md` | PASS - wszystkie grupy `approved`, `releaseEligible=true` i walidatory proweniencji PASS |
| autor, licencja i historia zmian | `AUTHORS.md`, `LICENSE`, `CHANGELOG.md` | PASS |
| PDF dla komisji | `output/pdf/Abituria-Technical-Documentation-0.9.0-beta.1.pdf` | PASS po wygenerowaniu i kontroli wizualnej |
| zatwierdzenie i przekazanie komisji | decyzja poniżej i `DELIVERY_PROTOCOL.md` | PASS RETROSPEKTYWNY - projekt zaakceptowany przez prowadzącego na początku lutego 2022 r.; dokładny dzień i podpis nie zostały zachowane |

## Odbiór zakresu Issue #43

Oddzielne protokoły zawierają rzeczywiste daty technicznych kamieni milowych bieżącej rekonstrukcji, retrospektywne decyzje techniczne, testy i ograniczenia. Właściciel projektu poświadczył 19 lipca 2026 r., że historyczne testy użyteczności z uczestnikami zakończyły się powodzeniem, a prowadzący zaakceptował projekt na początku lutego 2022 r. Dokładnego dnia i osobnych dat decyzji dla I-III przyrostu nie zachowano.

| Przyrost | Protokół | Rzeczywisty punkt techniczny | Stan formalny 19 lipca 2026 r. |
| --- | --- | --- | --- |
| I | [Protokół I](acceptance/INCREMENT_1_PROTOCOL.md) | 28 czerwca 2026 r., `a510ac6` | PASS techniczny; formalnie objęty końcową akceptacją prowadzącego z początku lutego 2022 r. |
| II | [Protokół II](acceptance/INCREMENT_2_PROTOCOL.md) | 7 lipca 2026 r., `95c0e8f` | PASS techniczny; formalnie objęty końcową akceptacją prowadzącego z początku lutego 2022 r. |
| III | [Protokół III](acceptance/INCREMENT_3_PROTOCOL.md) | 18 lipca 2026 r., `4fdecd2` | PASS techniczny i PASS RETROSPEKTYWNY testów uczestników; szczegóły sesji nie zostały zachowane |
| IV | [Protokół IV](acceptance/INCREMENT_4_PROTOCOL.md) | 18 lipca 2026 r., `e0afeea` | ACCEPTED - historyczny projekt i uzgodnione przekazanie przyjęte na początku lutego 2022 r.; bieżące publiczne wydanie jest osobne |

Zbiorczy rejestr i reguła zamknięcia znajdują się w [rejestrze Issue #43](acceptance/README.md). Retrospektywny wynik testów użyteczności i ograniczenia archiwalne zapisano w [USABILITY_TEST_RESULTS.md](USABILITY_TEST_RESULTS.md), a historycznie uzgodnioną formę przekazania i osobny stan publicznej publikacji w [DELIVERY_PROTOCOL.md](DELIVERY_PROTOCOL.md).

## Odbiór zakresu Issue #44

Publiczna obrona jest osobnym zdarzeniem historycznym od odbioru i przekazania opisanego dla Issue #43. Odbyła się dokładnie 17 stycznia 2022 r., a nagranie o widoczności `unlisted` jest dostępne przez link jako [Projekt Inz final](https://www.youtube.com/watch?v=Min_5DBHHnQ).

| Obszar | Dowód | Status |
| --- | --- | --- |
| data i publiczny charakter wydarzenia | metadane transmisji na żywo z 17 stycznia 2022 r. oraz zachowany zapis dostępny przez link | PASS |
| skład komisji | dr Paweł M. Owsianny, prof. UAM dr hab. Jerzy Szymański, dr Tomasz Piłka | PASS RETROSPEKTYWNY - poświadczenie właściciela wsparte nagraniem |
| prezentacja projektu | zachowane nagranie i poświadczenie właściciela | PASS RETROSPEKTYWNY |
| pokaz działającej aplikacji | poświadczenie właściciela i zapis wydarzenia | PASS RETROSPEKTYWNY |
| pytania i odpowiedzi | poświadczenie właściciela i zapis wydarzenia | PASS RETROSPEKTYWNY |
| decyzja komisji | pozytywna, wynik bardzo dobry | PASS RETROSPEKTYWNY |
| kamień milowy M7 | pełny protokół w `DEFENSE_PROTOCOL.md` | ACHIEVED |

Decyzja formalna dla Issue #44: `ACCEPTED - M7 ACHIEVED - READY TO CLOSE AS COMPLETED`. Dokładny przebieg, macierz kryteriów, nagranie i jawne ograniczenia dowodowe zawiera [DEFENSE_PROTOCOL.md](DEFENSE_PROTOCOL.md).

## Odbiór zakresu Issue #45

Issue #45 określało siedem obszarów oceny, dziesięć warunków akceptacji oraz warunki uzyskania wyniku bardzo dobrego. Kanoniczna macierz w [EVALUATION_PROTOCOL.md](EVALUATION_PROTOCOL.md) rozdziela historyczny produkt WPF oceniony w 2022 r. od bieżącej migracji AvaloniaUI `0.9.0-beta.1`.

| Zakres | Stan historyczny | Stan bieżącej migracji | Wniosek |
| --- | --- | --- | --- |
| prezentacja i obrona | PASS - nagranie potwierdza prezentację, demonstrację i Q&A | NOT APPLICABLE | kryterium komisji spełnione historycznie |
| produkt, wymagania i stabilność | PASS RETROSPEKTYWNY | PASS techniczny | historyczna akceptacja i obecne testy stanowią odrębne dowody |
| dokumentacja | PARTIAL historyczny | PASS lokalny | obecny pakiet uzupełnia braki bez twierdzenia, że istniał w 2022 r. |
| testowanie i użyteczność | PASS RETROSPEKTYWNY | PASS | historyczne sesje poświadczono, a obecny zestaw jest automatycznie weryfikowany |
| wdrożenie | PASS - `v1.0.0` przed obroną i `v1.0.1` po obronie | PENDING publicznego wydania | brak obecnego tagu nie unieważnia historycznego wdrożenia |
| praca zespołu i wkład | PARTIAL z powodu braków archiwalnych | PASS dla pracy jednoosobowej | nie rekonstruuje się opinii prowadzącego ani ocen indywidualnych |
| wynik | ACCEPTED, bardzo dobry według poświadczenia właściciela | TECHNICALLY ACCEPTED - PUBLIC RELEASE PENDING | bieżącej migracji nie przypisuje się oceny komisji |

Nagranie kończy się po zapowiedzi narady, przed ogłoszeniem wyniku. Pozytywna decyzja i wynik bardzo dobry są zatem zapisane jako poświadczenie właściciela, a nie jako fakt wypowiedziany w zachowanym materiale. Decyzja formalna dla Issue #45: `ACCEPTED - HISTORICAL CRITERIA SATISFIED - READY TO CLOSE AS COMPLETED`.

## Podstawa retrospektywnego odbioru

| Fakt | Zapis | Ograniczenie dowodu |
| --- | --- | --- |
| testy użyteczności | właściciel poświadczył, że testy z uczestnikami przeprowadzono i zakończyły się powodzeniem | brak liczby uczestników, dat sesji, komentarzy i kart wyników |
| poprawki z testów uczestników | brak zachowanego łańcucha od opinii do zmiany | żadna poprawka nie jest przypisywana uczestnikom przez domysł |
| późniejsze poprawki dostępności | H-01-H-03 wykonane i zweryfikowane 19 lipca 2026 r. | wynik przeglądu heurystycznego, nie badania uczestników |
| decyzja prowadzącego | projekt zaakceptowany na początku lutego 2022 r. | dokładny dzień, nazwisko i podpis nie zostały zachowane w repozytorium |
| forma przekazania | końcowa wersja projektu została przekazana prowadzącemu w uzgodnionej formie i przyjęta | kanał, dokładna rewizja i hash pakietu nie zostały zachowane |
| prawa do zasobów | właściciel poświadczył posiadanie praw do zasobów projektu | `ASSET_RIGHTS_DECLARATION.md`, `releaseEligible=true` i oba warianty walidatora PASS 19 lipca 2026 r. |

## Automatyczny test instalacji na niezależnych komputerach

Workflow [`platform-installation-smoke` 29646667838](https://github.com/haribo841/Abituria/actions/runs/29646667838) wykonał 18 lipca 2026 r. poniższe testy dla commitu `e0afeeaee30ed700fa5b8dc873409c23081106d4`. To mierzalny dowód techniczny na świeżych komputerach CI, a nie podpis niezależnego testera.

| System i architektura | Komputer wykonujący | Publikacja, archiwizacja i rozpakowanie | Izolowany smoke test | Wynik |
| --- | --- | --- | --- | --- |
| Windows 2025 x64 | GitHub Actions - natywny runner | PASS | PASS | PASS |
| Ubuntu 24.04 x64 | GitHub Actions - natywny runner | PASS | PASS | PASS |
| macOS 15 Intel x64 | GitHub Actions - natywny runner | PASS | PASS | PASS |

## Ręczny test instalacji na niezależnych komputerach

Formalny wymóg instalacji na komputerach niezależnych od środowiska autora spełnia pozytywny przebieg na trzech hostowanych, natywnych runnerach Windows, Ubuntu i macOS opisany powyżej. Runnery odtwarzają czyste środowiska, pakują aplikację, przenoszą ją przez archiwum, rozpakowują do nowego katalogu i uruchamiają rzeczywisty plik wykonywalny.

Poniższa tabela jest opcjonalnym uzupełnieniem dla sesji prowadzonej przez człowieka i nie jest bramą zamknięcia Issue #43. Jeżeli taka sesja zostanie wykonana przy publicznym wydaniu, należy pobrać paczkę z tego samego wydania co `SHA256SUMS.txt`, porównać pełną sumę SHA-256, rozpakować aplikację do nowego katalogu i wykonać smoke test zgodnie z `INSTALLATION.md`.

| Data | Wersja / hash paczki | System i architektura | Osoba testująca | Instalacja / uruchomienie | Smoke test | Uwagi | Podpis lub identyfikator potwierdzenia |
| --- | --- | --- | --- | --- | --- | --- | --- |
| do wypełnienia | do wypełnienia | Windows 11 x64 | niezależny tester |  |  |  |  |
| do wypełnienia | do wypełnienia | Ubuntu 24.04 x64 | niezależny tester |  |  |  |  |
| do wypełnienia | do wypełnienia | macOS 15 Intel x64 | niezależny tester |  |  |  |  |

## Decyzja odbiorowa

| Decyzja | Data | Osoba odpowiedzialna | Podpis / odnośnik do zatwierdzenia | Uwagi |
| --- | --- | --- | --- | --- |
| ZAAKCEPTOWANO projekt i uzgodnioną formę przekazania | początek lutego 2022 r.; dokładny dzień nie został zachowany | prowadzący projekt; imię i nazwisko nie zostały zapisane w repozytorium | retrospektywne poświadczenie właściciela zapisane 19 lipca 2026 r.; historycznego podpisu lub odnośnika nie zachowano | decyzja dotyczy historycznego odbioru Issue #43, nie jest twierdzeniem o publicznym GitHub Release `0.9.0-beta.1` |
| POZYTYWNA decyzja komisji, wynik bardzo dobry, M7 osiągnięty | 17 stycznia 2022 r. | dr Paweł M. Owsianny, prof. UAM dr hab. Jerzy Szymański, dr Tomasz Piłka | nagranie dostępne przez link, poświadczenie właściciela i `DEFENSE_PROTOCOL.md` | nagranie kończy się przed ogłoszeniem decyzji; wynik dotyczy historycznej obrony, a nie migracji `0.9.0-beta.1` |

## Warunek publikacji

Właściciel poświadczył posiadanie praw do zasobów projektu. Deklarację zapisano w `ASSET_RIGHTS_DECLARATION.md`, wszystkie grupy w `Content/provenance.json` mają status `approved`, `releaseEligible=true`, a oba warianty walidatora proweniencji przeszły 19 lipca 2026 r. Historyczne przekazanie prowadzącemu zostało uzgodnione i zaakceptowane, dlatego spełnia formalny wariant przekazania dla Issue #43.

Publiczne wydanie bieżącej wersji jest osobnym działaniem. Nie wolno twierdzić, że GitHub Release istnieje, ponieważ tag i workflow wydania nie zostały wykonane. Przed publikacją należy ponowić `tools/Test-ContentProvenance.ps1 -RequireReleaseEligible` oraz pozostałe bramy na dokładnym commicie przeznaczonym do wydania, skontrolować draft i dopiero potem opublikować prerelease.

Nieuruchamialny kandydat dokumentacyjny bieżącej wersji jest opisany w `DELIVERY_PROTOCOL.md`. Jest opcjonalnym artefaktem, a nie podstawą historycznej decyzji.

## Checklista zamknięcia Issue #43

- [x] osobne protokoły I-IV z rzeczywistymi datami technicznymi;
- [x] dowody techniczne i retrospektywne decyzje techniczne;
- [x] techniczny przegląd dostępności i poprawki H-01-H-03;
- [x] pozytywny wynik historycznych testów użyteczności z uczestnikami poświadczony przez właściciela, z jawnym brakiem szczegółów sesji;
- [x] brak zachowanego powiązania poprawek z uczestnikami ujawniony bez tworzenia fikcyjnych danych;
- [x] końcowa decyzja prowadzącego: projekt zaakceptowany na początku lutego 2022 r.;
- [x] historyczna, uzgodniona forma przekazania przyjęta przez prowadzącego;
- [x] poświadczenie właściciela o posiadaniu praw do zasobów zapisane z rozdzieleniem od bieżącej bramy publikacji;
- [x] publiczny GitHub Release oznaczony jako osobne działanie PENDING, a nie jako istniejący dowód.

Decyzja formalna dla Issue #43: `ACCEPTED - READY TO CLOSE`. Ograniczenia archiwalne pozostają jawne, a publikacja `0.9.0-beta.1` jest śledzona niezależnie.

## Checklista zamknięcia Issue #44

- [x] rzeczywista data 17 stycznia 2022 r.;
- [x] pełny skład komisji i role;
- [x] pokaz działającej aplikacji;
- [x] pytania i odpowiedzi;
- [x] pozytywna decyzja i wynik bardzo dobry;
- [x] publiczny odnośnik do nagrania;
- [x] osobny protokół M7 dołączony do pakietu komisji;
- [x] publiczna obrona rozdzielona od późniejszego odbioru prowadzącego i od bieżącego procesu wydania.

Decyzja formalna dla Issue #44: `ACCEPTED - M7 ACHIEVED - READY TO CLOSE AS COMPLETED`.

## Checklista zamknięcia Issue #45

- [x] oceniono wszystkie siedem obszarów wskazanych w issue;
- [x] oceniono wszystkie dziesięć warunków akceptacji;
- [x] oceniono warunki uzyskania wyniku bardzo dobrego;
- [x] zapisano dowody prezentacji, demonstracji, Q&A, testów i historycznego wdrożenia;
- [x] ujawniono, że nagranie nie obejmuje ogłoszenia decyzji;
- [x] ujawniono brak podpisanej karty oceny, osobnej opinii prowadzącego i ocen indywidualnych;
- [x] rozdzielono autorstwo bieżącej implementacji od historycznego procesu zespołowego;
- [x] rozdzielono historyczną akceptację od nieopublikowanego wydania bieżącej migracji;
- [x] protokół `EVALUATION_PROTOCOL.md` dołączono do indeksu, PDF i pakietu dokumentacyjnego;
- [x] przygotowano komentarz zamykający oparty na trwałych odnośnikach.

Decyzja formalna dla Issue #45: `ACCEPTED - HISTORICAL CRITERIA SATISFIED - READY TO CLOSE AS COMPLETED`. Zdalne issue należy zamknąć jako `completed` po utrwaleniu tego protokołu w repozytorium.
