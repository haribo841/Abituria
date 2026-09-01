# Matura maj 2026 PR - macierz pokrycia

## Zakres

Aktywny arkusz `matura-maj-2026-rozszerzona` odwzorowuje główny egzamin maturalny z matematyki na poziomie rozszerzonym z 11 maja 2026 r., kod `MMAP-R0-100-A-2605`. Okładka, puste strony i karta odpowiedzi nie są treścią aplikacji.

Transkrypcja zachowuje 12 oficjalnie numerowanych zadań jako 13 osobno ocenianych części, 180 minut pracy i 50 punktów. Zadania `12.1` i `12.2` mają wspólny identyfikator grupy, lecz oddzielny postęp i punktację. Arkusz rozszerzony występuje po arkuszu podstawowym 2026 i przed arkuszami 2025 oraz zachowanym arkuszem poprawkowym 2021.

## Przypięte źródła

- [Arkusz CKE MMAP-R0-100-A-2605](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2026/Matematyka/poziom_rozszerzony/MMAP-R0-100-A-2605-arkusz.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `DEC5F06020C35DCDABAB5747942BEDFC49CF7307B27F0AD105FAA93741D03964`
  - weryfikacja: 2026-08-03
- [Zasady oceniania CKE MMAP-R0-100-2605](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2026/Matematyka/poziom_rozszerzony/MMAP-R0-100-2605-zasady.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `D7C014240AF16885DBDD1711D923AFF24951B2F514B7C5659E0B6F16508878BD`
  - weryfikacja: 2026-08-03

Sumy obliczono z pobranych plików źródłowych. Te same adresy, sumy i data występują w `Content/exam-2026-main-extended.json` oraz w testach kontraktu.

## Liczniki

| Element | Wymagane | Stan katalogu |
| --- | ---: | ---: |
| oficjalnie numerowane zadania | 12 | 12 |
| jednostki postępu | 13 | 13 |
| punkty | 50 | 50 |
| wynik liczbowy | 3 | 3 |
| odpowiedź złożona | 3 | 3 |
| świadome ujawnienie rozwiązania | 7 | 7 |
| tematy wspólnego indeksu | 17 | 17 |
| diagramy źródłowe | 3 | 3 |

Identyfikatory używają prefiksu `mm26-r0-`, są globalnie unikalne i mają mniej niż 80 znaków. Historyczne identyfikatory `mp21-*` oraz identyfikatory arkusza podstawowego `mm26-p0-*` nie zostały zmienione.

## Diagramy

W `Content/diagrams.json` znajdują się trzy nowe deterministyczne definicje wektorowe:

| Id | Strona arkusza | Treść |
| --- | ---: | --- |
| `exam-mm26-r0-z04` | 8 | kwadrat z punktami K, L, M, P i Q |
| `exam-mm26-r0-z11` | 24 | czworokąt wpisany w okrąg i opisany na okręgu |
| `exam-mm26-r0-z12` | 28 | trójkątny kwietnik z wpisaną okrągłą fontanną |

Każda definicja ma `sourceId` równy `cke-2026-main-extended`, stronę źródłową, niepusty opis alternatywny i wyłącznie walidowane prymitywy wektorowe. Zadania 12.1 i 12.2 współdzielą ten sam diagram. Aplikacja nie paczkuje rastrów arkusza.

## Kurs Formuły 2023

Kurs zachowuje odrębną warstwę autorską. Audyt potwierdza dokładnie 4 grupy, 13 obszarów, 73 wymagania podstawowe, 46 dodatkowych wymagań rozszerzonych, 238 przykładów i 357 ćwiczeń Adama Kubisia. Żaden z tych materiałów nie został zastąpiony. Dodatkowy katalog zawiera osobno oznaczone 66 przykładów podstawowych i 31 rozszerzonych z informatorów CKE, wraz z oficjalnymi wymaganiami, zasadami oceniania, rozwiązaniami i stronami źródłowymi.

## Proweniencja i Issue #7

Transkrypcja arkusza rozszerzonego ma status `approved` w `Content/provenance.json` na podstawie rozszerzenia `docs/ASSET_RIGHTS_DECLARATION.md` z 3 sierpnia 2026 r. Trzy figury są autorskimi implementacjami wektorowymi Avalonia, bez aktywnych rastrów, `Image` lub `Bitmap`. Bieżący worktree ma jednak `releaseEligible=false`, ponieważ grupy matur głównych i poprawkowych 2019 i 2020, matur głównych 2021 PP i PR, matur głównych i poprawkowej 2022 PP oraz PR, matury głównej i poprawkowej 2023 PP oraz matur poprawkowych 2024 i 2025 PP wraz z osiemdziesięcioma czterema diagramami nie są objęte deklaracją.

Deklaracja wskazuje oba arkusze 2026, oba zestawy zasad oceniania, cztery adresy, cztery sumy SHA-256, zakres redystrybucji transkrypcji w Abiturii oraz dziesięć autorskich implementacji wektorowych Avalonia. Grupy `cke-2026-main-basic-exam` i `cke-2026-main-extended-exam` mają status `approved`, ale `runtime-vector-diagrams` jest chwilowo `blocked` z powodu nowych definicji matury głównej i poprawkowej 2023 PP.

## Testy

- `Matura2026ExtendedContentTests` sprawdza kontrakt `12/13/50`, źródła, sumy, etykiety, strony, punktację, tryby odpowiedzi, wyniki i trzy diagramy.
- `Matura2026ContentTests` zachowuje kontrakt `33/37/50` poziomu podstawowego oraz komplet 35 zadań `mp21-*`.
- `Matura2026UiTests` sprawdza dwadzieścia sześć arkuszy, 722 jednostki postępu, agregację 17 tematów, kontekst nawigacji, osobne liczniki i odpowiedzi złożone.
- `DiagramCatalogTests` renderuje wszystkie 195 diagramów w trzech motywach i przy rozmiarach `720x520`, `960x640` oraz `1280x820`.
- `ReleaseRuntimeTests` wymaga dwudziestu sześciu arkuszy i 722 jednostek postępu.
