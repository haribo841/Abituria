# Matura maj 2026 - macierz pokrycia

## Zakres

Aktywny arkusz `matura-maj-2026-podstawowa` odwzorowuje główny egzamin maturalny z matematyki na poziomie podstawowym z 5 maja 2026 r., kod `MMAP-P0-100-A-2605`. Okładka, puste strony i karta odpowiedzi nie są treścią aplikacji.

Transkrypcja zachowuje 33 oficjalnie numerowane zadania jako 37 osobno ocenianych części, 180 minut pracy i 50 punktów. W aplikacji arkusz występuje przed arkuszem rozszerzonym 2026 i zachowanym arkuszem poprawkowym 2021.

## Przypięte źródła

- [Arkusz CKE MMAP-P0-100-A-2605](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2026/Matematyka/poziom_podstawowy/MMAP-P0-100-A-2605-arkusz.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `B7BD89434CA5CCFA33824B0CF063FF7CDDFF47B353059ECF225418E29BEEE71D`
  - weryfikacja: 2026-08-02
- [Zasady oceniania CKE MMAP-P0-100-2605](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2026/Matematyka/poziom_podstawowy/MMAP-P0-100-2605-zasady.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `A982890CF5EA17206266E4A64B7BFDF96F46FAB08C7435B022CCE5B3908A65AC`
  - weryfikacja: 2026-08-02

Sumy zostały obliczone z pobranych plików źródłowych. Te same adresy, sumy i data występują w `Content/exam-2026-main-basic.json` oraz w testach kontraktu.

## Liczniki

| Element | Wymagane | Stan katalogu |
| --- | ---: | ---: |
| oficjalnie numerowane zadania | 33 | 33 |
| jednostki postępu | 37 | 37 |
| punkty | 50 | 50 |
| pojedynczy wybór | 21 | 21 |
| odpowiedź złożona | 6 | 6 |
| wynik liczbowy | 6 | 6 |
| świadome ujawnienie rozwiązania | 4 | 4 |
| tematy wspólnego indeksu | 17 | 17 |
| diagramy źródłowe | 7 | 7 |

Osobne jednostki postępu tworzą zadania `12.1`, `12.2`, `13.1`, `13.2`, `24.1`, `24.2`, `33.1` i `33.2`. Ich identyfikatory używają prefiksu `mm26-p0-` i mają mniej niż 80 znaków. Historyczne identyfikatory `mp21-*` nie zostały zmienione.

## Diagramy

W `Content/diagrams.json` znajdują się deterministyczne definicje wektorowe:

| Id | Strona arkusza | Treść |
| --- | ---: | --- |
| `exam-mm26-z12` | 12 | wykres funkcji przedziałami liniowej |
| `exam-mm26-z13` | 14 | prosta i kąt nachylenia |
| `exam-mm26-z18` | 20 | trójkąt prostokątny |
| `exam-mm26-z19` | 21 | kąty w okręgu |
| `exam-mm26-z20` | 22 | proste równoległe i sieczne |
| `exam-mm26-z21` | 23 | dwusieczna w trójkącie |
| `exam-mm26-z31` | 30 | dwa wykresy rozkładu ocen |

Każdy diagram ma stabilny identyfikator, `sourceId` równy `cke-2026-main-basic`, stronę źródłową, niepusty opis alternatywny i wyłącznie walidowane prymitywy wektorowe. Aplikacja nie paczkuje rastrów arkusza.

## Proweniencja

Treść arkusza ma status `approved` w `Content/provenance.json` na podstawie rozszerzenia `docs/ASSET_RIGHTS_DECLARATION.md` z 3 sierpnia 2026 r. Siedem figur jest autorskimi implementacjami wektorowymi Avalonia, bez aktywnych rastrów, `Image` lub `Bitmap`. Bieżący worktree ma jednak `releaseEligible=false`, ponieważ nowe grupy matur głównych 2022 PP i PR oraz poprawkowej 2022 PP, matury głównej i poprawkowej 2023 PP oraz matur poprawkowych 2024 i 2025 PP wraz z trzydziestoma sześcioma diagramami nie są objęte deklaracją.

Deklaracja wskazuje źródła obu matur 2026, ich adresy i sumy SHA-256, zakres redystrybucji w Abiturii oraz dziesięć autorskich implementacji wektorowych Avalonia. Grupy `cke-2026-main-basic-exam` i `cke-2026-main-extended-exam` mają status `approved`, natomiast wspólny katalog `runtime-vector-diagrams` jest chwilowo `blocked` z powodu nowych definicji matury głównej i poprawkowej 2023 PP.

## Testy

- `Matura2026ContentTests` sprawdza kontrakt `33/37/50`, źródła, sumy, etykiety, klucze, tryby, tematy, identyfikatory i diagramy.
- `Matura2026UiTests` sprawdza wybór arkusza, agregację 17 tematów, kontekst nawigacji, osobny postęp, odpowiedzi złożone, dostępność i trzy rozmiary okna.
- `CompoundAnswerEvaluatorTests` sprawdza wybór, wynik liczbowy, zapis tekstowy przedziału, przecinek lub kropkę i niepoprawne dane.
- `DiagramCatalogTests` renderuje wszystkie 139 diagramów w trzech motywach i przy rozmiarach `720x520`, `960x640` oraz `1280x820`.
- `ReleaseRuntimeTests` wymaga siedemnastu arkuszy i 473 jednostek postępu, zachowując 35 historycznych zadań 2021.
