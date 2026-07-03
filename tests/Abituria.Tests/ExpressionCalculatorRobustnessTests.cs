using System.Globalization;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Abituria.Services;
using Xunit.Sdk;

namespace Abituria.Tests;

public sealed class ExpressionCalculatorRobustnessTests
{
    private static readonly string[] FuzzTokens =
    [
        "0", "1", ".", ",", "E", "+", "-", "*", "/", "^", "(", ")",
        "√", "∛", "∜", ";", "Ans", "sqrt", "root", "@", ":"
    ];

    private readonly ExpressionCalculator _calculator = new();

    [Fact]
    public void Every_token_fragment_combination_up_to_length_four_is_controlled()
    {
        AssertControlled(null);
        AssertControlled(string.Empty);

        for (var length = 1; length <= 4; length++)
        {
            var combinations = (int)Math.Pow(FuzzTokens.Length, length);
            for (var encoded = 0; encoded < combinations; encoded++)
            {
                var value = encoded;
                var parts = new string[length];
                for (var position = 0; position < length; position++)
                {
                    parts[position] = FuzzTokens[value % FuzzTokens.Length];
                    value /= FuzzTokens.Length;
                }

                AssertControlled(string.Concat(parts), 7d);
            }
        }
    }

    [Fact]
    public void Ten_thousand_generated_expression_trees_match_their_independent_values()
    {
        var random = new Random(841_2026);
        for (var index = 0; index < 10_000; index++)
        {
            var generated = Generate(random, random.Next(2, 6));
            var result = _calculator.Evaluate(generated.Text);

            Assert.True(result.Success, $"Przypadek {index}: {generated.Text}: {result.ErrorCode}: {result.Message}");
            var actual = result.Value!.Value;
            var tolerance = Math.Max(1E-11d, Math.Abs(generated.Value) * 1E-11d);
            Assert.InRange(Math.Abs(actual - generated.Value), 0d, tolerance);
            Assert.True(double.IsFinite(actual), generated.Text);
        }
    }

    [Fact]
    public void Twenty_thousand_finite_double_values_round_trip_through_parser_syntax()
    {
        var random = new Random(2957_43);
        var bytes = new byte[sizeof(long)];
        var checkedValues = 0;

        while (checkedValues < 20_000)
        {
            random.NextBytes(bytes);
            var value = BitConverter.Int64BitsToDouble(BitConverter.ToInt64(bytes));
            if (!double.IsFinite(value)) continue;

            var expression = value.ToString("R", CultureInfo.InvariantCulture);
            var result = _calculator.Evaluate(expression);

            Assert.True(result.Success, $"{expression}: {result.ErrorCode}: {result.Message}");
            Assert.Equal(value, result.Value!.Value);
            checkedValues++;
        }
    }

    [Fact]
    public void Random_malformed_unicode_input_never_escapes_as_an_exception()
    {
        const string alphabet = "0123456789+-*/^(),.;:√∛∜×÷eEAnsrootxyz@#$%\0\t \r\n";
        var random = new Random(404_004);

        for (var sample = 0; sample < 20_000; sample++)
        {
            var length = random.Next(0, 80);
            var characters = new char[length];
            for (var index = 0; index < length; index++)
                characters[index] = alphabet[random.Next(alphabet.Length)];

            AssertControlled(new string(characters), random.Next(-10, 11));
        }
    }

    [Fact]
    public void Every_composite_zero_denominator_returns_division_by_zero_without_exception()
    {
        var numerators = new[] { "0", "1", "-7", "√9", "2^8", "Ans", "root(3;27)" };
        var zeroDenominators = new[]
        {
            "0", "-0", "0,0", "0.0E+100", "(1-1)", "√0", "∛0", "∜0",
            "sqrt(0)", "root(3;0)", "root(15;0)", "0^2", "Ans-Ans", "-(-0)"
        };
        var divisionSymbols = new[] { "/", "÷" };

        foreach (var numerator in numerators)
            foreach (var division in divisionSymbols)
                foreach (var denominator in zeroDenominators)
                {
                    var expression = $"({numerator}){division}({denominator})";
                    CalculationResult result;
                    try
                    {
                        result = _calculator.Evaluate(expression, 11d);
                    }
                    catch (Exception exception)
                    {
                        throw new XunitException($"{expression} zgłosiło {exception.GetType().Name}: {exception.Message}");
                    }

                    Assert.False(result.Success, expression);
                    Assert.Equal(CalculationErrorCode.DivisionByZero, result.ErrorCode);
                    Assert.Null(result.Value);
                    Assert.DoesNotContain("NaN", result.Message, StringComparison.OrdinalIgnoreCase);
                }
    }

