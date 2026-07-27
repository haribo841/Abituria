# Pokrycie tablic matematycznych CKE - Formuła 2023

Data weryfikacji: 27 lipca 2026 r.

Źródłem jest dokument Centralnej Komisji Egzaminacyjnej [Wybrane wzory matematyczne na egzamin maturalny z matematyki](https://bip.cke.gov.pl/attachments/download/9944), opublikowany na stronie egzaminu maturalnego w Formule 2023. Zweryfikowana suma SHA-256 pliku PDF:

57CFF1265A7E38C13ECB6A00F566A37CDFDA667ABF2D550BA65E19E166CC0D45

Audyt objął ekstrakcję tekstu oraz wizualne porównanie wszystkich 36 stron pliku PDF. Poprzedni katalog nie był kompletny i zawierał błędy merytoryczne, między innymi znak nierówności zamiast równości dla wartości bezwzględnej iloczynu, uszkodzony wzór Newtona, błędny wzór na sumę sześcianów oraz błędny licznik odległości punktu od prostej.

## Odwzorowanie sekcji

| Sekcja CKE | Strony drukowane | Dział aplikacji | Stan |
| --- | --- | --- | --- |
| 1. Wartość bezwzględna liczby | 4 | formula-1 | kompletna |
| 2. Potęgi i pierwiastki | 4-5 | formula-2 | kompletna |
| 3. Logarytmy | 5-6 | formula-3 | kompletna |
| 4. Silnia. Współczynnik dwumianowy | 6 | formula-4 | kompletna |
| 5. Wzór dwumianowy Newtona | 7 | formula-5 | kompletna |
| 6. Wzory skróconego mnożenia | 7 | formula-6 | kompletna |
| 7. Funkcja kwadratowa | 7-9 | formula-8 | kompletna |
| 8. Ciągi | 9-11 | formula-7, formula-16 | kompletna, rozdzielona na dwa historyczne działy |
| 9. Trygonometria | 11-14 | formula-12 | kompletna |
| 10. Planimetria | 15-22 | formula-10 | kompletna |
| 11. Geometria analityczna | 22-26 | formula-9 | kompletna |
| 12. Stereometria | 26-28 | formula-11 | kompletna |
| 13. Kombinatoryka | 28 | formula-13 | kompletna |
| 14. Rachunek prawdopodobieństwa | 29-30 | formula-14 | kompletna |
| 15. Parametry danych statystycznych | 31-32 | formula-15 | kompletna |
| 16. Pochodna funkcji | 32-33 | formula-17 | kompletna |
| 17. Tablica wartości funkcji trygonometrycznych | 34 | formula-18 | kompletna |

## Zasady utrzymania

Maszynową listę sekcji, wymaganych podpunktów, stron i identyfikatorów przechowuje tools/seeds/formula-2023-coverage.json. Testy wymagają:

- niezmiennych identyfikatorów i kolejności 18 działów;
- zgodności metadanych źródła i sumy SHA-256;
- odwzorowania każdego wymaganego podpunktu na istniejący dział;
- obecności poprawnych wzorów i nieobecności znanych błędnych zapisów;
- dokładnie 91 wierszy tabeli trygonometrycznej dla kątów od 0 do 90 stopni;
- poprawnego renderowania tekstu, matematyki, tabel i ilustracji.

PDF ani jego wycinki nie są paczkowane z aplikacją. Wzory i tabele są dostępne jako tekst, a odziedziczone diagramy są używane wyłącznie tam, gdzie ich znaczenie odpowiada oficjalnemu dokumentowi.
