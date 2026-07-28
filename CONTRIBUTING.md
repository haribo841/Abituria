# Współtworzenie Abiturii

Dziękujemy za zainteresowanie rozwojem Abiturii. Ten dokument opisuje warunki przyjęcia zmiany do bieżącej implementacji. Obowiązuje razem z [kodeksem postępowania](CODE_OF_CONDUCT.md), [polityką bezpieczeństwa](SECURITY.md) i dokumentacją techniczną repozytorium.

## Zanim zaczniesz

1. Sprawdź istniejące zgłoszenia oraz [roadmapę](docs/ROADMAP.md), aby nie powielać pracy.
2. Dla większej zmiany utwórz lub wskaż issue z opisem problemu, zakresem i kryteriami akceptacji.
3. Pytania o użycie kieruj zgodnie z [SUPPORT.md](SUPPORT.md).
4. Podatności, sekrety i dane użytkowników zgłaszaj wyłącznie prywatnie zgodnie z [SECURITY.md](SECURITY.md). Nie umieszczaj ich w publicznym issue ani pull requeście.

Przesyłając kod, dokumentację albo zasób, upewnij się, że masz prawo udostępnić go w projekcie i potrafisz wskazać jego autora, źródło oraz warunki użycia.

## Przygotowanie zmiany

- Utwórz krótką gałąź od aktualnej gałęzi `main`, na przykład `fix/historia-dzialan` albo `docs/testy-lokalne`.
- Ogranicz pull request do jednego problemu. Nie łącz poprawki z niezwiązanym formatowaniem lub przebudową kodu.
- Zachowaj aktualną architekturę opisaną w [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).
- Dodaj lub zaktualizuj testy dla każdego zmienionego zachowania. Poprawka regresji powinna zawierać test, który odtwarza błąd sprzed poprawki.
- Zaktualizuj dokumentację, gdy zmienia się zachowanie użytkowe, wymaganie, konfiguracja, sposób instalacji albo proces wydania.
- Nie dodawaj katalogów `bin`, `obj`, `artifacts`, `TestResults`, sekretów, baz użytkowników ani przypadkowych plików wygenerowanych.

Zmiany interfejsu muszą zachować obsługę klawiatury, widoczny fokus, skalowanie i kontrast opisane w [audycie WCAG](docs/ACCESSIBILITY_WCAG_AUDIT.md). Do pull requestu dołącz zrzuty ekranu dla istotnej zmiany wizualnej, o ile nie zawierają danych prywatnych.

## Brama lokalna

Przed wysłaniem pull requestu uruchom pełny zestaw kompilacji, testów z pokryciem i analizatorów. Repozytorium przypina wersję SDK w `global.json`, dlatego nie zastępuj jej inną wersją.

```powershell
dotnet restore Abituria.sln --configfile NuGet.Config --locked-mode
dotnet build Abituria.sln --configuration Release --no-restore --no-incremental
python -m pip install --requirement tools/requirements-test.txt
python -m pip check
dotnet test Abituria.sln --configuration Release --no-build --no-restore `
  --results-directory artifacts/coverage/csharp `
  --collect:"XPlat Code Coverage" `
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
python -m coverage run --branch --source=tools -m unittest discover -s tests/python -p "test_*.py"
python -m coverage xml -o artifacts/coverage/python-coverage.xml
$openCoverReport = Get-ChildItem artifacts/coverage/csharp -Recurse -Filter coverage.opencover.xml
pwsh -NoProfile -File tools/release/Test-CoverageThreshold.ps1 `
  -OpenCoverReport $openCoverReport.FullName `
  -PythonCoverageReport artifacts/coverage/python-coverage.xml
dotnet format Abituria.sln whitespace --verify-no-changes --no-restore
dotnet format Abituria.sln analyzers --verify-no-changes --no-restore --severity info
git diff --check
```

Nowy i zmieniony kod produkcyjny musi mieć adekwatne testy. Łączne pokrycie C# i Pythona musi wynosić co najmniej `90%`, a pokrycie gałęzi co najmniej `85%`. Bramka liczy wykonane linie i wyniki warunków z raportów OpenCover oraz Cobertura i nie używa wyłączeń pokrycia. Pełny opis kategorii testów znajduje się w [docs/TESTING.md](docs/TESTING.md).

Na Windows bez PowerShell 7 zastąp `pwsh` poleceniem `powershell.exe` w poniższych wywołaniach skryptów. Skrypty repozytorium wspierają oba hosty.

Uruchom również bramy wspólne dla kompilacji CI:

```powershell
pwsh -NoProfile -File tools/release/Test-NuGetVulnerabilities.ps1 `
  -ReportPath artifacts/audit/local.json
pwsh -NoProfile -File tools/Generate-DependencyDocumentation.ps1 -Verify
pwsh -NoProfile -File tools/Test-ContentProvenance.ps1
```

