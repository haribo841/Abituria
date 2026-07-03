using System.Globalization;
using Abituria.Services;

namespace Abituria.Tests;

public sealed class AnsCubeRootAndSquareTests
{
    private readonly ExpressionCalculator _calculator = new();

    [Theory]
    [InlineData(1d, 1d)]
    [InlineData(8d, 0.0625d)]
    [InlineData(-8d, 0.0625d)]
    [InlineData(0.125d, 16d)]
    [InlineData(-0.125d, 16d)]
    [InlineData(1E-30d, 1E40d)]
    public void Ans_times_its_cube_root_is_a_valid_denominator_for_nonzero_values(
        double ans,
        double expected)
    {
        var result = _calculator.Evaluate("1/(Ans∛(Ans))", ans);

        AssertValue(result, expected, ans.ToString("R"));
    }

    [Fact]
    public void Zero_ans_correctly_reports_division_by_zero()
    {
        var result = _calculator.Evaluate("1/(Ans∛(Ans))", 0d);

        Assert.False(result.Success);
        Assert.Equal(CalculationErrorCode.DivisionByZero, result.ErrorCode);
        Assert.Contains("dzielić przez zero", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Tiny_nonzero_ans_reports_range_underflow_instead_of_false_division_by_zero()
    {
        var result = _calculator.Evaluate("1/(Ans∛(Ans))", 1E-300d);

        Assert.False(result.Success);
        Assert.Equal(CalculationErrorCode.NonFiniteResult, result.ErrorCode);
        Assert.Contains("zbyt mały", result.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("dzielić przez zero", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Missing_ans_remains_a_controlled_error()
    {
        var result = _calculator.Evaluate("1/(Ans∛(Ans))");

        Assert.False(result.Success);
        Assert.Equal(CalculationErrorCode.MissingAns, result.ErrorCode);
    }

    [Theory]
    [InlineData(3d, 9d)]
    [InlineData(-3d, 9d)]
    [InlineData(1.5d, 2.25d)]
    [InlineData(1E-100d, 1E-200d)]
    public void Square_button_uses_the_last_displayed_result(double value, double expected)
    {
        var state = new CalculatorInputState();
        state.MarkResult(value);

        var edit = state.CreateSquare("poprzednie wyrażenie", 0, 0);
        var result = _calculator.Evaluate(edit.Text);

        var formatted = value.ToString("R", CultureInfo.InvariantCulture);
        Assert.Equal($"({formatted})^2", edit.Text);
        Assert.True(edit.ShouldCalculate);
        AssertValue(result, expected, edit.Text);
    }

    [Fact]
    public void Square_button_wraps_the_complete_expression()
    {
        var state = new CalculatorInputState();

        var edit = state.CreateSquare("2+3", 3, 3);

        Assert.Equal("(2+3)^2", edit.Text);
        AssertValue(_calculator.Evaluate(edit.Text), 25d, edit.Text);
    }

    [Fact]
    public void Square_button_wraps_only_the_selected_operand()
    {
        var state = new CalculatorInputState();

        var edit = state.CreateSquare("2+3", 2, 3);

        Assert.Equal("2+(3)^2", edit.Text);
        AssertValue(_calculator.Evaluate(edit.Text), 11d, edit.Text);
    }

    [Fact]
    public void Square_button_inserts_one_stable_template_for_empty_input()
    {
        var state = new CalculatorInputState();
        var edit = state.CreateSquare(string.Empty, 0, 0);

        Assert.Equal("()^2", edit.Text);
        Assert.Equal(1, edit.SelectionStart);
        Assert.False(edit.ShouldCalculate);

        for (var press = 0; press < 100; press++)
        {
            edit = state.CreateSquare(edit.Text, edit.SelectionStart, edit.SelectionStart);
            Assert.Equal("()^2", edit.Text);
            Assert.Equal(1, edit.SelectionStart);
            Assert.False(edit.ShouldCalculate);
        }
    }

    [Fact]
    public void Square_button_preserves_history_and_repeated_equals_semantics()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        var state = new CalculatorInputState();
        AssertValue(session.Calculate("2+3"), 5d, "2+3");
        state.MarkResult(session.LastResult!.Value);

        var edit = state.CreateSquare("2+3", 3, 3);
        AssertValue(session.Calculate(edit.Text), 25d, edit.Text);
        var squareItem = session.History[0];

        AssertValue(session.Calculate("10"), 10d, "10");
        AssertValue(session.RestoreHistory(squareItem), 25d, "odtworzenie x²");
        AssertValue(session.RepeatLast(), 625d, "kolejne =");
    }

    [Fact]
    public void Square_overflow_is_controlled_and_does_not_enter_history()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        var state = new CalculatorInputState();
        AssertValue(session.Calculate("1E200"), 1E200d, "wartość bazowa");
        var history = session.History.ToArray();
        state.MarkResult(session.LastResult!.Value);

        var edit = state.CreateSquare("1E200", 5, 5);
        var result = session.Calculate(edit.Text);

        Assert.False(result.Success);
        Assert.Equal(CalculationErrorCode.NonFiniteResult, result.ErrorCode);
        Assert.Equal(1E200d, session.LastResult);
        Assert.Equal(history, session.History);
    }

    private static void AssertValue(CalculationResult result, double expected, string context)
    {
        Assert.True(result.Success, $"{context}: {result.ErrorCode}: {result.Message}");
        var actual = result.Value!.Value;
        var tolerance = Math.Max(1E-12d, Math.Abs(expected) * 1E-12d);
        Assert.InRange(Math.Abs(actual - expected), 0d, tolerance);
        Assert.True(double.IsFinite(actual));
    }
}
