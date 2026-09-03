using System.Text.Json;
using Abituria.Models;
using Abituria.Services;

namespace Abituria.Tests;

public sealed class Matura2026ContentTests
{
    private const string ExamId = "matura-maj-2026-podstawowa";
    private const string PaperUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2026/Matematyka/poziom_podstawowy/MMAP-P0-100-A-2605-arkusz.pdf";
    private const string RulesUrl = "https://cke.gov.pl/images/_EGZAMIN_MATURALNY_OD_2023/Arkusze_egzaminacyjne/2026/Matematyka/poziom_podstawowy/MMAP-P0-100-2605-zasady.pdf";
    private const string PaperHash = "B7BD89434CA5CCFA33824B0CF063FF7CDDFF47B353059ECF225418E29BEEE71D";
    private const string RulesHash = "A982890CF5EA17206266E4A64B7BFDF96F46FAB08C7435B022CCE5B3908A65AC";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Official_exam_contract_is_exactly_33_tasks_37_progress_items_and_50_points()
    {
        var catalog = Read<ExamCatalog>("Content/exam-2026-main-basic.json");
        var exam = catalog.Exam;
        var expectedLabels = new[]
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12.1", "12.2",
            "13.1", "13.2", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23",
            "24.1", "24.2", "25", "26", "27", "28", "29", "30", "31", "32", "33.1", "33.2"
        };
        var expectedSourcePages = new[]
        {
            4, 4, 5, 5, 6, 6, 7, 8, 8, 9, 10, 12, 13, 14, 15, 16, 18, 19, 19,
            20, 21, 22, 23, 24, 24, 25, 25, 26, 26, 27, 28, 28, 29, 30, 31, 32, 33
        };
        var expectedSolutionPages = new[]
        {
            2, 3, 3, 4, 4, 5, 6, 8, 8, 9, 13, 15, 16, 17, 17, 18, 24, 25, 26,
            26, 27, 27, 28, 30, 31, 31, 32, 32, 33, 33, 34, 35, 35, 37, 37, 38, 38
        };
        var expectedPoints = new[]
        {
            1, 1, 1, 1, 1, 1, 2, 1, 1, 2, 2, 2, 2, 1, 1, 4, 3, 1, 1,
            1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1, 1
        };

        Assert.Equal(4, catalog.SchemaVersion);
        Assert.Equal(ExamId, exam.Id);
        Assert.Equal("Matura maj 2026 - poziom podstawowy", exam.Title);
        Assert.Equal(2026, exam.Year);
        Assert.Equal("główna", exam.Session);
        Assert.Equal("2023", exam.Formula);
        Assert.Equal("basic", exam.Level);
        Assert.Equal(180, exam.DurationMinutes);
        Assert.Equal(50, exam.MaximumPoints);
        Assert.Equal(33, exam.OfficialTaskCount);
        Assert.Equal(37, exam.ProgressItemCount);
        Assert.Equal(37, exam.Exercises.Count);
        Assert.Equal(50, exam.Exercises.Sum(item => item.Points));
        Assert.Equal(expectedLabels, exam.Exercises.Select(item => item.DisplayNumber));
        Assert.Equal(expectedSourcePages, exam.Exercises.Select(item => item.SourcePage));
        Assert.Equal(expectedSolutionPages, exam.Exercises.Select(item => item.SolutionSourcePage));
        Assert.Equal(expectedPoints, exam.Exercises.Select(item => item.Points));
        Assert.Equal(Enumerable.Range(1, 37), exam.Exercises.Select(item => item.EffectiveOrder));
        Assert.Equal(Enumerable.Range(1, 33), exam.Exercises.Select(item => item.Number).Distinct());
        Assert.Equal(33, exam.Exercises.Select(OfficialGroupId).Distinct(StringComparer.Ordinal).Count());

