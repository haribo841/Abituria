# Analiza biznesowa projektu Abituria

Status: aktywny dokument biznesowy dla bieżącej implementacji. Stan na 28 lipca 2026 r.

Ten dokument zastępuje historyczną checklistę `docs/legacy/analiza-biznesowa.md` jako podstawa oceny uzasadnienia, zakresu, ryzyk i warunków wydania. Nie zastępuje [wymagań projektowych](REQUIREMENTS.md), [architektury](ARCHITECTURE.md), [roadmapy](ROADMAP.md) ani [procesu wydania](RELEASE_PROCESS.md), lecz łączy ich perspektywę produktową.

## 1. Cel analizy

Celem analizy jest uzasadnienie realizacji Abiturii, określenie potrzeb użytkowników, wartości produktu, zakresu, sposobu udostępniania oraz mierzalnych warunków zakończenia projektu.

Abituria jest desktopową aplikacją edukacyjną do samodzielnej nauki matematyki na poziomie szkoły średniej i przygotowania do matury. Łączy materiały teoretyczne, przykłady, zadania, podpowiedzi oraz kalkulatory, a podstawowe funkcje działają lokalnie bez wymaganej usługi sieciowej.

Analiza opiera się na aktualnej implementacji, wymaganiach, testach, dokumentacji użytkownika i zachowanych dokumentach legacy. Nie przedstawia nieudokumentowanych badań ankietowych ani nie rekonstruuje historycznych terminów projektu.

## 2. Cele produktu i perspektywa klienta

Projekt nie ma potwierdzonego odrębnego klienta komercyjnego ani płatnego zamawiającego. W tej analizie odbiorcą zakresu jest opiekun projektu i osoba oceniająca produkt, a głównym odbiorcą wartości jest uczeń. Autorem i opiekunem aktualnej implementacji jest Adam Kubiś, zgodnie z [AUTHORS.md](../AUTHORS.md).

Cele produktu są następujące:

- ułatwić samodzielną, uporządkowaną naukę matematyki;
- połączyć teorię, przykłady, ćwiczenia, wskazówki i odpowiedzi w jednej aplikacji;
- umożliwić użycie kalkulatorów bez operowania na niebezpiecznie dzielonych fragmentach tekstu;
- zapewnić lokalne profile i zapis postępu bez obowiązkowego konta internetowego;
- utrzymać kod, treści, dokumentację i historię zmian w audytowalnym repozytorium;
- umożliwić dalszą rozbudowę bez ukrywania funkcji jeszcze nieukończonych.

Miernikami osiągnięcia celu nie są deklaracje funkcji, lecz wymagania F/NF, przypadki użycia, testy regresyjne, bramy CI oraz kryteria akceptacji opisane w [REQUIREMENTS.md](REQUIREMENTS.md).

## 3. Użytkownicy, potrzeby i interesariusze

### 3.1. Użytkownicy końcowi

Głównym użytkownikiem jest uczeń szkoły średniej, w szczególności maturzysta. Aplikacja obsługuje także użytkownika lokalnego, który chce zachować postęp, oraz profil gościa, który chce rozpocząć naukę bez hasła.

Najważniejsze potrzeby użytkownika są priorytetyzowane metodą MoSCoW w sekcji 9:

- szybkie dotarcie do działu, tablicy, zadania lub kalkulatora;
- czytelne objaśnienia i przykłady wraz z poprawnym renderowaniem wzorów;
- informacja o poprawności odpowiedzi oraz stopniowane podpowiedzi;
- kontrolowane komunikaty błędów zamiast awarii aplikacji;
- możliwość użycia podstawowych funkcji offline i bez opłaty;
- lokalny zapis postępu bez wysyłania danych do usługi chmurowej.

Produkt ogranicza rozproszenie materiałów, utrudnienie rozpoczęcia zadania, zbyt szybkie ujawnianie pełnej odpowiedzi oraz konieczność korzystania z wielu niezależnych narzędzi.

### 3.2. Interesariusze

