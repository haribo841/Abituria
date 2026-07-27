<!--
Dziękujemy za zmianę. Usuń komentarze pomocnicze, uzupełnij wszystkie sekcje
i wpisz "Nie dotyczy - <uzasadnienie>" tam, gdzie dany obszar nie ma zastosowania.
Nie oznaczaj kontroli jako wykonanej, jeśli jej nie uruchomiono.
-->

## Cel i powiązanie

<!-- Jaki problem rozwiązuje zmiana i dlaczego jest potrzebna? -->

Powiązane issue: <!-- np. Closes #123 -->

## Zakres

<!-- Opisz najważniejsze zmiany oraz elementy świadomie pozostawione poza zakresem. -->

## Rodzaj zmiany

- [ ] poprawka błędu lub regresji
- [ ] nowa funkcja
- [ ] refaktoryzacja lub wydajność bez zmiany kontraktu użytkowego
- [ ] testy
- [ ] dokumentacja lub warstwa organizacyjna
- [ ] treść edukacyjna albo zasób
- [ ] zależności, CI, pakowanie lub wydanie
- [ ] zmiana niekompatybilna wstecz

## Weryfikacja

### Polecenia i wyniki

<!-- Wymień rzeczywiście wykonane polecenia, liczbę testów i wynik każdej bramy. -->

### Testy i pokrycie

<!-- Jakie testy C# lub Python dodano albo zmieniono? Podaj łączne pokrycie i pokrycie gałęzi z Test-CoverageThreshold.ps1. -->

### SonarQube Cloud

<!-- Podaj wynik quality gate i liczbę nowych issues. Dla skanu pominiętego na forku opisz lokalne analizatory i dalszą weryfikację przed scaleniem. -->

### CodeQL

<!-- Podaj wynik code scanning i liczbę nowych alertów. Każde wyciszenie wymaga konkretnego uzasadnienia zaakceptowanego w przeglądzie. -->

## Wpływ zmiany

W każdym wierszu podaj wpływ i dowód albo `Nie dotyczy - <uzasadnienie>`.

| Obszar | Wpływ i dowód |
| --- | --- |
| UI, motywy i dostępność | |
| Dane użytkownika, SQLite i zgodność wsteczna | |
| Treści, zasoby, licencje i proweniencja | |
| Zależności i bezpieczeństwo łańcucha dostaw | |
| Dokumentacja, DocFX i odnośniki | |
| Pakowanie, instalacja i publiczne wydanie | |

## Materiały dla recenzenta

<!-- Dodaj zrzuty ekranu, logi bez sekretów, próbki danych lub wskazówki do ręcznej weryfikacji. -->

## Checklista autora

- [ ] Zmiana ma jeden spójny cel, a powiązane issue i zakres są opisane.
- [ ] Nowe lub zmienione zachowanie ma test regresyjny, jednostkowy albo integracyjny, lub wyjaśniłem, dlaczego nie dotyczy.
- [ ] Pełny build Release, testy C# i Python, raporty OpenCover i Cobertura oraz bramka pokrycia `90%`/`85%` zakończyły się powodzeniem.
- [ ] Sprawdziłem SonarQube Cloud: quality gate przechodzi i zmiana nie pozostawia nowych issues, albo jawnie opisałem brak skanu z forka.
- [ ] Sprawdziłem CodeQL: code scanning nie pozostawia nowych alertów albo jawnie opisałem, dlaczego skan nie był dostępny.
- [ ] Zaktualizowałem właściwą dokumentację i sprawdziłem DocFX oraz odnośniki, jeśli zmiana ich dotyczy.
- [ ] Zweryfikowałem `Content/provenance.json` i prawo do dystrybucji każdego zmienionego zasobu, jeśli zmiana ich dotyczy.
- [ ] Nie dodałem sekretów, prywatnych danych, baz użytkowników ani przypadkowych artefaktów generowanych.
- [ ] Istotna zmiana wizualna ma aktualne zrzuty ekranu bez danych prywatnych.
