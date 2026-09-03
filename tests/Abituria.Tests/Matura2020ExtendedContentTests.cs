using System.Text.Json;
using Abituria.Models;
using Abituria.Services;

namespace Abituria.Tests;

public sealed class Matura2020ExtendedContentTests
{
    private const string ExamId = "matura-maj-2020-rozszerzona";
    private const string PaperUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2020/formula_od_2015/matematyka/MMA-R1_1P-202.pdf";
    private const string RulesUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2015/Arkusze_egzaminacyjne/2020/formula_od_2015/Zasady_oceniania/MMA-PR-202_zasady.pdf";
    private const string PaperHash = "4D6DB1245E54AE6E9CFED9AF90D4293B167F14E9D82D7710FF4D4BE8FAA631BA";
    private const string RulesHash = "6A3B150CD9E68FD853AE5CC8D39F5FDCF48D464082D206172D3D0109766BEECD";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Formula_2015_extended_exam_has_exactly_15_tasks_50_points_and_pinned_sources()
    {
        var catalog = Read<ExamCatalog>("Content/exam-2020-main-extended.json");
        var exam = catalog.Exam;
        int[] expectedSourcePages = [2, 2, 2, 2, 4, 5, 6, 8, 9, 10, 12, 14, 16, 18, 20];
        int[] expectedSolutionPages = [2, 2, 2, 2, 3, 3, 10, 12, 14, 16, 19, 22, 27, 31, 33];
        int[] expectedKey = [2, 3, 1, 2];

        Assert.Equal(4, catalog.SchemaVersion);
        Assert.Equal(ExamId, exam.Id);
        Assert.Equal("Matura maj 2020 - poziom rozszerzony (Formuła 2015)", exam.Title);
        Assert.Equal("Matura maj 2020 PR (F2015)", exam.ProgressLabel);
        Assert.Equal(2020, exam.Year);
        Assert.Equal("główna", exam.Session);
        Assert.Equal("2015", exam.Formula);
        Assert.Equal("extended", exam.Level);
        Assert.Equal(180, exam.DurationMinutes);
        Assert.Equal(50, exam.MaximumPoints);
        Assert.Equal(15, exam.OfficialTaskCount);
        Assert.Equal(15, exam.ProgressItemCount);
        Assert.Equal(15, exam.Exercises.Count);
        Assert.Equal(50, exam.Exercises.Sum(item => item.Points));
        Assert.Equal(Enumerable.Range(1, 15).Select(number => number.ToString()), exam.Exercises.Select(item => item.DisplayNumber));
        Assert.Equal(Enumerable.Range(1, 15), exam.Exercises.Select(item => item.EffectiveOrder));
        Assert.Equal(Enumerable.Range(1, 15), exam.Exercises.Select(item => item.Number));
        Assert.Equal(expectedSourcePages, exam.Exercises.Select(item => item.SourcePage));
        Assert.Equal(expectedSolutionPages, exam.Exercises.Select(item => item.SolutionSourcePage));
        Assert.Equal(expectedKey, exam.Exercises.Take(4).Select(item => item.CorrectOption!.Value));
        Assert.Equal([1, 1, 1, 1, 2, 3, 3, 3, 4, 5, 4, 5, 4, 6, 7], exam.Exercises.Select(item => item.Points));
        Assert.All(exam.Exercises, exercise =>
        {
            Assert.StartsWith("mm20-r0-", exercise.Id, StringComparison.Ordinal);
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
        var source = Read<ExamCatalog>("Content/exam-2020-main-extended.json").Exam.Source;

        Assert.Equal("Centralna Komisja Egzaminacyjna", source.Publisher);
        Assert.Equal("MMA-R1_1P-202", source.DocumentCode);
        Assert.Equal("2020-05-07", source.ExamDate);
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
    public void Answer_modes_topics_and_four_vector_figures_cover_the_entire_exam()
    {
        var index = Read<ExamIndexCatalog>("Content/exams.json");
        var exam = Read<ExamCatalog>("Content/exam-2020-main-extended.json").Exam;
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        string[] expectedDiagramIds =
        [
            "exam-mm20-r0-z07", "exam-mm20-r0-z12", "exam-mm20-r0-z14", "exam-mm20-r0-z15"
        ];

        Assert.Equal(4, exam.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(4, exam.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(7, exam.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(0, exam.Exercises.Count(item => item.IsCompound));
        Assert.Equal(955d, exam.Exercises.Single(item => item.Id == "mm20-r0-z05").ExpectedValue!.Value, 12);
        Assert.Equal(3d, exam.Exercises.Single(item => item.Id == "mm20-r0-z10").ExpectedValue!.Value, 12);
        Assert.Equal(12960d, exam.Exercises.Single(item => item.Id == "mm20-r0-z13").ExpectedValue!.Value, 12);
        Assert.Equal(624d, exam.Exercises.Single(item => item.Id == "mm20-r0-z14").ExpectedValue!.Value, 12);
        Assert.All(exam.Exercises, exercise => Assert.Contains(index.Topics, topic => topic.Id == exercise.TopicId));

        var referenced = exam.Exercises.SelectMany(item => item.DiagramIds)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var definitions = diagrams.Diagrams.Where(item => item.SourceId == "cke-2020-main-extended")
            .OrderBy(item => item.Id)
            .ToArray();

        Assert.Equal(expectedDiagramIds, referenced);
        Assert.Equal(referenced, definitions.Select(item => item.Id));
        Assert.Equal([6, 14, 18, 20], definitions.Select(item => item.SourcePage));
        Assert.All(definitions, definition =>
        {
            Assert.False(string.IsNullOrWhiteSpace(definition.AlternativeText));
            Assert.NotEmpty(definition.Primitives);
        });
        DiagramCatalogValidator.Validate(diagrams);
    }

    [Fact]
    public void Formula_2015_archive_keeps_its_approved_source_while_new_archives_block_release()
    {
        using var provenance = JsonDocument.Parse(File.ReadAllText(Absolute("Content/provenance.json")));
        var root = provenance.RootElement;
        var groups = root.GetProperty("assets").EnumerateArray().ToDictionary(
            item => item.GetProperty("id").GetString()!,
            StringComparer.Ordinal);
        var rights = File.ReadAllText(Absolute("docs/ASSET_RIGHTS_DECLARATION.md"));
        var coverage = File.ReadAllText(Absolute("docs/MATURA_2020_EXTENDED_COVERAGE.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));

        Assert.False(root.GetProperty("releaseEligible").GetBoolean());
        Assert.Equal("approved", groups["cke-2020-main-extended-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("blocked", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
        Assert.Contains(PaperHash, groups["cke-2020-main-extended-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.Contains(RulesHash, groups["cke-2020-main-extended-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.DoesNotContain(PaperHash, rights, StringComparison.Ordinal);
        Assert.DoesNotContain(RulesHash, rights, StringComparison.Ordinal);
        Assert.Contains(PaperHash, coverage, StringComparison.Ordinal);
        Assert.Contains(RulesHash, coverage, StringComparison.Ordinal);
        Assert.Contains("MATURA_2020_EXTENDED_COVERAGE.md", toc, StringComparison.Ordinal);
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
