# Matura maj 2022 - poziom rozszerzony, Formuła 2015

## Źródła

- [Arkusz EMAP-R0-100-2205](https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2022/Matematyka/EMAP-R0-100-2205_compressed.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna, archiwum OKE Warszawa
  - SHA-256: `2AA11EADAE59BE3F60A61B97FD27DE782849F9631D0991D77B43C96D88B676A4`
  - weryfikacja: 2026-08-31
- [Zasady oceniania EMAP-R0-100-2205](https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2022/Matematyka/EMAP-R0-100-2205-zasady.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna, archiwum OKE Warszawa
  - SHA-256: `83D33D8C83F6E866851406950CCA7A0C1A336DC5E5F28AB1ED947F0321478D06`
  - weryfikacja: 2026-08-31

Arkusz główny z 11 maja 2022 r. ma kod `EMAP-R0-100-2205`, czas 180 minut, 15 numerowanych zadań i 50 punktów. Należy do Formuły 2015. Aplikacja przechowuje opisową adaptację zadań, punktacji, odpowiedzi i rozwiązań, a nie plik PDF ani raster źródłowy.

## Macierz pokrycia

| Zadanie | Strona arkusza | Strona zasad | Punkty | Temat | Tryb |
| --- | ---: | ---: | ---: | --- | --- |
| 1 | 2 | 2 | 1 | logarytmy | wybór |
| 2 | 2 | 2 | 1 | funkcja kwadratowa | wybór |
| 3 | 2 | 3 | 1 | trygonometria | wybór |
| 4 | 2 | 3 | 1 | prawdopodobieństwo | wybór |
| 5 | 4 | 4 | 2 | ciągi | liczbowy |
| 6 | 5 | 4 | 3 | zadania dowodowe | ujawnienie |
| 7 | 6 | 7 | 3 | równania | liczbowy |
| 8 | 8 | 10 | 3 | zadania dowodowe | ujawnienie |
| 9 | 10 | 15 | 4 | nierówności | ujawnienie |
| 10 | 12 | 17 | 4 | ciągi | liczbowy |
| 11 | 14 | 20 | 4 | trygonometria | ujawnienie |
| 12 | 16 | 24 | 5 | funkcja kwadratowa | ujawnienie |
| 13 | 18 | 28 | 5 | stereometria | liczbowy |
| 14 | 20 | 30 | 6 | proste i odcinki | ujawnienie |
| 15 | 22 | 40 | 7 | zadania dowodowe | ujawnienie |

Macierz wymaga dokładnie `15/15/50`: 15 oficjalnych zadań, 15 jednostek postępu i 50 punktów.

## Diagram wektorowy

Własna definicja Avalonia `exam-em22-r0-z13` odtwarza przestrzenny układ graniastosłupa, trójkąt `AFH`, wysokość oraz oznaczenie kąta alfa z zadania 13. Ma `sourceId: cke-2022-main-extended`, stronę 18 i niepusty opis alternatywny. Jest deterministycznym opisem wektorowym bez aktywnego PNG lub JPG.

## Prawa i testy

Content/provenance.json ma grupę `cke-2022-main-extended-exam` ze statusem `approved`. Rozszerzenie deklaracji właściciela z 1 września 2026 r. obejmuje ten arkusz, zasady oceniania i jego powiązane diagramy wyłącznie jako integralną część Abiturii. Wspólny katalog `runtime-vector-diagrams` jest obecnie `blocked`, ponieważ zawiera także diagramy z nieobjętych deklaracją arkuszy 2017 i 2016, więc manifest ma `releaseEligible=false`.

Test `Matura2022ExtendedContentTests` kontroluje kontrakt `15/15/50`, źródła, sumy SHA-256, strony, odpowiedzi, tryby, diagram, proweniencję i link w spisie dokumentacji. Testy katalogu, UI i smoke testu kontrolują 46 aktywnych arkuszy oraz 1 281 jednostek postępu.
