using Abituria.Services;

namespace Abituria.Tests;

public sealed class CalculatorInputStateTests
{
    private readonly ExpressionCalculator _calculator = new();

    [Fact]
    public void Digit_after_result_starts_a_new_expression()
    {
        var state = new CalculatorInputState();
        state.MarkResult(4d);

        var prepared = state.BeginValue("2+2");

        Assert.Equal(string.Empty, prepared);
        Assert.False(state.IsAfterResult);
        Assert.Equal("5", prepared + "5");
    }

    [Fact]
    public void Operator_after_result_continues_from_the_result_value()
    {
        var state = new CalculatorInputState();
        state.MarkResult(4d);

        var expression = state.BeginOperator("2+2") + "*5";
        var result = _calculator.Evaluate(expression);

        Assert.Equal("4*5", expression);
        Assert.True(result.Success, result.Message);
        Assert.Equal(20d, result.Value);
    }

    [Fact]
    public void Reciprocal_button_uses_the_last_displayed_result()
    {
        var state = new CalculatorInputState();
        state.MarkResult(9d);

        var edit = state.CreateReciprocal("3*3", 3, 3);
        var result = _calculator.Evaluate(edit.Text);

        Assert.Equal("1/(9)", edit.Text);
        Assert.True(edit.ShouldCalculate);
        Assert.True(result.Success, result.Message);
        Assert.Equal(1d / 9d, result.Value!.Value, 12);
    }

    [Fact]
    public void Reciprocal_button_wraps_the_complete_current_expression()
    {
        var state = new CalculatorInputState();

        var edit = state.CreateReciprocal("9*(1/9)", 9, 9);
        var result = _calculator.Evaluate(edit.Text);

        Assert.Equal("1/(9*(1/9))", edit.Text);
        Assert.True(result.Success, result.Message);
        Assert.Equal(1d, result.Value!.Value, 12);
    }

    [Fact]
    public void Reciprocal_button_inserts_an_editable_template_for_empty_input()
    {
        var state = new CalculatorInputState();

        var edit = state.CreateReciprocal(string.Empty, 0, 0);

        Assert.Equal("1/()", edit.Text);
        Assert.Equal(3, edit.SelectionStart);
        Assert.Equal(0, edit.SelectionLength);
        Assert.False(edit.ShouldCalculate);
    }

    [Fact]
    public void Reciprocal_button_wraps_only_the_selected_operand()
    {
        var state = new CalculatorInputState();

        var edit = state.CreateReciprocal("2+9", 2, 3);
        var result = _calculator.Evaluate(edit.Text);

        Assert.Equal("2+1/(9)", edit.Text);
        Assert.True(result.Success, result.Message);
        Assert.Equal(2d + 1d / 9d, result.Value!.Value, 12);
    }

    [Fact]
    public void Repeated_reciprocal_of_a_small_number_never_fails_on_scientific_notation()
    {
        var state = new CalculatorInputState();
        var source = "1÷55555";
        var smallResult = _calculator.Evaluate(source);
        Assert.True(smallResult.Success, smallResult.Message);
        var value = 0d;

        for (var press = 1; press <= 100; press++)
        {
            CalculatorTextEdit edit;
            if (press == 1)
            {
                edit = state.CreateReciprocal(source, source.Length, source.Length);
            }
            else
            {
                state.MarkResult(value);
                edit = state.CreateReciprocal(string.Empty, 0, 0);
            }

            var result = _calculator.Evaluate(edit.Text);

            Assert.True(result.Success, $"Naciśnięcie {press}: {edit.Text}: {result.Message}");
            Assert.DoesNotContain("NaN", result.DisplayValue, StringComparison.OrdinalIgnoreCase);
            value = result.Value!.Value;

            var expected = press % 2 == 1 ? 55555d : smallResult.Value!.Value;
            Assert.InRange(Math.Abs((value - expected) / expected), 0d, 1E-14d);
        }
    }

