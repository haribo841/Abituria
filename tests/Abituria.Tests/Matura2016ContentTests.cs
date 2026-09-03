using System.Text.Json;
using Abituria.Models;

namespace Abituria.Tests;

public sealed class Matura2016ContentTests
{
    private const string MainBasicPaperUrl = "https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2016/Matematyka/MMA-P1_1P-162.pdf";
    private const string MainBasicRulesUrl = "https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2016/Matematyka/MMA-P1-N.pdf";
    private const string MainExtendedPaperUrl = "https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2016/Matematyka/MMA-R1_1P-162.pdf";
    private const string MainExtendedRulesUrl = "https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2016/Matematyka/MMA-R1-N.pdf";
    private const string CorrectionPaperUrl = "https://arkusze.pl/maturalne/matematyka-2016-sierpien-poprawkowa-podstawowa.pdf";
    private const string CorrectionRulesUrl = "https://arkusze.pl/maturalne/matematyka-2016-sierpien-poprawkowa-podstawowa-odpowiedzi.pdf";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();
    private static readonly string[] ExpectedVectorDiagramIds =
    [
        "exam-mm16-p0-z07", "exam-mm16-p0-z10", "exam-mm16-p0-z13", "exam-mm16-p0-z16", "exam-mm16-p0-z19", "exam-mm16-p0-z24", "exam-mm16-p0-z29",
        "exam-mm16-r0-z03", "exam-mm16-r0-z09", "exam-mm16-r0-z16",
        "exam-mm16-p0p-z07", "exam-mm16-p0p-z19", "exam-mm16-p0p-z21", "exam-mm16-p0p-z32", "exam-mm16-p0p-z33"
    ];
    private static readonly string[] ExpectedDiagramSourceIds =
    [
        "cke-2016-main-basic-exam",
        "cke-2016-main-extended-exam",
        "cke-2016-correction-basic-exam"
    ];

