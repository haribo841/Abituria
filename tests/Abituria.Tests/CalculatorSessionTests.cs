using Abituria.Services;

namespace Abituria.Tests;

public sealed class CalculatorSessionTests
{
    private static readonly string[] InitialHistoryExpressions = ["Ans*2", "2+3"];

    private readonly CalculatorSession _session = new(new ExpressionCalculator());

    [Fact]
    public void Stores_successful_results_and_exposes_ans()
    {
        Assert.True(_session.Calculate("2+3").Success);
        var result = _session.Calculate("Ans*2");

        Assert.True(result.Success);
        Assert.Equal(10d, result.Value);
        Assert.Equal(10d, _session.LastResult);
        Assert.Equal(InitialHistoryExpressions, _session.History.Select(item => item.Expression));
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

    [Fact]
    public void Ans_composes_with_every_operator_and_root_function()
    {
        var seeds = new (string Expression, double Value)[]
        {
            ("3", 3d), ("-3", -3d), ("1,5", 1.5d), ("√2", Math.Sqrt(2d)), ("1E-10", 1E-10d)
        };

        foreach (var seed in seeds)
        {
            var continuations = new (string Expression, double Expected)[]
            {
                ("Ans+2", seed.Value + 2d),
                ("Ans-2", seed.Value - 2d),
                ("Ans*2", seed.Value * 2d),
                ("Ans×2", seed.Value * 2d),
                ("Ans/2", seed.Value / 2d),
                ("Ans÷2", seed.Value / 2d),
                ("Ans^2", Math.Pow(seed.Value, 2d)),
                ("√(Ans^2)", Math.Abs(seed.Value)),
                ("root(3;Ans^3)", seed.Value)
            };

            foreach (var continuation in continuations)
            {
                var session = new CalculatorSession(new ExpressionCalculator());
                Assert.True(session.Calculate(seed.Expression).Success, seed.Expression);
                var result = session.Calculate(continuation.Expression);

                Assert.True(result.Success, $"{seed.Expression}, {continuation.Expression}: {result.Message}");
                var tolerance = Math.Max(1E-12d, Math.Abs(continuation.Expected) * 1E-12d);
                Assert.InRange(Math.Abs(result.Value!.Value - continuation.Expected), 0d, tolerance);
                Assert.Equal(result.Value, session.LastResult);
                Assert.Equal(2, session.History.Count);
            }
        }
    }

    [Fact]
    public void Every_error_class_leaves_ans_and_history_unchanged()
    {
        var invalidExpressions = new[]
        {
            "", "1/0", "0/0", "0^0", "0^-1", "sqrt(-1)", "root(2;-1)",
            "root(1;8)", "root(2,5;8)", "1+", "(1+2", "abc", "1E+",
            new string('1', ExpressionCalculator.MaxExpressionLength + 1),
            new string('(', ExpressionCalculator.MaxNestingDepth + 1) + "1" +
            new string(')', ExpressionCalculator.MaxNestingDepth + 1)
        };

        Assert.True(_session.Calculate("6*7").Success);
        var expectedHistory = _session.History.ToArray();

        foreach (var expression in invalidExpressions)
        {
            var result = _session.Calculate(expression);

            Assert.False(result.Success, expression);
            Assert.Equal(42d, _session.LastResult);
            Assert.Equal(expectedHistory, _session.History);
            Assert.DoesNotContain("NaN", result.DisplayValue, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void Long_mixed_session_keeps_consistent_ans_and_bounded_history()
    {
        var expected = 1d;
        Assert.True(_session.Calculate("1").Success);

        for (var step = 1; step <= 1_000; step++)
        {
            var operation = step % 4;
            var expression = operation switch
            {
                0 => "Ans+1",
                1 => "Ans*2",
                2 => "Ans/2",
                _ => "Ans-1"
            };
            expected = operation switch
            {
                0 => expected + 1d,
                1 => expected * 2d,
                2 => expected / 2d,
                _ => expected - 1d
            };

            var result = _session.Calculate(expression);
            Assert.True(result.Success, $"Krok {step}: {result.Message}");
            Assert.Equal(expected, result.Value!.Value, 12);
            Assert.Equal(expected, _session.LastResult!.Value, 12);
            Assert.InRange(_session.History.Count, 1, CalculatorSession.HistoryLimit);

            if (step % 25 == 0)
            {
                var history = _session.History.ToArray();
                var lastResult = _session.LastResult;
                Assert.False(_session.Calculate("1/0").Success);
                Assert.Equal(lastResult, _session.LastResult);
                Assert.Equal(history, _session.History);
            }
        }

        Assert.Equal(CalculatorSession.HistoryLimit, _session.History.Count);
        Assert.Equal("Ans+1", _session.History[0].Expression);
    }
}
