using System;
using System.Globalization;

namespace Abituria.Services;

public sealed record CalculatorTextEdit(
    string Text,
    int SelectionStart,
    int SelectionLength,
    bool ShouldCalculate);

public sealed class CalculatorInputState
{
    private double? _resultValue;

    public bool IsAfterResult => _resultValue is not null;

    public void MarkResult(double value)
    {
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value), "Wynik musi być liczbą skończoną.");

        _resultValue = value == 0d ? 0d : value;
    }

    public void Reset() => _resultValue = null;

    public string BeginValue(string currentText)
    {
        if (_resultValue is null) return currentText;

        Reset();
        return string.Empty;
    }

    public string BeginOperator(string currentText)
    {
        if (_resultValue is not double result) return currentText;

        Reset();
        return Format(result);
    }

    public CalculatorTextEdit? CreateRootFromResult(int degree)
    {
        if (_resultValue is not double result) return null;
        if (degree < 2)
            throw new ArgumentOutOfRangeException(nameof(degree), "Stopień pierwiastka musi wynosić co najmniej 2.");

        Reset();
        var operand = Format(result);
        var text = degree switch
        {
            2 => $"√({operand})",
            3 => $"∛({operand})",
            4 => $"∜({operand})",
            _ => $"root({degree};{operand})"
        };
        return new CalculatorTextEdit(text, text.Length, 0, true);
    }

    public CalculatorTextEdit CreateReciprocal(string currentText, int selectionStart, int selectionEnd)
    {
        var source = currentText;
        if (_resultValue is double result)
        {
            source = Format(result);
            selectionStart = 0;
            selectionEnd = source.Length;
        }

        Reset();

        var start = Math.Clamp(Math.Min(selectionStart, selectionEnd), 0, source.Length);
        var end = Math.Clamp(Math.Max(selectionStart, selectionEnd), 0, source.Length);
        var hasSelection = start != end;
        var operand = hasSelection ? source[start..end] : source;

        if (string.IsNullOrWhiteSpace(operand))
            return new CalculatorTextEdit("1/()", 3, 0, false);
        if (!hasSelection && source == "1/()")
            return new CalculatorTextEdit(source, 3, 0, false);

        var reciprocal = $"1/({operand})";
        var text = hasSelection
            ? source[..start] + reciprocal + source[end..]
            : reciprocal;
        var caret = hasSelection ? start + reciprocal.Length : text.Length;
        return new CalculatorTextEdit(text, caret, 0, true);
    }

    private static string Format(double value) => value.ToString("R", CultureInfo.InvariantCulture);
}
