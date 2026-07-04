using System.Reflection;
using System.Text.RegularExpressions;
using Abituria.Services;
using Abituria.Ui;
using Abituria.Views;

namespace Abituria.Tests;

public sealed class SonarRegressionTests
{
    [Fact]
    public void Expression_calculator_normalizes_only_signed_zero()
    {
        var calculator = new ExpressionCalculator();

        var zero = calculator.Evaluate("-0");
        var subnormal = calculator.Evaluate("5E-324");

        Assert.True(zero.Success, zero.Message);
        Assert.Equal(0L, BitConverter.DoubleToInt64Bits(zero.Value!.Value));
        Assert.True(subnormal.Success, subnormal.Message);
        Assert.Equal(
            BitConverter.DoubleToInt64Bits(double.Epsilon),
            BitConverter.DoubleToInt64Bits(subnormal.Value!.Value));
    }

    [Fact]
    public void Input_state_normalizes_only_signed_zero()
    {
        var state = new CalculatorInputState();
        var negativeZero = BitConverter.Int64BitsToDouble(long.MinValue);

        state.MarkResult(negativeZero);
        Assert.Equal("0", state.BeginOperator(string.Empty));

        state.MarkResult(double.Epsilon);
        Assert.Equal(
            double.Epsilon.ToString("R", System.Globalization.CultureInfo.InvariantCulture),
            state.BeginOperator(string.Empty));
    }

    [Fact]
    public void Scanner_requested_members_have_the_required_api_shape()
    {
        var verifyPassword = typeof(PasswordHasher).GetMethod(nameof(PasswordHasher.VerifyPassword));
        var solve = typeof(QuadraticSolver).GetMethod(nameof(QuadraticSolver.Solve));
        var addTile = typeof(HomeView).GetMethod("AddTile", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(verifyPassword);
        Assert.True(verifyPassword.IsStatic);
        Assert.NotNull(solve);
        Assert.True(solve.IsStatic);
        Assert.NotNull(addTile);
        Assert.InRange(addTile.GetParameters().Length, 0, 7);
        Assert.True(typeof(ExpressionCalculator.CalculationException).IsNestedPublic);
    }

    [Fact]
    public void Inline_math_regex_has_a_finite_timeout()
    {
        var field = typeof(RichContentView).GetField("InlineMath", BindingFlags.NonPublic | BindingFlags.Static);
        var regex = Assert.IsType<Regex>(field?.GetValue(null));

        Assert.NotEqual(Regex.InfiniteMatchTimeout, regex.MatchTimeout);
        Assert.InRange(regex.MatchTimeout, TimeSpan.FromMilliseconds(1), TimeSpan.FromSeconds(1));
    }
}
