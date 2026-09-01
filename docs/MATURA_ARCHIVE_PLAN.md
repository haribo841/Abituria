# Plan uzupełnienia archiwum matur 2026-2018

## Cel i zakres

Plan porządkuje archiwum oficjalnych arkuszy matematyki CKE od aktualnego roku 2026 do roku 2018 włącznie. Obejmuje wyłącznie:

- termin główny - poziom podstawowy i rozszerzony;
- termin poprawkowy - poziom podstawowy.

Matematyka na poziomie rozszerzonym nie jest przedmiotem obowiązkowym, dlatego nie istnieje osobny arkusz poprawkowy PR. Termin dodatkowy nie jest częścią tego planu.

Arkusze od 2023 r. występują w dwóch formułach. Formuła 2023 jest przeznaczona dla osób zdających według nowej podstawy, natomiast Formuła 2015 pozostaje dostępna dla części osób zdających ponownie. Lata 2018-2022 należą wyłącznie do Formuły 2015. Aplikacja musi zawsze pokazywać formułę przy arkuszu, aby nie mieszać zakresów egzaminacyjnych.

## Legenda stanu

- **na `origin/main`** - arkusz jest opublikowany na gałęzi głównej;
- **lokalnie, bez commita** - komplet zmian znajduje się w bieżącym worktree, ale nie jest jeszcze opublikowany;
- **brak** - arkusz nie jest obecnie zaimplementowany;
- **nie dotyczy** - dana formuła nie obowiązywała w tym roku.

## Stan bieżący

Na `origin/main` znajduje się 7 aktywnych arkuszy: oba poziomy terminów głównych 2024, 2025 i 2026 oraz matura poprawkowa 2021 w Formule 2015. Bieżący worktree zawiera dodatkowo maturę główną 2023 PP, maturę poprawkową 2023 PP, maturę główną 2023 PR, matury poprawkowe 2024 i 2025 PP, matury główne i poprawkową 2021 i 2022 PP oraz PR, a także matury główne i poprawkowe 2020, 2019 i 2018 PP oraz PR w Formule 2015, dlatego lokalnie katalog ma 26 arkuszy. Żaden z dziewiętnastu lokalnie dodanych arkuszy nie jest jeszcze commitem ani częścią `origin/main`.

| Rok | F2023 główna PP | F2023 główna PR | F2023 poprawkowa PP | F2015 główna PP | F2015 główna PR | F2015 poprawkowa PP |
| --- | --- | --- | --- | --- | --- | --- |
| 2026 | na `origin/main` | na `origin/main` | brak | brak | brak | brak |
| 2025 | na `origin/main` | na `origin/main` | lokalnie, bez commita | brak | brak | brak |
| 2024 | na `origin/main` | na `origin/main` | lokalnie, bez commita | brak | brak | brak |
| 2023 | lokalnie, bez commita | lokalnie, bez commita | lokalnie, bez commita | brak | brak | brak |
| 2022 | nie dotyczy | nie dotyczy | nie dotyczy | lokalnie, bez commita | lokalnie, bez commita | lokalnie, bez commita |
| 2021 | nie dotyczy | nie dotyczy | nie dotyczy | lokalnie, bez commita | lokalnie, bez commita | na `origin/main` |
| 2020 | nie dotyczy | nie dotyczy | nie dotyczy | lokalnie, bez commita | lokalnie, bez commita | lokalnie, bez commita |
| 2019 | nie dotyczy | nie dotyczy | nie dotyczy | lokalnie, bez commita | lokalnie, bez commita | lokalnie, bez commita |
| 2018 | nie dotyczy | nie dotyczy | nie dotyczy | lokalnie, bez commita | lokalnie, bez commita | lokalnie, bez commita |

Pełne archiwum z tabeli obejmuje 39 arkuszy: 12 w Formule 2023 i 27 w Formule 2015. Wobec 26 arkuszy dostępnych lokalnie brakuje 13. Węższy zakres, bez równoległych arkuszy Formuły 2015 z lat 2023-2026, obejmuje 27 arkuszy i pozostawia 1 brak.

## Kolejność wdrożenia

### Etap 0 - domknięcie obecnego 2023 PR

1. Zachować istniejący lokalny arkusz `matura-maj-2023-rozszerzona` i jego identyfikatory `mm23-r0-*`.
2. Dokończyć jego pełną bramkę jakości przed łączeniem z kolejnymi arkuszami.
3. Nie zmieniać identyfikatorów ani postępu istniejących matur 2021 oraz 2024-2026.

