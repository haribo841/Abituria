using System.Globalization;
using Abituria.Data;
using Abituria.Models;
using Abituria.Services;
using Abituria.ViewModels;
using Abituria.Views;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Microsoft.Data.Sqlite;

namespace Abituria.Tests;

public sealed class Matura2026UiTests
{
    private static readonly Size[] SupportedSizes = [new(720, 520), new(960, 640), new(1280, 820)];

    [AvaloniaFact]
    public void Matura_lists_2026_before_2021_and_tasks_aggregate_all_17_topics()
    {
        var repository = new ContentRepository();
        var openedExams = new List<string>();
        var openedTopics = new List<string>();
        Assert.Same(repository.Exams[0], repository.GetExam("matura-maj-2026-podstawowa"));
        Assert.Same(repository.Exams[1], repository.GetExam("matura-maj-2026-rozszerzona"));
        Assert.Same(repository.Exams[2], repository.GetExam("matura-maj-2025-podstawowa"));
        Assert.Same(repository.Exams[3], repository.GetExam("matura-maj-2025-rozszerzona"));
        Assert.Same(repository.Exams[4], repository.Exam);
        Assert.Throws<ArgumentException>(() => repository.GetExam(" "));
        Assert.Throws<KeyNotFoundException>(() => repository.GetExam("missing"));
        Assert.Throws<ArgumentException>(() => repository.GetTopicExercises(""));
        Assert.Throws<KeyNotFoundException>(() => repository.GetTopicExercises("missing"));
        var matura = new MaturaView(
            repository.Exams,
            repository.Placeholders.Items,
            openedExams.Add,
            _ => { },
            (_, _) => { });
        var tasks = new TaskTopicsView(
            new TaskTopicsViewContent(
                repository.Exams,
                repository.ExamTopics,
                repository.ExamIndex.TopicIntroduction,
                repository.Placeholders.Items),
            new TaskTopicsViewActions(openedTopics.Add, _ => { }, (_, _) => { }));

        var examTitles = matura.GetLogicalDescendants().OfType<TextBlock>()
            .Select(item => item.Text)
            .Where(item => repository.Exams.Any(exam => exam.Title == item))
            .ToArray();
        Assert.Equal(repository.Exams.Select(item => item.Title), examTitles);
        Assert.Contains(
            matura.GetLogicalDescendants().OfType<Button>(),
            button => Equals(button.Content, "Otwórz arkusz - 33 zadania, 37 części ocenianych"));
        Assert.Contains(
            matura.GetLogicalDescendants().OfType<Button>(),
            button => Equals(button.Content, "Otwórz arkusz - 12 zadań, 13 części ocenianych"));
        Assert.Contains(
            matura.GetLogicalDescendants().OfType<Button>(),
            button => Equals(button.Content, "Otwórz arkusz - 31 zadań, 35 części ocenianych"));
        Assert.Contains(
            matura.GetLogicalDescendants().OfType<Button>(),
            button => Equals(button.Content, "Otwórz arkusz - 35 zadań"));

        var topicButtons = tasks.GetLogicalDescendants().OfType<Button>()
            .Where(button => button.Content is string text && text.StartsWith("Losuj zadanie z tematu:", StringComparison.Ordinal))
            .ToArray();
        Assert.Equal(17, topicButtons.Length);
        Assert.Equal(133, repository.ExamTopics.Sum(topic => repository.GetTopicExercises(topic.Id).Count));
        Assert.All(repository.ExamTopics, topic =>
        {
            var topicExercises = repository.GetTopicExercises(topic.Id);
            var expectedIds = repository.Exams.SelectMany(exam => exam.Exercises)
                .Where(exercise => exercise.TopicId == topic.Id)
                .Select(exercise => exercise.Id);
            Assert.Equal(expectedIds, topicExercises.Select(item => item.Id));
            var count = topicExercises.Count;
            Assert.Contains(
                tasks.GetLogicalDescendants().OfType<Button>(),
                button => Equals(button.Content, $"{topic.Title} - {ExerciseCountLabel(count)}"));
        });

        Click(matura, "Otwórz arkusz - 33 zadania, 37 części ocenianych");
        Click(tasks, $"{repository.ExamTopics[0].Title} - {ExerciseCountLabel(repository.GetTopicExercises(repository.ExamTopics[0].Id).Count)}");
        Assert.Equal([repository.Exams[0].Id], openedExams);
        Assert.Equal([repository.ExamTopics[0].Id], openedTopics);
    }

