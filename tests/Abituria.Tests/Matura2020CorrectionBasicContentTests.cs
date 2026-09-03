using System.Text.Json;
using Abituria.Models;
using Abituria.Services;

namespace Abituria.Tests;

public sealed class Matura2020CorrectionBasicContentTests
{
    private const string ExamId = "matura-poprawkowa-2020-podstawowa";
    private const string PaperUrl = "https://arkusze.pl/maturalne/matematyka-2020-wrzesien-poprawkowa-podstawowa.pdf";
    private const string RulesUrl = "https://arkusze.pl/maturalne/matematyka-2020-wrzesien-poprawkowa-podstawowa-odpowiedzi.pdf";
    private const string PaperHash = "C14040F3142B2E922EF6ED84F5647DD40EBB824AD616C048AC76F0BC37CDFFB3";
    private const string RulesHash = "A708976C4C495F922369C7EE3098BFD8EFF7AD2FCA037631824B26772596C9D7";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Formula_2015_correction_exam_has_exactly_34_tasks_34_progress_items_and_50_points()
    {
        var catalog = Read<ExamCatalog>("Content/exam-2020-correction-basic.json");
        var exam = catalog.Exam;
        int[] expectedSourcePages =
        [
            2, 2, 2, 2, 2, 2, 4, 4, 4, 4, 4, 6, 6, 6, 6, 8, 8, 8, 8, 10, 10, 10, 10, 10, 10, 12, 13, 14, 15, 16, 17, 18, 20, 22
        ];
        int[] expectedSolutionPages =
        [
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 4, 6, 8, 9, 10, 10, 13, 14
        ];
        int[] expectedKey =
        [
            3, 1, 4, 3, 1, 3, 3, 2, 1, 4, 3, 3, 4, 1, 4, 2, 2, 1, 4, 4, 2, 1, 4, 4, 3
        ];

        Assert.Equal(4, catalog.SchemaVersion);
        Assert.Equal(ExamId, exam.Id);
        Assert.Equal("Matura poprawkowa 2020 - poziom podstawowy (Formuła 2015)", exam.Title);
        Assert.Equal("Matura poprawkowa 2020 PP (F2015)", exam.ProgressLabel);
        Assert.Equal(2020, exam.Year);
        Assert.Equal("poprawkowa", exam.Session);
        Assert.Equal("2015", exam.Formula);
        Assert.Equal("basic", exam.Level);
        Assert.Equal(170, exam.DurationMinutes);
        Assert.Equal(50, exam.MaximumPoints);
        Assert.Equal(34, exam.OfficialTaskCount);
        Assert.Equal(34, exam.ProgressItemCount);
        Assert.Equal(34, exam.Exercises.Count);
        Assert.Equal(50, exam.Exercises.Sum(item => item.Points));
        Assert.Equal(Enumerable.Range(1, 34).Select(number => number.ToString()), exam.Exercises.Select(item => item.DisplayNumber));
        Assert.Equal(Enumerable.Range(1, 34), exam.Exercises.Select(item => item.EffectiveOrder));
        Assert.Equal(Enumerable.Range(1, 34), exam.Exercises.Select(item => item.Number));
        Assert.Equal(expectedSourcePages, exam.Exercises.Select(item => item.SourcePage));
        Assert.Equal(expectedSolutionPages, exam.Exercises.Select(item => item.SolutionSourcePage));
        Assert.Equal(expectedKey, exam.Exercises.Take(25).Select(item => item.CorrectOption!.Value));
        Assert.Equal(Enumerable.Repeat(1, 25).Concat([2, 2, 2, 2, 2, 2, 4, 4, 5]), exam.Exercises.Select(item => item.Points));
        Assert.All(exam.Exercises, exercise =>
        {
            Assert.StartsWith("mm20-p0p-", exercise.Id, StringComparison.Ordinal);
            Assert.InRange(exercise.Id.Length, 1, 79);
            Assert.Equal(ExamId, exercise.ExamId);
            Assert.Equal($"Zadanie {exercise.DisplayNumber}", exercise.Title);
            Assert.Equal("CKE MMA-P1_1P-204; publiczne archiwum PDF", exercise.VerificationSource);
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.True(exercise.Hints.Count >= 2);
            Assert.False(string.IsNullOrWhiteSpace(exercise.EffectiveSolution));
            Assert.False(string.IsNullOrWhiteSpace(exercise.ScoringCriteria));
        });
    }

