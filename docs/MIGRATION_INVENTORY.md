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
| Kalkulator ogólny | `Opis struktury systemu.txt`, prototyp i issues #1/#3 | Bezpieczny parser działań, nawiasów, potęg i pierwiastków, Ans, historia sesji, pojedynczy PiP i automatyczny schowek wyniku |
| Tablice matematyczne | historyczne `PageF1`-`PageF18` i `WPage`-`W18Page`; aktualny dokument CKE dla Formuły 2023 | 18 zachowanych artykułów w `Content/formulas.json`, uzupełnionych i zweryfikowanych według wszystkich 17 sekcji CKE |
| Wektory | `pages/chapters/WektoryPage.xaml` | Pełny artykuł i 8 ilustracji |
| Matura poprawkowa 2021 | `Z1Page`–`Z35Page` | 35 zadań, 169 zweryfikowanych podpowiedzi i 9 obrazów |
| Weryfikacja arkusza | CKE `EMAP-P0-100-2108` | Klucz 1–28, treści, odpowiedzi otwarte i strony źródłowe zapisane w danych |
| Matura główna 2025 PP | nowe źródła CKE `MMAP-P0-100-A-2505` i `MMAP-P0-100-2505` | 31 zadań, 35 jednostek postępu, 50 punktów, rozwiązania, kryteria i 9 diagramów wektorowych; nie pochodzi ze starego repozytorium |
| Matura główna 2025 PR | nowe źródła CKE `MMAP-R0-100-A-2505` i `MMAP-R0-100-2505` | 12 zadań, 13 jednostek postępu, 50 punktów, rozwiązania i kryteria; nie pochodzi ze starego repozytorium |
| Matura główna 2026 PP | nowe źródła CKE `MMAP-P0-100-A-2605` i `MMAP-P0-100-2605` | 33 zadania, 37 jednostek postępu, 50 punktów, rozwiązania, kryteria i 7 diagramów wektorowych; nie pochodzi ze starego repozytorium |
| Matura główna 2026 PR | nowe źródła CKE `MMAP-R0-100-A-2605` i `MMAP-R0-100-2605` | 12 zadań, 13 jednostek postępu, 50 punktów, rozwiązania, kryteria i 3 diagramy wektorowe; nie pochodzi ze starego repozytorium |
| Zadania według tematów | `ZadaniaPage.xaml` oraz nowy indeks arkuszy | 17 kategorii agregujących 133 jednostki postępu z pięciu arkuszy 2021, 2025 i 2026 |
| Brudnopis | pola tekstowe ekranów zadań | Wielowierszowe pole robocze przechowywane osobno dla profilu i zadania do zamknięcia aplikacji, z `Ctrl+V` lub `Cmd+V` i menu `Wklej` |
| Zadania 1–28 | checkboxy i `correctAnsw` | Cztery opcje, sprawdzanie i zapis po poprawnej odpowiedzi |
| Zadania 29–35 | `ShowAnsBtn`/`ConfirmBtn` | Tryb `revealOnly`; zapis po ujawnieniu odpowiedzi |
| Matury 2019/2020/2021 | pojedyncze ekrany informacyjne | Zachowane jako placeholdery z kontekstem historycznym i linkiem do roadmapy |
| Planowane kalkulatory | ekrany z `TODO` | Generator wykresów i kalkulator funkcji trygonometrycznych zachowane jako placeholdery |
| Działy issue #35 | `Uzupełnić Treść działów matematyki.txt` i niezależny seed `tools/seeds/issue-35-content.json` | Aksjomatyka i indukcja, alfabet grecki, liczby rzeczywiste i zbiory, algebra, równania i nierówności, wszystkie przypadki delty, ułamki, przybliżenia, potęgi i pierwiastki, przedziały, procenty, logarytmy oraz zadania |
| Kurs Formuły 2023 | podstawa programowa z 2024 r. oraz informatory CKE dla poziomu podstawowego i rozszerzonego | 4 grupy, 13 obszarów, 73 wymagania podstawowe i 46 rozszerzonych, 238 autorskich przykładów oraz 357 autorskich ćwiczeń; historyczne identyfikatory i materiały issue #35 zachowane w lekcjach |
| E1–E35 | puste szablony widoków | Jedna jawna pozycja „Zestaw E1–E35” zamiast 35 kopii pustego ekranu |
| `Window1` / WPF-Math | niepodłączone okno demonstracyjne z eksportem SVG/PNG | Jawna pozycja `superseded` w roadmapie; renderowanie wzorów zastąpił CSharpMath, a nieosiągalnego eksportu nie przeniesiono |
| Dokumenty projektowe | pięć plików `.txt` z katalogu starych wersji | Dokładne kopie w `docs/legacy/originals`, streszczenia w `docs/legacy` i pozycje roadmapy |
| Historyczna licencja | `Projekt-Inzynierski-master/LICENSE` | Dokładna kopia w archiwum wraz z sumą SHA-256; aktywny `LICENSE` pozostaje bez zmian |

