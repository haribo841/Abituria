# Matura maj 2017 PP - Formuła 2015

## Źródła przypięte

- [Arkusz CKE MMA-P1_1P-172](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2017/formula_od_2015/matematyka/MMA-P1_1P-172.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `AA9506313B735854BBFEC309F8C0C6496AD168E332AF63B9A58CDA7822F0BBAF`
  - weryfikacja: 2026-09-01
- [Zasady oceniania CKE MMA-P1-N](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2017/formula_od_2015/zasady_oceniania/MMA-P1-N.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `76866211FF1979E816A5F2091DB29ED0123D6921A51AE2BD2C928808601CD693`
  - weryfikacja: 2026-09-01

Arkusz z 5 maja 2017 r. ma 34 oficjalne zadania, 34 jednostki postępu, 50 punktów i czas pracy 170 minut. Dane aplikacji mają identyfikator `matura-maj-2017-podstawowa`; trwałe identyfikatory postępu to `mm17-p0-z01` do `mm17-p0-z34`.

## Macierz pokrycia

| Zadania | Strony arkusza | Strony zasad | Punkty | Tematy | Tryb |
| --- | --- | --- | --- | --- | --- |
| 1-6 | 2-4 | 2 | po 1 | potęgi, logarytmy, procenty, równania, nierówności | wybór |
| 7-13 | 4-6 | 3 | po 1 | nierówności, równania, funkcja liniowa, funkcja kwadratowa, ciągi | wybór |
| 14-20 | 8-12 | 4 | po 1 | trygonometria, planimetria, proste i odcinki | wybór |
| 21-25 | 12-14 | 5 | po 1 | stereometria, statystyka, prawdopodobieństwo | wybór |
| 26 | 16 | 6 | 2 | nierówności | ujawnienie rozwiązania |
| 27-28 | 17-18 | 8 | po 2 | zadania dowodowe | ujawnienie rozwiązania |
| 29 | 19 | 13 | 4 | funkcja kwadratowa | liczbowy, `-16/9` |
| 30-31 | 20-21 | 15-16 | po 2 | planimetria, ciągi | liczbowe, `60`, `9` |
| 32 | 22 | 17 | 5 | proste i odcinki | liczbowy, `243/7` |
| 33 | 23 | 22 | 2 | prawdopodobieństwo | liczbowy, `1/9` |
| 34 | 24 | 23 | 4 | stereometria | ujawnienie rozwiązania |

Siedem definicji wektorowych Avalonia `exam-mm17-p0-z07`, `exam-mm17-p0-z10`, `exam-mm17-p0-z11`, `exam-mm17-p0-z15`, `exam-mm17-p0-z16`, `exam-mm17-p0-z17` i `exam-mm17-p0-z22` odtwarza informacje graficzne bez aktywnych rastrów. Każda ma opis alternatywny, stabilny identyfikator i numer strony arkusza.

## Testy i proweniencja

`Matura2017ContentTests` kontroluje źródła, sumy SHA-256, kontrakt `34/34/50`, strony, punktację, tryby odpowiedzi, wyniki liczbowe i siedem diagramów. Testy katalogu, UI i smoke testu kontrolują 32 aktywne arkusze, 889 jednostek postępu i 226 diagramów.

Grupa `cke-2017-main-basic-exam` ma status `blocked`. Katalog `runtime-vector-diagrams` też ma tymczasowo status `blocked`, ponieważ zawiera siedem nowych figur pochodnych z tego arkusza. Manifest ma `releaseEligible=false` do czasu odrębnego rozszerzenia deklaracji właściciela obejmującego arkusz, zasady oceniania i pochodne diagramy 2017.
