# Abituria - szybkie uruchomienie

To jest samowystarczalna, przenośna paczka beta. Nie wymaga osobnej instalacji .NET. Paczka jest niepodpisana, dlatego przed uruchomieniem sprawdź jej sumę SHA-256 i attestation opublikowane razem z wydaniem.

## Windows 11 x64

Rozpakuj całe archiwum do nowego katalogu i uruchom `Abituria.exe`. Nie uruchamiaj programu bezpośrednio z archiwum. SmartScreen może wymagać jednorazowego potwierdzenia dla tej aplikacji - nie wyłączaj zabezpieczeń globalnie.

## Ubuntu 24.04 x64

Zainstaluj `libx11-6`, `libice6`, `libsm6` i `libfontconfig1`, rozpakuj całe archiwum, nadaj plikowi `Abituria` prawo wykonania poleceniem `chmod u+x ./Abituria`, a następnie uruchom `./Abituria`.

## macOS 15 Intel x64

Rozpakuj całe archiwum i otwórz `Abituria.app`. Paczka nie obsługuje Apple Silicon. Gatekeeper może wymagać jednorazowego użycia systemowej opcji „Open” dla tej aplikacji - nie wyłączaj zabezpieczeń globalnie.

## Dane i aktualizacja

Dane użytkownika są przechowywane poza katalogiem programu. Przy aktualizacji rozpakuj nową wersję do nowego katalogu, nie mieszaj plików wydań i wykonaj kopię katalogu danych przed pierwszym uruchomieniem.

Pełne instrukcje, wymagania, znane ograniczenia i pomoc:
https://haribo841.github.io/Abituria/

Licencję projektu zawiera `LICENSE`, a informacje o komponentach zewnętrznych znajdują się w `THIRD-PARTY-NOTICES.md` i katalogu `licenses`.