    [Fact]
    public void Boundary_length_depth_and_numeric_magnitude_inputs_are_controlled()
    {
        var atLengthLimit = new string('1', ExpressionCalculator.MaxExpressionLength);
        var beyondLengthLimit = atLengthLimit + "1";
        var extremeInputs = new[]
        {
            atLengthLimit,
            beyondLengthLimit,
            "1E308",
            "1E309",
            "5E-324",
            "1E-9999",
            double.MaxValue.ToString("R", CultureInfo.InvariantCulture),
            double.MinValue.ToString("R", CultureInfo.InvariantCulture),
            "0^-999999999999999999999999999999999999999"
        };

        foreach (var expression in extremeInputs) AssertControlled(expression);
        Assert.Equal(CalculationErrorCode.ExpressionTooLong, _calculator.Evaluate(beyondLengthLimit).ErrorCode);

        for (var depth = 0; depth <= ExpressionCalculator.MaxNestingDepth + 2; depth++)
        {
            var expression = new string('(', depth) + "1" + new string(')', depth);
            AssertControlled(expression);
            var result = _calculator.Evaluate(expression);
            if (depth <= ExpressionCalculator.MaxNestingDepth) Assert.True(result.Success, expression);
            else Assert.Equal(CalculationErrorCode.ExpressionTooDeep, result.ErrorCode);
        }
    }

    [Fact]
    public void Shared_calculator_is_thread_safe_across_fifty_thousand_evaluations()
    {
        var failures = new ConcurrentQueue<string>();

        Parallel.For(0, 50_000, index =>
        {
            var left = index % 97 - 48;
            var middle = index % 31 + 1;
            var multiplier = index % 11 - 5;
            var denominator = index % 13 + 1;
            var expression = $"(({left}+{middle})*{multiplier})/{denominator}";
            var expected = ((left + middle) * multiplier) / (double)denominator;

            try
            {
                var result = _calculator.Evaluate(expression);
                if (!result.Success || result.Value is null || Math.Abs(result.Value.Value - expected) > 1E-12d)
                    failures.Enqueue($"{expression}: {result.ErrorCode}: {result.Message}: {result.Value}");
            }
            catch (Exception exception)
            {
                failures.Enqueue($"{expression}: {exception.GetType().Name}: {exception.Message}");
            }
        });

        Assert.Empty(failures);
    }

    private void AssertControlled(string? expression, double? ans = null)
    {
        CalculationResult result;
        try
        {
            result = _calculator.Evaluate(expression, ans);
        }
        catch (Exception exception)
        {
            throw new XunitException($"{Printable(expression)} zgłosiło {exception.GetType().Name}: {exception.Message}");
        }

        if (result.Success)
        {
            if (result.Value is not double value || !double.IsFinite(value))
                throw new XunitException($"{Printable(expression)} zwróciło niefinitywny wynik.");
            if (result.ErrorCode is not null || result.ErrorPosition is not null)
                throw new XunitException($"{Printable(expression)} zwróciło sukces razem z błędem.");
        }
        else if (result.ErrorCode is null || result.ErrorPosition is null || string.IsNullOrWhiteSpace(result.Message))
        {
            throw new XunitException($"{Printable(expression)} zwróciło niekompletny błąd.");
        }

        if (ContainsNaNToken(result.DisplayValue + result.Message))
            throw new XunitException($"{Printable(expression)} ujawniło NaN.");
    }

    private static GeneratedExpression Generate(Random random, int depth)
    {
        if (depth <= 0) return GenerateLiteral(random);

        return random.Next(8) switch
        {
            0 => GenerateLiteral(random),
            1 => GenerateUnary(random, depth),
            2 => GenerateAdditive(random, depth),
            3 => GenerateMultiplicative(random, depth),
            4 => GenerateDivision(random, depth),
            5 => GeneratePower(random),
            6 => GenerateRoot(random),
            _ => GenerateImplicitProduct(random, depth)
        };
    }

