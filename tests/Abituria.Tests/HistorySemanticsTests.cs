using Abituria.Services;

namespace Abituria.Tests;

public sealed class HistorySemanticsTests
{
    public static TheoryData<string, double> FloatingPointPowers => new()
    {
        { "2,5^1,5", Math.Pow(2.5d, 1.5d) },
        { "0,000000000001^2", 1E-24d },
        { "1,25E-4^2,5", Math.Pow(1.25E-4d, 2.5d) },
        { "(-3,5)^4", Math.Pow(-3.5d, 4d) },
        { "9^(1/9)", Math.Pow(9d, 1d / 9d) },
        { "2^-2,5", Math.Pow(2d, -2.5d) }
    };

    [Theory]
    [MemberData(nameof(FloatingPointPowers))]
    public void Floating_point_power_is_replayed_from_history_with_the_same_value(
        string expression,
        double expected)
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        AssertValue(session.Calculate(expression), expected, expression);
        var item = session.History[0];
        AssertValue(session.Calculate("123"), 123d, "wartość późniejsza");
        var history = session.History.ToArray();

        var restored = session.RestoreHistory(item);

        AssertValue(restored, expected, expression);
        Assert.Equal(item.Value, session.LastResult);
        Assert.Equal(history, session.History);
        AssertValue(session.Calculate("Ans+1"), expected + 1d, "kontynuacja przez Ans");
    }

    [Fact]
    public void Complex_expression_with_ans_is_replayed_using_its_original_ans_context()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        AssertValue(session.Calculate("-3"), -3d, "wartość Ans");
        const string expression = "sqrt(Ans^2)+root(3;Ans^3)+2(Ans+4)^2";
        AssertValue(session.Calculate(expression), 2d, expression);
        var item = session.History[0];
        Assert.Equal(-3d, item.AnsInput);

        AssertValue(session.Calculate("10"), 10d, "nowe Ans");
        var history = session.History.ToArray();

        var restored = session.RestoreHistory(item);

        AssertValue(restored, 2d, expression);
        Assert.Equal(2d, session.LastResult);
        Assert.Equal(history, session.History);
        AssertValue(session.Calculate("Ans^2+1"), 5d, "Ans po odtworzeniu");
    }

    [Fact]
    public void Every_supported_operation_can_be_restored_in_any_history_order()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        var expressions = new[]
        {
            "1,5+2.25",
            "10-3,5",
            "2,5*4",
            "2,5×4",
            "9/2",
            "9:2",
            "9÷2",
            "2,5^1,5",
            "sqrt(81)",
            "√81",
            "∛(-8)",
            "∜16",
            "root(3;27)",
            "2(3+4)",
            "Ans+√16",
            "1/(1/(9√4))"
        };
        var recorded = new List<(CalculationHistoryItem Item, double Value)>();

        foreach (var expression in expressions)
        {
            var result = session.Calculate(expression);
            Assert.True(result.Success, $"{expression}: {result.ErrorCode}: {result.Message}");
            recorded.Add((session.History[0], result.Value!.Value));
        }

        AssertValue(session.Calculate("999"), 999d, "wartość końcowa");
        var history = session.History.ToArray();

        foreach (var entry in recorded.OrderByDescending(entry => entry.Value))
        {
            var restored = session.RestoreHistory(entry.Item);

            AssertValue(restored, entry.Value, entry.Item.Expression);
            Assert.Equal(entry.Value, session.LastResult);
            Assert.Equal(history, session.History);
        }
    }

    [Fact]
    public void Repeated_equals_after_history_restore_uses_the_restored_power_operation()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        var first = session.Calculate("2,5^1,5");
        Assert.True(first.Success, first.Message);
        var item = session.History[0];
        AssertValue(session.Calculate("2+3"), 5d, "późniejsze działanie");

        AssertValue(session.RestoreHistory(item), item.Value, "odtworzenie potęgi");
        var repeated = session.RepeatLast();

        AssertValue(repeated, Math.Pow(item.Value, 1.5d), "= po odtworzonej potędze");
    }

    [Fact]
    public void Reciprocal_of_a_root_expression_works_before_and_after_history_restore()
    {
        var calculator = new ExpressionCalculator();
        var inputState = new CalculatorInputState();
        const string source = "1/(9√4)";

        var directEdit = inputState.CreateReciprocal(source, source.Length, source.Length);
        Assert.Equal("1/(1/(9√4))", directEdit.Text);
        AssertValue(calculator.Evaluate(directEdit.Text), 18d, "historyczne 1/x");

        var session = new CalculatorSession(calculator);
        AssertValue(session.Calculate(source), 1d / 18d, source);
        var item = session.History[0];
        AssertValue(session.Calculate("7"), 7d, "późniejszy wynik");
        var restored = session.RestoreHistory(item);
        AssertValue(restored, 1d / 18d, "odtworzone wyrażenie z pierwiastkiem");

        inputState.MarkResult(restored.Value!.Value);
        var restoredEdit = inputState.CreateReciprocal(item.Expression, item.Expression.Length, item.Expression.Length);
        AssertValue(session.Calculate(restoredEdit.Text), 18d, "1/x po odtworzeniu historii");
    }

    [Fact]
    public void Removed_or_cleared_history_items_cannot_change_session_state()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        AssertValue(session.Calculate("1"), 1d, "pierwszy wpis");
        var removed = session.History[0];
        for (var value = 2; value <= CalculatorSession.HistoryLimit + 1; value++)
            Assert.True(session.Calculate(value.ToString()).Success);

        var lastResult = session.LastResult;
        var history = session.History.ToArray();
        var evictedResult = session.RestoreHistory(removed);

        Assert.False(evictedResult.Success);
        Assert.Equal(CalculationErrorCode.InvalidHistoryItem, evictedResult.ErrorCode);
        Assert.Equal(lastResult, session.LastResult);
        Assert.Equal(history, session.History);

        var cleared = session.History[0];
        session.ClearHistory();
        var clearedResult = session.RestoreHistory(cleared);

        Assert.False(clearedResult.Success);
        Assert.Equal(CalculationErrorCode.InvalidHistoryItem, clearedResult.ErrorCode);
        Assert.Null(session.LastResult);
        Assert.Empty(session.History);
    }

    private static void AssertValue(CalculationResult result, double expected, string context)
    {
        Assert.True(result.Success, $"{context}: {result.ErrorCode}: {result.Message}");
        var actual = result.Value!.Value;
        var tolerance = Math.Max(1E-14d, Math.Abs(expected) * 1E-12d);
        Assert.InRange(Math.Abs(actual - expected), 0d, tolerance);
        Assert.True(double.IsFinite(actual), context);
    }
}
