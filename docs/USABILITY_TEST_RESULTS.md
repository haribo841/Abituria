# Wyniki testów użyteczności i rejestr poprawek

Wersja bazowa bieżącej weryfikacji technicznej: `0.9.0-beta.1`, commit `e0afeeaee30ed700fa5b8dc873409c23081106d4`.

Data zapisania dowodu retrospektywnego: 19 lipca 2026 r.

Scenariusze i kryteria dla powtarzalnego badania bieżącej wersji są zdefiniowane w `USABILITY_TEST_PROTOCOL.md`.

## Status dowodów

Właściciel projektu poświadczył 19 lipca 2026 r., że w historycznym cyklu realizacji przeprowadzono testy użyteczności z uczestnikami i zakończyły się one powodzeniem. Projekt został następnie zaakceptowany przez prowadzącego na początku lutego 2022 r. Dokładny dzień odbioru nie został zachowany.

To jest retrospektywne, zbiorcze poświadczenie wyniku. W aktualnym repozytorium nie zachowano kart poszczególnych sesji, dlatego dokument nie ustala ani nie odtwarza przez domysł:

- liczby i danych uczestników;
- dat poszczególnych sesji;
- użytych wersji, systemów i sum SHA-256;
- wyników pojedynczych scenariuszy;
- komentarzy uczestników;
- listy problemów, poprawek wynikających z opinii uczestników ani ich retestów.

Na potrzeby formalnego śladu Issue #43 historyczny wynik testów uczestników ma status `PASS - poświadczenie retrospektywne z ograniczeniem archiwalnym`. Brak szczegółowych kart jest jawnym ograniczeniem dowodu, ale nie zmienia poświadczonej decyzji prowadzącego o przyjęciu projektu. Nie jest to dowód badania bieżącej wersji `0.9.0-beta.1` ani dowód publicznego wydania.

## Retrospektywne poświadczenie właściciela

| Pole | Zapis |
| --- | --- |
| data zapisania poświadczenia | 19 lipca 2026 r. |
| okres historycznych testów | przed końcowym odbiorem projektu; dokładne daty sesji nie zostały zachowane |
| rodzaj testu | testy użyteczności z uczestnikami |
| wynik zbiorczy | PASS - testy zakończyły się powodzeniem |
| decyzja końcowa prowadzącego | projekt zaakceptowany na początku lutego 2022 r.; dokładny dzień nie został zachowany |
| szczegóły sesji | brak zachowanego rejestru; nie są rekonstruowane |
| poprawki przypisywane uczestnikom | brak zachowanego powiązania; żadna późniejsza zmiana nie jest przypisywana uczestnikom bez dowodu |
| zakres formalny | historyczny odbiór Issue #43, bez twierdzenia o istnieniu GitHub Release |

## Techniczny przegląd heurystyczny - 19 lipca 2026 r.

Zakres: scenariusze U-04, U-06 i U-08, logiczne drzewo kontrolek Avalonia, obsługa klawiatury, nazwy automatyzacji oraz ogłaszanie dynamicznych wyników.

Środowisko: Windows 11 x64, .NET SDK `10.0.302`, konfiguracja `Release`.

| ID | Obserwacja | Ważność | Poprawka | Test regresyjny | Stan |
| --- | --- | --- | --- | --- | --- |
| H-01 | przyciski poprzedniego i następnego zadania prezentowały tylko symbole strzałek; pełny tytuł był wyłącznie w dymku | wysoka dla technologii asystujących | pełne nazwy "Poprzednie zadanie" i "Następne zadanie" wraz z tytułem celu | `AccessibilityRegressionTests.Exercise_navigation_and_feedback_have_descriptive_automation_metadata` | PASS lokalnie |
| H-02 | pola współczynników, wyrażenia i brudnopisu opierały identyfikację na placeholderach | średnia | stałe nazwy automatyzacji dla wszystkich tych pól | oba testy `AccessibilityRegressionTests` | PASS lokalnie |
| H-03 | wyniki kalkulatorów, status konta, podpowiedź i wynik odpowiedzi nie były oznaczone jako dynamiczne regiony | wysoka dla czytników ekranu | `AutomationLiveSetting.Polite` i opisowe nazwy regionów | oba testy `AccessibilityRegressionTests` | PASS lokalnie |

Wynik ukierunkowanego przebiegu po poprawkach: `2/2` testów `AccessibilityRegressionTests`, 0 błędów. Wynik pełnej bramy znajduje się w `TESTING.md`.

