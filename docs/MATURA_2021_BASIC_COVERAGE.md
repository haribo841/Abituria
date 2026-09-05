# Matura maj 2021 - poziom podstawowy, Formuła 2015

## Źródła

- [Arkusz EMAP-P0-100-2105](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2021/Matematyka/poziom_podstawowy/EMAP-P0-100-2105.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: 80AADA7793977EB615AE983AE2BD4762859EDB556A5115FE6B88607671B8D17C
  - weryfikacja: 2026-08-31
- [Zasady oceniania EMAP-P0-100-2105](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2021/Zasady_Oceniania/EMAP-P0-100-2105-zasady.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: 628D0C692EC414BB6C54251E97A6349D02A3D0E27B4A11EFA2866145DD9A1504
  - weryfikacja: 2026-08-31

Arkusz główny z 5 maja 2021 r. ma kod EMAP-P0-100-2105, czas 170 minut, 35 numerowanych zadań i 45 punktów. Należy do Formuły 2015. Aplikacja przechowuje transkrypcję strukturalną zadań, ich punktacji, odpowiedzi i rozwiązań, a nie źródłowy PDF ani raster.

## Macierz pokrycia

| Zadanie | Strona arkusza | Strona zasad | Punkty | Temat | Tryb |
| --- | ---: | ---: | ---: | --- | --- |
| 1 | 2 | 2 | 1 | potęgi | wybór |
| 2 | 2 | 2 | 1 | procenty | wybór |
| 3 | 2 | 3 | 1 | nierówności i przedziały | wybór |
| 4 | 2 | 3 | 1 | logarytmy | wybór |
| 5 | 2 | 4 | 1 | liczby rzeczywiste | wybór |
| 6 | 2 | 4 | 1 | nierówności | wybór |
| 7 | 4 | 4 | 1 | funkcja liniowa | wybór |
| 8 | 4 | 5 | 1 | układy równań | wybór |
| 9 | 6 | 5 | 1 | funkcja liniowa | wybór |
| 10 | 6 | 5 | 1 | wyrażenia algebraiczne | wybór |
| 11 | 6 | 6 | 1 | funkcje wykładnicze | wybór |
| 12 | 6 | 6 | 1 | funkcja kwadratowa | wybór |
| 13 | 6 | 6 | 1 | ciągi | wybór |
| 14 | 6 | 7 | 1 | ciągi | wybór |
| 15 | 8 | 7 | 1 | ciągi | wybór |
| 16 | 8 | 7 | 1 | trygonometria | wybór |
| 17 | 8 | 8 | 1 | planimetria | wybór |
| 18 | 8 | 8 | 1 | trygonometria | wybór |
| 19 | 10 | 8 | 1 | planimetria | wybór |
| 20 | 10 | 9 | 1 | planimetria | wybór |
| 21 | 10 | 9 | 1 | planimetria | wybór |
| 22 | 12 | 9 | 1 | planimetria | wybór |
| 23 | 12 | 10 | 1 | równania | wybór |
| 24 | 12 | 10 | 1 | planimetria | wybór |
| 25 | 14 | 10 | 1 | proste i odcinki | wybór |
| 26 | 14 | 11 | 1 | prawdopodobieństwo | wybór |
| 27 | 14 | 11 | 1 | kombinatoryka | wybór |
| 28 | 14 | 11 | 1 | statystyka | wybór |
| 29 | 16 | 12 | 2 | nierówności | ujawnienie |
| 30 | 17 | 14 | 2 | zadania dowodowe | ujawnienie |
| 31 | 18 | 18 | 2 | funkcja liniowa | ujawnienie |
| 32 | 19 | 19 | 2 | równania | ujawnienie |
| 33 | 20 | 20 | 2 | planimetria | liczbowy |
| 34 | 21 | 23 | 2 | prawdopodobieństwo | liczbowy |
| 35 | 22 | 26 | 5 | proste i odcinki | ujawnienie |

Macierz wymaga dokładnie 35/35/45: 35 oficjalnych zadań, 35 jednostek postępu i 45 punktów.

## Diagramy wektorowe

Własne definicje Avalonia zastępują figury źródłowe bez paczkowania rastrów:

- exam-em21-p0-z07 - wykres funkcji f;
- exam-em21-p0-z08 - geometryczna interpretacja układu równań;
- exam-em21-p0-z17 - okrąg ze styczną;
- exam-em21-p0-z18 - trójkąt prostokątny;
- exam-em21-p0-z20 - trójkąt z wysokością;
- exam-em21-p0-z21 - okrąg z kątami;
- exam-em21-p0-z22 - równoległobok z kątami;
- exam-em21-p0-z24 - pary stycznych kół.

Każdy diagram ma niepusty opis alternatywny, stronę źródłową i sourceId: cke-2021-main-basic. Są to deterministyczne implementacje wektorowe bez aktywnego PNG lub JPG.

## Prawa i testy

Content/provenance.json ma grupę `cke-2021-main-basic-exam` ze statusem `approved`. Rozszerzenie deklaracji właściciela z 1 września 2026 r. obejmuje ten arkusz, zasady oceniania i jego powiązane diagramy wyłącznie jako integralną część Abiturii. Wspólny katalog `runtime-vector-diagrams` jest obecnie `blocked`, ponieważ zawiera także diagramy z nieobjętych deklaracją arkuszy 2017 i 2016, więc manifest ma `releaseEligible=false`.

Test Matura2021BasicContentTests kontroluje kontrakt 35/35/45, źródła, sumy SHA-256, strony, klucz odpowiedzi, tryby odpowiedzi, diagramy, proweniencję i link w spisie dokumentacji. Testy katalogu, UI i smoke testu kontrolują 46 aktywnych arkuszy oraz 1 281 jednostek postępu.
