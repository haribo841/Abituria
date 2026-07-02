using System;
using Abituria.Services;
using Abituria.Ui;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace Abituria.Views;

public sealed class GeneralCalculatorView : UserControl
{
    private readonly TextBox _expression = new()
    {
        PlaceholderText = "Np. 2(3+4), sqrt(9) albo root(3; -8)",
        FontSize = 24,
        HorizontalAlignment = HorizontalAlignment.Stretch
    };

    private readonly StackPanel _historyItems = new() { Spacing = 8 };
    private readonly StackPanel _result = new() { Spacing = 6 };
    private readonly CalculatorSession _session;

    public GeneralCalculatorView(CalculatorSession session, Action back)
    {
        _session = session;
        var root = new StackPanel { Spacing = 18 };

        var backButton = new Button { Content = "Wróć do kalkulatorów", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
        backButton.Click += (_, _) => back();
        root.Children.Add(backButton);
        root.Children.Add(UiFactory.PageTitle("Kalkulator ogólny", "Działania podstawowe, nawiasy, potęgi i pierwiastki w jednym wyrażeniu."));
        root.Children.Add(UiFactory.InfoBand(
            "Składnia",
            "Możesz używać przecinka lub kropki, znaków × i ÷ albo * i /. Dostępne są sqrt(x), √x, ∛x, ∜x oraz root(stopień; liczba). Zapis 3√8 oznacza 3 × √8."));

        _expression.KeyDown += (_, eventArgs) =>
        {
            if (eventArgs.Key == Key.Enter)
            {
                Calculate();
                eventArgs.Handled = true;
            }
            else if (eventArgs.Key == Key.Escape)
            {
                ClearExpression();
                eventArgs.Handled = true;
            }
        };

        var workArea = new Grid { ColumnDefinitions = new ColumnDefinitions("3*,2*"), ColumnSpacing = 18 };
        var calculator = new StackPanel { Spacing = 14 };
        calculator.Children.Add(UiFactory.Card(_expression, new Thickness(16)));
        calculator.Children.Add(BuildKeypad());
        ResetResult();
        calculator.Children.Add(UiFactory.Card(_result, new Thickness(18), "#F7FAFC"));
        workArea.Children.Add(calculator);

        var history = BuildHistoryPanel();
        Grid.SetColumn(history, 1);
        workArea.Children.Add(history);
        root.Children.Add(workArea);

        RenderHistory();
        Content = UiFactory.PageScroll(root);
    }

    private Control BuildKeypad()
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*,*,*,*"),
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto,Auto,Auto"),
            ColumnSpacing = 6,
            RowSpacing = 6
        };

        var keys = new (string Label, Action Action, bool Primary)[]
        {
            ("(", () => InsertText("("), false), (")", () => InsertText(")"), false),
            ("Ans", () => InsertText("Ans"), false), ("⌫", Backspace, false), ("C", ClearExpression, false),
            ("7", () => InsertText("7"), false), ("8", () => InsertText("8"), false),
            ("9", () => InsertText("9"), false), ("÷", () => InsertText("÷"), false), ("^", () => InsertText("^"), false),
            ("4", () => InsertText("4"), false), ("5", () => InsertText("5"), false),
            ("6", () => InsertText("6"), false), ("×", () => InsertText("×"), false), ("√", () => InsertTemplate("√()", 2), false),
            ("1", () => InsertText("1"), false), ("2", () => InsertText("2"), false),
            ("3", () => InsertText("3"), false), ("-", () => InsertText("-"), false), ("∛", () => InsertTemplate("∛()", 2), false),
            ("0", () => InsertText("0"), false), (",", () => InsertText(","), false),
            ("ⁿ√", () => InsertTemplate("root(2; )", 5, 1), false), ("+", () => InsertText("+"), false), ("=", Calculate, true)
        };

        for (var index = 0; index < keys.Length; index++)
        {
            var key = keys[index];
            var button = new Button
            {
                Content = key.Label,
                MinHeight = 46,
                FontSize = 17,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            button.Classes.Add(key.Primary ? "primary" : "ghost");
            button.Click += (_, _) => key.Action();
            Grid.SetColumn(button, index % 5);
            Grid.SetRow(button, index / 5);
            grid.Children.Add(button);
        }

        return UiFactory.Card(grid, new Thickness(14));
    }

    private Control BuildHistoryPanel()
    {
        var panel = new StackPanel { Spacing = 12 };
        var header = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto"), ColumnSpacing = 10 };
        header.Children.Add(new TextBlock { Text = "Historia sesji", Classes = { "h2" }, VerticalAlignment = VerticalAlignment.Center });
        var clear = new Button { Content = "Wyczyść historię", Classes = { "ghost" } };
        clear.Click += (_, _) =>
        {
            _session.ClearHistory();
            RenderHistory();
        };
        Grid.SetColumn(clear, 1);
        header.Children.Add(clear);
        panel.Children.Add(header);
        panel.Children.Add(_historyItems);
        return UiFactory.Card(panel, new Thickness(16));
    }

