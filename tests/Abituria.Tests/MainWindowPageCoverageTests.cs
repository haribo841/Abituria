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

public sealed class MainWindowPageCoverageTests
{
    [AvaloniaFact]
    public async Task Authenticated_shell_builds_every_page_and_executes_contextual_navigation()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var accounts = new AccountService(
            new AppDbContextFactory(Path.Combine(directory, "all-pages.db")),
            new PasswordHasher(1_000));
        await accounts.InitializeAsync();
        var profile = (await accounts.GetProfilesAsync()).Single(item => item.Kind == ProfileKind.Guest);
        var viewModel = new AppViewModel();
        var content = new ContentRepository();
        var window = new MainWindow(
            viewModel,
            accounts,
            content,
            new CalculatorSession(new ExpressionCalculator()));

        try
        {
            window.Show();
            viewModel.Login(profile);
            Render();
            AssertPage<HomeView>(window);

            Click(window, "Wzory");
            AssertPage<FormulaListView>(window);

            viewModel.OpenFormula(content.Formulas.Articles[0]);
            Render();
            AssertPage<ArticleView>(window);

            Click(window, "Zadania");
            AssertPage<TaskTopicsView>(window);

            Click(window, "Matura");
            AssertPage<MaturaView>(window);

            viewModel.OpenExam();
            Render();
            AssertPage<ExerciseListView>(window);

            viewModel.Navigate(AppPage.Tasks);
            viewModel.OpenTopic(content.Exam.Topics[0].Id);
            Render();
            AssertPage<ExerciseListView>(window);

            viewModel.OpenExercise(content.Exam.Exercises[0]);
            Render();
            AssertPage<ExerciseView>(window);

            Click(window, "Działy");
            AssertPage<ChapterListView>(window);

            var area = content.MathCourse.Areas.First();
            viewModel.OpenCourseArea(area);
            Render();
            AssertPage<CourseAreaView>(window);

            var lesson = content.MathCourse.Lessons.First(item => item.AreaId == area.Id && item.RequirementIds.Count > 0);
            viewModel.OpenCourseLesson(lesson);
            Render();
            AssertPage<CourseLessonView>(window);

            var courseExercise = content.CourseExercises.Exercises.First(exercise => lesson.ExerciseIds.Contains(exercise.Id));
            viewModel.OpenCourseExercise(courseExercise);
            Render();
            AssertPage<ExerciseView>(window);
            Click(window, "← Działy");
            AssertPage<ChapterListView>(window);

            viewModel.Navigate(AppPage.Tasks);
            viewModel.OpenTopic(content.Exam.Topics[0].Id);
            viewModel.OpenExercise(content.Exam.Exercises.First(item => item.TopicId == content.Exam.Topics[0].Id));
            Render();
            AssertPage<ExerciseView>(window);
            Click(window, "← Zadania");
            AssertPage<TaskTopicsView>(window);

            Click(window, "Kalkulator");
            AssertPage<CalculatorView>(window);
            viewModel.OpenGeneralCalculator();
            Render();
            AssertPage<GeneralCalculatorView>(window);

            viewModel.OpenRoadmap("graph-generator");
            Render();
            AssertPage<RoadmapView>(window);

            Click(window, "O programie");
            AssertPage<AboutView>(window);

            Click(window, "Profil");
            AssertPage<ProfileView>(window);

            var calculatorPlaceholder = content.Placeholders.Items.First(item => item.Category == "calculator");
            viewModel.OpenPlaceholder(calculatorPlaceholder);
            Render();
            AssertPage<PlaceholderView>(window);
            Click(window, "← Wróć");
            AssertPage<CalculatorView>(window);

            var examPlaceholder = content.Placeholders.Items.First(item => item.Category == "exam");
            viewModel.OpenPlaceholder(examPlaceholder);
            Render();
            AssertPage<PlaceholderView>(window);
            Click(window, "← Wróć");
            AssertPage<MaturaView>(window);

            viewModel.Navigate((AppPage)int.MaxValue);
            Render();
            Assert.Contains(
                window.GetLogicalDescendants().OfType<TextBlock>(),
                text => text.Text == "Nie udało się otworzyć strony.");

            Click(window, "Start");
            AssertPage<HomeView>(window);

            Click(window, "Wyloguj");
            AssertPage<LoginView>(window);
        }
        finally
        {
            window.Close();
            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [AvaloniaFact]
    public void Placeholder_without_optional_content_only_invokes_back_action()
    {
        var backCalls = 0;
        var view = new PlaceholderView("Tytuł", "Komunikat", [], () => backCalls++);
        var window = new Window { Width = 800, Height = 600, Content = view };

        try
        {
            window.Show();
            Render();
            Assert.DoesNotContain(
                view.GetLogicalDescendants().OfType<Button>(),
                button => Equals(button.Content, "Zobacz w planie rozwoju"));
            Click(view, "← Wróć");
            Assert.Equal(1, backCalls);
        }
        finally
        {
            window.Close();
        }
    }

    private static T AssertPage<T>(MainWindow window)
        where T : Control =>
        Assert.Single(window.GetLogicalDescendants().OfType<T>());

    private static void Click(Control root, string content)
    {
        var button = root.GetLogicalDescendants()
            .OfType<Button>()
            .First(control => string.Equals(control.Content as string, content, StringComparison.Ordinal));
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Render();
    }

    private static void Render() => Dispatcher.UIThread.RunJobs();
}
