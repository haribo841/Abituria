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
| Matura poprawkowa 2021 | `Z1Page`–`Z35Page` | 35 zadań, 169 zweryfikowanych podpowiedzi i 9 obrazów |
| Weryfikacja arkusza | CKE `EMAP-P0-100-2108` | Klucz 1–28, treści, odpowiedzi otwarte i strony źródłowe zapisane w danych |
| Zadania według tematów | `ZadaniaPage.xaml` | 17 kategorii obejmujących każde z 35 zadań dokładnie raz |
| Brudnopis | pola tekstowe ekranów zadań | Wielowierszowe pole robocze utrzymywane w bieżącej sesji widoku |
| Zadania 1–28 | checkboxy i `correctAnsw` | Cztery opcje, sprawdzanie i zapis po poprawnej odpowiedzi |
| Zadania 29–35 | `ShowAnsBtn`/`ConfirmBtn` | Tryb `revealOnly`; zapis po ujawnieniu odpowiedzi |
| Matury 2019/2020/2021 | pojedyncze ekrany informacyjne | Zachowane jako placeholdery z kontekstem historycznym i linkiem do roadmapy |
| Planowane kalkulatory | ekrany z `TODO` | Kalkulator ogólny, generator wykresów i kalkulator funkcji trygonometrycznych zachowane jako placeholdery |
| Planowane działy | dwie linie starych ekranów | Sześć nazwanych placeholderów: zbiory i logika, równania i nierówności, ciągi, liczby pierwsze, funkcja kwadratowa i logarytmy |
| E1–E35 | puste szablony widoków | Jedna jawna pozycja „Zestaw E1–E35” zamiast 35 kopii pustego ekranu |
| `Window1` / WPF-Math | niepodłączone okno demonstracyjne z eksportem SVG/PNG | Jawna pozycja `superseded` w roadmapie; renderowanie wzorów zastąpił CSharpMath, a nieosiągalnego eksportu nie przeniesiono |
| Dokumenty projektowe | pięć plików `.txt` z katalogu starych wersji | Dokładne kopie w `docs/legacy/originals`, streszczenia w `docs/legacy` i pozycje roadmapy |
| Historyczna licencja | `Projekt-Inzynierski-master/LICENSE` | Dokładna kopia w archiwum wraz z sumą SHA-256; aktywny `LICENSE` pozostaje bez zmian |

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

Po porównaniu z arkuszem i zasadami oceniania CKE naprawiono również odziedziczone błędy semantyczne: odpowiedzi zadania 7, komplet treści zadania 17, podpowiedzi zadania 28 oraz błędne zapisy w kilku rozwiązaniach. Źródła weryfikacji i data kontroli są zapisane w `Content/exam-2021-correction.json`.

## Kryteria kompletności

Testy automatyczne wymagają:

- dokładnie 18 tablic, 7 pozycji działowych, 17 tematów i 7 grup placeholderów,
- dokładnie 35 kolejnych zadań: 28 zamkniętych i 7 otwartych,
- czterech niepustych opcji i klucza 1–4 dla każdego zadania zamkniętego,
- niepustej odpowiedzi ujawnianej dla każdego zadania otwartego,
- co najmniej jednej podpowiedzi dla każdego zadania,
- istnienia każdego obrazu wskazanego przez treść,
- braku znanych uszkodzonych komend LaTeX,
- poprawnego parsowania każdego wyrażenia matematycznego przez renderer CSharpMath.

Testy wymagają także 92 plików zasobów, pełnego klucza CKE dla zadań 1–28 oraz jednoznacznego przypisania wszystkich zadań do tematów.

Drugi audyt przed usunięciem snapshotów potwierdził `276/276` zgodnych wystąpień zasobów bez braków i różnic. Ujawnił dwa brakujące placeholdery kalkulatorów oraz brak dokładnych kopii dokumentów planistycznych; oba problemy zostały usunięte.

Końcowy audyt z 30 czerwca 2026 r. porównał wizualnie wszystkie 35 zadań, dziewięć ilustracji zadaniowych, pełny klucz 1-28 oraz odpowiedzi 29-35 z arkuszem i zasadami oceniania CKE. Ujawnił i usunął pozostałe błędy terminologiczne w podpowiedziach do zadań 6, 22 i 29, niejednoznaczny zapis kątów w zadaniach 18-19 oraz niepoprawny zapis przedziału w odpowiedzi do zadania 29. Data weryfikacji jest teraz pobierana z metadanych egzaminu.

Ten sam audyt sklasyfikował niepodłączony prototyp `Window1` / WPF-Math jako `superseded`, poprawił odmianę liczby zadań w nawigacji i potwierdził działanie interfejsu przy `1280x820` oraz minimalnym `960x640`. Lokalne odpowiedniki kroków CI (`restore`, `build Release`, `test Release`) zakończyły się powodzeniem; zdalne CI może ruszyć dopiero po wykonaniu przez właściciela commita i pusha.

## Celowo nieprzeniesione

- certyfikaty i klucze (`.pfx` oraz pozostałe materiały podpisujące),
- `bin`, `obj`, `packages`, `.vs`, pliki `.csproj.user` i narzędzia skanujące,
- stare aktywne polityki i szablony GitHub, które nie odpowiadają obecnemu repozytorium,
- Lorem Ipsum oraz techniczne szczegóły nawigacji WPF bez wartości dla użytkownika.

Po spełnieniu tych kryteriów stary WPF może zostać usunięty z aktywnego repozytorium. Historia pozostaje dostępna w Git.