| Interesariusz | Potrzeba lub oczekiwanie | Dowód albo kanał weryfikacji |
| --- | --- | --- |
| Uczeń | teoria, zadania, podpowiedzi, kalkulatory i czytelny interfejs | przypadki użycia UC-01 do UC-11, testy headless UI |
| Użytkownik profilu gościa | natychmiastowy dostęp bez hasła | F-01, `AccountServiceTests` |
| Użytkownik lokalnego konta | zapis postępu i ochrona hasła | F-02 do F-05, `AccountServiceTests` |
| Opiekun projektu lub osoba oceniająca | zgodność z zakresem, dokumentacją i kryteriami akceptacji | `REQUIREMENTS.md`, CI, historia repozytorium |
| Autor i opiekun implementacji | utrzymywalna architektura, testy, bezpieczne wydania | `ARCHITECTURE.md`, workflow, `AUTHORS.md` |
| Autor treści | możliwość aktualizacji treści bez mieszania ich z kodem | JSON w `Content`, `CONTENT_AUTHORING.md` |
| Użytkownik repozytorium | dostęp do kodu, instrukcji, licencji i kanału zgłoszeń | README, GitHub Pages, `SUPPORT.md`, `SECURITY.md` |

## 4. Model udostępniania i wartość produktu

### 4.1. Model biznesowy

Abituria jest projektem open source z kodem na licencji MIT. Wariant beta nie zakłada przychodów, płatności, reklam, obowiązkowego logowania ani płatnej infrastruktury serwerowej. Repozytorium, dokumentacja GitHub Pages i przyszłe paczki portable są publicznym kanałem udostępniania.

Potencjalne przyszłe źródła finansowania, takie jak granty edukacyjne, dobrowolne wsparcie, współpraca ze szkołami lub dodatkowe materiały, nie należą do bieżącego zakresu. Nie stanowią obietnicy produktu ani podstawy do zmiany licencji.

### 4.2. Wartość dla użytkownika

Użytkownik otrzymuje bezpłatny dostęp do lokalnej aplikacji, połączenie materiałów z ćwiczeniami, sprawdzanie odpowiedzi, podpowiedzi i narzędzia obliczeniowe. Może też zgłaszać błędy lub propozycje rozwoju przez kanały opisane w [SUPPORT.md](../SUPPORT.md).

### 4.3. Stan dystrybucji

