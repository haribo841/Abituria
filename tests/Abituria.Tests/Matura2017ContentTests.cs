using System.Text.Json;
using Abituria.Models;

namespace Abituria.Tests;

public sealed class Matura2017ContentTests
{
    private const string MainBasicPaperUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2017/formula_od_2015/matematyka/MMA-P1_1P-172.pdf";
    private const string MainBasicRulesUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2017/formula_od_2015/zasady_oceniania/MMA-P1-N.pdf";
    private const string MainExtendedPaperUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2017/formula_od_2015/matematyka/MMA-R1_1P-172.pdf";
    private const string MainExtendedRulesUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2017/formula_od_2015/zasady_oceniania/MMA-R1-N.pdf";
    private const string CorrectionPaperUrl = "https://arkusze.pl/maturalne/matematyka-2017-sierpien-poprawkowa-podstawowa.pdf";
    private const string CorrectionRulesUrl = "https://arkusze.pl/maturalne/matematyka-2017-sierpien-poprawkowa-podstawowa-odpowiedzi.pdf";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();
    private static readonly string[] ExpectedVectorDiagramIds =
    [
        "exam-mm17-p0-z07", "exam-mm17-p0-z10", "exam-mm17-p0-z11", "exam-mm17-p0-z15", "exam-mm17-p0-z16", "exam-mm17-p0-z17", "exam-mm17-p0-z22",
        "exam-mm17-r0-z03", "exam-mm17-r0-z08", "exam-mm17-r0-z09",
        "exam-mm17-p0p-z06", "exam-mm17-p0p-z10", "exam-mm17-p0p-z14", "exam-mm17-p0p-z15", "exam-mm17-p0p-z18", "exam-mm17-p0p-z21"
    ];
    private static readonly string[] ExpectedDiagramSourceIds =
    [
        "cke-2017-main-basic-exam",
        "cke-2017-main-extended-exam",
        "cke-2017-correction-basic-exam"
    ];

