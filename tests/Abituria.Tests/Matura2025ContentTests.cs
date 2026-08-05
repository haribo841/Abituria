using System.Text.Json;
using Abituria.Models;

namespace Abituria.Tests;

public sealed class Matura2025ContentTests
{
    private const string BasicExamId = "matura-maj-2025-podstawowa";
    private const string ExtendedExamId = "matura-maj-2025-rozszerzona";
    private const string BasicPaperHash = "C5F8AFDE91393BEA3E5980560ADA103389679473DBD0C11A7485040F06631C85";
    private const string BasicRulesHash = "D272201B35AD7829315C6897500F036A8619BBEE42B38291037DF952F9F150E5";
    private const string ExtendedPaperHash = "457B057602D81CF93A9688E7F4CB74103F4579B37C2B1A2A9AACE28C891CD4AD";
    private const string ExtendedRulesHash = "B196084F2B9505D14C66E3CBE0064BBA7E4BA0F3FFA613500ED701A97724E523";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Basic_exam_contract_is_exactly_31_tasks_35_progress_items_and_50_points()
    {
        var catalog = Read<ExamCatalog>("Content/exam-2025-main-basic.json");
        var exam = catalog.Exam;
        string[] expectedLabels =
        [
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12.1", "12.2", "12.3",
            "13", "14.1", "14.2", "15", "16", "17", "18.1", "18.2", "19", "20", "21", "22", "23",
            "24", "25", "26", "27", "28", "29", "30", "31"
        ];
        int[] expectedSourcePages =
        [
            4, 4, 5, 5, 6, 7, 8, 8, 9, 10, 11, 12, 13, 13, 14, 15, 15, 16, 17, 17, 18, 18,
            19, 20, 21, 22, 22, 23, 24, 25, 25, 26, 26, 27, 28
        ];
        int[] expectedSolutionPages =
        [
            2, 3, 3, 4, 4, 7, 7, 8, 8, 10, 12, 13, 15, 15, 16, 16, 17, 17, 22, 22, 23, 23,
            24, 24, 25, 25, 26, 26, 27, 31, 31, 32, 32, 33, 33
        ];
        int[] expectedPoints =
        [
            1, 1, 1, 1, 2, 1, 1, 1, 2, 2, 4, 2, 1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 3, 1, 1, 1, 1, 2, 4
        ];

        AssertExamContract(catalog, BasicExamId, "Matura maj 2025 - poziom podstawowy", "basic", 31, 35);
        Assert.Equal(expectedLabels, exam.Exercises.Select(item => item.DisplayNumber));
        Assert.Equal(expectedSourcePages, exam.Exercises.Select(item => item.SourcePage));
        Assert.Equal(expectedSolutionPages, exam.Exercises.Select(item => item.SolutionSourcePage));
        Assert.Equal(expectedPoints, exam.Exercises.Select(item => item.Points));
        Assert.Equal(Enumerable.Range(1, 31), exam.Exercises.Select(item => item.Number).Distinct());
        Assert.Equal(31, exam.Exercises.Select(OfficialGroupId).Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Extended_exam_contract_is_exactly_12_tasks_13_progress_items_and_50_points()
    {
        var catalog = Read<ExamCatalog>("Content/exam-2025-main-extended.json");
        var exam = catalog.Exam;
        string[] expectedLabels = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12.1", "12.2"];
        int[] expectedSourcePages = [4, 5, 6, 8, 10, 12, 14, 16, 18, 20, 22, 25, 26];
        int[] expectedSolutionPages = [2, 3, 5, 12, 13, 16, 21, 25, 28, 37, 43, 47, 48];
        int[] expectedPoints = [2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 2, 4];

        AssertExamContract(catalog, ExtendedExamId, "Matura maj 2025 - poziom rozszerzony", "extended", 12, 13);
        Assert.Equal(expectedLabels, exam.Exercises.Select(item => item.DisplayNumber));
        Assert.Equal(expectedSourcePages, exam.Exercises.Select(item => item.SourcePage));
        Assert.Equal(expectedSolutionPages, exam.Exercises.Select(item => item.SolutionSourcePage));
        Assert.Equal(expectedPoints, exam.Exercises.Select(item => item.Points));
        Assert.Equal(Enumerable.Range(1, 12), exam.Exercises.Select(item => item.Number).Distinct());
        Assert.Equal(12, exam.Exercises.Select(OfficialGroupId).Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Official_sources_hashes_dates_and_verification_are_pinned()
    {
        var basic = Read<ExamCatalog>("Content/exam-2025-main-basic.json").Exam.Source;
        var extended = Read<ExamCatalog>("Content/exam-2025-main-extended.json").Exam.Source;

        AssertSource(
            basic,
            "MMAP-P0-100-A-2505",
            "2025-05-06",
            "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2025/Matematyka/poziom_podstawowy/MMAP-P0-100-A-2505-arkusz.pdf",
            BasicPaperHash,
            "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2025/zasady_oceniania/MMAP-P0-100-2505-zasady.pdf",
            BasicRulesHash);
        AssertSource(
            extended,
            "MMAP-R0-100-A-2505",
            "2025-05-12",
            "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2025/Matematyka/poziom_rozszerzony/MMAP-R0-100-A-2505-arkusz.pdf",
            ExtendedPaperHash,
            "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2025/zasady_oceniania/MMAP-R0-100-2505-zasady.pdf",
            ExtendedRulesHash);
    }

    [Fact]
    public void Answer_modes_and_results_match_the_verified_marking_rules()
    {
        var basic = Read<ExamCatalog>("Content/exam-2025-main-basic.json").Exam;
        var extended = Read<ExamCatalog>("Content/exam-2025-main-extended.json").Exam;
        int[] basicChoiceKey = [2, 2, 1, 3, 4, 1, 3, 1, 3, 4, 2, 3, 4, 1, 3, 2, 3, 2, 2, 1, 1, 4, 3];

        Assert.Equal(23, basic.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(4, basic.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(4, basic.Exercises.Count(item => item.IsCompound));
        Assert.Equal(4, basic.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(basicChoiceKey, basic.Exercises.Where(item => item.IsMultipleChoice).Select(item => item.CorrectOption!.Value));
        AssertNumericValues(basic,
            ("mm25-p0-z09", 735_000d),
            ("mm25-p0-z12-3", 6d),
            ("mm25-p0-z15", 2.5d),
            ("mm25-p0-z25", 64d * Math.PI));
        Assert.Equal([4.5d, 6d], CompoundNumericValues(basic, "mm25-p0-z30"), new DoublePrecisionComparer(12));
        Assert.Equal([2, 2], CompoundChoiceKey(basic, "mm25-p0-z14-2"));
        Assert.Equal([2, 1], CompoundChoiceKey(basic, "mm25-p0-z21"));

        Assert.Equal(4, extended.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(3, extended.Exercises.Count(item => item.IsCompound));
        Assert.Equal(6, extended.Exercises.Count(item => item.IsRevealOnly));
        Assert.DoesNotContain(extended.Exercises, item => item.IsMultipleChoice);
        AssertNumericValues(extended,
            ("mm25-r0-z01", 25d),
            ("mm25-r0-z03", 45d),
            ("mm25-r0-z04", 9d / 25d),
            ("mm25-r0-z10", 918d));
        Assert.Equal([13.5d, 27d], CompoundNumericValues(extended, "mm25-r0-z06"), new DoublePrecisionComparer(12));
        Assert.Equal(
            [3.2d, -2.6d, -1d, -2d, 0.4d, -2.2d],
            CompoundNumericValues(extended, "mm25-r0-z08"),
            new DoublePrecisionComparer(12));
        Assert.Equal(
            [5d * Math.Sqrt(3d), 125d * Math.Sqrt(3d) * Math.PI / 2d],
            CompoundNumericValues(extended, "mm25-r0-z12-2"),
            new DoublePrecisionComparer(12));
    }

    [Fact]
    public void Basic_figures_and_release_provenance_are_complete()
    {
        var basic = Read<ExamCatalog>("Content/exam-2025-main-basic.json").Exam;
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        var referenced = basic.Exercises.SelectMany(item => item.DiagramIds)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var definitions = diagrams.Diagrams.Where(item => item.SourceId == "cke-2025-main-basic")
            .OrderBy(item => item.Id)
            .ToArray();
        using var provenance = JsonDocument.Parse(File.ReadAllText(Absolute("Content/provenance.json")));
        var groups = provenance.RootElement.GetProperty("assets").EnumerateArray().ToDictionary(
            item => item.GetProperty("id").GetString()!,
            StringComparer.Ordinal);
        var rights = File.ReadAllText(Absolute("docs/ASSET_RIGHTS_DECLARATION.md"));
        var coverage = File.ReadAllText(Absolute("docs/MATURA_2025_COVERAGE.md"));
        var extendedCoverage = File.ReadAllText(Absolute("docs/MATURA_2025_EXTENDED_COVERAGE.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));

        Assert.Equal(9, referenced.Length);
        Assert.Equal(referenced, definitions.Select(item => item.Id));
        Assert.Equal([7, 11, 12, 18, 19, 20, 21, 27, 28], definitions.Select(item => item.SourcePage));
        Assert.All(definitions, item =>
        {
            Assert.False(string.IsNullOrWhiteSpace(item.AlternativeText));
            Assert.NotEmpty(item.Primitives);
        });
        Assert.True(provenance.RootElement.GetProperty("releaseEligible").GetBoolean());
        Assert.Equal("approved", groups["cke-2025-main-basic-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("approved", groups["cke-2025-main-extended-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("approved", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
        Assert.Contains(BasicPaperHash, rights, StringComparison.Ordinal);
        Assert.Contains(ExtendedRulesHash, rights, StringComparison.Ordinal);
        Assert.Contains(BasicPaperHash, coverage, StringComparison.Ordinal);
        Assert.Contains(ExtendedRulesHash, extendedCoverage, StringComparison.Ordinal);
        Assert.Contains("MATURA_2025_COVERAGE.md", toc, StringComparison.Ordinal);
        Assert.Contains("MATURA_2025_EXTENDED_COVERAGE.md", toc, StringComparison.Ordinal);
    }

    private static void AssertExamContract(
        ExamCatalog catalog,
        string expectedId,
        string expectedTitle,
        string expectedLevel,
        int expectedOfficialTasks,
        int expectedProgressItems)
    {
        var exam = catalog.Exam;
        Assert.Equal(4, catalog.SchemaVersion);
        Assert.Equal(expectedId, exam.Id);
        Assert.Equal(expectedTitle, exam.Title);
        Assert.Equal(2025, exam.Year);
        Assert.Equal("główna", exam.Session);
        Assert.Equal("2023", exam.Formula);
        Assert.Equal(expectedLevel, exam.Level);
        Assert.Equal(180, exam.DurationMinutes);
        Assert.Equal(50, exam.MaximumPoints);
        Assert.Equal(expectedOfficialTasks, exam.OfficialTaskCount);
        Assert.Equal(expectedProgressItems, exam.ProgressItemCount);
        Assert.Equal(expectedProgressItems, exam.Exercises.Count);
        Assert.Equal(50, exam.Exercises.Sum(item => item.Points));
        Assert.Equal(Enumerable.Range(1, expectedProgressItems), exam.Exercises.Select(item => item.EffectiveOrder));
        Assert.Equal(expectedProgressItems, exam.Exercises.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.All(exam.Exercises, exercise =>
        {
            Assert.InRange(exercise.Id.Length, 1, 79);
            Assert.Equal(expectedId, exercise.ExamId);
            Assert.Equal($"Zadanie {exercise.DisplayNumber}", exercise.Title);
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.False(string.IsNullOrWhiteSpace(exercise.EffectiveSolution));
            Assert.False(string.IsNullOrWhiteSpace(exercise.ScoringCriteria));
        });
    }

    private static void AssertSource(
        SourceDocument source,
        string code,
        string examDate,
        string paperUrl,
        string paperHash,
        string rulesUrl,
        string rulesHash)
    {
        Assert.Equal("Centralna Komisja Egzaminacyjna", source.Publisher);
        Assert.Equal(code, source.DocumentCode);
        Assert.Equal(examDate, source.ExamDate);
        Assert.Equal(paperUrl, source.QuestionPaperUrl);
        Assert.Equal(paperHash, source.QuestionPaperSha256);
        Assert.Equal(rulesUrl, source.AnswerKeyUrl);
        Assert.Equal(rulesHash, source.AnswerKeySha256);
        Assert.Equal("2026-08-05", source.VerifiedOn);
        Assert.All(new[] { paperHash, rulesHash }, hash =>
        {
            Assert.Equal(64, hash.Length);
            Assert.True(hash.All(character => char.IsAsciiHexDigit(character) && !char.IsLower(character)));
        });
    }

    private static void AssertNumericValues(ExamDefinition exam, params (string Id, double Value)[] expected)
    {
        Assert.Equal(expected.Select(item => item.Id).Order(), exam.Exercises.Where(item => item.IsNumeric).Select(item => item.Id).Order());
        foreach (var item in expected)
            Assert.Equal(item.Value, exam.Exercises.Single(exercise => exercise.Id == item.Id).ExpectedValue!.Value, 12);
    }

    private static string OfficialGroupId(LearningExercise exercise) =>
        string.IsNullOrWhiteSpace(exercise.GroupId) ? exercise.Id : exercise.GroupId;

    private static IEnumerable<double> CompoundNumericValues(ExamDefinition exam, string exerciseId) =>
        exam.Exercises.Single(item => item.Id == exerciseId).AnswerParts.Select(item => item.ExpectedValue!.Value);

    private static IEnumerable<int> CompoundChoiceKey(ExamDefinition exam, string exerciseId) =>
        exam.Exercises.Single(item => item.Id == exerciseId).AnswerParts.Select(item => item.CorrectOption!.Value);

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
