using System.Text.Json;
using Abituria.Models;
using Abituria.Services;

namespace Abituria.Tests;

public sealed class Matura2024CorrectionBasicContentTests
{
    private const string ExamId = "matura-poprawkowa-2024-podstawowa";
    private const string PaperUrl = "https://arkusze.pl/maturalne/matematyka-2024-sierpien-poprawkowa-podstawowa.pdf";
    private const string RulesUrl = "https://arkusze.pl/maturalne/matematyka-2024-sierpien-poprawkowa-podstawowa-odpowiedzi.pdf";
    private const string PaperHash = "351773315AC15C190E12C8496FD518DA111C12AB150B3253A6121ECBA61DCC87";
    private const string RulesHash = "3068D4F4B20A46293A209E980507E7183226EF79CA5FB477C69388B3DB90CBE4";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Correction_basic_exam_contract_is_exactly_30_tasks_36_progress_items_and_46_points()
    {
        var catalog = Read<ExamCatalog>("Content/exam-2024-correction-basic.json");
        var exam = catalog.Exam;
        string[] expectedLabels =
        [
            "1", "2", "3", "4.1", "4.2", "5", "6", "7", "8", "9.1", "9.2", "10", "11.1", "11.2",
            "11.3", "12.1", "12.2", "12.3", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22",
            "23", "24", "25", "26", "27", "28", "29", "30"
        ];
        int[] expectedSourcePages =
        [
            4, 4, 5, 6, 6, 7, 7, 8, 10, 11, 11, 11, 12, 12, 13, 14, 14, 15, 16, 16, 17, 17, 18, 18, 19, 20, 22, 24,
            24, 25, 25, 26, 27, 27, 28, 29
        ];
        int[] expectedSolutionPages =
        [
            2, 3, 3, 5, 5, 5, 6, 6, 10, 10, 10, 11, 11, 12, 12, 13, 13, 14, 14, 15, 15, 16, 16, 17, 17, 18, 20, 25,
            25, 26, 26, 27, 27, 28, 29, 32
        ];
        int[] expectedPoints =
        [
            1, 1, 2, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 4, 1, 1, 1, 1, 1, 1, 1,
            2, 3
        ];

        Assert.Equal(4, catalog.SchemaVersion);
        Assert.Equal(ExamId, exam.Id);
        Assert.Equal("Matura poprawkowa 2024 - poziom podstawowy", exam.Title);
        Assert.Equal("Matura poprawkowa 2024 PP", exam.ProgressLabel);
        Assert.Equal(2024, exam.Year);
        Assert.Equal("poprawkowa", exam.Session);
        Assert.Equal("2023", exam.Formula);
        Assert.Equal("basic", exam.Level);
        Assert.Equal(180, exam.DurationMinutes);
        Assert.Equal(46, exam.MaximumPoints);
        Assert.Equal(30, exam.OfficialTaskCount);
        Assert.Equal(36, exam.ProgressItemCount);
        Assert.Equal(36, exam.Exercises.Count);
        Assert.Equal(46, exam.Exercises.Sum(item => item.Points));
        Assert.Equal(expectedLabels, exam.Exercises.Select(item => item.DisplayNumber));
        Assert.Equal(expectedSourcePages, exam.Exercises.Select(item => item.SourcePage));
        Assert.Equal(expectedSolutionPages, exam.Exercises.Select(item => item.SolutionSourcePage));
        Assert.Equal(expectedPoints, exam.Exercises.Select(item => item.Points));
        Assert.Equal(Enumerable.Range(1, 36), exam.Exercises.Select(item => item.EffectiveOrder));
        Assert.Equal(Enumerable.Range(1, 30), exam.Exercises.Select(item => item.Number).Distinct());
        Assert.Equal(30, exam.Exercises.Select(OfficialGroupId).Distinct(StringComparer.Ordinal).Count());

        Assert.All(exam.Exercises, exercise =>
        {
            Assert.StartsWith("mm24-p0p-", exercise.Id, StringComparison.Ordinal);
            Assert.InRange(exercise.Id.Length, 1, 79);
            Assert.Equal(ExamId, exercise.ExamId);
            Assert.Equal($"Zadanie {exercise.DisplayNumber}", exercise.Title);
            Assert.InRange(exercise.Points, 1, 4);
            Assert.InRange(exercise.SourcePage, 4, 29);
            Assert.InRange(exercise.SolutionSourcePage, 2, 32);
            Assert.Equal("CKE MMAP-P0-100-2408; publiczne archiwum PDF", exercise.VerificationSource);
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.True(exercise.Hints.Count >= 2);
            Assert.False(string.IsNullOrWhiteSpace(exercise.EffectiveSolution));
            Assert.False(string.IsNullOrWhiteSpace(exercise.ScoringCriteria));
        });
    }

