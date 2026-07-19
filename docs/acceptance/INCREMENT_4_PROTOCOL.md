# Protokół IV przyrostu

## Identyfikacja i daty

| Pole | Wartość |
| --- | --- |
| wymaganie | Issue #43 - IV przyrost |
| termin nominalny w Issue #43 | odbiór końcowy, bez konkretnej daty |
| historyczny odbiór oryginalnego projektu | zaakceptowany przez prowadzącego na początku lutego 2022 r.; dokładny dzień nie zachował się |
| historyczny wynik testów z uczestnikami | pozytywny według oświadczenia właściciela; szczegóły nie zachowały się |
| status praw do materiałów | właściciel oświadczył, że posiada prawa potrzebne do ich wykorzystywania w projekcie |
| podstawa zapisów historycznych | oświadczenie właściciela projektu z 19 lipca 2026 r. |
| techniczny kamień milowy migracji | 18 lipca 2026 r. |
| commit kamienia milowego | `e0afeeaee30ed700fa5b8dc873409c23081106d4` - `Record independent installation validation evidence` |
| wersja produktu migracji | `0.9.0-beta.1` |

Historyczne zatwierdzenie dotyczy oryginalnego projektu i zbiorczo ratyfikuje wcześniejsze przyrosty. Dowody z 2026 r. dotyczą technicznej rekonstrukcji i migracji Avalonia. Nie są przedstawiane jako powtórny odbiór przez prowadzącego.

## Zakres planowany

- kompletny uzgodniony zakres funkcjonalny;
- stabilna nawigacja, kalkulatory, treści, zadania, odpowiedzi i podpowiedzi;
- obsługa błędów;
- testy funkcjonalne, regresyjne i użyteczności;
- dokumentacja użytkownika i techniczna;
- instrukcja instalacji, licencje, autor, zależności i historia zmian;
- paczka wdrożeniowa i publiczne wydanie;
- zatwierdzenie prowadzącego.

## Rekonstrukcja techniczna zakresu

Commit `e0afeea` jest pierwszym stanem migracji z kompletem bieżących dowodów technicznych: pełnym testem automatycznym, zielonym buildem i SonarCloud, wdrożoną dokumentacją Pages, PDF dla komisji oraz natywnym smoke testem paczek na Windows, Ubuntu i macOS.

Wymagania F-01-F-22 oraz NF-01-NF-14 dla przyjętego zakresu beta mają dowody implementacyjne. Funkcje spoza beta są jawnie oznaczone jako planowane albo wyłączone.

## Wyniki testów i dowody

| Dowód | Wynik |
| --- | --- |
| końcowe zatwierdzenie oryginalnego projektu przez prowadzącego | PASS retrospektywnie - początek lutego 2022 r., dokładny dzień nie zachował się |
| historyczne testy z uczestnikami | PASS według oświadczenia właściciela; brak zachowanych danych szczegółowych |
| prawa do materiałów | potwierdzone oświadczeniem właściciela |
| [build dla `e0afeea`](https://github.com/haribo841/Abituria/actions/runs/29646657707) | PASS |
| [SonarCloud dla `e0afeea`](https://github.com/haribo841/Abituria/actions/runs/29646657701) | PASS |
| [GitHub Pages dla `e0afeea`](https://github.com/haribo841/Abituria/actions/runs/29646657748) | PASS |
| [platformowy test instalacyjny dla `e0afeea`](https://github.com/haribo841/Abituria/actions/runs/29646667838) | PASS na Windows, Ubuntu i macOS |
| pełny lokalny zestaw testów migracji | PASS - aktualny wynik w `../TESTING.md` |
| publiczny tag i GitHub Release obecnej migracji | NOT PERFORMED - osobny proces wydawniczy |

## Zachowane ograniczenia dokumentacyjne

- dokładny dzień zatwierdzenia, nazwisko prowadzącego i podpis nie zostały podane;
- nie zachowały się liczba uczestników, indywidualne wyniki, komentarze ani podział testów na rundy;
- dokładna forma historycznego przekazania nie została opisana;
- nie opublikowano tagu ani GitHub Release dla obecnej migracji `0.9.0-beta.1`;
- oświadczenie o prawach i bieżący manifest proweniencji pozwalają rozpocząć osobny proces wydawniczy, ale nie stanowią dowodu jego wykonania.

## Uwagi prowadzącego i uczestników

Szczegółowe uwagi, nazwiska, podpisy i dane uczestników nie zostały podane. Nie zostały uzupełnione przez domysł. Zachowany retrospektywnie wynik to pozytywne testy uczestników i zatwierdzenie oryginalnego projektu.

## Decyzje

| Rodzaj decyzji | Wynik | Data | Podstawa i ograniczenia |
| --- | --- | --- | --- |
| historyczny odbiór końcowy oryginalnego projektu | ACCEPTED | początek lutego 2022 r.; dokładny dzień nie zachował się | oświadczenie właściciela; nazwiska i podpisu nie podano |
| historyczne testy z uczestnikami | PASS | data szczegółowa nie zachowała się | oświadczenie właściciela; liczby uczestników i komentarzy nie podano |
| techniczna weryfikacja migracji | PASS | 19 lipca 2026 r. | funkcje, testy, dokumentacja i instalacja CI mają pozytywne dowody |
| publikacja migracji w GitHub Releases | NOT PERFORMED | stan na 19 lipca 2026 r. | osobne zadanie, nie zmienia historycznej decyzji Issue #43 |

## Wniosek dla Issue #43

Oryginalny projekt został zatwierdzony, a testy z uczestnikami miały wynik pozytywny. W połączeniu z jawną rekonstrukcją techniczną pozwala to zamknąć Issue #43 bez wymyślania brakujących szczegółów archiwalnych.

Publiczne wydanie obecnej migracji w GitHub Releases pozostaje niewykonanym, osobnym zadaniem. Oświadczenie o prawach jest udokumentowane w `../ASSET_RIGHTS_DECLARATION.md` i bieżącym manifeście, lecz tag oraz workflow wydawniczy nie zostały wykonane.