    [Theory]
    [InlineData("Content/exam-2016-main-basic.json", "matura-maj-2016-podstawowa", "główna", "basic", 170, 34, 34, 50)]
    [InlineData("Content/exam-2016-main-extended.json", "matura-maj-2016-rozszerzona", "główna", "extended", 180, 16, 16, 50)]
    [InlineData("Content/exam-2016-correction-basic.json", "matura-poprawkowa-2016-podstawowa", "poprawkowa", "basic", 170, 34, 34, 50)]
    public void Formula_2015_exam_contracts_are_complete_and_preserve_stable_progress_ids(
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
        Assert.Equal(2016, exam.Year);
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
        Assert.Equal(progressItemCount, exam.Exercises.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.All(exam.Exercises, exercise =>
        {
            Assert.StartsWith("mm16-", exercise.Id, StringComparison.Ordinal);
            Assert.InRange(exercise.Id.Length, 1, 79);
            Assert.Equal(id, exercise.ExamId);
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.True(exercise.Hints.Count >= 2);
            Assert.False(string.IsNullOrWhiteSpace(exercise.EffectiveSolution));
            Assert.False(string.IsNullOrWhiteSpace(exercise.ScoringCriteria));
            Assert.InRange(exercise.SourcePage, 2, 24);
            Assert.InRange(exercise.SolutionSourcePage, 2, 34);
        });

        Assert.Equal("2026-09-02", exam.Source.VerifiedOn);
        Assert.All(
            new[] { exam.Source.QuestionPaperSha256, exam.Source.AnswerKeySha256 },
            hash =>
            {
                Assert.Equal(64, hash.Length);
                Assert.True(hash.All(character => char.IsAsciiHexDigit(character) && !char.IsLower(character)));
            });
    }

    [Fact]
    public void Pinned_sources_have_exact_urls_hashes_dates_and_coverage_documents()
    {
        var basic = Read<ExamCatalog>("Content/exam-2016-main-basic.json").Exam.Source;
        var extended = Read<ExamCatalog>("Content/exam-2016-main-extended.json").Exam.Source;
        var correction = Read<ExamCatalog>("Content/exam-2016-correction-basic.json").Exam.Source;

        Assert.Equal(
            ("MMA-P1_1P-162", "2016-05-05", MainBasicPaperUrl, "3AD62333A0E84B0AB344D71449EAE8A051A2A90666D5578F04157812C99E5989", MainBasicRulesUrl, "CF772FEFB9321A6FB9E052C778AB8E4E54D2EEDCAEC050958DEAF84ABE386ED3"),
            (basic.DocumentCode, basic.ExamDate, basic.QuestionPaperUrl, basic.QuestionPaperSha256, basic.AnswerKeyUrl, basic.AnswerKeySha256));
        Assert.Equal(
            ("MMA-R1_1P-162", "2016-05-09", MainExtendedPaperUrl, "94D1D433D16FB9FAF91F0B68B548CCD2D2F833317BCD6E7023FB3EB44C57B34B", MainExtendedRulesUrl, "45A857F24A2919FE97CE5D7BA4EE6563F28FB47E70B52D196A9FEFF5F127B6F0"),
            (extended.DocumentCode, extended.ExamDate, extended.QuestionPaperUrl, extended.QuestionPaperSha256, extended.AnswerKeyUrl, extended.AnswerKeySha256));
        Assert.Equal(
            ("MMA-P1_1P-164", "2016-08-23", CorrectionPaperUrl, "82EB6402EE0422B09DF15CB5F65D22431FDC6F064ED8FBF250BB59888211975D", CorrectionRulesUrl, "BA3BD6A4B2AE4C1D87AE5BF499A8ECB533AA80791C8058DC58CB721CD87DCB82"),
            (correction.DocumentCode, correction.ExamDate, correction.QuestionPaperUrl, correction.QuestionPaperSha256, correction.AnswerKeyUrl, correction.AnswerKeySha256));

        var basicCoverage = File.ReadAllText(Absolute("docs/MATURA_2016_BASIC_COVERAGE.md"));
        var extendedCoverage = File.ReadAllText(Absolute("docs/MATURA_2016_EXTENDED_COVERAGE.md"));
        var correctionCoverage = File.ReadAllText(Absolute("docs/MATURA_2016_CORRECTION_BASIC_COVERAGE.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));

        Assert.Contains(basic.QuestionPaperSha256, basicCoverage, StringComparison.Ordinal);
        Assert.Contains(extended.QuestionPaperSha256, extendedCoverage, StringComparison.Ordinal);
        Assert.Contains(correction.QuestionPaperSha256, correctionCoverage, StringComparison.Ordinal);
        Assert.Contains("MATURA_2016_BASIC_COVERAGE.md", toc, StringComparison.Ordinal);
        Assert.Contains("MATURA_2016_EXTENDED_COVERAGE.md", toc, StringComparison.Ordinal);
        Assert.Contains("MATURA_2016_CORRECTION_BASIC_COVERAGE.md", toc, StringComparison.Ordinal);
    }

    [Fact]
    public void Answer_modes_and_official_closed_answer_keys_match_the_audited_sources()
    {
        var basic = Read<ExamCatalog>("Content/exam-2016-main-basic.json").Exam;
        var extended = Read<ExamCatalog>("Content/exam-2016-main-extended.json").Exam;
        var correction = Read<ExamCatalog>("Content/exam-2016-correction-basic.json").Exam;

        Assert.Equal(
            [1, 4, 1, 1, 3, 3, 4, 4, 1, 4, 2, 2, 1, 1, 4, 2, 3, 4, 2, 3, 2, 3, 4, 2, 3],
            basic.Exercises.Take(25).Select(item => item.CorrectOption!.Value));
        Assert.Equal(25, basic.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(9, basic.Exercises.Count(item => item.IsRevealOnly));

        Assert.Equal([3, 4, 2, 1, 4], extended.Exercises.Take(5).Select(item => item.CorrectOption!.Value));
        Assert.Equal(5, extended.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(2, extended.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(9, extended.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(753d, ExpectedValue(extended, "mm16-r0-z06"));
        Assert.Equal(187d, ExpectedValue(extended, "mm16-r0-z07"));

        Assert.Equal(
            [1, 2, 4, 2, 2, 1, 1, 2, 1, 4, 3, 4, 2, 3, 3, 4, 1, 4, 3, 3, 3, 3, 1, 4, 2],
            correction.Exercises.Take(25).Select(item => item.CorrectOption!.Value));
        Assert.Equal(25, correction.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(1, correction.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(8, correction.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(676368d, ExpectedValue(correction, "mm16-p0p-z31"));
    }

    [Fact]
    public void Audited_transcriptions_keep_the_source_equations_contexts_and_results()
    {
        var basic = Read<ExamCatalog>("Content/exam-2016-main-basic.json").Exam;
        var extended = Read<ExamCatalog>("Content/exam-2016-main-extended.json").Exam;
        var correction = Read<ExamCatalog>("Content/exam-2016-correction-basic.json").Exam;

        var basicTask19 = Exercise(basic, "mm16-p0-z19");
        Assert.Contains("Pole trójkąta", basicTask19.Prompt, StringComparison.Ordinal);
        Assert.Equal("2√33", basicTask19.Options[1]);
        Assert.Contains("(4 - x)(x² + 2x - 15)", Exercise(basic, "mm16-p0-z28").Prompt, StringComparison.Ordinal);
        Assert.Contains("{-5, 3, 4}", Exercise(basic, "mm16-p0-z28").RevealedAnswer, StringComparison.Ordinal);
        Assert.Contains("trzy razy większy", Exercise(basic, "mm16-p0-z32").Prompt, StringComparison.Ordinal);

        Assert.Contains("|f(x)| = p", Exercise(extended, "mm16-r0-z03").Prompt, StringComparison.Ordinal);
        Assert.Contains("f(x) = x - 2", Exercise(extended, "mm16-r0-z10").Prompt, StringComparison.Ordinal);
        Assert.Contains("2 cos x - √3", Exercise(extended, "mm16-r0-z11").Prompt, StringComparison.Ordinal);
        Assert.Contains("x² + 2(m + 1)x + 6m + 1", Exercise(extended, "mm16-r0-z12").Prompt, StringComparison.Ordinal);
        Assert.Contains("A = (30, 32)", Exercise(extended, "mm16-r0-z13").Prompt, StringComparison.Ordinal);

        Assert.Contains("(4⁵ · 5⁴) / 20⁴", Exercise(correction, "mm16-p0p-z03").Prompt, StringComparison.Ordinal);
        Assert.Equal("7", Exercise(correction, "mm16-p0p-z11").Options[2]);
        Assert.Contains("3x² - 6x", Exercise(correction, "mm16-p0p-z26").Prompt, StringComparison.Ordinal);
        Assert.Contains("14/23", Exercise(correction, "mm16-p0p-z27").RevealedAnswer, StringComparison.Ordinal);
        Assert.Contains("B = (5, 3)", Exercise(correction, "mm16-p0p-z32").RevealedAnswer, StringComparison.Ordinal);
        Assert.Contains("{1, 2, 3, 4, 5, 6, 7}", Exercise(correction, "mm16-p0p-z34").Prompt, StringComparison.Ordinal);
    }

    [Fact]
    public void Vector_definitions_are_referenced_and_new_material_is_legally_blocked()
    {
        var basic = Read<ExamCatalog>("Content/exam-2016-main-basic.json").Exam;
        var extended = Read<ExamCatalog>("Content/exam-2016-main-extended.json").Exam;
        var correction = Read<ExamCatalog>("Content/exam-2016-correction-basic.json").Exam;
        var actual = basic.Exercises.Concat(extended.Exercises).Concat(correction.Exercises)
            .SelectMany(item => item.DiagramIds)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        Assert.Equal(ExpectedVectorDiagramIds.Order(StringComparer.Ordinal), actual);

        var diagrams = JsonDocument.Parse(File.ReadAllText(Absolute("Content/diagrams.json"))).RootElement
            .GetProperty("diagrams")
            .EnumerateArray()
            .Where(item => ExpectedDiagramSourceIds.Contains(item.GetProperty("sourceId").GetString(), StringComparer.Ordinal))
            .ToArray();
        Assert.Equal(15, diagrams.Length);
        Assert.All(diagrams, diagram =>
        {
            Assert.True(diagram.GetProperty("sourcePage").GetInt32() > 0);
            Assert.False(string.IsNullOrWhiteSpace(diagram.GetProperty("alternativeText").GetString()));
            Assert.NotEmpty(diagram.GetProperty("primitives").EnumerateArray());
        });

        var manifest = JsonDocument.Parse(File.ReadAllText(Absolute("Content/provenance.json")));
        Assert.False(manifest.RootElement.GetProperty("releaseEligible").GetBoolean());
        var groups = manifest.RootElement.GetProperty("assets").EnumerateArray()
            .ToDictionary(item => item.GetProperty("id").GetString()!, StringComparer.Ordinal);
        foreach (var groupId in ExpectedDiagramSourceIds)
        {
            Assert.Equal("blocked", groups[groupId].GetProperty("distributionStatus").GetString());
            Assert.False(string.IsNullOrWhiteSpace(groups[groupId].GetProperty("blockedReason").GetString()));
        }
        Assert.Equal("blocked", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
    }

    [Fact]
    public void Catalog_exposes_the_2016_triplet_after_2017_without_affecting_previous_progress()
    {
        var index = Read<ExamIndexCatalog>("Content/exams.json");
        var exams = index.Exams.OrderBy(item => item.Order).Select(item => Read<ExamCatalog>(item.ContentPath).Exam).ToArray();

        Assert.Equal(32, exams.Length);
        Assert.Equal("matura-maj-2017-podstawowa", exams[26].Id);
        Assert.Equal("matura-poprawkowa-2017-podstawowa", exams[28].Id);
        Assert.Equal("matura-maj-2016-podstawowa", exams[29].Id);
        Assert.Equal("matura-maj-2016-rozszerzona", exams[30].Id);
        Assert.Equal("matura-poprawkowa-2016-podstawowa", exams[31].Id);
        Assert.Equal(889, exams.Sum(exam => exam.Exercises.Count));
    }

    private static double ExpectedValue(ExamDefinition exam, string exerciseId) =>
        exam.Exercises.Single(item => item.Id == exerciseId).ExpectedValue
        ?? throw new InvalidDataException($"Brak wartości liczbowej zadania {exerciseId}.");

    private static LearningExercise Exercise(ExamDefinition exam, string exerciseId) =>
        exam.Exercises.Single(item => item.Id == exerciseId);

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
