# Proces wydania

Ten dokument opisuje przygotowanie, walidację i publikację Abiturii. Przykłady dotyczą pierwszego prerelease `v0.9.0-beta.1`.

## Zasada nadrzędna

Build techniczny, artefakt workflow i GitHub Release to trzy różne stany. Publiczne wydanie istnieje dopiero po ręcznym opublikowaniu zweryfikowanego draftu prerelease. Issue #36 można zamknąć dopiero po publikacji prerelease, wdrożeniu GitHub Pages i dodaniu komentarza z linkami do wydania, workflow oraz checklisty.

Aktualny inwentarz ma `releaseEligible=false`. Nierozstrzygnięte prawa do materiałów CKE, odziedziczonych grafik matematycznych oraz ikon aplikacji blokują tagowanie i publiczne wydanie. Szczegóły: [CONTENT_PROVENANCE.md](CONTENT_PROVENANCE.md).

## Kontrakt wydania

- wersja jest zapisana jeden raz w `Directory.Build.props`;
- tag ma postać dokładnie `v<Version>`;
- assembly, ekran „O programie”, `release.json`, nazwy paczek, changelog i dokumentacja muszą zawierać tę samą wersję;
- SDK pochodzi z `global.json` i dla tego wydania ma wersję `10.0.302`;
- restore używa lockfile i `--locked-mode`;
- publikacja jest self-contained, nietrimowana, bez AOT, ReadyToRun, single-file i PDB;
- wszystkie trzy paczki są tworzone i testowane na natywnych runnerach;
- draft jest prerelease i nie może zostać opublikowany automatycznie;
- natywne instalatory, podpisy kodu i automatyczne aktualizacje nie należą do tego wydania.

## Role plików

| Plik lub katalog | Odpowiedzialność |
| --- | --- |
| `Directory.Build.props` | kanoniczna wersja produktu |
| `global.json` | kanoniczna wersja SDK |
| `packages.lock.json` i lockfile testów | dokładne grafy zależności |
| `.config/dotnet-tools.json` | DocFX `2.78.5` i Microsoft SBOM Tool `4.1.5` |
| `Content/provenance.json` | prawo do dystrybucji paczkowanych zasobów |
| `CHANGELOG.md` | notatki rzeczywistego wydania |
| `tools/release` | wersja, audyt, publikacja, architektura, smoke test, pakowanie i walidacja |
| `.github/workflows/release.yml` | macierz trzech platform i draft prerelease |
| `.github/workflows/pages.yml` | build i wdrożenie DocFX bez gałęzi `gh-pages` |

## 1. Przygotowanie wersji

1. Pracuj z aktualnej gałęzi `main` i czystego worktree.
2. Ustaw `Version` wyłącznie w `Directory.Build.props`.
3. Zaktualizuj sekcję tej wersji w `CHANGELOG.md`. Przed utworzeniem tagu zastąp „oczekuje na publikację” planowaną datą publikacji w formacie `YYYY-MM-DD`.
4. Zaktualizuj instrukcje, ograniczenia, wymagania, roadmapę oraz stronę wejściową.
5. Wygeneruj dokumenty zależności z dokładnie rozwiązanych lockfile:

   ```powershell
   powershell -ExecutionPolicy Bypass -File tools/Generate-DependencyDocumentation.ps1
   powershell -ExecutionPolicy Bypass -File tools/Generate-DependencyDocumentation.ps1 -Verify
   ```

6. Sprawdź, że w changelogu nie ma fikcyjnych historycznych wydań.

## 2. Bramy lokalne

Użyj SDK przypiętego w `global.json`:

```powershell
$ErrorActionPreference = "Stop"
dotnet --version
dotnet tool restore
dotnet restore Abituria.sln --configfile NuGet.Config --locked-mode
dotnet build Abituria.sln --configuration Release --no-restore
dotnet test Abituria.sln --configuration Release --no-build --no-restore
dotnet format Abituria.sln whitespace --verify-no-changes --no-restore
powershell -ExecutionPolicy Bypass -File tools/release/Test-NuGetVulnerabilities.ps1 `
  -ReportPath artifacts/audit/local.json
powershell -ExecutionPolicy Bypass -File tools/Generate-DependencyDocumentation.ps1 -Verify
powershell -ExecutionPolicy Bypass -File tools/Test-ContentProvenance.ps1 `
  -RequireReleaseEligible
dotnet tool run docfx -- docfx.json --warningsAsErrors
powershell -ExecutionPolicy Bypass -File tools/release/Test-DocumentationSite.ps1 `
  -SiteDirectory artifacts/site -CheckExternalLinks -ExternalLinkFailureAction Fail
