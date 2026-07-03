using Abituria.Services;

namespace Abituria.Tests;

public sealed class Issue7InfinityRegressionTests
{
    private readonly ExpressionCalculator _calculator = new();

    public static TheoryData<string> InfinityExpressions => new()
    {
        "∞",
        "+∞",
        "-∞",
        "1/∞",
        "∞/1",
        "∞/∞",
        "1+∞",
        "∞-1",
        "2*∞",
        "∞×2",
        "∞÷2",
        "∞:2",
        "∞^0",
        "2^∞",
        "(∞)",
        "2(∞)",
        "√∞",
        "∛∞",
        "∜∞",
        "sqrt(∞)",
        "root(3;∞)",
        "root(∞;8)",
        "Ans+∞",
        "1E308+∞"
    };

    public static TheoryData<string> InfinityAliases => new()
    {
        "Infinity",
        "infinity",
        "INFINITY",
        "Inf",
        "INF",
        "1/Infinity",
        "sqrt(inf)",
        "root(3;Infinity)"
    };

    public static TheoryData<string> DivisionByZeroExpressions => new()
    {
        "1/0",
        "0/0",
        "-5/0",
        "(2+3)/0",
        "2^10/0",
        "2(3+4)/0",
        "sqrt(81)/0",
        "root(3;27)/0",
        "1E308/0",
        "5:0",
        "5÷0",
        "5/(3-3)",
        "5/√0",
        "5/root(3;0)",
        "0√0:0√0",
        "Ans/0"
    };

    [Theory]
    [MemberData(nameof(InfinityExpressions))]
    [MemberData(nameof(InfinityAliases))]
    public void Infinity_is_rejected_as_a_non_finite_operand(string expression)
    {
        CalculationResult? result = null;

        var exception = Record.Exception(() => result = _calculator.Evaluate(expression, 42d));

        Assert.Null(exception);
        var calculation = Assert.IsType<CalculationResult>(result);
        Assert.False(calculation.Success);
        Assert.Equal(CalculationErrorCode.NonFiniteResult, calculation.ErrorCode);
        Assert.Null(calculation.Value);
        Assert.Empty(calculation.DisplayValue);
        Assert.NotNull(calculation.ErrorPosition);
        Assert.DoesNotContain("NaN", calculation.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Infinity", calculation.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Infinity_at_every_position_in_representative_expressions_is_always_controlled()
    {
        var expressions = new[]
        {
            "0", "12,5", "1E308", "2+3*4", "(2+3)^4", "sqrt(9)", "root(3;27)",
            "2(3+4)", "Ans/2", "0√0:0√0"
        };

        foreach (var source in expressions)
        {
            for (var position = 0; position <= source.Length; position++)
            {
                var expression = source.Insert(position, "∞");
                CalculationResult? result = null;
                var exception = Record.Exception(() => result = _calculator.Evaluate(expression, 42d));

                Assert.Null(exception);
                var calculation = Assert.IsType<CalculationResult>(result);
                Assert.False(calculation.Success, expression);
                Assert.Null(calculation.Value);
                Assert.NotNull(calculation.ErrorCode);
                Assert.DoesNotContain("NaN", calculation.DisplayValue, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("Infinity", calculation.DisplayValue, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    [Theory]
    [MemberData(nameof(DivisionByZeroExpressions))]
    public void Repeated_reciprocal_after_division_by_zero_keeps_the_expression_and_error_stable(string expression)
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        var inputState = new CalculatorInputState();
        Assert.True(session.Calculate("42").Success);
        var expectedHistory = session.History.ToArray();

        var initial = session.Calculate(expression);
        inputState.MarkError();

        Assert.False(initial.Success, expression);
        Assert.Equal(CalculationErrorCode.DivisionByZero, initial.ErrorCode);
        Assert.Equal(42d, session.LastResult);
        Assert.Equal(expectedHistory, session.History);

        var currentText = expression;
        for (var press = 1; press <= 250; press++)
        {
            var edit = inputState.CreateReciprocal(currentText, currentText.Length, currentText.Length);

            Assert.True(edit.ShouldCalculate);
            Assert.Equal(expression, edit.Text);
            Assert.InRange(edit.SelectionStart, 0, expression.Length);

            currentText = edit.Text;
            var result = session.Calculate(currentText);
            inputState.MarkError();

            Assert.False(result.Success, $"{expression}, kliknięcie {press}");
            Assert.Equal(CalculationErrorCode.DivisionByZero, result.ErrorCode);
            Assert.Null(result.Value);
            Assert.DoesNotContain("∞", result.DisplayValue, StringComparison.Ordinal);
            Assert.DoesNotContain("NaN", result.DisplayValue, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Infinity", result.DisplayValue, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(42d, session.LastResult);
            Assert.Equal(expectedHistory, session.History);
        }
    }

    [Fact]
    public void Editing_after_an_error_leaves_error_mode_and_allows_reciprocal_again()
    {
        var inputState = new CalculatorInputState();
        inputState.MarkError();

        Assert.True(inputState.IsAfterError);
        inputState.Reset();

        var edit = inputState.CreateReciprocal("2", 1, 1);
        var result = _calculator.Evaluate(edit.Text);

        Assert.False(inputState.IsAfterError);
        Assert.Equal("1/(2)", edit.Text);
        Assert.True(result.Success, result.Message);
        Assert.Equal(0.5d, result.Value);
    }
}
