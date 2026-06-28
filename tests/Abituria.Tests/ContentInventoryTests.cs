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

        Assert.Equal(18, formulas.Articles.Count);
        Assert.Equal(Enumerable.Range(1, 18), formulas.Articles.Select(item => item.Order));
        Assert.All(formulas.Articles, item => Assert.NotEmpty(item.Blocks));
        Assert.Equal(4, chapters.Chapters.Count);
        var availableChapter = Assert.Single(chapters.Chapters, item => item.IsAvailable);
        Assert.NotEmpty(availableChapter.Blocks);
        Assert.Equal(8, availableChapter.Blocks.Count(block => block.Type == "image"));
        Assert.Equal(5, placeholders.Items.Count);
        Assert.Equal(35, exam.Exercises.Count);
        Assert.Equal(28, exam.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(7, exam.Exercises.Count(item => !item.IsMultipleChoice));
        Assert.Equal(Enumerable.Range(1, 35), exam.Exercises.Select(item => item.Number));
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
        var exercises = Read<ExamCatalog>("Content/exam-2021-correction.json").Exam.Exercises;
        foreach (var text in formulas.Articles.SelectMany(item => item.Blocks).Concat(chapters.Chapters.SelectMany(item => item.Blocks))
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
