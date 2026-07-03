using Abituria.Services;

namespace Abituria.Tests;

public sealed class RepeatedEqualsTests
{
    [Theory]
    [InlineData("2+3", 5d, 8d, 11d)]
    [InlineData("10-2", 8d, 6d, 4d)]
    [InlineData("3*4", 12d, 48d, 192d)]
    [InlineData("3×4", 12d, 48d, 192d)]
    [InlineData("20/5", 4d, 0.8d, 0.16d)]
    [InlineData("20:5", 4d, 0.8d, 0.16d)]
    [InlineData("20÷5", 4d, 0.8d, 0.16d)]
    [InlineData("2^3", 8d, 512d, 134217728d)]
    public void Repeats_every_explicit_binary_operator(
        string expression,
        double firstExpected,
        double secondExpected,
        double thirdExpected)
    {
        var session = new CalculatorSession(new ExpressionCalculator());

        var first = session.Calculate(expression);
        var second = session.RepeatLast();
        var third = session.RepeatLast();

        AssertValue(first, firstExpected, expression);
        AssertValue(second, secondExpected, expression + " =");
        AssertValue(third, thirdExpected, expression + " = =");
        Assert.Equal(thirdExpected, session.LastResult!.Value, 12);
        Assert.Equal(3, session.History.Count);
    }

    [Theory]
    [InlineData("2+3*4", 14d, 26d, 38d)]
    [InlineData("(2+3)*4", 20d, 80d, 320d)]
    [InlineData("2(3+4)", 14d, 98d, 686d)]
    [InlineData("10/(2+3)", 2d, 0.4d, 0.08d)]
    [InlineData("10-(-2)", 12d, 14d, 16d)]
    [InlineData("2+-3", -1d, -4d, -7d)]
    [InlineData("9+√16", 13d, 17d, 21d)]
    [InlineData("2+3^2", 11d, 20d, 29d)]
    public void Repeats_the_outermost_last_operation_with_its_evaluated_right_operand(
        string expression,
        double firstExpected,
        double secondExpected,
        double thirdExpected)
    {
        var session = new CalculatorSession(new ExpressionCalculator());

        AssertValue(session.Calculate(expression), firstExpected, expression);
        AssertValue(session.RepeatLast(), secondExpected, expression + " =");
        AssertValue(session.RepeatLast(), thirdExpected, expression + " = =");
    }

    [Theory]
    [InlineData("5", 5d)]
    [InlineData("√16", 4d)]
    [InlineData("∛(-8)", -2d)]
    [InlineData("root(5;32)", 2d)]
    [InlineData("-2", -2d)]
    public void Repeated_equals_is_idempotent_when_there_is_no_binary_operation(
        string expression,
        double expected)
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        AssertValue(session.Calculate(expression), expected, expression);
        var historyCount = session.History.Count;

        for (var press = 0; press < 100; press++)
        {
            AssertValue(session.RepeatLast(), expected, expression);
            Assert.Equal(historyCount, session.History.Count);
        }
    }

    [Fact]
    public void A_new_successful_expression_replaces_the_repeated_operation()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        AssertValue(session.Calculate("2+3"), 5d, "2+3");
        AssertValue(session.RepeatLast(), 8d, "2+3 =");

        AssertValue(session.Calculate("4*5"), 20d, "4*5");
        AssertValue(session.RepeatLast(), 100d, "4*5 =");
        AssertValue(session.RepeatLast(), 500d, "4*5 = =");
    }

    [Fact]
    public void A_failed_expression_does_not_destroy_the_previous_repeat_operation()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        AssertValue(session.Calculate("2+3"), 5d, "2+3");
        var history = session.History.ToArray();

        var error = session.Calculate("1/0");

        Assert.False(error.Success);
        Assert.Equal(CalculationErrorCode.DivisionByZero, error.ErrorCode);
        Assert.Equal(5d, session.LastResult);
        Assert.Equal(history, session.History);
        AssertValue(session.RepeatLast(), 8d, "2+3 =");
    }

    [Fact]
    public void Clearing_history_also_clears_repeated_equals_state()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        AssertValue(session.Calculate("2+3"), 5d, "2+3");

        session.ClearHistory();
        var result = session.RepeatLast();

        Assert.False(result.Success);
        Assert.Equal(CalculationErrorCode.EmptyExpression, result.ErrorCode);
        Assert.Null(session.LastResult);
        Assert.Empty(session.History);
    }

    [Fact]
    public void Range_error_during_repeat_keeps_the_last_valid_value_and_history()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        AssertValue(session.Calculate("1E100*1E100"), 1E200d, "1E100*1E100");
        AssertValue(session.RepeatLast(), 1E300d, "1E200*1E100");
        var history = session.History.ToArray();

        var overflow = session.RepeatLast();

        Assert.False(overflow.Success);
        Assert.Equal(CalculationErrorCode.NonFiniteResult, overflow.ErrorCode);
        Assert.Equal(1E300d, session.LastResult);
        Assert.Equal(history, session.History);
    }

    [Fact]
    public void Repeated_equals_respects_history_limit()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        AssertValue(session.Calculate("1+1"), 2d, "1+1");

        for (var press = 0; press < 100; press++) session.RepeatLast();

        Assert.Equal(CalculatorSession.HistoryLimit, session.History.Count);
        Assert.Equal(102d, session.LastResult);
        Assert.Equal("(101)+(1)", session.History[0].Expression);
    }

    [Fact]
    public void Ans_expression_defines_the_next_repeated_operation()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        AssertValue(session.Calculate("2+3"), 5d, "2+3");
        AssertValue(session.Calculate("Ans*2"), 10d, "Ans*2");
        AssertValue(session.RepeatLast(), 20d, "Ans*2 =");
        AssertValue(session.RepeatLast(), 40d, "Ans*2 = =");
    }

    private static void AssertValue(CalculationResult result, double expected, string context)
    {
        Assert.True(result.Success, $"{context}: {result.ErrorCode}: {result.Message}");
        var actual = result.Value!.Value;
        var tolerance = Math.Max(1E-12d, Math.Abs(expected) * 1E-12d);
        Assert.InRange(Math.Abs(actual - expected), 0d, tolerance);
        Assert.DoesNotContain("NaN", result.DisplayValue, StringComparison.OrdinalIgnoreCase);
    }
}
