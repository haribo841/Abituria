# Podręcznik użytkownika

Podręcznik dotyczy Abiturii `0.9.0-beta.1`. Aplikacja działa lokalnie i nie wymaga połączenia z internetem do nauki, obliczeń ani zapisu postępu.

## Pierwsze uruchomienie

Po uruchomieniu zobaczysz ekran logowania. Jeżeli baza nie zawiera profilu, aplikacja tworzy profil gościa `Maturzysta`. Możesz od razu wybrać go z listy i kliknąć „Zaloguj”. Profil gościa nie wymaga hasła.

Jeżeli w systemowym katalogu danych aplikacji istnieje historyczny plik `Abituria/users.txt`, zapisane w nim nazwy są jednorazowo importowane jako profile gościa. Import nie usuwa pliku źródłowego i nie tworzy duplikatów przy kolejnych uruchomieniach.

Górna nawigacja po zalogowaniu zawiera:

- **Start** - skróty do głównych funkcji;
- **Wzory** - tablice matematyczne;
- **Matura** - wybór pełnej matury głównej 2026 PP albo arkusza poprawkowego 2021 oraz archiwalne placeholdery;
- **Zadania** - zadania z obu aktywnych arkuszy pogrupowane według 17 tematów;
- **Działy** - materiały edukacyjne;
- **Kalkulator** - funkcja kwadratowa i kalkulator ogólny;
- **Opcje** - sposób wyświetlania kalkulatora Picture in Picture;
- **Plan rozwoju** - funkcje ukończone, zaplanowane i zastąpione;
- **Profil** - postęp i zmiana hasła;
- **O programie** - wersja, commit, licencja, autor i repozytorium.

## Okno, motywy i dostępność

Abituria używa własnego paska tytułu inspirowanego historyczną wersją aplikacji. Po lewej znajdują się kolejno:

- `🍓` - zamknięcie aplikacji;
- `🍋` - maksymalizacja albo przywrócenie;
- `🍏` - minimalizacja.

Marka `🍀 Abituria` jest wyśrodkowana, a przycisk motywu znajduje się po prawej. Najechanie na dowolną kontrolkę paska pokazuje opisowy tooltip po krótkim opóźnieniu. Symbol `🍋` pozostaje taki sam po maksymalizacji, natomiast tooltip zmienia się na „Przywróć”.

Przeciągnięcie pustego obszaru paska przenosi okno, a dwuklik maksymalizuje je lub przywraca. Rozmiar zwykłego okna można zmieniać z każdej krawędzi i narożnika. Minimalny obsługiwany rozmiar to `720x520`.

Przycisk motywu przełącza cyklicznie ustawienia:

1. Systemowy.
2. Jasny.
3. Ciemny.
4. Wysoki kontrast.

Ustawienie systemowe śledzi jasny lub ciemny wariant systemu, a po wykryciu systemowej preferencji wysokiego kontrastu włącza kontrastową paletę aplikacji. Zmiana działa od razu, bez zamykania widoku.

Interfejs używa kroju Mulish. Teksty, karty i komunikaty korzystają ze wspólnych zasobów kolorów, a elementy interaktywne mają osobne stany najechania, naciśnięcia i fokusu. Przy obsłudze klawiaturą aktywna kontrolka ma wyraźną kontrastową ramkę.

Układ dostosowuje się do szerokości:

- formularz logowania układa sekcje pionowo poniżej `860`;
- kafle Start przechodzą do jednej kolumny poniżej `780`;
- historia kalkulatora ogólnego przechodzi pod klawiaturę poniżej `900`.

Dialogi kodu odzyskiwania można skalować. Jeżeli treść nie mieści się w oknie, pojawia się pionowe przewijanie. Szczegółowy zakres automatycznych i manualnych kontroli zawiera [audyt WCAG 2.2 A/AA](ACCESSIBILITY_WCAG_AUDIT.md).

## Profile i konta

### Profil gościa

Profil gościa zapisuje ukończone zadania w tej samej lokalnej bazie co konto chronione hasłem. Nie można użyć go do logowania na innym urządzeniu ani odzyskać za pomocą kodu.

### Nowe konto lokalne

W sekcji „Nowe konto” podaj:

- nazwę mającą od 1 do 30 znaków po usunięciu początkowych i końcowych spacji;
- hasło mające od 15 do 128 znaków;
- identyczne powtórzenie hasła.

