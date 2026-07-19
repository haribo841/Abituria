# Protokół testów użyteczności i dostępności

Wersja: `0.9.0-beta.1`.

## Cel

Protokół sprawdza, czy uczeń może rozpocząć naukę, odnaleźć materiał, rozwiązać zadanie, skorzystać z podpowiedzi oraz wykonać obliczenie bez instrukcji programistycznej. Uzupełnia automatyczne testy UI, ale ich nie zastępuje.

Właściciel projektu poświadczył 19 lipca 2026 r., że historyczne testy użyteczności z uczestnikami zakończyły się powodzeniem, a prowadzący zaakceptował projekt na początku lutego 2022 r. Dokładne daty sesji, liczba i dane uczestników, komentarze oraz karty wyników nie zostały zachowane w aktualnym repozytorium i nie są odtwarzane przez domysł.

Retrospektywny zapis wyniku, techniczny przegląd heurystyczny, ograniczenia dowodu oraz zasady ewentualnego powtórzenia badania znajdują się w [USABILITY_TEST_RESULTS.md](USABILITY_TEST_RESULTS.md). Scenariusze poniżej są powtarzalnym protokołem dla bieżącej wersji. Nie są przedstawiane jako dokładny formularz użyty w historycznych sesjach.

## Uczestnicy i środowisko

Poniższe wymagania obowiązują przy ponownym, szczegółowo dokumentowanym badaniu bieżącej wersji. Nie należy na ich podstawie rekonstruować liczby ani charakterystyki historycznych uczestników.

- minimum trzech osób, z których co najmniej dwie nie budowały aplikacji;
- co najmniej jedna osoba korzystająca z klawiatury jako głównego sposobu obsługi;
- każdy test na czystym profilu użytkownika i w rozdzielczości co najmniej `960x640`;
- jeden test na komputerze innym niż komputer budujący wydanie;
- prowadzący zapisuje wersję paczki, system, wynik zadania i obserwowane bariery.

## Scenariusze

| ID | Zadanie dla uczestnika | Kryterium powodzenia | Dowód automatyczny |
| --- | --- | --- |
| U-01 | Zaloguj się jako gość i wróć na ekran Start. | użytkownik odnajduje profil i widzi główną nawigację | `ExerciseAndRoutingCoverageTests` |
| U-02 | Otwórz wybrany dział i wróć do listy. | treść jest czytelna, a powrót nie tworzy drugiego okna | `NavigationArchitectureTests`, regresje wizualne |
| U-03 | Wylosuj zadanie z całego arkusza, a potem z wybranego tematu. | otwiera się zadanie z właściwej puli, poprzednie i następne zachowują kontekst | `ExerciseRandomizerTests` |
| U-04 | Rozwiąż zadanie zamknięte, użyj podpowiedzi i sprawdź odpowiedź. | użytkownik dostaje czytelny wynik, poprawna odpowiedź zapisuje postęp | `ExerciseAndRoutingCoverageTests` |
| U-05 | Otwórz zadanie otwarte, pokaż odpowiedź i sprawdź profil. | ujawnienie odpowiedzi jest jawne, postęp jest widoczny | `ExerciseAndRoutingCoverageTests` |
| U-06 | Oblicz poprawne wyrażenie, a potem wprowadź dzielenie przez zero. | poprawny wynik i zrozumiały błąd bez zamknięcia programu | `ExpressionCalculatorRobustnessTests` |
| U-07 | Otwórz historię kalkulatora po wielu obliczeniach. | historia jest przewijalna i osiągalna | `Discussion10VisualRegressionTests` |
| U-08 | Przejdź klawiaturą po głównych przyciskach i polach. | fokus jest widoczny, etykiety przycisków są zrozumiałe | obserwacja ręczna, wyniki poniżej |

## Skala wyniku

- `PASS` - zadanie wykonane bez pomocy i bez błędu krytycznego.
- `PASS Z UWAGĄ` - zadanie wykonane po krótkiej wskazówce lub z niekrytyczną trudnością.
- `FAIL` - zadanie nie zostało wykonane albo wystąpił błąd krytyczny.

Błąd krytyczny to zamknięcie aplikacji, utrata danych, brak możliwości kontynuacji albo element nieosiągalny bez obejścia. Każdy `FAIL` blokuje końcowy odbiór do czasu poprawy i powtórzenia scenariusza.

## Arkusz wyników

Tabela jest szablonem dla ewentualnego powtórzenia badania `0.9.0-beta.1`. Puste pola nie podważają poświadczonego historycznego wyniku, ale nie mogą być używane jako dowód nowej sesji.

| Data | Wersja i suma SHA-256 | Uczestnik | System i rozdzielczość | U-01 | U-02 | U-03 | U-04 | U-05 | U-06 | U-07 | U-08 | Uwagi / zgłoszenie |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| do wypełnienia | do wypełnienia | do wypełnienia | do wypełnienia |  |  |  |  |  |  |  |  |  |
| do wypełnienia | do wypełnienia | do wypełnienia | do wypełnienia |  |  |  |  |  |  |  |  |  |
| do wypełnienia | do wypełnienia | do wypełnienia | do wypełnienia |  |  |  |  |  |  |  |  |  |

## Powiązanie z odbiorami III i IV

- historyczne testy uczestników mają zbiorczy wynik `PASS` na podstawie poświadczenia właściciela;
- brak zachowanych kart nie pozwala wiarygodnie przypisać konkretnych sesji, problemów i retestów do osobnych rund przed III i IV odbiorem;
- końcowa akceptacja prowadzącego na początku lutego 2022 r. jest poświadczoną decyzją formalną zamykającą historyczny cykl odbioru;
- techniczny przegląd heurystyczny i testy Avalonia Headless z 2026 r. są późniejszym dowodem regresyjnym i nie są przedstawiane jako badanie uczestników;
- ograniczenie archiwalne oraz wynik zbiorczy są przenoszone do osobnych protokołów III i IV w `acceptance/`.

## Wniosek

Dla historycznego Issue #43 przeprowadzenie i pozytywny wynik testów z uczestnikami są zapisane jako poświadczenie retrospektywne, a nie jako odtworzone karty sesji. Projekt został zaakceptowany przez prowadzącego na początku lutego 2022 r., przy czym dokładny dzień nie został zachowany.

Przy ponownym badaniu bieżącej wersji prowadzący sesję powinien zapisać datę, uczestników, wyniki, uwagi, poprawki i retesty. Pełny audyt WCAG i test z czytnikiem ekranu pozostają osobnym, przyszłym zakresem opisanym w `KNOWN_LIMITATIONS.md`.
