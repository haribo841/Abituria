using Abituria.Services;

namespace Abituria.Tests;

public sealed class LargeRootRegressionTests
{
    private readonly ExpressionCalculator _calculator = new();

    [Theory]
    [InlineData("sqrt(1E308)", 1E154d)]
    [InlineData("√1E308", 1E154d)]
    [InlineData("sqrt(1,7976931348623157E308)", 1.3407807929942596E154d)]
    [InlineData("root(3;1E300)", 1E100d)]
    [InlineData("∛1E300", 1E100d)]
    [InlineData("root(3;-1E300)", -1E100d)]
    [InlineData("root(5;1E300)", 1E60d)]
    [InlineData("1E308/√1E308", 1E154d)]
    public void Evaluates_extreme_finite_roots_written_in_scientific_notation(
        string expression,
        double expected)
    {
        var result = _calculator.Evaluate(expression);

        Assert.True(result.Success, $"{expression}: {result.ErrorCode}: {result.Message}");
        var actual = result.Value!.Value;
        Assert.InRange(Math.Abs(actual / expected - 1d), 0d, 1E-12d);
        Assert.True(double.IsFinite(actual));
    }

    [Theory]
    [InlineData("√0", 0d)]
    [InlineData("sqrt(0)", 0d)]
    [InlineData("0√0", 0d)]
    [InlineData("9√16", 36d)]
    [InlineData("9:√16", 2.25d)]
    [InlineData("9/√16", 2.25d)]
    [InlineData("9*√16", 36d)]
    [InlineData("9+√16", 13d)]
    [InlineData("9-√16", 5d)]
    public void Evaluates_roots_of_ordinary_numbers_and_after_every_historical_operator(
        string expression,
        double expected)
    {
        var result = _calculator.Evaluate(expression);

        Assert.True(result.Success, $"{expression}: {result.ErrorCode}: {result.Message}");
        Assert.Equal(expected, result.Value!.Value, 12);
    }

    [Theory]
    [InlineData("sqrt(1E309)", CalculationErrorCode.NonFiniteResult)]
    [InlineData("root(3;1E309)", CalculationErrorCode.NonFiniteResult)]
    [InlineData("sqrt(-1E308)", CalculationErrorCode.InvalidRoot)]
    [InlineData("root(4;-1E300)", CalculationErrorCode.InvalidRoot)]
    [InlineData("sqrt(1E308*1E308)", CalculationErrorCode.NonFiniteResult)]
    public void Extreme_invalid_roots_return_controlled_errors(
        string expression,
        CalculationErrorCode expectedCode)
    {
        CalculationResult? result = null;

        var exception = Record.Exception(() => result = _calculator.Evaluate(expression));

        Assert.Null(exception);
        var calculation = Assert.IsType<CalculationResult>(result);
        Assert.False(calculation.Success);
        Assert.Equal(expectedCode, calculation.ErrorCode);
        Assert.Null(calculation.Value);
    }

    [Fact]
    public void Square_root_can_use_the_previous_calculation_through_ans()
    {
        var session = new CalculatorSession(_calculator);
        Assert.True(session.Calculate("9+7").Success);

        var result = session.Calculate("√Ans");

        Assert.True(result.Success, result.Message);
        Assert.Equal(4d, result.Value);
        Assert.Equal(4d, session.LastResult);
        Assert.Equal("√Ans", session.History[0].Expression);
    }

    [Theory]
    [InlineData(2, 16d, "√(16)", 4d)]
    [InlineData(3, -8d, "∛(-8)", -2d)]
    [InlineData(4, 81d, "∜(81)", 3d)]
    [InlineData(5, 32d, "root(5;32)", 2d)]
    public void Root_button_after_equals_wraps_and_calculates_the_displayed_result(
        int degree,
        double previousResult,
        string expectedExpression,
        double expectedValue)
    {
        var state = new CalculatorInputState();
        state.MarkResult(previousResult);

        var edit = state.CreateRootFromResult(degree);

        Assert.NotNull(edit);
        Assert.Equal(expectedExpression, edit.Text);
        Assert.True(edit.ShouldCalculate);
        var result = _calculator.Evaluate(edit.Text);
        Assert.True(result.Success, result.Message);
        Assert.Equal(expectedValue, result.Value!.Value, 12);
        Assert.False(state.IsAfterResult);
    }

    [Fact]
    public void Root_after_an_operator_can_follow_a_previous_result()
    {
        var operations = new (string Symbol, double Expected)[]
        {
            (":", 2.25d), ("/", 2.25d), ("*", 36d), ("+", 13d), ("-", 5d)
        };

        foreach (var operation in operations)
        {
            var state = new CalculatorInputState();
            state.MarkResult(9d);
            var expression = state.BeginOperator("3*3") + operation.Symbol + "√16";
            var result = _calculator.Evaluate(expression);

            Assert.True(result.Success, $"{expression}: {result.Message}");
            Assert.Equal(operation.Expected, result.Value!.Value, 12);
        }
    }
}