git diff --check
```

Wynik audytu NuGet musi zawierać zero podatności. Ostrzeżenia `NU1901`-`NU1904` są traktowane jako błędy. Nie wolno używać `NuGetAudit=false`, zmieniać lockfile podczas locked restore ani obniżać progu audytu.

Test pochodzenia z `-RequireReleaseEligible` jest twardą bramą. Jeżeli nie przechodzi, nie twórz tagu i nie próbuj ręcznie uruchamiać publikacji.

Walidator dokumentacji zawsze blokuje niedozwolone hosty, odnośniki HTTP, wyjście poza katalog strony i brakujące cele lokalne. Linki do plików tego samego repozytorium na gałęzi `main` są mapowane na bieżący checkout i wymagają istniejącego pliku, dzięki czemu kontrola przed pushem nie zależy od starszego stanu GitHub. Dostępność pozostałych adresów HTTPS jest sprawdzana trzy razy równolegle w ograniczonych partiach, a trwały brak odpowiedzi blokuje workflow. Celowo nieistniejący jeszcze publiczny adres draftu release i adres właśnie wdrażanej strony Pages są wyłączone wyłącznie z sondy online, wraz z uzasadnieniem w `tools/release/external-links-policy.json`. Po wdrożeniu Pages osobna twarda sonda sprawdza rzeczywisty adres strony.

## 3. CI przed tagiem

1. Zacommituj wszystkie zamierzone pliki, w tym oba lockfile i wygenerowaną dokumentację.
2. Otwórz PR albo wypchnij uzgodnioną zmianę do `main`.
3. Poczekaj na zielone wyniki build/test, SonarQube, audytu i Pages.
4. Otwórz wdrożoną stronę `https://haribo841.github.io/Abituria/` i ręcznie sprawdź stronę główną, pobieranie, wymagania, instrukcję, funkcje, licencję, autora, changelog, ograniczenia, wsparcie i wszystkie linki.

Po `deploy-pages` workflow sześć razy próbuje pobrać rzeczywisty URL wdrożenia i wymaga odpowiedzi HTTP 200 zawierającej nazwę Abituria. Nie oznaczaj jednak Pages jako gotowych wyłącznie na podstawie poprawnego builda DocFX i automatycznej sondy - wykonaj także powyższą kontrolę ręczną.

## 4. Tag

Tag musi wskazywać dokładnie zweryfikowany commit `main`:

```powershell
git switch main
git pull --ff-only
git status --short
git tag -a v0.9.0-beta.1 -m "Abituria v0.9.0-beta.1"
powershell -ExecutionPolicy Bypass -File tools/release/Test-ReleaseVersion.ps1 `
  -Tag v0.9.0-beta.1
git push origin v0.9.0-beta.1
```

Nie przesuwaj opublikowanego tagu. Jeśli tag wskazuje zły commit i workflow nie utworzył wydania, usuń go lokalnie i zdalnie tylko po sprawdzeniu stanu. Jeżeli draft lub attestation już istnieją, anuluj proces, opisz błąd i przygotuj nową wersję zamiast przepisywać historię.

## 5. Automatyczny workflow tagu

Workflow `release` wykonuje:

1. **Preflight na Ubuntu** - checkout tagu, zgodność tagu, wersji, README, changelogu i przypiętych narzędzi, build DocFX, deterministyczna polityka linków z sondą online oraz twarda brama `Test-ContentProvenance.ps1 -RequireReleaseEligible`.
2. **Macierz natywna**:

   | RID | Runner | Artefakt |
   | --- | --- | --- |
   | `win-x64` | `windows-2025` | `Abituria-v0.9.0-beta.1-win-x64.zip` |
   | `linux-x64` | `ubuntu-24.04` | `Abituria-v0.9.0-beta.1-linux-x64.tar.gz` |
   | `osx-x64` | `macos-15-intel` | `Abituria-v0.9.0-beta.1-osx-x64.zip` |

3. Na każdym runnerze - restore narzędzi, locked restore rozwiązania i RID, build, pełne testy, format, audyt zależności, `git diff --check`, publikacja, kontrola architektury, utworzenie archiwum, ponowne rozpakowanie archiwum i smoke test pliku rzeczywiście przeznaczonego dla użytkownika. Dla macOS uruchamiany jest `Abituria.app/Contents/MacOS/Abituria`, a nie pośredni katalog `publish`.
4. Dla macOS - utworzenie `Abituria.app` z `Info.plist`, identyfikatorem `io.github.haribo841.abituria`, wersją i ikoną `.icns`.
5. Dla każdej platformy - zewnętrzny i dołączony do paczki SBOM SPDX 2.2.
6. Po połączeniu artefaktów - `SHA256SUMS.txt`, walidacja kompletności archiwów, wersji, architektury, PDB, sekretów, snapshotów, sum i SBOM. Skan sekretów obejmuje strumieniowo również pliki binarne oraz wzorce kluczy prywatnych, tokenów GitHub/AWS/JWT/usług i poświadczeń w connection stringach.
7. Dwie atestacje dla każdego z trzech archiwów: domyślna SLSA build provenance oraz osobna atestacja odpowiadającego SBOM. Plik sum otrzymuje własną atestację pochodzenia.
8. Utworzenie lub odświeżenie prywatnego draftu GitHub Release oznaczonego jako prerelease.

Każde archiwum zawiera katalog główny nazwany jak paczka, aplikację, `LICENSE`, `RELEASE-README.md`, `THIRD-PARTY-NOTICES.md`, `release.json` i wewnętrzny SBOM. Obok archiwów wydawane są:

- `Abituria-v0.9.0-beta.1-win-x64.spdx.json`;
- `Abituria-v0.9.0-beta.1-linux-x64.spdx.json`;
- `Abituria-v0.9.0-beta.1-osx-x64.spdx.json`;
- `SHA256SUMS.txt`.

## 6. Kontrola draftu

Draft pozostaje prywatny. Pobierz wszystkie artefakty bez ich modyfikowania i odnotuj w checkliście:

- trzy natywne joby zakończone powodzeniem;
- pełne testy na każdym runnerze;
- zielony SonarQube na wydawanym commicie;
- zero podatności NuGet;
- `releaseEligible=true` i pozytywny log walidatora;
- poprawną nazwę, rozmiar i architekturę każdej paczki;
- brak PDB, sekretów i snapshotów;
- obecność `LICENSE`, instrukcji i notices;
- poprawne `release.json`, assembly version, ekran „O programie” i commit;
- zgodność sum SHA-256;
- prawidłowy SBOM SPDX dla każdej paczki;
- udaną weryfikację attestation;
- smoke test z izolowaną bazą na każdym runnerze;
- nową bazę i kopię bazy utworzonej przez .NET 9;
- profil gościa, rejestrację, logowanie, odzyskanie konta i trwałość postępu;
- oba kalkulatory oraz reprezentatywne treści;
- stronę Pages i działające linki;
- test instalacji co najmniej na jednym komputerze innym niż środowisko programistyczne dla każdego deklarowanego systemu.

Przykładowa weryfikacja domyślnej atestacji SLSA build provenance:

```powershell
gh attestation verify .\Abituria-v0.9.0-beta.1-win-x64.zip `
  --repo haribo841/Abituria
```