Proces techniczny tworzenia przenośnych paczek x64 dla Windows, Ubuntu i macOS jest zaimplementowany, a publiczne prerelease [`v0.9.0-beta.1`](https://github.com/haribo841/Abituria/releases/tag/v0.9.0-beta.1) stanowi kanał dystrybucji zatwierdzonej bety. Deklaracje z 3 i 5 sierpnia 2026 r. obejmują arkusze 2025 i 2026 na obu poziomach, zasady oceniania oraz dziewiętnaście autorskich implementacji wektorowych Avalonia. Później dodana transkrypcja 97 przykładów z informatorów CKE nie jest jeszcze objęta deklaracją, dlatego bieżąca brama `releaseEligible` w `Content/provenance.json` ma wartość `false` i blokuje następne wydanie. Szczegóły zawierają [znane ograniczenia](KNOWN_LIMITATIONS.md), [inwentarz pochodzenia](CONTENT_PROVENANCE.md), cztery macierze matur 2025 i 2026 oraz [oświadczenie o prawach](ASSET_RIGHTS_DECLARATION.md).

Koszty bieżącego wariantu obejmują przede wszystkim czas przygotowania treści, implementacji, testów, dokumentacji i utrzymania repozytorium. Aplikacja działa lokalnie, dlatego nie wymaga serwera aplikacyjnego ani przechowywania danych użytkowników przez zewnętrznego operatora.

## 5. Zakres produktu

### 5.1. Zakres bieżącej implementacji

Zakres bieżącej implementacji jest zdefiniowany normatywnie w [REQUIREMENTS.md](REQUIREMENTS.md). Obejmuje między innymi:

- profile gościa i lokalne konta z zapisem postępu w SQLite;
- tablice matematyczne oraz pełny kurs Formuły 2023 obejmujący 4 grupy, 13 obszarów, 119 wymagań, 238 przykładów i 357 ćwiczeń;
- sprawdzanie odpowiedzi zamkniętych, ujawnianie odpowiedzi otwartych oraz podpowiedzi;
- matury główne 2025 i 2026 na poziomie podstawowym i rozszerzonym oraz zachowany arkusz poprawkowy 2021 - razem 5 arkuszy i 133 jednostki postępu;
- odpowiedzi złożone dla tabel P/F i zadań wielopolowych oraz agregacja pięciu arkuszy według 17 tematów;
- kalkulator funkcji kwadratowej;
- kalkulator ogólny z parserem wyrażeń, nawiasami, potęgami, pierwiastkami, notacją naukową, `Ans`, historią i powtarzanym `=`;
- losowanie zadań z całego arkusza albo wybranego tematu;
- dokumentację, bramy jakości, diagnostyczny smoke test artefaktu i automatyzację przygotowania paczek.

Kurs odpowiada podstawie programowej z 2024 r. stosowanej dla matury 2026. Filtr podstawowy pokazuje 73 wymagania i 219 ćwiczeń, a rozszerzony dodaje 46 wymagań oraz 138 ćwiczeń. Katalog egzaminów obejmuje 35 i 13 jednostek postępu matury 2025, 37 i 13 jednostek matury 2026 oraz 35 zadań matury poprawkowej 2021. Transkrypcje i autorskie implementacje wektorowe Avalonia mają zatwierdzone przypisania w manifeście pochodzenia.

### 5.2. Poza zakresem beta

Poza zakresem bieżącej bety pozostają między innymi:

- materiały wideo, generator wykresów i pełny kalkulator trygonometryczny;
- synchronizacja między urządzeniami, konto internetowe, chmura i integracja z systemami szkolnymi;
- wersja webowa, mobilna i pakiet dla Apple Silicon;
- natywne instalatory, automatyczne aktualizacje oraz podpisywanie kodu;
- płatności, panel administracyjny i automatyczne pobieranie zadań z internetu.

Pełne rozróżnienie między funkcjami przeniesionymi, planowanymi i zastąpionymi zawiera [ROADMAP.md](ROADMAP.md).

## 6. Harmonogram i kamienie milowe

Historyczne daty realizacji nie są kompletne. Poniższy harmonogram jest referencyjnym planem 14-tygodniowym przekazanym dla projektu, zestawionym z aktualnym stanem. Publiczna obrona odbyła się 17 stycznia 2022 r. i zakończyła pozytywną decyzją komisji oraz wynikiem bardzo dobrym. Odrębny zapis retrospektywny potwierdza późniejsze zatwierdzenie projektu przez prowadzącego na początku lutego 2022 r., bez zachowanego dokładnego dnia.

Rzeczywiste daty najwcześniejszych weryfikowalnych stanów technicznych I-IV bieżącej migracji zostały odtworzone z Git w [rejestrze odbioru Issue #43](acceptance/README.md). Są to daty kamieni technicznych z 2026 r., odrębne od historycznego zatwierdzenia projektu przez prowadzącego na początku lutego 2022 r.

| Okres referencyjny | Kamień milowy | Odpowiedzialność | Aktualny stan i dowód |
| --- | --- | --- | --- |
| Tydzień 1-2 | M1 - zatwierdzenie analizy i wymagań | opiekun projektu, autor implementacji | zrealizowane w dokumentacji: analiza biznesowa, wymagania, kryteria akceptacji |
| Tydzień 3 | M2 - architektura i nawigacja | autor implementacji | zrealizowane: AvaloniaUI, jedno `MainWindow`, `AppViewModel`, testy architektury |
| Tydzień 4-5 | M3a - ekran startowy i nawigacja | autor implementacji | zrealizowane: shell, widoki i routing |
| Tydzień 6-7 | M3b - kalkulatory | autor implementacji | zrealizowane: kalkulator ogólny i funkcji kwadratowej z regresjami |
| Tydzień 8-11 | M4 - moduł edukacyjny | autor implementacji, autor treści | zrealizowane: pełny kurs `119/238/357`, pięć aktywnych arkuszy ze 133 jednostkami postępu, tablice i oddzielny postęp |
| Tydzień 12 | M5a - testy i regresje | autor implementacji | zrealizowane ciągle: testy jednostkowe, integracyjne, UI i CI |
| Tydzień 13 | M5b - dokumentacja i pakowanie | autor implementacji | technicznie zrealizowane: dokumentacja, workflow, paczki portable i smoke test |
| Tydzień 14 | M6 - publiczne prerelease | autor implementacji, właściciel praw | osiągnięte: zatwierdzona proweniencja, tag, natywne workflow i publiczny GitHub Release `v0.9.0-beta.1` |
| 17 stycznia 2022 r. | M7 - publiczna obrona projektu historycznego | Adam Kubiś i komisja | osiągnięte: pokaz działającej aplikacji, pytania i odpowiedzi, decyzja pozytywna, wynik bardzo dobry; dowód w `DEFENSE_PROTOCOL.md` |

M6 w tej tabeli dotyczy prerelease bieżącej migracji `0.9.0-beta.1`, natomiast M7 dokumentuje rzeczywistą obronę historycznej wersji projektu w 2022 r. Są to odrębne zdarzenia i dowody.

Przed ustaleniem daty M6 należy potwierdzić wynik wszystkich bram dla dokładnego commita. Prawa do materiałów CKE, odziedziczonych grafik matematycznych oraz grafik aplikacji zostały odnotowane na podstawie oświadczenia właściciela z 19 lipca 2026 r.; nie zastępuje ono technicznego workflow publikacji.

## 7. Kryteria akceptacji

Kryteria dzielą się na odbiór produktu i odbiór publicznego wydania:

| Obszar | Kryterium | Dowód |
| --- | --- | --- |
| Funkcje edukacyjne | użytkownik może korzystać z dostępnych materiałów, zadań, odpowiedzi i podpowiedzi | F-06 do F-13, UC-04 do UC-06 |
| Kalkulatory | zadeklarowane działania działają, a błędne dane są kontrolowane | F-14 do F-16, regresje kalkulatora |
| Konta i postęp | działa profil gościa, rejestracja, logowanie, odzyskiwanie hasła i zapis postępu | F-01 do F-05, testy kont |
| Jakość | build, testy, format, audyt zależności i analiza jakości przechodzą dla aktualnego commita | kryteria 1-6 w `REQUIREMENTS.md` |
| Artefakt | paczka przechodzi archiwizację i diagnostyczny smoke test na natywnych runnerach | kryteria 15-18 w `REQUIREMENTS.md` |
| Prawo do publikacji | każdy paczkowany zasób ma udokumentowane prawo do redystrybucji | kryterium 19 w `REQUIREMENTS.md` |
| Publiczne wydanie | istnieją tag, prerelease, działająca strona Pages i kontrola po publikacji | `RELEASE_PROCESS.md` |
| Historyczna ocena projektu | siedem obszarów oceny i dziesięć warunków akceptacji jest odwzorowanych bez mieszania wersji WPF z migracją AvaloniaUI | `EVALUATION_PROTOCOL.md` |

Spełnienie kryteriów technicznych nie zastępuje kryterium prawa do publicznej dystrybucji. Aktualny manifest zawiera grupę `cke-formula-2023-guide-examples` o statusie `blocked` i ma `releaseEligible=false`. Publiczne wydanie jest możliwe dopiero po rozszerzeniu deklaracji właściciela, zaliczeniu twardej bramy proweniencji oraz pozostałych bramek i zweryfikowaniu rzeczywistego GitHub Release.

Historyczny projekt został zaakceptowany i uzyskał wynik bardzo dobry. Zakres oraz ograniczenia tego dowodu opisuje `EVALUATION_PROTOCOL.md`. Wynik z 2022 r. nie jest oceną bieżącej migracji i nie zastępuje jej przyszłego procesu wydawniczego.

## 8. Model licencyjny, dane i prywatność

Kod Abiturii jest objęty licencją MIT. Licencje zależności są prowadzone osobno w [DEPENDENCIES.md](DEPENDENCIES.md) i [THIRD-PARTY-NOTICES.md](../THIRD-PARTY-NOTICES.md). Licencja kodu nie przenosi praw do materiałów edukacyjnych, obrazów ani fontów, dlatego ich pochodzenie jest weryfikowane niezależnie.

Aplikacja nie wymaga konta online. Profile, postęp i lokalna baza SQLite pozostają na komputerze użytkownika. Hasła i kody odzyskiwania są przechowywane jako skróty, natomiast cała baza nie jest szyfrowana. Szczegóły ograniczeń prywatności opisano w [KNOWN_LIMITATIONS.md](KNOWN_LIMITATIONS.md).

## 9. Metodyka wymagań i kontrola zmian

Projekt używa lekkiej metodyki łączącej:

- wymagania funkcjonalne F i niefunkcjonalne NF z priorytetem, statusem i dowodem implementacji;
- przypadki użycia UC;
- priorytetyzację MoSCoW;
- kryteria akceptacji i bezpośrednie regresje;
- macierz śledzenia od wymagania do kodu, testu albo dokumentu.

Wymagania pochodzą z dokumentów projektu, zachowanego kontekstu legacy, przeglądu bieżącej implementacji i zgłoszeń problemów. Nie należy przedstawiać ich jako wyniku badań użytkowników, których nie udokumentowano.

Zmiana zakresu wymaga następującego procesu:

1. Opis potrzeby, wpływu na użytkownika i kryterium akceptacji w issue lub udokumentowanej decyzji.
2. Aktualizacja `REQUIREMENTS.md`, tej analizy, roadmapy albo ograniczeń, jeśli zmienia się ich treść.
3. Implementacja wraz z bezpośrednimi testami regresyjnymi.
4. Ocena wpływu na treści, prywatność, licencje, pochodzenie zasobów i proces wydania.
5. Weryfikacja bram CI przed połączeniem zmiany z `main`.

### Priorytety MoSCoW

| Priorytet | Zakres |
| --- | --- |
| Must have | nawigacja, dostępne materiały, zadania, sprawdzanie odpowiedzi, podpowiedzi, kalkulatory, kontrola błędów i dokumentacja |
| Should have | lokalne konto, zapis postępu, historia kalkulatora, ekran „O programie” i diagnostyka wydania |
| Could have | dodatkowe arkusze, materiały wideo, wyszukiwanie zapisu matematycznego |
| Won't have w becie | mobilny i webowy klient, płatności, rozbudowany backend, synchronizacja urządzeń i automatyczne aktualizacje |

## 10. Architektura i ograniczenia technologiczne

Aktywna implementacja używa C#, .NET 10 LTS, AvaloniaUI 12 i SQLite z Entity Framework Core. Interfejs korzysta z jednego `MainWindow` oraz nawigacji między widokami, a nie z historycznego WPF i wielu niezależnych okien. Treści są przechowywane jako zasoby JSON, a wzory renderuje CSharpMath dla Avalonia.

Szczegóły odpowiedzialności warstw, modułów, danych i automatyzacji zawiera [ARCHITECTURE.md](ARCHITECTURE.md). Dokument techniczny jest źródłem prawdy dla struktury kodu, natomiast ta analiza opisuje uzasadnienie decyzji i ich wartość dla interesariuszy.

## 11. Ryzyka biznesowe i projektowe

| Ryzyko | Skutek | Działanie ograniczające |
| --- | --- | --- |
| Utrata lub nieaktualność dowodu praw do zasobów | ryzyko zakwestionowania dystrybucji | wersjonowane oświadczenie właściciela, niezmieniane przypisanie źródeł i obowiązkowa brama inwentarza przed wydaniem |
| Zbyt szeroki zakres treści | opóźnienie albo niepełna jakość | maszynowa macierz `119/238/357`, podzielone seedy, deterministyczny generator i testy kompletności |
| Błąd merytoryczny | utrata wartości edukacyjnej | treści, przykłady i zadania są objęte inwentarzem oraz regresjami |
| Regresja kalkulatora lub nawigacji | błędne wyniki albo trudność użycia | parser niezależny od UI, testy regresyjne i architektura jednego okna |
| Brak testów wydania na platformie docelowej | nieuruchamialna paczka | natywne workflow Windows, Ubuntu i macOS oraz smoke test finalnego archiwum |
| Brak zasobów utrzymaniowych | spowolnienie rozwoju | ograniczony zakres beta, dokumentacja i transparentne zgłoszenia GitHub |

## 12. Śledzenie Issue #9

Bieżącym zgłoszeniem analizy biznesowej jest [haribo841/Abituria#9](https://github.com/haribo841/Abituria/issues/9). Historyczny kontekst pozostaje wyłącznie pod pełnym adresem [Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski#38](https://github.com/Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski/issues/38). W starym repozytorium nie są wykonywane żadne zmiany.

| Wymaganie analizy | Aktywny dowód |
| --- | --- |
| cele klienta, potrzeby użytkownika i model biznesowy | sekcje 2-4 tego dokumentu |
| zakres, harmonogram, kamienie milowe i kryteria akceptacji | sekcje 5-7 oraz `REQUIREMENTS.md` |
| model licencyjny | sekcja 8, `LICENSE`, `CONTENT_PROVENANCE.md` |
| zebranie wymagań według metodyki | sekcja 9 i macierz wymagań |
| architektura oraz technologie | sekcja 10 i `ARCHITECTURE.md` |

Zamknięcie Issue #9 nie oznacza automatycznie, że spełniono warunek publicznego wydania. Status publikacji jest oceniany wyłącznie przez wymagania, pochodzenie zasobów i procedurę wydawniczą aktualnego repozytorium.
