# Znane ograniczenia `0.9.0-beta.1`

Dokument opisuje jawny zakres pierwszego prerelease. Ograniczenie nie jest automatycznie błędem, ale każde zachowanie wykraczające poza poniższy opis należy zgłosić.

## Status publikacji

- Maszynowy inwentarz pochodzenia potwierdza dopuszczenie paczkowanych zasobów do publicznej redystrybucji jako części Abiturii. Publiczny GitHub Release nie został jeszcze utworzony i nadal wymaga pełnej checklisty wydawniczej.
- Podstawa redystrybucji materiałów CKE i odziedziczonych grafik wynika z oświadczenia właściciela zapisanego w `ASSET_RIGHTS_DECLARATION.md`; rzeczywiste autorstwo i źródła pozostają przypisane w `Content/provenance.json`. Przed tagiem należy ponowić twardą walidację dla dokładnego commita wydania.
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
- Brudnopis zadania jest sesyjny i nie jest zapisywany po opuszczeniu widoku.

## Treści edukacyjne

- Dostępnych jest 7 działów, a „Ciągi liczbowe” i „Liczby pierwsze” pozostają placeholderami.
- Dostępny jest arkusz matury poprawkowej 2021. Arkusze 2019, 2020 i matura podstawowa 2021 pozostają w przygotowaniu.
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
- Kalkulator funkcji kwadratowej wymaga rzeczywistych współczynników i `a != 0`.

## Wsparcie

Instrukcje diagnostyczne i wymagane dane zgłoszenia znajdują się w [SUPPORT.md](../SUPPORT.md). Podatności należy zgłaszać prywatnie zgodnie z [SECURITY.md](../SECURITY.md).
