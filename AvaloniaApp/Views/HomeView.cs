using System;
using System.Globalization;
using System.Linq;
using Abituria.Services;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Abituria.Ui;

namespace Abituria.Views;

public sealed record HomeNavigationActions(
    Action ShowFormulas,
    Action ShowMatura,
    Action ShowTasks,
    Action ShowCalculator,
    Action ShowChapters,
    Action ShowRoadmap);

public sealed class HomeView : UserControl
{
    public HomeView(
        string profileName,
        ContentRepository content,
        HomeNavigationActions actions)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(actions);
        var root = new StackPanel { Spacing = 20 };
        root.Children.Add(UiFactory.PageTitle("Start", $"Cześć, {profileName}. Wybierz obszar nauki."));

        var grid = new Grid
        {
            Name = "HomeLayoutRoot",
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto"),
            ColumnSpacing = 16,
            RowSpacing = 16
        };
        var examCount = content.Exams.Count;
        var examExerciseCount = content.Exams.Sum(exam => exam.Exercises.Count);
        var firstExamYear = content.Exams.Min(exam => exam.Year);
        var lastExamYear = content.Exams.Max(exam => exam.Year);
        var requirementCount = content.MathCourse.Requirements.Count;
        var workedExampleCount = content.MathCourse.Lessons.Sum(lesson => lesson.WorkedExamples.Count);
        var officialExampleCount = content.OfficialCourseExamples.Examples.Count;
        var courseExerciseCount = content.CourseExercises.Exercises.Count;
        var copy = content.UiCopy;
        var tiles = new[]
        {
            AddTile(grid, new HomeTile(
                "Tablice matematyczne",
                copy.FormatRequired("home.formulas.description", content.Formulas.Articles.Count).Body,
                "📐",
                actions.ShowFormulas)),
            AddTile(grid, new HomeTile(
                "Matura",
                copy.FormatRequired("home.exams.description", examCount, firstExamYear, lastExamYear).Body,
                "🎓",
                actions.ShowMatura)),
            AddTile(grid, new HomeTile(
                "Zadania",
                copy.FormatRequired(
                    "home.tasks.description",
                    FormatCount(examExerciseCount),
                    examCount,
                    content.ExamTopics.Count).Body,
                "📝",
                actions.ShowTasks)),
            AddTile(grid, new HomeTile(
                "Kalkulatory",
                copy.GetRequired("home.calculators.description").Body,
                "🧮",
                actions.ShowCalculator)),
            AddTile(grid, new HomeTile(
                "Działy",
                copy.FormatRequired(
                    "home.course.description",
                    requirementCount,
                    workedExampleCount,
                    officialExampleCount,
                    courseExerciseCount).Body,
                "📚",
                actions.ShowChapters)),
            AddTile(grid, new HomeTile(
                "Plan rozwoju",
                copy.GetRequired("home.roadmap.description").Body,
                "🗺️",
                actions.ShowRoadmap))
        };

        AdaptiveLayout.ObserveWidth(this, 780, isCompact => ApplyTileLayout(grid, tiles, isCompact));

        root.Children.Add(grid);
        root.Children.Add(UiFactory.InfoBand(content.UiCopy.GetRequired("home.work-mode")));
        Content = UiFactory.PageScroll(root);
    }

    private static string FormatCount(int value) =>
        value.ToString("N0", CultureInfo.GetCultureInfo("pl-PL"))
            .Replace('\u00A0', ' ')
            .Replace('\u202F', ' ');

    private static Button AddTile(Grid grid, HomeTile tile)
    {
        var content = new StackPanel { Spacing = 10 };
        content.Children.Add(UiFactory.Glyph(tile.Glyph, 44, $"Ikona sekcji {tile.Title}"));
        content.Children.Add(new TextBlock { Text = tile.Title, Classes = { "h2" } });
        content.Children.Add(new TextBlock { Text = tile.Description, Classes = { "muted" }, TextWrapping = TextWrapping.Wrap });
        var button = new Button
        {
            Content = UiFactory.Card(content),
            Classes = { "home-tile" },
            Padding = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch
        };
        AutomationProperties.SetName(button, tile.Title);
        AutomationProperties.SetHelpText(button, tile.Description);
        button.Click += (_, _) => tile.Action();
        grid.Children.Add(button);
        return button;
    }

    private static void ApplyTileLayout(Grid grid, Button[] tiles, bool isCompact)
    {
        if (isCompact)
        {
            grid.ColumnDefinitions = new ColumnDefinitions("*");
            grid.RowDefinitions = new RowDefinitions("Auto,Auto,Auto,Auto,Auto,Auto");
            grid.ColumnSpacing = 0;
            for (var index = 0; index < tiles.Length; index++)
                Position(tiles[index], 0, index);
            return;
        }

        grid.ColumnDefinitions = new ColumnDefinitions("*,*");
        grid.RowDefinitions = new RowDefinitions("Auto,Auto,Auto");
        grid.ColumnSpacing = 16;
        Position(tiles[0], 0, 0);
        Position(tiles[1], 1, 0);
        Position(tiles[2], 0, 1);
        Position(tiles[3], 1, 1);
        Position(tiles[4], 0, 2);
        Position(tiles[5], 1, 2);
    }

    private static void Position(Control control, int column, int row, int columnSpan = 1)
    {
        Grid.SetColumn(control, column);
        Grid.SetRow(control, row);
        Grid.SetColumnSpan(control, columnSpan);
    }

    private sealed record HomeTile(string Title, string Description, string Glyph, Action Action);
}
