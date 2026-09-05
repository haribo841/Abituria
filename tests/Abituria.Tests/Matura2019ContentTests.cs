using System.Text.Json;
using Abituria.Models;
using Abituria.Services;
using Avalonia.Headless.XUnit;

namespace Abituria.Tests;

public sealed class Matura2019ContentTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();
    private static readonly string[] ExpectedVectorDiagramIds =
    [
        "exam-mm19-p0-z08to10", "exam-mm19-p0-z14", "exam-mm19-p0-z15", "exam-mm19-p0-z19", "exam-mm19-p0-z21", "exam-mm19-p0-z29", "exam-mm19-p0-z31", "exam-mm19-p0-z34",
        "exam-mm19-r0-z03", "exam-mm19-r0-z09", "exam-mm19-r0-z10", "exam-mm19-r0-z11", "exam-mm19-r0-z15",
        "exam-mm19-p0p-z08", "exam-mm19-p0p-z14", "exam-mm19-p0p-z16", "exam-mm19-p0p-z21", "exam-mm19-p0p-z29", "exam-mm19-p0p-z34"
    ];
    private static readonly string[] ExpectedDiagramSourceIds =
    ["cke-2019-main-basic-exam", "cke-2019-main-extended-exam", "cke-2019-correction-basic-exam"];

    [Theory]
    [InlineData("Content/exam-2019-main-basic.json", "matura-maj-2019-podstawowa", "główna", "basic", 170, 34, 34, 50)]
    [InlineData("Content/exam-2019-main-extended.json", "matura-maj-2019-rozszerzona", "główna", "extended", 180, 15, 15, 50)]
    [InlineData("Content/exam-2019-correction-basic.json", "matura-poprawkowa-2019-podstawowa", "poprawkowa", "basic", 170, 34, 34, 50)]
    public void Formula_2015_exam_contracts_have_pinned_metadata_and_complete_exercises(
        string path,
        string id,
        string session,
        string level,
        int durationMinutes,
        int officialTaskCount,
        int progressItemCount,
        int maximumPoints)
    {
        var catalog = Read<ExamCatalog>(path);
        var exam = catalog.Exam;

        Assert.Equal(4, catalog.SchemaVersion);
        Assert.Equal(id, exam.Id);
        Assert.Equal(2019, exam.Year);
        Assert.Equal(session, exam.Session);
        Assert.Equal("2015", exam.Formula);
        Assert.Equal(level, exam.Level);
        Assert.Equal(durationMinutes, exam.DurationMinutes);
        Assert.Equal(officialTaskCount, exam.OfficialTaskCount);
        Assert.Equal(progressItemCount, exam.ProgressItemCount);
        Assert.Equal(progressItemCount, exam.Exercises.Count);
        Assert.Equal(maximumPoints, exam.MaximumPoints);
        Assert.Equal(maximumPoints, exam.Exercises.Sum(item => item.Points));
        Assert.Equal(Enumerable.Range(1, progressItemCount), exam.Exercises.Select(item => item.Number));
        Assert.Equal(Enumerable.Range(1, progressItemCount), exam.Exercises.Select(item => item.EffectiveOrder));
        Assert.All(exam.Exercises, exercise =>
        {
            Assert.StartsWith("mm19-", exercise.Id, StringComparison.Ordinal);
            Assert.InRange(exercise.Id.Length, 1, 79);
            Assert.Equal(id, exercise.ExamId);
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.True(exercise.Hints.Count >= 2);
            Assert.False(string.IsNullOrWhiteSpace(exercise.EffectiveSolution));
            Assert.False(string.IsNullOrWhiteSpace(exercise.ScoringCriteria));
            Assert.InRange(exercise.SourcePage, 2, 24);
            Assert.InRange(exercise.SolutionSourcePage, 2, 49);
        });

        Assert.Equal("2026-09-01", exam.Source.VerifiedOn);
        Assert.Equal(64, exam.Source.QuestionPaperSha256.Length);
        Assert.Equal(64, exam.Source.AnswerKeySha256.Length);
        Assert.All(
            new[] { exam.Source.QuestionPaperSha256, exam.Source.AnswerKeySha256 },
            hash => Assert.True(hash.All(character => char.IsAsciiHexDigit(character) && !char.IsLower(character))));
    }

    [Fact]
    public void Basic_main_exam_preserves_all_pages_answer_modes_and_numeric_answer()
    {
        var exam = Read<ExamCatalog>("Content/exam-2019-main-basic.json").Exam;
        int[] expectedPages =
        [
            2, 2, 2, 2, 2, 2, 4, 4, 4, 4, 6, 6, 6, 6, 8, 8, 8, 8, 10, 10, 10, 12, 12, 12, 12, 14, 15, 16, 17, 18, 19, 20, 22, 24
        ];

        Assert.Equal(expectedPages, exam.Exercises.Select(item => item.SourcePage));
        Assert.Equal(25, exam.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Single(exam.Exercises, item => item.IsNumeric);
        Assert.Equal(8, exam.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(0.36d, exam.Exercises.Single(item => item.Id == "mm19-p0-z30").ExpectedValue!.Value, 12);
        Assert.Equal([2, 2, 2, 2, 2, 2, 4, 4, 5], exam.Exercises.Skip(25).Select(item => item.Points));
    }

    [Fact]
    public void Extended_and_correction_exams_preserve_scoring_modes_and_numeric_answers()
    {
        var extended = Read<ExamCatalog>("Content/exam-2019-main-extended.json").Exam;
        var correction = Read<ExamCatalog>("Content/exam-2019-correction-basic.json").Exam;

        Assert.Equal(4, extended.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(3, extended.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(8, extended.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(952d, extended.Exercises.Single(item => item.Id == "mm19-r0-z05").ExpectedValue!.Value, 12);
        Assert.Equal(6666600d, extended.Exercises.Single(item => item.Id == "mm19-r0-z06").ExpectedValue!.Value, 12);
        Assert.Equal(2019d, extended.Exercises.Single(item => item.Id == "mm19-r0-z07").ExpectedValue!.Value, 12);

        Assert.Equal(25, correction.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(3, correction.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(6, correction.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(5d / 18d, correction.Exercises.Single(item => item.Id == "mm19-p0p-z30").ExpectedValue!.Value, 12);
        Assert.Equal(10d, correction.Exercises.Single(item => item.Id == "mm19-p0p-z32").ExpectedValue!.Value, 12);
        Assert.Equal(22d, correction.Exercises.Single(item => item.Id == "mm19-p0p-z33").ExpectedValue!.Value, 12);
    }

    [Fact]
    public void Nineteen_vector_definitions_are_referenced_by_2019_exams_and_are_approved()
    {
        var basic = Read<ExamCatalog>("Content/exam-2019-main-basic.json").Exam;
        var extended = Read<ExamCatalog>("Content/exam-2019-main-extended.json").Exam;
        var correction = Read<ExamCatalog>("Content/exam-2019-correction-basic.json").Exam;
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        using var provenance = JsonDocument.Parse(File.ReadAllText(Absolute("Content/provenance.json")));
        var groups = provenance.RootElement.GetProperty("assets").EnumerateArray().ToDictionary(
            item => item.GetProperty("id").GetString()!, StringComparer.Ordinal);
        var expectedIds = ExpectedVectorDiagramIds.Order(StringComparer.Ordinal).ToArray();
        var referencedIds = basic.Exercises.Concat(extended.Exercises).Concat(correction.Exercises)
            .SelectMany(item => item.DiagramIds)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var definitions = diagrams.Diagrams.Where(item => item.Id.StartsWith("exam-mm19-", StringComparison.Ordinal))
            .OrderBy(item => item.Id)
            .ToArray();

        Assert.Equal(expectedIds, referencedIds);
        Assert.Equal(expectedIds, definitions.Select(item => item.Id));
        Assert.All(definitions, definition =>
        {
            Assert.False(string.IsNullOrWhiteSpace(definition.AlternativeText));
            Assert.NotEmpty(definition.Primitives);
            Assert.Contains(definition.SourceId, ExpectedDiagramSourceIds);
        });
        Assert.False(provenance.RootElement.GetProperty("releaseEligible").GetBoolean());
        Assert.Equal("approved", groups["cke-2019-main-basic-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("approved", groups["cke-2019-main-extended-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("approved", groups["cke-2019-correction-basic-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("blocked", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
        DiagramCatalogValidator.Validate(diagrams);
    }

    [AvaloniaFact]
    public void Catalog_retains_2019_formula_2015_exams_when_2017_is_added()
    {
        var repository = new ContentRepository();

        Assert.Equal(46, repository.Exams.Count);
        Assert.Equal("matura-maj-2019-podstawowa", repository.Exams[31].Id);
        Assert.Equal("matura-maj-2019-rozszerzona", repository.Exams[32].Id);
        Assert.Equal("matura-poprawkowa-2019-podstawowa", repository.Exams[33].Id);
        Assert.Equal(1_281, repository.Exams.Sum(exam => exam.Exercises.Count));
    }

    private static T Read<T>(string relativePath) => JsonSerializer.Deserialize<T>(
        File.ReadAllText(Absolute(relativePath)),
        JsonOptions) ?? throw new InvalidDataException($"Nie można odczytać {relativePath}.");

    private static string Absolute(string relativePath) =>
        Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Abituria.csproj")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Nie znaleziono katalogu repozytorium.");
    }
}
