using System.Text.Json;
using Abituria.Models;
using Abituria.Services;
using Avalonia.Headless.XUnit;

namespace Abituria.Tests;

public sealed class Matura2018ContentTests
{
    private const string MainBasicPaperUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2018/formula_od_2015/matematyka/MMA-P1_1P-182.pdf";
    private const string MainBasicRulesUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2018/formula_od_2015/Zasady_oceniania/MMA-P1_1P-182_zasady_oceniania.pdf";
    private const string MainExtendedPaperUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2018/formula_od_2015/matematyka/MMA-R1_1P-182.pdf";
    private const string MainExtendedRulesUrl = "https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2018/Matematyka/MMA-R1_1P-182_zasady_oceniania.pdf";
    private const string CorrectionPaperUrl = "https://arkusze.pl/maturalne/matematyka-2018-sierpien-poprawkowa-podstawowa.pdf";
    private const string CorrectionRulesUrl = "https://arkusze.pl/maturalne/matematyka-2018-sierpien-poprawkowa-podstawowa-odpowiedzi.pdf";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();
    private static readonly string[] ExpectedVectorDiagramIds =
    [
        "exam-mm18-p0-z14", "exam-mm18-p0-z16", "exam-mm18-p0-z17", "exam-mm18-p0-z20", "exam-mm18-p0-z21", "exam-mm18-p0-z22", "exam-mm18-p0-z29", "exam-mm18-p0-z34",
        "exam-mm18-r0-z07", "exam-mm18-r0-z10", "exam-mm18-r0-z14", "exam-mm18-r0-z15",
        "exam-mm18-p0p-z06", "exam-mm18-p0p-z17", "exam-mm18-p0p-z22", "exam-mm18-p0p-z28", "exam-mm18-p0p-z31", "exam-mm18-p0p-z32", "exam-mm18-p0p-z34"
    ];
    private static readonly string[] ExpectedDiagramSourceIds =
    ["cke-2018-main-basic-exam", "cke-2018-main-extended-exam", "cke-2018-correction-basic-exam"];

