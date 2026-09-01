using System.Text.Json;
using Abituria.Models;
using Abituria.Services;

namespace Abituria.Tests;

public sealed class Matura2023ExtendedContentTests
{
    private const string ExamId = "matura-maj-2023-rozszerzona";
    private const string PaperUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2023/Matematyka/poziom_rozszerzony/MMAP-R0-100-2305.pdf";
    private const string RulesUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2023/Matematyka/poziom_rozszerzony/MMAP-R0-100-2305-zasady.pdf";
    private const string PaperHash = "24EC13FEA77323841A8538E85B816C7EE36199E64F5F756E30340489864EC207";
    private const string RulesHash = "B8FECD4D23811033E0DFF6C532A405F04ECE0CCC469A6D60412E353F1BBDBD2B";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Official_extended_exam_contract_is_exactly_13_tasks_14_progress_items_and_50_points()
    {
        var catalog = Read<ExamCatalog>("Content/exam-2023-main-extended.json");
        var exam = catalog.Exam;
        string[] expectedLabels = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12.1", "12.2", "13"];
        int[] expectedSourcePages = [4, 5, 6, 7, 8, 10, 11, 12, 14, 16, 18, 20, 21, 22];
        int[] expectedSolutionPages = [3, 4, 8, 10, 12, 20, 23, 28, 31, 34, 36, 40, 41, 43];
        int[] expectedPoints = [2, 3, 3, 3, 3, 3, 4, 4, 4, 4, 5, 2, 4, 6];

        Assert.Equal(4, catalog.SchemaVersion);
        Assert.Equal(ExamId, exam.Id);
        Assert.Equal("Matura maj 2023 - poziom rozszerzony", exam.Title);
        Assert.Equal("Matura maj 2023 PR", exam.ProgressLabel);
        Assert.Equal(2023, exam.Year);
        Assert.Equal("główna", exam.Session);
        Assert.Equal("2023", exam.Formula);
        Assert.Equal("extended", exam.Level);
        Assert.Equal(180, exam.DurationMinutes);
        Assert.Equal(50, exam.MaximumPoints);
        Assert.Equal(13, exam.OfficialTaskCount);
        Assert.Equal(14, exam.ProgressItemCount);
        Assert.Equal(14, exam.Exercises.Count);
        Assert.Equal(50, exam.Exercises.Sum(item => item.Points));
        Assert.Equal(expectedLabels, exam.Exercises.Select(item => item.DisplayNumber));
        Assert.Equal(expectedSourcePages, exam.Exercises.Select(item => item.SourcePage));
        Assert.Equal(expectedSolutionPages, exam.Exercises.Select(item => item.SolutionSourcePage));
        Assert.Equal(expectedPoints, exam.Exercises.Select(item => item.Points));
        Assert.Equal(Enumerable.Range(1, 14), exam.Exercises.Select(item => item.EffectiveOrder));
        Assert.Equal(Enumerable.Range(1, 13), exam.Exercises.Select(item => item.Number).Distinct());
        Assert.Equal(13, exam.Exercises.Select(OfficialGroupId).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(14, exam.Exercises.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count());

        Assert.All(exam.Exercises, exercise =>
        {
            Assert.StartsWith("mm23-r0-", exercise.Id, StringComparison.Ordinal);
            Assert.InRange(exercise.Id.Length, 1, 79);
            Assert.Equal(ExamId, exercise.ExamId);
            Assert.Equal($"Zadanie {exercise.DisplayNumber}", exercise.Title);
            Assert.InRange(exercise.Points, 1, 6);
            Assert.InRange(exercise.SourcePage, 4, 22);
            Assert.InRange(exercise.SolutionSourcePage, 3, 43);
            Assert.Equal("CKE MMAP-R0-100-2305 i MMAP-R0-100-2305-zasady", exercise.VerificationSource);
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.False(string.IsNullOrWhiteSpace(exercise.EffectiveSolution));
            Assert.False(string.IsNullOrWhiteSpace(exercise.ScoringCriteria));
        });
    }

    [Fact]
    public void Official_extended_sources_hashes_and_verification_date_are_pinned()
    {
        var source = Read<ExamCatalog>("Content/exam-2023-main-extended.json").Exam.Source;

        Assert.Equal("Centralna Komisja Egzaminacyjna", source.Publisher);
        Assert.Equal("MMAP-R0-100-2305", source.DocumentCode);
        Assert.Equal("2023-05-12", source.ExamDate);
        Assert.Equal(PaperUrl, source.QuestionPaperUrl);
        Assert.Equal(PaperHash, source.QuestionPaperSha256);
        Assert.Equal(RulesUrl, source.AnswerKeyUrl);
        Assert.Equal(RulesHash, source.AnswerKeySha256);
        Assert.Equal("2026-08-12", source.VerifiedOn);
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
        var exam = Read<ExamCatalog>("Content/exam-2023-main-extended.json").Exam;
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        var expectedNumeric = new Dictionary<string, double>(StringComparer.Ordinal)
        {
            ["mm23-r0-z02"] = 1d / 64d,
            ["mm23-r0-z07"] = Math.Sqrt(6d),
            ["mm23-r0-z12-2"] = -4d
        };

        Assert.Equal(3, exam.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(2, exam.Exercises.Count(item => item.IsCompound));
        Assert.Equal(9, exam.Exercises.Count(item => item.IsRevealOnly));
        Assert.DoesNotContain(exam.Exercises, item => item.IsMultipleChoice);
        Assert.Equal(expectedNumeric.Keys.Order(), exam.Exercises.Where(item => item.IsNumeric).Select(item => item.Id).Order());
        foreach (var pair in expectedNumeric)
            Assert.Equal(pair.Value, exam.Exercises.Single(item => item.Id == pair.Key).ExpectedValue!.Value, 12);

        Assert.Equal(
            [-3d, -8d / 11d, 9d / 11d],
            CompoundNumericValues(exam, "mm23-r0-z03"),
            new DoublePrecisionComparer(12));
        Assert.Equal(
            [11d / 10d, -3d / 10d],
            CompoundNumericValues(exam, "mm23-r0-z13"),
            new DoublePrecisionComparer(12));
        Assert.All(exam.Exercises, exercise => Assert.Contains(index.Topics, topic => topic.Id == exercise.TopicId));

        string[] expectedDiagramIds =
        [
            "exam-mm23-r0-z05",
            "exam-mm23-r0-z07",
            "exam-mm23-r0-z10",
            "exam-mm23-r0-z13"
        ];
        var referenced = exam.Exercises.SelectMany(item => item.DiagramIds)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var definitions = diagrams.Diagrams.Where(item => item.SourceId == "cke-2023-main-extended")
            .OrderBy(item => item.Id)
            .ToArray();

        Assert.Equal(expectedDiagramIds, referenced);
        Assert.Equal(referenced, definitions.Select(item => item.Id));
        Assert.Equal([8, 11, 16, 22], definitions.Select(item => item.SourcePage));
        Assert.All(definitions, definition =>
        {
            Assert.False(string.IsNullOrWhiteSpace(definition.AlternativeText));
            Assert.NotEmpty(definition.Primitives);
        });
        DiagramCatalogValidator.Validate(diagrams);
    }

    [Fact]
    public void New_cke_exam_is_documented_and_approved_by_the_rights_extension()
    {
        using var provenance = JsonDocument.Parse(File.ReadAllText(Absolute("Content/provenance.json")));
        var root = provenance.RootElement;
        var groups = root.GetProperty("assets").EnumerateArray().ToDictionary(
            item => item.GetProperty("id").GetString()!,
            StringComparer.Ordinal);
        var rights = File.ReadAllText(Absolute("docs/ASSET_RIGHTS_DECLARATION.md"));
        var coverage = File.ReadAllText(Absolute("docs/MATURA_2023_EXTENDED_COVERAGE.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));

        Assert.True(root.GetProperty("releaseEligible").GetBoolean());
        Assert.Equal("approved", groups["cke-2023-main-extended-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("approved", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
        Assert.Contains(PaperHash, groups["cke-2023-main-extended-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.Contains(RulesHash, groups["cke-2023-main-extended-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.Contains(PaperHash, rights, StringComparison.Ordinal);
        Assert.Contains(RulesHash, rights, StringComparison.Ordinal);
        Assert.Contains("Rozszerzenie deklaracji z 12 sierpnia 2026 r.", rights, StringComparison.Ordinal);
        Assert.Contains(PaperHash, coverage, StringComparison.Ordinal);
        Assert.Contains(RulesHash, coverage, StringComparison.Ordinal);
        Assert.Contains("MATURA_2023_EXTENDED_COVERAGE.md", toc, StringComparison.Ordinal);
    }

    private static string OfficialGroupId(LearningExercise exercise) =>
        string.IsNullOrWhiteSpace(exercise.GroupId) ? exercise.Id : exercise.GroupId;

    private static IEnumerable<double> CompoundNumericValues(ExamDefinition exam, string exerciseId) =>
        exam.Exercises.Single(item => item.Id == exerciseId).AnswerParts.Select(item => item.ExpectedValue!.Value);

    private sealed class DoublePrecisionComparer(int precision) : IEqualityComparer<double>
    {
        public bool Equals(double x, double y) => Math.Round(x, precision) == Math.Round(y, precision);

        public int GetHashCode(double value) => Math.Round(value, precision).GetHashCode();
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
