using System.Diagnostics;
using System.Text.Json;
using Abituria.Data;
using Abituria.Models;
using Abituria.Services;
using Abituria.Ui;
using Abituria.ViewModels;
using Abituria.Views;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Microsoft.Data.Sqlite;

namespace Abituria.Tests;

public sealed class MathCourse2023ContentTests
{
    private const string Author = "Adam Kubiś";
    private const string LegalSourceId = "legal-basis-2024";
    private const string LegalHash = "4EC3AD07DC6912223F9973991F647B8759E7D41EB9889B94F396D6935FD5F8ED";
    private const string BasicGuideHash = "88A0EA8E2EE444506CCA5E89C860178E33B04F181650A36D9C9B4DC9BBE625B2";
    private const string ExtendedGuideHash = "BD408CDC8877E04EC79AAC3177FAB304E6F66C6B5FA152D8D3436D4ACFB2BC6F";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();
    private static readonly Size[] SupportedSizes = [new(720, 520), new(960, 640), new(1280, 820)];
    private static readonly string[] SupplementalLessonIds = ["prime-numbers", "greek-alphabet"];
    private static readonly string[] SupportedLessonLevels = ["basic", "extended", "supplemental"];
    private static readonly string[] SupportedExerciseLevels = ["basic", "extended"];
    private static readonly string[] RetainedLessonIds =
    [
        "natural-numbers",
        "prime-numbers",
        "sets-and-logic",
        "logarithms",
        "equations-and-inequalities",
        "quadratic-function",
        "number-sequences",
        "vectors",
        "greek-alphabet"
    ];

