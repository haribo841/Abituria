using System.Text.Json;
using Abituria.Models;
using Abituria.Services;

namespace Abituria.Tests;

public sealed class Matura2024ExtendedContentTests
{
    private const string ExamId = "matura-maj-2024-rozszerzona";
    private const string PaperUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2024/Matematyka/poziom_rozszerzony/MMAP-R0-100-A-2405-arkusz.pdf";
    private const string RulesUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2024/Matematyka/poziom_rozszerzony/MMAP-R0-100-2405-zasady.pdf";
    private const string PaperHash = "873691F2E3740126D969AAC957CBC5666FCAD7D7FCF8499442781E18F6AD53D6";
    private const string RulesHash = "6535405993D0A9F3360759A2B2335BF7523E177144B207B4D8A6E3D8A3A8AB92";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Official_extended_exam_contract_is_exactly_13_tasks_14_progress_items_and_50_points()
    {
        var catalog = Read<ExamCatalog>("Content/exam-2024-main-extended.json");
        var exam = catalog.Exam;
        string[] expectedLabels = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13.1", "13.2"];
        int[] expectedSourcePages = [4, 5, 6, 7, 8, 9, 10, 12, 14, 16, 18, 20, 23, 24];
        int[] expectedSolutionPages = [2, 3, 5, 6, 8, 10, 14, 18, 32, 40, 44, 48, 51, 52];
        int[] expectedPoints = [2, 2, 3, 3, 3, 3, 4, 4, 4, 5, 5, 6, 2, 4];

        Assert.Equal(4, catalog.SchemaVersion);
        Assert.Equal(ExamId, exam.Id);
        Assert.Equal("Matura maj 2024 - poziom rozszerzony", exam.Title);
        Assert.Equal("Matura maj 2024 PR", exam.ProgressLabel);
        Assert.Equal(2024, exam.Year);
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
            Assert.StartsWith("mm24-r0-", exercise.Id, StringComparison.Ordinal);
            Assert.InRange(exercise.Id.Length, 1, 79);
            Assert.Equal(ExamId, exercise.ExamId);
            Assert.Equal($"Zadanie {exercise.DisplayNumber}", exercise.Title);
            Assert.InRange(exercise.Points, 1, 6);
            Assert.InRange(exercise.SourcePage, 4, 24);
            Assert.InRange(exercise.SolutionSourcePage, 2, 52);
            Assert.Equal("CKE MMAP-R0-100-A-2405 i MMAP-R0-100-2405", exercise.VerificationSource);
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.False(string.IsNullOrWhiteSpace(exercise.EffectiveSolution));
            Assert.False(string.IsNullOrWhiteSpace(exercise.ScoringCriteria));
        });
    }

    [Fact]
    public void Official_extended_sources_hashes_and_verification_date_are_pinned()
    {
        var source = Read<ExamCatalog>("Content/exam-2024-main-extended.json").Exam.Source;

        Assert.Equal("Centralna Komisja Egzaminacyjna", source.Publisher);
        Assert.Equal("MMAP-R0-100-A-2405", source.DocumentCode);
        Assert.Equal("2024-05-15", source.ExamDate);
        Assert.Equal(PaperUrl, source.QuestionPaperUrl);
        Assert.Equal(PaperHash, source.QuestionPaperSha256);
        Assert.Equal(RulesUrl, source.AnswerKeyUrl);
        Assert.Equal(RulesHash, source.AnswerKeySha256);
        Assert.Equal("2026-08-08", source.VerifiedOn);
        Assert.All(
            new[] { source.QuestionPaperSha256, source.AnswerKeySha256 },
            hash =>
            {
                Assert.Equal(64, hash.Length);
                Assert.True(hash.All(character => char.IsAsciiHexDigit(character) && !char.IsLower(character)));
            });
    }

    [Fact]
    public void Answer_modes_results_topics_and_vector_figure_are_complete()
    {
        var index = Read<ExamIndexCatalog>("Content/exams.json");
        var exam = Read<ExamCatalog>("Content/exam-2024-main-extended.json").Exam;
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        var expectedNumeric = new Dictionary<string, double>(StringComparer.Ordinal)
        {
            ["mm24-r0-z01"] = 59d,
            ["mm24-r0-z03"] = 0.996d,
            ["mm24-r0-z06"] = 11_040d
        };

        Assert.Equal(3, exam.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(3, exam.Exercises.Count(item => item.IsCompound));
        Assert.Equal(8, exam.Exercises.Count(item => item.IsRevealOnly));
        Assert.DoesNotContain(exam.Exercises, item => item.IsMultipleChoice);
        Assert.Equal(expectedNumeric.Keys.Order(), exam.Exercises.Where(item => item.IsNumeric).Select(item => item.Id).Order());
        foreach (var pair in expectedNumeric)
            Assert.Equal(pair.Value, exam.Exercises.Single(item => item.Id == pair.Key).ExpectedValue!.Value, 12);

        Assert.Equal([3.5d, -5d], CompoundNumericValues(exam, "mm24-r0-z04"), new DoublePrecisionComparer(12));
        Assert.Equal([5d, 20d, 80d], CompoundNumericValues(exam, "mm24-r0-z07"), new DoublePrecisionComparer(12));
        Assert.Equal(
            [8d * Math.Sqrt(3d), 1728d + 96d * Math.Sqrt(3d)],
            CompoundNumericValues(exam, "mm24-r0-z13-2"),
            new DoublePrecisionComparer(12));
        Assert.All(exam.Exercises, exercise => Assert.Contains(index.Topics, topic => topic.Id == exercise.TopicId));

        var referenced = exam.Exercises.SelectMany(item => item.DiagramIds)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var definitions = diagrams.Diagrams.Where(item => item.SourceId == "cke-2024-main-extended")
            .OrderBy(item => item.Id)
            .ToArray();
        Assert.Equal(["exam-mm24-r0-z09"], referenced);
        Assert.Equal(referenced, definitions.Select(item => item.Id));
        Assert.Equal([14], definitions.Select(item => item.SourcePage));
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
        var coverage = File.ReadAllText(Absolute("docs/MATURA_2024_EXTENDED_COVERAGE.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));

        Assert.True(root.GetProperty("releaseEligible").GetBoolean());
        Assert.Equal("approved", groups["cke-2024-main-extended-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("approved", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
        Assert.Contains(PaperHash, groups["cke-2024-main-extended-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.Contains(RulesHash, groups["cke-2024-main-extended-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.Contains(PaperHash, rights, StringComparison.Ordinal);
        Assert.Contains(RulesHash, rights, StringComparison.Ordinal);
        Assert.Contains("Rozszerzenie deklaracji z 10 sierpnia 2026 r.", rights, StringComparison.Ordinal);
        Assert.Contains(PaperHash, coverage, StringComparison.Ordinal);
        Assert.Contains(RulesHash, coverage, StringComparison.Ordinal);
        Assert.Contains("MATURA_2024_EXTENDED_COVERAGE.md", toc, StringComparison.Ordinal);
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