        Assert.Equal(37, exam.Exercises.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.All(exam.Exercises, exercise =>
        {
            Assert.StartsWith("mm26-p0-", exercise.Id, StringComparison.Ordinal);
            Assert.InRange(exercise.Id.Length, 1, 79);
            Assert.Equal(ExamId, exercise.ExamId);
            Assert.Equal($"Zadanie {exercise.DisplayNumber}", exercise.Title);
            Assert.InRange(exercise.Points, 1, 4);
            Assert.InRange(exercise.SourcePage, 4, 33);
            Assert.InRange(exercise.SolutionSourcePage, 1, 42);
            Assert.Equal("CKE MMAP-P0-100-A-2605 i MMAP-P0-100-2605", exercise.VerificationSource);
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.False(string.IsNullOrWhiteSpace(exercise.EffectiveSolution));
            Assert.False(string.IsNullOrWhiteSpace(exercise.ScoringCriteria));
        });
    }

    [Fact]
    public void Official_sources_hashes_and_verification_date_are_pinned()
    {
        var source = Read<ExamCatalog>("Content/exam-2026-main-basic.json").Exam.Source;

        Assert.Equal("Centralna Komisja Egzaminacyjna", source.Publisher);
        Assert.Equal("MMAP-P0-100-A-2605", source.DocumentCode);
        Assert.Equal("2026-05-05", source.ExamDate);
        Assert.Equal(PaperUrl, source.QuestionPaperUrl);
        Assert.Equal(PaperHash, source.QuestionPaperSha256);
        Assert.Equal(RulesUrl, source.AnswerKeyUrl);
        Assert.Equal(RulesHash, source.AnswerKeySha256);
        Assert.Equal("2026-08-02", source.VerifiedOn);
        Assert.All(
            new[] { source.QuestionPaperSha256, source.AnswerKeySha256 },
            hash =>
            {
                Assert.Equal(64, hash.Length);
                Assert.True(hash.All(character => char.IsAsciiHexDigit(character) && !char.IsLower(character)));
            });
    }

