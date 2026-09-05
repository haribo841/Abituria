# Matura poprawkowa 2016 PP - Formuła 2015

## Źródła przypięte

- [Arkusz MMA-P1_1P-164 w zachowanym archiwum publicznym](https://arkusze.pl/maturalne/matematyka-2016-sierpien-poprawkowa-podstawowa.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna, archiwum publiczne: arkusze.pl
  - SHA-256: `82EB6402EE0422B09DF15CB5F65D22431FDC6F064ED8FBF250BB59888211975D`
  - weryfikacja: 2026-09-02
- [Zasady oceniania MMA-P1_1P-164 w zachowanym archiwum publicznym](https://arkusze.pl/maturalne/matematyka-2016-sierpien-poprawkowa-podstawowa-odpowiedzi.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna, archiwum publiczne: arkusze.pl
  - SHA-256: `BA3BD6A4B2AE4C1D87AE5BF499A8ECB533AA80791C8058DC58CB721CD87DCB82`
  - weryfikacja: 2026-09-02

Arkusz poprawkowy z 23 sierpnia 2016 r. ma 34 oficjalne zadania, 34 jednostki postępu, 50 punktów i czas pracy 170 minut. Dane aplikacji mają identyfikator `matura-poprawkowa-2016-podstawowa`; trwałe identyfikatory postępu to `mm16-p0p-z01` do `mm16-p0p-z34`.

## Macierz pokrycia

| Zadania | Strony arkusza | Strony zasad | Punkty | Tematy | Tryb |
| --- | --- | --- | --- | --- | --- |
| 1-6 | 2 | 2-3 | po 1 | równania, procenty, potęgi, logarytmy, nierówności, funkcja kwadratowa | wybór |
| 7-12 | 4-6 | 3 | po 1 | funkcja liniowa, ciągi, trygonometria, funkcja kwadratowa, układy równań | wybór |
| 13-18 | 6-8 | 4 | po 1 | potęgi, proste, stereometria, trygonometria, planimetria | wybór |
| 19-25 | 8-10 | 4-5 | po 1 | planimetria, proste, stereometria, statystyka, kombinatoryka, prawdopodobieństwo | wybór |
| 26-30 | 12-16 | 6-10 | po 2 | nierówności, prawdopodobieństwo, dowody, funkcja kwadratowa | ujawnienie rozwiązania |
| 31 | 17 | 11 | 4 | ciągi | liczbowy, `676368` |
| 32 | 18 | 12 | 4 | geometria analityczna | ujawnienie rozwiązania |
| 33 | 20 | 14 | 5 | stereometria | ujawnienie rozwiązania |
| 34 | 22 | 16 | 2 | prawdopodobieństwo | ujawnienie rozwiązania |

Pięć definicji wektorowych Avalonia `exam-mm16-p0p-z07`, `exam-mm16-p0p-z19`, `exam-mm16-p0p-z21`, `exam-mm16-p0p-z32` i `exam-mm16-p0p-z33` odtwarza informacje graficzne bez aktywnych rastrów. Każda ma opis alternatywny, stabilny identyfikator i numer strony arkusza.

## Testy i proweniencja

`Matura2016ContentTests` kontroluje źródła, sumy SHA-256, kontrakt `34/34/50`, strony, punktację, tryby odpowiedzi, klucz zadań zamkniętych, wynik liczbowy i pięć diagramów. Testy katalogu, UI i smoke testu kontrolują 46 aktywnych arkuszy, 1 281 jednostek postępu i 249 diagramów.

Grupa `cke-2016-correction-basic-exam` ma status `blocked`. Katalog `runtime-vector-diagrams` też ma tymczasowo status `blocked`, ponieważ zawiera figury pochodne z tego arkusza i dalszych niezatwierdzonych arkuszy 2017-2016. Manifest ma `releaseEligible=false` do czasu odrębnego rozszerzenia deklaracji właściciela obejmującego arkusz, zasady oceniania i pochodne diagramy.
