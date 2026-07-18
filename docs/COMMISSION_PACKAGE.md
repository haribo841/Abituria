# Pakiet dokumentacji technicznej dla komisji

Wersja: `0.9.0-beta.1`.

Ten indeks porządkuje materiał do przekazania komisji zgodnie z Issue #42. Źródłem prawdy są wersjonowane pliki Markdown i kod. PDF jest ich kontrolowanym skrótem przeznaczonym do wygodnego odczytu.

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
| użyteczność, instalacja niezależna i odbiór | `USABILITY_TEST_PROTOCOL.md`, `ACCEPTANCE_PROTOCOL.md` |
| ograniczenia | `KNOWN_LIMITATIONS.md` |
| licencja, autor i zmiany | `../LICENSE`, `../AUTHORS.md`, `../CHANGELOG.md` |
| pochodzenie treści i zasobów | `CONTENT_PROVENANCE.md`, `../Content/provenance.json` |

## PDF

Wygenerowany plik:

`output/pdf/Abituria-Technical-Documentation-0.9.0-beta.1.pdf`

Generator `tools/New-CommissionPdf.py` tworzy PDF z diagramem komponentów, aktualnym zrzutem z testu wizualnego i podsumowaniem weryfikacji. Po każdej zmianie dokumentacji należy uruchomić:

```powershell
& "C:\Users\Adam\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe" `
  tools/New-CommissionPdf.py
```

Po wygenerowaniu PDF musi zostać wyrenderowany do obrazów i sprawdzony wizualnie. Potwierdzenie tej kontroli jest częścią bieżącego zestawu dowodów.

## Zasady przekazania

1. Zbuduj i przetestuj dokładny commit przekazywany komisji.
2. Dołącz PDF, kod źródłowy albo jego trwały adres oraz instrukcję uruchomienia.
3. Dołącz wersję wdrożeniową tylko po przejściu bramy praw do redystrybucji.
4. Wypełnij testy niezależnej instalacji i użyteczności.
5. Zapisz decyzję osoby odpowiedzialnej w `ACCEPTANCE_PROTOCOL.md`.
6. Zapisz datę oraz kanał przekazania w zgłoszeniu lub w tym dokumencie przy wydaniu.

Pakiet jest technicznie przygotowany, ale nie można uznać go za przekazany ani zatwierdzony bez rzeczywistej decyzji komisji lub osoby odpowiedzialnej.
