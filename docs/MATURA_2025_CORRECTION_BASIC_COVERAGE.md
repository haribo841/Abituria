# Matura poprawkowa 2025 - poziom podstawowy

## Źródła

- [Arkusz MMAP-P0-100-2508](https://arkusze.pl/maturalne/matematyka-2025-sierpien-poprawkowa-podstawowa.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna, publiczne archiwum arkusze.pl
  - SHA-256: `1A25E6B62185D623CA68E120157506E0C503DB285725B1CB31A34D40D522F873`
  - weryfikacja: 2026-08-31
- [Zasady oceniania MMAP-P0-100-2508](https://arkusze.pl/maturalne/matematyka-2025-sierpien-poprawkowa-podstawowa-odpowiedzi.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna, publiczne archiwum arkusze.pl
  - SHA-256: `058D68212521D98B4D63E320921647988BC35108941422FC39BF836813D63037`
  - weryfikacja: 2026-08-31

Arkusz z 19 sierpnia 2025 r. ma kod `MMAP-P0-100-2508`, czas 180 minut i 50 punktów. Aplikacja zachowuje 31 oficjalnych zadań jako 36 osobno ocenianych jednostek postępu. Źródła z publicznego archiwum są przypięte w proweniencji. Rozszerzenie deklaracji właściciela z 1 września 2026 r. daje grupie status `approved` wyłącznie jako integralnej części Abiturii.

## Macierz pokrycia

| Jednostka | Strona arkusza | Strona zasad | Punkty | Temat | Tryb |
| --- | ---: | ---: | ---: | --- | --- |
| 1 | 4 | 2 | 1 | nierówności | wybór |
| 2 | 4 | 3 | 1 | potęgi | wybór |
| 3 | 5 | 3 | 1 | potęgi | wybór |
| 4 | 5 | 4 | 1 | logarytmy | wybór |
| 5 | 6 | 4 | 2 | zadania dowodowe | ujawnienie |
| 6 | 7 | 5 | 1 | wyrażenia algebraiczne | wybór |
| 7 | 7 | 5 | 1 | nierówności | wybór |
| 8 | 8 | 6 | 3 | równania | ujawnienie |
| 9 | 9 | 8 | 2 | nierówności | ujawnienie |
| 10 | 10 | 9 | 1 | równania | wybór |
| 11 | 11 | 10 | 2 | układy równań | ujawnienie |
| 12.1 | 12 | 11 | 2 | funkcja liniowa | złożony |
| 12.2 | 13 | 12 | 2 | funkcja liniowa | złożony |
| 13 | 13 | 12 | 1 | funkcja liniowa | wybór |
| 14.1 | 14 | 13 | 1 | funkcja kwadratowa | wybór |
| 14.2 | 15 | 13 | 1 | funkcja kwadratowa | złożony |
| 14.3 | 15 | 14 | 1 | funkcja kwadratowa | wybór |
| 15 | 16 | 14 | 1 | ciągi | wybór |
| 16 | 16 | 15 | 1 | ciągi | wybór |
| 17 | 17 | 15 | 1 | ciągi | wybór |
| 18 | 17 | 16 | 1 | ciągi | wybór |
| 19 | 18 | 16 | 1 | trygonometria | wybór |
| 20 | 18 | 17 | 1 | trygonometria | wybór |
| 21.1 | 19 | 17 | 1 | planimetria | wybór |
| 21.2 | 19 | 18 | 1 | planimetria | wybór |
| 22.1 | 20 | 18 | 1 | planimetria | wybór |
| 22.2 | 20 | 19 | 1 | planimetria | wybór |
| 23 | 21 | 19 | 1 | planimetria | złożony |
| 24 | 22 | 20 | 1 | proste i odcinki | liczbowy |
| 25 | 23 | 20 | 1 | proste i odcinki | złożony |
| 26 | 24 | 21 | 4 | stereometria | ujawnienie |
| 27 | 26 | 23 | 1 | stereometria | wybór |
| 28 | 26 | 23 | 1 | kombinatoryka | wybór |
| 29 | 27 | 24 | 2 | prawdopodobieństwo | ujawnienie |
| 30 | 28 | 25 | 3 | statystyka | złożony |
| 31 | 29 | 26 | 2 | funkcja kwadratowa | ujawnienie |

Macierz wymaga dokładnie `31/36/50`: 31 oficjalnych zadań, 36 jednostek postępu i 50 punktów.

## Diagramy wektorowe

Własne definicje Avalonia zastępują figury źródłowe bez paczkowania rastrów:

- `exam-mm25-p0p-z12` - wykres funkcji odcinkowej z zadania 12;
- `exam-mm25-p0p-z21` - okrąg ze średnicą BD i punktami A, B, C, D, S;
- `exam-mm25-p0p-z22` - trójkąt ABC z długościami 6, 4 i kątem 60 stopni;
- `exam-mm25-p0p-z23` - trapez ABCD z przekątnymi i punktem E;
- `exam-mm25-p0p-z26` - graniastosłup prawidłowy trójkątny;
- `exam-mm25-p0p-z30` - wykres słupkowy liczby usterek.

Każdy diagram ma niepusty opis alternatywny, stronę źródłową i `sourceId: cke-2025-correction-basic`. Są to deterministyczne implementacje wektorowe, bez aktywnego pliku PNG lub JPG.

## Prawa i weryfikacja

`Content/provenance.json` ma osobną grupę `cke-2025-correction-basic-exam` oraz katalog `runtime-vector-diagrams` ze statusem `approved`. Rozszerzenie deklaracji właściciela z 1 września 2026 r. obejmuje wszystkie 195 definicji, w tym sześć definicji dla `MMAP-P0-100-2508`. Manifest ma `releaseEligible=true`.

Test `Matura2025CorrectionBasicContentTests` kontroluje liczniki, kolejność, strony, punkty, identyfikatory, URL-e, sumy SHA-256, metody odpowiedzi, diagramy, proweniencję i link w spisie dokumentacji. Testy katalogu i UI dodatkowo kontrolują 26 aktywnych arkuszy oraz 722 jednostki postępu.
