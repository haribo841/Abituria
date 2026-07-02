using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Abituria.Services;

public sealed record CalculationHistoryItem(string Expression, string Result, double Value);

public sealed class CalculatorSession
{
    public const int HistoryLimit = 20;

    private readonly ExpressionCalculator _calculator;
    private readonly List<CalculationHistoryItem> _history = [];
    private readonly ReadOnlyCollection<CalculationHistoryItem> _readOnlyHistory;

    public CalculatorSession(ExpressionCalculator calculator)
    {
        _calculator = calculator;
        _readOnlyHistory = _history.AsReadOnly();
    }

    public double? LastResult { get; private set; }
    public IReadOnlyList<CalculationHistoryItem> History => _readOnlyHistory;

    public CalculationResult Calculate(string? expression)
    {
        var result = _calculator.Evaluate(expression, LastResult);
        if (!result.Success || result.Value is null) return result;

        LastResult = result.Value.Value;
        _history.Insert(0, new CalculationHistoryItem(expression!.Trim(), result.DisplayValue, result.Value.Value));
        if (_history.Count > HistoryLimit) _history.RemoveAt(_history.Count - 1);
        return result;
    }

    public void ClearHistory()
    {
        _history.Clear();
        LastResult = null;
    }
}