Nazwa nie może składać się wyłącznie ze spacji. Po udanej rejestracji aplikacja pokazuje jednorazowy kod odzyskiwania. Skopiuj go i przechowuj poza aplikacją. Po zamknięciu okna nie można ponownie wyświetlić tego samego kodu.

Nieudana rejestracja nie blokuje logowania i nie tworzy częściowego konta. Popraw dane lub wybierz istniejący profil.

### Logowanie

Wybierz profil. Dla konta lokalnego wpisz hasło, a następnie kliknij „Zaloguj”. Dla profilu gościa pole hasła nie jest wymagane. Przycisk „Wyloguj” kończy aktywną sesję i wraca do ekranu profili.

### Odzyskiwanie i zmiana hasła

Aby odzyskać konto, wpisz jego nazwę, zapisany kod odzyskiwania i nowe hasło. Po udanej operacji otrzymasz nowy kod, a poprzedni traci ważność.

Zalogowany użytkownik konta chronionego hasłem może zmienić hasło na stronie „Profil”. Wymagane jest bieżące hasło. Po zmianie również generowany jest nowy kod odzyskiwania.

## Wzory

Strona „Wzory” zawiera 18 działów obejmujących kompletny zakres oficjalnych tablic CKE dla Formuły 2023. Historyczny układ 18 pozycji został zachowany, dlatego sekcja CKE „Ciągi” jest rozdzielona między działy „Ciągi” i „Granica ciągu”. Wybierz pozycję z listy, aby otworzyć artykuł z tekstem, wzorami inline, dostępnymi tabelami i ilustracjami. Wróć do listy przyciskiem widoku lub górną nawigacją.

Źródło, data weryfikacji i suma SHA-256 dokumentu CKE są zapisane w danych aplikacji oraz w [macierzy pokrycia tablic](FORMULA_2023_COVERAGE.md).

Wzory są renderowane przez CSharpMath. Jeżeli konkretna formuła jest nieczytelna, zgłoś tytuł tablicy, treść wzoru i rozmiar okna zgodnie z [instrukcją wsparcia](../SUPPORT.md).

## Działy

Sekcja zawiera pełny kurs dla Formuły 2023 według podstawy programowej z 2024 r. Kurs obejmuje cztery grupy, 13 oficjalnych obszarów, 73 wymagania podstawowe i 46 dodatkowych wymagań rozszerzonych. Każde wymaganie ma dwa rozwiązane przykłady oraz trzy ćwiczenia, razem 238 przykładów i 357 ćwiczeń.

Filtr „Podstawowy” jest domyślny. Pokazuje 73 wymagania i 219 ćwiczeń podstawowych. Filtr „Rozszerzony” zachowuje treści podstawowe i dodaje 46 wymagań oraz 138 ćwiczeń części rozszerzonej. Alfabet grecki i lekcja o liczbach pierwszych pozostają widoczne na obu poziomach jako materiały pomocnicze; nie zmieniają urzędowych liczników wymagań ani ćwiczeń.

Nawigacja prowadzi kolejno przez obszar, lekcję i ćwiczenie. Przyciski poprzedniego i następnego zadania pozostają w bieżącej lekcji. Ćwiczenia mają trzy tryby:

- `multipleChoice` - ukończenie po wskazaniu poprawnej odpowiedzi;
- `numeric` - obliczenie sprawdzane bezpiecznym parserem, z przecinkiem lub kropką dziesiętną;
- `revealOnly` - dowód albo odpowiedź symboliczna oznaczana jako ukończona po świadomym ujawnieniu pełnego rozwiązania.

Dokładne wymagania pochodzą z aktu urzędowego. Przykłady, ćwiczenia, rozwiązania i diagramy są autorskie i przypisane Adamowi Kubisiowi. Zadania ani rozwiązania z informatorów CKE nie zostały przepisane. Źródła, sumy SHA-256 i pełną macierz opisuje [pokrycie kursu matematyki](MATH_COURSE_2023_COVERAGE.md).

## Matura i Zadania

Strona „Matura” pokazuje najpierw maturę główną 2026 na poziomie podstawowym, a następnie arkusz poprawkowy 2021. Wybierz arkusz, aby zobaczyć jego pełną listę. Losowanie w tym widoku korzysta wyłącznie z aktualnie wybranego arkusza.

Strona „Zadania” agreguje oba aktywne arkusze według 17 tematów. Każdy wpis na liście pokazuje źródłowy arkusz. Losowanie tematyczne nie wychodzi poza wybrany temat, ale może zwrócić zadanie z 2026 albo 2021. Strzałki poprzedniego i następnego zadania pozostają w puli wynikającej z drogi wejścia, a przycisk powrotu prowadzi odpowiednio do „Matury” lub „Zadań”.

