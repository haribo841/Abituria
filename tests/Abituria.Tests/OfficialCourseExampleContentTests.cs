using System.Text.Json;
using Abituria.Models;
using Abituria.Services;
using Abituria.ViewModels;
using Abituria.Views;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace Abituria.Tests;

public sealed class OfficialCourseExampleContentTests
{
    private const string BasicHash = "88A0EA8E2EE444506CCA5E89C860178E33B04F181650A36D9C9B4DC9BBE625B2";
    private const string ExtendedHash = "BD408CDC8877E04EC79AAC3177FAB304E6F66C6B5FA152D8D3436D4ACFB2BC6F";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Official_catalog_adds_both_complete_CKE_example_sets_without_changing_author_counts()
    {
        var course = Read<MathCourseCatalog>("Content/chapters.json");
        var exercises = Read<CourseExerciseCatalog>("Content/course-exercises.json");
        var official = Read<OfficialCourseExampleCatalog>("Content/official-course-examples.json");

        OfficialCourseExampleCatalogValidator.Validate(official, course);
        Assert.Equal(1, official.SchemaVersion);
        Assert.Equal(97, official.Examples.Count);
        Assert.Equal(66, official.Examples.Count(item => item.Level == "basic"));
        Assert.Equal(31, official.Examples.Count(item => item.Level == "extended"));
        Assert.Equal(147, official.Examples.Where(item => item.Level == "basic").Sum(item => item.MaximumPoints));
        Assert.Equal(120, official.Examples.Where(item => item.Level == "extended").Sum(item => item.MaximumPoints));
        Assert.Equal(238, course.Lessons.SelectMany(item => item.WorkedExamples).Count());
        Assert.Equal(357, exercises.Exercises.Count);
        Assert.Equal(219, exercises.Exercises.Count(item => item.Level == "basic"));
        Assert.Equal(138, exercises.Exercises.Count(item => item.Level == "extended"));
    }

