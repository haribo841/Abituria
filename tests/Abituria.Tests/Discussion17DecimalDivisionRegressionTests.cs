using Abituria.Services;

namespace Abituria.Tests;

public sealed class Discussion17DecimalDivisionRegressionTests
{
    [Fact]
    public void Historical_result_3_96_is_identical_for_every_division_path_and_reciprocal_button()
    {
        var calculator = new ExpressionCalculator();
        var session = new CalculatorSession(calculator);
        var expressions = new[]
        {
            "4*0,99",
            "7,92:2",
            "7,92/2",
            "7,92÷2"
        };

        foreach (var expression in expressions)
            AssertHistoricalResult(session.Calculate(expression), expression);

        var inputState = new CalculatorInputState();
        const string reciprocalOperand = "25/99";
        var reciprocalEdit = inputState.CreateReciprocal(
            reciprocalOperand,
            reciprocalOperand.Length,
            reciprocalOperand.Length);

        Assert.True(reciprocalEdit.ShouldCalculate);
        Assert.Equal("1/(25/99)", reciprocalEdit.Text);
        AssertHistoricalResult(session.Calculate(reciprocalEdit.Text), "przycisk 1/x");

        Assert.Equal(5, session.History.Count);
        Assert.All(session.History, item => Assert.Equal("3,96", item.Result));
    }

    private static void AssertHistoricalResult(CalculationResult result, string context)
    {
        Assert.True(result.Success, $"{context}: {result.ErrorCode}: {result.Message}");
        Assert.NotNull(result.Value);
        Assert.InRange(Math.Abs(result.Value.Value - 3.96d), 0d, 1E-12d);
        Assert.Equal("3,96", result.DisplayValue);
    }
}