    [Fact]
    public void Correction_archive_urls_hashes_date_and_answer_key_are_pinned()
    {
        var source = Read<ExamCatalog>("Content/exam-2020-correction-basic.json").Exam.Source;

        Assert.Contains("Centralna Komisja Egzaminacyjna", source.Publisher, StringComparison.Ordinal);
        Assert.Contains("arkusze.pl", source.Publisher, StringComparison.Ordinal);
        Assert.Equal("MMA-P1_1P-204", source.DocumentCode);
        Assert.Equal("2020-09-08", source.ExamDate);
        Assert.Equal(PaperUrl, source.QuestionPaperUrl);
        Assert.Equal(PaperHash, source.QuestionPaperSha256);
        Assert.Equal(RulesUrl, source.AnswerKeyUrl);
        Assert.Equal(RulesHash, source.AnswerKeySha256);
        Assert.Equal("2026-08-31", source.VerifiedOn);
        Assert.All(
            new[] { source.QuestionPaperSha256, source.AnswerKeySha256 },
            hash =>
            {
                Assert.Equal(64, hash.Length);
                Assert.True(hash.All(character => char.IsAsciiHexDigit(character) && !char.IsLower(character)));
            });
    }

    [Fact]
    public void Answer_modes_topics_and_six_vector_figures_cover_the_entire_exam()
    {
        var index = Read<ExamIndexCatalog>("Content/exams.json");
        var exam = Read<ExamCatalog>("Content/exam-2020-correction-basic.json").Exam;
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        string[] expectedDiagramIds =
        [
            "exam-mm20-p0p-z10", "exam-mm20-p0p-z15", "exam-mm20-p0p-z16",
            "exam-mm20-p0p-z20", "exam-mm20-p0p-z29", "exam-mm20-p0p-z32"
        ];

        Assert.Equal(25, exam.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(1, exam.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(8, exam.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(0, exam.Exercises.Count(item => item.IsCompound));
        Assert.Equal(28d, exam.Exercises.Single(item => item.Id == "mm20-p0p-z31").ExpectedValue!.Value, 12);
        Assert.All(exam.Exercises, exercise => Assert.Contains(index.Topics, topic => topic.Id == exercise.TopicId));

        var referenced = exam.Exercises.SelectMany(item => item.DiagramIds)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var definitions = diagrams.Diagrams.Where(item => item.SourceId == "cke-2020-correction-basic")
            .OrderBy(item => item.Id)
            .ToArray();

        Assert.Equal(expectedDiagramIds, referenced);
        Assert.Equal(referenced, definitions.Select(item => item.Id));
        Assert.Equal([4, 6, 8, 10, 15, 18], definitions.Select(item => item.SourcePage));
        Assert.All(definitions, definition =>
        {
            Assert.False(string.IsNullOrWhiteSpace(definition.AlternativeText));
            Assert.NotEmpty(definition.Primitives);
        });
        DiagramCatalogValidator.Validate(diagrams);
    }

    [Fact]
    public void Formula_2015_correction_keeps_its_approved_source_while_new_archives_block_release()
    {
        using var provenance = JsonDocument.Parse(File.ReadAllText(Absolute("Content/provenance.json")));
        var root = provenance.RootElement;
        var groups = root.GetProperty("assets").EnumerateArray().ToDictionary(
            item => item.GetProperty("id").GetString()!,
            StringComparer.Ordinal);
        var rights = File.ReadAllText(Absolute("docs/ASSET_RIGHTS_DECLARATION.md"));
        var coverage = File.ReadAllText(Absolute("docs/MATURA_2020_CORRECTION_BASIC_COVERAGE.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));

        Assert.False(root.GetProperty("releaseEligible").GetBoolean());
        Assert.Equal("approved", groups["cke-2020-correction-basic-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("blocked", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
        Assert.Contains(PaperHash, groups["cke-2020-correction-basic-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.Contains(RulesHash, groups["cke-2020-correction-basic-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.DoesNotContain(PaperHash, rights, StringComparison.Ordinal);
        Assert.DoesNotContain(RulesHash, rights, StringComparison.Ordinal);
        Assert.Contains(PaperHash, coverage, StringComparison.Ordinal);
        Assert.Contains(RulesHash, coverage, StringComparison.Ordinal);
        Assert.Contains("MATURA_2020_CORRECTION_BASIC_COVERAGE.md", toc, StringComparison.Ordinal);
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