Poprawki H-01-H-03 wynikają wyłącznie z późniejszego przeglądu heurystycznego i automatycznych testów dostępności z 19 lipca 2026 r. Nie są przypisywane uczestnikom historycznych badań.

## Uczestnicy i zgody

W aktualnym repozytorium nie zachowano listy uczestników ani formularzy zgód. Właściciel potwierdza udział uczestników, ale nie podał ich liczby, nazw, pseudonimów ani danych sesji. Tych danych nie wolno uzupełniać przez domysł.

W przypadku ponownego badania bieżącej wersji należy użyć pseudonimów, zapisać podstawę udziału oraz przechowywać zgodę poza publicznym repozytorium.

## Runda przed III odbiorem

| Pole | Zapis retrospektywny |
| --- | --- |
| wynik historycznych testów uczestników | PASS - zbiorcze poświadczenie właściciela |
| przypisanie konkretnych sesji do III odbioru | brak zachowanego rejestru, nie można wiarygodnie odtworzyć |
| data, liczba uczestników i wyniki scenariuszy | brak danych szczegółowych |
| skutek formalny | wymaganie zostało przyjęte w ramach końcowej decyzji prowadzącego na początku lutego 2022 r. |

## Runda przed IV odbiorem

| Pole | Zapis retrospektywny |
| --- | --- |
| wynik historycznych testów uczestników | PASS - zbiorcze poświadczenie właściciela |
| przypisanie konkretnych sesji do IV odbioru | brak zachowanego rejestru, nie można wiarygodnie odtworzyć |
| data, liczba uczestników i wyniki scenariuszy | brak danych szczegółowych |
| skutek formalny | pozytywny wynik testów i końcowy produkt zostały zaakceptowane przez prowadzącego na początku lutego 2022 r. |

## Wyniki scenariuszy

Nie zachowano wyników na poziomie U-01-U-08 ani czasów wykonania. Zbiorczy wynik `PASS` nie jest przepisywany do poszczególnych scenariuszy, ponieważ tworzyłoby to dane, których właściciel nie podał.

Przy ponownym badaniu dozwolone wartości to `PASS`, `PASS Z UWAGĄ` i `FAIL`, a każdy wpis musi wskazywać kartę sesji lub notatkę obserwatora.

## Rejestr problemów

Nie zachowano rejestru problemów zgłoszonych przez historycznych uczestników. Nie oznacza to, że problemów nie było. Oznacza wyłącznie, że nie można ich obecnie wymienić ani przypisać do konkretnej osoby lub rundy.

## Poprawki wynikające z badania

Brak zachowanego łańcucha od opinii uczestnika do zmiany w kodzie. Z tego powodu żadna poprawka nie jest retrospektywnie przedstawiana jako wynik badania uczestników.

Poprawki H-01-H-03 są udokumentowane osobno jako rezultat przeglądu heurystycznego i testów automatycznych z 2026 r.

## Retest poprawek

Nie zachowano kart retestów poprawek wynikających z historycznych badań uczestników. Późniejszy wynik `2/2` dotyczy wyłącznie technicznych regresji dostępności H-01-H-03 i nie jest przedstawiany jako retest z udziałem człowieka.

## Decyzje po rundach

| Zakres | Decyzja | Data | Podstawa | Ograniczenie |
| --- | --- | --- | --- | --- |
| historyczne testy użyteczności z uczestnikami | PASS | przed końcowym odbiorem; daty sesji nie zostały zachowane | poświadczenie właściciela zapisane 19 lipca 2026 r. | brak kart sesji i szczegółowych wyników |
| końcowy odbiór projektu | ZAAKCEPTOWANO | początek lutego 2022 r.; dokładny dzień nie został zachowany | poświadczenie właściciela o decyzji prowadzącego | brak podpisu lub trwałego odnośnika w repozytorium |
| przegląd heurystyczny bieżącej wersji | PASS po poprawkach | 19 lipca 2026 r. | H-01-H-03 i `AccessibilityRegressionTests` | nie jest badaniem uczestników |

Historyczny wymóg Issue #43 można zamknąć na podstawie poświadczonego wyniku zbiorczego i końcowej decyzji prowadzącego, z jawnym ograniczeniem archiwalnym. Powtórne badanie bieżącej wersji może dostarczyć dokładniejsze dane, ale jest osobnym działaniem jakościowym i nie zmienia historycznej decyzji.
