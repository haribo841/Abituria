using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abituria.Models;
using Abituria.Services;
using Abituria.Ui;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Abituria.Views;

public sealed class ExamOverviewView : UserControl
{
    public ExamOverviewView(
        ExamDefinition exam,
        IEnumerable<PlaceholderItem> placeholders,
        Action openExam,
        Action<string> openTopic,
        Action<PlaceholderItem> openPlaceholder)
    {
        var root = new StackPanel { Spacing = 14 };
        root.Children.Add(UiFactory.PageTitle("Zadania", "Pracuj z całym arkuszem albo wybierz zadania według tematu."));

        var tabs = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        var archivesTab = new Button { Content = "Arkusze", MinWidth = 120 };
        var topicsTab = new Button { Content = "Tematy", MinWidth = 120 };
        tabs.Children.Add(archivesTab);
        tabs.Children.Add(topicsTab);
        root.Children.Add(tabs);

        var content = new ContentControl();
        root.Children.Add(content);
        void ShowArchives()
        {
            SetSelected(archivesTab, true);
            SetSelected(topicsTab, false);
            var panel = new StackPanel { Spacing = 10 };
            if (exam.Introduction.Count > 0) panel.Children.Add(UiFactory.Card(new RichContentView(exam.Introduction)));
            panel.Children.Add(ListButton($"{exam.Title} - {ExerciseCountLabel(exam.Exercises.Count)}", openExam));
            foreach (var placeholder in placeholders.Where(item => item.Category is "exam" or "exercise"))
                panel.Children.Add(ListButton($"{placeholder.Title} - treść w przygotowaniu", () => openPlaceholder(placeholder)));
            content.Content = panel;
        }

        void ShowTopics()
        {
            SetSelected(archivesTab, false);
            SetSelected(topicsTab, true);
            var panel = new StackPanel { Spacing = 10 };
            if (exam.TopicIntroduction.Count > 0) panel.Children.Add(UiFactory.Card(new RichContentView(exam.TopicIntroduction)));
            foreach (var topic in exam.Topics)
                panel.Children.Add(ListButton($"{topic.Title} - {ExerciseCountLabel(topic.ExerciseNumbers.Count)}", () => openTopic(topic.Id)));
            content.Content = panel;
        }

        archivesTab.Click += (_, _) => ShowArchives();
        topicsTab.Click += (_, _) => ShowTopics();
        ShowArchives();
        Content = UiFactory.PageScroll(root);
    }

    private static void SetSelected(Button button, bool selected)
    {
        button.Classes.Clear();
        button.Classes.Add(selected ? "primary" : "ghost");
    }

    private static Button ListButton(string text, Action action)
    {
        var button = new Button { Content = text, Classes = { "list" }, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left };
        button.Click += (_, _) => action();
        return button;
    }

    private static string ExerciseCountLabel(int count)
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

