# Katalog matur matematycznych Formuły 2015 i Formuły 2023

## Stan katalogu

Katalog obejmuje wszystkie możliwe do zweryfikowania arkusze matematyki od wprowadzenia Formuły 2015 w 2015 r. do aktualnej sesji 2026 r. Aktywne są 46 arkusze i 1 281 osobno śledzonych jednostek postępu:

- 35 arkuszy Formuły 2015 - główne PP i PR z lat 2015-2026 oraz poprawkowe PP z lat 2015-2025;
- 11 arkuszy Formuły 2023 - główne PP i PR z lat 2023-2026 oraz poprawkowe PP z lat 2023-2025.

Arkusze poprawkowe obejmują wyłącznie poziom podstawowy. W chwili weryfikacji 4 września 2026 r. CKE nie opublikowała zasad oceniania dla obu poprawek z sierpnia 2026 r. Te dwa arkusze nie są aktywne, ponieważ nie zawieramy zgadywanych odpowiedzi, rozwiązań ani kryteriów punktowania.

| Rok | F2023 główna PP | F2023 główna PR | F2023 poprawkowa PP | F2015 główna PP | F2015 główna PR | F2015 poprawkowa PP |
| --- | --- | --- | --- | --- | --- | --- |
| 2026 | aktywna | aktywna | oczekuje na zasady | aktywna | aktywna | oczekuje na zasady |
| 2025 | aktywna | aktywna | aktywna | aktywna | aktywna | aktywna |
| 2024 | aktywna | aktywna | aktywna | aktywna | aktywna | aktywna |
| 2023 | aktywna | aktywna | aktywna | aktywna | aktywna | aktywna |
| 2022 | nie dotyczy | nie dotyczy | nie dotyczy | aktywna | aktywna | aktywna |
| 2021 | nie dotyczy | nie dotyczy | nie dotyczy | aktywna | aktywna | aktywna |
| 2020 | nie dotyczy | nie dotyczy | nie dotyczy | aktywna | aktywna | aktywna |
| 2019 | nie dotyczy | nie dotyczy | nie dotyczy | aktywna | aktywna | aktywna |
| 2018 | nie dotyczy | nie dotyczy | nie dotyczy | aktywna | aktywna | aktywna |
| 2017 | nie dotyczy | nie dotyczy | nie dotyczy | aktywna | aktywna | aktywna |
| 2016 | nie dotyczy | nie dotyczy | nie dotyczy | aktywna | aktywna | aktywna |
| 2015 | nie dotyczy | nie dotyczy | nie dotyczy | aktywna | aktywna | aktywna |

Kolejność w `Content/exams.json` jest malejąca według roku. Dla lat z równoległymi formułami najpierw występuje Formuła 2023, następnie Formuła 2015, a wewnątrz każdej formuły PP, PR i poprawka PP. Identyfikatory dotychczasowych arkuszy i postępu SQLite pozostają niezmienione.

## Źródła i kontrakt danych

Każdy aktywny arkusz ma osobny plik `Content/exam-*.json`, wpis indeksu, datę egzaminu, czas, punktację, numerowane jednostki postępu, stronę arkusza, stronę zasad oceniania, rozwiązanie, kryteria punktowania, dwie podpowiedzi i przypięte URL-e oraz SHA-256. Zadania są przypisane do istniejących 17 tematów, dlatego strona „Zadania” agreguje całą oś czasu bez tworzenia odrębnych kategorii historycznych.

Szczegółową macierz nowych źródeł 2015 oraz równoległych arkuszy F2015 z 2023-2026 zawiera [pokrycie pełnego archiwum Formuły 2015](MATURA_FORMULA_2015_ARCHIVE_COVERAGE.md). Pozostałe macierze są odnośnikami w spisie dokumentacji.

## Prawa do dystrybucji

Pełność katalogu technicznego nie jest zgodą na publiczną redystrybucję. Czternaście nowych grup źródłowych z 2015 oraz F2015 z 2023-2026 ma w `Content/provenance.json` status `blocked`, podobnie jak sześć wcześniej niepotwierdzonych grup z 2016-2017 i katalog diagramów zawierający ich figury pochodne. Dlatego `releaseEligible` pozostaje `false`.

Wpis można zmienić na `approved` wyłącznie po indywidualnym, pisemnym rozszerzeniu deklaracji właściciela na konkretny arkusz i zasady oceniania. Publiczny URL, kompletna transkrypcja, suma SHA-256 ani zielone testy nie są taką zgodą.

## Regresje i bramy jakości

`Formula2015ArchiveContentTests` kontroluje 14 nowych arkuszy, ich źródła, skróty SHA-256, daty, liczbę zadań, punktację, identyfikatory, rozwiązania, kryteria i podpowiedzi. `ContentInventoryTests`, testy interfejsu, test wydajności i smoke test wydania sprawdzają katalog 46 arkuszy oraz 1 281 jednostek postępu, a testy proweniencji wymagają dokładnie 21 grup `blocked`.

Przed autoryzowanym commitem obowiązują locked restore, Release build bez ostrzeżeń, testy z pokryciem, formatowanie, audyt zależności, walidacja proweniencji, DocFX, kontrola linków i `git diff --check`. SonarCloud, CodeQL, Build i Pages są weryfikowane wyłącznie po autoryzowanym pushu.
