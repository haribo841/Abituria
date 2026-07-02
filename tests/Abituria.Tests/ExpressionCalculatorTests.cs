using Abituria.Services;

namespace Abituria.Tests;

public sealed class ExpressionCalculatorTests
{
    private readonly ExpressionCalculator _calculator = new();

    [Theory]
    [InlineData("2+3*4", 14d)]
    [InlineData("(2+3)*4", 20d)]
    [InlineData("2^3^2", 512d)]
    [InlineData("-2^2", -4d)]
    [InlineData("(-2)^2", 4d)]
    [InlineData("2^-2", 0.25d)]
    [InlineData("1,5+2.25", 3.75d)]
    [InlineData("2(3+4)", 14d)]
    [InlineData("(2+3)(4-1)", 15d)]
    [InlineData("3√8", 8.48528137423857d)]
    public void Evaluates_operators_precedence_and_implicit_multiplication(string expression, double expected)
    {
        AssertSuccess(expression, expected);
    }

    [Theory]
    [InlineData("sqrt(9)", 3d)]
    [InlineData("√(2+7)", 3d)]
    [InlineData("∛(-8)", -2d)]
    [InlineData("∜16", 2d)]
    [InlineData("root(5; 32)", 2d)]
    public void Evaluates_supported_root_notations(string expression, double expected)
    {
        AssertSuccess(expression, expected);
    }

    [Theory]
    [InlineData("1/0", CalculationErrorCode.DivisionByZero)]
    [InlineData("0/0", CalculationErrorCode.DivisionByZero)]
    [InlineData("0^0", CalculationErrorCode.UndefinedPower)]
    [InlineData("0^-1", CalculationErrorCode.DivisionByZero)]
    [InlineData("sqrt(-1)", CalculationErrorCode.InvalidRoot)]
    [InlineData("root(4; -16)", CalculationErrorCode.InvalidRoot)]
    [InlineData("root(0; 8)", CalculationErrorCode.InvalidRoot)]
    [InlineData("root(2,5; 8)", CalculationErrorCode.InvalidRoot)]
    [InlineData("(-8)^(1/3)", CalculationErrorCode.UndefinedPower)]
    public void Returns_domain_errors_without_throwing(string expression, CalculationErrorCode expectedCode)
    {
        var result = _calculator.Evaluate(expression);

        Assert.False(result.Success);
        Assert.Equal(expectedCode, result.ErrorCode);
        Assert.NotEmpty(result.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("2+")]
    [InlineData("(2+3")]
    [InlineData("1,2,3")]
    [InlineData("2**3")]
    [InlineData("2 3")]
    [InlineData("abc(2)")]
    [InlineData("root(3 8)")]
    public void Rejects_malformed_expressions_without_throwing(string expression)
    {
        var exception = Record.Exception(() => _calculator.Evaluate(expression));
        var result = _calculator.Evaluate(expression);

        Assert.Null(exception);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorCode);
        Assert.NotNull(result.ErrorPosition);
    }

    [Fact]
    public void Enforces_length_and_nesting_limits()
    {
        var tooLong = _calculator.Evaluate(new string('1', ExpressionCalculator.MaxExpressionLength + 1));
        var tooDeepExpression = new string('(', ExpressionCalculator.MaxNestingDepth + 1) + "1" + new string(')', ExpressionCalculator.MaxNestingDepth + 1);
        var tooDeep = _calculator.Evaluate(tooDeepExpression);

        Assert.Equal(CalculationErrorCode.ExpressionTooLong, tooLong.ErrorCode);
        Assert.Equal(CalculationErrorCode.ExpressionTooDeep, tooDeep.ErrorCode);
    }

    [Fact]
    public void Uses_ans_only_when_a_previous_value_exists()
    {
        Assert.Equal(CalculationErrorCode.MissingAns, _calculator.Evaluate("Ans+1").ErrorCode);
        var result = _calculator.Evaluate("2Ans", 3d);

        Assert.True(result.Success);
        Assert.Equal(6d, result.Value);
    }

    [Fact]
    public void Formats_results_with_polish_separator_and_without_negative_zero()
    {
        Assert.Equal("3,75", _calculator.Evaluate("1,5+2.25").DisplayValue);
        Assert.Equal("0", _calculator.Evaluate("-0").DisplayValue);
    }

    [Fact]
    public void Rejects_non_finite_results()
    {
        var expression = string.Join("*", Enumerable.Repeat("999999999999999", 30));
        var result = _calculator.Evaluate(expression);

        Assert.False(result.Success);
        Assert.Equal(CalculationErrorCode.NonFiniteResult, result.ErrorCode);
    }

    private void AssertSuccess(string expression, double expected)
    {
        var result = _calculator.Evaluate(expression);

        Assert.True(result.Success, result.Message);
        Assert.NotNull(result.Value);
        Assert.Equal(expected, result.Value.Value, 12);
        Assert.NotEmpty(result.DisplayValue);
    }
}