    public ExerciseListView(ExamDefinition exam, string? topicId, LocalProfile profile, AccountService accounts, Action<ExerciseDefinition> open, Action back)
    {
        var root = new StackPanel { Spacing = 16 };
        var backButton = new Button { Content = "← Arkusze", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
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

    private async Task LoadAsync(ExamDefinition exam, ExerciseTopicDefinition? topic, LocalProfile profile, AccountService accounts, Action<ExerciseDefinition> open)
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

public sealed class ExerciseView : UserControl
{
    private readonly TextBlock _status = new() { TextWrapping = TextWrapping.Wrap };
    private readonly Border _hintHost = UiFactory.Card(new TextBlock { Text = "Podpowiedź pojawi się tutaj.", Classes = { "muted" } }, new Thickness(16), "#F7FAFC");
    private int _hintIndex;
    private int? _selectedOption;

    public ExerciseView(ExerciseDefinition exercise, IReadOnlyList<ExerciseDefinition> exerciseContext, SourceDocument source, LocalProfile profile, AccountService accounts, Action back, Action<ExerciseDefinition> openExercise)
    {
        var root = new StackPanel { Spacing = 16 };
        var topButtons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        var backButton = new Button { Content = "← Lista zadań", Classes = { "ghost" } };
        backButton.Click += (_, _) => back();
        topButtons.Children.Add(backButton);
        var currentIndex = exerciseContext.ToList().FindIndex(item => item.Id == exercise.Id);
        var previous = currentIndex > 0 ? exerciseContext[currentIndex - 1] : null;
        var next = currentIndex >= 0 && currentIndex < exerciseContext.Count - 1 ? exerciseContext[currentIndex + 1] : null;
        if (previous is not null) topButtons.Children.Add(NavigationButton("←", previous));
        if (next is not null) topButtons.Children.Add(NavigationButton("→", next));
        root.Children.Add(topButtons);
        root.Children.Add(UiFactory.PageTitle(exercise.Title, exercise.IsMultipleChoice ? "Wybierz jedną odpowiedź." : "Rozwiązuj samodzielnie i odsłaniaj kolejne wskazówki."));
        root.Children.Add(UiFactory.Card(new RichContentView([new ContentBlock { Type = "richText", Text = exercise.Prompt }])));
        foreach (var asset in exercise.Assets) root.Children.Add(UiFactory.Card(UiFactory.AssetImage(asset, 820, 470)));

        var scratchpad = new TextBox
        {
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 130,
            PlaceholderText = "Zapisz tutaj własne obliczenia. Brudnopis nie jest zapisywany po opuszczeniu zadania."
        };
        var scratchPanel = new StackPanel { Spacing = 8 };
        scratchPanel.Children.Add(new TextBlock { Text = "Brudnopis", FontSize = 18, FontWeight = FontWeight.SemiBold });
        scratchPanel.Children.Add(scratchpad);
        root.Children.Add(UiFactory.Card(scratchPanel));

        if (exercise.IsMultipleChoice)
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
            check.Click += async (_, _) =>
            {
                if (_selectedOption is null) { ShowStatus("Najpierw wybierz odpowiedź.", false); return; }
                if (_selectedOption == exercise.CorrectOption)
                {
                    await accounts.MarkExerciseCompletedAsync(profile.Id, exercise.Id);
                    ShowStatus("Poprawna odpowiedź. Zadanie zapisano jako ukończone.", true);
                }
                else ShowStatus("To nie jest poprawna odpowiedź. Skorzystaj z podpowiedzi i spróbuj ponownie.", false);
            };
            root.Children.Add(check);
        }
        else
        {
            var reveal = new Button { Content = "Pokaż odpowiedź", Classes = { "primary" }, HorizontalAlignment = HorizontalAlignment.Left };
            reveal.Click += async (_, _) =>
            {
                _hintHost.Child = RichContentView.CreateText(exercise.RevealedAnswer ?? "Brak zapisanej odpowiedzi.");
                await accounts.MarkExerciseCompletedAsync(profile.Id, exercise.Id);
                ShowStatus("Odpowiedź została ujawniona. Zadanie zapisano jako ukończone.", true);
            };
            root.Children.Add(reveal);
        }

        var hint = new Button { Content = "Następna podpowiedź", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
        hint.Click += (_, _) =>
        {
            if (_hintIndex >= exercise.Hints.Count) { ShowStatus("To była ostatnia podpowiedź.", true); return; }
            _hintHost.Child = RichContentView.CreateText(exercise.Hints[_hintIndex++]);
        };
        root.Children.Add(hint);
        root.Children.Add(_hintHost);
        root.Children.Add(UiFactory.InfoBand("Źródło", $"{exercise.VerificationSource}, strona {exercise.SourcePage}. Treść i klucz odpowiedzi zweryfikowano {FormatVerifiedOn(source.VerifiedOn)}."));
        root.Children.Add(_status);
        Content = UiFactory.PageScroll(root);

        Button NavigationButton(string label, ExerciseDefinition target)
        {
            var button = new Button { Content = label, Classes = { "ghost" } };
            ToolTip.SetTip(button, target.Title);
            button.Click += (_, _) => openExercise(target);
            return button;
        }
    }

    private void ShowStatus(string message, bool success)
    {
        _status.Text = message;
        _status.Foreground = UiFactory.Brush(success ? "#19733B" : "#B42318");
    }

    private static string FormatVerifiedOn(string value) =>
        DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date.ToString("d MMMM yyyy 'r'", CultureInfo.GetCultureInfo("pl-PL"))
            : value;
}
