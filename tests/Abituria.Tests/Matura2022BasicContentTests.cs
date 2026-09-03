using System.Text.Json;
using Abituria.Models;
using Abituria.Services;

namespace Abituria.Tests;

public sealed class Matura2022BasicContentTests
{
    private const string ExamId = "matura-maj-2022-podstawowa";
    private const string PaperUrl = "https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2022/Matematyka/EMAP-P0-100-2205_compressed.pdf";
    private const string RulesUrl = "https://www.oke.waw.pl/wp-content/uploads/OKE_WARSZAWA/EM/EM_2015/Arkusze/Arkusze_2022/Matematyka/EMAP-P0-100-2205-zasady.pdf";
    private const string PaperHash = "34101686FFB987572AB3110F344EC1A8692D8FACDBEF194498DBBD245568A2D2";
    private const string RulesHash = "DD236C15820CBD5F7BD15D2EA194D075A3732B640C559A064062FBF3101F90B8";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Formula_2015_basic_exam_has_exactly_35_tasks_45_points_and_pinned_sources()
    {
        var catalog = Read<ExamCatalog>("Content/exam-2022-main-basic.json");
        var exam = catalog.Exam;
        int[] expectedSourcePages =
        [
            2, 2, 2, 2, 2, 4, 4, 4, 4, 6, 8, 8, 8, 8, 8, 8, 10, 10, 12, 12, 12, 12, 12, 14, 14, 14, 14, 14, 16, 17, 18, 19, 20, 21, 22
        ];
        int[] expectedSolutionPages =
        [
            2, 2, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 7, 7, 7, 8, 8, 9, 9, 9, 10, 10, 10, 11, 11, 11, 12, 12, 13, 15, 16, 19, 21, 23, 25
        ];
        int[] expectedKey =
        [
            1, 2, 3, 2, 1, 2, 3, 3, 2, 4, 4, 2, 4, 1, 1, 4, 2, 4, 4, 1, 2, 3, 1, 1, 2, 4, 2, 3
        ];

        Assert.Equal(4, catalog.SchemaVersion);
        Assert.Equal(ExamId, exam.Id);
        Assert.Equal("Matura maj 2022 - poziom podstawowy (Formuła 2015)", exam.Title);
        Assert.Equal("Matura maj 2022 PP (F2015)", exam.ProgressLabel);
        Assert.Equal(2022, exam.Year);
        Assert.Equal("główna", exam.Session);
        Assert.Equal("2015", exam.Formula);
        Assert.Equal("basic", exam.Level);
        Assert.Equal(170, exam.DurationMinutes);
        Assert.Equal(45, exam.MaximumPoints);
        Assert.Equal(35, exam.OfficialTaskCount);
        Assert.Equal(35, exam.ProgressItemCount);
        Assert.Equal(35, exam.Exercises.Count);
        Assert.Equal(45, exam.Exercises.Sum(item => item.Points));
        Assert.Equal(Enumerable.Range(1, 35).Select(number => number.ToString()), exam.Exercises.Select(item => item.DisplayNumber));
        Assert.Equal(Enumerable.Range(1, 35), exam.Exercises.Select(item => item.EffectiveOrder));
        Assert.Equal(Enumerable.Range(1, 35), exam.Exercises.Select(item => item.Number));
        Assert.Equal(expectedSourcePages, exam.Exercises.Select(item => item.SourcePage));
        Assert.Equal(expectedSolutionPages, exam.Exercises.Select(item => item.SolutionSourcePage));
        Assert.Equal(expectedKey, exam.Exercises.Take(28).Select(item => item.CorrectOption!.Value));
        Assert.Equal(Enumerable.Repeat(1, 28).Concat([2, 2, 2, 2, 2, 2, 5]), exam.Exercises.Select(item => item.Points));
        Assert.All(exam.Exercises, exercise =>
        {
            Assert.StartsWith("em22-p0-", exercise.Id, StringComparison.Ordinal);
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
    public void Official_archive_urls_hashes_and_verification_date_are_pinned()
    {
        var source = Read<ExamCatalog>("Content/exam-2022-main-basic.json").Exam.Source;

        Assert.Contains("Centralna Komisja Egzaminacyjna", source.Publisher, StringComparison.Ordinal);
        Assert.Contains("OKE Warszawa", source.Publisher, StringComparison.Ordinal);
        Assert.Equal("EMAP-P0-100-2205", source.DocumentCode);
        Assert.Equal("2022-05-05", source.ExamDate);
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
    public void Answer_modes_topics_and_six_vector_figures_cover_the_entire_exam()
    {
        var index = Read<ExamIndexCatalog>("Content/exams.json");
        var exam = Read<ExamCatalog>("Content/exam-2022-main-basic.json").Exam;
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        string[] expectedDiagramIds =
        [
            "exam-em22-p0-z09", "exam-em22-p0-z10", "exam-em22-p0-z17", "exam-em22-p0-z18", "exam-em22-p0-z26", "exam-em22-p0-z33"
        ];

        Assert.Equal(28, exam.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(2, exam.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(5, exam.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(0, exam.Exercises.Count(item => item.IsCompound));
        Assert.Equal(14750d, exam.Exercises.Single(item => item.Id == "em22-p0-z30").ExpectedValue!.Value, 12);
        Assert.Equal(0.8d, exam.Exercises.Single(item => item.Id == "em22-p0-z32").ExpectedValue!.Value, 12);
        Assert.All(exam.Exercises, exercise => Assert.Contains(index.Topics, topic => topic.Id == exercise.TopicId));

        var referenced = exam.Exercises.SelectMany(item => item.DiagramIds)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var definitions = diagrams.Diagrams.Where(item => item.SourceId == "cke-2022-main-basic")
            .OrderBy(item => item.Id)
            .ToArray();

        Assert.Equal(expectedDiagramIds, referenced);
        Assert.Equal(referenced, definitions.Select(item => item.Id));
        Assert.Equal([4, 6, 10, 10, 14, 20], definitions.Select(item => item.SourcePage));
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
        var coverage = File.ReadAllText(Absolute("docs/MATURA_2022_BASIC_COVERAGE.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));

        Assert.False(root.GetProperty("releaseEligible").GetBoolean());
        Assert.Equal("approved", groups["cke-2022-main-basic-exam"].GetProperty("distributionStatus").GetString());
        Assert.Equal("blocked", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
        Assert.Contains(PaperHash, groups["cke-2022-main-basic-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.Contains(RulesHash, groups["cke-2022-main-basic-exam"].GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.DoesNotContain(PaperHash, rights, StringComparison.Ordinal);
        Assert.DoesNotContain(RulesHash, rights, StringComparison.Ordinal);
        Assert.Contains(PaperHash, coverage, StringComparison.Ordinal);
        Assert.Contains(RulesHash, coverage, StringComparison.Ordinal);
        Assert.Contains("MATURA_2022_BASIC_COVERAGE.md", toc, StringComparison.Ordinal);
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
