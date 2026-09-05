# Matura poprawkowa 2017 PP - Formuła 2015

## Źródła przypięte

- [Arkusz MMA-P1_1P-174 w zachowanym archiwum publicznym](https://arkusze.pl/maturalne/matematyka-2017-sierpien-poprawkowa-podstawowa.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna, zachowany adres: arkusze.pl
  - SHA-256: `D3975FF6D80430737C1D7186143BFC79F42EFBA06E749128447E2D39E42BD4A9`
  - weryfikacja: 2026-09-01
- [Zasady oceniania MMA-P1_1P-174 w zachowanym archiwum publicznym](https://arkusze.pl/maturalne/matematyka-2017-sierpien-poprawkowa-podstawowa-odpowiedzi.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna, zachowany adres: arkusze.pl
  - SHA-256: `1295525A054ECDD6E1CF132AD4C13B54E29C79AAFC63C723508061A7C211266C`
  - weryfikacja: 2026-09-01

Arkusz poprawkowy z 22 sierpnia 2017 r. ma 34 oficjalne zadania, 34 jednostki postępu, 50 punktów i czas pracy 170 minut. Dane aplikacji mają identyfikator `matura-poprawkowa-2017-podstawowa`; trwałe identyfikatory postępu to `mm17-p0p-z01` do `mm17-p0p-z34`.

## Macierz pokrycia

| Zadania | Strony arkusza | Strony zasad | Punkty | Tematy | Tryb |
| --- | --- | --- | --- | --- | --- |
| 1-5 | 2 | 2 | po 1 | procenty, potęgi, logarytmy | wybór |
| 6-9 | 4 | 2-3 | po 1 | nierówności, równania, procenty | wybór |
| 10-13 | 6 | 3 | po 1 | funkcja kwadratowa, ciągi, trygonometria | wybór |
| 14-16 | 8 | 4 | po 1 | planimetria | wybór |
| 17-20 | 10 | 4-5 | po 1 | proste i odcinki, stereometria | wybór |
| 21-25 | 12 | 5 | po 1 | proste i odcinki, stereometria, statystyka, kombinatoryka, prawdopodobieństwo | wybór |
| 26-29 | 14-17 | 2-7 | po 2 | nierówności, równania, zadania dowodowe | ujawnienie rozwiązania |
| 30-32 | 18-20 | 10-13 | 2, 2, 4 | prawdopodobieństwo, ciągi, funkcja kwadratowa | liczbowe, `12/25`, `50`, `-16/3` |
| 33 | 22 | 16 | 4 | proste i odcinki | ujawnienie rozwiązania |
| 34 | 24 | 19 | 5 | stereometria | liczbowy, `192` |

Sześć definicji wektorowych Avalonia `exam-mm17-p0p-z06`, `exam-mm17-p0p-z10`, `exam-mm17-p0p-z14`, `exam-mm17-p0p-z15`, `exam-mm17-p0p-z18` i `exam-mm17-p0p-z21` odtwarza informacje graficzne bez aktywnych rastrów. Każda ma opis alternatywny, stabilny identyfikator i numer strony arkusza.

## Testy i proweniencja

`Matura2017ContentTests` kontroluje źródła, sumy SHA-256, kontrakt `34/34/50`, strony, punktację, tryby odpowiedzi, wyniki liczbowe i sześć diagramów. Testy katalogu, UI i smoke testu kontrolują 46 aktywnych arkuszy, 1 281 jednostek postępu i 249 diagramów.

Grupa `cke-2017-correction-basic-exam` ma status `blocked`. Katalog `runtime-vector-diagrams` też ma tymczasowo status `blocked`, ponieważ zawiera sześć nowych figur pochodnych z tego arkusza. Manifest ma `releaseEligible=false` do czasu odrębnego rozszerzenia deklaracji właściciela obejmującego arkusz, zasady oceniania i pochodne diagramy 2017.