    private static GeneratedExpression GenerateLiteral(Random random)
    {
        var style = random.Next(3);
        if (style == 0)
        {
            var integer = random.Next(-20, 21);
            return new GeneratedExpression(integer.ToString(CultureInfo.InvariantCulture), integer);
        }

        if (style == 1)
        {
            var value = random.Next(-40, 41) / 2d;
            var text = value.ToString("R", CultureInfo.InvariantCulture);
            if (random.Next(2) == 0) text = text.Replace('.', ',');
            return new GeneratedExpression(text, value);
        }

        var mantissa = random.Next(1, 10) * (random.Next(2) == 0 ? 1 : -1);
        var exponent = random.Next(-6, 7);
        var scientificValue = mantissa * Math.Pow(10d, exponent);
        var exponentSign = exponent >= 0 ? "+" : string.Empty;
        return new GeneratedExpression($"{mantissa}E{exponentSign}{exponent}", scientificValue);
    }

    private static GeneratedExpression GenerateUnary(Random random, int depth)
    {
        var child = Generate(random, depth - 1);
        var sign = random.Next(2) == 0 ? '+' : '-';
        return new GeneratedExpression($"{sign}({child.Text})", sign == '-' ? -child.Value : child.Value);
    }

    private static GeneratedExpression GenerateAdditive(Random random, int depth)
    {
        var left = Generate(random, depth - 1);
        var right = Generate(random, depth - 1);
        var symbol = random.Next(2) == 0 ? '+' : '-';
        var value = symbol == '+' ? left.Value + right.Value : left.Value - right.Value;
        return IsSafe(value)
            ? new GeneratedExpression($"({left.Text}){symbol}({right.Text})", value)
            : GenerateLiteral(random);
    }

    private static GeneratedExpression GenerateMultiplicative(Random random, int depth)
    {
        var left = Generate(random, depth - 1);
        var right = Generate(random, depth - 1);
        var value = left.Value * right.Value;
        var symbol = random.Next(2) == 0 ? "*" : "×";
        return IsSafe(value)
            ? new GeneratedExpression($"({left.Text}){symbol}({right.Text})", value)
            : GenerateLiteral(random);
    }

    private static GeneratedExpression GenerateDivision(Random random, int depth)
    {
        var left = Generate(random, depth - 1);
        var right = Generate(random, depth - 1);
        if (right.Value == 0d) right = new GeneratedExpression("1", 1d);
        var value = left.Value / right.Value;
        var symbol = random.Next(2) == 0 ? "/" : "÷";
        return IsSafe(value)
            ? new GeneratedExpression($"({left.Text}){symbol}({right.Text})", value)
            : GenerateLiteral(random);
    }

    private static GeneratedExpression GeneratePower(Random random)
    {
        var baseValue = random.Next(-8, 9);
        if (baseValue == 0) baseValue = 1;
        var exponent = random.Next(-4, 7);
        var value = Math.Pow(baseValue, exponent);
        return new GeneratedExpression($"({baseValue})^({exponent})", value);
    }

    private static GeneratedExpression GenerateRoot(Random random)
    {
        var degree = random.Next(2, 8);
        var expectedRoot = random.Next(-5, 6);
        if (expectedRoot < 0 && degree % 2 == 0) expectedRoot = -expectedRoot;
        var radicand = Math.Pow(expectedRoot, degree);
        var text = degree switch
        {
            2 when random.Next(2) == 0 => $"sqrt({radicand:R})",
            2 => $"√({radicand:R})",
            3 when random.Next(2) == 0 => $"∛({radicand:R})",
            4 when random.Next(2) == 0 => $"∜({radicand:R})",
            _ => $"root({degree};{radicand:R})"
        };
        return new GeneratedExpression(text, expectedRoot);
    }

    private static GeneratedExpression GenerateImplicitProduct(Random random, int depth)
    {
        var left = Generate(random, depth - 1);
        var right = Generate(random, depth - 1);
        var value = left.Value * right.Value;
        return IsSafe(value)
            ? new GeneratedExpression($"({left.Text})({right.Text})", value)
            : GenerateLiteral(random);
    }

    private static bool IsSafe(double value) => double.IsFinite(value) && Math.Abs(value) <= 1E100d;

    private static bool ContainsNaNToken(string value) => Regex.IsMatch(
        value,
        @"(?<![\p{L}\p{N}_])NaN(?![\p{L}\p{N}_])",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static string Printable(string? expression) => expression is null
        ? "<null>"
        : '"' + expression.Replace("\0", "\\0", StringComparison.Ordinal) + '"';

    private sealed record GeneratedExpression(string Text, double Value);
}
