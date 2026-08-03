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
        IReadOnlyList<ExamDefinition> exams,
        IEnumerable<PlaceholderItem> placeholders,
        Action<string> openExam,
        Action<PlaceholderItem> openPlaceholder,
        Action<LearningExercise, string?> openExercise,
        ExerciseRandomizer? randomizer = null)
    {
        var exerciseRandomizer = randomizer ?? new ExerciseRandomizer();
        var root = new StackPanel { Spacing = 14 };
        root.Children.Add(UiFactory.PageTitle("Matura", "Pracuj z pełnym arkuszem albo wybierz archiwalny zestaw."));
        foreach (var exam in exams)
        {
            var examPanel = new StackPanel { Spacing = 10 };
            examPanel.Children.Add(new TextBlock { Text = exam.Title, Classes = { "h2" } });
            if (exam.Introduction.Count > 0)
                examPanel.Children.Add(new RichContentView(exam.Introduction));
            examPanel.Children.Add(ExamOverviewControls.ListButton(
                $"Otwórz arkusz - {ExamOverviewControls.ExamCountLabel(exam)}",
                () => openExam(exam.Id)));
            examPanel.Children.Add(ExamOverviewControls.RandomExerciseButton(
                $"Losuj zadanie z arkusza: {exam.Title}", exam.Exercises, null, exerciseRandomizer, openExercise));
            root.Children.Add(UiFactory.Card(examPanel));
        }
        foreach (var placeholder in placeholders.Where(item => item.Category == "exam"))
            root.Children.Add(ExamOverviewControls.ListButton(
                $"{placeholder.Title} - treść w przygotowaniu", () => openPlaceholder(placeholder)));
        Content = UiFactory.PageScroll(root);
    }
}

public sealed record TaskTopicsViewContent(
    IReadOnlyList<ExamDefinition> Exams,
    IReadOnlyList<ExerciseTopicDefinition> Topics,
    IReadOnlyList<ContentBlock> TopicIntroduction,
    IEnumerable<PlaceholderItem> Placeholders);

public sealed record TaskTopicsViewActions(
    Action<string> OpenTopic,
    Action<PlaceholderItem> OpenPlaceholder,
    Action<LearningExercise, string?> OpenExercise);

public sealed class TaskTopicsView : UserControl
{
    public TaskTopicsView(
        TaskTopicsViewContent content,
        TaskTopicsViewActions actions,
        ExerciseRandomizer? randomizer = null)
    {
        var exerciseRandomizer = randomizer ?? new ExerciseRandomizer();
        var root = new StackPanel { Spacing = 14 };
        root.Children.Add(UiFactory.PageTitle("Zadania", "Wybierz jeden z 17 tematów albo wylosuj zadanie tematyczne."));
        if (content.TopicIntroduction.Count > 0)
            root.Children.Add(UiFactory.Card(new RichContentView(content.TopicIntroduction)));
        var allExercises = content.Exams.SelectMany(item => item.Exercises).ToArray();
        foreach (var topic in content.Topics.OrderBy(item => item.Order))
        {
            var topicExercises = allExercises.Where(item => item.TopicId == topic.Id).ToArray();
            root.Children.Add(ExamOverviewControls.RandomExerciseButton(
                $"Losuj zadanie z tematu: {topic.Title}", topicExercises, topic.Id, exerciseRandomizer, actions.OpenExercise));
            root.Children.Add(ExamOverviewControls.ListButton(
                $"{topic.Title} - {ExamOverviewControls.ExerciseCountLabel(topicExercises.Length)}",
                () => actions.OpenTopic(topic.Id)));
        }
        foreach (var placeholder in content.Placeholders.Where(item => item.Category == "exercise"))
            root.Children.Add(ExamOverviewControls.ListButton(
                $"{placeholder.Title} - treść w przygotowaniu", () => actions.OpenPlaceholder(placeholder)));
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

    public static string ExamCountLabel(ExamDefinition exam) =>
        exam.OfficialTaskCount == exam.ProgressItemCount
            ? ExerciseCountLabel(exam.OfficialTaskCount)
            : $"{ExerciseCountLabel(exam.OfficialTaskCount)}, {exam.ProgressItemCount} części ocenianych";
}

public sealed record ExerciseListViewContent(
    string Title,
    string Subtitle,
    IReadOnlyList<LearningExercise> Exercises,
    IReadOnlyDictionary<string, string> ExamTitles,
    bool ShowExamSource);

public sealed record ExerciseListViewActions(
    Action<LearningExercise> Open,
    string BackLabel,
    Action Back);

public sealed class ExerciseListView : UserControl
{
    private readonly StackPanel _list = new() { Spacing = 8 };

