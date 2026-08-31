# Abituria

Abituria to działająca offline aplikacja desktopowa wspierająca naukę matematyki na poziomie szkoły średniej. Aktualna implementacja używa C#, .NET 10 LTS i AvaloniaUI 12. Dane profili oraz postęp są przechowywane lokalnie w SQLite.

Bieżące publiczne wydanie beta: [`0.9.2`](https://github.com/haribo841/Abituria/releases/tag/v0.9.2).

> [!IMPORTANT]
> Bieżący worktree ma `releaseEligible=false`. Dodane adaptacje matur głównych 2022 PP i PR oraz poprawkowej 2022 PP, transkrypcje matury głównej i poprawkowej 2023 PP oraz matur poprawkowych 2024 i 2025 PP wraz z trzydziestoma sześcioma nowymi diagramami pozostają zablokowane do czasu osobistego rozszerzenia deklaracji praw. Opublikowane `0.9.2` zachowuje wcześniejszy, zatwierdzony zakres i nie obejmuje tych lokalnych zmian.

## Pobieranie

Zweryfikowane paczki są dostępne w [GitHub Releases](https://github.com/haribo841/Abituria/releases/tag/v0.9.2):

| System | Paczka | Deklarowane środowisko beta |
| --- | --- | --- |
| Windows | `Abituria-v0.9.2-win-x64.exe` | Windows 11 24H2 x64 |
| Ubuntu | `Abituria-v0.9.2-linux-x64.tar.gz` | Ubuntu 24.04 x64 |
| macOS | `Abituria-v0.9.2-osx-x64.zip` | macOS 15 na komputerze Intel |

Artefakty są samowystarczalne i nie wymagają instalacji środowiska .NET. Windows otrzymuje pojedynczy plik EXE, natomiast Ubuntu i macOS pozostają wydaniami portable w archiwach. Nie ma instalatora, automatycznej aktualizacji, podpisu kodu ani AOT. Dla Windows dodatkowe archiwum ZIP zachowuje pełne dowody licencyjne i SBOM. Przed uruchomieniem należy sprawdzić sumę SHA-256 oraz attestation artefaktu.

- [Instrukcja instalacji, aktualizacji i odinstalowania](docs/INSTALLATION.md)
- [Dokumentacja online](https://haribo841.github.io/Abituria/)
- [Znane ograniczenia](docs/KNOWN_LIMITATIONS.md)

## Funkcje

- lokalne konta chronione hasłem oraz profile gościa;
- jednorazowe kody odzyskiwania i postęp zapisywany osobno dla każdego profilu;
- 18 tablic matematycznych zgodnych zakresem z oficjalnym dokumentem CKE dla Formuły 2023;
- pełny kurs Formuły 2023: 4 grupy, 13 obszarów, 119 wymagań, 238 rozwiązanych przykładów i 357 ćwiczeń;
- filtr poziomu podstawowego i rozszerzonego oraz tryby odpowiedzi: wybór, wynik liczbowy, odpowiedź złożona i ujawnienie rozwiązania;
- matury główną i poprawkową 2023 na poziomie podstawowym, maturę główną 2023 na poziomie rozszerzonym, matury główne 2024, 2025 i 2026 na poziomie podstawowym i rozszerzonym, matury poprawkowe 2022, 2024 i 2025 PP, matury główne 2021 i 2022 PP i PR w Formule 2015 oraz zachowane 35 zadań matury poprawkowej 2021 - łącznie 17 arkuszy i 473 jednostki postępu;
- wybór arkusza, losowanie w obrębie wybranego arkusza i agregacja siedemnastu arkuszy według 17 tematów;
- sprawdzanie odpowiedzi A-D, podpowiedzi, ujawnianie odpowiedzi i brudnopis przechowywany osobno dla profilu i zadania do zamknięcia aplikacji;
- kalkulator ogólny z nawiasami, potęgami, pierwiastkami, notacją naukową, `Ans`, historią, powtarzaniem `=` i automatycznym kopiowaniem wyniku;
- pojedynczy kalkulator Picture in Picture w oknie nad Abiturią, oknie zawsze na wierzchu albo panelu aplikacji;
- kalkulator funkcji kwadratowej z postacią ogólną, kanoniczną i iloczynową;
- główne okno aplikacji z nawigacją między materiałami, zadaniami, kalkulatorami, opcjami, profilem i planem rozwoju;
- własny pasek tytułu z pełnym sterowaniem oknem oraz skalowalne układy dla szerokości od `720` pikseli;
- motyw systemowy, jasny, ciemny i wysokiego kontrastu, widoczny fokus oraz jawne stany interakcji kontrolek;
- ekran „O programie” z wersją, identyfikatorem commita, licencją, autorem i adresem repozytorium.

Szczegółową instrukcję korzystania z tych funkcji zawiera [podręcznik użytkownika](docs/USER_GUIDE.md).

## Szybki start z paczki

1. Pobierz artefakt przeznaczony dla swojego systemu oraz `SHA256SUMS.txt` z tego samego wydania.
2. Zweryfikuj sumę i attestation zgodnie z [instrukcją instalacji](docs/INSTALLATION.md#sprawdzenie-integralności-i-pochodzenia).
3. Na Windows zapisz pojedynczy plik EXE w nowym katalogu. Na Ubuntu i macOS rozpakuj aplikację do nowego katalogu i wejdź do utworzonego podkatalogu `Abituria-v0.9.2-<rid>`.
4. Uruchom plik EXE Windows, `Abituria` albo `Abituria.app`, zależnie od systemu.
5. Wybierz profil gościa lub utwórz lokalne konto.

Wydanie beta jest niepodpisane. SmartScreen lub Gatekeeper może wyświetlić ostrzeżenie. Dokumentacja opisuje bezpieczną obsługę komunikatu dla konkretnej, zweryfikowanej paczki i nie zaleca globalnego wyłączania zabezpieczeń systemu.

## Dane lokalne

Baza `abituria.db` znajduje się poza katalogiem programu, w systemowym katalogu danych lokalnych użytkownika, w podkatalogu `Abituria`. Dzięki temu zastąpienie katalogu aplikacji nowszą wersją nie usuwa kont ani postępu. Przed aktualizacją zalecane jest wykonanie kopii bazy.

Hasła nie są przechowywane jawnie. Aplikacja używa PBKDF2-HMAC-SHA256, osobnej soli dla każdego konta i wersjonowanej liczby iteracji. Kod odzyskiwania jest wyświetlany tylko raz, a w bazie pozostaje jego skrót.

Przy pierwszym uruchomieniu aplikacja może zaimportować istniejące nazwy z pliku `Abituria/users.txt` w systemowym katalogu danych aplikacji jako profile gościa. Plik źródłowy nie jest usuwany, a import jest idempotentny. Jeżeli nie istnieje żaden profil, tworzony jest gość `Maturzysta`.

## Uruchomienie ze źródeł

Wymagany jest .NET SDK `10.0.302`, przypięty w `global.json`. Na Windows testy integracyjne skryptów wydawniczych działają z wbudowanym PowerShell 5.1 lub PowerShell 7 (`pwsh`); na macOS i Linux wymagany jest `pwsh` w `PATH`.

```powershell
dotnet restore Abituria.sln --configfile NuGet.Config --locked-mode
dotnet build Abituria.sln --configuration Release --no-restore
dotnet test Abituria.sln --configuration Release --no-build
dotnet run --project Abituria.csproj
```

Podstawowe kontrole developerskie:

```powershell
dotnet list Abituria.sln package --vulnerable --include-transitive
dotnet format whitespace Abituria.sln --verify-no-changes --no-restore
git diff --check
```

Pełny zestaw bram, obejmujący wymuszony audyt podatności, pochodzenie zasobów, aktualność dokumentacji zależności, DocFX i odnośniki, opisuje [proces wydania](docs/RELEASE_PROCESS.md#2-bramy-lokalne).

Diagnostyka opublikowanego artefaktu działa bez otwierania UI i bez używania prawdziwych danych:

```powershell
$process = Start-Process -FilePath .\Abituria.exe `
  -ArgumentList '--release-smoke-test --data-directory "C:\Temp\abituria-smoke"' `
  -WindowStyle Hidden -Wait -PassThru
if ($process.ExitCode -ne 0) { throw "Smoke test nie powiódł się." }
```

Na Ubuntu i macOS należy użyć odpowiednio `./Abituria` lub pliku wykonywalnego wewnątrz `Abituria.app`.

## Dokumentacja

| Dokument | Zakres |
| --- | --- |
| [Instalacja](docs/INSTALLATION.md) | wymagania systemowe, integralność, instalacja, aktualizacja i odinstalowanie |
| [Podręcznik użytkownika](docs/USER_GUIDE.md) | profile, materiały, zadania i kalkulatory |
| [Analiza biznesowa](docs/BUSINESS_ANALYSIS.md) | uzasadnienie produktu, interesariusze, wartość, model udostępniania, ryzyka i kamienie milowe |
| [Wymagania](docs/REQUIREMENTS.md) | wymagania funkcjonalne, niefunkcjonalne i kryteria akceptacji |
| [Architektura](docs/ARCHITECTURE.md) | komponenty, dane i odpowiedzialności modułów |
| [Proces wydania](docs/RELEASE_PROCESS.md) | bramy, pakowanie, smoke test, Pages i publikacja |
| [Testy końcowe](docs/TESTING.md) | zakres testów funkcjonalnych, regresyjnych, wydajnościowych i pamięciowych |
| [Audyt dostępności WCAG 2.2 A/AA](docs/ACCESSIBILITY_WCAG_AUDIT.md) | pełna macierz kryteriów, dowody techniczne i jawna lista kontroli manualnych |
| [Pakiet dla komisji](docs/COMMISSION_PACKAGE.md) | indeks dokumentacji technicznej, PDF i protokół odbioru |
| [Odbiór Issue #43](docs/acceptance/README.md) | osobne protokoły przyrostów I-IV, decyzje i bramy zamknięcia |
| [Publiczna obrona Issue #44](docs/DEFENSE_PROTOCOL.md) | data, komisja, przebieg, nagranie, wynik i potwierdzenie kamienia milowego M7 |
| [Kryteria oceny Issue #45](docs/EVALUATION_PROTOCOL.md) | macierz siedmiu obszarów, warunki akceptacji, wynik bardzo dobry i gotowy komentarz zamykający |
| [Testy użyteczności](docs/USABILITY_TEST_RESULTS.md) | dwie wymagane rundy, techniczny przegląd, problemy, poprawki i retesty |
| [Przekazanie](docs/DELIVERY_PROTOCOL.md) | publiczne wydanie albo ograniczona, prawnie dopuszczalna forma przekazania |
| [Zależności](docs/DEPENDENCIES.md) | dokładnie rozwiązane pakiety produkcyjne i testowe |
| [Pochodzenie treści](docs/CONTENT_PROVENANCE.md) | zasady dopuszczania treści, fontów i obrazów do paczek oraz oświadczenie o prawach |
| [Plan archiwum matur 2026-2020](docs/MATURA_ARCHIVE_PLAN.md) | stan arkuszy na `origin/main` i lokalnie, braki oraz etapy uzupełnienia Formuł 2023 i 2015 |
| [Matura maj 2022 PP - Formuła 2015](docs/MATURA_2022_BASIC_COVERAGE.md) | przypięte źródła, SHA-256, kontrakt 35/35/45, diagramy i blokada proweniencji |
| [Matura maj 2022 PR - Formuła 2015](docs/MATURA_2022_EXTENDED_COVERAGE.md) | przypięte źródła, SHA-256, kontrakt 15/15/50, diagram i blokada proweniencji |
| [Matura poprawkowa 2022 PP - Formuła 2015](docs/MATURA_2022_CORRECTION_BASIC_COVERAGE.md) | przypięte źródła z archiwum publicznego, SHA-256, kontrakt 35/35/45, diagramy i blokada proweniencji |
| [Matura maj 2023 PP](docs/MATURA_2023_BASIC_COVERAGE.md) | przypięte źródła, SHA-256, kontrakt 31/34/46, diagramy i blokada proweniencji |
| [Matura poprawkowa 2024 PP](docs/MATURA_2024_CORRECTION_BASIC_COVERAGE.md) | przypięte źródła z archiwum publicznego, SHA-256, kontrakt 30/36/46, diagramy i blokada proweniencji |
| [Matura maj 2023 PR](docs/MATURA_2023_EXTENDED_COVERAGE.md) | przypięte źródła, SHA-256, kontrakt 13/14/50, diagramy i status proweniencji |
| [Matura maj 2025 PP](docs/MATURA_2025_COVERAGE.md) | przypięte źródła, SHA-256, kontrakt 31/35/50, diagramy i status proweniencji |
| [Matura maj 2025 PR](docs/MATURA_2025_EXTENDED_COVERAGE.md) | przypięte źródła, SHA-256, kontrakt 12/13/50 i status proweniencji |
| [Matura maj 2026 PP](docs/MATURA_2026_COVERAGE.md) | przypięte źródła, SHA-256, kontrakt 33/37/50, diagramy i status proweniencji |
| [Matura maj 2026 PR](docs/MATURA_2026_EXTENDED_COVERAGE.md) | przypięte źródła, SHA-256, kontrakt 12/13/50, diagramy i status proweniencji |
| [Znane ograniczenia](docs/KNOWN_LIMITATIONS.md) | jawny zakres wersji beta |
| [Historia zmian](CHANGELOG.md) | pierwsze rzeczywiste wydanie i dalsze zmiany |
| [Współtworzenie](CONTRIBUTING.md) | przygotowanie zmian, testy, SonarQube Cloud i wymagania pull requestu |
| [Wsparcie](SUPPORT.md) | zgłaszanie błędów i wymagane dane diagnostyczne |
| [Bezpieczeństwo](SECURITY.md) | prywatne zgłoszenia podatności i wspierane wersje |

Kompletność kursu rozwijanego w ramach Issue #3, zachowanie treści historycznego Issue #35 oraz regresje opisują [macierz kursu](docs/MATH_COURSE_2023_COVERAGE.md) i [inwentarz migracji](docs/MIGRATION_INVENTORY.md). Stan pełnego archiwum matur 2026-2020, w tym rozróżnienie Formuł 2023 i 2015, opisuje [plan archiwum](docs/MATURA_ARCHIVE_PLAN.md). Zakres arkuszy 2022, 2023, 2024, 2025 i 2026 opisują osobne macierze poziomu podstawowego lub rozszerzonego wymienione powyżej. Nowe lokalne arkusze pozostają zablokowane przed publicznym wydaniem do czasu osobistego rozszerzenia deklaracji praw.

## Autor i licencje

Autorem i opiekunem aktualnej implementacji jest [Adam Kubiś](AUTHORS.md).

Kod projektu jest udostępniany na licencji [MIT](https://github.com/haribo841/Abituria/blob/main/LICENSE). Licencje zależności i dodatkowe informacje dystrybucyjne zawiera [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md). Licencja kodu nie oznacza automatycznie prawa do redystrybucji każdego materiału edukacyjnego lub obrazu. O dopuszczeniu zasobu do paczki rozstrzygają [inwentarz pochodzenia](docs/CONTENT_PROVENANCE.md) i powiązane dowody, w tym [oświadczenie właściciela projektu](docs/ASSET_RIGHTS_DECLARATION.md).

## Zgłoszenia

- zwykły błąd lub propozycja: [GitHub Issues](https://github.com/haribo841/Abituria/issues/new);
- propozycja zmiany w kodzie lub dokumentacji: [CONTRIBUTING.md](CONTRIBUTING.md);
- pytanie o użycie: [SUPPORT.md](SUPPORT.md);
- podatność lub dane wrażliwe: [SECURITY.md](SECURITY.md).
