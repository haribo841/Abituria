using System;
using Abituria.Models;
using Abituria.Services;
using Abituria.Ui;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace Abituria.Views;

public sealed class GeneralCalculatorView : UserControl
{
    private const string GhostButtonClass = "ghost";

    private readonly TextBox _expression = new()
    {
        PlaceholderText = "Np. 2(3+4), sqrt(9) albo root(3; -8)",
        FontSize = 24,
        HorizontalAlignment = HorizontalAlignment.Stretch
    };

    private readonly StackPanel _historyItems = new() { Spacing = 8 };
    private readonly StackPanel _result = new() { Spacing = 6 };
    private readonly CalculatorInputState _inputState = new();
    private readonly CalculatorSession _session;
    private bool _inputWasNormalized;
    private bool _repeatOnNextEquals;

    public GeneralCalculatorView(CalculatorSession session, UiCopyCatalog copy, Action back)
    {
        _session = session;
        AutomationProperties.SetName(_expression, "Wyrażenie matematyczne");
        AutomationProperties.SetLiveSetting(_result, AutomationLiveSetting.Polite);
        AutomationProperties.SetName(_result, "Wynik kalkulatora");
        var root = new StackPanel { Spacing = 18 };

        var backButton = new Button { Content = "Wróć do kalkulatorów", Classes = { GhostButtonClass }, HorizontalAlignment = HorizontalAlignment.Left };
        backButton.Click += (_, _) => back();
        root.Children.Add(backButton);
        root.Children.Add(UiFactory.PageTitle("Kalkulator ogólny", "Działania podstawowe, nawiasy, potęgi i pierwiastki w jednym wyrażeniu."));
        root.Children.Add(UiFactory.InfoBand(copy.GetRequired("calculator.general.syntax")));

        _expression.AddHandler(InputElement.TextInputEvent, OnTextInput, RoutingStrategies.Tunnel);
        _expression.KeyDown += OnExpressionKeyDown;

        var workArea = new Grid
        {
            Name = "CalculatorLayoutRoot",
            ColumnDefinitions = new ColumnDefinitions("3*,2*"),
            RowDefinitions = new RowDefinitions("Auto"),
            ColumnSpacing = 18
        };
        var calculator = new StackPanel { Spacing = 14 };
        calculator.Children.Add(UiFactory.Card(_expression, new Thickness(16)));
        calculator.Children.Add(BuildKeypad());
        ResetResult();
        calculator.Children.Add(UiFactory.Card(_result, new Thickness(18), "SurfaceAltBrush"));
        workArea.Children.Add(calculator);

        var history = BuildHistoryPanel();
        Grid.SetColumn(history, 1);
        workArea.Children.Add(history);

        AdaptiveLayout.ObserveWidth(this, 900, isCompact =>
        {
            workArea.ColumnDefinitions = new ColumnDefinitions(isCompact ? "*" : "3*,2*");
            workArea.RowDefinitions = new RowDefinitions(isCompact ? "Auto,Auto" : "Auto");
            workArea.ColumnSpacing = isCompact ? 0 : 18;
            workArea.RowSpacing = isCompact ? 18 : 0;

            Grid.SetColumn(calculator, 0);
            Grid.SetRow(calculator, 0);
            Grid.SetColumn(history, isCompact ? 0 : 1);
            Grid.SetRow(history, isCompact ? 1 : 0);
        });

        root.Children.Add(workArea);

        RenderHistory();
        Content = UiFactory.PageScroll(root);
    }