    [Theory]
    [InlineData("Content/exam-2018-main-basic.json", "matura-maj-2018-podstawowa", "główna", "basic", 170, 34, 34, 50)]
    [InlineData("Content/exam-2018-main-extended.json", "matura-maj-2018-rozszerzona", "główna", "extended", 180, 15, 15, 50)]
    [InlineData("Content/exam-2018-correction-basic.json", "matura-poprawkowa-2018-podstawowa", "poprawkowa", "basic", 170, 34, 34, 50)]
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
        Assert.Equal(2018, exam.Year);
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
            Assert.StartsWith("mm18-", exercise.Id, StringComparison.Ordinal);
            Assert.InRange(exercise.Id.Length, 1, 79);
            Assert.Equal(id, exercise.ExamId);
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.True(exercise.Hints.Count >= 2);
            Assert.False(string.IsNullOrWhiteSpace(exercise.EffectiveSolution));
            Assert.False(string.IsNullOrWhiteSpace(exercise.ScoringCriteria));
            Assert.InRange(exercise.SourcePage, 2, 24);
            Assert.InRange(exercise.SolutionSourcePage, 2, 31);
        });

        Assert.Equal("2026-09-01", exam.Source.VerifiedOn);
        Assert.Equal(64, exam.Source.QuestionPaperSha256.Length);
        Assert.Equal(64, exam.Source.AnswerKeySha256.Length);
        Assert.All(
            new[] { exam.Source.QuestionPaperSha256, exam.Source.AnswerKeySha256 },
            hash => Assert.True(hash.All(character => char.IsAsciiHexDigit(character) && !char.IsLower(character))));
    }

    [Fact]
    public void Main_basic_exam_preserves_pages_answer_modes_and_numeric_answers()
    {
        var exam = Read<ExamCatalog>("Content/exam-2018-main-basic.json").Exam;
        int[] expectedPages =
        [
            2, 2, 2, 2, 2, 2, 4, 4, 4, 4, 4, 6, 6, 6, 6, 8, 8, 8, 8, 10, 10, 10, 12, 12, 12, 14, 15, 16, 17, 18, 19, 20, 22, 24
        ];
        int[] expectedAnswers =
        [
            2, 3, 3, 3, 1, 3, 4, 4, 3, 4, 1, 1, 2, 3, 1, 1, 2, 2, 2, 4, 1, 1, 2, 4, 4
        ];

        Assert.Equal(expectedPages, exam.Exercises.Select(item => item.SourcePage));
        Assert.Equal(expectedAnswers, exam.Exercises.Take(25).Select(item => item.CorrectOption!.Value));
        Assert.Equal(25, exam.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(3, exam.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(6, exam.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(-3d, exam.Exercises.Single(item => item.Id == "mm18-p0-z31").ExpectedValue!.Value, 12);
        Assert.Equal(16d / 49d, exam.Exercises.Single(item => item.Id == "mm18-p0-z33").ExpectedValue!.Value, 12);
        Assert.Equal(40.5d, exam.Exercises.Single(item => item.Id == "mm18-p0-z34").ExpectedValue!.Value, 12);
        Assert.Equal([2, 2, 2, 2, 2, 2, 5, 4, 4], exam.Exercises.Skip(25).Select(item => item.Points));
    }

    [Fact]
    public void Pinned_2018_sources_have_exact_urls_hashes_and_exam_dates()
    {
        var mainBasic = Read<ExamCatalog>("Content/exam-2018-main-basic.json").Exam.Source;
        var mainExtended = Read<ExamCatalog>("Content/exam-2018-main-extended.json").Exam.Source;
        var correction = Read<ExamCatalog>("Content/exam-2018-correction-basic.json").Exam.Source;

        Assert.Equal(("MMA-P1_1P-182", "2018-05-07", MainBasicPaperUrl, "D19CC243C36128FFEE5BBF5ECA511CCE18A436C75C10FDF293A46C59156746AA", MainBasicRulesUrl, "A6067EB5B97EAAD2373ED82134193AE9F542886FDF7A003816091AF24324DCE9"),
            (mainBasic.DocumentCode, mainBasic.ExamDate, mainBasic.QuestionPaperUrl, mainBasic.QuestionPaperSha256, mainBasic.AnswerKeyUrl, mainBasic.AnswerKeySha256));
        Assert.Equal(("MMA-R1_1P-182", "2018-05-09", MainExtendedPaperUrl, "7EFD8731C3DDD97F4CFACF7E4D450F7E8C818FDE5318C57F4CA320F1280E42DD", MainExtendedRulesUrl, "A8874470FB79681CDB8D39A35D55016D21D7378BB501E7C815CFA21DB2C1CD7F"),
            (mainExtended.DocumentCode, mainExtended.ExamDate, mainExtended.QuestionPaperUrl, mainExtended.QuestionPaperSha256, mainExtended.AnswerKeyUrl, mainExtended.AnswerKeySha256));
        Assert.Equal(("MMA-P1_1P-184", "2018-08-21", CorrectionPaperUrl, "7DFB5C1A05A34126483B425D3958D1B13FCD3760B0778FEEFF7B662E7A7F5BC2", CorrectionRulesUrl, "F9C9FCB9CCC03544F1960E0026CBD3E0627EF8D1CB2903DBC504D5031708D7DB"),
            (correction.DocumentCode, correction.ExamDate, correction.QuestionPaperUrl, correction.QuestionPaperSha256, correction.AnswerKeyUrl, correction.AnswerKeySha256));
        Assert.All([mainBasic, mainExtended, correction], source => Assert.Equal("2026-09-01", source.VerifiedOn));

        var basicCoverage = File.ReadAllText(Absolute("docs/MATURA_2018_BASIC_COVERAGE.md"));
        var extendedCoverage = File.ReadAllText(Absolute("docs/MATURA_2018_EXTENDED_COVERAGE.md"));
        var correctionCoverage = File.ReadAllText(Absolute("docs/MATURA_2018_CORRECTION_BASIC_COVERAGE.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));
        Assert.Contains(mainBasic.QuestionPaperSha256, basicCoverage, StringComparison.Ordinal);
        Assert.Contains(mainExtended.QuestionPaperSha256, extendedCoverage, StringComparison.Ordinal);
        Assert.Contains(correction.QuestionPaperSha256, correctionCoverage, StringComparison.Ordinal);
        Assert.Contains("MATURA_2018_BASIC_COVERAGE.md", toc, StringComparison.Ordinal);
        Assert.Contains("MATURA_2018_EXTENDED_COVERAGE.md", toc, StringComparison.Ordinal);
        Assert.Contains("MATURA_2018_CORRECTION_BASIC_COVERAGE.md", toc, StringComparison.Ordinal);
    }

    [Fact]
    public void Extended_and_correction_exams_preserve_scoring_modes_and_numeric_answers()
    {
        var extended = Read<ExamCatalog>("Content/exam-2018-main-extended.json").Exam;
        var correction = Read<ExamCatalog>("Content/exam-2018-correction-basic.json").Exam;

        Assert.Equal([1, 1, 1, 1, 2, 3, 3, 3, 4, 4, 4, 6, 4, 6, 7], extended.Exercises.Select(item => item.Points));
        Assert.Equal(4, extended.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(4, extended.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(7, extended.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(166d, extended.Exercises.Single(item => item.Id == "mm18-r0-z05").ExpectedValue!.Value, 12);
        Assert.Equal(5d / 14d, extended.Exercises.Single(item => item.Id == "mm18-r0-z09").ExpectedValue!.Value, 12);
        Assert.Equal(9d / Math.Sqrt(106d), extended.Exercises.Single(item => item.Id == "mm18-r0-z10").ExpectedValue!.Value, 12);
        Assert.Equal(15d, extended.Exercises.Single(item => item.Id == "mm18-r0-z13").ExpectedValue!.Value, 12);

        Assert.Equal(25, correction.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(3, correction.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(6, correction.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(2d * Math.Sqrt(7d) / 7d, correction.Exercises.Single(item => item.Id == "mm18-p0p-z32").ExpectedValue!.Value, 12);
        Assert.Equal(1d / 8d, correction.Exercises.Single(item => item.Id == "mm18-p0p-z33").ExpectedValue!.Value, 12);
        Assert.Equal(30d, correction.Exercises.Single(item => item.Id == "mm18-p0p-z34").ExpectedValue!.Value, 12);
    }

    [Fact]
    public void Source_fidelity_regressions_preserve_visually_audited_2018_values()
    {
        var basic = Read<ExamCatalog>("Content/exam-2018-main-basic.json").Exam;
        var extended = Read<ExamCatalog>("Content/exam-2018-main-extended.json").Exam;
        var correction = Read<ExamCatalog>("Content/exam-2018-correction-basic.json").Exam;

        var basicSequence = basic.Exercises.Single(item => item.Id == "mm18-p0-z13");
        Assert.Contains("a₁ = √2, a₂ = 2√2, a₃ = 4√2", basicSequence.Prompt, StringComparison.Ordinal);
        Assert.Equal(["(√2)ⁿ", "2ⁿ/√2", "(√2/2)ⁿ", "(√2)ⁿ/2"], basicSequence.Options);
        Assert.Contains("q = 2", basicSequence.EffectiveSolution, StringComparison.Ordinal);
        Assert.Contains("1/(2a) + 1/(2b) ≥ 2/(a + b)", basic.Exercises.Single(item => item.Id == "mm18-p0-z28").Prompt, StringComparison.Ordinal);
        var basicAnalyticGeometry = basic.Exercises.Single(item => item.Id == "mm18-p0-z32");
        Assert.Contains("B = (10, 5)", basicAnalyticGeometry.Prompt, StringComparison.Ordinal);
        Assert.Contains("y - 5 = -3(x - 10)", basicAnalyticGeometry.EffectiveSolution, StringComparison.Ordinal);
        Assert.Contains("a = 6", basic.Exercises.Single(item => item.Id == "mm18-p0-z34").EffectiveSolution, StringComparison.Ordinal);

        var extendedHomography = extended.Exercises.Single(item => item.Id == "mm18-r0-z05");
        Assert.Contains("A = (-5, 3)", extendedHomography.Prompt, StringComparison.Ordinal);
        Assert.Contains("ilorazu d/a", extendedHomography.Prompt, StringComparison.Ordinal);
        var extendedTangent = extended.Exercises.Single(item => item.Id == "mm18-r0-z06");
        Assert.Contains("y = √3x² - 1", extendedTangent.Prompt, StringComparison.Ordinal);
        Assert.Contains("x₀ = 1/6", extendedTangent.EffectiveSolution, StringComparison.Ordinal);
        var extendedParameter = extended.Exercises.Single(item => item.Id == "mm18-r0-z12");
        Assert.Contains("- m² + 1", extendedParameter.Prompt, StringComparison.Ordinal);
        Assert.Contains("(-∞, -3) ∪ (3/5, 3/4)", extendedParameter.EffectiveSolution, StringComparison.Ordinal);
        Assert.Contains("A = (7, -1)", extended.Exercises.Single(item => item.Id == "mm18-r0-z14").Prompt, StringComparison.Ordinal);
        Assert.Contains("a = √2", extended.Exercises.Single(item => item.Id == "mm18-r0-z15").EffectiveSolution, StringComparison.Ordinal);

        var correctionAnalyticGeometry = correction.Exercises.Single(item => item.Id == "mm18-p0p-z31");
        Assert.Contains("C = (4, -2)", correctionAnalyticGeometry.Prompt, StringComparison.Ordinal);
        Assert.Contains("D = (3, 1)", correctionAnalyticGeometry.EffectiveSolution, StringComparison.Ordinal);
        Assert.Contains("B = {-1, 0, 1, 2}", correction.Exercises.Single(item => item.Id == "mm18-p0p-z33").Prompt, StringComparison.Ordinal);
    }

    [Fact]
    public void Nineteen_vector_definitions_are_referenced_by_2018_exams_and_are_approved()
    {
        var basic = Read<ExamCatalog>("Content/exam-2018-main-basic.json").Exam;
        var extended = Read<ExamCatalog>("Content/exam-2018-main-extended.json").Exam;
        var correction = Read<ExamCatalog>("Content/exam-2018-correction-basic.json").Exam;
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
        var definitions = diagrams.Diagrams.Where(item => item.Id.StartsWith("exam-mm18-", StringComparison.Ordinal))
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
        Assert.Equal("approved", groups["cke-2018-main-basic-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("approved", groups["cke-2018-main-extended-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("approved", groups["cke-2018-correction-basic-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("blocked", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
        DiagramCatalogValidator.Validate(diagrams);
    }

    [AvaloniaFact]
    public void Catalog_retains_2018_before_the_new_2017_formula_2015_year()
    {
        var repository = new ContentRepository();

        Assert.Equal(46, repository.Exams.Count);
        Assert.Equal("matura-maj-2018-podstawowa", repository.Exams[34].Id);
        Assert.Equal("matura-maj-2018-rozszerzona", repository.Exams[35].Id);
        Assert.Equal("matura-poprawkowa-2018-podstawowa", repository.Exams[36].Id);
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
