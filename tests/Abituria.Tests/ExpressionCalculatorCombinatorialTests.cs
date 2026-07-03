using Abituria.Services;
using System.Globalization;

namespace Abituria.Tests;

public sealed class ExpressionCalculatorCombinatorialTests
{
    private static readonly (string Text, double Value)[] Operands =
    [
        ("0", 0d),
        ("1", 1d),
        ("-1", -1d),
        ("2,5", 2.5d),
        ("-3.25", -3.25d),
        ("4E2", 400d),
        ("5E-2", 0.05d)
    ];

    private static readonly (string Symbol, Func<double, double, double> Apply)[] Operators =
    [
        ("+", (left, right) => left + right),
        ("-", (left, right) => left - right),
        ("*", (left, right) => left * right),
        ("×", (left, right) => left * right),
        ("/", (left, right) => left / right),
        ("÷", (left, right) => left / right),
        ("^", Math.Pow)
    ];

    private static readonly (string Text, double Value)[] RootExpressions =
    [
        ("√9", 3d),
        ("sqrt(16)", 4d),
        ("∛(-8)", -2d),
        ("∜16", 2d),
        ("root(5;32)", 2d),
        ("root(3;-27)", -3d),
        ("√(2+7)", 3d)
    ];

    private readonly ExpressionCalculator _calculator = new();

    [Fact]
    public void Evaluates_every_binary_operator_for_every_operand_pair()
    {
        foreach (var left in Operands)
            foreach (var operation in Operators)
                foreach (var right in Operands)
                {
                    var expression = $"({left.Text}){operation.Symbol}({right.Text})";
                    var result = _calculator.Evaluate(expression);

                    if (operation.Symbol is "/" or "÷" && right.Value == 0d)
                    {
                        AssertError(expression, result, CalculationErrorCode.DivisionByZero);
                        continue;
                    }

                    if (operation.Symbol == "^" && left.Value == 0d && right.Value == 0d)
                    {
                        AssertError(expression, result, CalculationErrorCode.UndefinedPower);
                        continue;
                    }

                    if (operation.Symbol == "^" && left.Value == 0d && right.Value < 0d)
                    {
                        AssertError(expression, result, CalculationErrorCode.DivisionByZero);
                        continue;
                    }

                    var expected = operation.Apply(left.Value, right.Value);
                    if (double.IsNaN(expected))
                    {
                        AssertError(expression, result, CalculationErrorCode.UndefinedPower);
                        continue;
                    }

                    if (!double.IsFinite(expected))
                    {
                        AssertError(expression, result, CalculationErrorCode.NonFiniteResult);
                        continue;
                    }

                    if (expected == 0d && left.Value != 0d && right.Value != 0d &&
                        operation.Symbol is "*" or "×" or "/" or "÷" or "^")
                    {
                        AssertError(expression, result, CalculationErrorCode.NonFiniteResult);
                        continue;
                    }

                    AssertEquivalent(expression, expected, result);
                }
    }

    [Fact]
    public void Applies_precedence_and_associativity_for_every_operator_pair()
    {
        var symbols = Operators.Select(item => item.Symbol).ToArray();
        foreach (var first in symbols)
            foreach (var second in symbols)
            {
                var expression = $"2{first}3{second}2";
                var explicitExpression = ExpectedGrouping("2", first, "3", second, "2");
                var expected = _calculator.Evaluate(explicitExpression);
                var actual = _calculator.Evaluate(expression);

                Assert.True(expected.Success, $"Wyrażenie kontrolne {explicitExpression}: {expected.Message}");
                AssertEquivalent(expression, expected.Value!.Value, actual);
            }
    }

    [Fact]
    public void Evaluates_all_root_degrees_for_exact_positive_and_negative_powers()
    {
        for (var degree = 2; degree <= 15; degree++)
            for (var root = -5; root <= 5; root++)
            {
                if (root < 0 && degree % 2 == 0) continue;

                var radicand = Math.Pow(root, degree);
                var expression = $"root({degree};{radicand:R})";
                AssertEquivalent(expression, root, _calculator.Evaluate(expression));
            }

        var symbolCases = new (string Symbol, int Degree)[] { ("√", 2), ("∛", 3), ("∜", 4) };
        foreach (var (symbol, degree) in symbolCases)
            for (var root = degree % 2 == 0 ? 0 : -10; root <= 10; root++)
            {
                var radicand = Math.Pow(root, degree);
                var expression = $"{symbol}({radicand:R})";
                AssertEquivalent(expression, root, _calculator.Evaluate(expression));
            }
    }

    [Fact]
    public void Combines_every_root_notation_with_every_binary_operator()
    {
        foreach (var left in RootExpressions)
            foreach (var operation in Operators)
                foreach (var right in RootExpressions)
                {
                    var expression = $"({left.Text}){operation.Symbol}({right.Text})";
                    var expected = operation.Apply(left.Value, right.Value);
                    var result = _calculator.Evaluate(expression);

                    if (double.IsNaN(expected))
                        AssertError(expression, result, CalculationErrorCode.UndefinedPower);
                    else if (!double.IsFinite(expected))
                        AssertError(expression, result, CalculationErrorCode.NonFiniteResult);
                    else
                        AssertEquivalent(expression, expected, result);
                }
    }