    private Border BuildKeypad()
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*,*,*,*"),
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto,Auto,Auto,Auto"),
            ColumnSpacing = 6,
            RowSpacing = 6
        };

        var keys = new (string Label, Action Action, bool Primary)[]
        {
            ("(", () => InsertValueText("("), false), (")", () => InsertValueText(")"), false),
            ("Ans", () => InsertValueText("Ans"), false), ("⌫", Backspace, false), ("C", ClearExpression, false),
            ("7", () => InsertValueText("7"), false), ("8", () => InsertValueText("8"), false),
            ("9", () => InsertValueText("9"), false), ("÷", () => InsertOperatorText("÷"), false), ("^", () => InsertOperatorText("^"), false),
            ("4", () => InsertValueText("4"), false), ("5", () => InsertValueText("5"), false),
            ("6", () => InsertValueText("6"), false), ("×", () => InsertOperatorText("×"), false), ("√", () => InsertRoot(2, "√()", 2), false),
            ("1", () => InsertValueText("1"), false), ("2", () => InsertValueText("2"), false),
            ("3", () => InsertValueText("3"), false), ("-", () => InsertOperatorText("-"), false), ("∛", () => InsertRoot(3, "∛()", 2), false),
            ("0", () => InsertValueText("0"), false), (",", () => InsertValueText(","), false),
            ("1/x", CalculateReciprocal, false), ("+", () => InsertOperatorText("+"), false), ("=", Calculate, true)
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
            AutomationProperties.SetName(button, AccessibleKeyName(key.Label));
            button.Classes.Add(key.Primary ? "primary" : GhostButtonClass);
            button.Click += (_, _) => key.Action();
            Grid.SetColumn(button, index % 5);
            Grid.SetRow(button, index / 5);
            grid.Children.Add(button);
        }

        var generalRoot = new Button
        {
            Content = "ⁿ√  Pierwiastek n-tego stopnia",
            MinHeight = 42,
            FontSize = 16,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Classes = { GhostButtonClass }
        };
        AutomationProperties.SetName(generalRoot, "Pierwiastek n-tego stopnia");
        generalRoot.Click += (_, _) => InsertValueTemplate("root(2; )", 5, 1);
        Grid.SetRow(generalRoot, 5);
        Grid.SetColumn(generalRoot, 2);
        Grid.SetColumnSpan(generalRoot, 3);
        grid.Children.Add(generalRoot);

        var square = new Button
        {
            Content = "x²",
            MinHeight = 42,
            FontSize = 17,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Classes = { GhostButtonClass }
        };
        AutomationProperties.SetName(square, "Podnieś wynik do kwadratu");
        square.Click += (_, _) => CalculateSquare();
        Grid.SetRow(square, 5);
        Grid.SetColumnSpan(square, 2);
        grid.Children.Add(square);

        return UiFactory.Card(grid, new Thickness(14));
    }

    private static string AccessibleKeyName(string label) => label switch
    {
        "(" => "Lewy nawias",
        ")" => "Prawy nawias",
        "Ans" => "Poprzedni wynik Ans",
        "⌫" => "Usuń ostatni znak",
        "C" => "Wyczyść wyrażenie",
        "÷" => "Dzielenie",
        "^" => "Potęgowanie",
        "×" => "Mnożenie",
        "√" => "Pierwiastek kwadratowy",
        "-" => "Odejmowanie",
        "∛" => "Pierwiastek sześcienny",
        "," => "Przecinek dziesiętny",
        "1/x" => "Odwrotność liczby",
        "+" => "Dodawanie",
        "=" => "Oblicz wynik",
        _ => $"Cyfra {label}"
    };

    private Border BuildHistoryPanel()
    {
        var panel = new StackPanel { Spacing = 12 };
        var header = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto"), ColumnSpacing = 10 };
        header.Children.Add(new TextBlock { Text = "Historia sesji", Classes = { "h2" }, VerticalAlignment = VerticalAlignment.Center });
        var clear = new Button { Content = "Wyczyść historię", Classes = { GhostButtonClass } };
        clear.Click += (_, _) =>
        {
            _session.ClearHistory();
            _repeatOnNextEquals = false;
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
        var inputWasNormalized = _inputWasNormalized;
        _inputWasNormalized = false;
        var repeating = _repeatOnNextEquals;
        _repeatOnNextEquals = false;
        var calculation = repeating
            ? _session.RepeatLast()
            : _session.Calculate(_expression.Text);
        if (calculation.Success && inputWasNormalized && !calculation.WasNormalized)
        {
            calculation = calculation with
            {
                Message = ExpressionCalculator.LeadingZeroNormalizationMessage,
                NormalizedExpression = _expression.Text,
                WasNormalized = true
            };
        }
        ShowResult(calculation);
        if (calculation.Success && calculation.Value is double value)
        {
            if (repeating) SetExpression(calculation.DisplayValue, calculation.DisplayValue.Length);
            else if (calculation.WasNormalized && calculation.NormalizedExpression is not null)
                SetExpression(calculation.NormalizedExpression, calculation.NormalizedExpression.Length);
            _inputState.MarkResult(value);
            _repeatOnNextEquals = true;
            RenderHistory();
        }
        else
        {
            _inputState.MarkError();
            if (calculation.ErrorPosition is not null)
            {
                var position = Math.Clamp(calculation.ErrorPosition.Value, 0, (_expression.Text ?? string.Empty).Length);
                _expression.CaretIndex = position;
                _expression.SelectionStart = position;
                _expression.SelectionEnd = position;
                _expression.Focus();
            }
        }
    }

    private void ShowResult(CalculationResult calculation)
    {
        _result.Children.Clear();
        var heading = new TextBlock
        {
            Text = calculation.Success ? "Wynik" : "Nie można obliczyć",
            FontSize = 17,
            FontWeight = FontWeight.SemiBold
        };
        UiFactory.UseResource(heading, TextBlock.ForegroundProperty, calculation.Success ? "SuccessBrush" : "ErrorBrush");
        _result.Children.Add(heading);
        _result.Children.Add(new TextBlock
        {
            Text = calculation.Success
                ? calculation.DisplayValue
                : $"{calculation.Message} Pozycja: {(calculation.ErrorPosition ?? 0) + 1}.",
            FontSize = calculation.Success ? 25 : 16,
            TextWrapping = TextWrapping.Wrap
        });
        if (calculation.Success && calculation.WasNormalized)
        {
            var normalization = new TextBlock
            {
                Text = calculation.Message,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap
            };
            UiFactory.UseResource(normalization, TextBlock.ForegroundProperty, "WarningBrush");
            _result.Children.Add(normalization);
        }
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
                _inputWasNormalized = false;
                SetExpression(item.Expression, item.Expression.Length);
                var restored = _session.RestoreHistory(item);
                ShowResult(restored);
                if (restored.Success && restored.Value is double value)
                {
                    _inputState.MarkResult(value);
                    _repeatOnNextEquals = true;
                }
                else
                {
                    _inputState.MarkError();
                    _repeatOnNextEquals = false;
                }
            };
            _historyItems.Children.Add(button);
        }
    }

    private void InsertValueText(string text)
    {
        PrepareForValueInput();
        ApplyNormalizedInsertion(text);
    }

    private void InsertOperatorText(string text)
    {
        PrepareForOperatorInput();
        ReplaceSelection(text, text.Length, 0);
    }

    private void InsertValueTemplate(string text, int caretOffset, int selectionLength = 0)
    {
        PrepareForValueInput();
        ReplaceSelection(text, caretOffset, selectionLength);
    }

    private void InsertRoot(int degree, string template, int caretOffset)
    {
        var edit = _inputState.CreateRootFromResult(degree);
        if (edit is null)
        {
            InsertValueTemplate(template, caretOffset);
            return;
        }

        ApplyEdit(edit);
        _repeatOnNextEquals = false;
        Calculate();
    }

    private void CalculateReciprocal()
    {
        var edit = _inputState.CreateReciprocal(
            _expression.Text ?? string.Empty,
            _expression.SelectionStart,
            _expression.SelectionEnd);
        ApplyEdit(edit);
        if (edit.ShouldCalculate)
        {
            _repeatOnNextEquals = false;
            Calculate();
        }
    }

    private void CalculateSquare()
    {
        var edit = _inputState.CreateSquare(
            _expression.Text ?? string.Empty,
            _expression.SelectionStart,
            _expression.SelectionEnd);
        ApplyEdit(edit);
        if (edit.ShouldCalculate)
        {
            _repeatOnNextEquals = false;
            Calculate();
        }
    }

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
        _inputState.Reset();
        _repeatOnNextEquals = false;
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
        _inputState.Reset();
        _inputWasNormalized = false;
        _repeatOnNextEquals = false;
        _expression.Text = string.Empty;
        ResetResult();
        _expression.Focus();
    }

    private void OnExpressionKeyDown(object? sender, KeyEventArgs eventArgs)
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
        else if (eventArgs.Key is Key.Back or Key.Delete)
        {
            _inputState.Reset();
            _repeatOnNextEquals = false;
        }
        else if (eventArgs.Key == Key.V && eventArgs.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            PrepareForValueInput();
        }
    }

    private void OnTextInput(object? sender, TextInputEventArgs eventArgs)
    {
        if (string.IsNullOrEmpty(eventArgs.Text)) return;
        if (_inputState.IsAfterError)
        {
            _inputState.Reset();
            _repeatOnNextEquals = false;
        }
        else if (_inputState.IsAfterResult)
        {
            if (IsBinaryOperator(eventArgs.Text)) PrepareForOperatorInput();
            else PrepareForValueInput();
        }

        _repeatOnNextEquals = false;
        ApplyNormalizedInsertion(eventArgs.Text);
        eventArgs.Handled = true;
    }

    private void PrepareForValueInput()
    {
        if (_inputState.IsAfterError)
        {
            _inputState.Reset();
            _repeatOnNextEquals = false;
            return;
        }
        if (!_inputState.IsAfterResult) return;

        _repeatOnNextEquals = false;
        var text = _inputState.BeginValue(_expression.Text ?? string.Empty);
        SetExpression(text, text.Length);
    }

    private void PrepareForOperatorInput()
    {
        if (_inputState.IsAfterError)
        {
            _inputState.Reset();
            _repeatOnNextEquals = false;
            return;
        }
        if (!_inputState.IsAfterResult) return;

        _repeatOnNextEquals = false;
        var text = _inputState.BeginOperator(_expression.Text ?? string.Empty);
        SetExpression(text, text.Length);
    }

    private void ApplyEdit(CalculatorTextEdit edit) =>
        SetExpression(edit.Text, edit.SelectionStart, edit.SelectionLength);

    private void ApplyNormalizedInsertion(string text)
    {
        var edit = CalculatorInputState.CreateNormalizedInsertion(
            _expression.Text ?? string.Empty,
            _expression.SelectionStart,
            _expression.SelectionEnd,
            text);
        ApplyEdit(edit);
        if (!edit.WasNormalized) return;

        _inputWasNormalized = true;
        ShowInputNormalization();
    }

    private void SetExpression(string text, int selectionStart, int selectionLength = 0)
    {
        var start = Math.Clamp(selectionStart, 0, text.Length);
        var end = Math.Clamp(start + selectionLength, start, text.Length);
        _expression.Text = text;
        _expression.SelectionStart = start;
        _expression.SelectionEnd = end;
        _expression.CaretIndex = end;
        _expression.Focus();
    }

    private static bool IsBinaryOperator(string text) => text is "+" or "-" or "*" or "/" or ":" or "^" or "×" or "÷";

    private void ResetResult()
    {
        _result.Children.Clear();
        _result.Children.Add(new TextBlock { Text = "Wynik pojawi się tutaj.", Classes = { "muted" } });
    }

    private void ShowInputNormalization()
    {
        _result.Children.Clear();
        var heading = new TextBlock
        {
            Text = "Poprawiono zapis",
            FontSize = 17,
            FontWeight = FontWeight.SemiBold
        };
        UiFactory.UseResource(heading, TextBlock.ForegroundProperty, "WarningBrush");
        _result.Children.Add(heading);
        _result.Children.Add(new TextBlock
        {
            Text = ExpressionCalculator.LeadingZeroNormalizationMessage,
            FontSize = 15,
            TextWrapping = TextWrapping.Wrap
        });
    }
}
