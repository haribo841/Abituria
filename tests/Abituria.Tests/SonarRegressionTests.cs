using System.Reflection;
using Abituria.Services;
using Abituria.Ui;
using Abituria.Views;
using Avalonia.Controls;

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

    [Theory]
    [InlineData(@"\(x")]
    [InlineData(@"x\)")]
    [InlineData(@"\)x\(")]
    [InlineData(@"\(outer \(nested\)\)")]
    [InlineData(@"\(   \)")]
    public void Inline_math_validator_rejects_malformed_lines(string line)
    {
        Assert.False(RichContentView.HasBalancedInlineMathDelimiters(line));
    }

    [Fact]
    public void Scanner_requested_static_members_and_concrete_return_types_are_preserved()
    {
        const BindingFlags privateInstance = BindingFlags.NonPublic | BindingFlags.Instance;
        const BindingFlags privateStatic = BindingFlags.NonPublic | BindingFlags.Static;
        var normalizedInsertion = typeof(CalculatorInputState).GetMethod(nameof(CalculatorInputState.CreateNormalizedInsertion));
        var evaluateWithRepeat = typeof(ExpressionCalculator).GetMethod("EvaluateWithRepeat", privateStatic);
        var tokenize = typeof(ExpressionCalculator).GetMethod("Tokenize", privateStatic);

        Assert.NotNull(normalizedInsertion);
        Assert.True(normalizedInsertion.IsStatic);
        Assert.NotNull(evaluateWithRepeat);
        Assert.True(evaluateWithRepeat.IsStatic);
        Assert.NotNull(tokenize);
        Assert.Equal(typeof(List<>), tokenize.ReturnType.GetGenericTypeDefinition());
        Assert.All(typeof(ExerciseView).GetConstructors(), constructor =>
            Assert.InRange(constructor.GetParameters().Length, 0, 7));
        Assert.Equal(typeof(Border), typeof(GeneralCalculatorView).GetMethod("BuildKeypad", privateInstance)?.ReturnType);
        Assert.Equal(typeof(Border), typeof(GeneralCalculatorView).GetMethod("BuildHistoryPanel", privateInstance)?.ReturnType);
        Assert.Equal(typeof(Border), typeof(MainWindow).GetMethod("BuildTopBar", privateInstance)?.ReturnType);
        Assert.Equal(typeof(Grid), typeof(LoginView).GetMethod("Build", privateInstance)?.ReturnType);
    }
}