### Etap 1 - brakująca Formuła 2023

1. Matura maj 2023 PP - wykonana lokalnie jako `matura-maj-2023-podstawowa`, 31 zadań, 34 jednostki postępu i 46 punktów.
2. Matura poprawkowa 2023 PP - wykonana lokalnie jako `matura-poprawkowa-2023-podstawowa`, 33 zadania, 36 jednostek postępu i 46 punktów.
3. Matura poprawkowa 2024 PP - wykonana lokalnie jako `matura-poprawkowa-2024-podstawowa`, 30 zadań, 36 jednostek postępu i 46 punktów.
4. Matura poprawkowa 2025 PP - wykonana lokalnie jako `matura-poprawkowa-2025-podstawowa`, 31 zadań, 36 jednostek postępu i 50 punktów.
5. Matura poprawkowa PP Formuły 2023 z 2026 r. - dopiero po udostępnieniu zasad oceniania.
6. Dla każdego pozostałego arkusza dodać oddzielny plik `Content/exam-<rok>-<session>-<formula>-basic.json`, wpis indeksu oraz macierz pokrycia.

Po tym etapie komplet Formuły 2023 będzie zawierał 12 arkuszy: po PP, PR i poprawce PP dla każdego roku 2023-2026.

### Etap 2 - rdzeń historyczny Formuły 2015

1. Matury główne PP i PR oraz poprawkowe PP z 2018, 2019, 2020 i 2022 r. są wykonane lokalnie.
2. Matury główne PP i PR z 2021 r. są wykonane lokalnie, przy zachowaniu istniejącej poprawki 2021 bez modyfikacji identyfikatorów `mp21-*`.

Etap zapewnia ciągłe archiwum 2018-2022 i nie miesza go z treściami kursu Formuły 2023.

### Etap 3 - równoległe arkusze Formuły 2015 z lat 2023-2026

1. Dodać po PP, PR i poprawce PP Formuły 2015 dla lat 2023, 2024, 2025 i 2026.
2. Pokaż przy każdym arkuszu widoczny znacznik `Formuła 2015` albo `Formuła 2023` w widoku Matura, liście tematów i źródle zadania.
3. Zachować kolejność: rok malejąco, następnie formuła, termin główny PP, termin główny PR, termin poprawkowy PP.

Etap 3 jest potrzebny wyłącznie do pełnego katalogu 36 arkuszy. Nie jest wymagany dla węższego archiwum 24 arkuszy.

## Kontrakt pojedynczego arkusza

Każdy nowy arkusz wymaga przed dodaniem do aplikacji:

1. oficjalnego PDF arkusza CKE i zasad oceniania, z zapisanym URL-em, SHA-256 oraz datą weryfikacji;
2. odrębnego stabilnego `ExamId` i identyfikatorów zadań krótszych niż 80 znaków, niekolidujących z postępem SQLite;
3. zgodnej transkrypcji treści, punktacji, odpowiedzi, rozwiązania, kryteriów i stron źródłowych;
4. przypisania każdego zadania do jednego z istniejących 17 tematów;
5. własnych diagramów wektorowych Avalonia dla każdej figury, z opisem alternatywnym, źródłem i numerem strony - bez aktywnych rastrów;
6. osobnej macierzy pokrycia oraz grupy w `Content/provenance.json`.

Nowe materiały CKE pozostają `blocked`, dopóki właściciel repozytorium nie rozszerzy indywidualnie deklaracji praw do redystrybucji na konkretne arkusze i zasady oceniania. Status `approved` nie może zostać nadany na podstawie samego publicznego adresu PDF ani przejścia testów.

## Wymagane testy i bramy

Każdy etap ma dodać testy kontraktowe sprawdzające liczbę zadań, jednostek postępu, punktów, źródeł, sum SHA-256, kolejności, diagramów i niekolidujących identyfikatorów. Testy UI muszą potwierdzić widoczność formuły, wybór arkusza, losowanie tylko z wybranego arkusza, agregację 17 tematów, oddzielny postęp oraz dostępność przy rozmiarach 720x520, 960x640 i 1280x820.

Przed każdym commitem wymagane są: locked restore, Release build bez ostrzeżeń, wszystkie testy C# i Python z wymaganym pokryciem, formatowanie whitespace i analyzerów, audyt NuGet, walidacja proweniencji, DocFX, kontrola linków oraz `git diff --check`. Zdalny SonarCloud, CodeQL, Build i Pages są weryfikowane dopiero po autoryzowanym pushu. Żaden commit ani push nie jest częścią tego planu.
