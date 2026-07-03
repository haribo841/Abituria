using Abituria.Services;

namespace Abituria.Tests;

public sealed class LegacyCalculatorRegressionTests
{
    private readonly ExpressionCalculator _calculator = new();

    [Fact]
    public void Historical_zero_root_colon_expression_reports_division_by_zero_without_exception()
    {
        CalculationResult? result = null;

        var exception = Record.Exception(() => result = _calculator.Evaluate("0√0:0√0"));

        Assert.Null(exception);
        var calculation = Assert.IsType<CalculationResult>(result);
        Assert.False(calculation.Success);
        Assert.Equal(CalculationErrorCode.DivisionByZero, calculation.ErrorCode);
        Assert.Equal(3, calculation.ErrorPosition);
        Assert.Contains("dzielić przez zero", calculation.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("0√0/0√0")]
    [InlineData("0√0÷0√0")]
    [InlineData("1/(2-2)")]
    [InlineData("1/√0")]
    public void Division_by_zero_from_composite_expressions_is_reported_without_exception(string expression)
    {
        CalculationResult? result = null;

        var exception = Record.Exception(() => result = _calculator.Evaluate(expression));

        Assert.Null(exception);
        var calculation = Assert.IsType<CalculationResult>(result);
        Assert.False(calculation.Success);
        Assert.Equal(CalculationErrorCode.DivisionByZero, calculation.ErrorCode);
        Assert.Contains("dzielić przez zero", calculation.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("√16/√4", 2d)]
    [InlineData("sqrt(16)/sqrt(4)", 2d)]
    [InlineData("root(3; 8)/√4", 1d)]
    public void Division_with_valid_root_expressions_returns_a_result(string expression, double expected)
    {
        var result = _calculator.Evaluate(expression);

        Assert.True(result.Success, result.Message);
        Assert.NotNull(result.Value);
        Assert.Equal(expected, result.Value.Value, 12);
    }

    [Theory]
    [InlineData("0:0√0", CalculationErrorCode.DivisionByZero)]
    [InlineData("1/abc", CalculationErrorCode.InvalidToken)]
    [InlineData("1/", CalculationErrorCode.UnexpectedToken)]
    [InlineData("√/√", CalculationErrorCode.UnexpectedToken)]
    public void Other_historical_partial_division_states_are_rejected_without_exception(
        string expression,
        CalculationErrorCode expectedCode)
    {
        CalculationResult? result = null;

        var exception = Record.Exception(() => result = _calculator.Evaluate(expression));

        Assert.Null(exception);
        var calculation = Assert.IsType<CalculationResult>(result);
        Assert.False(calculation.Success);
        Assert.Equal(expectedCode, calculation.ErrorCode);
        Assert.NotNull(calculation.ErrorPosition);
    }

    [Theory]
    [InlineData("9*(1/9)", 1d)]
    [InlineData("9/(1/9)", 81d)]
    [InlineData("1/(1/9)", 9d)]
    public void Reciprocal_is_evaluated_as_an_expression_instead_of_text_fragments(
        string expression,
        double expected)
    {
        var result = _calculator.Evaluate(expression);

        Assert.True(result.Success, result.Message);
        Assert.NotNull(result.Value);
        Assert.Equal(expected, result.Value.Value, 12);
        Assert.DoesNotContain("NaN", result.DisplayValue, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("1/(0)")]
    [InlineData("0/(1/0)")]
    public void Reciprocal_of_zero_returns_a_controlled_error_without_nan(string expression)
    {
        CalculationResult? result = null;

        var exception = Record.Exception(() => result = _calculator.Evaluate(expression));

        Assert.Null(exception);
        var calculation = Assert.IsType<CalculationResult>(result);
        Assert.False(calculation.Success);
        Assert.Equal(CalculationErrorCode.DivisionByZero, calculation.ErrorCode);
        Assert.DoesNotContain("NaN", calculation.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Historical_reciprocal_marker_is_rejected_without_format_exception()
    {
        CalculationResult? result = null;

        var exception = Record.Exception(() => result = _calculator.Evaluate("x:(1/x)"));

        Assert.Null(exception);
        var calculation = Assert.IsType<CalculationResult>(result);
        Assert.False(calculation.Success);
        Assert.Equal(CalculationErrorCode.InvalidToken, calculation.ErrorCode);
    }

    [Theory]
    [InlineData("5:")]
    [InlineData("5:1/")]
    [InlineData("0:1/x")]
    [InlineData("1/NaN")]
    [InlineData("1/NaN9")]
    [InlineData("9*(1/x)")]
    public void Exact_historical_partial_reciprocal_text_never_throws(string expression)
    {
        CalculationResult? result = null;

        var exception = Record.Exception(() => result = _calculator.Evaluate(expression));

        Assert.Null(exception);
        var calculation = Assert.IsType<CalculationResult>(result);
        Assert.False(calculation.Success);
        Assert.NotNull(calculation.ErrorCode);
        Assert.Null(calculation.Value);
    }

    [Theory]
    [InlineData("9*(1/9)", 1d)]
    [InlineData("9*(1/9)^2", 0.1111111111111111d)]
    [InlineData("(1/9)^2", 0.012345679012345678d)]
    [InlineData("1/(9*9)", 0.012345679012345678d)]
    [InlineData("9^(1/9)", 1.2765180070092417d)]
    public void Historical_reciprocal_and_power_compositions_have_mathematical_results(
        string expression,
        double expected)
    {
        var result = _calculator.Evaluate(expression);

        Assert.True(result.Success, result.Message);
        Assert.Equal(expected, result.Value!.Value, 12);
        Assert.DoesNotContain("NaN", result.DisplayValue, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Production_calculator_does_not_use_throwing_legacy_number_conversion()
    {
        var root = FindRepositoryRoot();
        var calculatorFiles = new[]
        {
            Path.Combine(root, "AvaloniaApp", "Services", "ExpressionCalculator.cs"),
            Path.Combine(root, "AvaloniaApp", "Services", "CalculatorInputState.cs"),
            Path.Combine(root, "AvaloniaApp", "Services", "CalculatorSession.cs"),
            Path.Combine(root, "AvaloniaApp", "Views", "GeneralCalculatorView.cs")
        };

        foreach (var file in calculatorFiles)
        {
            var source = File.ReadAllText(file);
            Assert.DoesNotContain("double.Parse(", source, StringComparison.Ordinal);
            Assert.DoesNotContain("Double.Parse(", source, StringComparison.Ordinal);
            Assert.DoesNotContain("Convert.ToDouble(", source, StringComparison.Ordinal);
        }
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Abituria.sln"))) return directory.FullName;
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Nie znaleziono katalogu repozytorium Abituria.");
    }
}