Jeżeli zmieniasz zależności, odtwórz pliki blokad i dokumentację zależności świadomie, a następnie ponownie uruchom weryfikację. Nie edytuj ręcznie plików generowanych `docs/DEPENDENCIES.md` i `THIRD-PARTY-NOTICES.md`.

## Dokumentacja i odnośniki

Po każdej zmianie pliku Markdown, konfiguracji DocFX albo odnośnika zbuduj stronę z ostrzeżeniami traktowanymi jak błędy i sprawdź wszystkie odnośniki:

```powershell
dotnet tool restore
dotnet tool run docfx -- docfx.json --warningsAsErrors
pwsh -NoProfile -File tools/release/Test-DocumentationSite.ps1 `
  -SiteDirectory artifacts/site `
  -CheckExternalLinks `
  -ExternalLinkFailureAction Fail
```

Nowy host zewnętrzny wymaga świadomej aktualizacji `tools/release/external-links-policy.json`. Nie dodawaj hosta tylko po to, aby ominąć błąd walidatora. Link musi być potrzebny, używać HTTPS i prowadzić do wiarygodnego źródła.

## SonarQube Cloud

Workflow `.github/workflows/sonarcloud.yml` buduje rozwiązanie, uruchamia pełne testy C# i Pythona z raportami OpenCover oraz Cobertura, egzekwuje progi `90%`/`85%` i czeka na quality gate. Pull request nie jest gotowy do scalenia, jeśli wprowadza nowe issue SonarQube Cloud albo którakolwiek brama jakości nie przechodzi.

- Napraw przyczynę issue. Nie wyciszaj reguły i nie oznaczaj problemu jako fałszywie dodatniego bez udokumentowanego uzasadnienia zaakceptowanego w przeglądzie.
- Sprawdź analizatory lokalnie przed wysłaniem zmiany.
- Skan pull requestu z forka może zostać pominięty, ponieważ sekrety GitHub Actions nie są przekazywane do forka. W takim przypadku opisz lokalną weryfikację w pull requeście, a opiekun musi uzyskać miarodajny wynik SonarQube Cloud przed scaleniem.

Szczegóły konfiguracji znajdują się w [docs/SONARQUBE.md](docs/SONARQUBE.md).

## CodeQL

Workflow `.github/workflows/codeql.yml` analizuje kod C# przy pushu i pull requeście do `main`, raz w tygodniu oraz po ręcznym uruchomieniu. Używa przypiętego SDK, locked restore i ręcznego buildu Release, dzięki czemu skan odpowiada rzeczywistej kompilacji repozytorium.

Pull request nie jest gotowy do scalenia, jeżeli CodeQL zgłasza nowy alert code scanning. Napraw przyczynę alertu i dodaj test regresyjny, jeżeli problem może zostać odtworzony wykonaniem kodu. Nie zamykaj alertu jako użytego w testach, zaakceptowanego ryzyka ani fałszywie dodatniego bez konkretnego uzasadnienia zatwierdzonego w przeglądzie.

## Treści, zasoby i wydanie

Każdy paczkowany tekst, obraz, font lub ikona musi być objęty `Content/provenance.json`. Przy dodaniu albo zmianie zasobu:

1. Dodaj dowód autorstwa, źródła i prawa do dystrybucji.
2. Zaktualizuj manifest bez nakładających się wzorców.
3. Uruchom `tools/Test-ContentProvenance.ps1` oraz testy inwentarza i renderowania.
4. Nie ustawiaj `approved` ani `releaseEligible=true` bez wystarczającego dowodu.

Pełny kontrakt opisuje [docs/CONTENT_PROVENANCE.md](docs/CONTENT_PROVENANCE.md). Zmiana wpływająca na publiczne paczki lub publikację musi dodatkowo spełnić [proces wydania](docs/RELEASE_PROCESS.md), w tym wariant walidatora `-RequireReleaseEligible`.

## Pull request

Wypełnij automatycznie podstawiony szablon i podaj:

- powiązane issue oraz cel zmiany;
- zakres wykonany i świadomie niewykonany;
- dokładne polecenia i wyniki weryfikacji;
- dodane lub zmienione testy oraz wpływ na pokrycie;
- wyniki SonarQube Cloud i CodeQL albo przyczynę braku miarodajnego skanu;
- wpływ na UI, dostępność, dane, zależności, dokumentację, proweniencję i wydanie;
- zrzuty ekranu lub inne dowody potrzebne recenzentowi.

Jeżeli obszar nie dotyczy zmiany, wpisz `Nie dotyczy` i krótkie uzasadnienie. Nie oznaczaj bramy jako wykonanej, jeśli jej nie uruchomiono.

Pull request jest gotowy do scalenia dopiero po przeglądzie, rozwiązaniu uwag, zielonych wymaganych checkach, przejściu quality gate SonarQube Cloud, braku nowych alertów CodeQL oraz potwierdzeniu, że dokumentacja i testy odpowiadają końcowemu zakresowi.