    [Theory]
    [InlineData("Content/exam-2017-main-basic.json", "matura-maj-2017-podstawowa", "główna", "basic", 170, 34, 34, 50)]
    [InlineData("Content/exam-2017-main-extended.json", "matura-maj-2017-rozszerzona", "główna", "extended", 180, 15, 15, 50)]
    [InlineData("Content/exam-2017-correction-basic.json", "matura-poprawkowa-2017-podstawowa", "poprawkowa", "basic", 170, 34, 34, 50)]
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
        Assert.Equal(2017, exam.Year);
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
            Assert.StartsWith("mm17-", exercise.Id, StringComparison.Ordinal);
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
    public void Pinned_sources_have_exact_urls_hashes_and_dates()
    {
        var basic = Read<ExamCatalog>("Content/exam-2017-main-basic.json").Exam.Source;
        var extended = Read<ExamCatalog>("Content/exam-2017-main-extended.json").Exam.Source;
        var correction = Read<ExamCatalog>("Content/exam-2017-correction-basic.json").Exam.Source;

        Assert.Equal(
            ("MMA-P1_1P-172", "2017-05-05", MainBasicPaperUrl, "AA9506313B735854BBFEC309F8C0C6496AD168E332AF63B9A58CDA7822F0BBAF", MainBasicRulesUrl, "76866211FF1979E816A5F2091DB29ED0123D6921A51AE2BD2C928808601CD693"),
            (basic.DocumentCode, basic.ExamDate, basic.QuestionPaperUrl, basic.QuestionPaperSha256, basic.AnswerKeyUrl, basic.AnswerKeySha256));
        Assert.Equal(
            ("MMA-R1_1P-172", "2017-05-09", MainExtendedPaperUrl, "4D6F682AA0BB350CA67CA4F2CAE967332264339DA1AF006D5A4F0C5F23EB0277", MainExtendedRulesUrl, "2B3DDFC0161632B6F6E5FD5CBC47BAE14FE618942D7636D4B870F9C8B3F4E36C"),
            (extended.DocumentCode, extended.ExamDate, extended.QuestionPaperUrl, extended.QuestionPaperSha256, extended.AnswerKeyUrl, extended.AnswerKeySha256));
        Assert.Equal(
            ("MMA-P1_1P-174", "2017-08-22", CorrectionPaperUrl, "D3975FF6D80430737C1D7186143BFC79F42EFBA06E749128447E2D39E42BD4A9", CorrectionRulesUrl, "1295525A054ECDD6E1CF132AD4C13B54E29C79AAFC63C723508061A7C211266C"),
            (correction.DocumentCode, correction.ExamDate, correction.QuestionPaperUrl, correction.QuestionPaperSha256, correction.AnswerKeyUrl, correction.AnswerKeySha256));

        var basicCoverage = File.ReadAllText(Absolute("docs/MATURA_2017_BASIC_COVERAGE.md"));
        var extendedCoverage = File.ReadAllText(Absolute("docs/MATURA_2017_EXTENDED_COVERAGE.md"));
        var correctionCoverage = File.ReadAllText(Absolute("docs/MATURA_2017_CORRECTION_BASIC_COVERAGE.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));

        Assert.Contains(basic.QuestionPaperSha256, basicCoverage, StringComparison.Ordinal);
        Assert.Contains(extended.QuestionPaperSha256, extendedCoverage, StringComparison.Ordinal);
        Assert.Contains(correction.QuestionPaperSha256, correctionCoverage, StringComparison.Ordinal);
        Assert.Contains("MATURA_2017_BASIC_COVERAGE.md", toc, StringComparison.Ordinal);
        Assert.Contains("MATURA_2017_EXTENDED_COVERAGE.md", toc, StringComparison.Ordinal);
        Assert.Contains("MATURA_2017_CORRECTION_BASIC_COVERAGE.md", toc, StringComparison.Ordinal);
    }

    [Fact]
    public void Answer_modes_and_numerical_results_match_the_audited_sources()
    {
        var basic = Read<ExamCatalog>("Content/exam-2017-main-basic.json").Exam;
        var extended = Read<ExamCatalog>("Content/exam-2017-main-extended.json").Exam;
        var correction = Read<ExamCatalog>("Content/exam-2017-correction-basic.json").Exam;

        Assert.Equal(
            [1, 3, 1, 1, 3, 4, 4, 3, 3, 3, 4, 2, 1, 2, 3, 2, 3, 2, 4, 1, 1, 2, 4, 4, 2],
            basic.Exercises.Take(25).Select(item => item.CorrectOption!.Value));
        Assert.Equal(25, basic.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(5, basic.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(4, basic.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(-16d / 9d, GetExpectedValue(basic, "mm17-p0-z29"), 12);
        Assert.Equal(243d / 7d, GetExpectedValue(basic, "mm17-p0-z32"), 12);
        Assert.Equal(1d / 9d, GetExpectedValue(basic, "mm17-p0-z33"), 12);

        Assert.Equal(4, extended.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(2, extended.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(9, extended.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(125d, GetExpectedValue(extended, "mm17-r0-z05"), 12);
        Assert.Equal(11d / 16d, GetExpectedValue(extended, "mm17-r0-z11"), 12);

        Assert.Equal(25, correction.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(4, correction.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(5, correction.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(12d / 25d, GetExpectedValue(correction, "mm17-p0p-z30"), 12);
        Assert.Equal(-16d / 3d, GetExpectedValue(correction, "mm17-p0p-z32"), 12);
        Assert.Equal(192d, GetExpectedValue(correction, "mm17-p0p-z34"), 12);
    }

    [Fact]
    public void Vector_definitions_are_referenced_and_new_material_is_safely_blocked()
    {
        var basic = Read<ExamCatalog>("Content/exam-2017-main-basic.json").Exam;
        var extended = Read<ExamCatalog>("Content/exam-2017-main-extended.json").Exam;
        var correction = Read<ExamCatalog>("Content/exam-2017-correction-basic.json").Exam;
        var expected = ExpectedVectorDiagramIds.Order(StringComparer.Ordinal).ToArray();
        var actual = basic.Exercises
            .Concat(extended.Exercises)
            .Concat(correction.Exercises)
            .SelectMany(item => item.DiagramIds)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expected, actual);

        var diagramDocument = JsonDocument.Parse(File.ReadAllText(Absolute("Content/diagrams.json")));
        var definitions = diagramDocument.RootElement.GetProperty("diagrams")
            .EnumerateArray()
            .Where(item => ExpectedDiagramSourceIds.Contains(item.GetProperty("sourceId").GetString(), StringComparer.Ordinal))
            .ToArray();

        Assert.Equal(16, definitions.Length);
        Assert.All(definitions, definition =>
        {
            Assert.True(definition.GetProperty("sourcePage").GetInt32() > 0);
            Assert.False(string.IsNullOrWhiteSpace(definition.GetProperty("alternativeText").GetString()));
        });

        var manifest = JsonDocument.Parse(File.ReadAllText(Absolute("Content/provenance.json")));
        Assert.False(manifest.RootElement.GetProperty("releaseEligible").GetBoolean());
        var groups = manifest.RootElement.GetProperty("assets")
            .EnumerateArray()
            .ToDictionary(item => item.GetProperty("id").GetString()!, StringComparer.Ordinal);

        foreach (var groupId in ExpectedDiagramSourceIds)
        {
            Assert.Equal("blocked", groups[groupId].GetProperty("distributionStatus").GetString());
            Assert.False(string.IsNullOrWhiteSpace(groups[groupId].GetProperty("blockedReason").GetString()));
        }

        Assert.Equal("blocked", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
    }

    [Fact]
    public void Catalog_exposes_2017_after_the_complete_2018_triplet()
    {
        var index = Read<ExamIndexCatalog>("Content/exams.json");
        var exams = index.Exams
            .OrderBy(item => item.Order)
            .Select(item => Read<ExamCatalog>(item.ContentPath).Exam)
            .ToArray();

        Assert.Equal(46, exams.Length);
        Assert.Equal("matura-maj-2018-podstawowa", exams[34].Id);
        Assert.Equal("matura-poprawkowa-2018-podstawowa", exams[36].Id);
        Assert.Equal("matura-maj-2017-podstawowa", exams[37].Id);
        Assert.Equal("matura-maj-2017-rozszerzona", exams[38].Id);
        Assert.Equal("matura-poprawkowa-2017-podstawowa", exams[39].Id);
        Assert.Equal(1_281, exams.Sum(exam => exam.Exercises.Count));
    }

    private static double GetExpectedValue(ExamDefinition exam, string exerciseId) =>
        exam.Exercises.Single(item => item.Id == exerciseId).ExpectedValue
        ?? throw new InvalidDataException($"Brak wartości liczbowej zadania {exerciseId}.");

    private static T Read<T>(string relativePath) =>
        JsonSerializer.Deserialize<T>(File.ReadAllText(Absolute(relativePath)), JsonOptions)
        ?? throw new InvalidDataException($"Nie można odczytać {relativePath}.");

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
