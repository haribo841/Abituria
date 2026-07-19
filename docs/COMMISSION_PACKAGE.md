# Pakiet dokumentacji technicznej dla komisji

Wersja: `0.9.0-beta.1`.

Ten indeks porządkuje materiał do przekazania komisji zgodnie z Issues #42, #43, #44 i #45. Źródłem prawdy są wersjonowane pliki Markdown i kod. PDF jest ich kontrolowanym skrótem przeznaczonym do wygodnego odczytu.

## Zawartość techniczna

| Element | Plik |
| --- | --- |
| cel, zakres, interesariusze i ryzyka | `BUSINESS_ANALYSIS.md` |
| wymagania funkcjonalne, niefunkcjonalne i kryteria | `REQUIREMENTS.md` |
| architektura, moduły, klasy i nawigacja | `ARCHITECTURE.md` |
| instalacja, aktualizacja i dane użytkownika | `INSTALLATION.md` |
| instrukcja dla użytkownika | `USER_GUIDE.md` |
| zależności i komponenty zewnętrzne | `DEPENDENCIES.md`, `THIRD-PARTY-NOTICES.md` |
| testy oraz wyniki niefunkcjonalne | `TESTING.md` |
| użyteczność, instalacja niezależna i odbiór | `USABILITY_TEST_PROTOCOL.md`, `USABILITY_TEST_RESULTS.md`, `ACCEPTANCE_PROTOCOL.md` |
| osobne protokoły przyrostów I-IV | `acceptance/README.md`, `acceptance/INCREMENT_1_PROTOCOL.md`, `acceptance/INCREMENT_2_PROTOCOL.md`, `acceptance/INCREMENT_3_PROTOCOL.md`, `acceptance/INCREMENT_4_PROTOCOL.md` |
| legalne wydanie albo ograniczone przekazanie | `DELIVERY_PROTOCOL.md` |
| publiczna obrona i decyzja komisji | `DEFENSE_PROTOCOL.md` |
| kryteria akceptacji i oceny projektu | `EVALUATION_PROTOCOL.md` |
| ograniczenia | `KNOWN_LIMITATIONS.md` |
| licencja, autor i zmiany | `../LICENSE`, `../AUTHORS.md`, `../CHANGELOG.md` |
| pochodzenie treści i zasobów | `CONTENT_PROVENANCE.md`, `ASSET_RIGHTS_DECLARATION.md`, `../Content/provenance.json` |

## PDF

Wygenerowany plik:

`output/pdf/Abituria-Technical-Documentation-0.9.0-beta.1.pdf`

Generator `tools/New-CommissionPdf.py` tworzy PDF z diagramem komponentów, aktualnym zrzutem z testu wizualnego i podsumowaniem weryfikacji. Po każdej zmianie dokumentacji należy uruchomić:

```powershell
& "C:\Users\Adam\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe" `
  tools/New-CommissionPdf.py
```

Po wygenerowaniu PDF musi zostać wyrenderowany do obrazów i sprawdzony wizualnie. Potwierdzenie tej kontroli jest częścią bieżącego zestawu dowodów.

Ostatnia kontrola: 19 lipca 2026 r., 9 stron A4 wyrenderowanych przez Poppler przy 130 DPI. Sprawdzono wszystkie strony, w tym sekcję publicznej obrony M7 i nową macierz Issue #45; nie stwierdzono obcięć, nakładania tekstu, uszkodzonych znaków ani nieczytelnych tabel.

## Zasady przekazania

1. Zbuduj i przetestuj dokładny commit przekazywany komisji.
2. Dołącz PDF, kod źródłowy albo jego trwały adres oraz instrukcję uruchomienia.
3. Dołącz wersję wdrożeniową dopiero po pozytywnym wykonaniu bramy praw do redystrybucji dla przekazywanego commita.
4. Zachowaj oświadczenie właściciela z 19 lipca 2026 r. oraz retrospektywny zapis pomyślnych testów użyteczności i zatwierdzenia projektu na początku lutego 2022 r.
5. Zachowaj decyzje w protokołach I-IV i `ACCEPTANCE_PROTOCOL.md`.
6. Zapisz wybrany legalny wariant, znane dane przekazania i hash bieżącego pakietu w `DELIVERY_PROTOCOL.md`.
7. Zachowaj protokół publicznej obrony z 17 stycznia 2022 r., skład komisji, wynik oraz odnośnik do nagrania w `DEFENSE_PROTOCOL.md`.
8. Zachowaj macierz kryteriów, ograniczenia dowodowe, decyzję i gotowy komentarz zamykający Issue #45 w `EVALUATION_PROTOCOL.md`.
9. Zamknij Issues #43-#45 dopiero po utrwaleniu odpowiadających im protokołów w zdalnym repozytorium; publikację GitHub Release prowadź osobno według Issue #36.

Publiczna obrona odbyła się 17 stycznia 2022 r. przed komisją w składzie: dr Paweł M. Owsianny, prof. UAM dr hab. Jerzy Szymański i dr Tomasz Piłka. Pokazano działającą aplikację i przeprowadzono pytania oraz odpowiedzi. Nagranie dostępne przez link potwierdza przebieg do zapowiedzi narady, natomiast pozytywna decyzja i wynik bardzo dobry są retrospektywnym poświadczeniem właściciela. Podstawę opisują `DEFENSE_PROTOCOL.md` i `EVALUATION_PROTOCOL.md`.

Historyczny projekt został następnie przekazany w uzgodnionej formie i zatwierdzony przez prowadzącego na początku lutego 2022 r.; dokładny dzień i kanał tego osobnego przekazania nie zachowały się w dostępnych materiałach. Bieżący pakiet dokumentacyjny jest techniczną rekonstrukcją dowodów i nie jest równoznaczny z opublikowanym GitHub Release bieżącej migracji.
