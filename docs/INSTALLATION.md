# Instalacja, aktualizacja i odinstalowanie

Ta instrukcja dotyczy wersji `0.9.0-beta.1`. Abituria jest publikowana jako samowystarczalna aplikacja portable. Nie wymaga osobnej instalacji .NET, ale nie zawiera natywnego instalatora ani automatycznej aktualizacji.

> [!IMPORTANT]
> Inwentarz pochodzenia treści, fontów i obrazów ma wynik pozytywny. Paczki staną się przeznaczone do publicznego pobierania dopiero po wykonaniu kompletnej checklisty, utworzeniu tagu i opublikowaniu zweryfikowanego GitHub Release. Nie należy udostępniać artefaktów utworzonych lokalnie z pominięciem tej kontroli.

## Obsługiwane środowiska beta

| System | Architektura | Wymagania |
| --- | --- | --- |
| Windows 11 24H2 | x64 | aktualne poprawki systemu |
| Ubuntu 24.04 | x64 | `libx11-6`, `libice6`, `libsm6`, `libfontconfig1` |
| macOS 15 | Intel x64 | aktualne poprawki systemu |

macOS na Apple Silicon, Windows ARM64, inne dystrybucje Linuksa, systemy mobilne i przeglądarka nie są objęte deklaracją tej wersji beta.

## Pobieranie

