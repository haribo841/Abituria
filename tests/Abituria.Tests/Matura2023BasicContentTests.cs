using System.Text.Json;
using Abituria.Models;
using Abituria.Services;

namespace Abituria.Tests;

public sealed class Matura2023BasicContentTests
{
    private const string ExamId = "matura-maj-2023-podstawowa";
    private const string PaperUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2023/Matematyka/poziom_podstawowy/MMAP-P0-100-2305.pdf";
    private const string RulesUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2023/Matematyka/poziom_podstawowy/MMAP-P0-100-2305-zasady.pdf";
    private const string PaperHash = "FC5BD519FBF0D6D0D66993167121ABC01ADEC3466922E1678E42DFF565DA001C";
    private const string RulesHash = "811765BD8A860BAC1BC1745646FAC898EBF62464964EAA2E58FC70EDB2913BF9";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Official_basic_exam_contract_is_exactly_31_tasks_34_progress_items_and_46_points()
    {
        var catalog = Read<ExamCatalog>("Content/exam-2023-main-basic.json");
        var exam = catalog.Exam;
        string[] expectedLabels =
        [
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12.1", "12.2", "12.3",
            "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26",
            "27", "28", "29", "30", "31.1", "31.2"
        ];
        int[] expectedSourcePages =
        [
            4, 4, 5, 6, 6, 7, 7, 8, 8, 10, 11, 12, 12, 13, 13, 14, 15, 15, 16, 17, 17, 18, 18, 19,
            20, 21, 21, 22, 24, 24, 25, 26, 27, 27
        ];
        int[] expectedSolutionPages =
        [
            2, 3, 3, 6, 6, 7, 7, 8, 8, 11, 11, 12, 12, 13, 13, 14, 14, 15, 15, 19, 19, 20, 20, 21,
            24, 24, 25, 25, 27, 28, 28, 29, 31, 32
        ];
        int[] expectedPoints =
        [
            1, 1, 2, 1, 1, 1, 1, 1, 3, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 4,
            1, 1, 2, 2, 1, 2
        ];

        Assert.Equal(4, catalog.SchemaVersion);
        Assert.Equal(ExamId, exam.Id);
        Assert.Equal("Matura maj 2023 - poziom podstawowy", exam.Title);
        Assert.Equal("Matura maj 2023 PP", exam.ProgressLabel);
        Assert.Equal(2023, exam.Year);
        Assert.Equal("główna", exam.Session);
        Assert.Equal("2023", exam.Formula);
        Assert.Equal("basic", exam.Level);
        Assert.Equal(180, exam.DurationMinutes);
        Assert.Equal(46, exam.MaximumPoints);
        Assert.Equal(31, exam.OfficialTaskCount);
        Assert.Equal(34, exam.ProgressItemCount);
        Assert.Equal(34, exam.Exercises.Count);
        Assert.Equal(46, exam.Exercises.Sum(item => item.Points));
        Assert.Equal(expectedLabels, exam.Exercises.Select(item => item.DisplayNumber));
        Assert.Equal(expectedSourcePages, exam.Exercises.Select(item => item.SourcePage));
        Assert.Equal(expectedSolutionPages, exam.Exercises.Select(item => item.SolutionSourcePage));
        Assert.Equal(expectedPoints, exam.Exercises.Select(item => item.Points));
        Assert.Equal(Enumerable.Range(1, 34), exam.Exercises.Select(item => item.EffectiveOrder));
        Assert.Equal(Enumerable.Range(1, 31), exam.Exercises.Select(item => item.Number).Distinct());
        Assert.Equal(31, exam.Exercises.Select(OfficialGroupId).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(34, exam.Exercises.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count());

        Assert.All(exam.Exercises, exercise =>
        {
            Assert.StartsWith("mm23-p0-", exercise.Id, StringComparison.Ordinal);
            Assert.InRange(exercise.Id.Length, 1, 79);
            Assert.Equal(ExamId, exercise.ExamId);
            Assert.Equal($"Zadanie {exercise.DisplayNumber}", exercise.Title);
            Assert.InRange(exercise.Points, 1, 4);
            Assert.InRange(exercise.SourcePage, 4, 27);
            Assert.InRange(exercise.SolutionSourcePage, 2, 32);
            Assert.Equal("CKE MMAP-P0-100-2305 i MMAP-P0-100-2305-zasady", exercise.VerificationSource);
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.False(string.IsNullOrWhiteSpace(exercise.EffectiveSolution));
            Assert.False(string.IsNullOrWhiteSpace(exercise.ScoringCriteria));
        });
    }

    [Fact]
    public void Official_basic_sources_hashes_and_verification_date_are_pinned()
    {
        var source = Read<ExamCatalog>("Content/exam-2023-main-basic.json").Exam.Source;

        Assert.Equal("Centralna Komisja Egzaminacyjna", source.Publisher);
        Assert.Equal("MMAP-P0-100-2305", source.DocumentCode);
        Assert.Equal("2023-05-08", source.ExamDate);
        Assert.Equal(PaperUrl, source.QuestionPaperUrl);
        Assert.Equal(PaperHash, source.QuestionPaperSha256);
        Assert.Equal(RulesUrl, source.AnswerKeyUrl);
        Assert.Equal(RulesHash, source.AnswerKeySha256);
        Assert.Equal("2026-08-30", source.VerifiedOn);
        Assert.All(
            new[] { source.QuestionPaperSha256, source.AnswerKeySha256 },
            hash =>
            {
                Assert.Equal(64, hash.Length);
                Assert.True(hash.All(character => char.IsAsciiHexDigit(character) && !char.IsLower(character)));
            });
    }

    [Fact]
    public void Answer_modes_results_topics_and_vector_figures_are_complete()
    {
        var index = Read<ExamIndexCatalog>("Content/exams.json");
        var exam = Read<ExamCatalog>("Content/exam-2023-main-basic.json").Exam;
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        int[] expectedChoiceKey =
        [
            2, 1, 4, 1, 3, 4, 1, 4, 1, 3, 4, 3, 1, 4, 3, 4, 1, 2, 2, 4, 2, 2, 3
        ];
        string[] expectedDiagramIds =
        [
            "exam-mm23-p0-z01", "exam-mm23-p0-z10", "exam-mm23-p0-z12", "exam-mm23-p0-z13",
            "exam-mm23-p0-z18", "exam-mm23-p0-z21", "exam-mm23-p0-z26", "exam-mm23-p0-z29"
        ];

        Assert.Equal(23, exam.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(3, exam.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(4, exam.Exercises.Count(item => item.IsCompound));
        Assert.Equal(4, exam.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(expectedChoiceKey, exam.Exercises.Where(item => item.IsMultipleChoice).Select(item => item.CorrectOption!.Value));
        Assert.Equal(750d, exam.Exercises.Single(item => item.Id == "mm23-p0-z17").ExpectedValue!.Value, 12);
        Assert.Equal(120d, exam.Exercises.Single(item => item.Id == "mm23-p0-z22").ExpectedValue!.Value, 12);
        Assert.Equal(3d / 32d, exam.Exercises.Single(item => item.Id == "mm23-p0-z30").ExpectedValue!.Value, 12);
        Assert.Equal([3, 5], CompoundChoiceKey(exam, "mm23-p0-z11"));
        Assert.Equal([1, 2], CompoundChoiceKey(exam, "mm23-p0-z23"));
        Assert.Equal([3, 1], CompoundChoiceKey(exam, "mm23-p0-z29"));
        Assert.Equal([2, 1], CompoundChoiceKey(exam, "mm23-p0-z31-1"));
        Assert.All(exam.Exercises, exercise => Assert.Contains(index.Topics, topic => topic.Id == exercise.TopicId));

        var referenced = exam.Exercises.SelectMany(item => item.DiagramIds)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var definitions = diagrams.Diagrams.Where(item => item.SourceId == "cke-2023-main-basic")
            .OrderBy(item => item.Id)
            .ToArray();

        Assert.Equal(expectedDiagramIds, referenced);
        Assert.Equal(referenced, definitions.Select(item => item.Id));
        Assert.Equal([4, 10, 12, 13, 17, 18, 22, 25], definitions.Select(item => item.SourcePage));
        Assert.All(definitions, definition =>
        {
            Assert.False(string.IsNullOrWhiteSpace(definition.AlternativeText));
            Assert.NotEmpty(definition.Primitives);
        });
        DiagramCatalogValidator.Validate(diagrams);
    }

    [Fact]
    public void New_cke_exam_and_derived_diagrams_are_approved_after_explicit_rights_declaration()
    {
        using var provenance = JsonDocument.Parse(File.ReadAllText(Absolute("Content/provenance.json")));
        var root = provenance.RootElement;
        var groups = root.GetProperty("assets").EnumerateArray().ToDictionary(
            item => item.GetProperty("id").GetString()!,
            StringComparer.Ordinal);
        var rights = File.ReadAllText(Absolute("docs/ASSET_RIGHTS_DECLARATION.md"));
        var coverage = File.ReadAllText(Absolute("docs/MATURA_2023_BASIC_COVERAGE.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));

        Assert.True(root.GetProperty("releaseEligible").GetBoolean());
        Assert.Equal("approved", groups["cke-2023-main-basic-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("approved", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
        Assert.Contains(PaperHash, groups["cke-2023-main-basic-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.Contains(RulesHash, groups["cke-2023-main-basic-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.DoesNotContain(PaperHash, rights, StringComparison.Ordinal);
        Assert.DoesNotContain(RulesHash, rights, StringComparison.Ordinal);
        Assert.Contains(PaperHash, coverage, StringComparison.Ordinal);
        Assert.Contains(RulesHash, coverage, StringComparison.Ordinal);
        Assert.Contains("MATURA_2023_BASIC_COVERAGE.md", toc, StringComparison.Ordinal);
    }

    private static string OfficialGroupId(LearningExercise exercise) =>
        string.IsNullOrWhiteSpace(exercise.GroupId) ? exercise.Id : exercise.GroupId;

    private static IEnumerable<int> CompoundChoiceKey(ExamDefinition exam, string exerciseId) =>
        exam.Exercises.Single(item => item.Id == exerciseId).AnswerParts.Select(item => item.CorrectOption!.Value);

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
