using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Abituria.Services;

public sealed record CalculationHistoryItem(
    string Expression,
    string Result,
    double Value,
    double? AnsInput);

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
        var ansInput = LastResult;
        var evaluation = ExpressionCalculator.EvaluateWithRepeat(expression, ansInput);
        var result = evaluation.Result;
        if (!result.Success || result.Value is null) return result;

        LastResult = result.Value.Value;
        _repeatOperation = evaluation.RepeatOperation;
        AddHistory((result.NormalizedExpression ?? expression!).Trim(), result, ansInput);
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
        AddHistory(expression, result, lastResult);
        return result;
    }

    public CalculationResult RestoreHistory(CalculationHistoryItem? item)
    {
        if (item is null || !_history.Any(candidate => ReferenceEquals(candidate, item)))
            return new CalculationResult(
                false,
                null,
                string.Empty,
                CalculationErrorCode.InvalidHistoryItem,
                "Wybrany wpis nie jest już dostępny w historii sesji.",
                0);

        var evaluation = ExpressionCalculator.EvaluateWithRepeat(item.Expression, item.AnsInput);
        var result = evaluation.Result;
        if (!result.Success || result.Value is null) return result;
        if (!result.Value.Value.Equals(item.Value))
            return new CalculationResult(
                false,
                null,
                string.Empty,
                CalculationErrorCode.InvalidHistoryItem,
                "Nie można odtworzyć wpisu historii z pierwotnym wynikiem.",
                0);

        LastResult = item.Value;
        _repeatOperation = evaluation.RepeatOperation;
        return result;
    }

    public void ClearHistory()
    {
        _history.Clear();
        LastResult = null;
        _repeatOperation = null;
    }

    private void AddHistory(string expression, CalculationResult result, double? ansInput)
    {
        _history.Insert(0, new CalculationHistoryItem(expression, result.DisplayValue, result.Value!.Value, ansInput));
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