    [Fact]
    public void Repeated_reciprocal_is_stable_for_a_wide_value_grid()
    {
        var expressions = new[]
        {
            "1", "-1", "0,5", "-0.5", "55555", "1/55555", "√2", "root(3;2)",
            "2^50", "1,8E-13", "1E-300", "-1E-300", "1E300", "-1E300",
        };

        foreach (var expression in expressions)
        {
            var initial = _calculator.Evaluate(expression);
            Assert.True(initial.Success, $"{expression}: {initial.Message}");
            var initialValue = initial.Value!.Value;
            var value = initialValue;
            var state = new CalculatorInputState();

            for (var press = 1; press <= 200; press++)
            {
                state.MarkResult(value);
                var edit = state.CreateReciprocal(string.Empty, 0, 0);
                var result = _calculator.Evaluate(edit.Text);

                Assert.True(result.Success, $"{expression}, naciśnięcie {press}, {edit.Text}: {result.Message}");
                value = result.Value!.Value;
                var expected = press % 2 == 1 ? 1d / initialValue : initialValue;
                var relativeError = expected == 0d ? Math.Abs(value) : Math.Abs(value / expected - 1d);
                Assert.InRange(relativeError, 0d, 1E-14d);
                Assert.DoesNotContain("NaN", result.DisplayValue, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    [Fact]
    public void Reciprocal_chain_at_double_boundary_returns_a_controlled_range_error()
    {
        var state = new CalculatorInputState();
        state.MarkResult(double.MaxValue);
        var firstEdit = state.CreateReciprocal(string.Empty, 0, 0);
        var first = _calculator.Evaluate(firstEdit.Text);
        Assert.True(first.Success, first.Message);

        state.MarkResult(first.Value!.Value);
        var secondEdit = state.CreateReciprocal(string.Empty, 0, 0);
        var second = _calculator.Evaluate(secondEdit.Text);

        Assert.False(second.Success);
        Assert.Equal(CalculationErrorCode.NonFiniteResult, second.ErrorCode);
        Assert.Null(second.Value);
        Assert.DoesNotContain("NaN", second.DisplayValue, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Reciprocal_of_every_zero_notation_is_a_controlled_error()
    {
        var zeroExpressions = new[]
        {
            "0", "-0", "0,0", "0.0E100", "1-1", "√0", "∛0", "root(9;0)", "0^2"
        };

        foreach (var expression in zeroExpressions)
        {
            var state = new CalculatorInputState();
            var edit = state.CreateReciprocal(expression, expression.Length, expression.Length);
            var result = _calculator.Evaluate(edit.Text);

            Assert.False(result.Success, edit.Text);
            Assert.Equal(CalculationErrorCode.DivisionByZero, result.ErrorCode);
            Assert.Null(result.Value);
        }
    }

    [Fact]
    public void Every_selection_boundary_for_reciprocal_is_clamped_and_controlled()
    {
        const string expression = "2+3*4";
        for (var selectionStart = -3; selectionStart <= expression.Length + 3; selectionStart++)
            for (var selectionEnd = -3; selectionEnd <= expression.Length + 3; selectionEnd++)
            {
                var state = new CalculatorInputState();
                var exception = Record.Exception(() =>
                {
                    var edit = state.CreateReciprocal(expression, selectionStart, selectionEnd);
                    Assert.InRange(edit.SelectionStart, 0, edit.Text.Length);
                    Assert.InRange(edit.SelectionStart + edit.SelectionLength, 0, edit.Text.Length);
                    var result = _calculator.Evaluate(edit.Text);
                    Assert.True(result.Success || result.ErrorCode is not null, edit.Text);
                });

                Assert.Null(exception);
            }
    }

    [Fact]
    public void Pressing_reciprocal_repeatedly_on_empty_input_keeps_one_editable_template()
    {
        var state = new CalculatorInputState();
        var edit = state.CreateReciprocal(string.Empty, 0, 0);

        for (var press = 0; press < 100; press++)
        {
            edit = state.CreateReciprocal(edit.Text, edit.SelectionStart, edit.SelectionStart);
            Assert.Equal("1/()", edit.Text);
            Assert.Equal(3, edit.SelectionStart);
            Assert.False(edit.ShouldCalculate);
        }
    }

    [Fact]
    public void Non_finite_values_cannot_enter_post_result_state()
    {
        var state = new CalculatorInputState();

        Assert.Throws<ArgumentOutOfRangeException>(() => state.MarkResult(double.NaN));
        Assert.Throws<ArgumentOutOfRangeException>(() => state.MarkResult(double.PositiveInfinity));
        Assert.Throws<ArgumentOutOfRangeException>(() => state.MarkResult(double.NegativeInfinity));
        Assert.False(state.IsAfterResult);
    }
}
