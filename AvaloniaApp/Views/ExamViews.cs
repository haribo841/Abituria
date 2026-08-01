using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abituria.Models;
using Abituria.Services;
using Abituria.Ui;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Abituria.Views;

public sealed class MaturaView : UserControl
{
    public MaturaView(
        ExamDefinition exam,
        IEnumerable<PlaceholderItem> placeholders,
        Action openExam,
        Action<PlaceholderItem> openPlaceholder,
        Action<LearningExercise, string?> openExercise,
        ExerciseRandomizer? randomizer = null)
    {
        var exerciseRandomizer = randomizer ?? new ExerciseRandomizer();
        var root = new StackPanel { Spacing = 14 };
        root.Children.Add(UiFactory.PageTitle("Matura", "Pracuj z pełnym arkuszem albo wybierz archiwalny zestaw."));
        if (exam.Introduction.Count > 0)
            root.Children.Add(UiFactory.Card(new RichContentView(exam.Introduction)));
        root.Children.Add(ExamOverviewControls.ListButton(
            $"{exam.Title} - {ExamOverviewControls.ExerciseCountLabel(exam.Exercises.Count)}",
            openExam));
        root.Children.Add(ExamOverviewControls.RandomExerciseButton(
            "Losuj zadanie z tego arkusza", exam.Exercises, null, exerciseRandomizer, openExercise));
        foreach (var placeholder in placeholders.Where(item => item.Category == "exam"))
            root.Children.Add(ExamOverviewControls.ListButton(
                $"{placeholder.Title} - treść w przygotowaniu", () => openPlaceholder(placeholder)));
        Content = UiFactory.PageScroll(root);
    }
}

public sealed class TaskTopicsView : UserControl
{
    public TaskTopicsView(
        ExamDefinition exam,
        IEnumerable<PlaceholderItem> placeholders,
        Action<string> openTopic,
        Action<PlaceholderItem> openPlaceholder,
        Action<LearningExercise, string?> openExercise,
        ExerciseRandomizer? randomizer = null)
    {
        var exerciseRandomizer = randomizer ?? new ExerciseRandomizer();
        var root = new StackPanel { Spacing = 14 };
        root.Children.Add(UiFactory.PageTitle("Zadania", "Wybierz jeden z 17 tematów albo wylosuj zadanie tematyczne."));
        if (exam.TopicIntroduction.Count > 0)
            root.Children.Add(UiFactory.Card(new RichContentView(exam.TopicIntroduction)));
        foreach (var topic in exam.Topics)
        {
            var topicExercises = exam.Exercises.Where(item => item.TopicId == topic.Id).ToArray();
            root.Children.Add(ExamOverviewControls.RandomExerciseButton(
                $"Losuj zadanie z tematu: {topic.Title}", topicExercises, topic.Id, exerciseRandomizer, openExercise));
            root.Children.Add(ExamOverviewControls.ListButton(
                $"{topic.Title} - {ExamOverviewControls.ExerciseCountLabel(topic.ExerciseNumbers.Count)}",
                () => openTopic(topic.Id)));
        }
        foreach (var placeholder in placeholders.Where(item => item.Category == "exercise"))
            root.Children.Add(ExamOverviewControls.ListButton(
                $"{placeholder.Title} - treść w przygotowaniu", () => openPlaceholder(placeholder)));
        Content = UiFactory.PageScroll(root);
    }
}

internal static class ExamOverviewControls
{
    public static Button ListButton(string text, Action action)
    {
        var button = new Button { Content = text, Classes = { "list" }, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left };
        button.Click += (_, _) => action();
        return button;
    }

    public static Button RandomExerciseButton(
        string text,
        IReadOnlyList<LearningExercise> exercises,
        string? topicId,
        ExerciseRandomizer randomizer,
        Action<LearningExercise, string?> openExercise)
    {
        var button = new Button
        {
            Content = text,
            Classes = { "primary" },
            HorizontalAlignment = HorizontalAlignment.Left,
            IsEnabled = exercises.Count > 0
        };
        button.Click += (_, _) =>
        {
            var exercise = randomizer.Select(exercises);
            if (exercise is not null) openExercise(exercise, topicId);
        };
        return button;
    }

    public static string ExerciseCountLabel(int count)
    {
        var lastTwoDigits = count % 100;
        if (count == 1) return "1 zadanie";
        if (lastTwoDigits is >= 12 and <= 14) return $"{count} zadań";
        return count % 10 is >= 2 and <= 4 ? $"{count} zadania" : $"{count} zadań";
    }
}

public sealed class ExerciseListView : UserControl
{
    private readonly StackPanel _list = new() { Spacing = 8 };

