using System.Text.Json;
using Abituria.Models;
using Abituria.Services;

namespace Abituria.Tests;

public sealed class Matura2022ExtendedContentTests
{
    private const string ExamId = "matura-maj-2022-rozszerzona";
    private const string PaperUrl = "https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2022/Matematyka/EMAP-R0-100-2205_compressed.pdf";
    private const string RulesUrl = "https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2022/Matematyka/EMAP-R0-100-2205-zasady.pdf";
    private const string PaperHash = "2AA11EADAE59BE3F60A61B97FD27DE782849F9631D0991D77B43C96D88B676A4";
    private const string RulesHash = "83D33D8C83F6E866851406950CCA7A0C1A336DC5E5F28AB1ED947F0321478D06";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Formula_2015_extended_exam_has_exactly_15_tasks_15_progress_items_and_50_points()
    {
        var catalog = Read<ExamCatalog>("Content/exam-2022-main-extended.json");
        var exam = catalog.Exam;
        int[] expectedSourcePages = [2, 2, 2, 2, 4, 5, 6, 8, 10, 12, 14, 16, 18, 20, 22];
        int[] expectedSolutionPages = [2, 2, 3, 3, 4, 4, 7, 10, 15, 17, 20, 24, 28, 30, 40];
        int[] expectedPoints = [1, 1, 1, 1, 2, 3, 3, 3, 4, 4, 4, 5, 5, 6, 7];
        int[] expectedKey = [1, 3, 1, 1];

        Assert.Equal(4, catalog.SchemaVersion);
        Assert.Equal(ExamId, exam.Id);
        Assert.Equal("Matura maj 2022 - poziom rozszerzony (Formuła 2015)", exam.Title);
        Assert.Equal("Matura maj 2022 PR (F2015)", exam.ProgressLabel);
        Assert.Equal(2022, exam.Year);
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
        Assert.Equal(expectedPoints, exam.Exercises.Select(item => item.Points));
        Assert.Equal(expectedKey, exam.Exercises.Take(4).Select(item => item.CorrectOption!.Value));
        Assert.All(exam.Exercises, exercise =>
        {
            Assert.StartsWith("em22-r0-", exercise.Id, StringComparison.Ordinal);
            Assert.InRange(exercise.Id.Length, 1, 79);
            Assert.Equal(ExamId, exercise.ExamId);
            Assert.Equal($"Zadanie {exercise.DisplayNumber}", exercise.Title);
            Assert.Equal("CKE EMAP-R0-100-2205 i zasady oceniania", exercise.VerificationSource);
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.True(exercise.Hints.Count >= 2);
            Assert.False(string.IsNullOrWhiteSpace(exercise.EffectiveSolution));
            Assert.False(string.IsNullOrWhiteSpace(exercise.ScoringCriteria));
        });
    }

    [Fact]
    public void Official_archive_urls_hashes_and_verification_date_are_pinned()
    {
        var source = Read<ExamCatalog>("Content/exam-2022-main-extended.json").Exam.Source;

        Assert.Contains("Centralna Komisja Egzaminacyjna", source.Publisher, StringComparison.Ordinal);
        Assert.Contains("OKE Warszawa", source.Publisher, StringComparison.Ordinal);
        Assert.Equal("EMAP-R0-100-2205", source.DocumentCode);
        Assert.Equal("2022-05-11", source.ExamDate);
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
    public void Answer_modes_topics_and_vector_figure_cover_the_entire_exam()
    {
        var index = Read<ExamIndexCatalog>("Content/exams.json");
        var exam = Read<ExamCatalog>("Content/exam-2022-main-extended.json").Exam;
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        var expectedNumeric = new Dictionary<string, double>(StringComparer.Ordinal)
        {
            ["em22-r0-z05"] = 411d,
            ["em22-r0-z07"] = -8d / 3d,
            ["em22-r0-z10"] = 129d,
            ["em22-r0-z13"] = Math.Sqrt(22d)
        };

        Assert.Equal(4, exam.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(4, exam.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(7, exam.Exercises.Count(item => item.IsRevealOnly));
        Assert.DoesNotContain(exam.Exercises, item => item.IsCompound);
        Assert.Equal(expectedNumeric.Keys.Order(), exam.Exercises.Where(item => item.IsNumeric).Select(item => item.Id).Order());
        foreach (var pair in expectedNumeric)
            Assert.Equal(pair.Value, exam.Exercises.Single(item => item.Id == pair.Key).ExpectedValue!.Value, 12);

        Assert.All(exam.Exercises, exercise => Assert.Contains(index.Topics, topic => topic.Id == exercise.TopicId));
        var referenced = exam.Exercises.SelectMany(item => item.DiagramIds)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var definitions = diagrams.Diagrams.Where(item => item.SourceId == "cke-2022-main-extended")
            .OrderBy(item => item.Id)
            .ToArray();

        Assert.Equal(["exam-em22-r0-z13"], referenced);
        Assert.Equal(referenced, definitions.Select(item => item.Id));
        Assert.Equal([18], definitions.Select(item => item.SourcePage));
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
        var coverage = File.ReadAllText(Absolute("docs/MATURA_2022_EXTENDED_COVERAGE.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));

        Assert.False(root.GetProperty("releaseEligible").GetBoolean());
        Assert.Equal("approved", groups["cke-2022-main-extended-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("blocked", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
        Assert.Contains(PaperHash, groups["cke-2022-main-extended-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.Contains(RulesHash, groups["cke-2022-main-extended-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.DoesNotContain(PaperHash, rights, StringComparison.Ordinal);
        Assert.DoesNotContain(RulesHash, rights, StringComparison.Ordinal);
        Assert.Contains(PaperHash, coverage, StringComparison.Ordinal);
        Assert.Contains(RulesHash, coverage, StringComparison.Ordinal);
        Assert.Contains("MATURA_2022_EXTENDED_COVERAGE.md", toc, StringComparison.Ordinal);
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
