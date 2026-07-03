using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Abituria.Services;

public sealed record CalculationHistoryItem(string Expression, string Result, double Value);

public sealed class CalculatorSession
{
    public const int HistoryLimit = 20;

    private readonly ExpressionCalculator _calculator;
    private readonly List<CalculationHistoryItem> _history = [];
    private readonly ReadOnlyCollection<CalculationHistoryItem> _readOnlyHistory;
    private RepeatOperation? _repeatOperation;

    public CalculatorSession(ExpressionCalculator calculator)
    {
        _calculator = calculator;
        _readOnlyHistory = _history.AsReadOnly();
    }

    public double? LastResult { get; private set; }
    public IReadOnlyList<CalculationHistoryItem> History => _readOnlyHistory;

    public CalculationResult Calculate(string? expression)
    {
        var evaluation = _calculator.EvaluateWithRepeat(expression, LastResult);
        var result = evaluation.Result;
        if (!result.Success || result.Value is null) return result;

        LastResult = result.Value.Value;
        _repeatOperation = evaluation.RepeatOperation;
        AddHistory(expression!.Trim(), result);
        return result;
    }

    public CalculationResult RepeatLast()
    {
        if (LastResult is not double lastResult)
            return _calculator.Evaluate(null);

        if (_repeatOperation is null)
            return _calculator.Evaluate(Format(lastResult), lastResult);

        var expression = $"({Format(lastResult)}){Symbol(_repeatOperation.Operator)}({Format(_repeatOperation.Operand)})";
        var result = _calculator.Evaluate(expression, lastResult);
        if (!result.Success || result.Value is null) return result;

        LastResult = result.Value.Value;
        AddHistory(expression, result);
        return result;
    }

    public void ClearHistory()
    {
        _history.Clear();
        LastResult = null;
        _repeatOperation = null;
    }

    private void AddHistory(string expression, CalculationResult result)
    {
        _history.Insert(0, new CalculationHistoryItem(expression, result.DisplayValue, result.Value!.Value));
        if (_history.Count > HistoryLimit) _history.RemoveAt(_history.Count - 1);
    }

    private static string Format(double value) => value.ToString("R", CultureInfo.InvariantCulture);

    private static string Symbol(RepeatOperator operation) => operation switch
    {
        RepeatOperator.Add => "+",
        RepeatOperator.Subtract => "-",
        RepeatOperator.Multiply => "*",
        RepeatOperator.Divide => "/",
        RepeatOperator.Power => "^",
        _ => throw new ArgumentOutOfRangeException(nameof(operation))
    };
}
