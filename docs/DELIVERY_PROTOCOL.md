# Protokół legalnego wydania albo przekazania

Wersja bieżącej dokumentacji: `0.9.0-beta.1`.

Data zapisania poświadczenia retrospektywnego: 19 lipca 2026 r.

## Stan prawny i techniczny

Właściciel projektu poświadczył 19 lipca 2026 r., że posiada prawa do zasobów projektu oraz że końcowa wersja projektu została przekazana w uzgodnionej formie i zaakceptowana przez prowadzącego na początku lutego 2022 r. Dokładny dzień, kanał, hash historycznego pakietu i zewnętrzne potwierdzenie odbioru nie zostały zachowane w aktualnym repozytorium.

Na podstawie tego poświadczenia historyczne przekazanie było autoryzowane przez właściciela praw i zostało przyjęte przez prowadzącego. Jest to podstawa formalnego zamknięcia Issue #43 z jawnym ograniczeniem archiwalnym. Dokument nie stanowi niezależnej opinii prawnej i nie odtwarza brakujących danych.

Stan bieżącego procesu publikacji jest odrębny. Techniczna wersja `0.9.0-beta.1` przeszła build, testy, SonarCloud, dokumentację i instalacyjny smoke test na trzech systemach, ale nie istnieje jeszcze tag ani publiczny GitHub Release tej wersji.

Deklaracja właściciela została zapisana w `ASSET_RIGHTS_DECLARATION.md` i przypisana jako dowód do trzech wcześniej nierozstrzygniętych grup w `Content/provenance.json`. Manifest ma obecnie `releaseEligible=true`, wszystkie paczkowane grupy mają status `approved`, a walidator zwykły oraz `Test-ContentProvenance.ps1 -RequireReleaseEligible` zakończyły się powodzeniem 19 lipca 2026 r.

Techniczna i formalna blokada proweniencji została więc usunięta. Nie oznacza to automatycznego utworzenia tagu ani publikacji GitHub Release.

## Dopuszczalne warianty przekazania

| Wariant | Zawartość | Stan 19 lipca 2026 r. | Znaczenie |
| --- | --- | --- | --- |
| H - historyczne uzgodnione przekazanie | końcowa wersja projektu przedstawiona prowadzącemu w historycznym cyklu realizacji | COMPLETED AND ACCEPTED - początek lutego 2022 r. | spełnia formalny warunek Issue #43 na podstawie poświadczenia właściciela; dokładny pakiet i kanał nie zostały zachowane |
| A - publiczne wydanie bieżącej aplikacji | kompletne paczki Windows, Ubuntu i macOS | READY FOR RELEASE PROCESS / NOT PUBLISHED | proweniencja jest zatwierdzona; pozostały tag, workflow, kontrola draftu i ręczna publikacja |
| B - nieuruchamialny kandydat dokumentacyjny | aktywne dokumenty, PDF, licencja, autor, notices, manifest proweniencji i sumy SHA-256 | PREPARED LOCALLY | opcjonalny, odtwarzalny pakiet dokumentacyjny bieżącego stanu; nie zastępuje aplikacji |
| C - oczyszczona uruchamialna wersja | aplikacja po usunięciu lub zastąpieniu wybranych zasobów | NOT REQUIRED FOR RIGHTS GATE | opcjonalna zmiana produktu, a nie obecny warunek prawny publikacji |

Wariant H jest historyczną, uzgodnioną formą przekazania i podstawą decyzji dla Issue #43. Warianty A-C dotyczą wyłącznie dalszego udostępniania bieżącej implementacji.

## Warunki publicznego wydania

Przed utworzeniem tagu albo GitHub Release bieżącej wersji muszą przejść wszystkie bramy z `RELEASE_PROCESS.md`, w tym:

```powershell
pwsh -NoProfile -File tools/Test-ContentProvenance.ps1 -RequireReleaseEligible
```

Dowód praw został przypisany do odpowiednich grup, `releaseEligible=true`, a oba warianty walidatora przeszły 19 lipca 2026 r. Przed publikacją należy powtórzyć pełną bramę na dokładnym commicie przeznaczonym do tagowania i zachować log workflow. Nie wolno omijać tej kontroli przez ręczne pakowanie.

Publiczne wydanie `0.9.0-beta.1` ma status `PENDING`. Ten protokół nie twierdzi, że GitHub Release istnieje.

## Manifest przekazywanego pakietu