    [AvaloniaFact]
    public void Exam_and_topic_navigation_preserve_the_selected_exam_and_return_context()
    {
        var repository = new ContentRepository();
        var current = repository.Exams[0];
        var legacy = repository.Exams[4];
        var viewModel = new AppViewModel();

        viewModel.OpenExam(current.Id);
        Assert.Equal(AppPage.ExerciseList, viewModel.CurrentPage);
        Assert.Equal(current.Id, viewModel.SelectedExamId);
        Assert.Equal(ExamNavigationOrigin.Matura, viewModel.ExamNavigationOrigin);
        Assert.Null(viewModel.SelectedTopicId);

        viewModel.OpenExercise(current.Exercises[0]);
        Assert.Equal(current.Id, viewModel.SelectedExamId);
        Assert.Equal(AppPage.Exercise, viewModel.CurrentPage);

        var topic = repository.ExamTopics[0];
        var legacyTopicExercise = legacy.Exercises.First(item => item.TopicId == topic.Id);
        viewModel.OpenTopic(topic.Id);
        viewModel.OpenExercise(legacyTopicExercise);
        Assert.Equal(topic.Id, viewModel.SelectedTopicId);
        Assert.Equal(legacy.Id, viewModel.SelectedExamId);
        Assert.Equal(ExamNavigationOrigin.Tasks, viewModel.ExamNavigationOrigin);

        viewModel.OpenRandomExercise(current.Exercises[1], null);
        Assert.Equal(current.Id, viewModel.SelectedExamId);
        Assert.Null(viewModel.SelectedTopicId);
        Assert.Equal(ExamNavigationOrigin.Matura, viewModel.ExamNavigationOrigin);

        viewModel.OpenRandomExercise(legacyTopicExercise, topic.Id);
        Assert.Equal(legacy.Id, viewModel.SelectedExamId);
        Assert.Equal(topic.Id, viewModel.SelectedTopicId);
        Assert.Equal(ExamNavigationOrigin.Tasks, viewModel.ExamNavigationOrigin);
    }

    [AvaloniaFact]
    public async Task Topic_list_shows_each_exams_source_and_profile_tracks_progress_separately()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var accounts = new AccountService(
            new AppDbContextFactory(Path.Combine(directory, "multi-exam.db")),
            new PasswordHasher(1_000));
        await accounts.InitializeAsync();
        var profile = (await accounts.GetProfilesAsync()).Single(item => item.Kind == ProfileKind.Guest);
        var repository = new ContentRepository();
        var topic = repository.ExamTopics.First(item =>
            repository.Exams.All(exam => exam.Exercises.Any(exercise => exercise.TopicId == item.Id)));
        var exercises = repository.GetTopicExercises(topic.Id);
        var examTitles = repository.Exams.ToDictionary(item => item.Id, item => item.Title, StringComparer.Ordinal);
        var list = new ExerciseListView(
            new ExerciseListViewContent(
                topic.Title,
                "Zadania z aktywnych arkuszy.",
                exercises,
                examTitles,
                true),
            profile,
            accounts,
            new ExerciseListViewActions(_ => { }, "Wróć", () => { }));
        var window = new Window { Width = 960, Height = 640, Content = list };

