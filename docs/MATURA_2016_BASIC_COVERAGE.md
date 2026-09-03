# Matura maj 2016 PP - Formuła 2015

## Źródła przypięte

- [Arkusz CKE MMA-P1_1P-162](https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2016/Matematyka/MMA-P1_1P-162.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna, archiwum OKE Warszawa
  - SHA-256: `3AD62333A0E84B0AB344D71449EAE8A051A2A90666D5578F04157812C99E5989`
  - weryfikacja: 2026-09-02
- [Zasady oceniania CKE MMA-P1-N](https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2016/Matematyka/MMA-P1-N.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna, archiwum OKE Warszawa
  - SHA-256: `CF772FEFB9321A6FB9E052C778AB8E4E54D2EEDCAEC050958DEAF84ABE386ED3`
  - weryfikacja: 2026-09-02

Arkusz z 5 maja 2016 r. ma 34 oficjalne zadania, 34 jednostki postępu, 50 punktów i czas pracy 170 minut. Dane aplikacji mają identyfikator `matura-maj-2016-podstawowa`; trwałe identyfikatory postępu to `mm16-p0-z01` do `mm16-p0-z34`.

## Macierz pokrycia

| Zadania | Strony arkusza | Strony zasad | Punkty | Tematy | Tryb |
| --- | --- | --- | --- | --- | --- |
| 1-6 | 2 | 2-3 | po 1 | potęgi, logarytmy, procenty, równania, nierówności | wybór |
| 7-11 | 2-4 | 3 | po 1 | planimetria, funkcja liniowa i kwadratowa | wybór |
| 12-16 | 6 | 3-4 | po 1 | potęgi, planimetria, ciągi | wybór |
| 17-20 | 8 | 4-5 | po 1 | trygonometria, planimetria, proste i odcinki | wybór |
| 21-25 | 10 | 5 | po 1 | proste i odcinki, prawdopodobieństwo, stereometria, statystyka | wybór |
| 26-31 | 12-17 | 6-12 | po 2 | statystyka, nierówności, równania, dowody, ciągi, logarytmy | ujawnienie rozwiązania |
| 32 | 18 | 13 | 4 | planimetria | ujawnienie rozwiązania |
| 33 | 20 | 15 | 5 | stereometria | ujawnienie rozwiązania |
| 34 | 22 | 17 | 4 | prawdopodobieństwo | ujawnienie rozwiązania |

Siedem definicji wektorowych Avalonia `exam-mm16-p0-z07`, `exam-mm16-p0-z10`, `exam-mm16-p0-z13`, `exam-mm16-p0-z16`, `exam-mm16-p0-z19`, `exam-mm16-p0-z24` i `exam-mm16-p0-z29` odtwarza informacje graficzne bez aktywnych rastrów. Każda ma opis alternatywny, stabilny identyfikator i numer strony arkusza.

## Testy i proweniencja

`Matura2016ContentTests` kontroluje źródła, sumy SHA-256, kontrakt `34/34/50`, strony, punktację, tryby odpowiedzi, klucz zadań zamkniętych i siedem diagramów. Testy katalogu, UI i smoke testu kontrolują 32 aktywne arkusze, 889 jednostek postępu i 226 diagramów.

Grupa `cke-2016-main-basic-exam` ma status `blocked`. Katalog `runtime-vector-diagrams` też ma tymczasowo status `blocked`, ponieważ zawiera figury pochodne z tego arkusza i dalszych niezatwierdzonych arkuszy 2017-2016. Manifest ma `releaseEligible=false` do czasu odrębnego rozszerzenia deklaracji właściciela obejmującego arkusz, zasady oceniania i pochodne diagramy.
