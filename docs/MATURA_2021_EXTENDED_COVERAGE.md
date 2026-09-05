# Matura maj 2021 - poziom rozszerzony, Formuła 2015

## Źródła

- [Arkusz EMAP-R0-100-2105](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2021/Matematyka/poziom_rozszerzony/EMAP-R0-100-2105.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: EFAD08442C8C6A16317D783497630A6FBEF86744F5E3766FCBFA59B0E70C1C73
  - weryfikacja: 2026-08-31
- [Zasady oceniania EMAP-R0-100-2105](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2021/Zasady_Oceniania/EMAP-R0-100-2105-zasady.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: E034E018D5070D6AF2BCA5D033B595B3C3F0FAC0D40C012AC7DD069BFB60D2CA
  - weryfikacja: 2026-08-31

Arkusz główny z 11 maja 2021 r. ma kod EMAP-R0-100-2105, czas 180 minut, 15 numerowanych zadań i 50 punktów. Należy do Formuły 2015. Aplikacja przechowuje transkrypcję strukturalną zadań, ich punktacji, odpowiedzi i rozwiązań, a nie źródłowy PDF ani raster.

## Macierz pokrycia

| Zadanie | Strona arkusza | Strona zasad | Punkty | Temat | Tryb |
| --- | ---: | ---: | ---: | --- | --- |
| 1 | 2 | 2 | 1 | trygonometria | wybór |
| 2 | 2 | 2 | 1 | trygonometria | wybór |
| 3 | 2 | 3 | 1 | wyrażenia algebraiczne | wybór |
| 4 | 2 | 3 | 1 | równania | wybór |
| 5 | 4 | 4 | 2 | granice ciągów | liczbowy |
| 6 | 5 | 4 | 3 | logarytmy | ujawnienie |
| 7 | 6 | 6 | 3 | nierówności | ujawnienie |
| 8 | 8 | 9 | 3 | planimetria | ujawnienie |
| 9 | 10 | 21 | 4 | prawdopodobieństwo warunkowe | liczbowy |
| 10 | 12 | 23 | 4 | proste i odcinki | ujawnienie |
| 11 | 14 | 28 | 5 | funkcja kwadratowa | ujawnienie |
| 12 | 16 | 31 | 5 | trygonometria | ujawnienie |
| 13 | 18 | 37 | 4 | trygonometria | liczbowy |
| 14 | 20 | 46 | 6 | funkcja kwadratowa | ujawnienie |
| 15 | 24 | 52 | 7 | optymalizacja | ujawnienie |

Macierz wymaga dokładnie 15/15/50: 15 oficjalnych zadań, 15 jednostek postępu i 50 punktów.

## Diagramy wektorowe

Własne definicje Avalonia zastępują figury źródłowe bez paczkowania rastrów:

- exam-em21-r0-z02 - fragment wykresu funkcji;
- exam-em21-r0-z08 - trójkąt równoboczny z punktami D, E i P;
- exam-em21-r0-z14 - parabola z punktami A, B i C.

Każdy diagram ma niepusty opis alternatywny, stronę źródłową i sourceId: cke-2021-main-extended. Są to deterministyczne implementacje wektorowe bez aktywnego PNG lub JPG.

## Prawa i testy

Content/provenance.json ma grupę `cke-2021-main-extended-exam` ze statusem `approved`. Rozszerzenie deklaracji właściciela z 1 września 2026 r. obejmuje ten arkusz, zasady oceniania i jego powiązane diagramy wyłącznie jako integralną część Abiturii. Wspólny katalog `runtime-vector-diagrams` jest obecnie `blocked`, ponieważ zawiera także diagramy z nieobjętych deklaracją arkuszy 2017 i 2016, więc manifest ma `releaseEligible=false`.

Test Matura2021ExtendedContentTests kontroluje kontrakt 15/15/50, źródła, sumy SHA-256, strony, klucz odpowiedzi, tryby odpowiedzi, diagramy, proweniencję i link w spisie dokumentacji. Testy katalogu, UI i smoke testu kontrolują 46 aktywnych arkuszy oraz 1 281 jednostek postępu.
