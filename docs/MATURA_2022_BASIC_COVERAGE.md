# Matura maj 2022 - poziom podstawowy, Formuła 2015

## Źródła

- [Arkusz EMAP-P0-100-2205](https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2022/Matematyka/EMAP-P0-100-2205_compressed.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna, archiwum OKE Warszawa
  - SHA-256: 34101686FFB987572AB3110F344EC1A8692D8FACDBEF194498DBBD245568A2D2
  - weryfikacja: 2026-08-31
- [Zasady oceniania EMAP-P0-100-2205](https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2022/Matematyka/EMAP-P0-100-2205-zasady.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna, archiwum OKE Warszawa
  - SHA-256: DD236C15820CBD5F7BD15D2EA194D075A3732B640C559A064062FBF3101F90B8
  - weryfikacja: 2026-08-31

Arkusz główny z 5 maja 2022 r. ma kod EMAP-P0-100-2205, czas 170 minut, 35 numerowanych zadań i 45 punktów. Należy do Formuły 2015. Aplikacja przechowuje opisową adaptację zadań, ich punktacji, odpowiedzi i rozwiązań, a nie plik PDF ani raster źródłowy.

## Macierz pokrycia

| Zadanie | Strona arkusza | Strona zasad | Punkty | Temat | Tryb |
| --- | ---: | ---: | ---: | --- | --- |
| 1 | 2 | 2 | 1 | potęgi i pierwiastki | wybór |
| 2 | 2 | 2 | 1 | wyrażenia algebraiczne | wybór |
| 3 | 2 | 3 | 1 | logarytmy | wybór |
| 4 | 2 | 3 | 1 | procenty | wybór |
| 5 | 2 | 4 | 1 | potęgi | wybór |
| 6 | 4 | 4 | 1 | równania | wybór |
| 7 | 4 | 4 | 1 | nierówności | wybór |
| 8 | 4 | 5 | 1 | równania | wybór |
| 9 | 4 | 5 | 1 | funkcja liniowa | wybór |
| 10 | 6 | 5 | 1 | funkcja liniowa | wybór |
| 11 | 8 | 6 | 1 | funkcja liniowa | wybór |
| 12 | 8 | 6 | 1 | funkcja kwadratowa | wybór |
| 13 | 8 | 7 | 1 | ciągi | wybór |
| 14 | 8 | 7 | 1 | ciągi | wybór |
| 15 | 8 | 7 | 1 | ciągi | wybór |
| 16 | 8 | 8 | 1 | trygonometria | wybór |
| 17 | 10 | 8 | 1 | planimetria | wybór |
| 18 | 10 | 9 | 1 | planimetria | wybór |
| 19 | 12 | 9 | 1 | planimetria | wybór |
| 20 | 12 | 9 | 1 | planimetria | wybór |
| 21 | 12 | 10 | 1 | proste i odcinki | wybór |
| 22 | 12 | 10 | 1 | proste i odcinki | wybór |
| 23 | 12 | 10 | 1 | proste i odcinki | wybór |
| 24 | 14 | 11 | 1 | proste i odcinki | wybór |
| 25 | 14 | 11 | 1 | stereometria | wybór |
| 26 | 14 | 11 | 1 | stereometria | wybór |
| 27 | 14 | 12 | 1 | kombinatoryka | wybór |
| 28 | 14 | 12 | 1 | statystyka | wybór |
| 29 | 16 | 13 | 2 | nierówności | ujawnienie |
| 30 | 17 | 15 | 2 | ciągi | liczbowy |
| 31 | 18 | 16 | 2 | zadania dowodowe | ujawnienie |
| 32 | 19 | 19 | 2 | trygonometria | liczbowy |
| 33 | 20 | 21 | 2 | planimetria | ujawnienie |
| 34 | 21 | 23 | 2 | prawdopodobieństwo | ujawnienie |
| 35 | 22 | 25 | 5 | funkcja kwadratowa | ujawnienie |

Macierz wymaga dokładnie 35/35/45: 35 oficjalnych zadań, 35 jednostek postępu i 45 punktów.

## Diagramy wektorowe

Własne definicje Avalonia zastępują figury źródłowe bez paczkowania rastrów:

- exam-em22-p0-z09 - wykres funkcji odcinkowej;
- exam-em22-p0-z10 - dwa wykresy funkcji przesuniętych względem siebie;
- exam-em22-p0-z17 - okrąg z cięciwą, średnicą i kątami alfa oraz gamma;
- exam-em22-p0-z18 - romb wpisany w okrąg;
- exam-em22-p0-z26 - sześcian z ostrosłupem EFGB;
- exam-em22-p0-z33 - trójkąt równoramienny z dwusieczną.

Każdy diagram ma niepusty opis alternatywny, stronę źródłową i sourceId: cke-2022-main-basic. Są to deterministyczne implementacje wektorowe bez aktywnego PNG lub JPG.

## Prawa i testy

Content/provenance.json ma grupę `cke-2022-main-basic-exam` ze statusem `approved`. Rozszerzenie deklaracji właściciela z 1 września 2026 r. obejmuje ten arkusz, zasady oceniania i jego powiązane diagramy wyłącznie jako integralną część Abiturii. Wspólny katalog `runtime-vector-diagrams` jest obecnie `blocked`, ponieważ zawiera także diagramy z nieobjętych deklaracją arkuszy 2017 i 2016, więc manifest ma `releaseEligible=false`.

Test Matura2022BasicContentTests kontroluje kontrakt 35/35/45, źródła, sumy SHA-256, strony, klucz odpowiedzi, tryby odpowiedzi, diagramy, proweniencję i link w spisie dokumentacji. Testy katalogu, UI i smoke testu kontrolują 46 aktywnych arkuszy oraz 1 281 jednostek postępu.