    [Fact]
    public void Sources_tasks_requirements_solutions_and_visual_descriptions_are_traceable()
    {
        var course = Read<MathCourseCatalog>("Content/chapters.json");
        var official = Read<OfficialCourseExampleCatalog>("Content/official-course-examples.json");
        var requirementIds = course.Requirements.Select(item => item.Id).ToHashSet(StringComparer.Ordinal);

        var basic = Assert.Single(official.Sources, item => item.Level == "basic");
        Assert.Equal("cke-basic-guide-2023", basic.Id);
        Assert.Equal(BasicHash, basic.DocumentSha256);
        Assert.Equal("https://bip.cke.gov.pl/attachments/download/10085", basic.DocumentUrl);
        Assert.Equal("2026-07-28", basic.VerifiedOn);
        Assert.Equal((12, 138, 66), (basic.FirstExamplePage, basic.LastExamplePage, basic.ExampleCount));

        var extended = Assert.Single(official.Sources, item => item.Level == "extended");
        Assert.Equal("cke-extended-guide-2023", extended.Id);
        Assert.Equal(ExtendedHash, extended.DocumentSha256);
        Assert.Equal("https://bip.cke.gov.pl/attachments/download/10088", extended.DocumentUrl);
        Assert.Equal("2026-07-28", extended.VerifiedOn);
        Assert.Equal((12, 106, 31), (extended.FirstExamplePage, extended.LastExamplePage, extended.ExampleCount));

        Assert.Equal(97, official.Examples.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.All(official.Examples, example =>
        {
            Assert.True(example.Id.Length < 80);
            Assert.StartsWith($"Zadanie {example.OfficialNumber}.", example.Transcription, StringComparison.Ordinal);
            Assert.Contains("Wymagani", example.Transcription, StringComparison.Ordinal);
            Assert.Contains("Zasady oceniania", example.Transcription, StringComparison.Ordinal);
            Assert.Contains("rozwiązani", example.Transcription, StringComparison.OrdinalIgnoreCase);
            Assert.All(example.RequirementIds, id => Assert.Contains(id, requirementIds));
            Assert.All(example.VisualReferences, visual =>
            {
                Assert.Contains(visual.SourcePage, example.SourcePages);
                Assert.False(string.IsNullOrWhiteSpace(visual.AlternativeText));
            });
        });
        Assert.Equal(53, official.Examples.SelectMany(item => item.VisualReferences).Count());
        Assert.Equal(90, official.Examples.SelectMany(item => item.RequirementIds).Distinct(StringComparer.Ordinal).Count());
        Assert.DoesNotContain(official.Examples, item => item.Level == "basic" && item.Order > 66);
        Assert.DoesNotContain(official.Examples, item => item.Level == "extended" && item.Order > 31);
    }

    [Fact]
    public void Official_examples_are_approved_by_the_august_rights_extension()
    {
        using var provenance = JsonDocument.Parse(File.ReadAllText(Path.Combine(RepositoryRoot, "Content/provenance.json")));
        var root = provenance.RootElement;
        var group = root.GetProperty("assets").EnumerateArray().Single(item =>
            item.GetProperty("id").GetString() == "cke-formula-2023-guide-examples");
        var rights = File.ReadAllText(Path.Combine(RepositoryRoot, "docs/ASSET_RIGHTS_DECLARATION.md"));

        Assert.True(root.GetProperty("releaseEligible").GetBoolean());
        Assert.Equal("approved", group.GetProperty("distributionStatus").GetString());
        Assert.Contains(BasicHash, group.GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.Contains(ExtendedHash, group.GetProperty("source").GetString(), StringComparison.Ordinal);
        Assert.Contains(BasicHash, rights, StringComparison.Ordinal);
        Assert.Contains(ExtendedHash, rights, StringComparison.Ordinal);
        Assert.Contains("Rozszerzenie deklaracji z 10 sierpnia 2026 r.", rights, StringComparison.Ordinal);
    }

    [Fact]
    public void Compound_official_examples_preserve_subtasks_pages_and_total_points()
    {
        var catalog = Read<OfficialCourseExampleCatalog>("Content/official-course-examples.json");
        var basic = catalog.Examples.Single(item => item.Id == "cke-basic-guide-task-27");
        Assert.Equal(4, basic.MaximumPoints);
        Assert.Equal([44, 45, 46], basic.SourcePages);
        Assert.Contains("Zadanie 27.1. (0–1)", basic.Transcription, StringComparison.Ordinal);
        Assert.Contains("Zadanie 27.3. (0–2)", basic.Transcription, StringComparison.Ordinal);

        var extended = catalog.Examples.Single(item => item.Id == "cke-extended-guide-task-18");
        Assert.Equal(8, extended.MaximumPoints);
        Assert.Equal([52, 53, 54, 55, 56, 57], extended.SourcePages);
        Assert.Equal(["III.E.5", "XIII.E.6"], extended.RequirementIds);
        Assert.Contains("Zadanie 18.1. (0–4)", extended.Transcription, StringComparison.Ordinal);
        Assert.Contains("Zadanie 18.2. (0–4)", extended.Transcription, StringComparison.Ordinal);
    }

    [AvaloniaFact]
    public void Lesson_view_separates_author_examples_from_collapsed_official_source_cards()
    {
        var repository = new ContentRepository();
        var lesson = repository.MathCourse.Lessons.Single(item => item.Id == "real-numbers");
        var expectedBasic = repository.OfficialCourseExamples.Examples.Count(example =>
            example.Level == "basic" && example.RequirementIds.Any(lesson.RequirementIds.Contains));
        var view = new CourseLessonView(
            repository.MathCourse,
            repository.CourseExercises,
            lesson,
            CourseLevelFilter.Basic,
            _ => { },
            () => { },
            new CourseLessonResources(
                repository.Diagrams,
                repository.OfficialCourseExamples));
        var window = new Window { Width = 960, Height = 640, Content = view };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            Assert.Contains(view.GetLogicalDescendants().OfType<TextBlock>(),
                text => text.Text == "Autorskie rozwiązane przykłady");
            Assert.Contains(view.GetLogicalDescendants().OfType<TextBlock>(),
                text => text.Text == "Oficjalne przykłady CKE - warstwa źródłowa");
            var cards = view.GetLogicalDescendants().OfType<Expander>().ToArray();
            Assert.Equal(expectedBasic, cards.Length);
            Assert.All(cards, card =>
            {
                Assert.False(card.IsExpanded);
                Assert.StartsWith("Oficjalny przykład CKE:", AutomationProperties.GetName(card), StringComparison.Ordinal);
            });
            Assert.Contains(view.GetLogicalDescendants().OfType<TextBlock>(), text =>
                text.Text?.Contains("Nie zastępują autorskich przykładów", StringComparison.Ordinal) == true);
            cards[0].IsExpanded = true;
            window.Width = 720;
            window.Height = 520;
            Dispatcher.UIThread.RunJobs();
            var scroll = Assert.Single(view.GetLogicalDescendants().OfType<ScrollViewer>());
            Assert.True(scroll.Extent.Width <= scroll.Viewport.Width + 1,
                $"Oficjalny przykład CKE przepełnia widok: {scroll.Extent.Width} > {scroll.Viewport.Width}.");

            var logarithms = repository.MathCourse.Lessons.Single(item => item.Id == "logarithms");
            var visualView = new CourseLessonView(
                repository.MathCourse,
                repository.CourseExercises,
                logarithms,
                CourseLevelFilter.Basic,
                _ => { },
                () => { },
                new CourseLessonResources(
                    repository.Diagrams,
                    repository.OfficialCourseExamples));
            window.Content = visualView;
            Dispatcher.UIThread.RunJobs();
            Assert.Contains(visualView.GetLogicalDescendants().OfType<TextBlock>(), text =>
                text.Text?.Contains("Wykres rosnącej funkcji logarytmicznej", StringComparison.Ordinal) == true);

            var extendedLesson = repository.MathCourse.Lessons.Single(item => item.Id == "equations-and-inequalities-extended");
            var expectedExtended = repository.OfficialCourseExamples.Examples.Count(example =>
                example.RequirementIds.Any(extendedLesson.RequirementIds.Contains));
            var extendedView = new CourseLessonView(
                repository.MathCourse,
                repository.CourseExercises,
                extendedLesson,
                CourseLevelFilter.Extended,
                _ => { },
                () => { },
                new CourseLessonResources(
                    repository.Diagrams,
                    repository.OfficialCourseExamples));
            window.Content = extendedView;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(expectedExtended, extendedView.GetLogicalDescendants().OfType<Expander>().Count());
            Assert.Contains(extendedView.GetLogicalDescendants().OfType<Expander>(), item =>
                AutomationProperties.GetName(item)?.Contains("poziom rozszerzony", StringComparison.Ordinal) == true);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Validator_rejects_detached_source_and_invalid_example_references()
    {
        var course = Read<MathCourseCatalog>("Content/chapters.json");
        var catalog = Read<OfficialCourseExampleCatalog>("Content/official-course-examples.json");

        catalog.Sources[0].DocumentSha256 = "BAD";
        Assert.Throws<InvalidOperationException>(() => OfficialCourseExampleCatalogValidator.Validate(catalog, course));

        catalog = Read<OfficialCourseExampleCatalog>("Content/official-course-examples.json");
        catalog.Examples[0].RequirementIds[0] = "missing";
        Assert.Throws<InvalidOperationException>(() => OfficialCourseExampleCatalogValidator.Validate(catalog, course));

        catalog = Read<OfficialCourseExampleCatalog>("Content/official-course-examples.json");
        catalog.Examples.First(item => item.VisualReferences.Count > 0).VisualReferences[0].AlternativeText = string.Empty;
        Assert.Throws<InvalidOperationException>(() => OfficialCourseExampleCatalogValidator.Validate(catalog, course));
    }

    private static T Read<T>(string relativePath)
    {
        var json = File.ReadAllText(Path.Combine(RepositoryRoot, relativePath));
        return JsonSerializer.Deserialize<T>(json, JsonOptions)
            ?? throw new InvalidOperationException($"Nie udało się odczytać {relativePath}.");
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Abituria.sln")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Nie znaleziono katalogu repozytorium.");
    }
}
