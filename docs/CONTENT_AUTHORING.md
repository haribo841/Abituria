# Treść i podgląd materiałów

Kod C# odpowiada za wczytanie, walidację i wyświetlenie treści. Długie opisy, materiały edukacyjne i wzory są przechowywane poza kodem:

- `Content/formulas.json` - kuratorowana transkrypcja tablic CKE dla Formuły 2023,
- `Content/chapters.json` - wygenerowany katalog kursu Formuły 2023 w schemacie 4,
- `Content/course-exercises.json` - wygenerowane ćwiczenia kursowe,
- `Content/official-course-examples.json` - osobna transkrypcja 97 przykładów z dwóch informatorów CKE,
- `Content/exams.json` - indeks aktywnych arkuszy i 17 wspólnych tematów w schemacie 1,
- `Content/exam-2025-main-basic.json` - matura główna 2025 PP w schemacie 4,
- `Content/exam-2025-main-extended.json` - matura główna 2025 PR w schemacie 4,
- `Content/exam-2025-correction-basic.json` - matura poprawkowa 2025 PP w schemacie 4,
- `Content/exam-2024-main-basic.json` - matura główna 2024 PP w schemacie 4,
- `Content/exam-2024-main-extended.json` - matura główna 2024 PR w schemacie 4,
- `Content/exam-2024-correction-basic.json` - matura poprawkowa 2024 PP w schemacie 4,
- `Content/exam-2023-main-basic.json` - matura główna 2023 PP w schemacie 4,
- `Content/exam-2023-correction-basic.json` - matura poprawkowa 2023 PP w schemacie 4,
- `Content/exam-2023-main-extended.json` - matura główna 2023 PR w schemacie 4,
- `Content/exam-2022-main-basic.json` - matura główna 2022 PP w Formule 2015 w schemacie 4,
- `Content/exam-2022-main-extended.json` - matura główna 2022 PR w Formule 2015 w schemacie 4,
- `Content/exam-2022-correction-basic.json` - matura poprawkowa 2022 PP w Formule 2015 w schemacie 4,
- `Content/exam-2026-main-basic.json` - matura główna 2026 PP w schemacie 4,
- `Content/exam-2026-main-extended.json` - matura główna 2026 PR w schemacie 4,
- `Content/exam-2015-*.json` - pierwsza sesja Formuły 2015: PP, PR i poprawkowa PP w schemacie 4,
- `Content/exam-2023-f2015-*.json` do `Content/exam-2025-f2015-*.json` - równoległe sesje F2015: PP, PR i poprawkowa PP w schemacie 4,
- `Content/exam-2026-f2015-*.json` - równoległa sesja F2015: PP i PR w schemacie 4,
- `Content/exam-2021-correction.json` - zgodny wstecznie arkusz poprawkowy 2021 w schemacie 3,
- `Content/diagrams.json` - 249 aktywnych definicji wektorowych w schemacie 1,
- `Content/placeholders.json` - treść ekranów zaplanowanych lub zastąpionych,
- `Content/roadmap.json` - opis planu rozwoju,
- `Content/ui-copy.json` - dłuższe statyczne objaśnienia interfejsu.

Kanoniczne źródło treści historycznych wprowadzonych dla issue #35 znajduje się w `tools/seeds/issue-35-content.json`. Jest ono włączane do lekcji kursu bez zmiany identyfikatorów. Kompatybilny wrapper zachowuje dotychczasowy punkt wejścia:

```powershell
pwsh -NoProfile -File tools/Sync-Issue35Content.ps1
```

Źródłem prawdy dla pełnego kursu jest katalog `tools/seeds/math-course`: metadane grup, obszarów, lekcji i źródeł znajdują się w `catalog.json`, a 119 wymagań jest podzielonych między cztery pliki etapów. Po zmianie seeda należy odtworzyć oba katalogi aplikacji i macierz pokrycia:

```powershell
pwsh -NoProfile -File tools/New-MathCourseContent.ps1
```

Skrypt wymaga dokładnie `4/13/73/46/238/357`, sprawdza unikalność i długość identyfikatorów oraz zapisuje deterministyczny wynik do `Content/chapters.json`, `Content/course-exercises.json` i `docs/MATH_COURSE_2023_COVERAGE.md`.

Oficjalne przykłady nie należą do liczników autorskich. Importer wymaga oryginalnych plików PDF o przypiętych sumach SHA-256 i odrzuca plik innej wersji:

```powershell
python tools/Import-CkeInformerExamples.py `
  --basic-pdf <informator-podstawowy.pdf> `
  --extended-pdf <informator-rozszerzony.pdf> `
  --output Content/official-course-examples.json
