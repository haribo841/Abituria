# Matura maj 2018 PR - Formuła 2015

## Źródła przypięte

- [Arkusz CKE MMA-R1_1P-182](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2018/formula_od_2015/matematyka/MMA-R1_1P-182.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `7EFD8731C3DDD97F4CFACF7E4D450F7E8C818FDE5318C57F4CA320F1280E42DD`
  - weryfikacja: 2026-09-01
- [Zasady oceniania MMA-R1_1P-182](https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2018/Matematyka/MMA-R1_1P-182_zasady_oceniania.pdf)
  - wydawca: Okręgowa Komisja Egzaminacyjna w Warszawie
  - SHA-256: `A8874470FB79681CDB8D39A35D55016D21D7378BB501E7C815CFA21DB2C1CD7F`
  - weryfikacja: 2026-09-01

Arkusz z 9 maja 2018 r. ma 15 oficjalnych zadań, 15 jednostek postępu, 50 punktów i czas pracy 180 minut. Dane aplikacji mają identyfikator `matura-maj-2018-rozszerzona`; trwałe identyfikatory postępu to `mm18-r0-z01` do `mm18-r0-z15`.

## Macierz pokrycia

| Zadania | Strony arkusza | Strony zasad | Punkty | Tematy | Tryb |
| --- | --- | --- | --- | --- | --- |
| 1-4 | 2 | 2 | po 1 | potęgi, równania, logarytmy, funkcja kwadratowa | wybór |
| 5 | 4 | 2 | 2 | funkcja liniowa | liczbowy, `166` |
| 6 | 4 | 3 | 3 | funkcja kwadratowa | ujawnienie rozwiązania |
| 7 | 5 | 5 | 3 | zadania dowodowe | ujawnienie rozwiązania |
| 8 | 6 | 9 | 3 | zadania dowodowe | ujawnienie rozwiązania |
| 9 | 7 | 11 | 4 | prawdopodobieństwo | liczbowy, `5/14` |
| 10 | 8 | 16 | 4 | stereometria | liczbowy, `9/√106` |
| 11 | 9 | 18 | 4 | trygonometria | ujawnienie rozwiązania |
| 12 | 10 | 19 | 6 | funkcja kwadratowa | ujawnienie rozwiązania |
| 13 | 12 | 22 | 4 | ciągi | liczbowy, `15` |
| 14 | 14 | 24 | 6 | proste i odcinki | ujawnienie rozwiązania |
| 15 | 16 | 31 | 7 | planimetria | ujawnienie rozwiązania |

Wektorowe definicje `exam-mm18-r0-z07`, `exam-mm18-r0-z10`, `exam-mm18-r0-z14` i `exam-mm18-r0-z15` odtwarzają figury arkusza. Każda ma opis alternatywny, stabilny identyfikator i stronę źródłową. Nie dodano aktywnych rastrów.

## Testy i proweniencja

`Matura2018ContentTests` kontroluje źródła, sumy SHA-256, kontrakt `15/15/50`, punktację, tryby odpowiedzi, wyniki liczbowe i cztery diagramy. Testy katalogu, UI i smoke testu kontrolują 26 aktywnych arkuszy, 722 jednostki postępu i 195 diagramów.

Grupa `cke-2018-main-extended-exam` oraz katalog `runtime-vector-diagrams` mają status `blocked`. `releaseEligible=false` pozostaje prawdziwe do czasu osobistego rozszerzenia deklaracji praw do redystrybucji arkusza i zasad oceniania.