    [Fact]
    public void Archive_sources_hashes_and_verification_date_are_pinned_without_claiming_a_cke_host()
    {
        var source = Read<ExamCatalog>("Content/exam-2024-correction-basic.json").Exam.Source;

        Assert.Contains("Centralna Komisja Egzaminacyjna", source.Publisher, StringComparison.Ordinal);
        Assert.Contains("arkusze.pl", source.Publisher, StringComparison.Ordinal);
        Assert.Equal("MMAP-P0-100-2408", source.DocumentCode);
        Assert.Equal("2024-08-20", source.ExamDate);
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
    public void Answer_modes_topics_and_vector_figures_are_complete()
    {
        var index = Read<ExamIndexCatalog>("Content/exams.json");
        var exam = Read<ExamCatalog>("Content/exam-2024-correction-basic.json").Exam;
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        string[] expectedDiagramIds =
        [
            "exam-mm24-p0p-z08", "exam-mm24-p0p-z12", "exam-mm24-p0p-z19", "exam-mm24-p0p-z20", "exam-mm24-p0p-z26"
        ];

        Assert.Equal(27, exam.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(3, exam.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(2, exam.Exercises.Count(item => item.IsCompound));
        Assert.Equal(4, exam.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(0.5d, exam.Exercises.Single(item => item.Id == "mm24-p0p-z10").ExpectedValue!.Value, 12);
        Assert.Equal(20d, exam.Exercises.Single(item => item.Id == "mm24-p0p-z20").ExpectedValue!.Value, 12);
        Assert.Equal(4d / 15d, exam.Exercises.Single(item => item.Id == "mm24-p0p-z29").ExpectedValue!.Value, 12);
        Assert.All(exam.Exercises, exercise => Assert.Contains(index.Topics, topic => topic.Id == exercise.TopicId));

        var referenced = exam.Exercises.SelectMany(item => item.DiagramIds)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var definitions = diagrams.Diagrams.Where(item => item.SourceId == "cke-2024-correction-basic")
            .OrderBy(item => item.Id)
            .ToArray();

        Assert.Equal(expectedDiagramIds, referenced);
        Assert.Equal(referenced, definitions.Select(item => item.Id));
        Assert.Equal([10, 14, 19, 20, 26], definitions.Select(item => item.SourcePage));
        Assert.All(definitions, definition =>
        {
            Assert.False(string.IsNullOrWhiteSpace(definition.AlternativeText));
            Assert.NotEmpty(definition.Primitives);
        });
        DiagramCatalogValidator.Validate(diagrams);
    }

    [Fact]
    public void Archive_exam_and_derived_diagrams_are_approved_after_explicit_rights_declaration()
    {
        using var provenance = JsonDocument.Parse(File.ReadAllText(Absolute("Content/provenance.json")));
        var root = provenance.RootElement;
        var groups = root.GetProperty("assets").EnumerateArray().ToDictionary(
            item => item.GetProperty("id").GetString()!,
            StringComparer.Ordinal);
        var rights = File.ReadAllText(Absolute("docs/ASSET_RIGHTS_DECLARATION.md"));
        var coverage = File.ReadAllText(Absolute("docs/MATURA_2024_CORRECTION_BASIC_COVERAGE.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));

        Assert.True(root.GetProperty("releaseEligible").GetBoolean());
        Assert.Equal("approved", groups["cke-2024-correction-basic-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("approved", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
        Assert.Contains(PaperHash, groups["cke-2024-correction-basic-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.Contains(RulesHash, groups["cke-2024-correction-basic-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.DoesNotContain(PaperHash, rights, StringComparison.Ordinal);
        Assert.DoesNotContain(RulesHash, rights, StringComparison.Ordinal);
        Assert.Contains(PaperHash, coverage, StringComparison.Ordinal);
        Assert.Contains(RulesHash, coverage, StringComparison.Ordinal);
        Assert.Contains("MATURA_2024_CORRECTION_BASIC_COVERAGE.md", toc, StringComparison.Ordinal);
    }

    private static string OfficialGroupId(LearningExercise exercise) =>
        string.IsNullOrWhiteSpace(exercise.GroupId) ? exercise.Id : exercise.GroupId;

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