    private void Calculate()
    {
        var calculation = _session.Calculate(_expression.Text);
        ShowResult(calculation);
        if (calculation.Success)
        {
            RenderHistory();
        }
        else if (calculation.ErrorPosition is not null)
        {
            var position = Math.Clamp(calculation.ErrorPosition.Value, 0, (_expression.Text ?? string.Empty).Length);
            _expression.CaretIndex = position;
            _expression.SelectionStart = position;
            _expression.SelectionEnd = position;
            _expression.Focus();
        }
    }

    private void ShowResult(CalculationResult calculation)
    {
        _result.Children.Clear();
        _result.Children.Add(new TextBlock
        {
            Text = calculation.Success ? "Wynik" : "Nie można obliczyć",
            FontSize = 17,
            FontWeight = FontWeight.SemiBold,
            Foreground = UiFactory.Brush(calculation.Success ? "#19733B" : "#B42318")
        });
        _result.Children.Add(new TextBlock
        {
            Text = calculation.Success
                ? calculation.DisplayValue
                : $"{calculation.Message} Pozycja: {(calculation.ErrorPosition ?? 0) + 1}.",
            FontSize = calculation.Success ? 25 : 16,
            TextWrapping = TextWrapping.Wrap
        });
    }

    private void ShowHistoricalResult(CalculationHistoryItem item)
    {
        _result.Children.Clear();
        _result.Children.Add(new TextBlock { Text = "Wynik", FontSize = 17, FontWeight = FontWeight.SemiBold, Foreground = UiFactory.Brush("#19733B") });
        _result.Children.Add(new TextBlock { Text = item.Result, FontSize = 25, TextWrapping = TextWrapping.Wrap });
    }

    private void RenderHistory()
    {
        _historyItems.Children.Clear();
        if (_session.History.Count == 0)
        {
            _historyItems.Children.Add(new TextBlock { Text = "Brak obliczeń w tej sesji.", Classes = { "muted" } });
            return;
        }

        foreach (var item in _session.History)
        {
            var content = new StackPanel { Spacing = 3 };
            content.Children.Add(new TextBlock { Text = item.Expression, FontSize = 15, TextWrapping = TextWrapping.Wrap });
            content.Children.Add(new TextBlock { Text = $"= {item.Result}", Classes = { "muted" } });
            var button = new Button
            {
                Content = content,
                Classes = { "list" },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
            button.Click += (_, _) =>
            {
                _expression.Text = item.Expression;
                _expression.CaretIndex = item.Expression.Length;
                ShowHistoricalResult(item);
                _expression.Focus();
            };
            _historyItems.Children.Add(button);
        }
    }

    private void InsertText(string text) => ReplaceSelection(text, text.Length, 0);

    private void InsertTemplate(string text, int caretOffset, int selectionLength = 0) =>
        ReplaceSelection(text, caretOffset, selectionLength);

    private void ReplaceSelection(string text, int caretOffset, int selectionLength)
    {
        var source = _expression.Text ?? string.Empty;
        var start = Math.Clamp(Math.Min(_expression.SelectionStart, _expression.SelectionEnd), 0, source.Length);
        var end = Math.Clamp(Math.Max(_expression.SelectionStart, _expression.SelectionEnd), 0, source.Length);
        _expression.Text = source[..start] + text + source[end..];
        var caret = start + caretOffset;
        _expression.SelectionStart = caret;
        _expression.SelectionEnd = caret + selectionLength;
        _expression.CaretIndex = caret + selectionLength;
        _expression.Focus();
    }

    private void Backspace()
    {
        var source = _expression.Text ?? string.Empty;
        var start = Math.Clamp(Math.Min(_expression.SelectionStart, _expression.SelectionEnd), 0, source.Length);
        var end = Math.Clamp(Math.Max(_expression.SelectionStart, _expression.SelectionEnd), 0, source.Length);
        if (start == end && start > 0) start--;
        if (start == end) return;

        _expression.Text = source.Remove(start, end - start);
        _expression.CaretIndex = start;
        _expression.SelectionStart = start;
        _expression.SelectionEnd = start;
        _expression.Focus();
    }

    private void ClearExpression()
    {
        _expression.Text = string.Empty;
        ResetResult();
        _expression.Focus();
    }

    private void ResetResult()
    {
        _result.Children.Clear();
        _result.Children.Add(new TextBlock { Text = "Wynik pojawi się tutaj.", Classes = { "muted" } });
    }
}