        try
        {
            window.Show();
            await WaitUntilAsync(() => list.GetLogicalDescendants().OfType<Button>()
                .Count(button => button.Classes.Contains("list")) == exercises.Count);
            var labels = list.GetLogicalDescendants().OfType<Button>()
                .Select(button => button.Content as string)
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .ToArray();
            Assert.All(exercises, exercise => Assert.Contains(
                labels,
                label => label!.Contains(examTitles[exercise.ExamId], StringComparison.Ordinal)));

            foreach (var exam in repository.Exams)
                await accounts.MarkExerciseCompletedAsync(profile.Id, exam.Exercises[0].Id);
            var profileView = new ProfileView(profile, accounts, repository.Exams, repository.CourseExercises, () => { });
            window.Content = profileView;
            Render();
            await WaitUntilAsync(() => profileView.GetLogicalDescendants().OfType<TextBlock>()
                .Any(text => text.Text?.Contains("Matura maj 2026 PP: 1 / 37", StringComparison.Ordinal) == true));
            var progress = profileView.GetLogicalDescendants().OfType<TextBlock>()
                .Single(text => AutomationProperties.GetName(text) == "Postęp w zadaniach");
            Assert.Contains("Matura maj 2026 PP: 1 / 37", progress.Text, StringComparison.Ordinal);
            Assert.Contains("Matura maj 2026 PR: 1 / 13", progress.Text, StringComparison.Ordinal);
            Assert.Contains("Matura maj 2025 PP: 1 / 35", progress.Text, StringComparison.Ordinal);
            Assert.Contains("Matura maj 2025 PR: 1 / 13", progress.Text, StringComparison.Ordinal);
            Assert.Contains("Matura poprawkowa 2021: 1 / 35", progress.Text, StringComparison.Ordinal);
        }
        finally
        {
            window.Close();
            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [AvaloniaFact]
    public async Task Every_compound_exam_item_can_be_completed_with_keyboard_accessible_controls()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var accounts = new AccountService(
            new AppDbContextFactory(Path.Combine(directory, "compound-ui.db")),
            new PasswordHasher(1_000));
        await accounts.InitializeAsync();
        var profile = (await accounts.GetProfilesAsync()).Single(item => item.Kind == ProfileKind.Guest);
        var repository = new ContentRepository();
        var window = new Window { Width = 720, Height = 520 };

        try
        {
            window.Show();
            foreach (var exam in repository.Exams)
            {
                foreach (var exercise in exam.Exercises.Where(item => item.IsCompound))
                {
                    var context = new ExerciseViewContext(
                        exam.Exercises,
                        exam.Source,
                        repository.UiCopy,
                        profile,
                        accounts,
                        repository.Diagrams,
                        () => { },
                        _ => { });
                    var view = new ExerciseView(exercise, context);
                    window.Content = view;
                    Render();
                    FillCompoundAnswer(view, exercise);
                    var check = view.GetLogicalDescendants().OfType<Button>()
                        .Single(button => Equals(button.Content, "Sprawdź wszystkie odpowiedzi"));
                    Assert.True(check.Focusable);
                    Assert.False(string.IsNullOrWhiteSpace(AutomationProperties.GetHelpText(check)));
                    Click(check);
                    await WaitUntilAsync(async () =>
                        (await accounts.GetCompletedExerciseIdsAsync(profile.Id)).Contains(exercise.Id));
                    Assert.Contains(
                        view.GetLogicalDescendants().OfType<TextBlock>(),
                        text => text.Text == "Wszystkie części odpowiedzi są poprawne. Zadanie zapisano jako ukończone.");
                }
            }
        }
        finally
        {
            window.Close();
            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [AvaloniaFact]
    public void Matura_and_tasks_render_without_overflow_at_supported_sizes()
    {
        var repository = new ContentRepository();
        var window = new Window();
        var views = new Control[]
        {
            new MaturaView(repository.Exams, repository.Placeholders.Items, _ => { }, _ => { }, (_, _) => { }),
            new TaskTopicsView(
                new TaskTopicsViewContent(
                    repository.Exams,
                    repository.ExamTopics,
                    repository.ExamIndex.TopicIntroduction,
                    repository.Placeholders.Items),
                new TaskTopicsViewActions(_ => { }, _ => { }, (_, _) => { }))
        };

        try
        {
            window.Show();
            foreach (var view in views)
            {
                window.Content = view;
                foreach (var size in SupportedSizes)
                {
                    window.Width = size.Width;
                    window.Height = size.Height;
                    Render();
                    Assert.InRange(view.Bounds.Width, 1, size.Width);
                    Assert.InRange(view.Bounds.Height, 1, size.Height);
                    using var frame = Assert.IsType<WriteableBitmap>(window.CaptureRenderedFrame());
                }
            }
        }
        finally
        {
            window.Close();
        }
    }

    private static void FillCompoundAnswer(Control view, LearningExercise exercise)
    {
        foreach (var part in exercise.AnswerParts)
        {
            if (part.IsMultipleChoice)
            {
                var choices = view.GetLogicalDescendants().OfType<RadioButton>()
                    .Where(button => button.GroupName == $"{exercise.Id}-{part.Id}")
                    .ToArray();
                choices[part.CorrectOption!.Value - 1].IsChecked = true;
                continue;
            }

            var answer = view.GetLogicalDescendants().OfType<TextBox>()
                .Single(box => AutomationProperties.GetName(box) == $"Odpowiedź do części: {part.Prompt}");
            answer.Text = part.IsNumeric
                ? part.ExpectedValue!.Value.ToString(CultureInfo.InvariantCulture)
                : part.AcceptedAnswers[0];
        }
        Render();
    }

    private static string ExerciseCountLabel(int count)
    {
        if (count == 1) return "1 zadanie";
        if (count % 100 is >= 12 and <= 14) return $"{count} zadań";
        return count % 10 is >= 2 and <= 4 ? $"{count} zadania" : $"{count} zadań";
    }

    private static void Click(Control root, string content)
    {
        var button = root.GetLogicalDescendants().OfType<Button>()
            .Single(item => Equals(item.Content, content));
        Click(button);
    }

    private static void Click(Button button)
    {
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Render();
    }

    private static void Render() => Dispatcher.UIThread.RunJobs();

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            Render();
            if (condition()) return;
            await Task.Delay(10);
        }
        Assert.Fail("Warunek interfejsu nie został spełniony.");
    }

    private static async Task WaitUntilAsync(Func<Task<bool>> condition)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            Render();
            if (await condition()) return;
            await Task.Delay(10);
        }
        Assert.Fail("Warunek interfejsu nie został spełniony.");
    }
}
