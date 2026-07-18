# Protokół testów użyteczności i dostępności

Wersja: `0.9.0-beta.1`.

## Cel

Protokół sprawdza, czy uczeń może rozpocząć naukę, odnaleźć materiał, rozwiązać zadanie, skorzystać z podpowiedzi oraz wykonać obliczenie bez instrukcji programistycznej. Uzupełnia automatyczne testy UI, ale ich nie zastępuje.

Nie przeprowadzono jeszcze badania z niezależnymi użytkownikami. Ten dokument nie przedstawia hipotetycznych odpowiedzi ankietowych jako wyników.

## Uczestnicy i środowisko

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

| Data | Wersja i suma SHA-256 | Uczestnik | System i rozdzielczość | U-01 | U-02 | U-03 | U-04 | U-05 | U-06 | U-07 | U-08 | Uwagi / zgłoszenie |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| do wypełnienia | do wypełnienia | do wypełnienia | do wypełnienia |  |  |  |  |  |  |  |  |  |
| do wypełnienia | do wypełnienia | do wypełnienia | do wypełnienia |  |  |  |  |  |  |  |  |  |
| do wypełnienia | do wypełnienia | do wypełnienia | do wypełnienia |  |  |  |  |  |  |  |  |  |

## Wniosek

Po wykonaniu wszystkich scenariuszy prowadzący dopisuje datę, liczbę wyników `PASS`, listę uwag oraz decyzję. Dopiero wtedy można twierdzić, że przeprowadzono ręczny test użyteczności. Pełny audyt WCAG i test z czytnikiem ekranu pozostają osobnym, przyszłym zakresem opisanym w `KNOWN_LIMITATIONS.md`.
