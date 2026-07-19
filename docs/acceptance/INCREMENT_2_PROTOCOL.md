# Protokół II przyrostu

## Identyfikacja i daty

| Pole | Wartość |
| --- | --- |
| wymaganie | Issue #43 - II przyrost |
| termin nominalny w Issue #43 | do 11 czerwca, bez wskazanego roku |
| historyczny odbiór oryginalnego projektu | zaakceptowany zbiorczo na początku lutego 2022 r.; dokładny dzień nie zachował się |
| podstawa zapisu historycznego | oświadczenie właściciela projektu z 19 lipca 2026 r. |
| techniczny kamień milowy migracji | 7 lipca 2026 r. |
| commit kamienia milowego | `95c0e8fc4c0dec0cc8e813b73ab71fe8f5d82904` - `Add password change validation tests` |
| wersja użyta do ponownej weryfikacji | `0.9.0-beta.1`, commit `e0afeeaee30ed700fa5b8dc873409c23081106d4` |

Nie zachował się osobny protokół odbioru II przyrostu. Końcowe zatwierdzenie oryginalnego projektu na początku lutego 2022 r. zbiorczo ratyfikuje ten etap. Data 7 lipca 2026 r. jest rzeczywistą datą technicznego stanu migracji, a nie datą przypisaną wstecz do 11 czerwca.

## Zakres planowany

- rozwinięta nawigacja i stabilny kalkulator;
- treści matematyczne i zadania;
- sprawdzanie odpowiedzi i podstawowe podpowiedzi;
- obsługa błędnych danych;
- testy funkcjonalne i regresyjne;
- poprawki z I odbioru oraz aktualna dokumentacja.

## Rekonstrukcja techniczna zakresu

Commit `95c0e8f` obejmował kalkulator ogólny z kontrolowanymi błędami i historią, kalkulator funkcji kwadratowej, treści, zadania, odpowiedzi zamknięte i otwarte, podpowiedzi, postęp, nawigację w jednym oknie oraz testy regresyjne i headless UI. Drzewo zawierało 23 klasy testowe i 11 dokumentów Markdown.

## Wyniki testów i dowody techniczne

| Dowód | Wynik |
| --- | --- |
| [build dla `95c0e8f`](https://github.com/haribo841/Abituria/actions/runs/28856922957) | PASS |
| [SonarCloud dla `95c0e8f`](https://github.com/haribo841/Abituria/actions/runs/28856923027) | PASS |
| kalkulatory i błędne dane | PASS - testy kalkulatora i testy odporności |
| nawigacja, odpowiedzi, podpowiedzi i postęp | PASS - testy headless UI |
| ponowna pełna weryfikacja bieżącej wersji | PASS - wynik w `../TESTING.md` |

## Zachowane ograniczenia dokumentacyjne

- brak osobnego dokumentu z zakresem formalnie zatwierdzonym dla II przyrostu;
- brak zachowanej listy szczegółowych uwag po I odbiorze;
- dokładny dzień końcowego zatwierdzenia, nazwisko prowadzącego i podpis nie zostały podane;
- aktywny dokument wymagań migracji dodano 11 lipca 2026 r. w `d6dd0e4`;
- dowody z 2026 r. opisują migrację i nie rekonstruują przebiegu spotkania historycznego.

## Uwagi prowadzącego

Nie zachowały się szczegółowe uwagi przypisane do II przyrostu ani ich bezpośrednie powiązanie z poprawkami. Nie zostały wymyślone.

## Decyzje

| Rodzaj decyzji | Wynik | Data | Podstawa i ograniczenia |
| --- | --- | --- | --- |
| historyczny odbiór oryginalnego projektu | ACCEPTED zbiorczo | początek lutego 2022 r.; dokładny dzień nie zachował się | oświadczenie właściciela z 19 lipca 2026 r.; nazwiska i podpisu nie podano |
| techniczna weryfikacja migracji | PASS | 19 lipca 2026 r. | zakres II działa i jest chroniony testami |

## Zakres następnego przyrostu

Kolejne działy, losowanie zadań, stabilność, wydajność, pamięć, ergonomia, regresje wizualne, badanie użyteczności i poprawki wynikające z jego ustaleń.

