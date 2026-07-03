using Abituria.Services;

namespace Abituria.Tests;

public sealed class Issue9LeadingZeroRegressionTests
{
    private readonly ExpressionCalculator _calculator = new();

    public static TheoryData<string, string, double> NonCanonicalExpressions => new()
    {
        { "000001", "1", 1d },
        { "0000,1", "0,1", 0.1d },
        { "0000.1", "0.1", 0.1d },
        { "0:00000,000001", "0:0,000001", 0d },
        { "0/000000000000001", "0/1", 0d },
        { "5+0002", "5+2", 7d },
        { "0000", "0", 0d },
        { "0000,0000", "0,0000", 0d },
        { "0001,5E+02", "1,5E+02", 150d },
        { "sqrt(0009)+root(0003;000027)", "sqrt(9)+root(3;27)", 6d },
        { "0002(0003+0004,5)^0002", "2(3+4,5)^2", 112.5d }
    };

    [Theory]
    [MemberData(nameof(NonCanonicalExpressions))]
    public void Parser_normalizes_every_noncanonical_number_and_reports_it(
        string expression,
        string expectedExpression,
        double expectedValue)
    {
        var result = _calculator.Evaluate(expression);

        Assert.True(result.Success, $"{expression}: {result.ErrorCode}: {result.Message}");
        Assert.Equal(expectedValue, result.Value!.Value, 12);
        Assert.True(result.WasNormalized);
        Assert.Equal(expectedExpression, result.NormalizedExpression);
        Assert.Equal(ExpressionCalculator.LeadingZeroNormalizationMessage, result.Message);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("0,1")]
    [InlineData("0.000001")]
    [InlineData("10")]
    [InlineData("1000")]
    [InlineData("1E-3")]
    [InlineData("root(3;0)")]
    public void Canonical_numbers_are_not_reported_as_modified(string expression)
    {
        var result = _calculator.Evaluate(expression);

        Assert.True(result.Success, result.Message);
        Assert.False(result.WasNormalized);
        Assert.Equal(expression, result.NormalizedExpression);
        Assert.Equal("Wynik", result.Message);
    }

    [Fact]
    public void Exact_issue_9_nonzero_denominator_returns_zero_instead_of_division_error()
    {
        var result = _calculator.Evaluate("0:00000,000001");

        Assert.True(result.Success, result.Message);
        Assert.Equal(0d, result.Value);
        Assert.Equal("0:0,000001", result.NormalizedExpression);
        Assert.Equal(ExpressionCalculator.LeadingZeroNormalizationMessage, result.Message);
    }

    [Theory]
    [InlineData("0:000000")]
    [InlineData("0/0000,0000")]
    [InlineData("0÷000.000")]
    public void Actual_zero_denominator_still_has_the_correct_error(string expression)
    {
        var result = _calculator.Evaluate(expression);

        Assert.False(result.Success);
        Assert.Equal(CalculationErrorCode.DivisionByZero, result.ErrorCode);
        Assert.Contains("dzielić przez zero", result.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Null(result.Value);
    }

    [Fact]
    public void Screen_keypad_cannot_build_the_issue_9_leading_zero_sequence()
    {
        var state = new CalculatorInputState();
        var text = string.Empty;
        var normalizedCount = 0;

        foreach (var character in "0:00000,000001")
        {
            var edit = state.CreateNormalizedInsertion(text, text.Length, text.Length, character.ToString());
            text = edit.Text;
            if (edit.WasNormalized) normalizedCount++;
            Assert.DoesNotContain(":00", text, StringComparison.Ordinal);
        }

        Assert.Equal("0:0,000001", text);
        Assert.True(normalizedCount >= 4);
        var result = _calculator.Evaluate(text);
        Assert.True(result.Success, result.Message);
        Assert.Equal(0d, result.Value);
    }

    [Theory]
    [InlineData("000001", "1")]
    [InlineData("0000,1", "0,1")]
    [InlineData("0:00000,000001", "0:0,000001")]
    public void Pasted_input_is_normalized_before_it_reaches_the_text_field(
        string pasted,
        string expected)
    {
        var state = new CalculatorInputState();

        var edit = state.CreateNormalizedInsertion(string.Empty, 0, 0, pasted);

        Assert.Equal(expected, edit.Text);
        Assert.True(edit.WasNormalized);
        Assert.Equal(expected.Length, edit.SelectionStart);
    }

    [Fact]
    public void Session_history_stores_and_restores_only_the_normalized_expression()
    {
        var session = new CalculatorSession(new ExpressionCalculator());

        var result = session.Calculate("0:00000,000001");

        Assert.True(result.Success, result.Message);
        var item = Assert.Single(session.History);
        Assert.Equal("0:0,000001", item.Expression);
        Assert.Equal(0d, item.Value);
        Assert.True(session.RestoreHistory(item).Success);
        Assert.Equal(0d, session.LastResult);
    }
}