    public ExerciseListView(
        ExamDefinition exam,
        string? topicId,
        LocalProfile profile,
        AccountService accounts,
        Action<LearningExercise> open,
        string backLabel,
        Action back)
    {
        var root = new StackPanel { Spacing = 16 };
        var backButton = new Button { Content = backLabel, Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
        backButton.Click += (_, _) => back();
        root.Children.Add(backButton);
        var topic = exam.Topics.SingleOrDefault(item => item.Id == topicId);
        var title = topic is null ? exam.Title : topic.Title;
        var subtitle = topic is null
            ? "Zadania 1–28 są zamknięte; zadania 29–35 prowadzą przez rozwiązanie otwarte."
            : $"Zadania z arkusza {exam.Title.ToLowerInvariant()} przypisane do wybranego zagadnienia.";
        root.Children.Add(UiFactory.PageTitle(title, subtitle));
        root.Children.Add(_list);
        Content = UiFactory.PageScroll(root);
        AttachedToVisualTree += async (_, _) => await LoadAsync(exam, topic, profile, accounts, open);
    }

    private async Task LoadAsync(ExamDefinition exam, ExerciseTopicDefinition? topic, LocalProfile profile, AccountService accounts, Action<LearningExercise> open)
    {
        var completed = await accounts.GetCompletedExerciseIdsAsync(profile.Id);
        _list.Children.Clear();
        var exercises = topic is null
            ? exam.Exercises
            : exam.Exercises.Where(item => item.TopicId == topic.Id);
        foreach (var exercise in exercises.OrderBy(item => item.Number))
        {
            var done = completed.Contains(exercise.Id) ? " ✓" : string.Empty;
            var type = exercise.IsMultipleChoice ? "A–D" : "otwarte";
            var button = new Button
            {
                Content = $"{exercise.Title} · {type}{done}",
                Classes = { "list" },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
            button.Click += (_, _) => open(exercise);
            _list.Children.Add(button);
        }
    }
}

public sealed record ExerciseViewContext(
    IReadOnlyList<LearningExercise> Exercises,
    SourceDocument Source,
    UiCopyCatalog Copy,
    LocalProfile Profile,
    AccountService Accounts,
    DiagramCatalog Diagrams,
    Action Back,
    Action<LearningExercise> OpenExercise)
{
    public string BackLabel { get; init; } = "Lista zadań";
    public string? SourceUrl { get; init; }
    public ExerciseScratchpadSession? Scratchpads { get; init; }
    public ITextClipboard? Clipboard { get; init; }
    public Action? OpenCalculatorPip { get; init; }
}

public sealed class ExerciseView : UserControl
{
    private const string GhostButtonClass = "ghost";

    private readonly TextBlock _status = new() { TextWrapping = TextWrapping.Wrap };
    private readonly Border _hintHost = UiFactory.Card(new TextBlock { Text = "Podpowiedź pojawi się tutaj.", Classes = { "muted" } }, new Thickness(16), "SurfaceAltBrush");
    private int _hintIndex;
    private int? _selectedOption;

    public ExerciseView(LearningExercise exercise, ExerciseViewContext context)
    {
        AutomationProperties.SetLiveSetting(_status, AutomationLiveSetting.Polite);
        AutomationProperties.SetName(_status, "Wynik sprawdzania zadania");
        AutomationProperties.SetLiveSetting(_hintHost, AutomationLiveSetting.Polite);
        AutomationProperties.SetName(_hintHost, "Aktualna podpowiedź lub odpowiedź");
        var root = new StackPanel { Spacing = 16 };
        root.Children.Add(BuildNavigation(exercise, context));
        AddPrompt(root, exercise, context.Diagrams);
        AddScratchpad(root, exercise, context);
        AddAnswerControls(root, exercise, context);
        AddHintControls(root, exercise);
        root.Children.Add(BuildSourceBand(exercise, context));
        root.Children.Add(_status);
        Content = UiFactory.PageScroll(root);
    }

    private static StackPanel BuildNavigation(LearningExercise exercise, ExerciseViewContext context)
    {
        var topButtons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        var backButton = new Button { Content = $"← {context.BackLabel}", Classes = { GhostButtonClass } };
        backButton.Click += (_, _) => context.Back();
        topButtons.Children.Add(backButton);
        var currentIndex = context.Exercises.ToList().FindIndex(item => item.Id == exercise.Id);
        var previous = currentIndex > 0 ? context.Exercises[currentIndex - 1] : null;
        var next = currentIndex >= 0 && currentIndex < context.Exercises.Count - 1 ? context.Exercises[currentIndex + 1] : null;
        if (previous is not null)
            topButtons.Children.Add(NavigationButton("←", "Poprzednie zadanie", previous, context.OpenExercise));
        if (next is not null)
            topButtons.Children.Add(NavigationButton("→", "Następne zadanie", next, context.OpenExercise));
        return topButtons;
    }

    private static Button NavigationButton(
        string label,
        string actionName,
        LearningExercise target,
        Action<LearningExercise> openExercise)
    {
        var button = new Button { Content = label, Classes = { GhostButtonClass } };
        ToolTip.SetTip(button, target.Title);
        AutomationProperties.SetName(button, $"{actionName}: {target.Title}");
        AutomationProperties.SetHelpText(button, target.Title);
        button.Click += (_, _) => openExercise(target);
        return button;
    }

    private static void AddPrompt(StackPanel root, LearningExercise exercise, DiagramCatalog diagrams)
    {
        root.Children.Add(UiFactory.PageTitle(exercise.Title, ExerciseSubtitle(exercise)));
        root.Children.Add(UiFactory.Card(new RichContentView([new ContentBlock { Type = "richText", Text = exercise.Prompt }])));
        foreach (var diagramId in exercise.DiagramIds)
            root.Children.Add(UiFactory.Card(new DiagramView(diagrams.GetRequired(diagramId))));
    }

    private void AddScratchpad(StackPanel root, LearningExercise exercise, ExerciseViewContext context)
    {
        var scratchpad = new TextBox
        {
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 130,
            Text = context.Scratchpads?.GetText(context.Profile.Id, exercise.Id) ?? string.Empty,
            PlaceholderText = "Zapisz tutaj własne obliczenia. Brudnopis jest przechowywany do zamknięcia aplikacji."
        };
        AutomationProperties.SetName(scratchpad, "Brudnopis do zadania");
        if (context.Scratchpads is not null)
            scratchpad.TextChanged += (_, _) => context.Scratchpads.SetText(context.Profile.Id, exercise.Id, scratchpad.Text);
        if (context.Clipboard is not null)
            TextBoxClipboardBehavior.Attach(scratchpad, context.Clipboard, message => ShowStatus(message, false));

        var scratchPanel = new StackPanel { Spacing = 8 };
        var header = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto"), ColumnSpacing = 10 };
        header.Children.Add(new TextBlock
        {
            Text = "Brudnopis",
            FontSize = 18,
            FontWeight = FontWeight.SemiBold,
            VerticalAlignment = VerticalAlignment.Center
        });
        if (context.OpenCalculatorPip is not null)
        {
            var openCalculator = new Button { Content = "Otwórz kalkulator PiP", Classes = { GhostButtonClass } };
            AutomationProperties.SetName(openCalculator, "Otwórz kalkulator Picture in Picture");
            openCalculator.Click += (_, _) => context.OpenCalculatorPip();
            Grid.SetColumn(openCalculator, 1);
            header.Children.Add(openCalculator);
        }
        scratchPanel.Children.Add(header);
        scratchPanel.Children.Add(scratchpad);
        root.Children.Add(UiFactory.Card(scratchPanel));
    }

    private void AddAnswerControls(StackPanel root, LearningExercise exercise, ExerciseViewContext context)
    {
        if (exercise.IsMultipleChoice)
            AddMultipleChoiceControls(root, exercise, context);
        else if (exercise.IsNumeric)
            AddNumericControls(root, exercise, context);
        else
            AddRevealControl(root, exercise, context);
    }

    private void AddMultipleChoiceControls(StackPanel root, LearningExercise exercise, ExerciseViewContext context)
    {
        var options = new StackPanel { Spacing = 8 };
        for (var index = 0; index < exercise.Options.Count; index++)
        {
            var optionNumber = index + 1;
            var radio = new RadioButton
            {
                GroupName = "exercise-answer",
                Content = RichContentView.CreateText($"{(char)('A' + index)}. {exercise.Options[index]}")
            };
            radio.IsCheckedChanged += (_, _) => { if (radio.IsChecked == true) _selectedOption = optionNumber; };
            options.Children.Add(radio);
        }
        root.Children.Add(UiFactory.Card(options));

        var check = new Button { Content = "Sprawdź odpowiedź", Classes = { "primary" }, HorizontalAlignment = HorizontalAlignment.Left };
        AutomationProperties.SetHelpText(
            check,
            "Poprawna odpowiedź zostanie zapisana w lokalnym profilu jako ukończone zadanie.");
        check.Click += async (_, _) =>
        {
            if (_selectedOption is null) { ShowStatus("Najpierw wybierz odpowiedź.", false); return; }
            if (_selectedOption == exercise.CorrectOption)
            {
                await context.Accounts.MarkExerciseCompletedAsync(context.Profile.Id, exercise.Id);
                ShowStatus("Poprawna odpowiedź. Zadanie zapisano jako ukończone.", true);
            }
            else ShowStatus("To nie jest poprawna odpowiedź. Skorzystaj z podpowiedzi i spróbuj ponownie.", false);
        };
        root.Children.Add(check);
    }

    private void AddNumericControls(StackPanel root, LearningExercise exercise, ExerciseViewContext context)
    {
        var answer = new TextBox { PlaceholderText = "Wpisz liczbę lub proste wyrażenie" };
        AutomationProperties.SetName(answer, "Odpowiedź liczbowa");
        if (context.Clipboard is not null)
            TextBoxClipboardBehavior.Attach(answer, context.Clipboard, message => ShowStatus(message, false));
        root.Children.Add(answer);

        var check = new Button
        {
            Content = "Sprawdź wynik",
            Classes = { "primary" },
            HorizontalAlignment = HorizontalAlignment.Left
        };
        AutomationProperties.SetHelpText(
            check,
            "Możesz użyć przecinka lub kropki dziesiętnej. Poprawny wynik zostanie zapisany w lokalnym profilu.");
        check.Click += async (_, _) =>
        {
            var result = new NumericAnswerEvaluator(new ExpressionCalculator()).Evaluate(exercise, answer.Text);
            if (result.IsCorrect)
                await context.Accounts.MarkExerciseCompletedAsync(context.Profile.Id, exercise.Id);
            ShowStatus(result.Message, result.IsCorrect);
        };
        root.Children.Add(check);
    }

    private void AddRevealControl(StackPanel root, LearningExercise exercise, ExerciseViewContext context)
    {
        var reveal = new Button
        {
            Content = "Pokaż odpowiedź i oznacz jako ukończone",
            Classes = { "primary" },
            HorizontalAlignment = HorizontalAlignment.Left
        };
        AutomationProperties.SetHelpText(
            reveal,
            "Akcja ujawni odpowiedź i zapisze zadanie w lokalnym profilu jako ukończone.");
        reveal.Click += async (_, _) =>
        {
            _hintHost.Child = RichContentView.CreateText(exercise.RevealedAnswer ?? "Brak zapisanej odpowiedzi.");
            await context.Accounts.MarkExerciseCompletedAsync(context.Profile.Id, exercise.Id);
            ShowStatus("Odpowiedź została ujawniona. Zadanie zapisano jako ukończone.", true);
        };
        root.Children.Add(reveal);
    }

    private void AddHintControls(StackPanel root, LearningExercise exercise)
    {
        var hint = new Button { Content = "Następna podpowiedź", Classes = { GhostButtonClass }, HorizontalAlignment = HorizontalAlignment.Left };
        hint.Click += (_, _) =>
        {
            if (_hintIndex >= exercise.Hints.Count) { ShowStatus("To była ostatnia podpowiedź.", true); return; }
            _hintHost.Child = RichContentView.CreateText(exercise.Hints[_hintIndex++]);
        };
        root.Children.Add(hint);
        root.Children.Add(_hintHost);
    }

    private void ShowStatus(string message, bool success)
    {
        _status.Text = message;
        UiFactory.UseResource(_status, TextBlock.ForegroundProperty, success ? "SuccessBrush" : "ErrorBrush");
    }

    private static string FormatVerifiedOn(string value) =>
        DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date.ToString("d MMMM yyyy 'r'", CultureInfo.GetCultureInfo("pl-PL"))
            : value;

    private static string ExerciseSubtitle(LearningExercise exercise)
    {
        if (exercise.IsMultipleChoice)
            return "Wybierz jedną odpowiedź.";
        if (exercise.IsNumeric)
            return "Oblicz wynik. Możesz wpisać liczbę albo bezpieczne wyrażenie matematyczne.";
        return "Rozwiązuj samodzielnie i świadomie ujawnij pełne rozwiązanie.";
    }

    private static Border BuildSourceBand(LearningExercise exercise, ExerciseViewContext context)
    {
        if (!exercise.IsCourseExercise)
            return UiFactory.InfoBand(context.Copy.FormatRequired(
                "exam.source",
                exercise.VerificationSource,
                exercise.SourcePage,
                FormatVerifiedOn(context.Source.VerifiedOn)));

        var sourceUrl = string.IsNullOrWhiteSpace(context.SourceUrl) ? string.Empty : $"\n{context.SourceUrl}";
        return UiFactory.InfoBand(
            "Źródło wymagania",
            $"{exercise.VerificationSource}. Weryfikacja: {FormatVerifiedOn(context.Source.VerifiedOn)}.{sourceUrl}");
    }
}