    public ExerciseListView(
        ExerciseListViewContent content,
        LocalProfile profile,
        AccountService accounts,
        ExerciseListViewActions actions)
    {
        var root = new StackPanel { Spacing = 16 };
        var backButton = new Button { Content = actions.BackLabel, Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
        backButton.Click += (_, _) => actions.Back();
        root.Children.Add(backButton);
        root.Children.Add(UiFactory.PageTitle(content.Title, content.Subtitle));
        root.Children.Add(_list);
        Content = UiFactory.PageScroll(root);
        AttachedToVisualTree += async (_, _) =>
            await LoadAsync(content.Exercises, content.ExamTitles, content.ShowExamSource, profile, accounts, actions.Open);
    }

    private async Task LoadAsync(
        IReadOnlyList<LearningExercise> exercises,
        IReadOnlyDictionary<string, string> examTitles,
        bool showExamSource,
        LocalProfile profile,
        AccountService accounts,
        Action<LearningExercise> open)
    {
        var completed = await accounts.GetCompletedExerciseIdsAsync(profile.Id);
        _list.Children.Clear();
        foreach (var exercise in exercises)
        {
            var done = completed.Contains(exercise.Id) ? " ✓" : string.Empty;
            var source = showExamSource && examTitles.TryGetValue(exercise.ExamId, out var examTitle)
                ? $" · {examTitle}"
                : string.Empty;
            var button = new Button
            {
                Content = $"{exercise.Title} · {AnswerTypeLabel(exercise)}{source}{done}",
                Classes = { "list" },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
            button.Click += (_, _) => open(exercise);
            _list.Children.Add(button);
        }
    }

    private static string AnswerTypeLabel(LearningExercise exercise)
    {
        if (exercise.IsMultipleChoice) return "wybór A-D";
        if (exercise.IsCompound) return "odpowiedź złożona";
        if (exercise.IsNumeric) return "wynik liczbowy";
        return "rozwiązanie otwarte";
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
    private const string PrimaryButtonClass = "primary";

    private readonly TextBlock _status = new() { TextWrapping = TextWrapping.Wrap };
    private readonly Border _hintHost = UiFactory.Card(new TextBlock { Text = "Podpowiedź pojawi się tutaj.", Classes = { "muted" } }, new Thickness(16), "SurfaceAltBrush");
    private readonly Dictionary<string, string?> _compoundAnswers = new(StringComparer.Ordinal);
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
        else if (exercise.IsCompound)
            AddCompoundControls(root, exercise, context);
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

        var check = new Button { Content = "Sprawdź odpowiedź", Classes = { PrimaryButtonClass }, HorizontalAlignment = HorizontalAlignment.Left };
        AutomationProperties.SetHelpText(
            check,
            "Poprawna odpowiedź zostanie zapisana w lokalnym profilu jako ukończone zadanie.");
        check.Click += async (_, _) =>
        {
            if (_selectedOption is null) { ShowStatus("Najpierw wybierz odpowiedź.", false); return; }
            if (_selectedOption == exercise.CorrectOption)
            {
                await context.Accounts.MarkExerciseCompletedAsync(context.Profile.Id, exercise.Id);
                ShowSolution(exercise);
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
            Classes = { PrimaryButtonClass },
            HorizontalAlignment = HorizontalAlignment.Left
        };
        AutomationProperties.SetHelpText(
            check,
            "Możesz użyć przecinka lub kropki dziesiętnej. Poprawny wynik zostanie zapisany w lokalnym profilu.");
        check.Click += async (_, _) =>
        {
            var result = new NumericAnswerEvaluator(new ExpressionCalculator()).Evaluate(exercise, answer.Text);
            if (result.IsCorrect)
            {
                await context.Accounts.MarkExerciseCompletedAsync(context.Profile.Id, exercise.Id);
                ShowSolution(exercise);
            }
            ShowStatus(result.Message, result.IsCorrect);
        };
        root.Children.Add(check);
    }

    private void AddCompoundControls(StackPanel root, LearningExercise exercise, ExerciseViewContext context)
    {
        var answerPanel = new StackPanel { Spacing = 14 };
        foreach (var part in exercise.AnswerParts)
            answerPanel.Children.Add(BuildCompoundPart(exercise, part, context));
        root.Children.Add(UiFactory.Card(answerPanel));

        var check = new Button
        {
            Content = "Sprawdź wszystkie odpowiedzi",
            Classes = { PrimaryButtonClass },
            HorizontalAlignment = HorizontalAlignment.Left
        };
        AutomationProperties.SetHelpText(
            check,
            "Wszystkie części muszą być uzupełnione poprawnie, aby zadanie zostało zapisane jako ukończone.");
        check.Click += async (_, _) =>
        {
            var numeric = new NumericAnswerEvaluator(new ExpressionCalculator());
            var result = new CompoundAnswerEvaluator(numeric).Evaluate(exercise, _compoundAnswers);
            if (result.IsCorrect)
            {
                await context.Accounts.MarkExerciseCompletedAsync(context.Profile.Id, exercise.Id);
                ShowSolution(exercise);
            }
            ShowStatus(result.Message, result.IsCorrect);
        };
        root.Children.Add(check);
    }

    private StackPanel BuildCompoundPart(
        LearningExercise exercise,
        LearningAnswerPart part,
        ExerciseViewContext context)
    {
        var panel = new StackPanel { Spacing = 8 };
        panel.Children.Add(RichContentView.CreateText(part.Prompt));
        if (part.IsMultipleChoice)
        {
            for (var index = 0; index < part.Options.Count; index++)
            {
                var optionNumber = index + 1;
                var radio = new RadioButton
                {
                    GroupName = $"{exercise.Id}-{part.Id}",
                    Content = RichContentView.CreateText(part.Options[index])
                };
                radio.IsCheckedChanged += (_, _) =>
                {
                    if (radio.IsChecked == true)
                        _compoundAnswers[part.Id] = optionNumber.ToString(CultureInfo.InvariantCulture);
                };
                panel.Children.Add(radio);
            }
            return panel;
        }

        var answer = new TextBox
        {
            PlaceholderText = part.IsNumeric ? "Wpisz liczbę lub proste wyrażenie" : "Wpisz odpowiedź"
        };
        AutomationProperties.SetName(answer, $"Odpowiedź do części: {part.Prompt}");
        answer.TextChanged += (_, _) => _compoundAnswers[part.Id] = answer.Text;
        if (context.Clipboard is not null)
            TextBoxClipboardBehavior.Attach(answer, context.Clipboard, message => ShowStatus(message, false));
        panel.Children.Add(answer);
        return panel;
    }

    private void AddRevealControl(StackPanel root, LearningExercise exercise, ExerciseViewContext context)
    {
        var reveal = new Button
        {
            Content = "Pokaż odpowiedź i oznacz jako ukończone",
            Classes = { PrimaryButtonClass },
            HorizontalAlignment = HorizontalAlignment.Left
        };
        AutomationProperties.SetHelpText(
            reveal,
            "Akcja ujawni odpowiedź i zapisze zadanie w lokalnym profilu jako ukończone.");
        reveal.Click += async (_, _) =>
        {
            ShowSolution(exercise);
            await context.Accounts.MarkExerciseCompletedAsync(context.Profile.Id, exercise.Id);
            ShowStatus("Odpowiedź została ujawniona. Zadanie zapisano jako ukończone.", true);
        };
        root.Children.Add(reveal);
    }

    private void AddHintControls(StackPanel root, LearningExercise exercise)
    {
        if (exercise.Hints.Count > 0)
        {
            var hint = new Button { Content = "Następna podpowiedź", Classes = { GhostButtonClass }, HorizontalAlignment = HorizontalAlignment.Left };
            hint.Click += (_, _) =>
            {
                if (_hintIndex >= exercise.Hints.Count) { ShowStatus("To była ostatnia podpowiedź.", true); return; }
                _hintHost.Child = RichContentView.CreateText(exercise.Hints[_hintIndex++]);
            };
            root.Children.Add(hint);
        }
        root.Children.Add(_hintHost);
    }

    private void ShowSolution(LearningExercise exercise)
    {
        var solution = exercise.EffectiveSolution;
        if (string.IsNullOrWhiteSpace(solution))
            return;
        var scoring = string.IsNullOrWhiteSpace(exercise.ScoringCriteria)
            ? string.Empty
            : $"\n\nKryteria punktowania:\n{exercise.ScoringCriteria}";
        _hintHost.Child = RichContentView.CreateText($"Pełne rozwiązanie:\n{solution}{scoring}");
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
        if (exercise.IsCompound)
            return "Uzupełnij wszystkie części odpowiedzi i sprawdź je łącznie.";
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