## Korekty treści

Importer zachowuje kolejność tekstu, wzorów i obrazów dla treści historycznych innych niż tablice. Katalog tablic jest obecnie kuratorowaną transkrypcją CKE i pełny import kopiuje go z parametru `FormulaCatalogPath`, zamiast ponownie odczytywać stare ekrany WPF. Chroni to poprawioną treść przed cofnięciem przez kolejny import.

W historycznej ścieżce importu poprawiane są znane błędy:

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

- dokładnie 18 tablic i kompletne odwzorowanie wszystkich podpunktów 17 sekcji CKE dla Formuły 2023,
- dokładnie 91 kątów od 0 do 90 stopni w tekstowej tabeli trygonometrycznej,
- dokładnie 4 grupy, 13 obszarów, 73 wymagania podstawowe, 46 dodatkowych wymagań rozszerzonych, 238 przykładów i 357 ćwiczeń kursowych,
- dokładnie 219 ćwiczeń podstawowych i 138 rozszerzonych, po 3 unikalne ćwiczenia i 2 rozwiązane przykłady dla każdego wymagania,
- zachowania dokładnie 35 kolejnych zadań 2021: 28 zamkniętych i 7 otwartych, wraz z identyfikatorami `mp21-*`,
- kontrakty `31/35/50` i `12/13/50` dla matury 2025 oraz `33/37/50` i `12/13/50` dla matury 2026,
- czterech niepustych opcji i klucza 1–4 dla każdego zadania zamkniętego,
- niepustej odpowiedzi ujawnianej dla każdego zadania otwartego,
- co najmniej jednej podpowiedzi dla każdego zadania,
- istnienia każdego diagramu wskazanego przez treść, niepustego opisu alternatywnego i strony dla dziewiętnastu figur matur 2025 i 2026,
- braku znanych uszkodzonych komend LaTeX,
- poprawnego parsowania każdego wyrażenia matematycznego przez renderer CSharpMath.

Bezpośrednia regresja issue #35 wymaga wszystkich wskazanych sekcji, 24 liter alfabetu greckiego, przykładów, wskazówek i odpowiedzi, poprawnego przypadku `\(\Delta{}<0\)` oraz zachowania historycznych identyfikatorów lekcji. Regresja kursu wymaga także rozdzielonych seedów czterech etapów, deterministycznego generatora, filtra poziomu, trzech trybów odpowiedzi, 12 diagramów kursu z opisami alternatywnymi oraz zgodności macierzy `119/238/357`. Testy wymagają ponadto pełnego klucza CKE dla zadań 2021, kontraktów `31/35/50`, dwóch `12/13/50` i `33/37/50` dla matur 2025 i 2026 oraz jednoznacznego przypisania pięciu arkuszy do wspólnych tematów.

Drugi audyt przed usunięciem snapshotów potwierdził `276/276` zgodnych wystąpień zasobów bez braków i różnic. Ujawnił dwa brakujące placeholdery kalkulatorów oraz brak dokładnych kopii dokumentów planistycznych; oba problemy zostały usunięte.

Końcowy audyt z 30 czerwca 2026 r. porównał wizualnie wszystkie 35 zadań, dziewięć ilustracji zadaniowych, pełny klucz 1-28 oraz odpowiedzi 29-35 z arkuszem i zasadami oceniania CKE. Ujawnił i usunął pozostałe błędy terminologiczne w podpowiedziach do zadań 6, 22 i 29, niejednoznaczny zapis kątów w zadaniach 18-19 oraz niepoprawny zapis przedziału w odpowiedzi do zadania 29. Data weryfikacji jest teraz pobierana z metadanych egzaminu.

Ten sam audyt sklasyfikował niepodłączony prototyp `Window1` / WPF-Math jako `superseded`, poprawił odmianę liczby zadań w nawigacji i potwierdził działanie interfejsu przy `1280x820` oraz minimalnym `960x640`. Regresje kursu rozszerzają kontrolę o `720x520`, `960x640` i `1280x820`. Lokalne i zdalne bramy repozytorium wykonują `restore`, `build Release`, pełne testy, format, audyt zależności oraz analizę SonarQube. Proces tagu rozszerza je o natywne pakowanie i smoke test na trzech deklarowanych systemach.

## Celowo nieprzeniesione

- certyfikaty i klucze (`.pfx` oraz pozostałe materiały podpisujące),
- `bin`, `obj`, `packages`, `.vs`, pliki `.csproj.user` i narzędzia skanujące,
- stare aktywne polityki i szablony GitHub, które nie odpowiadają obecnemu repozytorium,
- Lorem Ipsum oraz techniczne szczegóły nawigacji WPF bez wartości dla użytkownika.

Po spełnieniu tych kryteriów stary WPF może zostać usunięty z aktywnego repozytorium. Historia pozostaje dostępna w Git.