Historyczny manifest wariantu H, hash pakietu i dokładna rewizja nie zostały zachowane. Brakujące dane są ograniczeniem archiwalnym i nie są rekonstruowane.

Skrypt `tools/New-CommissionHandoffPackage.ps1` tworzy lokalnie odtwarzalny kandydat wariantu B w `artifacts/handoff`. Pakiet zawiera:

- aktywne pliki Markdown dokumentacji, bez `docs/legacy`;
- PDF dla komisji;
- `README.md`, `LICENSE`, `AUTHORS.md`, `CHANGELOG.md`, `THIRD-PARTY-NOTICES.md`, `SECURITY.md` i `SUPPORT.md`;
- `Content/provenance.json`;
- generowany `HANDOFF-MANIFEST.json` z listą plików i SHA-256;
- zewnętrzny plik `SHA256SUMS.txt` dla archiwum.

Archiwum wariantu B nie zawiera wykonywalnej aplikacji, bibliotek, źródeł treści egzaminacyjnych, obrazów aplikacji, katalogu `.git`, historii Git ani plików `docs/legacy`.

## Wyłączone zasoby i skutki funkcjonalne

Wariant B celowo zawiera wyłącznie dokumentację, dlatego nie obejmuje zasobów wykonawczych i edukacyjnych niezależnie od ich zatwierdzonego statusu prawnego. W szczególności nie zawiera `Content/exam-2021-correction.json`, `img/`, `tools/Import-LegacyContent.ps1`, DLL, EXE, `.app` ani pełnego źródła projektu.

Skutek: pakiet wariantu B nie jest uruchamialny i nie może być przedstawiany jako publiczne wydanie, instalator albo kompletna paczka Abiturii. Dostarcza wyłącznie materiał do oceny technicznej i formalnej.

Ograniczenie wariantu B nie jest przypisywane historycznemu wariantowi H. Zawartości historycznego przekazania nie można obecnie zweryfikować plik po pliku.

## Data, kanał i odbiorca

| Wariant | Data przekazania | Wersja i SHA-256 | Kanał | Odbiorca | Potwierdzenie odbioru |
| --- | --- | --- | --- | --- | --- |
| H - historyczne przekazanie | początek lutego 2022 r.; dokładny dzień nie został zachowany | nie zachowano | nie zachowano | prowadzący projekt; imię i nazwisko nie zostały zapisane w repozytorium | projekt zaakceptowany; poświadczenie właściciela zapisane 19 lipca 2026 r. |
| B - bieżący kandydat dokumentacyjny | nie przekazano | generowane przy utworzeniu pakietu | do ustalenia | do ustalenia | brak - wariant opcjonalny |

## Zgoda prowadzącego lub odbiorcy

| Wybrany wariant | Decyzja | Data | Osoba | Dowód zachowany w repozytorium | Ograniczenie |
| --- | --- | --- | --- | --- | --- |
| H | ZAAKCEPTOWANO | początek lutego 2022 r.; dokładny dzień nie został zachowany | prowadzący projekt; danych nie zachowano | retrospektywne poświadczenie właściciela z 19 lipca 2026 r. | brak historycznego podpisu lub trwałego odnośnika |
| A | PENDING - nie opublikowano | nie dotyczy | opiekun bieżącego wydania | brak, ponieważ GitHub Release nie istnieje | proweniencja PASS; wymaga tagu, workflow, kontroli draftu i publikacji |

## Decyzja końcowa i dowody

Decyzja dla Issue #43: `ACCEPTED - READY TO CLOSE`.

Podstawa:

- testy użyteczności z uczestnikami zakończyły się powodzeniem według poświadczenia właściciela;
- prowadzący zaakceptował projekt na początku lutego 2022 r.;
- historyczna forma przekazania została uzgodniona i przyjęta;
- właściciel poświadczył posiadanie praw do zasobów objętych historycznym przekazaniem;
- dokładny dzień, kanał, hash, uczestnicy i podpisy nie zostały zachowane i są jawnie oznaczone jako ograniczenie archiwalne.

Publiczne wydanie bieżącej wersji pozostaje osobnym działaniem `PENDING`. Nie utworzono tagu ani GitHub Release. Brama `-RequireReleaseEligible` przeszła 19 lipca 2026 r., ale musi zostać ponowiona wraz z pełnym zestawem kontroli na dokładnym commicie wydania.
