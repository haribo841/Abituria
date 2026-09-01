using Abituria.Data;
using Abituria.Models;
using Abituria.Services;
using Abituria.ViewModels;
using Abituria.Views;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace Abituria.Tests;

public sealed class Issue4NavigationTests
{
    private static readonly string[] HomeTileNames =
        ["Wzory", "Matura", "Zadania", "Kalkulator", "Działy", "Plan rozwoju"];

    [AvaloniaFact]
    public void Home_has_six_named_tiles_and_switches_between_two_by_three_and_one_by_six()
    {
        var repository = new ContentRepository();
        var calls = new int[6];
        var home = new HomeView(
            "Tester",
            repository.UiCopy,
            new HomeNavigationActions(
                () => calls[0]++,
                () => calls[1]++,
                () => calls[2]++,
                () => calls[3]++,
                () => calls[4]++,
                () => calls[5]++));
        var window = new Window { Width = 960, Height = 640, Content = home };

        try
        {
            window.Show();
            Render();
            var grid = home.GetLogicalDescendants().OfType<Grid>().Single(item => item.Name == "HomeLayoutRoot");
            var tiles = home.GetLogicalDescendants().OfType<Button>()
                .Where(button => HomeTileNames.Contains(AutomationProperties.GetName(button), StringComparer.Ordinal))
                .ToArray();

            Assert.Equal(6, tiles.Length);
            Assert.Equal(HomeTileNames.Order(), tiles.Select(AutomationProperties.GetName).Order());
            Assert.Equal(2, grid.ColumnDefinitions.Count);
            Assert.Equal(3, grid.RowDefinitions.Count);
            foreach (var tile in tiles)
                tile.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.Equal([1, 1, 1, 1, 1, 1], calls);

            window.Width = 720;
            window.Height = 520;
            Render();
            Assert.Single(grid.ColumnDefinitions);
            Assert.Equal(6, grid.RowDefinitions.Count);
            Assert.Equal(Enumerable.Range(0, 6), tiles.OrderBy(Grid.GetRow).Select(Grid.GetRow));
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Matura_and_tasks_have_independent_content_randomization_and_placeholder_categories()
    {
        var repository = new ContentRepository();
        var openedExams = new List<string>();
        var openedTopics = new List<string>();
        var openedPlaceholders = new List<string>();
        var randomized = new List<(string ExerciseId, string? TopicId)>();
        var matura = new MaturaView(
            repository.Exams,
            repository.Placeholders.Items,
            openedExams.Add,
            item => openedPlaceholders.Add(item.Id),
            (exercise, topicId) => randomized.Add((exercise.Id, topicId)));
        var tasks = new TaskTopicsView(
            new TaskTopicsViewContent(
                repository.Exams,
                repository.ExamTopics,
                repository.ExamIndex.TopicIntroduction,
                repository.Placeholders.Items),
            new TaskTopicsViewActions(
                openedTopics.Add,
                item => openedPlaceholders.Add(item.Id),
                (exercise, topicId) => randomized.Add((exercise.Id, topicId))));

        Assert.Equal(26, repository.Exams.Count);
        Assert.Equal(17, repository.ExamTopics.Count);
        Assert.Contains(matura.GetLogicalDescendants().OfType<Button>(), button =>
            button.Content is string text && text.Contains("33 zadania, 37 części ocenianych", StringComparison.Ordinal));
        Assert.Contains(matura.GetLogicalDescendants().OfType<Button>(), button =>
            button.Content is string text && text.Contains("12 zadań, 13 części ocenianych", StringComparison.Ordinal));
        Assert.Contains(matura.GetLogicalDescendants().OfType<Button>(), button =>
            button.Content is string text && text.Contains("13 zadań, 14 części ocenianych", StringComparison.Ordinal));
        Assert.Contains(matura.GetLogicalDescendants().OfType<Button>(), button =>
            button.Content is string text && text.Contains("31 zadań, 35 części ocenianych", StringComparison.Ordinal));
        Assert.Equal(0, matura.GetLogicalDescendants().OfType<Button>().Count(button =>
            button.Content is string text && repository.Placeholders.Items.Any(item =>
                item.Category == "exam" && text.StartsWith(item.Title, StringComparison.Ordinal))));
        Assert.DoesNotContain(matura.GetLogicalDescendants().OfType<Button>(), button =>
            button.Content is string text && text.StartsWith("Zestaw E1-E35", StringComparison.Ordinal));
        Assert.Contains(matura.GetLogicalDescendants().OfType<TextBlock>(), text =>
            text.Text == "Matura maj 2019 - poziom podstawowy (Formuła 2015)");
        Assert.Contains(matura.GetLogicalDescendants().OfType<TextBlock>(), text =>
            text.Text == "Matura maj 2019 - poziom rozszerzony (Formuła 2015)");
        Assert.Contains(matura.GetLogicalDescendants().OfType<TextBlock>(), text =>
            text.Text == "Matura poprawkowa 2019 - poziom podstawowy (Formuła 2015)");
        Assert.Contains(matura.GetLogicalDescendants().OfType<TextBlock>(), text =>
            text.Text == "Matura maj 2018 - poziom podstawowy (Formuła 2015)");
        Assert.Contains(matura.GetLogicalDescendants().OfType<TextBlock>(), text =>
            text.Text == "Matura maj 2018 - poziom rozszerzony (Formuła 2015)");
        Assert.Contains(matura.GetLogicalDescendants().OfType<TextBlock>(), text =>
            text.Text == "Matura poprawkowa 2018 - poziom podstawowy (Formuła 2015)");
        Assert.Equal(17, tasks.GetLogicalDescendants().OfType<Button>().Count(button =>
            button.Content is string text && text.StartsWith("Losuj zadanie z tematu:", StringComparison.Ordinal)));
        Assert.Single(tasks.GetLogicalDescendants().OfType<Button>(), button =>
            button.Content is string text && text.StartsWith("Zestaw E1-E35", StringComparison.Ordinal));
        Assert.DoesNotContain(tasks.GetLogicalDescendants().OfType<Button>(), button =>
            button.Content is string text && text.StartsWith("Matura 2019", StringComparison.Ordinal));

        var newestExam = repository.Exams[0];
        Click(matura, "Otwórz arkusz - 33 zadania, 37 części ocenianych");
        Click(matura, $"Losuj zadanie z arkusza: {newestExam.Title}");
        var firstTopic = repository.ExamTopics[0];
        var firstTopicCount = repository.GetTopicExercises(firstTopic.Id).Count;
        Click(tasks, $"{firstTopic.Title} - {ExerciseCountLabel(firstTopicCount)}");
        Click(tasks, $"Losuj zadanie z tematu: {firstTopic.Title}");
        Click(tasks, "Zestaw E1-E35 - treść w przygotowaniu");

        Assert.Equal([newestExam.Id], openedExams);
        Assert.Equal([firstTopic.Id], openedTopics);
        Assert.Equal(["exercise-set-e"], openedPlaceholders);
        Assert.DoesNotContain(repository.Placeholders.Items, item => item.Category == "exam");
        Assert.Null(randomized[0].TopicId);
        Assert.Equal(firstTopic.Id, randomized[1].TopicId);
    }

    [AvaloniaFact]
    public void Login_and_header_expose_the_clover_brand_name()
    {
        var repository = new ContentRepository();
        var accounts = new AccountService(
            new AppDbContextFactory(Path.Combine(Path.GetTempPath(), "Abituria.Tests", "issue4-brand.db")),
            new PasswordHasher(1_000));
        var login = new LoginView(accounts, repository.UiCopy, _ => { });

        Assert.Contains(login.GetLogicalDescendants().OfType<Control>(), control =>
            AutomationProperties.GetName(control) == "🍀 Abituria");
        var xaml = File.ReadAllText(Absolute("AvaloniaApp/MainWindow.axaml"));
        Assert.Contains("AutomationProperties.Name=\"🍀 Abituria\"", xaml, StringComparison.Ordinal);
    }

    [AvaloniaFact]
    public void Lesson_level_labels_keep_all_three_explicit_messages()
    {
        var course = new MathCourseCatalog();
        var exercises = new CourseExerciseCatalog();
        var cases = new[]
        {
            (new MathCourseLesson { Title = "Pomoc", Level = "supplemental", AlwaysVisible = true },
                "Materiał pomocniczy widoczny na obu poziomach."),
            (new MathCourseLesson { Title = "Podstawa", Level = "basic" },
                "Lekcja poziomu podstawowego."),
            (new MathCourseLesson { Title = "Rozszerzenie", Level = "extended" },
                "Lekcja poziomu rozszerzonego.")
        };

        foreach (var (lesson, expected) in cases)
        {
            var view = new CourseLessonView(course, exercises, lesson, CourseLevelFilter.Basic, _ => { }, () => { });
            Assert.Contains(view.GetLogicalDescendants().OfType<TextBlock>(), text => text.Text == expected);
        }
    }

    private static void Click(Control root, string text)
    {
        var button = root.GetLogicalDescendants().OfType<Button>()
            .Single(item => string.Equals(item.Content as string, text, StringComparison.Ordinal));
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Render();
    }

    private static string ExerciseCountLabel(int count)
    {
        if (count == 1) return "1 zadanie";
        if (count % 100 is >= 12 and <= 14) return $"{count} zadań";
        if (count % 10 is >= 2 and <= 4) return $"{count} zadania";
        return $"{count} zadań";
    }

    private static void Render() => Dispatcher.UIThread.RunJobs();

    private static string Absolute(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Abituria.sln")))
            directory = directory.Parent;
        var root = directory?.FullName ?? throw new DirectoryNotFoundException("Nie znaleziono repozytorium Abituria.");
        return Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));
    }
}
