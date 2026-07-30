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
        IReadOnlyList<LearningExercise> exercises = [first, second];
        var randomizer = new ExerciseRandomizer(new FixedRandom(1));

        var selected = randomizer.Select(exercises);

        Assert.Same(second, selected);
        Assert.Equal([first, second], exercises);
    }

    [AvaloniaFact]
    public void Matura_and_tasks_open_random_exercises_from_their_current_pools()
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
        LearningExercise? opened = null;
        string? selectedTopicId = "previous-topic";
        var maturaView = new MaturaView(
            exam,
            [],
            () => { },
            _ => { },
            (exercise, topicId) =>
            {
                opened = exercise;
                selectedTopicId = topicId;
            },
            new ExerciseRandomizer(new FixedRandom(1)));
        var window = new Window { Width = 960, Height = 640, Content = maturaView };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            ClickButton(maturaView, "Losuj zadanie z tego arkusza");

            Assert.Same(geometry, opened);
            Assert.Null(selectedTopicId);

            var tasksView = new TaskTopicsView(
                exam,
                [],
                _ => { },
                _ => { },
                (exercise, topicId) =>
                {
                    opened = exercise;
                    selectedTopicId = topicId;
                },
                new ExerciseRandomizer(new FixedRandom(0)));
            window.Content = tasksView;
            Dispatcher.UIThread.RunJobs();
            ClickButton(tasksView, "Losuj zadanie z tematu: Algebra");

            Assert.Same(algebra, opened);
            Assert.Equal("algebra", selectedTopicId);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Matura_disables_randomization_for_an_empty_pool()
    {
        var view = new MaturaView(
            new ExamDefinition(),
            [],
            () => { },
            _ => { },
            (_, _) => { });

        var button = view.GetLogicalDescendants()
            .OfType<Button>()
            .Single(control => string.Equals(control.Content as string, "Losuj zadanie z tego arkusza", StringComparison.Ordinal));

        Assert.False(button.IsEnabled);
    }

    private static LearningExercise CreateExercise(string id, string topicId) => new()
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
