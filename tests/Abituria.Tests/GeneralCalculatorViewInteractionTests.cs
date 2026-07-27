using Abituria.Data;
using Abituria.Services;
using Abituria.Views;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace Abituria.Tests;

public sealed class GeneralCalculatorViewInteractionTests
{
    [AvaloniaFact]
    public void Keypad_calculates_repeats_restores_history_and_returns()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        var backCalls = 0;
        var view = new GeneralCalculatorView(
            session,
            new ContentRepository().UiCopy,
            () => backCalls++);
        var window = Show(view);

        try
        {
            Click(view, "2");
            Click(view, "+");
            Click(view, "3");
            Click(view, "=");

            Assert.Equal("2+3", Expression(view).Text);
            Assert.Equal(5, session.LastResult);
            Assert.Contains(ResultTexts(view), text => text == "5");

            Click(view, "=");
            Assert.Equal("8", Expression(view).Text);
            Assert.Equal(8, session.LastResult);
            Assert.Equal(2, session.History.Count);

            var restoredItem = session.History[1];
            var historyButton = view.GetLogicalDescendants()
                .OfType<Button>()
                .Single(button => button.Content is StackPanel panel &&
                    panel.GetLogicalDescendants().OfType<TextBlock>().Any(text => text.Text == restoredItem.Expression));
            Click(historyButton);

            Assert.Equal(restoredItem.Expression, Expression(view).Text);
            Assert.Equal(restoredItem.Value, session.LastResult);

            Click(view, "Wyczyść historię");
            Assert.Empty(session.History);
            Assert.Contains(
                view.GetLogicalDescendants().OfType<TextBlock>(),
                text => text.Text == "Brak obliczeń w tej sesji.");

            Click(view, "Wróć do kalkulatorów");
            Assert.Equal(1, backCalls);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Keypad_edits_selection_and_executes_templates()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        var view = new GeneralCalculatorView(session, new ContentRepository().UiCopy, () => { });
        var window = Show(view);

        try
        {
            var expression = Expression(view);

            SetExpression(expression, "123", 1, 1);
            Click(view, "9");
            Assert.Equal("193", expression.Text);

            SetExpression(expression, "193", 1, 1);
            Click(view, "+");
            Assert.Equal("1+3", expression.Text);

            SetExpression(expression, "123", 1, 1);
            Click(view, "⌫");
            Assert.Equal("13", expression.Text);
            Click(view, "⌫");
            Assert.Equal("3", expression.Text);
            SetExpression(expression, string.Empty, 0);
            Click(view, "⌫");
            Assert.Equal(string.Empty, expression.Text);

            Click(view, "√");
            Assert.Equal("√()", expression.Text);
            Assert.Equal(2, expression.SelectionStart);

            Click(view, "C");
            Click(view, "ⁿ√  Pierwiastek n-tego stopnia");
            Assert.Equal("root(2; )", expression.Text);
            Assert.Equal(5, expression.SelectionStart);
            Assert.Equal(6, expression.SelectionEnd);

            Click(view, "C");
            Click(view, "1/x");
            Assert.Equal("1/()", expression.Text);
            Click(view, "1/x");
            Assert.Equal("1/()", expression.Text);

            Click(view, "C");
            Click(view, "x²");
            Assert.Equal("()^2", expression.Text);
            Click(view, "x²");
            Assert.Equal("()^2", expression.Text);

            SetExpression(expression, "8", 0, 1);
            Click(view, "1/x");
            Assert.Equal(0.125, session.LastResult);

            Click(view, "x²");
            Assert.Equal(0.015625, session.LastResult);

            Click(view, "C");
            SetExpression(expression, "9", 1);
            Click(view, "=");
            Click(view, "√");
            Assert.Equal(3, session.LastResult);

            Click(view, "C");
            SetExpression(expression, "8", 1);
            Click(view, "=");
            Click(view, "∛");
            Assert.Equal(2, session.LastResult);

            Click(view, "C");
            Assert.Equal(string.Empty, expression.Text);
            Assert.Contains(ResultTexts(view), text => text == "Wynik pojawi się tutaj.");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Keyboard_and_text_input_cover_success_error_and_normalization_states()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        var view = new GeneralCalculatorView(session, new ContentRepository().UiCopy, () => { });
        var window = Show(view);

        try
        {
            var expression = Expression(view);

            SetExpression(expression, "2+", 2);
            Click(view, "=");
            Assert.Contains(ResultTexts(view), text => text == "Nie można obliczyć");
            RaiseTextInput(expression, "7");
            Assert.Equal("2+7", expression.Text);

            SetExpression(expression, "2+3", 3);
            RaiseKey(expression, Key.Enter);
            Assert.Equal(5, session.LastResult);

            RaiseTextInput(expression, "+");
            Assert.Equal("5+", expression.Text);

            SetExpression(expression, "4+5", 3);
            RaiseKey(expression, Key.Enter);
            RaiseTextInput(expression, "7");
            Assert.Equal("7", expression.Text);

            RaiseKey(expression, Key.Escape);
            Assert.Equal(string.Empty, expression.Text);

            RaiseTextInput(expression, "0");
            RaiseTextInput(expression, "5");
            Assert.Equal("5", expression.Text);
            Assert.Contains(ResultTexts(view), text => text == "Poprawiono zapis");
            Click(view, "=");
            Assert.Contains(ResultTexts(view), text => text == ExpressionCalculator.LeadingZeroNormalizationMessage);

            RaiseKey(expression, Key.Back);
            RaiseKey(expression, Key.Delete);
            RaiseKey(expression, Key.V, KeyModifiers.Control);
            Assert.Equal(string.Empty, expression.Text);

            RaiseTextInput(expression, string.Empty);
            Assert.Equal(string.Empty, expression.Text);
        }
        finally
        {
            window.Close();
        }
    }

    private static Window Show(Control content)
    {
        var window = new Window { Width = 1100, Height = 760, Content = content };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static TextBox Expression(Control view) =>
        view.GetLogicalDescendants()
            .OfType<TextBox>()
            .Single(textBox => AutomationProperties.GetName(textBox) == "Wyrażenie matematyczne");

    private static IEnumerable<string> ResultTexts(Control view) =>
        view.GetLogicalDescendants()
            .OfType<TextBlock>()
            .Where(text => text.Text is not null)
            .Select(text => text.Text!);

    private static void SetExpression(TextBox expression, string text, int selectionStart, int selectionLength = 0)
    {
        expression.Text = text;
        expression.CaretIndex = selectionStart + selectionLength;
        expression.SelectionStart = selectionStart;
        expression.SelectionEnd = selectionStart + selectionLength;
    }

    private static void Click(Control view, string content)
    {
        var button = view.GetLogicalDescendants()
            .OfType<Button>()
            .Single(control => string.Equals(control.Content as string, content, StringComparison.Ordinal));
        Click(button);
    }

    private static void Click(Button button)
    {
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Dispatcher.UIThread.RunJobs();
    }

    private static KeyEventArgs RaiseKey(TextBox expression, Key key, KeyModifiers modifiers = KeyModifiers.None)
    {
        var eventArgs = new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = key,
            KeyModifiers = modifiers
        };
        expression.RaiseEvent(eventArgs);
        Dispatcher.UIThread.RunJobs();
        return eventArgs;
    }

    private static TextInputEventArgs RaiseTextInput(TextBox expression, string text)
    {
        var eventArgs = new TextInputEventArgs
        {
            RoutedEvent = InputElement.TextInputEvent,
            Text = text
        };
        expression.RaiseEvent(eventArgs);
        Dispatcher.UIThread.RunJobs();
        return eventArgs;
    }
}
