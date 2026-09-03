# Matura maj 2016 PR - Formuła 2015

## Źródła przypięte

- [Arkusz CKE MMA-R1_1P-162](https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2016/Matematyka/MMA-R1_1P-162.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna, archiwum OKE Warszawa
  - SHA-256: `94D1D433D16FB9FAF91F0B68B548CCD2D2F833317BCD6E7023FB3EB44C57B34B`
  - weryfikacja: 2026-09-02
- [Zasady oceniania CKE MMA-R1-N](https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2016/Matematyka/MMA-R1-N.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna, archiwum OKE Warszawa
  - SHA-256: `45A857F24A2919FE97CE5D7BA4EE6563F28FB47E70B52D196A9FEFF5F127B6F0`
  - weryfikacja: 2026-09-02

Arkusz z 9 maja 2016 r. ma 16 oficjalnych zadań, 16 jednostek postępu, 50 punktów i czas pracy 180 minut. Dane aplikacji mają identyfikator `matura-maj-2016-rozszerzona`; trwałe identyfikatory postępu to `mm16-r0-z01` do `mm16-r0-z16`.

## Macierz pokrycia

| Zadania | Strony arkusza | Strony zasad | Punkty | Tematy | Tryb |
| --- | --- | --- | --- | --- | --- |
| 1-5 | 2-4 | 2 | po 1 | algebra, funkcje, pochodne, granice | wybór |
| 6 | 4 | 2 | 2 | prawdopodobieństwo warunkowe | liczbowy, `753` |
| 7 | 5 | 3 | 2 | ciągi geometryczne | liczbowy, `187` |
| 8-9 | 6-8 | 4, 11 | po 3 | dowód, planimetria | ujawnienie rozwiązania |
| 10-11 | 10-11 | 14, 17 | po 4 | funkcja liniowa, trygonometria | ujawnienie rozwiązania |
| 12 | 12 | 20 | 6 | funkcja kwadratowa | ujawnienie rozwiązania |
| 13 | 14 | 23 | 5 | geometria analityczna | ujawnienie rozwiązania |
| 14 | 16 | 26 | 3 | kombinatoryka | ujawnienie rozwiązania |
| 15 | 18 | 30 | 6 | stereometria | ujawnienie rozwiązania |
| 16 | 20 | 32 | 7 | optymalizacja i pochodna | ujawnienie rozwiązania |

Trzy definicje wektorowe Avalonia `exam-mm16-r0-z03`, `exam-mm16-r0-z09` i `exam-mm16-r0-z16` odtwarzają wykres oraz figury z arkusza bez aktywnych rastrów. Każda ma opis alternatywny, stabilny identyfikator i numer strony arkusza.

## Testy i proweniencja

`Matura2016ContentTests` kontroluje źródła, sumy SHA-256, kontrakt `16/16/50`, strony, punktację, tryby odpowiedzi, klucz zadań zamkniętych, wyniki liczbowe i trzy diagramy. Testy katalogu, UI i smoke testu kontrolują 32 aktywne arkusze, 889 jednostek postępu i 226 diagramów.

Grupa `cke-2016-main-extended-exam` ma status `blocked`. Katalog `runtime-vector-diagrams` też ma tymczasowo status `blocked`, ponieważ zawiera figury pochodne z tego arkusza i dalszych niezatwierdzonych arkuszy 2017-2016. Manifest ma `releaseEligible=false` do czasu odrębnego rozszerzenia deklaracji właściciela obejmującego arkusz, zasady oceniania i pochodne diagramy.
