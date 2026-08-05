# Znane ograniczenia `0.9.0-beta.1`

Dokument opisuje jawny zakres pierwszego prerelease. Ograniczenie nie jest automatycznie błędem, ale każde zachowanie wykraczające poza poniższy opis należy zgłosić.

## Status publikacji

- Maszynowy inwentarz pochodzenia ma obecnie `releaseEligible=true`. Rozszerzenia `ASSET_RIGHTS_DECLARATION.md` z 3 i 5 sierpnia 2026 r. obejmują arkusze CKE 2025 i 2026 na obu poziomach, zasady oceniania oraz dziewiętnaście autorskich implementacji wektorowych Avalonia.
- Rzeczywiste autorstwo i źródła są przypisane w `Content/provenance.json`. Przed tagiem trzeba ponownie przejść bramę `-RequireReleaseEligible` na dokładnym commicie wydania.
- Lokalnie zbudowany artefakt nie jest oficjalnym wydaniem, nawet jeśli przechodzi testy techniczne.

## Platformy i dystrybucja

- Oficjalny zakres beta obejmuje wyłącznie Windows 11 24H2 x64, Ubuntu 24.04 x64 i macOS 15 Intel x64.
- Apple Silicon, ARM64, 32-bit, inne dystrybucje Linuksa, aplikacja mobilna i wersja webowa nie są wspierane.
- Paczki są portable i self-contained. Nie ma instalatora, integracji z menu systemu ani automatycznego aktualizatora.
- Wydanie jest nietrimowane, bez AOT, ReadyToRun i single-file.
- Paczki nie są podpisane ani notaryzowane. SmartScreen i Gatekeeper mogą wyświetlać ostrzeżenia.
- Na Ubuntu wymagane są zewnętrzne biblioteki `libx11-6`, `libice6`, `libsm6` i `libfontconfig1`.

## Stan beta

- Minimalny obsługiwany rozmiar głównego okna to `720x520`. Breakpointy poprawiają Login, Start i kalkulator ogólny, ale bardzo duże skalowanie systemowe może nadal wymagać przewijania.
- Przegląd obejmuje wszystkie kryteria WCAG 2.2 A/AA, ale WCAG jest standardem treści internetowych, a repozytorium nie składa formalnej deklaracji zgodności aplikacji desktopowej. Szczegóły zawiera [ACCESSIBILITY_WCAG_AUDIT.md](ACCESSIBILITY_WCAG_AUDIT.md).
- Nie wykonano udokumentowanej weryfikacji bieżącej wersji z czytnikami ekranu, monitorem brajlowskim, sterowaniem głosowym, przełącznikami ani menedżerami haseł na wszystkich wspieranych systemach.
- Własny pasek tytułu i uchwyty rozmiaru używają natywnych operacji Avalonia, ale ich integracja z każdym menedżerem okien i technologią asystującą wymaga kontroli na rzeczywistych platformach.
- Motyw wysokiego kontrastu jest wariantem aplikacji i reaguje na systemową preferencję kontrastu. Nie zastępuje testu z każdym systemowym trybem wymuszonych kolorów i powiększenia.
- Nie ma synchronizacji między urządzeniami, konta internetowego, chmury ani współdzielenia postępu.
- Nie ma automatycznej kopii zapasowej. Użytkownik odpowiada za kopię lokalnej bazy.
- Cała baza SQLite nie jest szyfrowana. Hasła i kody odzyskiwania są przechowywane wyłącznie jako skróty, ale nazwy profili oraz postęp pozostają czytelne dla osoby mającej dostęp do pliku.
- Brudnopis zadania jest przechowywany przy nawigacji, ale tylko w pamięci procesu. Znika po zamknięciu aplikacji i nie jest częścią kopii bazy SQLite.

## Treści edukacyjne

- Kurs matematyki obejmuje Formułę 2023 na poziomie podstawowym i rozszerzonym; nie zawiera osobnego kursu Formuły 2015.
- Ćwiczenia kursowe są autorskie i nie odtwarzają zadań ani rozwiązań z informatorów CKE.
- Dostępne są matury główne 2025 i 2026 na poziomie podstawowym i rozszerzonym oraz arkusz poprawkowy 2021. Arkusze 2019, 2020 i zwykła matura podstawowa 2021 pozostają w przygotowaniu.
- Generator wykresów i kalkulator funkcji trygonometrycznych pozostają placeholderami.
- Renderer CSharpMath obsługuje używany podzbiór zapisu matematycznego, ale nie jest pełnym silnikiem TeX. Treści muszą przechodzić walidację delimitera i regresje renderowania.
- Materiały edukacyjne nie zastępują nauczyciela, oficjalnego informatora egzaminacyjnego ani aktualnych komunikatów CKE.

## Kalkulatory

- Kalkulator ogólny działa na liczbach rzeczywistych typu `double`.
- Nie obsługuje liczb zespolonych, procentów, silni, logarytmów, funkcji trygonometrycznych ani dowolnej precyzji.
- Notacja naukowa jest obsługiwana, ale wynik podlega precyzji i zakresowi IEEE 754 `double`.
- Wyrażenie ma limit 512 znaków i 64 poziomów zagnieżdżenia.
- Pierwiastek stopnia parzystego z liczby ujemnej, dzielenie przez zero, `0^0`, `NaN`, nieskończoność i wynik zespolony są kontrolowanymi błędami.
- Historia kalkulatora ogólnego ma maksymalnie 20 poprawnych pozycji i istnieje tylko do zamknięcia aplikacji. Błędy nie są zapisywane.
- Poprawny wynik kalkulatora ogólnego automatycznie zastępuje tekst w schowku systemowym. Niedostępny schowek nie blokuje obliczenia, ale integracja schowka oraz zachowanie okien `Owned` i `Topmost` nadal wymagają natywnego smoke testu na każdym wspieranym systemie.
- Kalkulator PiP ma pojedynczą instancję i nie przechowuje własnej historii niezależnej od pełnego kalkulatora. Oba widoki współdzielą `Ans`, historię i bieżącą sesję obliczeń.
- Kalkulator funkcji kwadratowej wymaga rzeczywistych współczynników i `a != 0`.

## Wsparcie

Instrukcje diagnostyczne i wymagane dane zgłoszenia znajdują się w [SUPPORT.md](../SUPPORT.md). Podatności należy zgłaszać prywatnie zgodnie z [SECURITY.md](../SECURITY.md).
