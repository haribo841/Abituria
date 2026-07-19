# Protokół III przyrostu

## Identyfikacja i daty

| Pole | Wartość |
| --- | --- |
| wymaganie | Issue #43 - III przyrost |
| termin nominalny w Issue #43 | połowa semestru, bez konkretnej daty |
| historyczny odbiór oryginalnego projektu | zaakceptowany zbiorczo na początku lutego 2022 r.; dokładny dzień nie zachował się |
| historyczny wynik testów z uczestnikami | pozytywny według oświadczenia właściciela; szczegóły nie zachowały się |
| podstawa zapisu historycznego | oświadczenie właściciela projektu z 19 lipca 2026 r. |
| techniczny kamień milowy migracji | 18 lipca 2026 r. |
| commit kamienia milowego | `4fdecd29111c1896f9bf48fd2bb4d1d9e26771fc` - `Fix cross-platform release validation runners` |
| wersja użyta do ponownej weryfikacji | `0.9.0-beta.1`, commit bazowy `e0afeeaee30ed700fa5b8dc873409c23081106d4` |

Commit `14b2c27` nie jest końcowym technicznym punktem odniesienia, ponieważ jego build, SonarCloud i test platformowy nie były zielone. `4fdecd2` jest pierwszym późniejszym stanem z kompletem pozytywnych dowodów automatycznych dla zakresu III.

Historyczne testy z uczestnikami dotyczyły oryginalnego projektu. Oświadczenie nie podaje liczby osób, scenariuszy, komentarzy ani podziału na rundy, dlatego protokół zapisuje wyłącznie potwierdzony wynik pozytywny.

## Zakres planowany

- kolejne działy i zadania maturalne;
- podpowiedzi krokowe i losowanie zadań;
- poprawa ergonomii, stabilności i wydajności;
- usunięcie duplikacji i martwego kodu;
- testy użyteczności z uczestnikami;
- zebrane opinie i wdrożone najważniejsze poprawki;
- aktualna dokumentacja.

## Rekonstrukcja techniczna zakresu

Stan `4fdecd2` obejmuje 7 dostępnych działów, 18 tablic, 35 zadań, podpowiedzi krokowe, odpowiedzi, postęp, losowanie z całej puli i tematu, regresje wizualne, formalne testy wydajności, pamięci i obciążenia oraz instalacyjny smoke test na trzech systemach.

19 lipca 2026 r. wykonano ponadto techniczny przegląd dostępności migracji. Jego wyniki są dowodem inżynierskim i pozostają oddzielone od historycznych testów z uczestnikami.

## Wyniki testów i dowody

| Dowód | Wynik |
| --- | --- |
| historyczne testy z uczestnikami oryginalnego projektu | PASS według oświadczenia właściciela; brak zachowanych danych szczegółowych |
| [build dla `4fdecd2`](https://github.com/haribo841/Abituria/actions/runs/29646446887) | PASS |
| [SonarCloud dla `4fdecd2`](https://github.com/haribo841/Abituria/actions/runs/29646446881) | PASS |
| [instalacja CI na Windows, Ubuntu i macOS](https://github.com/haribo841/Abituria/actions/runs/29646454681) | PASS |
| testy wydajności, pamięci i obciążenia migracji | PASS - `../TESTING.md` |
| techniczny przegląd dostępności migracji | PASS lokalnie - `AccessibilityRegressionTests` |

## Zachowane ograniczenia dokumentacyjne

- nominalny termin nie został w Issue #43 zastąpiony konkretną datą;
- nie zachowały się liczba uczestników, indywidualne karty, komentarze ani szczegółowa lista poprawek historycznego badania;
- oświadczenie nie pozwala rozdzielić testów na dokładnie datowane rundy;
- nazwisko prowadzącego i podpis nie zostały podane;
- techniczny przegląd z 2026 r. nie jest przedstawiany jako historyczne badanie z użytkownikami.

## Uwagi prowadzącego i uczestników

Szczegółowe uwagi nie zachowały się i nie zostały odtworzone. Zachowany retrospektywnie fakt ogranicza się do pozytywnego wyniku testów z uczestnikami i końcowego zatwierdzenia projektu.

## Decyzje

| Rodzaj decyzji | Wynik | Data | Podstawa i ograniczenia |
| --- | --- | --- | --- |
| historyczny odbiór oryginalnego projektu | ACCEPTED zbiorczo | początek lutego 2022 r.; dokładny dzień nie zachował się | oświadczenie właściciela; nazwiska i podpisu nie podano |
| historyczne testy z uczestnikami | PASS | data szczegółowa nie zachowała się | oświadczenie właściciela; liczby uczestników i komentarzy nie podano |
| techniczna weryfikacja migracji | PASS | 19 lipca 2026 r. | część implementacyjna i automatyczna zakresu III jest zaliczona |

## Zakres następnego przyrostu

Końcowa dokumentacja, instalacja, weryfikacja całego uzgodnionego zakresu i przygotowanie procesu wydawniczego obecnej migracji.