    [Fact]
    public void Catalog_matches_complete_formula_2023_coverage_contract()
    {
        var course = Read<MathCourseCatalog>("Content/chapters.json");
        var exerciseCatalog = Read<CourseExerciseCatalog>("Content/course-exercises.json");

        Assert.Equal(4, course.SchemaVersion);
        Assert.Equal(Author, course.Author);
        Assert.Equal(1, exerciseCatalog.SchemaVersion);
        Assert.Equal(Author, exerciseCatalog.Author);
        Assert.Equal(4, course.Groups.Count);
        Assert.Equal(13, course.Areas.Count);
        Assert.Equal(73, course.Requirements.Count(item => item.Level == "basic"));
        Assert.Equal(46, course.Requirements.Count(item => item.Level == "extended"));
        Assert.Equal(119, course.Requirements.Count);
        Assert.Equal(238, course.Lessons.SelectMany(item => item.WorkedExamples).Count());
        Assert.Equal(357, exerciseCatalog.Exercises.Count);
        Assert.Equal(119, exerciseCatalog.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(119, exerciseCatalog.Exercises.Count(item => item.IsNumeric));
        Assert.Equal(119, exerciseCatalog.Exercises.Count(item => item.IsRevealOnly));
        Assert.Equal(219, exerciseCatalog.Exercises.Count(item => item.Level == "basic"));
        Assert.Equal(138, exerciseCatalog.Exercises.Count(item => item.Level == "extended"));

        AssertSources(course);
        AssertGroupAndAreaMapping(course);
        AssertRequirementPackages(course, exerciseCatalog);
        AssertExerciseContracts(exerciseCatalog);
        Assert.All(RetainedLessonIds, id => Assert.Contains(course.Lessons, lesson => lesson.Id == id));
        foreach (var supplementalId in SupplementalLessonIds)
        {
            var supplemental = course.Lessons.Single(item => item.Id == supplementalId);
            Assert.Equal("supplemental", supplemental.Level);
            Assert.True(supplemental.AlwaysVisible);
            Assert.NotEmpty(supplemental.Blocks);
        }
        Assert.Contains("756", course.Lessons.Single(item => item.Id == "prime-numbers").Blocks.Single().Text,
            StringComparison.Ordinal);
        Assert.All(course.Lessons, lesson => Assert.Contains(lesson.Level, SupportedLessonLevels));
    }

    [Fact]
    public void Course_diagrams_use_stable_catalog_ids_with_alternative_descriptions()
    {
        var course = Read<MathCourseCatalog>("Content/chapters.json");
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        var diagramBlocks = course.Lessons.SelectMany(lesson => lesson.Blocks)
            .Where(block => block.Type == "diagram")
            .ToArray();

        Assert.Equal(12, diagramBlocks.Length);
        Assert.Equal(12, diagramBlocks.Select(block => block.DiagramId).Distinct(StringComparer.Ordinal).Count());
        Assert.All(diagramBlocks, block =>
            Assert.Contains(diagrams.Diagrams, diagram => diagram.Id == block.DiagramId &&
                !string.IsNullOrWhiteSpace(diagram.AlternativeText)));
        Assert.Equal(4, diagrams.Diagrams.Count(diagram => diagram.SourceId == "adam-course"));
        Assert.Equal(8, diagrams.Diagrams.Count(diagram => diagram.SourceId == "legacy-vectors"));
    }

    [Fact]
    public void Split_source_stages_and_generated_document_match_declared_totals()
    {
        var stageFiles = Directory.GetFiles(Absolute("tools/seeds/math-course"), "stage-*.json")
            .Order(StringComparer.Ordinal)
            .ToArray();
        Assert.Equal(4, stageFiles.Length);
        Assert.Equal([35, 33, 43, 8], stageFiles.Select(ReadStageCount));

        var requirements = stageFiles.SelectMany(ReadStageRequirements).ToArray();
        Assert.Equal(119, requirements.Length);
        Assert.Equal(119, requirements.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(73, requirements.Count(item => item.Level == "basic"));
        Assert.Equal(46, requirements.Count(item => item.Level == "extended"));

        var learningFiles = Directory.GetFiles(Absolute("tools/seeds/math-course"), "learning-stage-*.json")
            .Order(StringComparer.Ordinal)
            .ToArray();
        Assert.Equal(4, learningFiles.Length);
        var scenarios = learningFiles.SelectMany(ReadLearningScenarios).ToArray();
        Assert.Equal(119, scenarios.Length);
        Assert.Equal(requirements.Select(item => item.Id).Order(), scenarios.Select(item => item.Id).Order());
        Assert.All(scenarios, scenario => Assert.Equal(4, scenario.Prompts.Distinct(StringComparer.Ordinal).Count()));

        var coverage = File.ReadAllText(Absolute("docs/MATH_COURSE_2023_COVERAGE.md"));
        foreach (var marker in new[] { "4 / 4", "13 / 13", "73 / 73", "46 / 46", "238 / 238", "357 / 357" })
            Assert.Contains(marker, coverage, StringComparison.Ordinal);
        Assert.Contains("Content/chapters.json", coverage, StringComparison.Ordinal);
        Assert.Contains("Content/course-exercises.json", coverage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Generator_reproduces_both_catalogs_without_touching_the_legacy_repository()
    {
        var temporaryRoot = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var outputRoot = Path.Combine(temporaryRoot, "Content");
        var documentationPath = Path.Combine(temporaryRoot, "coverage.md");
        Directory.CreateDirectory(outputRoot);

        try
        {
            var result = await RunPowerShellAsync(
                Absolute("tools/New-MathCourseContent.ps1"),
                TestContext.Current.CancellationToken,
                "-ContentRoot",
                outputRoot,
                "-DocumentationPath",
                documentationPath,
                "-LegacyCatalogPath",
                Absolute("Content/chapters.json"));
            Assert.True(result.ExitCode == 0, result.StandardOutput + Environment.NewLine + result.StandardError);

            AssertJsonEquivalent(Absolute("Content/chapters.json"), Path.Combine(outputRoot, "chapters.json"));
            AssertJsonEquivalent(Absolute("Content/course-exercises.json"), Path.Combine(outputRoot, "course-exercises.json"));
            Assert.True(File.Exists(documentationPath));
            var generatedDocumentation = NormalizeLineEndings(File.ReadAllText(documentationPath));
            var trackedDocumentation = NormalizeLineEndings(File.ReadAllText(Absolute("docs/MATH_COURSE_2023_COVERAGE.md")));
            Assert.Equal(trackedDocumentation, generatedDocumentation);
            Assert.Contains("- [Rozporządzenie Ministra Edukacji", generatedDocumentation, StringComparison.Ordinal);
            Assert.Contains("  - wydawca: Rzeczpospolita Polska", generatedDocumentation, StringComparison.Ordinal);
            Assert.Contains($"  - SHA-256: `{LegalHash}`", generatedDocumentation, StringComparison.Ordinal);
            Assert.Contains($"  - SHA-256: `{BasicGuideHash}`", generatedDocumentation, StringComparison.Ordinal);
            Assert.Contains($"  - SHA-256: `{ExtendedGuideHash}`", generatedDocumentation, StringComparison.Ordinal);
            Assert.Equal(3, generatedDocumentation.Split("  - weryfikacja: 2026-07-28", StringSplitOptions.None).Length - 1);
            foreach (var forbidden in new[] { "$(@{", ".documentSha256", "System.Management.Automation" })
                Assert.DoesNotContain(forbidden, generatedDocumentation, StringComparison.Ordinal);
            Assert.Contains("119 ", result.StandardOutput, StringComparison.Ordinal);
            Assert.Contains("357 ", result.StandardOutput, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(temporaryRoot)) Directory.Delete(temporaryRoot, true);
        }
    }

    [Fact]
    public void Level_filter_includes_basic_material_and_only_adds_extended_requirements()
    {
        var course = Read<MathCourseCatalog>("Content/chapters.json");
        var basicRequirementCount = 0;
        var extendedRequirementCount = 0;

        foreach (var area in course.Areas)
        {
            var basicRequirements = MathCourseNavigation.GetVisibleRequirements(course, area.Id, CourseLevelFilter.Basic);
            var extendedRequirements = MathCourseNavigation.GetVisibleRequirements(course, area.Id, CourseLevelFilter.Extended);
            var basicLessons = MathCourseNavigation.GetVisibleLessons(course, area.Id, CourseLevelFilter.Basic);
            var extendedLessons = MathCourseNavigation.GetVisibleLessons(course, area.Id, CourseLevelFilter.Extended);

            Assert.All(basicRequirements, item => Assert.Equal("basic", item.Level));
            Assert.Subset(extendedRequirements.Select(item => item.Id).ToHashSet(StringComparer.Ordinal),
                basicRequirements.Select(item => item.Id).ToHashSet(StringComparer.Ordinal));
            Assert.True(extendedLessons.Length >= basicLessons.Length);
            Assert.All(basicLessons, lesson => Assert.True(lesson.Level == "basic" || lesson.AlwaysVisible));
            basicRequirementCount += basicRequirements.Length;
            extendedRequirementCount += extendedRequirements.Length;
        }

        Assert.Equal(73, basicRequirementCount);
        Assert.Equal(119, extendedRequirementCount);
        var greek = course.Lessons.Single(item => item.Id == "greek-alphabet");
        var extendedLesson = course.Lessons.First(item => item.Level == "extended");
        Assert.True(MathCourseNavigation.IsVisible(greek, CourseLevelFilter.Basic));
        Assert.True(MathCourseNavigation.IsVisible(greek, CourseLevelFilter.Extended));
        Assert.False(MathCourseNavigation.IsVisible(extendedLesson, CourseLevelFilter.Basic));
        Assert.True(MathCourseNavigation.IsVisible(extendedLesson, CourseLevelFilter.Extended));
    }

    [Theory]
    [InlineData("2,5", 2.5)]
    [InlineData("2.5", 2.5)]
    [InlineData("5/2", 2.5)]
    [InlineData("1+1,5", 2.5)]
    public void Numeric_answers_accept_safe_expressions_and_both_decimal_separators(string answer, double expected)
    {
        var exercise = NumericExercise(expected);
        var result = new NumericAnswerEvaluator(new ExpressionCalculator()).Evaluate(exercise, answer);

        Assert.True(result.IsValidInput);
        Assert.True(result.IsCorrect);
        Assert.Contains("Poprawny", result.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Numeric_answers_apply_absolute_and_relative_tolerance_and_reject_invalid_results()
    {
        var evaluator = new NumericAnswerEvaluator(new ExpressionCalculator());
        var absolute = NumericExercise(0d);
        absolute.AbsoluteTolerance = 0.01;
        absolute.RelativeTolerance = 0d;
        Assert.True(evaluator.Evaluate(absolute, "0.009").IsCorrect);
        Assert.False(evaluator.Evaluate(absolute, "0.02").IsCorrect);

        var relative = NumericExercise(1_000_000d);
        relative.AbsoluteTolerance = 0d;
        relative.RelativeTolerance = 0.000001;
        Assert.True(evaluator.Evaluate(relative, "1000000.5").IsCorrect);
        Assert.False(evaluator.Evaluate(relative, "1000002").IsCorrect);

        foreach (var invalid in new[] { string.Empty, "1/0", "NaN", "Infinity", "∞", "abc" })
        {
            var result = evaluator.Evaluate(relative, invalid);
            Assert.False(result.IsValidInput);
            Assert.False(result.IsCorrect);
            Assert.False(string.IsNullOrWhiteSpace(result.Message));
        }
    }

    [Fact]
    public void Numeric_answer_configuration_rejects_missing_values_wrong_modes_and_invalid_tolerances()
    {
        var evaluator = new NumericAnswerEvaluator(new ExpressionCalculator());
        var wrongMode = NumericExercise(2d);
        wrongMode.Mode = "revealOnly";
        Assert.False(evaluator.Evaluate(wrongMode, "2").IsValidInput);

        var missing = NumericExercise(2d);
        missing.ExpectedValue = null;
        Assert.False(evaluator.Evaluate(missing, "2").IsValidInput);

        var nonFinite = NumericExercise(double.PositiveInfinity);
        Assert.False(evaluator.Evaluate(nonFinite, "2").IsValidInput);

        var negativeTolerance = NumericExercise(2d);
        negativeTolerance.AbsoluteTolerance = -1d;
        Assert.False(evaluator.Evaluate(negativeTolerance, "2").IsValidInput);

        var infiniteTolerance = NumericExercise(2d);
        infiniteTolerance.RelativeTolerance = double.PositiveInfinity;
        Assert.False(evaluator.Evaluate(infiniteTolerance, "2").IsValidInput);
        Assert.Throws<ArgumentNullException>(() => new NumericAnswerEvaluator(null!));
        Assert.Throws<ArgumentNullException>(() => evaluator.Evaluate(null!, "2"));
    }

    [AvaloniaFact]
    public void Course_views_render_filter_navigation_examples_exercises_and_accessibility_at_supported_sizes()
    {
        var course = Read<MathCourseCatalog>("Content/chapters.json");
        var exercises = Read<CourseExerciseCatalog>("Content/course-exercises.json");
        var openedAreas = new List<CourseArea>();
        var selectedLevels = new List<CourseLevelFilter>();
        var list = new ChapterListView(course, CourseLevelFilter.Basic, selectedLevels.Add, openedAreas.Add);
        var window = new Window { Width = 960, Height = 640, Content = list };

        try
        {
            window.Show();
            Render();
            var firstAreaButton = list.GetLogicalDescendants().OfType<Button>()
                .First(button => AutomationProperties.GetName(button)?.StartsWith("Otwórz obszar", StringComparison.Ordinal) == true);
            Assert.True(firstAreaButton.Focus(NavigationMethod.Tab));
            Assert.True(firstAreaButton.IsFocused);
            Click(list, "Rozszerzony");
            Assert.Equal([CourseLevelFilter.Extended], selectedLevels);
            ClickStartingWith(list, "I. Liczby rzeczywiste - ");
            Assert.Equal("area-i", Assert.Single(openedAreas).Id);
            AssertResponsive(window, list);

            var area = course.Areas.Single(item => item.Id == "area-i");
            var openedLessons = new List<MathCourseLesson>();
            var areaView = new CourseAreaView(
                course,
                area,
                CourseLevelFilter.Basic,
                selectedLevels.Add,
                openedLessons.Add,
                () => { });
            window.Content = areaView;
            Render();
            Assert.Contains(areaView.GetLogicalDescendants().OfType<Button>(), button =>
                Equals(AutomationProperties.GetName(button), "Otwórz lekcję: Alfabet grecki"));
            Assert.DoesNotContain(areaView.GetLogicalDescendants().OfType<Button>(), button =>
                Equals(AutomationProperties.GetName(button), "Otwórz lekcję: Zmiana podstawy logarytmu"));

            var extendedAreaView = new CourseAreaView(
                course,
                area,
                CourseLevelFilter.Extended,
                selectedLevels.Add,
                openedLessons.Add,
                () => { });
            window.Content = extendedAreaView;
            Render();
            Assert.Contains(extendedAreaView.GetLogicalDescendants().OfType<Button>(), button =>
                Equals(AutomationProperties.GetName(button), "Otwórz lekcję: Zmiana podstawy logarytmu"));
            AssertResponsive(window, extendedAreaView);

            var lesson = course.Lessons.Single(item => item.Id == "real-numbers");
            var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
            var openedExercises = new List<LearningExercise>();
            var lessonView = new CourseLessonView(
                course,
                exercises,
                lesson,
                CourseLevelFilter.Basic,
                openedExercises.Add,
                () => { },
                new CourseLessonResources(diagrams));
            window.Content = lessonView;
            Render();
            Assert.Equal(lesson.WorkedExamples.Count, lessonView.GetLogicalDescendants().OfType<TextBlock>()
                .Count(text => text.Text?.StartsWith("Przykład ", StringComparison.Ordinal) == true));
            Click(lessonView, exercises.Exercises.Single(item => item.Id == lesson.ExerciseIds[0]).Title);
            Assert.Equal(lesson.ExerciseIds[0], Assert.Single(openedExercises).Id);
            AssertResponsive(window, lessonView);

            var lessonExercises = lesson.ExerciseIds
                .Select(id => exercises.Exercises.Single(item => item.Id == id))
                .ToArray();
            var navigatedExercises = new List<LearningExercise>();
            var navigationContext = new ExerciseViewContext(
                lessonExercises,
                new SourceDocument { VerifiedOn = "2026-07-28" },
                Read<UiCopyCatalog>("Content/ui-copy.json"),
                new LocalProfile(Guid.NewGuid(), "Kurs", ProfileKind.Guest),
                new AccountService(
                    new AppDbContextFactory(Path.Combine(Path.GetTempPath(), $"course-navigation-{Guid.NewGuid():N}.db")),
                    new PasswordHasher(1_000)),
                diagrams,
                () => { },
                navigatedExercises.Add)
            {
                BackLabel = "Lekcja",
                SourceUrl = course.Sources.Single(item => item.Id == LegalSourceId).DocumentUrl
            };
            var firstExerciseView = new ExerciseView(lessonExercises[0], navigationContext);
            window.Content = firstExerciseView;
            Render();
            AssertResponsive(window, firstExerciseView);
            Click(firstExerciseView, "→");
            Assert.Equal(lessonExercises[1].Id, Assert.Single(navigatedExercises).Id);
            navigatedExercises.Clear();
            var secondExerciseView = new ExerciseView(lessonExercises[1], navigationContext);
            window.Content = secondExerciseView;
            Render();
            Click(secondExerciseView, "←");
            Assert.Equal(lessonExercises[0].Id, Assert.Single(navigatedExercises).Id);

            var diagramLesson = course.Lessons.Single(item => item.Id == "trigonometry");
            var diagramView = new CourseLessonView(
                course,
                exercises,
                diagramLesson,
                CourseLevelFilter.Basic,
                _ => { },
                () => { },
                new CourseLessonResources(diagrams));
            window.Content = diagramView;
            Render();
            Assert.Contains(diagramView.GetLogicalDescendants().OfType<DiagramView>(), diagram =>
                AutomationProperties.GetName(diagram)?.Contains("Trójkąt prostokątny", StringComparison.Ordinal) == true);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task Numeric_view_marks_only_correct_answers_and_profile_separates_all_three_progress_buckets()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var databasePath = Path.Combine(directory, "course-progress.db");
        var accounts = new AccountService(new AppDbContextFactory(databasePath), new PasswordHasher(1_000));
        await accounts.InitializeAsync();
        var profile = (await accounts.GetProfilesAsync()).Single(item => item.Kind == ProfileKind.Guest);
        var repository = new ContentRepository();
        var basic = repository.CourseExercises.Exercises.First(item => item.IsNumeric && item.Level == "basic");
        var extended = repository.CourseExercises.Exercises.First(item => item.IsNumeric && item.Level == "extended");
        var context = new ExerciseViewContext(
            [basic],
            new SourceDocument { VerifiedOn = "2026-07-28" },
            repository.UiCopy,
            profile,
            accounts,
            repository.Diagrams,
            () => { },
            _ => { })
        {
            BackLabel = "Lekcja",
            SourceUrl = repository.MathCourse.Sources.Single(item => item.Id == LegalSourceId).DocumentUrl
        };
        var view = new ExerciseView(basic, context);
        var window = new Window { Width = 720, Height = 520, Content = view };

        try
        {
            window.Show();
            Render();
            var answer = view.GetLogicalDescendants().OfType<TextBox>()
                .Single(control => AutomationProperties.GetName(control) == "Odpowiedź liczbowa");
            answer.Text = "bad";
            Click(view, "Sprawdź wynik");
            Assert.DoesNotContain(basic.Id, await accounts.GetCompletedExerciseIdsAsync(profile.Id));

            answer.Text = basic.ExpectedValue!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Click(view, "Sprawdź wynik");
            await WaitUntilAsync(async () => (await accounts.GetCompletedExerciseIdsAsync(profile.Id)).Contains(basic.Id));
            Click(view, "Sprawdź wynik");

            await accounts.MarkExerciseCompletedAsync(profile.Id, extended.Id);
            await accounts.MarkExerciseCompletedAsync(profile.Id, "mp21-z1");
            var profileView = new ProfileView(profile, accounts, repository.Exams, repository.CourseExercises, () => { });
            window.Content = profileView;
            Render();
            await WaitUntilAsync(() => Task.FromResult(profileView.GetLogicalDescendants().OfType<TextBlock>()
                .Any(text => text.Text?.Contains("Podstawa: 1 / 219", StringComparison.Ordinal) == true)));
            var progress = profileView.GetLogicalDescendants().OfType<TextBlock>()
                .Single(text => AutomationProperties.GetName(text) == "Postęp w zadaniach");
            Assert.Contains("Matura maj 2026 PP: 0 / 37", progress.Text, StringComparison.Ordinal);
            Assert.Contains("Matura poprawkowa 2021: 1 / 35", progress.Text, StringComparison.Ordinal);
            Assert.Contains("Podstawa: 1 / 219", progress.Text, StringComparison.Ordinal);
            Assert.Contains("Część rozszerzona: 1 / 138", progress.Text, StringComparison.Ordinal);

            var restarted = new AccountService(new AppDbContextFactory(databasePath), new PasswordHasher(1_000));
            await restarted.InitializeAsync();
            var persisted = await restarted.GetCompletedExerciseIdsAsync(profile.Id);
            Assert.Contains(basic.Id, persisted);
            Assert.Contains(extended.Id, persisted);
            Assert.Contains("mp21-z1", persisted);
        }
        finally
        {
            window.Close();
            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    private static void AssertSources(MathCourseCatalog course)
    {
        Assert.Equal(3, course.Sources.Count);
        Assert.Equal(LegalHash, course.Sources.Single(item => item.Id == LegalSourceId).DocumentSha256);
        Assert.Equal(BasicGuideHash, course.Sources.Single(item => item.Id == "cke-basic-guide-2023").DocumentSha256);
        Assert.Equal(ExtendedGuideHash, course.Sources.Single(item => item.Id == "cke-extended-guide-2023").DocumentSha256);
        Assert.All(course.Sources, source =>
        {
            Assert.StartsWith("https://", source.DocumentUrl, StringComparison.Ordinal);
            Assert.Equal(64, source.DocumentSha256.Length);
            Assert.Equal("2026-07-28", source.VerifiedOn);
        });
    }

    private static void AssertGroupAndAreaMapping(MathCourseCatalog course)
    {
        Assert.Equal(Enumerable.Range(1, 4), course.Groups.Select(item => item.Order));
        Assert.Equal(Enumerable.Range(1, 13), course.Areas.Select(item => item.Order));
        var mappedAreaIds = course.Groups.SelectMany(item => item.AreaIds).ToArray();
        Assert.Equal(13, mappedAreaIds.Length);
        Assert.Equal(13, mappedAreaIds.Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(course.Areas.Select(item => item.Id).Order(), mappedAreaIds.Order());
        Assert.All(course.Areas, area =>
        {
            Assert.Contains(course.Groups, group => group.Id == area.GroupId && group.AreaIds.Contains(area.Id));
            Assert.NotEmpty(area.LessonIds);
            Assert.Equal(area.LessonIds, course.Lessons.Where(lesson => lesson.AreaId == area.Id).Select(lesson => lesson.Id));
        });
    }

    private static void AssertRequirementPackages(MathCourseCatalog course, CourseExerciseCatalog exerciseCatalog)
    {
        var requirementsById = course.Requirements.ToDictionary(item => item.Id, StringComparer.Ordinal);
        var examplesById = course.Lessons.SelectMany(item => item.WorkedExamples)
            .ToDictionary(item => item.Id, StringComparer.Ordinal);
        var exercisesById = exerciseCatalog.Exercises.ToDictionary(item => item.Id, StringComparer.Ordinal);

        Assert.Equal(119, requirementsById.Count);
        Assert.Equal(238, examplesById.Count);
        Assert.Equal(357, exercisesById.Count);
        foreach (var requirement in course.Requirements)
        {
            Assert.False(string.IsNullOrWhiteSpace(requirement.Text));
            Assert.Equal(LegalSourceId, requirement.SourceId);
            Assert.Contains(course.Lessons, lesson => lesson.Id == requirement.LessonId && lesson.RequirementIds.Contains(requirement.Id));
            Assert.Equal(2, requirement.WorkedExampleIds.Count);
            Assert.Equal(3, requirement.ExerciseIds.Count);
            Assert.Equal(["foundation", "exam"], requirement.WorkedExampleIds.Select(id => examplesById[id].Kind));
            Assert.All(requirement.WorkedExampleIds, id =>
            {
                var example = examplesById[id];
                Assert.Equal(requirement.Id, example.RequirementId);
                Assert.Equal(Author, example.Author);
                Assert.False(string.IsNullOrWhiteSpace(example.Prompt));
                Assert.False(string.IsNullOrWhiteSpace(example.Solution));
            });
            Assert.All(requirement.ExerciseIds, id => Assert.Equal(requirement.Id, exercisesById[id].RequirementId));
            Assert.True(exercisesById[requirement.ExerciseIds[0]].IsMultipleChoice);
            Assert.True(exercisesById[requirement.ExerciseIds[1]].IsNumeric);
            Assert.True(exercisesById[requirement.ExerciseIds[2]].IsRevealOnly);
            var packagePrompts = requirement.WorkedExampleIds.Select(id => examplesById[id].Prompt)
                .Concat(requirement.ExerciseIds.Select(id => exercisesById[id].Prompt))
                .ToArray();
            Assert.Equal(5, packagePrompts.Distinct(StringComparer.Ordinal).Count());
        }
    }

    private static void AssertExerciseContracts(CourseExerciseCatalog exerciseCatalog)
    {
        Assert.Equal(357, exerciseCatalog.Exercises.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.All(exerciseCatalog.Exercises, exercise =>
        {
            Assert.StartsWith("course-", exercise.Id, StringComparison.Ordinal);
            Assert.InRange(exercise.Id.Length, 1, 79);
            Assert.Equal(Author, exercise.Author);
            Assert.False(string.IsNullOrWhiteSpace(exercise.RequirementId));
            Assert.Contains(exercise.Level, SupportedExerciseLevels);
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.Equal(2, exercise.Hints.Count);
            Assert.All(exercise.Hints, hint => Assert.False(string.IsNullOrWhiteSpace(hint)));
            Assert.False(string.IsNullOrWhiteSpace(exercise.RevealedAnswer));
            Assert.InRange(Math.Abs(exercise.AbsoluteTolerance - 1e-9), 0d, 1e-15);
            Assert.InRange(Math.Abs(exercise.RelativeTolerance - 1e-9), 0d, 1e-15);

            if (exercise.IsMultipleChoice)
            {
                Assert.Equal(4, exercise.Options.Count);
                Assert.Equal(1, exercise.CorrectOption);
            }
            else if (exercise.IsNumeric)
            {
                Assert.Empty(exercise.Options);
                Assert.Null(exercise.CorrectOption);
                Assert.True(double.IsFinite(exercise.ExpectedValue!.Value));
            }
            else
            {
                Assert.True(exercise.IsRevealOnly);
                Assert.Empty(exercise.Options);
                Assert.Null(exercise.CorrectOption);
            }
        });
    }

    private static LearningExercise NumericExercise(double expected) => new()
    {
        Id = "course-test-numeric",
        Mode = "numeric",
        Prompt = "Podaj wynik.",
        ExpectedValue = expected,
        AbsoluteTolerance = 1e-9,
        RelativeTolerance = 1e-9
    };

    private static int ReadStageCount(string path) => ReadStageRequirements(path).Length;

    private static StageRequirement[] ReadStageRequirements(string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        return document.RootElement.GetProperty("requirements")
            .EnumerateArray()
            .Select(item => new StageRequirement(
                item.GetProperty("id").GetString()!,
                item.GetProperty("level").GetString()!))
            .ToArray();
    }

    private static LearningScenario[] ReadLearningScenarios(string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        return document.RootElement.GetProperty("scenarios")
            .EnumerateArray()
            .Select(item => new LearningScenario(
                item.GetProperty("requirementId").GetString()!,
                [
                    item.GetProperty("foundationPrompt").GetString()!,
                    item.GetProperty("examPrompt").GetString()!,
                    item.GetProperty("numericPrompt").GetString()!,
                    item.GetProperty("reasoningPrompt").GetString()!
                ]))
            .ToArray();
    }

    private static void AssertResponsive(Window window, Control view)
    {
        foreach (var size in SupportedSizes)
        {
            window.Width = size.Width;
            window.Height = size.Height;
            Render();
            var scroll = Assert.Single(view.GetLogicalDescendants().OfType<ScrollViewer>());
            Assert.InRange(view.Bounds.Width, 1d, window.Bounds.Width);
            Assert.InRange(view.Bounds.Height, 1d, window.Bounds.Height);
            Assert.True(scroll.Extent.Width <= scroll.Viewport.Width + 1,
                $"Przepełnienie poziome przy {size.Width}x{size.Height}: {scroll.Extent.Width} > {scroll.Viewport.Width}.");
        }
    }

    private static void Click(Control root, string label)
    {
        var button = root.GetLogicalDescendants().OfType<Button>()
            .Single(item => string.Equals(item.Content as string, label, StringComparison.Ordinal));
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Render();
    }

    private static void ClickStartingWith(Control root, string prefix)
    {
        var button = root.GetLogicalDescendants().OfType<Button>()
            .Single(item => item.Content is string text && text.StartsWith(prefix, StringComparison.Ordinal));
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Render();
    }

    private static async Task WaitUntilAsync(Func<Task<bool>> condition)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            Render();
            if (await condition()) return;
            await Task.Delay(10);
        }

        Assert.Fail("Warunek interfejsu nie został spełniony w wyznaczonym czasie.");
    }

    private static async Task<PowerShellResult> RunPowerShellAsync(
        string scriptPath,
        CancellationToken cancellationToken,
        params string[] arguments)
    {
        var startInfo = new ProcessStartInfo("pwsh")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(scriptPath);
        foreach (var argument in arguments) startInfo.ArgumentList.Add(argument);

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Nie uruchomiono PowerShell 7.");
        var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        return new PowerShellResult(process.ExitCode, await outputTask, await errorTask);
    }

    private static void AssertJsonEquivalent(string expectedPath, string actualPath)
    {
        using var expected = JsonDocument.Parse(File.ReadAllText(expectedPath));
        using var actual = JsonDocument.Parse(File.ReadAllText(actualPath));
        Assert.True(JsonElement.DeepEquals(expected.RootElement, actual.RootElement), actualPath);
    }

    private static string NormalizeLineEndings(string value) => value
        .Replace("\r\n", "\n", StringComparison.Ordinal)
        .Replace('\r', '\n');

    private static void Render() => Dispatcher.UIThread.RunJobs();

    private static T Read<T>(string relativePath) => JsonSerializer.Deserialize<T>(
        File.ReadAllText(Absolute(relativePath)),
        JsonOptions) ?? throw new InvalidDataException($"Nie odczytano {relativePath}.");

    private static string Absolute(string relativePath) =>
        Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Abituria.sln")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Nie znaleziono katalogu repozytorium Abituria.");
    }

    private sealed record StageRequirement(string Id, string Level);
    private sealed record LearningScenario(string Id, string[] Prompts);
    private sealed record PowerShellResult(int ExitCode, string StandardOutput, string StandardError);
}