    [Fact]
    public void Answer_modes_and_keys_match_the_verified_marking_rules()
    {
        var exercises = Read<ExamCatalog>("Content/exam-2026-main-basic.json").Exam.Exercises;
        var expectedChoiceKey = new[]
        {
            3, 2, 3, 2, 1, 3, 4, 1, 2, 3, 3, 2, 4, 2, 4, 4, 4, 1, 3, 4, 1
        };
        var expectedNumeric = new Dictionary<string, double>(StringComparer.Ordinal)
        {
            ["mm26-p0-z11"] = 78,
            ["mm26-p0-z15"] = 41,
            ["mm26-p0-z17"] = 18,
            ["mm26-p0-z22"] = 27,
            ["mm26-p0-z27"] = 128,
            ["mm26-p0-z30"] = 9d / 25d
        };

        Assert.Equal(21, exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(6, exercises.Count(item => item.IsCompound));
        Assert.Equal(6, exercises.Count(item => item.IsNumeric));
        Assert.Equal(4, exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(expectedChoiceKey, exercises.Where(item => item.IsMultipleChoice).Select(item => item.CorrectOption!.Value));
        Assert.Equal(
            ["mm26-p0-z07", "mm26-p0-z10", "mm26-p0-z14", "mm26-p0-z21"],
            exercises.Where(item => item.IsRevealOnly).Select(item => item.Id));
        Assert.Equal(expectedNumeric.Keys.Order(), exercises.Where(item => item.IsNumeric).Select(item => item.Id).Order());
        foreach (var pair in expectedNumeric)
            Assert.Equal(pair.Value, exercises.Single(item => item.Id == pair.Key).ExpectedValue!.Value, 12);

        Assert.Equal([1, 1], CompoundChoiceKey(exercises, "mm26-p0-z05"));
        Assert.Equal([1d, 4d], exercises.Single(item => item.Id == "mm26-p0-z12-1")
            .AnswerParts.Select(item => item.ExpectedValue!.Value));
        Assert.Equal([2, 2], CompoundChoiceKey(exercises, "mm26-p0-z13-1"));
        Assert.Equal([1, 2], CompoundChoiceKey(exercises, "mm26-p0-z25"));
        Assert.Equal([1, 1], CompoundChoiceKey(exercises, "mm26-p0-z31"));
        var intervalTask = exercises.Single(item => item.Id == "mm26-p0-z12-2");
        Assert.Equal(["text", "text"], intervalTask.AnswerParts.Select(item => item.Mode));
        Assert.Equal(["[-2,4]", "<-2,4>"], intervalTask.AnswerParts[0].AcceptedAnswers);
        Assert.Equal(["(-1,4)"], intervalTask.AnswerParts[1].AcceptedAnswers);
        Assert.All(exercises.Where(item => item.IsCompound), item => Assert.Equal(2, item.AnswerParts.Count));
    }

    [Fact]
    public void Canonical_topics_and_vector_diagrams_cover_every_exam_item_without_orphans()
    {
        var index = Read<ExamIndexCatalog>("Content/exams.json");
        var exam = Read<ExamCatalog>("Content/exam-2026-main-basic.json").Exam;
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        var topicIds = index.Topics.Select(item => item.Id).ToHashSet(StringComparer.Ordinal);
        var referenced = exam.Exercises.SelectMany(item => item.DiagramIds).Distinct(StringComparer.Ordinal).Order().ToArray();
        var definitions = diagrams.Diagrams.Where(item => item.SourceId == "cke-2026-main-basic")
            .OrderBy(item => item.Id)
            .ToArray();

        Assert.Equal(17, topicIds.Count);
        Assert.Equal(
            ["basic", "extended", "basic", "extended", "basic", "basic", "extended", "basic", "basic", "basic", "extended", "basic", "extended", "basic", "basic", "extended", "basic", "basic", "extended", "basic", "basic", "extended", "basic", "basic", "extended", "basic", "basic", "extended", "basic", "basic", "extended", "basic"],
            index.Exams.OrderBy(item => item.Order).Select(item => item.Level));
        Assert.Equal(topicIds.Order(), exam.Exercises.Select(item => item.TopicId).Distinct(StringComparer.Ordinal).Order());
        Assert.All(exam.Exercises, item => Assert.Contains(item.TopicId, topicIds));
        Assert.Equal(7, referenced.Length);
        Assert.Equal(referenced, definitions.Select(item => item.Id));
        Assert.Equal([12, 14, 20, 21, 22, 23, 30], definitions.Select(item => item.SourcePage).Order());
        Assert.All(definitions, definition =>
        {
            Assert.False(string.IsNullOrWhiteSpace(definition.AlternativeText));
            Assert.NotEmpty(definition.Primitives);
        });
    }

    [Fact]
    public void Multi_exam_index_preserves_the_2021_contract_and_aggregates_topics_in_index_order()
    {
        var index = Read<ExamIndexCatalog>("Content/exams.json");
        var current = Read<ExamCatalog>("Content/exam-2026-main-basic.json").Exam;
        var extended = Read<ExamCatalog>("Content/exam-2026-main-extended.json").Exam;
        var basic2025 = Read<ExamCatalog>("Content/exam-2025-main-basic.json").Exam;
        var extended2025 = Read<ExamCatalog>("Content/exam-2025-main-extended.json").Exam;
        var correction2025 = Read<ExamCatalog>("Content/exam-2025-correction-basic.json").Exam;
        var basic2024 = Read<ExamCatalog>("Content/exam-2024-main-basic.json").Exam;
        var extended2024 = Read<ExamCatalog>("Content/exam-2024-main-extended.json").Exam;
        var correction2024 = Read<ExamCatalog>("Content/exam-2024-correction-basic.json").Exam;
        var basic2023 = Read<ExamCatalog>("Content/exam-2023-main-basic.json").Exam;
        var correction2023 = Read<ExamCatalog>("Content/exam-2023-correction-basic.json").Exam;
        var extended2023 = Read<ExamCatalog>("Content/exam-2023-main-extended.json").Exam;
        var basic2022 = Read<ExamCatalog>("Content/exam-2022-main-basic.json").Exam;
        var extended2022 = Read<ExamCatalog>("Content/exam-2022-main-extended.json").Exam;
        var correction2022 = Read<ExamCatalog>("Content/exam-2022-correction-basic.json").Exam;
        var basic2021 = Read<ExamCatalog>("Content/exam-2021-main-basic.json").Exam;
        var extended2021 = Read<ExamCatalog>("Content/exam-2021-main-extended.json").Exam;
        var legacy = Read<ExamCatalog>("Content/exam-2021-correction.json").Exam;
        var basic2020 = Read<ExamCatalog>("Content/exam-2020-main-basic.json").Exam;
        var extended2020 = Read<ExamCatalog>("Content/exam-2020-main-extended.json").Exam;
        var correction2020 = Read<ExamCatalog>("Content/exam-2020-correction-basic.json").Exam;
        var basic2019 = Read<ExamCatalog>("Content/exam-2019-main-basic.json").Exam;
        var extended2019 = Read<ExamCatalog>("Content/exam-2019-main-extended.json").Exam;
        var correction2019 = Read<ExamCatalog>("Content/exam-2019-correction-basic.json").Exam;
        var basic2018 = Read<ExamCatalog>("Content/exam-2018-main-basic.json").Exam;
        var extended2018 = Read<ExamCatalog>("Content/exam-2018-main-extended.json").Exam;
        var correction2018 = Read<ExamCatalog>("Content/exam-2018-correction-basic.json").Exam;
        var basic2017 = Read<ExamCatalog>("Content/exam-2017-main-basic.json").Exam;
        var extended2017 = Read<ExamCatalog>("Content/exam-2017-main-extended.json").Exam;
        var correction2017 = Read<ExamCatalog>("Content/exam-2017-correction-basic.json").Exam;
        var basic2016 = Read<ExamCatalog>("Content/exam-2016-main-basic.json").Exam;
        var extended2016 = Read<ExamCatalog>("Content/exam-2016-main-extended.json").Exam;
        var correction2016 = Read<ExamCatalog>("Content/exam-2016-correction-basic.json").Exam;
        var exams = new[]
        {
            current, extended, basic2025, extended2025, correction2025, basic2024, extended2024,
            correction2024, basic2023, correction2023, extended2023, basic2022, extended2022,
            correction2022, basic2021, extended2021, legacy, basic2020, extended2020, correction2020,
            basic2019, extended2019, correction2019, basic2018, extended2018, correction2018, basic2017,
            extended2017, correction2017, basic2016, extended2016, correction2016
        };

        Assert.Equal(
            [
                ExamId,
                "matura-maj-2026-rozszerzona",
                "matura-maj-2025-podstawowa",
                "matura-maj-2025-rozszerzona",
                "matura-poprawkowa-2025-podstawowa",
                "matura-maj-2024-podstawowa",
                "matura-maj-2024-rozszerzona",
                "matura-poprawkowa-2024-podstawowa",
                "matura-maj-2023-podstawowa",
                "matura-poprawkowa-2023-podstawowa",
                "matura-maj-2023-rozszerzona",
                "matura-maj-2022-podstawowa",
                "matura-maj-2022-rozszerzona",
                "matura-poprawkowa-2022-podstawowa",
                "matura-maj-2021-podstawowa",
                "matura-maj-2021-rozszerzona",
                "matura-poprawkowa-2021",
                "matura-maj-2020-podstawowa",
                "matura-maj-2020-rozszerzona",
                "matura-poprawkowa-2020-podstawowa",
                "matura-maj-2019-podstawowa",
                "matura-maj-2019-rozszerzona",
                "matura-poprawkowa-2019-podstawowa",
                "matura-maj-2018-podstawowa",
                "matura-maj-2018-rozszerzona",
                "matura-poprawkowa-2018-podstawowa",
                "matura-maj-2017-podstawowa",
                "matura-maj-2017-rozszerzona",
                "matura-poprawkowa-2017-podstawowa",
                "matura-maj-2016-podstawowa",
                "matura-maj-2016-rozszerzona",
                "matura-poprawkowa-2016-podstawowa"
            ],
            index.Exams.Where(item => item.IsActive).OrderBy(item => item.Order).Select(item => item.Id));
        Assert.Equal(35, legacy.Exercises.Count);
        Assert.Equal(Enumerable.Range(1, 35).Select(number => $"mp21-z{number}"), legacy.Exercises.Select(item => item.Id));
        Assert.Equal(37, current.Exercises.Count);
        Assert.Equal(13, extended.Exercises.Count);
        Assert.Equal(35, basic2025.Exercises.Count);
        Assert.Equal(13, extended2025.Exercises.Count);
        Assert.Equal(36, correction2025.Exercises.Count);
        Assert.Equal(35, basic2024.Exercises.Count);
        Assert.Equal(14, extended2024.Exercises.Count);
        Assert.Equal(36, correction2024.Exercises.Count);
        Assert.Equal(34, basic2023.Exercises.Count);
        Assert.Equal(36, correction2023.Exercises.Count);
        Assert.Equal(14, extended2023.Exercises.Count);
        Assert.Equal(35, basic2022.Exercises.Count);
        Assert.Equal(15, extended2022.Exercises.Count);
        Assert.Equal(35, correction2022.Exercises.Count);
        Assert.Equal(35, basic2021.Exercises.Count);
        Assert.Equal(15, extended2021.Exercises.Count);
        Assert.Equal(34, basic2020.Exercises.Count);
        Assert.Equal(15, extended2020.Exercises.Count);
        Assert.Equal(34, correction2020.Exercises.Count);
        Assert.Equal(34, basic2019.Exercises.Count);
        Assert.Equal(15, extended2019.Exercises.Count);
        Assert.Equal(34, correction2019.Exercises.Count);
        Assert.Equal(34, basic2018.Exercises.Count);
        Assert.Equal(15, extended2018.Exercises.Count);
        Assert.Equal(34, correction2018.Exercises.Count);
        Assert.Equal(34, basic2017.Exercises.Count);
        Assert.Equal(15, extended2017.Exercises.Count);
        Assert.Equal(34, correction2017.Exercises.Count);
        Assert.Equal(34, basic2016.Exercises.Count);
        Assert.Equal(16, extended2016.Exercises.Count);
        Assert.Equal(34, correction2016.Exercises.Count);
        Assert.Equal(17, index.Topics.Count);

        foreach (var topic in index.Topics.OrderBy(item => item.Order))
        {
            var expected = exams.SelectMany(exam => exam.Exercises)
                .Where(exercise => exercise.TopicId == topic.Id)
                .Select(exercise => exercise.Id);
            Assert.NotEmpty(expected);
        }
    }

    [Fact]
    public void Approved_2026_assets_are_retained_while_new_archives_block_release()
    {
        using var provenance = JsonDocument.Parse(File.ReadAllText(Absolute("Content/provenance.json")));
        var root = provenance.RootElement;
        var groups = root.GetProperty("assets").EnumerateArray().ToDictionary(
            item => item.GetProperty("id").GetString()!,
            StringComparer.Ordinal);
        var rights = File.ReadAllText(Absolute("docs/ASSET_RIGHTS_DECLARATION.md"));
        var coverage = File.ReadAllText(Absolute("docs/MATURA_2026_COVERAGE.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));

        Assert.Equal("approved", groups["cke-2026-main-basic-exam"].GetProperty("distributionStatus").GetString());
        Assert.False(root.GetProperty("releaseEligible").GetBoolean());
        Assert.Equal("blocked", groups["runtime-vector-diagrams"].GetProperty("distributionStatus").GetString());
        Assert.Contains("MMAP-P0-100-A-2605", rights, StringComparison.Ordinal);
        Assert.Contains(PaperHash, rights, StringComparison.Ordinal);
        Assert.Contains(RulesHash, rights, StringComparison.Ordinal);
        Assert.Contains("autorskich implementacji wektorowych Avalonia", rights, StringComparison.Ordinal);
        Assert.Contains(PaperHash, coverage, StringComparison.Ordinal);
        Assert.Contains(RulesHash, coverage, StringComparison.Ordinal);
        Assert.Contains("MATURA_2026_COVERAGE.md", toc, StringComparison.Ordinal);
    }

    private static string OfficialGroupId(LearningExercise exercise) =>
        string.IsNullOrWhiteSpace(exercise.GroupId) ? exercise.Id : exercise.GroupId;

    private static IEnumerable<int> CompoundChoiceKey(
        IEnumerable<LearningExercise> exercises,
        string exerciseId) => exercises.Single(item => item.Id == exerciseId)
        .AnswerParts.Select(item => item.CorrectOption!.Value);

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
