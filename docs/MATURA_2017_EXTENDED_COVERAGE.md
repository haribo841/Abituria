# Matura maj 2017 PR - Formuła 2015

## Źródła przypięte

- [Arkusz CKE MMA-R1_1P-172](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2017/formula_od_2015/matematyka/MMA-R1_1P-172.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `4D6F682AA0BB350CA67CA4F2CAE967332264339DA1AF006D5A4F0C5F23EB0277`
  - weryfikacja: 2026-09-01
- [Zasady oceniania CKE MMA-R1-N](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2017/formula_od_2015/zasady_oceniania/MMA-R1-N.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `2B3DDFC0161632B6F6E5FD5CBC47BAE14FE618942D7636D4B870F9C8B3F4E36C`
  - weryfikacja: 2026-09-01

Arkusz z 9 maja 2017 r. ma 15 oficjalnych zadań, 15 jednostek postępu, 50 punktów i czas pracy 180 minut. Dane aplikacji mają identyfikator `matura-maj-2017-rozszerzona`; trwałe identyfikatory postępu to `mm17-r0-z01` do `mm17-r0-z15`.

## Macierz pokrycia

| Zadania | Strony arkusza | Strony zasad | Punkty | Tematy | Tryb |
| --- | --- | --- | --- | --- | --- |
| 1-4 | 2 | 2 | po 1 | potęgi, ciągi, planimetria, proste i odcinki | wybór |
| 5 | 4 | 2 | 2 | równania | liczbowy, `125` |
| 6-8 | 4-6 | 3-7 | po 3 | funkcja kwadratowa, zadania dowodowe | ujawnienie rozwiązania |
| 9-11 | 7-9 | 11-18 | po 4 | stereometria, trygonometria, prawdopodobieństwo | ujawnienie lub liczbowy `11/16` |
| 12-13 | 10-12 | 20-23 | po 5 | równania, proste i odcinki | ujawnienie rozwiązania |
| 14 | 14 | 27 | 6 | ciągi | ujawnienie rozwiązania |
| 15 | 16 | 31 | 7 | stereometria | ujawnienie rozwiązania |

Trzy definicje wektorowe Avalonia `exam-mm17-r0-z03`, `exam-mm17-r0-z08` i `exam-mm17-r0-z09` odtwarzają informacje graficzne bez aktywnych rastrów. Każda ma opis alternatywny, stabilny identyfikator i numer strony arkusza.

## Testy i proweniencja

`Matura2017ContentTests` kontroluje źródła, sumy SHA-256, kontrakt `15/15/50`, strony, punktację, tryby odpowiedzi, wyniki liczbowe i trzy diagramy. Testy katalogu, UI i smoke testu kontrolują 46 aktywnych arkuszy, 1 281 jednostek postępu i 249 diagramów.

Grupa `cke-2017-main-extended-exam` ma status `blocked`. Katalog `runtime-vector-diagrams` też ma tymczasowo status `blocked`, ponieważ zawiera trzy nowe figury pochodne z tego arkusza. Manifest ma `releaseEligible=false` do czasu odrębnego rozszerzenia deklaracji właściciela obejmującego arkusz, zasady oceniania i pochodne diagramy 2017.
