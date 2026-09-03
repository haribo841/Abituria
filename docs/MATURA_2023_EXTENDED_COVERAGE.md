# Matura maj 2023 PR - macierz pokrycia

## Zakres

Aktywny arkusz matura-maj-2023-rozszerzona odwzorowuje główny egzamin maturalny z matematyki na poziomie rozszerzonym z 12 maja 2023 r., kod MMAP-R0-100-2305. Okładka, instrukcja, puste strony, brudnopis i karta odpowiedzi nie są treścią aplikacji.

Transkrypcja zachowuje 13 oficjalnie numerowanych zadań jako 14 osobno ocenianych jednostek postępu, 180 minut pracy i 50 punktów. Zadania 12.1 i 12.2 mają wspólny identyfikator grupy, lecz oddzielny postęp i punktację. Arkusz jest jedenastym aktywnym zestawem - po maturze podstawowej i poprawkowej 2023 oraz maturach 2026, 2025 i 2024, a przed arkuszami 2022 i zachowanym arkuszem poprawkowym 2021.

## Przypięte źródła

- [Arkusz CKE MMAP-R0-100-2305](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2023/Matematyka/poziom_rozszerzony/MMAP-R0-100-2305.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `24EC13FEA77323841A8538E85B816C7EE36199E64F5F756E30340489864EC207`
  - weryfikacja: 2026-08-12
- [Zasady oceniania CKE MMAP-R0-100-2305](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2023/Matematyka/poziom_rozszerzony/MMAP-R0-100-2305-zasady.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `B8FECD4D23811033E0DFF6C532A405F04ECE0CCC469A6D60412E353F1BBDBD2B`
  - weryfikacja: 2026-08-12

## Macierz zadań

| Zadanie | Identyfikator | Punkty | Strona arkusza | Strona zasad | Temat | Tryb odpowiedzi | Wynik lub zakres |
| --- | --- | ---: | ---: | ---: | --- | --- | --- |
| 1 | mm23-r0-z01 | 2 | 4 | 3 | Ciągi | revealOnly | m(t)=4·0,81ᵗ, 5 dób |
| 2 | mm23-r0-z02 | 3 | 5 | 4 | Prawdopodobieństwo | numeric | 1/64 |
| 3 | mm23-r0-z03 | 3 | 6 | 8 | Funkcja kwadratowa | compound | x₀=-3, y=-8x/11+9/11 |
| 4 | mm23-r0-z04 | 3 | 7 | 10 | Zadania dowodowe | revealOnly | x=y=2 |
| 5 | mm23-r0-z05 | 3 | 8 | 12 | Zadania dowodowe | revealOnly | długość ND = √3+1 |
| 6 | mm23-r0-z06 | 3 | 10 | 20 | Trygonometria | revealOnly | x=7π/12+kπ lub 11π/12+kπ |
| 7 | mm23-r0-z07 | 4 | 11 | 23 | Stereometria | numeric | √6 |
| 8 | mm23-r0-z08 | 4 | 12 | 28 | Planimetria | revealOnly | 16√3+10 |
| 9 | mm23-r0-z09 | 4 | 14 | 31 | Nierówności | revealOnly | (-11/3, 14/3) |
| 10 | mm23-r0-z10 | 4 | 16 | 34 | Ciągi | revealOnly | 8(4+√10)a/3 |
| 11 | mm23-r0-z11 | 5 | 18 | 36 | Funkcja kwadratowa | revealOnly | (11/5, 9/4) |
| 12.1 | mm23-r0-z12-1 | 2 | 20 | 40 | Logarytmy | revealOnly | f(x)=x⁴+x²-6x |
| 12.2 | mm23-r0-z12-2 | 4 | 21 | 41 | Funkcja kwadratowa | numeric | -4 |
| 13 | mm23-r0-z13 | 6 | 22 | 43 | Proste i odcinki | compound | C=(11/10, -3/10) |

Suma punktów w macierzy wynosi 50. Identyfikatory mają mniej niż 80 znaków i nie kolidują z historycznymi identyfikatorami mp21-*, dlatego zapis postępu SQLite pozostaje zgodny wstecznie.

## Treść, rozwiązania i diagramy

Content/exam-2023-main-extended.json zawiera treść każdego zadania, wynik, pełne rozwiązanie, kryteria punktowania oraz strony obu dokumentów źródłowych. Tryby numeric, compound i revealOnly są istniejącymi mechanizmami katalogu matur i nie wymagają zmiany schematu bazy.

Zadania 5, 7, 10 i 13 odwołują się odpowiednio do exam-mm23-r0-z05, exam-mm23-r0-z07, exam-mm23-r0-z10 oraz exam-mm23-r0-z13 w Content/diagrams.json. Są to autorskie, deterministyczne definicje wektorowe Avalonia z opisami alternatywnymi i numerami stron 8, 11, 16 oraz 22. Katalog diagramów zawiera obecnie 226 definicji; nie dodano aktywnego rastra.

## Walidacja

Test Matura2023ExtendedContentTests sprawdza:

- kontrakt 13/14/50, kolejność, etykiety, punkty oraz strony obu źródeł;
- przypięte URL-e, sumy SHA-256, datę weryfikacji, wyniki i tryby odpowiedzi;
- zachowanie katalogów 2021, 2024, 2025 i 2026, 17 tematów oraz niezmienionych identyfikatorów postępu;
- kompletność czterech diagramów, opisy alternatywne, źródła i renderowanie bez rastra;
- zatwierdzoną proweniencję nowej transkrypcji.

Matura2026UiTests dodatkowo obejmuje wybór trzydziestu dwóch arkuszy, losowanie ograniczone do wybranego arkusza, agregację 889 jednostek postępu według 17 tematów, kontekst powrotu oraz osobny licznik Matura maj 2023 PR: x / 14.

## Prawa i wydanie

Grupa cke-2023-main-extended-exam w Content/provenance.json ma status approved. Osobiste rozszerzenie docs/ASSET_RIGHTS_DECLARATION.md z 12 sierpnia 2026 r. obejmuje arkusz MMAP-R0-100-2305, zasady MMAP-R0-100-2305 i ich transkrypcję wyłącznie jako integralną część Abiturii.

Grupa tego arkusza ma status `approved`. Wspólny katalog `runtime-vector-diagrams` jest obecnie `blocked`, ponieważ zawiera także figury z arkuszy 2017 i 2016 nieobjętych deklaracją. Manifest ma dlatego `releaseEligible=false`; przed wydaniem zwykła walidacja proweniencji i wariant `Test-ContentProvenance.ps1 -RequireReleaseEligible` muszą przejść po rozszerzeniu deklaracji.
