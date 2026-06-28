# Inwentarz migracji do AvaloniaUI

## Źródła

| Snapshot | Stan | Wykorzystanie |
| --- | --- | --- |
| `Abituria-Core` | Kod WPF zgodny bajtowo z `Projekt-Inzynierski-master`; wcześniej użyty jako baza portu Avalonia | Zasoby `img`, `fonts` i aktualny shell Avalonia |
| `Projekt-Inzynierski-master` | Najpełniejsza wersja treści zadań i działu Wektory | Źródło 35 zadań, podpowiedzi, odpowiedzi, wzorów oraz Wektorów |
| `Abituria-main` | Nowsze menu, kalkulator i widoczne placeholdery | Pełny algorytm kalkulatora, organizacja treści i komunikaty placeholderów |
| `Abituria-Framework` | Starsza wersja .NET Framework 4.7.2 | Brak unikalnej działającej funkcji; jej interfejs ukończenia zadania został zastąpiony trwałym postępem SQLite |

`Abituria-Core` i `Projekt-Inzynierski-master` nie są traktowane jako dwie niezależne linie funkcji, ponieważ ich stare pliki C#/XAML są identyczne.

## Pokrycie

| Obszar | Źródło | Wynik Avalonia |
| --- | --- | --- |
| Profile `users.txt` | `MainWindowLogin` i obecny port | Jednorazowy import jako profile gościa |
| Rejestracja i logowanie | niedokończony prototyp EF/LocalDB | Lokalne konta SQLite z rzeczywistą walidacją hasła |
| Ukończone zadania | `CompleteExerciseService`, niewpięty do UI | Idempotentny zapis ukończenia per profil |
| Kalkulator kwadratowy | `CalcQuadraticFunc`, `QuadraticPage` | Delta, miejsca zerowe, wierzchołek, trzy postacie i kroki obliczeń |
| Tablice matematyczne | `PageF1`–`PageF18`, `WPage`–`W18Page` | 18 artykułów w `Content/formulas.json` |
| Wektory | `pages/chapters/WektoryPage.xaml` | Pełny artykuł i 8 ilustracji |
| Matura poprawkowa 2021 | `Z1Page`–`Z35Page` | 35 zadań, 172 podpowiedzi i 8 obrazów |
| Zadania 1–28 | checkboxy i `correctAnsw` | Cztery opcje, sprawdzanie i zapis po poprawnej odpowiedzi |
| Zadania 29–35 | `ShowAnsBtn`/`ConfirmBtn` | Tryb `revealOnly`; zapis po ujawnieniu odpowiedzi |
| Matury 2019/2020/2021 | pojedyncze ekrany informacyjne | Zachowane jako generyczne placeholdery |
| Kalkulator ogólny | ekran z `TODO` | Zachowany jako placeholder |
| Działy I–III | tekst „Treść ogólna działu…” | Zachowane jako placeholdery |
| E1–E35 | puste szablony widoków | Jedna jawna pozycja „Zestaw E1–E35” zamiast 35 kopii pustego ekranu |

## Korekty treści

Importer zachowuje kolejność tekstu, wzorów i obrazów. W czasie importu poprawiane są znane błędy:

- `/cdot` → `\cdot`,
- `/text` → `\text`,
- `\tg` → `\operatorname{tg}`,
- `\gt` i `\lt` → obsługiwane operatory `>` i `<`,
- stara składnia `\cases` i escapowane nawiasy `\left\[` → składnia obsługiwana przez CSharpMath,
- błędny symbol stopni zapisany jako `^ \cdot`,
- oczywiste literówki, np. „tójkąt”, „będzia” i „funkcji kwadratowe”,
- błędne oznaczenie `q` oraz indeksy we wzorach Viète’a,
- współrzędne punktu `B=(x_1,y_1)` → `B=(x_2,y_2)` w definicji wektora.

## Kryteria kompletności

Testy automatyczne wymagają:

- dokładnie 18 tablic, 4 pozycji działowych i 5 grup placeholderów,
- dokładnie 35 kolejnych zadań: 28 zamkniętych i 7 otwartych,
- czterech niepustych opcji i klucza 1–4 dla każdego zadania zamkniętego,
- niepustej odpowiedzi ujawnianej dla każdego zadania otwartego,
- co najmniej jednej podpowiedzi dla każdego zadania,
- istnienia każdego obrazu wskazanego przez treść,
- braku znanych uszkodzonych komend LaTeX,
- poprawnego parsowania każdego wyrażenia matematycznego przez renderer CSharpMath.

Po spełnieniu tych kryteriów stary WPF może zostać usunięty z aktywnego repozytorium. Historia pozostaje dostępna w Git.
