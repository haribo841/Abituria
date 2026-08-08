using System;
using System.Collections.Generic;
using System.Linq;
using Abituria.Models;
using Abituria.Services;
using Abituria.Ui;
using Abituria.ViewModels;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Abituria.Views;

internal static class CourseContentLevels
{
    public const string Basic = "basic";
}

public sealed class FormulaListView : UserControl
{
    public FormulaListView(FormulaCatalog catalog, Action<FormulaArticle> open)
    {
        var root = new StackPanel { Spacing = 14 };
        root.Children.Add(UiFactory.PageTitle("Tablice matematyczne", "18 działów zgodnych zakresem z tablicami CKE dla Formuły 2023."));
        if (catalog.Introduction.Count > 0)
            root.Children.Add(UiFactory.Card(new RichContentView(catalog.Introduction)));
        foreach (var article in catalog.Articles.OrderBy(item => item.Order))
        {
            var button = new Button
            {
                Content = $"{article.Order}. {article.Title}",
                Classes = { "list" },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
            button.Click += (_, _) => open(article);
            root.Children.Add(button);
        }
        Content = UiFactory.PageScroll(root);
    }
}

public sealed class ArticleView : UserControl
{
    public ArticleView(
        string title,
        string subtitle,
        IReadOnlyList<ContentBlock> blocks,
        Action back,
        DiagramCatalog? diagrams = null)
    {
        var root = new StackPanel { Spacing = 18 };
        var backButton = new Button { Content = "← Wróć", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
        backButton.Click += (_, _) => back();
        root.Children.Add(backButton);
        root.Children.Add(UiFactory.PageTitle(title, subtitle));
        root.Children.Add(UiFactory.Card(new RichContentView(blocks, diagrams), new Thickness(22)));
        Content = UiFactory.PageScroll(root);
    }
}

public sealed class ChapterListView : UserControl
{
    public ChapterListView(
        MathCourseCatalog catalog,
        CourseLevelFilter level,
        Action<CourseLevelFilter> selectLevel,
        Action<CourseArea> openArea)
    {
        var root = new StackPanel { Spacing = 14 };
        root.Children.Add(UiFactory.PageTitle(
            "Działy",
            "Pełny kurs matematyki dla Formuły 2023 według podstawy programowej stosowanej na maturze 2026."));
        root.Children.Add(BuildLevelFilter(level, selectLevel));
        if (catalog.Introduction.Count > 0)
            root.Children.Add(UiFactory.Card(new RichContentView(catalog.Introduction)));

        foreach (var group in catalog.Groups.OrderBy(item => item.Order))
        {
            root.Children.Add(new TextBlock
            {
                Text = group.Title,
                Classes = { "h2" },
                Margin = new Thickness(0, 8, 0, 0)
            });

            foreach (var areaId in group.AreaIds)
            {
                var area = catalog.Areas.Single(item => item.Id == areaId);
                var requirements = MathCourseNavigation.GetVisibleRequirements(catalog, area.Id, level);
                var button = new Button
                {
                    Content = $"{area.OfficialNumber}. {area.Title} - {requirements.Length} wymagań",
                    Classes = { "list" },
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left
                };
                AutomationProperties.SetName(button, $"Otwórz obszar {area.OfficialNumber}: {area.Title}");
                button.Click += (_, _) => openArea(area);
                root.Children.Add(button);
            }
        }

        Content = UiFactory.PageScroll(root);
    }

    internal static StackPanel BuildLevelFilter(
        CourseLevelFilter selectedLevel,
        Action<CourseLevelFilter> selectLevel)
    {
        var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        panel.Children.Add(LevelButton("Podstawowy", CourseLevelFilter.Basic, selectedLevel, selectLevel));
        panel.Children.Add(LevelButton("Rozszerzony", CourseLevelFilter.Extended, selectedLevel, selectLevel));
        return panel;
    }

