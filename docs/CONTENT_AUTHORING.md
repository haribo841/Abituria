# Treść i podgląd materiałów

Kod C# odpowiada za wczytanie, walidację i wyświetlenie treści. Długie opisy, materiały edukacyjne i wzory są przechowywane poza kodem:

- `Content/formulas.json` - kuratorowana transkrypcja tablic CKE dla Formuły 2023,
- `Content/chapters.json` - wygenerowany katalog kursu Formuły 2023 w schemacie 3,
- `Content/course-exercises.json` - wygenerowane ćwiczenia kursowe,
- `Content/exam-2021-correction.json` - zadania, odpowiedzi i podpowiedzi,
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

Skrypt wymaga dokładnie `4/13/73/46/238/357`, sprawdza unikalność i długość identyfikatorów oraz zapisuje deterministyczny wynik do `Content/chapters.json`, `Content/course-exercises.json` i `docs/MATH_COURSE_2023_COVERAGE.md`. Diagramy kursu są autorskimi zasobami generowanymi poleceniem:

```powershell
pwsh -NoProfile -File tools/New-MathCourseDiagrams.ps1
```

Test end-to-end generuje katalog do pustego katalogu i wymaga semantycznej zgodności z aktywnymi plikami, więc rozbieżność seedów, generatora i aplikacji blokuje testy. Nie należy ręcznie edytować plików wygenerowanych.

`Content/formulas.json` jest kanonicznym katalogiem tablic. Wersja schematu 3 wymaga obiektu `source` z wydawcą, tytułem, adresem dokumentu, sumą SHA-256, datą publikacji i datą weryfikacji. Maszynową macierz sekcji i podpunktów zawiera `tools/seeds/formula-2023-coverage.json`. Pełny importer przyjmuje `-FormulaCatalogPath` i kopiuje ten katalog bez przetwarzania historycznych ekranów wzorów.

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
