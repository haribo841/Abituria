using Abituria.Services;

namespace Abituria.Tests;

public sealed class CalculatorSessionTests
{
    private readonly CalculatorSession _session = new(new ExpressionCalculator());

    [Fact]
    public void Stores_successful_results_and_exposes_ans()
    {
        Assert.True(_session.Calculate("2+3").Success);
        var result = _session.Calculate("Ans*2");

        Assert.True(result.Success);
        Assert.Equal(10d, result.Value);
        Assert.Equal(10d, _session.LastResult);
        Assert.Equal(new[] { "Ans*2", "2+3" }, _session.History.Select(item => item.Expression));
    }

    [Fact]
    public void Errors_do_not_change_ans_or_history()
    {
        _session.Calculate("6*7");
        var before = _session.History.ToArray();

        var result = _session.Calculate("1/0");

        Assert.False(result.Success);
        Assert.Equal(42d, _session.LastResult);
        Assert.Equal(before, _session.History);
    }

    [Fact]
    public void Keeps_only_the_twenty_newest_results()
    {
        for (var number = 1; number <= 25; number++) _session.Calculate(number.ToString());

        Assert.Equal(CalculatorSession.HistoryLimit, _session.History.Count);
        Assert.Equal("25", _session.History[0].Expression);
        Assert.Equal("6", _session.History[^1].Expression);
    }

    [Fact]
    public void Clearing_history_also_clears_ans()
    {
        _session.Calculate("4+5");

        _session.ClearHistory();

        Assert.Empty(_session.History);
        Assert.Null(_session.LastResult);
        Assert.Equal(CalculationErrorCode.MissingAns, _session.Calculate("Ans").ErrorCode);
    }
}
