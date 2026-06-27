using System;
using System.Collections.Generic;
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
    public ExamOverviewView(ExamDefinition exam, IEnumerable<PlaceholderItem> placeholders, Action openExam, Action<PlaceholderItem> openPlaceholder)
    {
        var root = new StackPanel { Spacing = 14 };
        root.Children.Add(UiFactory.PageTitle("Zadania", "Arkusze i zestawy zachowane ze starszych wersji projektu."));
        var examButton = ListButton($"{exam.Title} — {exam.Exercises.Count} zadań", openExam);
        root.Children.Add(examButton);
        foreach (var placeholder in placeholders.Where(item => item.Category is "exam" or "exercise"))
            root.Children.Add(ListButton($"{placeholder.Title} — treść w przygotowaniu", () => openPlaceholder(placeholder)));
        Content = UiFactory.PageScroll(root);
    }

    private static Button ListButton(string text, Action action)
    {
        var button = new Button { Content = text, Classes = { "list" }, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left };
        button.Click += (_, _) => action();
        return button;
    }
}

public sealed class ExerciseListView : UserControl
{
    private readonly StackPanel _list = new() { Spacing = 8 };

    public ExerciseListView(ExamDefinition exam, LocalProfile profile, AccountService accounts, Action<ExerciseDefinition> open, Action back)
    {
        var root = new StackPanel { Spacing = 16 };
        var backButton = new Button { Content = "← Arkusze", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
        backButton.Click += (_, _) => back();
        root.Children.Add(backButton);
        root.Children.Add(UiFactory.PageTitle(exam.Title, "Zadania 1–28 są zamknięte; zadania 29–35 prowadzą przez rozwiązanie otwarte."));
        root.Children.Add(_list);
        Content = UiFactory.PageScroll(root);
        AttachedToVisualTree += async (_, _) => await LoadAsync(exam, profile, accounts, open);
    }

    private async Task LoadAsync(ExamDefinition exam, LocalProfile profile, AccountService accounts, Action<ExerciseDefinition> open)
    {
        var completed = await accounts.GetCompletedExerciseIdsAsync(profile.Id);
        _list.Children.Clear();
        foreach (var exercise in exam.Exercises.OrderBy(item => item.Number))
        {
            var done = completed.Contains(exercise.Id) ? " ✓" : string.Empty;
            var type = exercise.IsMultipleChoice ? "A–D" : "otwarte";
            var button = new Button
            {
                Content = $"{exercise.Title} · {type}{done}", Classes = { "list" },
                HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left
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

    public ExerciseView(ExerciseDefinition exercise, ExamDefinition exam, LocalProfile profile, AccountService accounts, Action back, Action<ExerciseDefinition> openExercise)
    {
        var root = new StackPanel { Spacing = 16 };
        var topButtons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        var backButton = new Button { Content = "← Lista zadań", Classes = { "ghost" } };
        backButton.Click += (_, _) => back();
        topButtons.Children.Add(backButton);
        var previous = exam.Exercises.FirstOrDefault(item => item.Number == exercise.Number - 1);
        var next = exam.Exercises.FirstOrDefault(item => item.Number == exercise.Number + 1);
        if (previous is not null) topButtons.Children.Add(NavigationButton("←", previous));
        if (next is not null) topButtons.Children.Add(NavigationButton("→", next));
        root.Children.Add(topButtons);
        root.Children.Add(UiFactory.PageTitle(exercise.Title, exercise.IsMultipleChoice ? "Wybierz jedną odpowiedź." : "Rozwiązuj samodzielnie i odsłaniaj kolejne wskazówki."));
        root.Children.Add(UiFactory.Card(new RichContentView([new ContentBlock { Type = "richText", Text = exercise.Prompt }])));
        foreach (var asset in exercise.Assets) root.Children.Add(UiFactory.Card(UiFactory.AssetImage(asset, 820, 470)));

        if (exercise.IsMultipleChoice)
        {
            var options = new StackPanel { Spacing = 8 };
            for (var index = 0; index < exercise.Options.Count; index++)
            {
                var optionNumber = index + 1;
                var radio = new RadioButton
                {
                    GroupName = "exercise-answer", Content = RichContentView.CreateText($"{(char)('A' + index)}. {exercise.Options[index]}")
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
}