    [Fact]
    public void Evaluates_every_sequence_of_up_to_eight_unary_signs()
    {
        for (var length = 1; length <= 8; length++)
        {
            var variants = 1 << length;
            for (var mask = 0; mask < variants; mask++)
            {
                var signs = new char[length];
                var minusCount = 0;
                for (var index = 0; index < length; index++)
                {
                    var minus = (mask & (1 << index)) != 0;
                    signs[index] = minus ? '-' : '+';
                    if (minus) minusCount++;
                }

                var prefix = new string(signs);
                var multiplier = minusCount % 2 == 0 ? 1d : -1d;
                AssertEquivalent(prefix + "2^2", multiplier * 4d, _calculator.Evaluate(prefix + "2^2"));
                AssertEquivalent(prefix + "√9", multiplier * 3d, _calculator.Evaluate(prefix + "√9"));
            }
        }
    }

    [Fact]
    public void Evaluates_all_supported_implicit_multiplication_starts()
    {
        var leftExpressions = new (string Text, double Value)[]
        {
            ("2", 2d), ("-3", -3d), ("1,5", 1.5d), ("2^3", 8d), ("√9", 3d)
        };
        var rightExpressions = new (string Text, double Value, double? Ans)[]
        {
            ("(3+4)", 7d, null),
            ("√9", 3d, null),
            ("∛8", 2d, null),
            ("∜16", 2d, null),
            ("sqrt(25)", 5d, null),
            ("root(3;27)", 3d, null),
            ("Ans", 4d, 4d)
        };

        foreach (var left in leftExpressions)
            foreach (var right in rightExpressions)
            {
                var expression = $"({left.Text}){right.Text}";
                var result = _calculator.Evaluate(expression, right.Ans);
                AssertEquivalent(expression, left.Value * right.Value, result);
            }
    }

    [Fact]
    public void Accepts_parentheses_at_the_limit_and_rejects_the_next_level()
    {
        var atLimit = new string('(', ExpressionCalculator.MaxNestingDepth) + "1" +
                      new string(')', ExpressionCalculator.MaxNestingDepth);
        var beyondLimit = "(" + atLimit + ")";

        AssertEquivalent(atLimit, 1d, _calculator.Evaluate(atLimit));
        AssertError(beyondLimit, _calculator.Evaluate(beyondLimit), CalculationErrorCode.ExpressionTooDeep);
    }

    [Fact]
    public void Handles_whitespace_decimal_separators_and_scientific_notation_in_combinations()
    {
        var whitespace = new[] { "", " ", "\t", "\r\n" };
        var leftNumbers = new (string Text, double Value)[]
        {
            (".5", 0.5d), (",5", 0.5d), ("5.", 5d), ("5,", 5d),
            ("1.25E2", 125d), ("1,25e+2", 125d), ("5E-3", 0.005d)
        };

        foreach (var left in leftNumbers)
            foreach (var operation in Operators.Where(item => item.Symbol != "^"))
                foreach (var space in whitespace)
                {
                    var expression = $"{space}{left.Text}{space}{operation.Symbol}{space}2{space}";
                    var expected = operation.Apply(left.Value, 2d);
                    AssertEquivalent(expression, expected, _calculator.Evaluate(expression));
                }
    }

    [Fact]
    public void Parsing_is_identical_under_polish_english_german_french_and_turkish_cultures()
    {
        var expressions = new (string Text, double Expected)[]
        {
            ("1,5+2.25", 3.75d),
            ("1.80000000000018E-13", 1.80000000000018E-13d),
            ("1,8e+3/2", 900d),
            ("root(3;-8)+√9", 1d),
            ("9*(1/9)", 1d)
        };
        var cultures = new[] { "pl-PL", "en-US", "de-DE", "fr-FR", "tr-TR" };
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            foreach (var cultureName in cultures)
            {
                var culture = CultureInfo.GetCultureInfo(cultureName);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

                foreach (var expression in expressions)
                    AssertEquivalent(expression.Text, expression.Expected, _calculator.Evaluate(expression.Text));
            }
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    private static string ExpectedGrouping(string left, string first, string middle, string second, string right)
    {
        var firstPrecedence = Precedence(first);
        var secondPrecedence = Precedence(second);
        if (firstPrecedence < secondPrecedence) return $"{left}{first}({middle}{second}{right})";
        if (firstPrecedence > secondPrecedence) return $"({left}{first}{middle}){second}{right}";
        return first == "^"
            ? $"{left}{first}({middle}{second}{right})"
            : $"({left}{first}{middle}){second}{right}";
    }

    private static int Precedence(string symbol) => symbol switch
    {
        "+" or "-" => 1,
        "*" or "×" or "/" or "÷" => 2,
        "^" => 3,
        _ => throw new ArgumentOutOfRangeException(nameof(symbol))
    };

    private static void AssertEquivalent(string expression, double expected, CalculationResult result)
    {
        Assert.True(result.Success, $"{expression}: {result.ErrorCode}: {result.Message}");
        var actual = Assert.IsType<double>(result.Value);
        var tolerance = Math.Max(1E-12d, Math.Abs(expected) * 1E-12d);
        Assert.InRange(Math.Abs(actual - expected), 0d, tolerance);
        Assert.True(double.IsFinite(actual), expression);
        Assert.DoesNotContain("NaN", result.DisplayValue, StringComparison.OrdinalIgnoreCase);
    }

    private static void AssertError(string expression, CalculationResult result, CalculationErrorCode expectedCode)
    {
        Assert.False(result.Success, expression);
        Assert.Equal(expectedCode, result.ErrorCode);
        Assert.Null(result.Value);
        Assert.NotEmpty(result.Message);
        Assert.DoesNotContain("NaN", result.Message, StringComparison.OrdinalIgnoreCase);
    }
}
