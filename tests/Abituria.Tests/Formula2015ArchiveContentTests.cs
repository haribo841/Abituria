using System.Text.Json;
using Abituria.Models;

namespace Abituria.Tests;

public sealed class Formula2015ArchiveContentTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string[] SupportedExerciseModes = ["multipleChoice", "numeric", "compound", "revealOnly"];
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Theory]
    [InlineData("exam-2015-correction-basic.json", "matura-poprawkowa-2015-podstawowa", 2015, "poprawkowa", "basic", 170, 34, 50, "MMA-P1_1P-154", "E10AFB6E7E4525C517AAAB73366A305F9EEFB2BE522B25B6CBB532E33112E2D8", "8CF61A709FD4A0AA929B43B33DA1110905E3CD6D42088C6F7945A3606435F9BC")]
    [InlineData("exam-2015-main-basic.json", "matura-maj-2015-podstawowa", 2015, "główna", "basic", 170, 34, 50, "MMA-P1_1P-152", "677C719F87A6E07F6C5559A218D9DB670F077BC3AEC849BAF37B288D2FEBB3EE", "7DE249A72303E8F922A0B3FB950F25DB88578F7E226CE61EA5F55EE9ED98AC40")]
    [InlineData("exam-2015-main-extended.json", "matura-maj-2015-rozszerzona", 2015, "główna", "extended", 180, 16, 50, "MMA-R1_1P-152", "87D52CFE7F56D7D0CAC3CD4BF952AAC33A2F6EB19B275483BE45333F8B706791", "7F5260F5F2628CD781697DE057CABAF145BA1223836148C70D40AEFA2D90A7E4")]
    [InlineData("exam-2023-f2015-correction-basic.json", "matura-poprawkowa-2023-podstawowa-f2015", 2023, "poprawkowa", "basic", 170, 36, 46, "EMAP-P0-100-2308", "7A1E3BD81B8B281C6DE59B1E53D6C5FFC0E5F94EC542498221C929B9C8F4BF1D", "5D93892F2B51AEF581C701B58C2FF1E1B7580EE9F24296A5B5B567BBCFDEFBA2")]
    [InlineData("exam-2023-f2015-main-basic.json", "matura-maj-2023-podstawowa-f2015", 2023, "główna", "basic", 170, 36, 46, "EMAP-P0-100-2305", "287EF39ABE5097331FB446FD403E636D65CB198E8627FB142AD4424B2A06B1D6", "B29002576C2B6B8948535B7D7C46D6D86B1C23DB236B7FF33228AAACB4741322")]
    [InlineData("exam-2023-f2015-main-extended.json", "matura-maj-2023-rozszerzona-f2015", 2023, "główna", "extended", 180, 16, 50, "EMAP-R0-100-2305", "A74D6D26BCEBFF58779403F3AE879A8C90EE4AEBBABF9D34E7DCFDA1B2A5FA08", "D6A2312FBDFC142F95EA6BC35B61B017C9619803728D16FA119F4D88C30A471A")]
    [InlineData("exam-2024-f2015-correction-basic.json", "matura-poprawkowa-2024-podstawowa-f2015", 2024, "poprawkowa", "basic", 170, 36, 46, "EMAP-P0-100-2408", "992F987394D46A6D2DCD67FC134BA53AB3A5983B613001708B4A1451E6799008", "FF41C64637D5CBB745BAA2E593EAF6897733633C0EDBCDD3B515B2B829F68B89")]
    [InlineData("exam-2024-f2015-main-basic.json", "matura-maj-2024-podstawowa-f2015", 2024, "główna", "basic", 170, 36, 46, "EMAP-P0-100-2405", "480CEFBC34C8D8ABCAB4C671E922DE84E30628C33E44CDC84918F710F3538C35", "3D0C741C6F7F21FB0CAF0D2E06DBC53D0BB37D4D6FCDED0591DFEA8CDCED2587")]
    [InlineData("exam-2024-f2015-main-extended.json", "matura-maj-2024-rozszerzona-f2015", 2024, "główna", "extended", 180, 16, 50, "EMAP-R0-100-2405", "688E5A7E9C6A3E411122AC9C6FCCB4876BA02BEF471BC15CB5ED8133C54C12A5", "BB0BD76B32F70298F035B5A2A9E30067DED5A27A665CDB0B571F2E7FADEF7CAF")]
    [InlineData("exam-2025-f2015-correction-basic.json", "matura-poprawkowa-2025-podstawowa-f2015", 2025, "poprawkowa", "basic", 170, 34, 50, "EMAP-P0-100-2508", "E0CDE8E52C91023AD831E7BC73930A4DB109416FC92CF6A9670DCA308AC3D29D", "34A374F86F40B0709CC1896EC16D62BEF732FDDDFBA231EFC75E23E83453B9BE")]
    [InlineData("exam-2025-f2015-main-basic.json", "matura-maj-2025-podstawowa-f2015", 2025, "główna", "basic", 170, 34, 50, "EMAP-P0-100-2505", "BE6358F95A065193798E42999422BB9E250991F2016B09C09FB5D9CDB504FA7B", "73A6EC2FA481394171B57EA99A39452B7486CEE3BCA1294966EE10E10C0B8E1E")]
    [InlineData("exam-2025-f2015-main-extended.json", "matura-maj-2025-rozszerzona-f2015", 2025, "główna", "extended", 180, 15, 50, "EMAP-R0-100-2505", "D9DDF492BE9B2BEF5C63D4CE9D9074011B00218128E1001D20B3D0FDC6A1240A", "12C474E22794E967F4C5569B503D8F98C61813ACE979E69CABCE64E5C87FC655")]
    [InlineData("exam-2026-f2015-main-basic.json", "matura-maj-2026-podstawowa-f2015", 2026, "główna", "basic", 170, 34, 50, "EMAP-P0-100-2605", "ACFE6B6E600606E6E1EE1666D97D017A31382DF052211840F3DC9E9EEA05DA23", "A5E6BD8CE8709FA7257AAB789103829475D92D4901A9569385246471EFEDAD48")]
    [InlineData("exam-2026-f2015-main-extended.json", "matura-maj-2026-rozszerzona-f2015", 2026, "główna", "extended", 180, 15, 50, "EMAP-R0-100-2605", "B57931121071DA7BB6B14AEB1EACD79F755E4E96715B32E4A126AB6F3E1FA4B0", "4A3F9B36705869C81133C96C11C6FE52ED8683CFB07AC577AF0F9770F1CD02AA")]
    public void Source_pinned_formula_2015_archives_have_complete_task_contracts(
        string fileName,
        string id,
        int year,
        string session,
        string level,
        int durationMinutes,
        int taskCount,
        int maximumPoints,
        string documentCode,
        string paperHash,
        string rulesHash)
    {
        var catalog = Read<ExamCatalog>($"Content/{fileName}");
        var exam = catalog.Exam;

        Assert.Equal(4, catalog.SchemaVersion);
        Assert.Equal((id, year, session, "2015", level, durationMinutes, taskCount, taskCount, maximumPoints),
            (exam.Id, exam.Year, exam.Session, exam.Formula, exam.Level, exam.DurationMinutes, exam.OfficialTaskCount, exam.ProgressItemCount, exam.MaximumPoints));
        Assert.Equal((documentCode, paperHash, rulesHash, "2026-09-04"),
            (exam.Source.DocumentCode, exam.Source.QuestionPaperSha256, exam.Source.AnswerKeySha256, exam.Source.VerifiedOn));
        Assert.Equal(taskCount, exam.Exercises.Count);
        Assert.Equal(maximumPoints, exam.Exercises.Sum(item => item.Points));
        Assert.Equal(Enumerable.Range(1, taskCount), exam.Exercises.Select(item => item.Number));
        Assert.Equal(Enumerable.Range(1, taskCount), exam.Exercises.Select(item => item.EffectiveOrder));
        Assert.All(exam.Exercises, exercise => AssertCompleteExercise(exam, exercise));
    }

    [Fact]
    public void Catalog_has_a_complete_attestable_formula_2015_timeline_and_keeps_2026_corrections_pending()
    {
        var index = Read<ExamIndexCatalog>("Content/exams.json");
        var exams = index.Exams.Where(item => item.IsActive)
            .OrderBy(item => item.Order)
            .Select(item => Read<ExamCatalog>(item.ContentPath).Exam)
            .ToArray();
        var ids = exams.Select(item => item.Id).ToArray();
        var formula2015 = exams.Where(item => item.Formula == "2015").ToArray();

        Assert.Equal(46, exams.Length);
        Assert.Equal(1_281, exams.Sum(exam => exam.Exercises.Count));
        Assert.Equal(35, formula2015.Length);
        Assert.Equal(2015, formula2015.Min(item => item.Year));
        Assert.Equal(2026, formula2015.Max(item => item.Year));
        Assert.Contains("matura-maj-2015-podstawowa", ids);
        Assert.Contains("matura-maj-2015-rozszerzona", ids);
        Assert.Contains("matura-poprawkowa-2015-podstawowa", ids);

        foreach (var year in Enumerable.Range(2023, 4))
        {
            Assert.Contains($"matura-maj-{year}-podstawowa-f2015", ids);
            Assert.Contains($"matura-maj-{year}-rozszerzona-f2015", ids);
        }

        foreach (var year in Enumerable.Range(2023, 3))
            Assert.Contains($"matura-poprawkowa-{year}-podstawowa-f2015", ids);

        Assert.DoesNotContain("matura-poprawkowa-2026-podstawowa", ids);
        Assert.DoesNotContain("matura-poprawkowa-2026-podstawowa-f2015", ids);
    }

    [Fact]
    public void Source_figures_required_by_the_new_formula_2015_transcriptions_are_vector_diagrams_with_accessible_text()
    {
        var expectedDiagramIds = new[]
        {
            "exam-mm15-p0-z08", "exam-mm15-p0-z14", "exam-mm15-p0-z21", "exam-mm15-p0-z33", "exam-mm15-r0-z01",
            "exam-mm15-p0p-z07", "exam-mm15-p0p-z19", "exam-em23-p0-z05", "exam-em23-p0-z10", "exam-em23-p0-z11-13",
            "exam-em23-r0-z11", "exam-em23-p0p-z12-13", "exam-em23-p0p-z28", "exam-em24-p0-z10", "exam-em24-p0-z11",
            "exam-em24-p0-z26-27", "exam-em24-p0p-z08", "exam-em24-p0p-z29", "exam-em25-p0-z06", "exam-em25-p0-z30",
            "exam-em25-p0p-z30", "exam-em26-p0-z28", "exam-em26-p0-z34"
        };
        var sourceExamFiles = new[]
        {
            "exam-2015-correction-basic.json", "exam-2015-main-basic.json", "exam-2015-main-extended.json",
            "exam-2023-f2015-correction-basic.json", "exam-2023-f2015-main-basic.json", "exam-2023-f2015-main-extended.json",
            "exam-2024-f2015-correction-basic.json", "exam-2024-f2015-main-basic.json", "exam-2024-f2015-main-extended.json",
            "exam-2025-f2015-correction-basic.json", "exam-2025-f2015-main-basic.json", "exam-2025-f2015-main-extended.json",
            "exam-2026-f2015-main-basic.json", "exam-2026-f2015-main-extended.json"
        };
        var references = sourceExamFiles
            .SelectMany(file => Read<ExamCatalog>($"Content/{file}").Exam.Exercises)
            .SelectMany(exercise => exercise.DiagramIds)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json").Diagrams
            .Where(diagram => expectedDiagramIds.Contains(diagram.Id, StringComparer.Ordinal))
            .ToArray();

        Assert.Equal(expectedDiagramIds.OrderBy(id => id, StringComparer.Ordinal), references);
        Assert.Equal(expectedDiagramIds.Length, diagrams.Length);
        Assert.All(diagrams, diagram =>
        {
            Assert.StartsWith("cke-", diagram.SourceId, StringComparison.Ordinal);
            Assert.True(diagram.SourcePage > 0);
            Assert.False(string.IsNullOrWhiteSpace(diagram.AlternativeText));
            Assert.NotEmpty(diagram.Primitives);
        });
    }

    private static void AssertCompleteExercise(ExamDefinition exam, LearningExercise exercise)
    {
        Assert.StartsWith(exam.Year == 2015 ? "mm15-" : $"em{exam.Year % 100:00}-", exercise.Id, StringComparison.Ordinal);
        Assert.InRange(exercise.Id.Length, 1, 79);
        Assert.Equal(exam.Id, exercise.ExamId);
        Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
        Assert.True(exercise.SourcePage > 0);
        Assert.True(exercise.SolutionSourcePage > 0);
        Assert.False(string.IsNullOrWhiteSpace(exercise.VerificationSource));
        Assert.True(exercise.Hints.Count >= 2);
        Assert.All(exercise.Hints, hint => Assert.False(string.IsNullOrWhiteSpace(hint)));
        Assert.False(string.IsNullOrWhiteSpace(exercise.EffectiveSolution));
        Assert.False(string.IsNullOrWhiteSpace(exercise.ScoringCriteria));
        Assert.Contains(exercise.Mode, SupportedExerciseModes);
    }

    private static T Read<T>(string relativePath) => JsonSerializer.Deserialize<T>(
        File.ReadAllText(Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar))),
        JsonOptions) ?? throw new InvalidDataException($"Nie można odczytać {relativePath}.");

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Abituria.csproj")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Nie znaleziono katalogu repozytorium.");
    }
}