Lista oznacza ukończone zadania znakiem wyboru. Matura 2026 ma 33 oficjalnie numerowane zadania podzielone na 37 osobno ocenianych części i łącznie 50 punktów. Szczegóły źródeł i liczników zawiera [macierz matury 2026](MATURA_2026_COVERAGE.md).

### Tryby odpowiedzi

- W zadaniu pojedynczego wyboru wskaż jedną odpowiedź i kliknij „Sprawdź odpowiedź”.
- W zadaniu liczbowym wpisz liczbę albo proste wyrażenie. Możesz użyć przecinka lub kropki dziesiętnej.
- W zadaniu złożonym uzupełnij wszystkie pola lub wiersze P/F, a następnie kliknij „Sprawdź wszystkie odpowiedzi”. Ukończenie następuje dopiero po poprawnym wypełnieniu całego zestawu.
- W dowodzie albo odpowiedzi symbolicznej rozwiąż zadanie samodzielnie i świadomie ujawnij pełne rozwiązanie wraz z kryteriami punktowania.

### Arkusz poprawkowy 2021: zadania 1-28

1. Wybierz odpowiedź A-D.
2. Kliknij „Sprawdź odpowiedź”.
3. Poprawna odpowiedź zapisuje zadanie jako ukończone.
4. Przy błędnej odpowiedzi możesz spróbować ponownie lub odsłaniać podpowiedzi.

### Arkusz poprawkowy 2021: zadania 29-35

Rozwiązuj zadanie samodzielnie, korzystaj z kolejnych podpowiedzi i kliknij „Pokaż odpowiedź”, gdy chcesz porównać rozwiązanie. Ujawnienie odpowiedzi oznacza zadanie jako ukończone.

### Brudnopis i nawigacja

Brudnopis jest przechowywany osobno dla aktywnego profilu i zadania do czasu zamknięcia aplikacji. Możesz przejść do innego zadania lub modułu i wrócić bez utraty tekstu. Dane nie trafiają do bazy SQLite. `Ctrl+V` na Windows i Linuksie albo `Cmd+V` na macOS oraz prawoklik i polecenie „Wklej” wstawiają tekst w miejscu kursora lub zastępują zaznaczenie.

Przycisk „Otwórz kalkulator PiP” obok nagłówka brudnopisu otwiera kompaktowy kalkulator bez opuszczania zadania. Przyciski strzałek przechodzą do poprzedniego i następnego zadania w aktualnym kontekście, czyli w całym arkuszu albo w wybranym temacie.

Strona „Profil” pokazuje osobno `Matura maj 2026 PP: x/37`, `Matura maj 2026 PR: x/13`, `Matura poprawkowa 2021: x/35`, podstawę `x/219` oraz część rozszerzoną `x/138`. Postęp jest lokalny i oddzielny dla każdego profilu. Nowe liczniki używają istniejącego zapisu identyfikatorów ukończonych zadań, więc aktualizacja nie zmienia schematu bazy i nie usuwa wcześniejszego postępu `mp21-*`.

## Kalkulator funkcji kwadratowej

Wprowadź rzeczywiste współczynniki `a`, `b` i `c` funkcji `f(x) = ax² + bx + c`, a następnie kliknij „Oblicz”. Współczynnik `a` nie może być zerem.

Kalkulator prezentuje wynik i kolejne elementy analizy, w tym wyróżnik, miejsca zerowe, wierzchołek oraz dostępne postacie funkcji. Akceptuje przecinek i kropkę jako separator dziesiętny. Nie zastępuje samodzielnego uzasadnienia rozwiązania zadania.

## Kalkulator ogólny

Kliknij „Kalkulator ogólny” na stronie kalkulatora funkcji kwadratowej. Wyrażenie można wpisać klawiaturą albo zbudować przyciskami ekranowymi.

Na tej samej stronie znajduje się przycisk „Otwórz kalkulator PiP”. Ponowne użycie dowolnego przycisku otwierającego aktywuje istniejący kalkulator zamiast tworzyć duplikat. Strona „Opcje” udostępnia trzy tryby:

1. „Nad Abiturią” - zwykłe przesuwalne okno należące do głównego okna aplikacji.
2. „Zawsze na wierzchu” - okno pozostające także nad innymi aplikacjami.
3. „Panel w aplikacji” - przewijalny panel w prawym dolnym rogu Abiturii.

