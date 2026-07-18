using Abituria.Models;
using Abituria.Services;
using Abituria.Views;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace Abituria.Tests;

public sealed class ExerciseRandomizerTests
{
    [Fact]
    public void Select_returns_null_for_an_empty_pool()
    {
        var randomizer = new ExerciseRandomizer(new FixedRandom(0));

        Assert.Null(randomizer.Select([]));
    }

    [Fact]
    public void Select_returns_the_item_at_the_random_index_without_modifying_the_pool()
    {
        var first = CreateExercise("first", "algebra");
        var second = CreateExercise("second", "geometry");
        IReadOnlyList<ExerciseDefinition> exercises = [first, second];
        var randomizer = new ExerciseRandomizer(new FixedRandom(1));

        var selected = randomizer.Select(exercises);

        Assert.Same(second, selected);
        Assert.Equal([first, second], exercises);
    }

    [AvaloniaFact]
    public void Overview_opens_a_random_exercise_from_the_current_pool()
    {
        var algebra = CreateExercise("algebra", "algebra");
        var geometry = CreateExercise("geometry", "geometry");
        var exam = new ExamDefinition
        {
            Exercises = [algebra, geometry],
            Topics =
            [
                new ExerciseTopicDefinition { Id = "algebra", Title = "Algebra", ExerciseNumbers = [1] },
                new ExerciseTopicDefinition { Id = "geometry", Title = "Geometria", ExerciseNumbers = [2] }
            ]
        };
        ExerciseDefinition? opened = null;
        string? selectedTopicId = "previous-topic";
        var view = new ExamOverviewView(
            exam,
            [],
            () => { },
            _ => { },
            _ => { },
            (exercise, topicId) =>
            {
                opened = exercise;
                selectedTopicId = topicId;
            },
            new ExerciseRandomizer(new FixedRandom(1)));
        var window = new Window { Width = 960, Height = 640, Content = view };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            ClickButton(view, "Losuj zadanie z tego arkusza");

            Assert.Same(geometry, opened);
            Assert.Null(selectedTopicId);

            ClickButton(view, "Tematy");
            ClickButton(view, "Losuj zadanie z tematu: Algebra");

            Assert.Same(algebra, opened);
            Assert.Equal("algebra", selectedTopicId);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Overview_disables_randomization_for_an_empty_pool()
    {
        var view = new ExamOverviewView(
            new ExamDefinition(),
            [],
            () => { },
            _ => { },
            _ => { },
            (_, _) => { });

        var button = view.GetLogicalDescendants()
            .OfType<Button>()
            .Single(control => string.Equals(control.Content as string, "Losuj zadanie z tego arkusza", StringComparison.Ordinal));

        Assert.False(button.IsEnabled);
    }

    private static ExerciseDefinition CreateExercise(string id, string topicId) => new()
    {
        Id = id,
        TopicId = topicId,
        Title = id,
        Mode = "multipleChoice"
    };

    private static void ClickButton(Control root, string text)
    {
        var button = root.GetLogicalDescendants()
            .OfType<Button>()
            .Single(control => string.Equals(control.Content as string, text, StringComparison.Ordinal));
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Dispatcher.UIThread.RunJobs();
    }

    private sealed class FixedRandom(int value) : Random
    {
        public override int Next(int maxValue) => value % maxValue;
    }
}
