# Pokrycie matury maj 2025 - poziom podstawowy

## Zakres

Aktywny arkusz `matura-maj-2025-podstawowa` odwzorowuje egzamin maturalny z matematyki na poziomie podstawowym z 6 maja 2025 r., kod `MMAP-P0-100-A-2505`. Aplikacja zawiera treść 31 oficjalnie numerowanych zadań jako 35 osobno ocenianych jednostek postępu, rozwiązania, kryteria punktowania i dziewięć deterministycznych diagramów wektorowych. Maksymalny wynik wynosi 50 punktów, a czas pracy 180 minut.

Okładka, puste strony, brudnopis i karta odpowiedzi nie są treścią aplikacji.

## Przypięte źródła

- [Arkusz CKE MMAP-P0-100-A-2505](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2025/Matematyka/poziom_podstawowy/MMAP-P0-100-A-2505-arkusz.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `C5F8AFDE91393BEA3E5980560ADA103389679473DBD0C11A7485040F06631C85`
  - weryfikacja: 2026-08-05
- [Zasady oceniania CKE MMAP-P0-100-2505](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2025/zasady_oceniania/MMAP-P0-100-2505-zasady.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `D272201B35AD7829315C6897500F036A8619BBEE42B38291037DF952F9F150E5`
  - weryfikacja: 2026-08-05

## Macierz treści

| Zakres | Wartość |
| --- | ---: |
| oficjalne zadania | 31 |
| jednostki postępu | 35 |
| maksymalna punktacja | 50 |
| pojedynczy wybór | 23 |
| odpowiedź liczbowa | 4 |
| odpowiedź złożona | 4 |
| pełne rozwiązanie do ujawnienia | 4 |
| diagramy wektorowe | 9 |

Podzielone zadania zachowują oficjalne oznaczenia: `12.1`-`12.3`, `14.1`-`14.2` oraz `18.1`-`18.2`. Pozostałe zadania mają oznaczenia `1`-`11`, `13`, `15`-`17` i `19`-`31`. Każda jednostka ma unikalny identyfikator `mm25-p0-*`, numer strony arkusza, numer strony zasad oceniania, punktację, zweryfikowaną odpowiedź, rozwiązanie i kryterium punktowania.

## Diagramy

Dziewięć figur z zadań 6, 11, 12, 18, 19, 20, 21, 30 i 31 zapisano w `Content/diagrams.json` jako definicje `exam-mm25-*`. Są renderowane przez `DiagramView` z prymitywów Avalonia, skalowane z zachowaniem proporcji i opisane tekstem alternatywnym. Aplikacja nie pakuje stron PDF ani rastrów z arkusza.

## Walidacja

`Matura2025ContentTests` wymaga kontraktu `31/35/50`, dokładnych etykiet, stron, punktacji, kluczy odpowiedzi, identyfikatorów źródeł, sum SHA-256 i dziewięciu używanych diagramów. Testy wspólnego repozytorium wymagają dwudziestu sześciu aktywnych arkuszy, 722 jednostek postępu, agregacji wszystkich zadań w 17 tematach i zachowania 35 identyfikatorów `mp21-*`.

Pochodzenie i podstawa redystrybucji są zapisane w [manifeście proweniencji](CONTENT_PROVENANCE.md) oraz w [deklaracji praw do zasobów](ASSET_RIGHTS_DECLARATION.md).