Zmiana trybu działa od razu i zachowuje wpisane wyrażenie oraz `Ans`.

### Obsługiwana składnia

- działania: `+`, `-`, `*`, `×`, `/`, `:`, `÷`, `^`;
- nawiasy i znaki unarne;
- przecinek albo kropka dziesiętna;
- notacja naukowa, na przykład `1,8E-13`;
- mnożenie niejawne, na przykład `2(3+4)` lub `3√8`;
- `sqrt(x)`, `√x`, `∛x`, `∜x` i `root(stopień; liczba)`;
- `Ans` jako ostatni poprawny wynik.

Potęgowanie jest prawostronne, więc `2^3^2` daje `512`. Potęga ma pierwszeństwo przed minusem unarnym, więc `-2^2` daje `-4`.

Stopień w `root(n; x)` musi być dodatnią liczbą całkowitą co najmniej 2. Ujemna liczba podpierwiastkowa jest dozwolona wyłącznie dla stopnia nieparzystego.

### Przyciski i klawiatura

- `Enter` albo `=` oblicza wyrażenie;
- kolejne `=` powtarza ostatnią zewnętrzną operację binarną z jej prawym argumentem;
- `Escape` albo `C` czyści pole i wynik;
- `⌫` cofa zaznaczenie lub poprzedni znak;
- `Ans` wstawia ostatni poprawny wynik sesji;
- `1/x` oblicza odwrotność zaznaczonego wyrażenia, bieżącego wyrażenia lub poprzedniego wyniku;
- `x²` podnosi wybrane wyrażenie albo poprzedni wynik do kwadratu;
- `√`, `∛` i `ⁿ√` wstawiają gotowe szablony albo działają na poprzednim wyniku.

Po obliczeniu `2+3` kolejne naciśnięcia `=` dają `8`, `11` i tak dalej. Dla wyrażenia bez operacji binarnej, na przykład `√16`, kolejne `=` pozostawia wynik `4`.

### Historia i błędy

Historia przechowuje maksymalnie 20 poprawnych obliczeń, najnowsze na początku. Kliknięcie wpisu odtwarza wyrażenie wraz z historyczną wartością `Ans`, dlatego złożone obliczenie powinno dać ten sam wynik. Błędy nie zmieniają `Ans`, nie niszczą operacji powtarzanego `=` i nie trafiają do historii.

Każdy poprawny wynik pełnego kalkulatora lub PiP jest automatycznie kopiowany do schowka dokładnie w postaci pokazanej na ekranie. Dotyczy to zwykłego obliczenia, kolejnego `=`, pierwiastka, odwrotności, kwadratu i odtworzenia historii. Błąd oraz wyczyszczenie kalkulatora nie zmieniają schowka. Jeżeli system odmówi dostępu, obliczenie pozostaje poprawne, a kalkulator pokazuje ostrzeżenie.

„Wyczyść historię” usuwa także `Ans` i stan powtarzanego `=`. Historia nie jest zapisywana w SQLite i znika po zamknięciu aplikacji.

Kalkulator normalizuje niekanoniczne zera wiodące, na przykład `000001` lub `0000,1`, i informuje o korekcie. Nie dopuszcza nieskończenie długiego ciągu zer z klawiatury ekranowej.

Kontrolowany komunikat zawiera opis i pozycję błędu między innymi dla pustego lub niepełnego wyrażenia, błędnego znaku, nawiasów, dzielenia przez zero, `0^0`, niedozwolonego pierwiastka, wyniku zespolonego, `NaN`, nieskończoności, przekroczenia 512 znaków albo 64 poziomów zagnieżdżenia.

## Dane, kopia i aktualizacja

Profile, postęp i wybrany tryb PiP są zapisywane w `abituria.db` poza katalogiem programu. Historia kalkulatora i brudnopis istnieją tylko do zamknięcia aplikacji. Instrukcja wykonania kopii, aktualizacji bez utraty danych i odinstalowania znajduje się w [INSTALLATION.md](INSTALLATION.md#dane-użytkownika-i-kopia-zapasowa).

## Pomoc

- problemy z uruchomieniem: [instalacja](INSTALLATION.md#najczęstsze-problemy);
- znany zakres beta: [KNOWN_LIMITATIONS.md](KNOWN_LIMITATIONS.md);
- zgłoszenie błędu: [SUPPORT.md](../SUPPORT.md);
- prywatne zgłoszenie podatności: [SECURITY.md](../SECURITY.md).
