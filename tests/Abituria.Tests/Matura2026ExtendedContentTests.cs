using System.Text.Json;
using Abituria.Models;

namespace Abituria.Tests;

public sealed class Matura2026ExtendedContentTests
{
    private const string ExamId = "matura-maj-2026-rozszerzona";
    private const string PaperUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2026/Matematyka/poziom_rozszerzony/MMAP-R0-100-A-2605-arkusz.pdf";
    private const string RulesUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2026/Matematyka/poziom_rozszerzony/MMAP-R0-100-2605-zasady.pdf";
    private const string PaperHash = "DEC5F06020C35DCDABAB5747942BEDFC49CF7307B27F0AD105FAA93741D03964";
    private const string RulesHash = "D7C014240AF16885DBDD1711D923AFF24951B2F514B7C5659E0B6F16508878BD";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Official_extended_exam_contract_is_exactly_12_tasks_13_progress_items_and_50_points()
    {
        var catalog = Read<ExamCatalog>("Content/exam-2026-main-extended.json");
        var exam = catalog.Exam;
        string[] expectedLabels = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12.1", "12.2"];
        int[] expectedSourcePages = [4, 5, 6, 8, 10, 12, 14, 16, 18, 20, 24, 28, 30];
        int[] expectedSolutionPages = [2, 3, 4, 8, 15, 21, 24, 28, 34, 39, 47, 49, 55];
        int[] expectedPoints = [2, 3, 3, 3, 4, 4, 4, 4, 5, 5, 6, 3, 4];

        Assert.Equal(4, catalog.SchemaVersion);
        Assert.Equal(ExamId, exam.Id);
        Assert.Equal("Matura maj 2026 - poziom rozszerzony", exam.Title);
        Assert.Equal("Matura maj 2026 PR", exam.ProgressLabel);
        Assert.Equal(2026, exam.Year);
        Assert.Equal("główna", exam.Session);
        Assert.Equal("2023", exam.Formula);
        Assert.Equal("extended", exam.Level);
        Assert.Equal(180, exam.DurationMinutes);
        Assert.Equal(50, exam.MaximumPoints);
        Assert.Equal(12, exam.OfficialTaskCount);
        Assert.Equal(13, exam.ProgressItemCount);
        Assert.Equal(13, exam.Exercises.Count);
        Assert.Equal(50, exam.Exercises.Sum(item => item.Points));
        Assert.Equal(expectedLabels, exam.Exercises.Select(item => item.DisplayNumber));
        Assert.Equal(expectedSourcePages, exam.Exercises.Select(item => item.SourcePage));
        Assert.Equal(expectedSolutionPages, exam.Exercises.Select(item => item.SolutionSourcePage));
        Assert.Equal(expectedPoints, exam.Exercises.Select(item => item.Points));
        Assert.Equal(Enumerable.Range(1, 13), exam.Exercises.Select(item => item.EffectiveOrder));
        Assert.Equal(Enumerable.Range(1, 12), exam.Exercises.Select(item => item.Number).Distinct());
        Assert.Equal(12, exam.Exercises.Select(OfficialGroupId).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(13, exam.Exercises.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count());

        Assert.All(exam.Exercises, exercise =>
        {
            Assert.StartsWith("mm26-r0-", exercise.Id, StringComparison.Ordinal);
            Assert.InRange(exercise.Id.Length, 1, 79);
            Assert.Equal(ExamId, exercise.ExamId);
            Assert.Equal($"Zadanie {exercise.DisplayNumber}", exercise.Title);
            Assert.InRange(exercise.Points, 1, 6);
            Assert.InRange(exercise.SourcePage, 4, 33);
            Assert.InRange(exercise.SolutionSourcePage, 1, 57);
            Assert.Equal("CKE MMAP-R0-100-A-2605 i MMAP-R0-100-2605", exercise.VerificationSource);
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.False(string.IsNullOrWhiteSpace(exercise.EffectiveSolution));
            Assert.False(string.IsNullOrWhiteSpace(exercise.ScoringCriteria));
        });
    }

    [Fact]
    public void Official_extended_sources_hashes_and_verification_date_are_pinned()
    {
        var source = Read<ExamCatalog>("Content/exam-2026-main-extended.json").Exam.Source;

        Assert.Equal("Centralna Komisja Egzaminacyjna", source.Publisher);
        Assert.Equal("MMAP-R0-100-A-2605", source.DocumentCode);
        Assert.Equal("2026-05-11", source.ExamDate);
        Assert.Equal(PaperUrl, source.QuestionPaperUrl);
        Assert.Equal(PaperHash, source.QuestionPaperSha256);
        Assert.Equal(RulesUrl, source.AnswerKeyUrl);
        Assert.Equal(RulesHash, source.AnswerKeySha256);
        Assert.Equal("2026-08-03", source.VerifiedOn);
        Assert.All(
            new[] { source.QuestionPaperSha256, source.AnswerKeySha256 },
            hash =>
            {
                Assert.Equal(64, hash.Length);
                Assert.True(hash.All(character => char.IsAsciiHexDigit(character) && !char.IsLower(character)));
            });
    }

    [Fact]
    public void Extended_answer_modes_results_and_official_figures_are_complete()
    {
        var exam = Read<ExamCatalog>("Content/exam-2026-main-extended.json").Exam;
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        var expectedNumeric = new Dictionary<string, double>(StringComparer.Ordinal)
        {
            ["mm26-r0-z01"] = 1d / 3d,
            ["mm26-r0-z02"] = 1d / 28d,
            ["mm26-r0-z06"] = -1_026_168d
        };

        Assert.Equal(3, exam.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(3, exam.Exercises.Count(item => item.IsCompound));
        Assert.Equal(7, exam.Exercises.Count(item => item.IsRevealOnly));
        Assert.DoesNotContain(exam.Exercises, item => item.IsMultipleChoice);
        Assert.Equal(expectedNumeric.Keys.Order(), exam.Exercises.Where(item => item.IsNumeric).Select(item => item.Id).Order());
        foreach (var pair in expectedNumeric)
            Assert.Equal(pair.Value, exam.Exercises.Single(item => item.Id == pair.Key).ExpectedValue!.Value, 12);

        Assert.Equal(
            [3d * Math.Sqrt(6d), 5d / 14d],
            CompoundNumericValues(exam, "mm26-r0-z08"),
            new DoublePrecisionComparer(12));
        Assert.Equal(
            [5d, 6d, 30d * Math.Sqrt(3d)],
            CompoundNumericValues(exam, "mm26-r0-z11"),
            new DoublePrecisionComparer(12));
        Assert.Equal(
            [4d * Math.Sqrt(3d), 12d * Math.Sqrt(3d)],
            CompoundNumericValues(exam, "mm26-r0-z12-2"),
            new DoublePrecisionComparer(12));

        var referenced = exam.Exercises.SelectMany(item => item.DiagramIds)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var definitions = diagrams.Diagrams.Where(item => item.SourceId == "cke-2026-main-extended")
            .OrderBy(item => item.Id)
            .ToArray();
        Assert.Equal(["exam-mm26-r0-z04", "exam-mm26-r0-z11", "exam-mm26-r0-z12"], referenced);
        Assert.Equal(referenced, definitions.Select(item => item.Id));
        Assert.Equal([8, 24, 28], definitions.Select(item => item.SourcePage));
        Assert.All(definitions, definition =>
        {
            Assert.False(string.IsNullOrWhiteSpace(definition.AlternativeText));
            Assert.NotEmpty(definition.Primitives);
        });
    }

    [Fact]
    public void Formula_2023_course_preserves_exact_73_basic_and_46_extended_requirements_with_own_materials()
    {
        var course = Read<MathCourseCatalog>("Content/chapters.json");
        var exercises = Read<CourseExerciseCatalog>("Content/course-exercises.json");
        var examples = course.Lessons.SelectMany(item => item.WorkedExamples).ToArray();

        Assert.Equal(4, course.Groups.Count);
        Assert.Equal(13, course.Areas.Count);
        Assert.Equal(119, course.Requirements.Count);
        Assert.Equal(73, course.Requirements.Count(item => item.Level == "basic"));
        Assert.Equal(46, course.Requirements.Count(item => item.Level == "extended"));
        Assert.Equal(238, examples.Length);
        Assert.Equal(357, exercises.Exercises.Count);
        Assert.Equal(219, exercises.Exercises.Count(item => item.Level == "basic"));
        Assert.Equal(138, exercises.Exercises.Count(item => item.Level == "extended"));
        Assert.All(course.Requirements, requirement =>
        {
            Assert.False(string.IsNullOrWhiteSpace(requirement.Text));
            Assert.Equal(2, requirement.WorkedExampleIds.Count);
            Assert.Equal(3, requirement.ExerciseIds.Count);
        });
        Assert.All(examples, example => Assert.Equal("Adam Kubiś", example.Author));
        Assert.Equal("Adam Kubiś", exercises.Author);
    }

    [Fact]
    public void Extended_cke_assets_are_approved_by_the_extended_rights_declaration()
    {
        using var provenance = JsonDocument.Parse(File.ReadAllText(Absolute("Content/provenance.json")));
        var root = provenance.RootElement;
        var groups = root.GetProperty("assets").EnumerateArray().ToDictionary(
            item => item.GetProperty("id").GetString()!,
            StringComparer.Ordinal);
        var rights = File.ReadAllText(Absolute("docs/ASSET_RIGHTS_DECLARATION.md"));
        var coverage = File.ReadAllText(Absolute("docs/MATURA_2026_EXTENDED_COVERAGE.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));

        Assert.Equal("approved", groups["cke-2026-main-extended-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("blocked", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
        Assert.Contains("MMAP-R0-100-A-2605", rights, StringComparison.Ordinal);
        Assert.Contains(PaperHash, rights, StringComparison.Ordinal);
        Assert.Contains(RulesHash, rights, StringComparison.Ordinal);
        Assert.Contains("autorskich implementacji wektorowych Avalonia", rights, StringComparison.Ordinal);
        Assert.Contains(PaperHash, coverage, StringComparison.Ordinal);
        Assert.Contains(RulesHash, coverage, StringComparison.Ordinal);
        Assert.Contains("MATURA_2026_EXTENDED_COVERAGE.md", toc, StringComparison.Ordinal);
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
        File.ReadAllText(Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar))),
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