    private static Button LevelButton(
        string label,
        CourseLevelFilter value,
        CourseLevelFilter selectedLevel,
        Action<CourseLevelFilter> selectLevel)
    {
        var button = new Button
        {
            Content = label,
            Classes = { value == selectedLevel ? "primary" : "ghost" },
            MinWidth = 132
        };
        AutomationProperties.SetName(button, $"Poziom kursu: {label}");
        button.Click += (_, _) => selectLevel(value);
        return button;
    }
}

public sealed class CourseAreaView : UserControl
{
    public CourseAreaView(
        MathCourseCatalog catalog,
        CourseArea area,
        CourseLevelFilter level,
        Action<CourseLevelFilter> selectLevel,
        Action<MathCourseLesson> openLesson,
        Action back)
    {
        var root = new StackPanel { Spacing = 14 };
        root.Children.Add(BackButton("← Wszystkie obszary", back));
        root.Children.Add(UiFactory.PageTitle(
            $"{area.OfficialNumber}. {area.Title}",
            level == CourseLevelFilter.Basic
                ? "Poziom podstawowy oraz materiały pomocnicze."
                : "Poziom podstawowy, rozszerzony oraz materiały pomocnicze."));
        root.Children.Add(ChapterListView.BuildLevelFilter(level, selectLevel));

        foreach (var lesson in MathCourseNavigation.GetVisibleLessons(catalog, area.Id, level))
        {
            var requirementCount = lesson.RequirementIds.Count(id =>
                catalog.Requirements.Any(requirement => requirement.Id == id &&
                    (level == CourseLevelFilter.Extended || requirement.Level == CourseContentLevels.Basic)));
            var suffix = lesson.AlwaysVisible
                ? " - materiał pomocniczy"
                : $" - {requirementCount} wymagań";
            var button = new Button
            {
                Content = lesson.Title + suffix,
                Classes = { "list" },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
            AutomationProperties.SetName(button, $"Otwórz lekcję: {lesson.Title}");
            button.Click += (_, _) => openLesson(lesson);
            root.Children.Add(button);
        }

        Content = UiFactory.PageScroll(root);
    }

    private static Button BackButton(string label, Action back)
    {
        var button = new Button { Content = label, Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
        button.Click += (_, _) => back();
        return button;
    }
}

public sealed record CourseLessonResources(
    DiagramCatalog? Diagrams = null,
    OfficialCourseExampleCatalog? OfficialExamples = null);

public sealed class CourseLessonView : UserControl
{
    public CourseLessonView(
        MathCourseCatalog catalog,
        CourseExerciseCatalog exerciseCatalog,
        MathCourseLesson lesson,
        CourseLevelFilter level,
        Action<LearningExercise> openExercise,
        Action back,
        CourseLessonResources? resources = null)
    {
        var root = new StackPanel { Spacing = 16 };
        var backButton = new Button { Content = "← Lekcje", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
        backButton.Click += (_, _) => back();
        root.Children.Add(backButton);
        root.Children.Add(UiFactory.PageTitle(lesson.Title, LevelLabel(lesson)));

        if (lesson.Blocks.Count > 0)
            root.Children.Add(UiFactory.Card(new RichContentView(lesson.Blocks, resources?.Diagrams)));

        var visibleRequirementIds = VisibleRequirementIds(catalog, lesson, level);
        AddRequirements(root, catalog, visibleRequirementIds);
        AddWorkedExamples(root, lesson, visibleRequirementIds);
        AddOfficialExamples(root, resources?.OfficialExamples, visibleRequirementIds, level);
        AddExercises(root, exerciseCatalog, lesson, visibleRequirementIds, openExercise);
        Content = UiFactory.PageScroll(root);
    }

    private static HashSet<string> VisibleRequirementIds(
        MathCourseCatalog catalog,
        MathCourseLesson lesson,
        CourseLevelFilter level) => lesson.RequirementIds
        .Where(id => catalog.Requirements.Any(requirement => requirement.Id == id &&
            (level == CourseLevelFilter.Extended || requirement.Level == CourseContentLevels.Basic)))
        .ToHashSet(StringComparer.Ordinal);

    private static void AddRequirements(
        StackPanel root,
        MathCourseCatalog catalog,
        HashSet<string> requirementIds)
    {
        if (requirementIds.Count == 0)
            return;

        root.Children.Add(new TextBlock { Text = "Wymagania", Classes = { "h2" } });
        foreach (var requirement in catalog.Requirements.Where(item => requirementIds.Contains(item.Id)))
        {
            var level = requirement.Level == CourseContentLevels.Basic ? "podstawowy" : "rozszerzony";
            var panel = new StackPanel { Spacing = 6 };
            panel.Children.Add(new TextBlock
            {
                Text = $"{requirement.Id} - poziom {level}",
                FontWeight = FontWeight.SemiBold,
                TextWrapping = TextWrapping.Wrap
            });
            panel.Children.Add(RichContentView.CreateText(requirement.Text));
            root.Children.Add(UiFactory.Card(panel, new Thickness(16)));
        }
    }

    private static void AddWorkedExamples(
        StackPanel root,
        MathCourseLesson lesson,
        HashSet<string> requirementIds)
    {
        var examples = lesson.WorkedExamples
            .Where(example => requirementIds.Contains(example.RequirementId))
            .ToArray();
        if (examples.Length == 0)
            return;

        root.Children.Add(new TextBlock { Text = "Autorskie rozwiązane przykłady", Classes = { "h2" } });
        foreach (var example in examples)
        {
            var panel = new StackPanel { Spacing = 8 };
            panel.Children.Add(new TextBlock
            {
                Text = $"{example.Title} ({example.RequirementId})",
                FontWeight = FontWeight.SemiBold,
                TextWrapping = TextWrapping.Wrap
            });
            panel.Children.Add(RichContentView.CreateText(example.Prompt));
            panel.Children.Add(RichContentView.CreateText($"Rozwiązanie: {example.Solution}"));
            root.Children.Add(UiFactory.Card(panel, new Thickness(16)));
        }
    }

    private static void AddOfficialExamples(
        StackPanel root,
        OfficialCourseExampleCatalog? catalog,
        HashSet<string> requirementIds,
        CourseLevelFilter level)
    {
        if (catalog is null)
            return;

        var examples = catalog.Examples
            .Where(example => level == CourseLevelFilter.Extended || example.Level == CourseContentLevels.Basic)
            .Where(example => example.RequirementIds.Any(requirementIds.Contains))
            .OrderBy(example => example.Level == CourseContentLevels.Basic ? 0 : 1)
            .ThenBy(example => example.Order)
            .ToArray();
        if (examples.Length == 0)
            return;

        root.Children.Add(new TextBlock
        {
            Text = "Oficjalne przykłady CKE - warstwa źródłowa",
            Classes = { "h2" }
        });
        root.Children.Add(UiFactory.InfoBand(
            "Materiały dodatkowe",
            "Poniższe transkrypcje pochodzą z informatorów CKE. Nie zastępują autorskich przykładów i ćwiczeń kursu."));

        foreach (var example in examples)
        {
            var source = catalog.Sources.Single(item => item.Id == example.SourceId);
            var levelLabel = example.Level == CourseContentLevels.Basic ? "poziom podstawowy" : "poziom rozszerzony";
            var requirementLabel = string.Join(", ", example.RequirementIds.Where(requirementIds.Contains));
            var panel = new StackPanel { Spacing = 8 };
            panel.Children.Add(RichContentView.CreateText(
                $"Źródło: {source.Publisher}, {source.Title}\n" +
                $"Strony PDF: {FormatPages(example.SourcePages)}\n" +
                $"Wymagania kursu: {requirementLabel}\n" +
                $"Dokument: {source.DocumentUrl}#page={example.SourcePages[0]}"));
            foreach (var visual in example.VisualReferences)
            {
                panel.Children.Add(UiFactory.InfoBand(
                    $"Opis figury - s. {visual.SourcePage}",
                    visual.AlternativeText));
            }
            panel.Children.Add(RichContentView.CreateText(example.Transcription));

            var header = $"CKE - zadanie {example.OfficialNumber}, {levelLabel}, " +
                $"0-{example.MaximumPoints} pkt";
            var expander = new Expander
            {
                Header = header,
                Content = panel,
                IsExpanded = false,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            AutomationProperties.SetName(expander, $"Oficjalny przykład CKE: zadanie {example.OfficialNumber}, {levelLabel}");
            root.Children.Add(UiFactory.Card(expander, new Thickness(16)));
        }
    }

    private static string FormatPages(List<int> pages) =>
        pages.Count == 1 ? pages[0].ToString(System.Globalization.CultureInfo.InvariantCulture) :
        $"{pages[0]}-{pages[^1]}";

    private static void AddExercises(
        StackPanel root,
        CourseExerciseCatalog exerciseCatalog,
        MathCourseLesson lesson,
        HashSet<string> requirementIds,
        Action<LearningExercise> openExercise)
    {
        var exercises = exerciseCatalog.Exercises
            .Where(exercise => lesson.ExerciseIds.Contains(exercise.Id, StringComparer.Ordinal))
            .Where(exercise => exercise.RequirementId is not null && requirementIds.Contains(exercise.RequirementId))
            .OrderBy(exercise => exercise.Number)
            .ToArray();
        if (exercises.Length == 0)
            return;

        root.Children.Add(new TextBlock { Text = "Ćwiczenia", Classes = { "h2" } });
        foreach (var exercise in exercises)
        {
            var button = new Button
            {
                Content = exercise.Title,
                Classes = { "list" },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
            button.Click += (_, _) => openExercise(exercise);
            root.Children.Add(button);
        }
    }

    private static string LevelLabel(MathCourseLesson lesson)
    {
        if (lesson.AlwaysVisible)
            return "Materiał pomocniczy widoczny na obu poziomach.";

        if (lesson.Level == CourseContentLevels.Basic)
            return "Lekcja poziomu podstawowego.";

        return "Lekcja poziomu rozszerzonego.";
    }
}

public sealed class PlaceholderView : UserControl
{
    public PlaceholderView(string title, string message, IReadOnlyList<ContentBlock> blocks, Action back, Action? openRoadmap = null)
    {
        var root = new StackPanel { Spacing = 18 };
        var backButton = new Button { Content = "← Wróć", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
        backButton.Click += (_, _) => back();
        root.Children.Add(backButton);
        root.Children.Add(UiFactory.PageTitle(title, "Zachowana pozycja ze starszej wersji aplikacji."));
        root.Children.Add(UiFactory.InfoBand("Status", message));
        if (blocks.Count > 0) root.Children.Add(UiFactory.Card(new RichContentView(blocks)));
        if (openRoadmap is not null)
        {
            var roadmap = new Button { Content = "Zobacz w planie rozwoju", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
            roadmap.Click += (_, _) => openRoadmap();
            root.Children.Add(roadmap);
        }
        Content = UiFactory.PageScroll(root);
    }
}

public sealed class RoadmapView : UserControl
{
    public RoadmapView(RoadmapCatalog catalog, string? focusedItemId)
    {
        var root = new StackPanel { Spacing = 18 };
        root.Children.Add(UiFactory.PageTitle("Plan rozwoju", "Stan migracji funkcji i treści ze wszystkich wersji projektu."));
        if (catalog.Introduction.Count > 0)
            root.Children.Add(UiFactory.Card(new RichContentView(catalog.Introduction)));

        AddGroup(root, catalog, RoadmapStatus.Migrated, "Przeniesione", "SuccessBrush", focusedItemId);
        AddGroup(root, catalog, RoadmapStatus.Planned, "Zaplanowane", "WarningBrush", focusedItemId);
        AddGroup(root, catalog, RoadmapStatus.Superseded, "Zastąpione", "TextMutedBrush", focusedItemId);
        Content = UiFactory.PageScroll(root);
    }

    private static void AddGroup(StackPanel root, RoadmapCatalog catalog, RoadmapStatus status, string title, string colorResource, string? focusedItemId)
    {
        root.Children.Add(new TextBlock { Text = title, Classes = { "h2" }, Margin = new Thickness(0, 8, 0, 0) });
        var items = catalog.Items.Where(item => item.Status == status)
            .OrderByDescending(item => string.Equals(item.Id, focusedItemId, StringComparison.Ordinal))
            .ThenBy(item => item.Title);
        foreach (var item in items)
        {
            var panel = new StackPanel { Spacing = 5 };
            panel.Children.Add(new TextBlock { Text = item.Title, FontSize = 18, FontWeight = Avalonia.Media.FontWeight.SemiBold, TextWrapping = TextWrapping.Wrap });
            panel.Children.Add(new TextBlock { Text = item.Summary, Classes = { "muted" } });
            var source = new TextBlock
            {
                Text = $"Obszar: {item.Context} · Źródła: {string.Join(", ", item.SourceRefs)}",
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap
            };
            UiFactory.UseResource(source, TextBlock.ForegroundProperty, "TextMutedBrush");
            panel.Children.Add(source);
            var focused = string.Equals(item.Id, focusedItemId, StringComparison.Ordinal);
            var card = UiFactory.Card(panel, new Thickness(16), focused ? "WarningSurfaceBrush" : "SurfaceBrush");
            UiFactory.UseResource(card, Border.BorderBrushProperty, focused ? colorResource : "BorderBrush");
            card.BorderThickness = new Thickness(focused ? 2 : 1);
            root.Children.Add(card);
        }
    }
}