GitHub CLI domyślnie wymaga predykatu `https://slsa.dev/provenance/v1`, dlatego powyższe polecenie nie może zostać zaliczone wyłącznie przez atestację SBOM. Atestację SPDX sprawdź niezależnie:

```powershell
gh attestation verify .\Abituria-v0.9.0-beta.1-win-x64.zip `
  --repo haribo841/Abituria `
  --predicate-type https://spdx.dev/Document/v2.3
```

## 7. Publikacja prerelease

Publikację wykonuje opiekun dopiero po podpisaniu checklisty. W interfejsie GitHub wybierz „Publish release” i upewnij się, że opcja prerelease pozostaje zaznaczona. Alternatywnie:

```powershell
gh release edit v0.9.0-beta.1 `
  --repo haribo841/Abituria `
  --draft=false `
  --prerelease
```

Po publikacji:

1. Otwórz publiczny release w sesji niezalogowanej.
2. Pobierz po jednej paczce i ponownie sprawdź sumę oraz attestation.
3. Sprawdź, czy Pages prowadzą do właściwego release.
4. Dodaj do issue #36 komentarz z linkami do release, workflow, Pages i kompletnej checklisty.
5. Zamknij issue dopiero po wykonaniu wszystkich poprzednich punktów.

## 8. Reakcja na błąd po wydaniu

- Dla błędu dokumentacji popraw `main`, ponownie wdroż Pages i opisz korektę.
- Dla błędu binarnego, bezpieczeństwa, wersji lub pochodzenia zasobów nie podmieniaj po cichu opublikowanych plików.
- Oznacz wydanie jako problematyczne, usuń je z publicznej dostępności, jeśli dystrybucja jest niedozwolona, i opublikuj informację bezpieczeństwa lub ograniczenie.
- Przygotuj nową wersję i nowy tag po przejściu pełnej procedury.
- Zachowaj logi workflow, sumy, SBOM, attestation i decyzję o wycofaniu jako ślad audytowy.

## Checklista zamknięcia Issue #36

- [ ] inwentarz pochodzenia jest kompletny i `releaseEligible=true`;
- [ ] wszystkie lokalne oraz zdalne bramy są zielone;
- [ ] Pages są publiczne i zweryfikowane;
- [ ] tag wskazuje zweryfikowany commit;
- [ ] draft zawiera dokładny komplet artefaktów;
- [ ] testy instalacyjne na trzech systemach są udokumentowane;
- [ ] prerelease jest publicznie dostępny;
- [ ] sumy, SBOM i attestation są publicznie weryfikowalne;
- [ ] komentarz końcowy w issue zawiera linki i checklistę;
- [ ] issue #36 zostało zamknięte dopiero po wykonaniu powyższych punktów.
