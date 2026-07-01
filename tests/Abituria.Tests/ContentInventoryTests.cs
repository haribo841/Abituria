using System.Text.Json;
using System.Text.RegularExpressions;
using Abituria.Models;
using CSharpMath.Avalonia;

namespace Abituria.Tests;

public sealed class ContentInventoryTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Migrated_content_has_expected_inventory()
    {
        var formulas = Read<FormulaCatalog>("Content/formulas.json");
        var chapters = Read<ChapterCatalog>("Content/chapters.json");
        var exam = Read<ExamCatalog>("Content/exam-2021-correction.json").Exam;
        var placeholders = Read<PlaceholderCatalog>("Content/placeholders.json");
        var roadmap = Read<RoadmapCatalog>("Content/roadmap.json");

        Assert.Equal(18, formulas.Articles.Count);
        Assert.NotEmpty(formulas.Introduction);
        Assert.Equal(Enumerable.Range(1, 18), formulas.Articles.Select(item => item.Order));
        Assert.All(formulas.Articles, item => Assert.NotEmpty(item.Blocks));
        Assert.Equal(7, chapters.Chapters.Count);
        Assert.NotEmpty(chapters.Introduction);
        var availableChapter = Assert.Single(chapters.Chapters, item => item.IsAvailable);
        Assert.Equal("vectors", availableChapter.Id);
        Assert.NotEmpty(availableChapter.Blocks);
        Assert.Equal(8, availableChapter.Blocks.Count(block => block.Type == "image"));
        Assert.Equal(7, placeholders.Items.Count);
        Assert.Equal(
            new[] { "exercise-set-e", "general-calculator", "graph-generator", "matura-2019", "matura-2020", "matura-2021", "trigonometric-calculator" },
            placeholders.Items.Select(item => item.Id).Order());
        Assert.Equal(
            new[] { "Ciągi liczbowe", "Funkcja kwadratowa", "Liczby pierwsze", "Logarytmy", "Rachunek zbiorów i logika", "Równania i nierówności" },
            chapters.Chapters.Where(item => !item.IsAvailable).Select(item => item.Title).Order());
        Assert.Equal(17, exam.Topics.Count);
        Assert.NotEmpty(exam.Introduction);
        Assert.NotEmpty(exam.TopicIntroduction);
        Assert.Equal(35, exam.Exercises.Count);
        Assert.Equal(28, exam.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(7, exam.Exercises.Count(item => !item.IsMultipleChoice));
        Assert.Equal(Enumerable.Range(1, 35), exam.Exercises.Select(item => item.Number));
        Assert.Contains(roadmap.Items, item => item.Status == RoadmapStatus.Migrated);
        Assert.Contains(roadmap.Items, item => item.Status == RoadmapStatus.Planned);
        Assert.Contains(roadmap.Items, item => item.Status == RoadmapStatus.Superseded);
        Assert.Contains(roadmap.Items, item => item.Id == "graph-generator" && item.Status == RoadmapStatus.Planned);
        Assert.Contains(roadmap.Items, item => item.Id == "trigonometric-calculator" && item.Status == RoadmapStatus.Planned);
        Assert.Contains(roadmap.Items, item => item.Id == "formula-editor-prototype" && item.Status == RoadmapStatus.Superseded);
        Assert.Contains(placeholders.Items.Single(item => item.Id == "matura-2021").Blocks,
            block => block.Text is not null && block.Text.Contains("79%", StringComparison.Ordinal) && block.Text.Contains("56%", StringComparison.Ordinal));

        var assetCount = Directory.GetFiles(Path.Combine(RepositoryRoot, "img"), "*", SearchOption.AllDirectories).Length
            + Directory.GetFiles(Path.Combine(RepositoryRoot, "fonts"), "*", SearchOption.AllDirectories).Length;
        Assert.Equal(92, assetCount);
    }

    [Fact]
    public void Topic_mapping_covers_every_exercise_exactly_once()
    {
        var exam = Read<ExamCatalog>("Content/exam-2021-correction.json").Exam;
        var mappedNumbers = exam.Topics.SelectMany(topic => topic.ExerciseNumbers).ToArray();

        Assert.Equal(35, mappedNumbers.Length);
        Assert.Equal(Enumerable.Range(1, 35), mappedNumbers.Order());
        Assert.Equal(35, mappedNumbers.Distinct().Count());
        Assert.All(exam.Exercises, exercise => Assert.Contains(exam.Topics, topic => topic.Id == exercise.TopicId && topic.ExerciseNumbers.Contains(exercise.Number)));
    }

    [Fact]
    public void Cke_contract_and_known_legacy_defects_are_corrected()
    {
        var exam = Read<ExamCatalog>("Content/exam-2021-correction.json").Exam;
        var expectedKey = new[] { 4, 3, 1, 1, 4, 3, 2, 3, 1, 3, 4, 4, 3, 2, 3, 2, 4, 1, 4, 2, 4, 1, 3, 3, 1, 2, 3, 2 };

        Assert.Equal("EMAP-P0-100-2108", exam.Source.DocumentCode);
        Assert.Equal("2026-06-30", exam.Source.VerifiedOn);
        Assert.Equal(expectedKey, exam.Exercises.Take(28).Select(item => item.CorrectOption!.Value));
        Assert.All(exam.Exercises, item =>
        {
            Assert.Equal("CKE EMAP-P0-100-2108", item.VerificationSource);
            Assert.InRange(item.SourcePage, 2, 22);
        });

        var task7 = exam.Exercises.Single(item => item.Number == 7);
        Assert.Equal(new[] { "\\(g(x)=-2x+2\\)", "\\(g(x)=-2x\\)", "\\(g(x)=-2x+6\\)", "\\(g(x)=-2x+8\\)" }, task7.Options);
        var task17 = exam.Exercises.Single(item => item.Number == 17);
        Assert.Contains("BSC", task17.Prompt);
        Assert.Equal("\\(40^{\\circ}\\)", task17.Options[3]);
        Assert.Contains("img/mp21z17.png", task17.Assets);
        Assert.DoesNotContain("x=-2", string.Join(' ', task17.Hints), StringComparison.Ordinal);
        var task18 = exam.Exercises.Single(item => item.Number == 18);
        Assert.Contains("\\angle BOC", task18.Prompt, StringComparison.Ordinal);
        var task19 = exam.Exercises.Single(item => item.Number == 19);
        Assert.Contains("\\angle ACB", task19.Prompt, StringComparison.Ordinal);
        var task22 = exam.Exercises.Single(item => item.Number == 22);
        Assert.Contains(task22.Hints, hint => hint.Contains("prostej prostopadłej", StringComparison.Ordinal));
        var task28 = exam.Exercises.Single(item => item.Number == 28);
        Assert.Contains(task28.Hints, hint => hint.Contains("35x+40", StringComparison.Ordinal));
        var task29 = exam.Exercises.Single(item => item.Number == 29);
        Assert.Equal("\\(\\text{Odpowiedź: } x\\in(-\\infty,-1]\\cup[5,+\\infty)\\)", task29.RevealedAnswer);
        Assert.Contains("x=-\\frac{1}{2}", exam.Exercises.Single(item => item.Number == 30).RevealedAnswer, StringComparison.Ordinal);
        Assert.Contains("(a-2b)^2+b^2", exam.Exercises.Single(item => item.Number == 31).RevealedAnswer, StringComparison.Ordinal);
        Assert.Equal("\\(|BD|=12\\)", exam.Exercises.Single(item => item.Number == 32).RevealedAnswer);
        Assert.Contains("\\frac{16}{3}", exam.Exercises.Single(item => item.Number == 33).RevealedAnswer, StringComparison.Ordinal);
        Assert.Contains("\\frac{1}{9}", exam.Exercises.Single(item => item.Number == 34).RevealedAnswer, StringComparison.Ordinal);
        Assert.Contains("x=0", exam.Exercises.Single(item => item.Number == 35).RevealedAnswer, StringComparison.Ordinal);
        Assert.Contains("q =-2", exam.Exercises.Single(item => item.Number == 35).RevealedAnswer, StringComparison.Ordinal);

        var allHints = string.Join(' ', exam.Exercises.SelectMany(item => item.Hints));
        Assert.DoesNotContain("Rozwiąż równanie. Zacznij", allHints, StringComparison.Ordinal);
        Assert.DoesNotContain("kawadratowej", allHints, StringComparison.Ordinal);
        Assert.DoesNotContain("czegokąty", allHints, StringComparison.Ordinal);
        Assert.DoesNotContain("Stosunek ku białych", allHints, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("Analiza biznesowa.txt", "7216E37D9565480CE220590FDAC7B7BCAA509650D749A68777A78E3BC534E3CC")]
    [InlineData("Definicja projektu.txt", "8D690148DDE6EDE54EA53C6CEBDBD5A3365C38C3AD85FD2C54813756BFB804E4")]
    [InlineData("Implementacja.txt", "500D2EB2695DDC18842441916D97CC3ECD98151DFC612044E493805E38BB798F")]
    [InlineData("Opis struktury systemu.txt", "66E6B14916D1DE78A53B36F0FC24CA791D6DF5C80A0FE3260FFB105237E508F3")]
    [InlineData("Uzupełnić Treść działów matematyki.txt", "8EBD73B3FC283F683C1D0A40C850D70874EE4AACE6B67BE792AB57D7F81F59CF")]
    [InlineData("LICENSE-2022-Ich-Troje.txt", "F36B81A276CF8EC8889310086D7DF99667AA0A06BB348328E99F65A09A7DDCC5")]
    public void Legacy_originals_are_preserved_byte_for_byte(string fileName, string expectedHash)
    {
        var path = Path.Combine(RepositoryRoot, "docs", "legacy", "originals", fileName);
        var actualHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(path)));

        Assert.Equal(expectedHash, actualHash);
    }

    [Fact]
    public void Every_exercise_has_complete_answer_contract()
    {
        var exercises = Read<ExamCatalog>("Content/exam-2021-correction.json").Exam.Exercises;
        foreach (var exercise in exercises)
        {
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.NotEmpty(exercise.Hints);
            if (exercise.IsMultipleChoice)
            {
                Assert.Equal(4, exercise.Options.Count);
                Assert.InRange(exercise.CorrectOption ?? 0, 1, 4);
                Assert.All(exercise.Options, option => Assert.False(string.IsNullOrWhiteSpace(option)));
            }
            else
            {
                Assert.Empty(exercise.Options);
                Assert.Null(exercise.CorrectOption);
                Assert.False(string.IsNullOrWhiteSpace(exercise.RevealedAnswer));
            }
        }
    }

    [Fact]
    public void Referenced_images_exist_and_legacy_latex_typos_are_removed()
    {
        var formulas = Read<FormulaCatalog>("Content/formulas.json");
        var chapters = Read<ChapterCatalog>("Content/chapters.json");
        var exam = Read<ExamCatalog>("Content/exam-2021-correction.json").Exam;
        var assets = formulas.Articles.SelectMany(item => item.Blocks)
            .Concat(chapters.Chapters.SelectMany(item => item.Blocks))
            .Where(block => block.Type == "image")
            .Select(block => block.Asset!)
            .Concat(exam.Exercises.SelectMany(item => item.Assets))
            .Distinct(StringComparer.OrdinalIgnoreCase);
        Assert.All(assets, asset => Assert.True(File.Exists(Path.Combine(RepositoryRoot, asset.Replace('/', Path.DirectorySeparatorChar))), asset));

        var allJson = string.Join('\n', Directory.GetFiles(Path.Combine(RepositoryRoot, "Content"), "*.json").Select(File.ReadAllText));
        Assert.DoesNotContain("/cdot", allJson, StringComparison.Ordinal);
        Assert.DoesNotContain("/text", allJson, StringComparison.Ordinal);
        Assert.DoesNotContain("\\tg", allJson, StringComparison.Ordinal);
        Assert.DoesNotContain("1=-\\frac{\\Delta}", allJson, StringComparison.Ordinal);
        Assert.DoesNotContain("x_1+x_1=", allJson, StringComparison.Ordinal);
        Assert.DoesNotContain("x_1\\cdot{x_1}=", allJson, StringComparison.Ordinal);
        Assert.DoesNotContain("B=(x_1,y_1)", allJson, StringComparison.Ordinal);
    }

    [Fact]
    public void Every_math_expression_is_supported_by_renderer()
    {
        foreach (var expression in ReadRichTexts().SelectMany(text => Regex.Matches(text, @"\\\((.*?)\\\)", RegexOptions.Singleline)
                     .Select(match => match.Groups[1].Value)))
        {
            var painter = new MathPainter { LaTeX = expression };
            Assert.True(string.IsNullOrWhiteSpace(painter.ErrorMessage), $"{expression}: {painter.ErrorMessage}");
        }
    }

    private static IEnumerable<string> ReadRichTexts()
    {
        var formulas = Read<FormulaCatalog>("Content/formulas.json");
        var chapters = Read<ChapterCatalog>("Content/chapters.json");
        var exam = Read<ExamCatalog>("Content/exam-2021-correction.json").Exam;
        var exercises = exam.Exercises;
        foreach (var text in formulas.Introduction.Concat(formulas.Articles.SelectMany(item => item.Blocks))
                     .Concat(chapters.Introduction).Concat(chapters.Chapters.SelectMany(item => item.Blocks))
                     .Concat(exam.Introduction).Concat(exam.TopicIntroduction)
                     .Select(block => block.Text).Where(text => !string.IsNullOrWhiteSpace(text)))
            yield return text!;
        foreach (var exercise in exercises)
        {
            yield return exercise.Prompt;
            foreach (var option in exercise.Options) yield return option;
            foreach (var hint in exercise.Hints) yield return hint;
            if (!string.IsNullOrWhiteSpace(exercise.RevealedAnswer)) yield return exercise.RevealedAnswer;
        }
    }

    private static T Read<T>(string relativePath) => JsonSerializer.Deserialize<T>(
        File.ReadAllText(Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar))), JsonOptions)!;

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Abituria.csproj"))) directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Nie znaleziono katalogu repozytorium.");
    }
}
