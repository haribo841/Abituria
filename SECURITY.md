# Polityka bezpieczeństwa

## Wspierane wersje

| Wersja | Status |
| --- | --- |
| `0.9.0-beta.1` | wsparcie rozpocznie się po publicznej publikacji prerelease |
| buildy z gałęzi i historyczne snapshoty | niewspierane |

Dopóki `0.9.0-beta.1` nie zostanie opublikowana, repozytorium nie udostępnia wspieranej wersji binarnej. Po wydaniu poprawki bezpieczeństwa wsparcie obejmuje najnowszą opublikowaną wersję beta, chyba że informacja przy wydaniu stanowi inaczej.

## Prywatne zgłaszanie podatności

Nie opisuj podatności, sekretów ani danych użytkownika w publicznym issue. Użyj [prywatnego zgłoszenia bezpieczeństwa GitHub](https://github.com/haribo841/Abituria/security/advisories/new).

Zgłoszenie powinno zawierać:

- wersję i commit;
- dotknięty system operacyjny;
- opis wpływu i warunków wykorzystania;
- minimalne kroki odtworzenia lub proof of concept;
- proponowane działanie naprawcze, jeżeli jest znane;
- informację, czy podatność została już ujawniona innym osobom.

Nie dołączaj prawdziwej bazy użytkownika. Jeśli odtworzenie wymaga danych, przygotuj minimalną bazę testową bez danych osobowych, haseł i kodów odzyskiwania.

Opiekun projektu potwierdzi otrzymanie zgłoszenia, oceni wpływ i uzgodni sposób ujawnienia. Czas naprawy zależy od istotności, odtwarzalności i dostępności bezpiecznego rozwiązania.

## Zakres bezpieczeństwa danych

Abituria działa lokalnie i nie wymaga internetu do podstawowych funkcji. Przechowuje w SQLite:

- nazwy profili;
- skróty haseł PBKDF2-HMAC-SHA256, osobne sole i liczbę iteracji;
- skróty jednorazowych kodów odzyskiwania;
- oznaczenia ukończonych zadań i daty operacji.

Aplikacja nie szyfruje całej bazy. Ochrona pliku zależy od uprawnień konta i zabezpieczeń systemu operacyjnego. Użytkownik powinien chronić konto systemowe, dysk oraz kopie zapasowe. Utrata jedynej kopii kodu odzyskiwania może uniemożliwić odzyskanie hasła bez usunięcia lokalnych danych konta.

## Zależności i łańcuch dostaw

Warunki wydania wymagają:

- restore z przypiętych lockfile w trybie `--locked-mode`;
- audytu wszystkich bezpośrednich i przechodnich pakietów NuGet od poziomu `low`;
- traktowania `NU1901`-`NU1904` jako błędów;
- zera znanych podatności w audycie przed publikacją;
- osobnego SBOM SPDX dla każdej paczki;
- `SHA256SUMS.txt` i attestation pochodzenia artefaktów;
- kontroli paczek pod kątem PDB, sekretów oraz snapshotów;
- pozytywnego inwentarza licencji i pochodzenia każdego paczkowanego zasobu.

Instrukcja weryfikacji pobranego artefaktu znajduje się w [docs/INSTALLATION.md](docs/INSTALLATION.md#sprawdzenie-integralności-i-pochodzenia). Brak podpisu kodu w wersji beta jest jawnym ograniczeniem, a suma lub attestation nie zastępuje ostrożności użytkownika.

## Aktualizacje bezpieczeństwa

Abituria nie ma automatycznej aktualizacji. Informacje o poprawkach są publikowane w [GitHub Releases](https://github.com/haribo841/Abituria/releases) i [CHANGELOG.md](CHANGELOG.md). Aktualizację wykonuje się przez zamknięcie aplikacji, zweryfikowanie nowej paczki i zastąpienie katalogu programu. Baza jest przechowywana osobno, ale przed aktualizacją należy wykonać jej kopię.

## Poza zakresem

- problemy występujące wyłącznie w zmodyfikowanych, nieoficjalnych buildach;
- globalne wyłączenie SmartScreen, Gatekeepera, antywirusa lub kontroli uprawnień;
- podatności zależne od utraconego dostępu do konta systemowego bez dodatkowego wpływu aplikacji;
- publiczne materiały bez jednoznacznie ustalonego prawa dystrybucji, które zgodnie z [inwentarzem pochodzenia](docs/CONTENT_PROVENANCE.md) nie mogą wejść do wydania.
