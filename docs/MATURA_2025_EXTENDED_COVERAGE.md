# Pokrycie matury maj 2025 - poziom rozszerzony

## Zakres

Aktywny arkusz `matura-maj-2025-rozszerzona` odwzorowuje egzamin maturalny z matematyki na poziomie rozszerzonym z 12 maja 2025 r., kod `MMAP-R0-100-A-2505`. Aplikacja zawiera treść 12 oficjalnie numerowanych zadań jako 13 osobno ocenianych jednostek postępu, wyniki, rozwiązania i kryteria punktowania. Maksymalny wynik wynosi 50 punktów, a czas pracy 180 minut.

Okładka, puste strony, brudnopis i karta odpowiedzi nie są treścią aplikacji. W treści zadań tego arkusza nie ma figur wymagających osobnych definicji diagramów.

## Przypięte źródła

- [Arkusz CKE MMAP-R0-100-A-2505](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2025/Matematyka/poziom_rozszerzony/MMAP-R0-100-A-2505-arkusz.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `457B057602D81CF93A9688E7F4CB74103F4579B37C2B1A2A9AACE28C891CD4AD`
  - weryfikacja: 2026-08-05
- [Zasady oceniania CKE MMAP-R0-100-2505](https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2025/zasady_oceniania/MMAP-R0-100-2505-zasady.pdf)
  - wydawca: Centralna Komisja Egzaminacyjna
  - SHA-256: `B196084F2B9505D14C66E3CBE0064BBA7E4BA0F3FFA613500ED701A97724E523`
  - weryfikacja: 2026-08-05

## Macierz treści

| Zakres | Wartość |
| --- | ---: |
| oficjalne zadania | 12 |
| jednostki postępu | 13 |
| maksymalna punktacja | 50 |
| odpowiedź liczbowa | 4 |
| odpowiedź złożona | 3 |
| pełne rozwiązanie do ujawnienia | 6 |

Zadanie 12 zachowuje oficjalny podział na `12.1` i `12.2`. Pozostałe zadania mają oznaczenia `1`-`11`. Każda jednostka ma unikalny identyfikator `mm25-r0-*`, numer strony arkusza, numer strony zasad oceniania, punktację, zweryfikowany wynik, rozwiązanie i kryterium punktowania.

Tryb `compound` obsługuje dwie sumy ciągów w zadaniu 6, sześć współrzędnych w zadaniu 8 oraz wysokość i objętość w zadaniu 12.2. Zadania dowodowe i wyniki symboliczne używają trybu `revealOnly`, dzięki czemu pełne rozwiązanie jest ujawniane świadomie.

## Walidacja

`Matura2025ContentTests` wymaga kontraktu `12/13/50`, dokładnych etykiet, stron, punktacji, wyników, identyfikatorów źródeł i sum SHA-256. Testy interfejsu przechodzą przez wszystkie odpowiedzi złożone, osobne liczniki postępu, agregację 17 tematów i renderowanie list przy obsługiwanych rozmiarach okna.

Pochodzenie i podstawa redystrybucji są zapisane w [manifeście proweniencji](CONTENT_PROVENANCE.md) oraz w [deklaracji praw do zasobów](ASSET_RIGHTS_DECLARATION.md).
