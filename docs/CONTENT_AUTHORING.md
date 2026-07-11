# Treść i podgląd materiałów

Kod C# odpowiada za wczytanie, walidację i wyświetlenie treści. Długie opisy, materiały edukacyjne i wzory są przechowywane poza kodem:

- `Content/formulas.json` - tablice wzorów,
- `Content/chapters.json` - materiały działowe,
- `Content/exam-2021-correction.json` - zadania, odpowiedzi i podpowiedzi,
- `Content/ui-copy.json` - dłuższe statyczne objaśnienia interfejsu.

Kanoniczne źródło treści i statusów wprowadzonych dla issue #35 znajduje się w `tools/seeds/issue-35-content.json`. Po jego edycji należy odtworzyć aktywne `Content/chapters.json` i `Content/roadmap.json`:

```powershell
powershell -ExecutionPolicy Bypass -File tools/Sync-Issue35Content.ps1
```

Test end-to-end uruchamia importer do pustego katalogu i wymaga semantycznej zgodności wyniku z aktywnymi katalogami, więc rozbieżność seeda, importera i aplikacji blokuje testy.

Tekst matematyczny używa ograniczników `\(` oraz `\)`. Każdy logiczny wiersz jest renderowany jako jeden przepływ inline. Historyczny znacznik listy `\(-\)` jest wyświetlany jako zwykły znak `-`, a pozostałe fragmenty matematyczne używają `MathView` na wspólnej linii bazowej.

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
