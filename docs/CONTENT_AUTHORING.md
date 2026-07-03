# Treść i podgląd materiałów

Kod C# odpowiada za wczytanie, walidację i wyświetlenie treści. Długie opisy, materiały edukacyjne i wzory są przechowywane poza kodem:

- `Content/formulas.json` - tablice wzorów,
- `Content/chapters.json` - materiały działowe,
- `Content/exam-2021-correction.json` - zadania, odpowiedzi i podpowiedzi,
- `Content/ui-copy.json` - dłuższe statyczne objaśnienia interfejsu.

Tekst matematyczny używa ograniczników `\(` oraz `\)`. Każdy logiczny wiersz jest renderowany jako jeden przepływ inline. Historyczny znacznik listy `\(-\)` jest wyświetlany jako zwykły znak `-`, a pozostałe fragmenty matematyczne używają `MathView` na wspólnej linii bazowej.

Obrazy kontrolne listy z materiału „Potęgi i pierwiastki” znajdują się w `tests/Abituria.Tests/VisualBaselines` dla rozmiarów `960x640` i `1280x820`. Można je otworzyć bez kompilowania aplikacji.

Po świadomej zmianie wyglądu wzorca obraz aktualizuje polecenie:

```powershell
$env:UPDATE_VISUAL_BASELINES = '1'
dotnet test Abituria.sln --filter FullyQualifiedName~Mathematical_list_matches_the_reviewed_visual_baseline
Remove-Item Env:UPDATE_VISUAL_BASELINES
```

Aktualizację obrazu należy zawsze sprawdzić wzrokowo przed zatwierdzeniem.
