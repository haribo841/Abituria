# Matura maj 2024 PR - macierz pokrycia

## Zakres

Aktywny arkusz matura-maj-2024-rozszerzona odwzorowuje główny egzamin maturalny z matematyki na poziomie rozszerzonym z 15 maja 2024 r., kod MMAP-R0-100-A-2405. Okładka, instrukcja, puste strony, brudnopis i karta odpowiedzi nie są treścią aplikacji.

Transkrypcja zachowuje 13 oficjalnie numerowanych zadań jako 14 osobno ocenianych jednostek postępu, 180 minut pracy i 50 punktów. Zadania 13.1 i 13.2 mają wspólny identyfikator grupy, lecz oddzielny postęp i punktację. Arkusz jest siódmym aktywnym zestawem historycznym - po maturach 2026 i 2025, a przed zachowanym arkuszem poprawkowym 2021.

## Przypięte źródła

- [Arkusz CKE MMAP-R0-100-A-2405](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2024/Matematyka/poziom_rozszerzony/MMAP-R0-100-A-2405-arkusz.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `873691F2E3740126D969AAC957CBC5666FCAD7D7FCF8499442781E18F6AD53D6`
  - weryfikacja: 2026-08-08
- [Zasady oceniania CKE MMAP-R0-100-2405](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2024/Matematyka/poziom_rozszerzony/MMAP-R0-100-2405-zasady.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `6535405993D0A9F3360759A2B2335BF7523E177144B207B4D8A6E3D8A3A8AB92`
  - weryfikacja: 2026-08-08

## Macierz zadań

| Zadanie | Identyfikator | Punkty | Strona arkusza | Strona zasad | Temat | Tryb odpowiedzi | Wynik lub zakres |
| --- | --- | ---: | ---: | ---: | --- | --- | --- |
| 1 | mm24-r0-z01 | 2 | 4 | 2 | Procenty | numeric | 59 |
| 2 | mm24-r0-z02 | 2 | 5 | 3 | Ciągi | revealOnly | -∞ |
| 3 | mm24-r0-z03 | 3 | 6 | 5 | Prawdopodobieństwo | numeric | 0,996 |
| 4 | mm24-r0-z04 | 3 | 7 | 6 | Proste i odcinki | compound | a=7/2, b=-5 |
| 5 | mm24-r0-z05 | 3 | 8 | 8 | Logarytmy | revealOnly | dowód tożsamości |
| 6 | mm24-r0-z06 | 3 | 9 | 10 | Kombinatoryka | numeric | 11040 |
| 7 | mm24-r0-z07 | 4 | 10 | 14 | Ciągi | compound | x=5, y=20, z=80 |
| 8 | mm24-r0-z08 | 4 | 12 | 18 | Zadania dowodowe | revealOnly | dowód geometryczny |
| 9 | mm24-r0-z09 | 4 | 14 | 32 | Planimetria | revealOnly | P_AGF=a²/12, P_CEFG=a²/6 |
| 10 | mm24-r0-z10 | 5 | 16 | 40 | Trygonometria | revealOnly | 6 rozwiązań w [0, 2π] |
| 11 | mm24-r0-z11 | 5 | 18 | 44 | Proste i odcinki | revealOnly | S=(6,7) albo S=(-4,-3) |
| 12 | mm24-r0-z12 | 6 | 20 | 48 | Funkcja kwadratowa | revealOnly | m∈(-∞,-3) |
| 13.1 | mm24-r0-z13-1 | 2 | 23 | 51 | Stereometria | revealOnly | wyprowadzenie wzoru P(a) |
| 13.2 | mm24-r0-z13-2 | 4 | 24 | 52 | Funkcja kwadratowa | compound | a=8√3, P=1728+96√3 |

Suma punktów w macierzy wynosi 50. Identyfikatory mają mniej niż 80 znaków i nie kolidują z historycznymi identyfikatorami mp21-*, dlatego zapis postępu SQLite pozostaje zgodny wstecznie.

## Treść, rozwiązania i diagram

Content/exam-2024-main-extended.json zawiera tekst każdego zadania, wynik, pełne rozwiązanie, kryteria punktowania oraz strony obu dokumentów źródłowych. Tryby numeric, compound i revealOnly są istniejącymi mechanizmami katalogu matur i nie wymagają zmiany schematu bazy.

Zadanie 9 odwołuje się do exam-mm24-r0-z09 w Content/diagrams.json. Jest to jedna autorska, deterministyczna definicja wektorowa Avalonia - kwadrat, punkt E, przekątne, odcinek AE oraz punkty F i G - z opisem alternatywnym i numerem strony 14. Aktualny katalog diagramów zawiera 139 definicji; nie dodano aktywnego rastra.

## Walidacja

Test Matura2024ExtendedContentTests sprawdza:

- kontrakt 13/14/50, kolejność, etykiety, punkty oraz strony obu źródeł;
- przypięte URL-e, sumy SHA-256, datę weryfikacji, wyniki i wszystkie wymagane tryby odpowiedzi;
- zachowanie katalogów 2021, 2025 i 2026, 17 tematów oraz niezmienionych identyfikatorów postępu;
- kompletność diagramu, opis alternatywny, źródło i renderowanie bez rastra;
- zatwierdzoną proweniencję nowej transkrypcji.

Matura2026UiTests dodatkowo obejmuje wybór siedemnastu arkuszy, losowanie ograniczone do wybranego arkusza, agregację 473 jednostek postępu według 17 tematów, kontekst powrotu oraz osobny licznik Matura maj 2024 PR: x / 14.

## Prawa i wydanie

Grupa `cke-2024-main-extended-exam` w `Content/provenance.json` ma status `approved`. Osobiste rozszerzenie `docs/ASSET_RIGHTS_DECLARATION.md` z 10 sierpnia 2026 r. obejmuje arkusz MMAP-R0-100-A-2405, zasady MMAP-R0-100-2405 i ich transkrypcję wyłącznie jako integralną część Abiturii.

Grupa tego arkusza pozostaje `approved`, ale bieżący worktree ma `releaseEligible=false` przez osobno zablokowane grupy matur głównych 2022 PP i PR oraz poprawkowej 2022 PP, matury głównej i poprawkowej 2023 PP oraz matur poprawkowych 2024 i 2025 PP wraz z trzydziestoma sześcioma diagramami. Zwykła walidacja proweniencji musi przechodzić; `Test-ContentProvenance.ps1 -RequireReleaseEligible` nie może przejść przed rozszerzeniem deklaracji.