Po zatwierdzeniu wydania pobierz z [GitHub Release `v0.9.0-beta.1`](https://github.com/haribo841/Abituria/releases/tag/v0.9.0-beta.1) dokładnie jedną paczkę:

- `Abituria-v0.9.0-beta.1-win-x64.zip`;
- `Abituria-v0.9.0-beta.1-linux-x64.tar.gz`;
- `Abituria-v0.9.0-beta.1-osx-x64.zip`.

Pobierz również `SHA256SUMS.txt`. Każdej paczce odpowiada osobny SBOM SPDX oraz dwie atestacje wygenerowane przez workflow wydania: domyślna SLSA build provenance i atestacja dokumentu SBOM.

## Sprawdzenie integralności i pochodzenia

Nie uruchamiaj paczki, jeżeli suma nie zgadza się z `SHA256SUMS.txt` albo weryfikacja attestation wskazuje inne repozytorium.

### Windows PowerShell

```powershell
$archive = ".\Abituria-v0.9.0-beta.1-win-x64.zip"
(Get-FileHash $archive -Algorithm SHA256).Hash
Select-String -Path .\SHA256SUMS.txt -Pattern ([IO.Path]::GetFileName($archive))
gh attestation verify $archive --repo haribo841/Abituria
```

Porównaj pełne wartości SHA-256 bez pomijania znaków. Polecenie `gh attestation verify` wymaga [GitHub CLI](https://cli.github.com/).
Bez parametru `--predicate-type` GitHub CLI sprawdza domyślną atestację SLSA build provenance. Atestację SBOM można sprawdzić osobno:

```powershell
gh attestation verify $archive `
  --repo haribo841/Abituria `
  --predicate-type https://spdx.dev/Document/v2.3
```

### Ubuntu

```bash
sha256sum Abituria-v0.9.0-beta.1-linux-x64.tar.gz
grep 'Abituria-v0.9.0-beta.1-linux-x64.tar.gz' SHA256SUMS.txt
gh attestation verify Abituria-v0.9.0-beta.1-linux-x64.tar.gz --repo haribo841/Abituria
```

### macOS

```bash
shasum -a 256 Abituria-v0.9.0-beta.1-osx-x64.zip
grep 'Abituria-v0.9.0-beta.1-osx-x64.zip' SHA256SUMS.txt
gh attestation verify Abituria-v0.9.0-beta.1-osx-x64.zip --repo haribo841/Abituria
```

## Windows

1. Zweryfikuj pobrany plik.
2. Utwórz nowy katalog, na przykład `%LOCALAPPDATA%\Programs\Abituria-0.9.0-beta.1`.
3. Rozpakuj ZIP do tego katalogu. Archiwum utworzy podkatalog `Abituria-v0.9.0-beta.1-win-x64`. Nie uruchamiaj programu bezpośrednio z archiwum.
4. Wejdź do utworzonego podkatalogu i uruchom `Abituria.exe`.

Wydanie beta nie ma podpisu kodu. SmartScreen może zgłosić nierozpoznanego wydawcę. Po sprawdzeniu sumy i attestation konkretnej paczki można użyć opcji systemu pokazującej więcej informacji i zezwalającej na jednorazowe uruchomienie tego pliku. Nie wyłączaj globalnie SmartScreen ani programu antywirusowego.

Smoke test bez otwierania okna i bez używania prawdziwej bazy:

```powershell
$data = Join-Path $env:TEMP "abituria-smoke-$([Guid]::NewGuid())"
$app = Join-Path $env:LOCALAPPDATA "Programs\Abituria-0.9.0-beta.1\Abituria-v0.9.0-beta.1-win-x64\Abituria.exe"
try {
    $process = Start-Process `
        -FilePath $app `
        -ArgumentList "--release-smoke-test --data-directory `"$data`"" `
        -WindowStyle Hidden `
        -Wait `
        -PassThru
    if ($process.ExitCode -ne 0) { throw "Smoke test nie powiódł się z kodem $($process.ExitCode)." }
}
finally {
    if (Test-Path -LiteralPath $data) { Remove-Item -Recurse -LiteralPath $data }
}
```

## Ubuntu

1. Zainstaluj wymagane biblioteki systemowe:

   ```bash
   sudo apt update
   sudo apt install libx11-6 libice6 libsm6 libfontconfig1
   ```

2. Zweryfikuj pobrany plik.
3. Rozpakuj paczkę do nowego katalogu:

   ```bash
   mkdir -p "$HOME/.local/opt/abituria-0.9.0-beta.1"
   tar -xzf Abituria-v0.9.0-beta.1-linux-x64.tar.gz \
     -C "$HOME/.local/opt/abituria-0.9.0-beta.1"
   cd "$HOME/.local/opt/abituria-0.9.0-beta.1/Abituria-v0.9.0-beta.1-linux-x64"
   chmod u+x ./Abituria
   ./Abituria
   ```

Smoke test:

```bash
data="$(mktemp -d)"
./Abituria --release-smoke-test --data-directory "$data"
status=$?
rm -rf -- "$data"
exit "$status"
```

## macOS Intel

1. Zweryfikuj pobrany plik.
2. Rozpakuj ZIP. Archiwum utworzy katalog `Abituria-v0.9.0-beta.1-osx-x64`, wewnątrz którego znajduje się kompletne `Abituria.app`.
3. Przenieś aplikację `Abituria-v0.9.0-beta.1-osx-x64/Abituria.app` do katalogu `/Applications` albo uruchamiaj ją z rozpakowanego katalogu użytkownika.
4. Otwórz aplikację.

Wydanie beta nie jest podpisane ani notaryzowane. Gatekeeper może zablokować pierwsze uruchomienie. Po zweryfikowaniu sumy i attestation użyj systemowej, jednorazowej procedury otwarcia konkretnej aplikacji, na przykład Control-click, „Open”, a następnie potwierdzenie, lub opcji „Open Anyway” dla Abiturii w ustawieniach Privacy & Security. Nie wyłączaj Gatekeepera globalnie.

Smoke test:

```bash
data="$(mktemp -d)"
app="/Applications/Abituria.app"
# Jeśli aplikacja pozostała w rozpakowanym katalogu, ustaw zamiast tego:
# app="$PWD/Abituria-v0.9.0-beta.1-osx-x64/Abituria.app"
"$app/Contents/MacOS/Abituria" \
  --release-smoke-test --data-directory "$data"
status=$?
rm -rf -- "$data"
exit "$status"
```

## Dane użytkownika i kopia zapasowa

Program zapisuje bazę poza katalogiem aplikacji. Typowe lokalizacje wynikające z systemowego `LocalApplicationData` to:

| System | Baza |
| --- | --- |
| Windows | `%LOCALAPPDATA%\Abituria\abituria.db` |
| Ubuntu | `${XDG_DATA_HOME:-$HOME/.local/share}/Abituria/abituria.db` |
| macOS | `$HOME/Library/Application Support/Abituria/abituria.db` |

Przed kopiowaniem bazy całkowicie zamknij aplikację. Skopiuj cały katalog `Abituria`, aby zachować bazę i ewentualne pliki pomocnicze. Kopię przechowuj jak dane prywatne, ponieważ nazwy profili i postęp nie są szyfrowane na poziomie całej bazy.

## Aktualizacja

1. Zamknij Abiturię.
2. Wykonaj kopię katalogu danych użytkownika.
3. Pobierz nową paczkę, jej sumy i zweryfikuj attestation.
4. Rozpakuj nową wersję do nowego katalogu zamiast mieszać pliki z dwóch wersji.
5. Uruchom smoke test z katalogiem tymczasowym.
6. Uruchom nową aplikację. Migracje bazy wykonują się automatycznie.
7. Sprawdź na ekranie „O programie” wersję i commit, a następnie zaloguj się i potwierdź postęp.
8. Usuń stary katalog programu dopiero po poprawnym uruchomieniu nowej wersji.

Aktualizacja z bazy utworzonej przez poprzednią implementację .NET 9 jest kryterium regresyjnym wydania. Mimo tego kopia zapasowa pozostaje obowiązkowym środkiem ostrożności.

## Odinstalowanie

Usunięcie samej aplikacji nie usuwa profili ani postępu:

1. Zamknij Abiturię.
2. Usuń rozpakowany katalog programu lub `Abituria.app`.
3. Jeśli chcesz zachować dane na potrzeby ponownej instalacji, zakończ w tym miejscu.
4. Opcjonalnie usuń katalog danych `Abituria` z lokalizacji wskazanej wyżej.

Ostatni krok jest nieodwracalny i usuwa lokalne konta, kody odzyskiwania zapisane jako skróty i postęp. Najpierw wykonaj kopię, jeżeli dane mogą być jeszcze potrzebne.

## Najczęstsze problemy

| Objaw | Działanie |
| --- | --- |
| System blokuje uruchomienie | sprawdź sumę i attestation, potem użyj jednorazowej procedury systemowej dla tej paczki |
| Ubuntu zgłasza brak biblioteki | zainstaluj cztery pakiety z sekcji Ubuntu i uruchom ponownie |
| Aplikacja nie widzi dawnego profilu | sprawdź, czy działa na tym samym koncie systemowym i czy katalog danych nie został przeniesiony |
| Smoke test dotyka zwykłej bazy | przerwij i zgłoś błąd - poprawny test używa wyłącznie podanego katalogu tymczasowego |
| Baza po aktualizacji nie otwiera się | nie nadpisuj kopii, wróć do poprzedniego katalogu aplikacji i utwórz zgłoszenie z wersją, bez publikowania bazy |

Pomoc w zgłoszeniu problemu: [SUPPORT.md](../SUPPORT.md).
