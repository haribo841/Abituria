# Abituria

Abituria to działająca offline aplikacja desktopowa wspierająca naukę matematyki na poziomie szkoły średniej. Aktualna implementacja używa C#, .NET 10 LTS i AvaloniaUI 12. Dane profili oraz postęp są przechowywane lokalnie w SQLite.

Wersja przygotowywana do pierwszej publicznej publikacji: `0.9.0-beta.1`.

> [!IMPORTANT]
> Publiczne wydanie jest obecnie zablokowane do czasu uzyskania kompletnego, pozytywnego wyniku inwentarza pochodzenia zasobów. Dotyczy to w szczególności materiałów CKE i odziedziczonych grafik bez jednoznacznie udokumentowanej podstawy dystrybucji. Sam build techniczny nie znosi tej blokady.

## Pobieranie

Po zamknięciu checklisty wydawniczej paczki pojawią się w [GitHub Releases](https://github.com/haribo841/Abituria/releases/tag/v0.9.0-beta.1):

| System | Paczka | Deklarowane środowisko beta |
| --- | --- | --- |
| Windows | `Abituria-v0.9.0-beta.1-win-x64.zip` | Windows 11 24H2 x64 |
| Ubuntu | `Abituria-v0.9.0-beta.1-linux-x64.tar.gz` | Ubuntu 24.04 x64 |
| macOS | `Abituria-v0.9.0-beta.1-osx-x64.zip` | macOS 15 na komputerze Intel |

Paczki są samowystarczalne i nie wymagają instalacji środowiska .NET. Są to wydania portable, bez instalatora, automatycznej aktualizacji, podpisu kodu, AOT i trybu single-file. Przed uruchomieniem należy sprawdzić sumę SHA-256 oraz attestation artefaktu.

- [Instrukcja instalacji, aktualizacji i odinstalowania](docs/INSTALLATION.md)
- [Dokumentacja online](https://haribo841.github.io/Abituria/)
- [Znane ograniczenia](docs/KNOWN_LIMITATIONS.md)

## Funkcje

- lokalne konta chronione hasłem oraz profile gościa;
- jednorazowe kody odzyskiwania i postęp zapisywany osobno dla każdego profilu;
- 18 tablic matematycznych;
- 7 dostępnych działów matematycznych i 2 jawnie oznaczone działy w przygotowaniu;
- 35 zadań z matury poprawkowej 2021, dostępnych według arkusza lub 17 tematów;
- sprawdzanie odpowiedzi A-D, podpowiedzi, ujawnianie odpowiedzi i sesyjny brudnopis;
- kalkulator ogólny z nawiasami, potęgami, pierwiastkami, notacją naukową, `Ans`, historią i powtarzaniem `=`;
- kalkulator funkcji kwadratowej z postacią ogólną, kanoniczną i iloczynową;
- jedno okno aplikacji z nawigacją między materiałami, zadaniami, kalkulatorami, profilem i planem rozwoju;
- ekran „O programie” z wersją, identyfikatorem commita, licencją, autorem i adresem repozytorium.

Szczegółową instrukcję korzystania z tych funkcji zawiera [podręcznik użytkownika](docs/USER_GUIDE.md).

## Szybki start z paczki

1. Pobierz paczkę przeznaczoną dla swojego systemu oraz `SHA256SUMS.txt` z tego samego wydania.
2. Zweryfikuj sumę i attestation zgodnie z [instrukcją instalacji](docs/INSTALLATION.md#sprawdzenie-integralności-i-pochodzenia).
3. Rozpakuj aplikację do nowego katalogu i wejdź do utworzonego podkatalogu `Abituria-v0.9.0-beta.1-<rid>`.
4. Uruchom `Abituria.exe`, `Abituria` albo `Abituria.app`, zależnie od systemu.
5. Wybierz profil gościa lub utwórz lokalne konto.

Wydanie beta jest niepodpisane. SmartScreen lub Gatekeeper może wyświetlić ostrzeżenie. Dokumentacja opisuje bezpieczną obsługę komunikatu dla konkretnej, zweryfikowanej paczki i nie zaleca globalnego wyłączania zabezpieczeń systemu.

## Dane lokalne

Baza `abituria.db` znajduje się poza katalogiem programu, w systemowym katalogu danych lokalnych użytkownika, w podkatalogu `Abituria`. Dzięki temu zastąpienie katalogu aplikacji nowszą wersją nie usuwa kont ani postępu. Przed aktualizacją zalecane jest wykonanie kopii bazy.

Hasła nie są przechowywane jawnie. Aplikacja używa PBKDF2-HMAC-SHA256, osobnej soli dla każdego konta i wersjonowanej liczby iteracji. Kod odzyskiwania jest wyświetlany tylko raz, a w bazie pozostaje jego skrót.

Przy pierwszym uruchomieniu aplikacja może zaimportować istniejące nazwy z pliku `Abituria/users.txt` w systemowym katalogu danych aplikacji jako profile gościa. Plik źródłowy nie jest usuwany, a import jest idempotentny. Jeżeli nie istnieje żaden profil, tworzony jest gość `Maturzysta`.

## Uruchomienie ze źródeł

Wymagany jest .NET SDK `10.0.302`, przypięty w `global.json`. Testy integracyjne skryptów wydawniczych wymagają również PowerShell 7 (`pwsh`) dostępnego w `PATH`.

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
| [Zależności](docs/DEPENDENCIES.md) | dokładnie rozwiązane pakiety produkcyjne i testowe |
| [Pochodzenie treści](docs/CONTENT_PROVENANCE.md) | zasady dopuszczania treści, fontów i obrazów do paczek |
| [Znane ograniczenia](docs/KNOWN_LIMITATIONS.md) | jawny zakres wersji beta |
| [Historia zmian](CHANGELOG.md) | pierwsze rzeczywiste wydanie i dalsze zmiany |
| [Wsparcie](SUPPORT.md) | zgłaszanie błędów i wymagane dane diagnostyczne |
| [Bezpieczeństwo](SECURITY.md) | prywatne zgłoszenia podatności i wspierane wersje |

Kompletność sześciu materiałów uzupełnionych w ramach historycznego `issue #35`, ich kanoniczny seed oraz regresje opisuje [inwentarz migracji](docs/MIGRATION_INVENTORY.md).

## Autor i licencje

Autorem i opiekunem aktualnej implementacji jest [Adam Kubiś](AUTHORS.md).

Kod projektu jest udostępniany na licencji [MIT](https://github.com/haribo841/Abituria/blob/main/LICENSE). Licencje zależności i dodatkowe informacje dystrybucyjne zawiera [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md). Licencja kodu nie oznacza automatycznie prawa do redystrybucji każdego materiału edukacyjnego lub obrazu. O tym, czy zasób może wejść do publicznej paczki, rozstrzyga inwentarz pochodzenia.

## Zgłoszenia

- zwykły błąd lub propozycja: [GitHub Issues](https://github.com/haribo841/Abituria/issues/new);
- pytanie o użycie: [SUPPORT.md](SUPPORT.md);
- podatność lub dane wrażliwe: [SECURITY.md](SECURITY.md).
