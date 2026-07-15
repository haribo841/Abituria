# Podręcznik użytkownika

Podręcznik dotyczy Abiturii `0.9.0-beta.1`. Aplikacja działa lokalnie i nie wymaga połączenia z internetem do nauki, obliczeń ani zapisu postępu.

## Pierwsze uruchomienie

Po uruchomieniu zobaczysz ekran logowania. Jeżeli baza nie zawiera profilu, aplikacja tworzy profil gościa `Maturzysta`. Możesz od razu wybrać go z listy i kliknąć „Zaloguj”. Profil gościa nie wymaga hasła.

Jeżeli w systemowym katalogu danych aplikacji istnieje historyczny plik `Abituria/users.txt`, zapisane w nim nazwy są jednorazowo importowane jako profile gościa. Import nie usuwa pliku źródłowego i nie tworzy duplikatów przy kolejnych uruchomieniach.

Górna nawigacja po zalogowaniu zawiera:

- **Start** - skróty do głównych funkcji;
- **Wzory** - tablice matematyczne;
- **Zadania** - arkusze i zadania według tematów;
- **Działy** - materiały edukacyjne;
- **Kalkulator** - funkcja kwadratowa i kalkulator ogólny;
- **Plan rozwoju** - funkcje ukończone, zaplanowane i zastąpione;
- **Profil** - postęp i zmiana hasła;
- **O programie** - wersja, commit, licencja, autor i repozytorium.

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

Strona „Wzory” zawiera 18 tablic. Wybierz pozycję z listy, aby otworzyć artykuł z tekstem, wzorami inline i ilustracjami. Wróć do listy przyciskiem widoku lub górną nawigacją.

Wzory są renderowane przez CSharpMath. Jeżeli konkretna formuła jest nieczytelna, zgłoś tytuł tablicy, treść wzoru i rozmiar okna zgodnie z [instrukcją wsparcia](../SUPPORT.md).

## Działy

Dostępnych jest 7 materiałów:

1. Wektory.
2. Liczby naturalne i indukcja.
3. Alfabet grecki.
4. Liczby rzeczywiste, zbiory i logika.
5. Wyrażenia algebraiczne, równania i nierówności.
6. Funkcja kwadratowa i równanie kwadratowe.
7. Logarytmy.

„Ciągi liczbowe” i „Liczby pierwsze” są jawnie oznaczone jako treść w przygotowaniu. Ich widok prowadzi do odpowiedniej pozycji planu rozwoju zamiast udawać gotowy materiał.

## Zadania

Strona „Zadania” pozwala przejść do całego arkusza matury poprawkowej 2021 albo wybrać jeden z 17 tematów. Lista oznacza ukończone zadania znakiem wyboru.

### Zadania 1-28

1. Wybierz odpowiedź A-D.
2. Kliknij „Sprawdź odpowiedź”.
3. Poprawna odpowiedź zapisuje zadanie jako ukończone.
4. Przy błędnej odpowiedzi możesz spróbować ponownie lub odsłaniać podpowiedzi.

### Zadania 29-35

Rozwiązuj zadanie samodzielnie, korzystaj z kolejnych podpowiedzi i kliknij „Pokaż odpowiedź”, gdy chcesz porównać rozwiązanie. Ujawnienie odpowiedzi oznacza zadanie jako ukończone.

### Brudnopis i nawigacja

Brudnopis służy wyłącznie bieżącej pracy. Nie jest zapisywany po opuszczeniu zadania. Przyciski strzałek przechodzą do poprzedniego i następnego zadania w aktualnym kontekście, czyli w całym arkuszu albo w wybranym temacie.

Strona „Profil” pokazuje liczbę ukończonych zadań z 35. Postęp jest lokalny i oddzielny dla każdego profilu.

## Kalkulator funkcji kwadratowej

Wprowadź rzeczywiste współczynniki `a`, `b` i `c` funkcji `f(x) = ax² + bx + c`, a następnie kliknij „Oblicz”. Współczynnik `a` nie może być zerem.

Kalkulator prezentuje wynik i kolejne elementy analizy, w tym wyróżnik, miejsca zerowe, wierzchołek oraz dostępne postacie funkcji. Akceptuje przecinek i kropkę jako separator dziesiętny. Nie zastępuje samodzielnego uzasadnienia rozwiązania zadania.

## Kalkulator ogólny

Kliknij „Kalkulator ogólny” na stronie kalkulatora funkcji kwadratowej. Wyrażenie można wpisać klawiaturą albo zbudować przyciskami ekranowymi.

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

„Wyczyść historię” usuwa także `Ans` i stan powtarzanego `=`. Historia nie jest zapisywana w SQLite i znika po zamknięciu aplikacji.

Kalkulator normalizuje niekanoniczne zera wiodące, na przykład `000001` lub `0000,1`, i informuje o korekcie. Nie dopuszcza nieskończenie długiego ciągu zer z klawiatury ekranowej.

Kontrolowany komunikat zawiera opis i pozycję błędu między innymi dla pustego lub niepełnego wyrażenia, błędnego znaku, nawiasów, dzielenia przez zero, `0^0`, niedozwolonego pierwiastka, wyniku zespolonego, `NaN`, nieskończoności, przekroczenia 512 znaków albo 64 poziomów zagnieżdżenia.

## Dane, kopia i aktualizacja

Profile i postęp są zapisywane w `abituria.db` poza katalogiem programu. Historia kalkulatora i brudnopis nie są trwałe. Instrukcja wykonania kopii, aktualizacji bez utraty danych i odinstalowania znajduje się w [INSTALLATION.md](INSTALLATION.md#dane-użytkownika-i-kopia-zapasowa).

## Pomoc

- problemy z uruchomieniem: [instalacja](INSTALLATION.md#najczęstsze-problemy);
- znany zakres beta: [KNOWN_LIMITATIONS.md](KNOWN_LIMITATIONS.md);
- zgłoszenie błędu: [SUPPORT.md](../SUPPORT.md);
- prywatne zgłoszenie podatności: [SECURITY.md](../SECURITY.md).
