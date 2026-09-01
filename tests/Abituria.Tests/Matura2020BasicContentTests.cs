using System.Text.Json;
using Abituria.Models;
using Abituria.Services;

namespace Abituria.Tests;

public sealed class Matura2020BasicContentTests
{
    private const string ExamId = "matura-maj-2020-podstawowa";
    private const string PaperUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2020/formula_od_2015/matematyka/MMA-P1_1P-202.pdf";
    private const string RulesUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2020/formula_od_2015/Zasady_oceniania/MMA-PP-202_zasady.pdf";
    private const string PaperHash = "678D7765CCDABABAC3AF11C3D96A1F1038BD54AAB2111726472FA4F92FE45C0E";
    private const string RulesHash = "0EEEAA2970026EC0EAD1BA8B34B9E222A7212F31151485ED1C6282E0B4B41C32";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Formula_2015_basic_exam_has_exactly_34_tasks_50_points_and_pinned_sources()
    {
        var catalog = Read<ExamCatalog>("Content/exam-2020-main-basic.json");
        var exam = catalog.Exam;
        int[] expectedSourcePages =
        [
            2, 2, 2, 2, 2, 2, 4, 4, 4, 6, 6, 6, 6, 8, 8, 8, 8, 8, 10, 10, 10, 10, 12, 12, 12, 14, 15, 16, 17, 18, 19, 20, 22, 24
        ];
        int[] expectedKey =
        [
            2, 3, 4, 1, 1, 2, 4, 3, 2, 2, 4, 2, 3, 4, 3, 4, 4, 1, 2, 1, 3, 3, 1, 1, 2
        ];

        Assert.Equal(4, catalog.SchemaVersion);
        Assert.Equal(ExamId, exam.Id);
        Assert.Equal("Matura maj 2020 - poziom podstawowy (Formuła 2015)", exam.Title);
        Assert.Equal("Matura maj 2020 PP (F2015)", exam.ProgressLabel);
        Assert.Equal(2020, exam.Year);
        Assert.Equal("główna", exam.Session);
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
        Assert.Equal(expectedKey, exam.Exercises.Take(25).Select(item => item.CorrectOption!.Value));
        Assert.Equal(Enumerable.Repeat(1, 25).Concat([2, 2, 2, 2, 2, 2, 4, 4, 5]), exam.Exercises.Select(item => item.Points));
        Assert.All(exam.Exercises, exercise =>
        {
            Assert.StartsWith("mm20-p0-", exercise.Id, StringComparison.Ordinal);
            Assert.InRange(exercise.Id.Length, 1, 79);
            Assert.Equal(ExamId, exercise.ExamId);
            Assert.Equal($"Zadanie {exercise.DisplayNumber}", exercise.Title);
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.True(exercise.Hints.Count >= 2);
            Assert.False(string.IsNullOrWhiteSpace(exercise.EffectiveSolution));
            Assert.False(string.IsNullOrWhiteSpace(exercise.ScoringCriteria));
        });
    }

    [Fact]
    public void Official_CKE_urls_hashes_and_verification_date_are_pinned()
    {
        var source = Read<ExamCatalog>("Content/exam-2020-main-basic.json").Exam.Source;

        Assert.Equal("Centralna Komisja Egzaminacyjna", source.Publisher);
        Assert.Equal("MMA-P1_1P-202", source.DocumentCode);
        Assert.Equal("2020-05-05", source.ExamDate);
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
    public void Answer_modes_topics_and_eight_vector_figures_cover_the_entire_exam()
    {
        var index = Read<ExamIndexCatalog>("Content/exams.json");
        var exam = Read<ExamCatalog>("Content/exam-2020-main-basic.json").Exam;
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        string[] expectedDiagramIds =
        [
            "exam-mm20-p0-z07", "exam-mm20-p0-z11", "exam-mm20-p0-z17", "exam-mm20-p0-z19",
            "exam-mm20-p0-z22", "exam-mm20-p0-z25", "exam-mm20-p0-z29", "exam-mm20-p0-z34"
        ];

        Assert.Equal(25, exam.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(3, exam.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(6, exam.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(0, exam.Exercises.Count(item => item.IsCompound));
        Assert.Equal(11d / 36d, exam.Exercises.Single(item => item.Id == "mm20-p0-z30").ExpectedValue!.Value, 12);
        Assert.Equal(0.5d, exam.Exercises.Single(item => item.Id == "mm20-p0-z31").ExpectedValue!.Value, 12);
        Assert.Equal(3d, exam.Exercises.Single(item => item.Id == "mm20-p0-z33").ExpectedValue!.Value, 12);
        Assert.All(exam.Exercises, exercise => Assert.Contains(index.Topics, topic => topic.Id == exercise.TopicId));

        var referenced = exam.Exercises.SelectMany(item => item.DiagramIds)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var definitions = diagrams.Diagrams.Where(item => item.SourceId == "cke-2020-main-basic")
            .OrderBy(item => item.Id)
            .ToArray();

        Assert.Equal(expectedDiagramIds, referenced);
        Assert.Equal(referenced, definitions.Select(item => item.Id));
        Assert.Equal([4, 6, 8, 10, 10, 12, 17, 24], definitions.Select(item => item.SourcePage));
        Assert.All(definitions, definition =>
        {
            Assert.False(string.IsNullOrWhiteSpace(definition.AlternativeText));
            Assert.NotEmpty(definition.Primitives);
        });
        DiagramCatalogValidator.Validate(diagrams);
    }

    [Fact]
    public void Formula_2015_archive_is_approved_after_explicit_rights_declaration()
    {
        using var provenance = JsonDocument.Parse(File.ReadAllText(Absolute("Content/provenance.json")));
        var root = provenance.RootElement;
        var groups = root.GetProperty("assets").EnumerateArray().ToDictionary(
            item => item.GetProperty("id").GetString()!,
            StringComparer.Ordinal);
        var rights = File.ReadAllText(Absolute("docs/ASSET_RIGHTS_DECLARATION.md"));
        var coverage = File.ReadAllText(Absolute("docs/MATURA_2020_BASIC_COVERAGE.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));

        Assert.True(root.GetProperty("releaseEligible").GetBoolean());
        Assert.Equal("approved", groups["cke-2020-main-basic-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("approved", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
        Assert.Contains(PaperHash, groups["cke-2020-main-basic-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.Contains(RulesHash, groups["cke-2020-main-basic-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.DoesNotContain(PaperHash, rights, StringComparison.Ordinal);
        Assert.DoesNotContain(RulesHash, rights, StringComparison.Ordinal);
        Assert.Contains(PaperHash, coverage, StringComparison.Ordinal);
        Assert.Contains(RulesHash, coverage, StringComparison.Ordinal);
        Assert.Contains("MATURA_2020_BASIC_COVERAGE.md", toc, StringComparison.Ordinal);
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
