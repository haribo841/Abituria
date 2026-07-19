# Protokół I przyrostu

## Identyfikacja i daty

| Pole | Wartość |
| --- | --- |
| wymaganie | Issue #43 - I przyrost |
| termin nominalny w Issue #43 | połowa maja, bez wskazanego roku |
| historyczny odbiór oryginalnego projektu | zaakceptowany zbiorczo na początku lutego 2022 r.; dokładny dzień nie zachował się |
| podstawa zapisu historycznego | oświadczenie właściciela projektu z 19 lipca 2026 r. |
| techniczny kamień milowy migracji | 28 czerwca 2026 r. |
| commit kamienia milowego | `a510ac632efa5086915880222e975c93dbb82159` - `Complete Avalonia migration` |
| wejście tego stanu na `main` | 29 czerwca 2026 r., `2957b43` |
| wersja użyta do ponownej weryfikacji | `0.9.0-beta.1`, commit `e0afeeaee30ed700fa5b8dc873409c23081106d4` |

Nie zachował się osobny protokół odbioru I przyrostu. Końcowe zatwierdzenie oryginalnego projektu przez prowadzącego na początku lutego 2022 r. jest dlatego zapisane jako zbiorcza, retrospektywna ratyfikacja tego etapu. Data techniczna z 2026 r. opisuje migrację Avalonia i nie jest historyczną datą decyzji prowadzącego.

## Zakres planowany

- ekran startowy i prototyp interfejsu;
- podstawowa nawigacja;
- kalkulator albo jego podstawowe funkcje;
- co najmniej jeden dział matematyczny;
- wstępna obsługa danych wejściowych;
- podstawowa dokumentacja i scenariusze testowe;
- jedna funkcja biznesowa możliwa do demonstracji od początku do końca.

## Rekonstrukcja techniczna zakresu

Commit `a510ac6` zawierał shell Avalonia, logowanie, ekran główny, nawigację, kalkulator funkcji kwadratowej, dział Wektory, 18 tablic, 35 zadań, odpowiedzi, podpowiedzi, SQLite i pierwsze testy automatyczne. Kalkulator ogólny pozostawał wtedy pozycją planowaną.

Bieżąca wersja `e0afeea` zachowuje i rozszerza ten zakres. Jest to dowód techniczny ciągłości funkcji, a nie zamiennik historycznej decyzji.

## Wyniki testów i dowody techniczne

| Dowód | Wynik |
| --- | --- |
| [build dla `a510ac6`](https://github.com/haribo841/Abituria/actions/runs/28303713453) | PASS |
| `AccountServiceTests`, `ContentInventoryTests`, `QuadraticSolverTests` w drzewie kamienia milowego | obecne |
| ponowna pełna weryfikacja bieżącej wersji | PASS - wynik w `../TESTING.md` |
| nawigacja i pojedyncze okno | PASS - `NavigationArchitectureTests` |
| przepływ zadania, odpowiedzi i podpowiedzi | PASS - `ExerciseAndRoutingCoverageTests` |

## Zachowane ograniczenia dokumentacyjne

- brak osobnego protokołu z nominalnego terminu;
- dokładny dzień końcowego zatwierdzenia nie zachował się;
- nazwisko prowadzącego, szczegółowe uwagi i podpis nie zostały podane;
- końcowa akceptacja całego projektu nie jest przedstawiana jako osobna decyzja z połowy maja;
- techniczna rekonstrukcja z 2026 r. dotyczy migracji, nie oryginalnego wykonania.

## Uwagi prowadzącego

Nie zachowały się szczegółowe uwagi przypisane do I przyrostu. Nie zostały one odtworzone ani zastąpione domyślną treścią.

## Decyzje

| Rodzaj decyzji | Wynik | Data | Podstawa i ograniczenia |
| --- | --- | --- | --- |
| historyczny odbiór oryginalnego projektu | ACCEPTED zbiorczo | początek lutego 2022 r.; dokładny dzień nie zachował się | oświadczenie właściciela z 19 lipca 2026 r.; nazwiska i podpisu nie podano |
| techniczna weryfikacja migracji | PASS | 19 lipca 2026 r. | zakres I jest zachowany i chroniony testami |

## Zakres następnego przyrostu

Stabilizacja kalkulatora ogólnego, pełniejsze testy regresyjne i headless UI, rozwinięcie obsługi kont, zadań, błędów oraz dokumentacji wymagań.