```

Importer pobiera wyłącznie główne zestawy przykładów: zadania 1-66 na stronach PDF 12-138 informatora podstawowego oraz zadania 1-31 na stronach 12-106 informatora rozszerzonego. Nie dubluje późniejszych powtórzeń przeznaczonych dla zdających ze specjalnymi potrzebami. Każdy rekord zachowuje numer, punktację, strony, mapowanie wymagań, pełną transkrypcję oraz opisy informacji wizualnej. Po zmianie katalogu trzeba osobno ocenić jego status w `Content/provenance.json`; test techniczny nie zastępuje deklaracji praw do publicznej redystrybucji.

Aktywny katalog 249 diagramów jest kanonicznym plikiem `Content/diagrams.json` i jest walidowany przez testy kontraktu. `tools/New-DiagramCatalog.ps1` odtwarza starszy bazowy zestaw 76 definicji. Skrypt odmawia nadpisania aktywnego katalogu, aby nie usunąć później dodanych diagramów. Pełna konsolidacja jego źródeł pozostaje osobnym zadaniem utrzymaniowym.

Generator zapisuje wyłącznie dane prymitywów wektorowych i nie tworzy PNG. Historyczne rastry znajdują się poza aplikacją w `docs/legacy/originals/images/`; ich mapowanie i sumy odtwarza `tools/New-LegacyImageArchiveManifest.ps1`.

## Arkusze maturalne

`Content/exams.json` określa kolejność, aktywność, ścieżkę pliku i wspólne tematy. Każdy aktywny arkusz musi mieć unikalny identyfikator, a każdy `LearningExercise` musi wskazywać ten sam `ExamId`, jeden z 17 tematów i globalnie unikalny identyfikator krótszy niż 80 znaków. Nie wolno zmieniać istniejących identyfikatorów `mp21-*`.

Schemat 4 arkuszy 2015-2026 przechowuje formułę, poziom, czas, maksimum punktów, oficjalną liczbę zadań i liczbę jednostek postępu. `DisplayNumber` zachowuje oznaczenia takie jak `12.1` i `13.2`, `GroupId` łączy części jednego oficjalnego zadania, a `Points`, `Solution`, `ScoringCriteria` i `SolutionSourcePage` odtwarzają umowę oceniania. Tryb `compound` zawiera co najmniej dwie części `multipleChoice`, `numeric` lub `text`.

Źródła i liczniki matur opisują macierze roczne oraz [MATURA_FORMULA_2015_ARCHIVE_COVERAGE.md](MATURA_FORMULA_2015_ARCHIVE_COVERAGE.md). Przed dodaniem lub zmianą treści źródłowej trzeba zweryfikować pliki, adresy i SHA-256, zaktualizować testy kontraktu oraz ocenić `Content/provenance.json`. Brak osobistej podstawy redystrybucji oznacza status `blocked`; nie wolno zmieniać go na `approved` na podstawie samego przejścia testów technicznych.

Testy kontraktu walidują aktywny katalog `Content/diagrams.json`. Nie należy uruchamiać starszego generatora bez jawnej ścieżki wyjściowej ani traktować jego wyniku jako kompletnego katalogu aplikacji.

`Content/formulas.json` jest kanonicznym katalogiem tablic. Wersja schematu 4 wymaga obiektu `source` z wydawcą, tytułem, adresem dokumentu, sumą SHA-256, datą publikacji i datą weryfikacji oraz używa identyfikatorów diagramów zamiast ścieżek obrazów. Maszynową macierz sekcji i podpunktów zawiera `tools/seeds/formula-2023-coverage.json`. Pełny importer przyjmuje `-FormulaCatalogPath` i kopiuje ten katalog bez przetwarzania historycznych ekranów wzorów.

Tekst matematyczny używa ograniczników `\(` oraz `\)`. Każdy fizyczny wiersz musi zawierać kompletne, niezagnieżdżone i niepuste pary ograniczników. Nie wolno otwierać wzoru w jednym wierszu lub bloku i zamykać go w następnym. `TextView` oraz jego `TextPainter` renderują cały poprawny wiersz jako jeden przepływ tekstu i matematyki. Historyczny znacznik listy `\(-\)` jest wcześniej normalizowany do zwykłego znaku `-`.

Niepoprawny wiersz jest wyświetlany w całości jako zwykły tekst i nie trafia nawet częściowo do CSharpMath. Test inwentarza sprawdza ten kontrakt dla wszystkich treści faktycznie kierowanych do `RichContentView`. Importer usuwa fizyczne końce wiersza występujące wewnątrz historycznych wzorów, zachowując polecenia LaTeX `\\` odpowiedzialne za zamierzone łamanie wzoru.

Tabele w bloku `richText` używają składni z pionowymi kreskami i obowiązkowym wierszem separatora. Renderer tworzy z niej prawdziwy układ kolumnowy, a treść pozostaje czytelna bez kompilacji:

```text
Kolumna A | Kolumna B
--- | ---
wartość 1 | wartość 2
```

Znak `|` wewnątrz `\(...\)`, na przykład w `\(|x|\)`, pozostaje częścią wzoru. Poza matematyką dosłowną kreskę pionową zapisuje się jako `\|`. Wiersz separatora przyjmuje co najmniej trzy myślniki na kolumnę i opcjonalny pojedynczy dwukropek na każdym krańcu.

Obrazy kontrolne listy z materiału „Potęgi i pierwiastki” znajdują się w `tests/Abituria.Tests/VisualBaselines` dla rozmiarów `960x640` i `1280x820`. Można je otworzyć bez kompilowania aplikacji.

Po świadomej zmianie wyglądu wzorca obraz aktualizuje polecenie:

```powershell
$env:UPDATE_VISUAL_BASELINES = '1'
dotnet test Abituria.sln --filter FullyQualifiedName~Mathematical_list_matches_the_reviewed_visual_baseline
Remove-Item Env:UPDATE_VISUAL_BASELINES
```

Aktualizację obrazu należy zawsze sprawdzić wzrokowo przed zatwierdzeniem.
