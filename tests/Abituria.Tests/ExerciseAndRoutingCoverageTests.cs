using Abituria.Data;
using Abituria.Models;
using Abituria.Services;
using Abituria.ViewModels;
using Abituria.Views;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Microsoft.Data.Sqlite;

namespace Abituria.Tests;

public sealed class ExerciseAndRoutingCoverageTests
{
    [AvaloniaFact]
    public async Task Exercise_view_navigation_hints_and_answers_are_executable()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var accounts = new AccountService(new AppDbContextFactory(Path.Combine(directory, "exercise-view.db")), new PasswordHasher(1_000));
        await accounts.InitializeAsync();
        var profile = (await accounts.GetProfilesAsync()).Single(item => item.Kind == ProfileKind.Guest);
        var copy = new ContentRepository().UiCopy;
        var source = new SourceDocument { VerifiedOn = "2026-07-07" };
        var exercises = CreateExercises();
        var opened = new List<string>();
        var backCalls = 0;
        var context = new ExerciseViewContext(
            exercises,
            source,
            copy,
            profile,
            accounts,
            () => backCalls++,
            exercise => opened.Add(exercise.Id));
        var view = new ExerciseView(exercises[1], context);
        var window = new Window { Width = 960, Height = 640, Content = view };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            ClickButtonContaining(view, "Lista");
            ClickNavigationButton(view, exercises[0].Title);
            ClickNavigationButton(view, exercises[2].Title);
            Assert.Equal(1, backCalls);
            Assert.Equal([exercises[0].Id, exercises[2].Id], opened);

            ClickButtonContaining(view, "Nast");
            ClickButtonContaining(view, "Nast");
            Assert.Contains(
                view.GetLogicalDescendants().OfType<TextBlock>(),
                control => control.Text == "To była ostatnia podpowiedź.");

            var radio = view.GetLogicalDescendants().OfType<RadioButton>().ElementAt(1);
            radio.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            ClickButtonContaining(view, "Sprawd");
            await WaitUntilAsync(async () => (await accounts.GetCompletedExerciseIdsAsync(profile.Id)).Contains(exercises[1].Id));
            Assert.Contains(
                view.GetLogicalDescendants().OfType<TextBlock>(),
                control => control.Text == "Poprawna odpowiedź. Zadanie zapisano jako ukończone.");

            var openView = new ExerciseView(exercises[3], context);
            window.Content = openView;
            Dispatcher.UIThread.RunJobs();
            ClickButtonContaining(openView, "Poka");
            await WaitUntilAsync(async () => (await accounts.GetCompletedExerciseIdsAsync(profile.Id)).Contains(exercises[3].Id));
            Assert.Contains(
                openView.GetLogicalDescendants().OfType<TextBlock>(),
                control => control.Text == "Odpowiedź została ujawniona. Zadanie zapisano jako ukończone.");
        }
        finally
        {
            window.Close();
            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [AvaloniaFact]
    public async Task Main_window_routes_calculator_and_exercise_pages_through_real_shell()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var accounts = new AccountService(new AppDbContextFactory(Path.Combine(directory, "routing.db")), new PasswordHasher(1_000));
        await accounts.InitializeAsync();
        var profile = (await accounts.GetProfilesAsync()).Single(item => item.Kind == ProfileKind.Guest);
        var viewModel = new AppViewModel();
        var content = new ContentRepository();
        var calculatorSession = new CalculatorSession(new ExpressionCalculator());
        var window = new MainWindow(viewModel, accounts, content, calculatorSession);

        try
        {
            window.Show();
            viewModel.Login(profile);
            Dispatcher.UIThread.RunJobs();

            viewModel.Navigate(AppPage.Calculator);
            Dispatcher.UIThread.RunJobs();
            var calculator = PageControl<CalculatorView>(window);
            ClickButtonContaining(calculator, "Oblicz");
            Assert.Contains(
                calculator.GetLogicalDescendants().OfType<TextBlock>(),
                control => control.Text is not null && control.Text.Contains("miejsca zerowe", StringComparison.OrdinalIgnoreCase));

            ClickButtonContaining(calculator, "og");
            Dispatcher.UIThread.RunJobs();
            var generalCalculator = PageControl<GeneralCalculatorView>(window);
            ClickButtonWithExactText(generalCalculator, "1");
            Assert.Equal(AppPage.GeneralCalculator, viewModel.CurrentPage);

            viewModel.OpenExercise(content.Exam.Exercises[1]);
            Dispatcher.UIThread.RunJobs();
            var exerciseView = PageControl<ExerciseView>(window);
            ClickButtonContaining(exerciseView, "Lista");
            Assert.Equal(AppPage.ExerciseList, viewModel.CurrentPage);
        }
        finally
        {
            window.Close();
            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    private static IReadOnlyList<ExerciseDefinition> CreateExercises() =>
    [
        new()
        {
            Id = "previous",
            Number = 1,
            Title = "Poprzednie zadanie",
            Mode = "multipleChoice",
            Prompt = "Treść poprzedniego zadania.",
            Options = ["A", "B"],
            CorrectOption = 1,
            VerificationSource = "test",
            SourcePage = 1
        },
        new()
        {
            Id = "multiple-choice",
            Number = 2,
            Title = "Zadanie wyboru",
            Mode = "multipleChoice",
            Prompt = "Wybierz poprawną odpowiedź.",
            Options = ["błąd", "poprawnie"],
            CorrectOption = 2,
            Hints = ["Pierwsza wskazówka testowa."],
            VerificationSource = "test",
            SourcePage = 2
        },
        new()
        {
            Id = "next",
            Number = 3,
            Title = "Następne zadanie",
            Mode = "multipleChoice",
            Prompt = "Treść następnego zadania.",
            Options = ["A", "B"],
            CorrectOption = 1,
            VerificationSource = "test",
            SourcePage = 3
        },
        new()
        {
            Id = "open-answer",
            Number = 4,
            Title = "Zadanie otwarte",
            Mode = "open",
            Prompt = "Rozwiąż zadanie otwarte.",
            RevealedAnswer = "Odpowiedź testowa.",
            VerificationSource = "test",
            SourcePage = 4
        }
    ];

    private static T PageControl<T>(MainWindow window)
        where T : Control =>
        Assert.Single(window.GetLogicalDescendants().OfType<T>());

    private static void ClickNavigationButton(Control root, string targetTitle)
    {
        var button = root.GetLogicalDescendants()
            .OfType<Button>()
            .Single(control => Equals(ToolTip.GetTip(control), targetTitle));
        Click(button);
    }

    private static void ClickButtonWithExactText(Control root, string text)
    {
        var button = root.GetLogicalDescendants()
            .OfType<Button>()
            .Single(control => string.Equals(control.Content as string, text, StringComparison.Ordinal));
        Click(button);
    }

    private static void ClickButtonContaining(Control root, string text)
    {
        var button = root.GetLogicalDescendants()
            .OfType<Button>()
            .First(control => control.Content is string content && content.Contains(text, StringComparison.OrdinalIgnoreCase));
        Click(button);
    }

    private static void Click(Button button)
    {
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Dispatcher.UIThread.RunJobs();
    }

    private static async Task WaitUntilAsync(Func<Task<bool>> condition)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            Dispatcher.UIThread.RunJobs();
            if (await condition()) return;
            await Task.Delay(10);
        }

        Assert.Fail("Warunek interfejsu nie został spełniony w wyznaczonym czasie.");
    }
}
