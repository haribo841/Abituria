using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Abituria.Services;

public enum CalculationErrorCode
{
    EmptyExpression,
    ExpressionTooLong,
    ExpressionTooDeep,
    InvalidToken,
    UnexpectedToken,
    MissingClosingParenthesis,
    DivisionByZero,
    InvalidRoot,
    UndefinedPower,
    MissingAns,
    InvalidHistoryItem,
    NonFiniteResult
}

public sealed record CalculationResult(
    bool Success,
    double? Value,
    string DisplayValue,
    CalculationErrorCode? ErrorCode,
    string Message,
    int? ErrorPosition)
{
    public string? NormalizedExpression { get; init; }
    public bool WasNormalized { get; init; }
}

internal enum RepeatOperator
{
    Add,
    Subtract,
    Multiply,
    Divide,
    Power
}

internal sealed record RepeatOperation(RepeatOperator Operator, double Operand);

internal sealed record CalculationEvaluation(CalculationResult Result, RepeatOperation? RepeatOperation);

public sealed class ExpressionCalculator
{
    public const int MaxExpressionLength = 512;
    public const int MaxNestingDepth = 64;
    public const string LeadingZeroNormalizationMessage = "Usunięto niedozwolone zera wiodące.";

    private static readonly CultureInfo PolishCulture = CultureInfo.GetCultureInfo("pl-PL");

    public CalculationResult Evaluate(string? expression, double? ans = null) =>
        EvaluateWithRepeat(expression, ans).Result;

    internal CalculationEvaluation EvaluateWithRepeat(string? expression, double? ans = null)
    {
        var source = expression ?? string.Empty;
        if (string.IsNullOrWhiteSpace(source))
            return EvaluationError(CalculationErrorCode.EmptyExpression, "Wpisz wyrażenie do obliczenia.", 0);
        if (source.Length > MaxExpressionLength)
            return EvaluationError(CalculationErrorCode.ExpressionTooLong, $"Wyrażenie może mieć maksymalnie {MaxExpressionLength} znaków.", MaxExpressionLength);

        try
        {
            var tokens = Tokenize(source);
            var evaluation = new Parser(tokens, ans).Parse();
            var value = evaluation.Value;
            if (!double.IsFinite(value))
                throw new CalculationException(CalculationErrorCode.NonFiniteResult, "Wynik wykracza poza zakres obsługiwanych liczb.", source.Length);

            value = NormalizeZero(value);
            var normalizedExpression = NormalizeNumericLiterals(source);
            var wasNormalized = !string.Equals(source, normalizedExpression, StringComparison.Ordinal);
            var result = new CalculationResult(
                true,
                value,
                value.ToString("G15", PolishCulture),
                null,
                wasNormalized ? LeadingZeroNormalizationMessage : "Wynik",
                null)
            {
                NormalizedExpression = normalizedExpression,
                WasNormalized = wasNormalized
            };
            return new CalculationEvaluation(result, evaluation.RepeatOperation);
        }
        catch (CalculationException exception)
        {
            return EvaluationError(exception.Code, exception.Message, exception.Position);
        }
    }

    private static CalculationResult Error(CalculationErrorCode code, string message, int position) =>
        new(false, null, string.Empty, code, message, position);

    private static CalculationEvaluation EvaluationError(CalculationErrorCode code, string message, int position) =>
        new(Error(code, message, position), null);

    internal static string NormalizeNumericLiterals(string source)
    {
        if (string.IsNullOrEmpty(source)) return source;

        var normalized = new StringBuilder(source.Length);
        var index = 0;
        while (index < source.Length)
        {
            if (!StartsNumericLiteral(source[index]))
            {
                normalized.Append(source[index]);
                index++;
                continue;
            }

            index = AppendNormalizedNumericLiteral(source, index, normalized);
        }

        return normalized.ToString();
    }

    private static bool StartsNumericLiteral(char character) =>
        char.IsDigit(character) || character is '.' or ',';

    private static int AppendNormalizedNumericLiteral(string source, int start, StringBuilder normalized)
    {
        var mantissaEnd = FindMantissaEnd(source, start);
        var literalEnd = FindExponentEnd(source, mantissaEnd);
        var firstKeptDigit = FindFirstKeptIntegerDigit(source, start, mantissaEnd);
        normalized.Append(source, firstKeptDigit, literalEnd - firstKeptDigit);
        return literalEnd;
    }

    private static int FindMantissaEnd(string source, int start)
    {
        var index = start;
        while (index < source.Length && StartsNumericLiteral(source[index])) index++;
        return index;
    }

    private static int FindExponentEnd(string source, int mantissaEnd)
    {
        var index = mantissaEnd;
        if (index >= source.Length || source[index] is not ('e' or 'E')) return index;

        index++;
        if (index < source.Length && source[index] is '+' or '-') index++;
        while (index < source.Length && char.IsDigit(source[index])) index++;
        return index;
    }

    private static int FindFirstKeptIntegerDigit(string source, int start, int mantissaEnd)
    {
        var integerEnd = start;
        while (integerEnd < mantissaEnd && char.IsDigit(source[integerEnd])) integerEnd++;

        var firstKeptDigit = start;
        while (firstKeptDigit + 1 < integerEnd && source[firstKeptDigit] == '0') firstKeptDigit++;
        return firstKeptDigit;
    }

    private static double NormalizeZero(double value) =>
        Math.Abs(value) < double.Epsilon ? 0d : value;

    private static IReadOnlyList<Token> Tokenize(string source)
    {
        var tokens = new List<Token>();
        var index = 0;
        while (index < source.Length)
        {
            var character = source[index];
            if (char.IsWhiteSpace(character))
            {
                index++;
                continue;
            }

            if (char.IsDigit(character) || character is '.' or ',')
            {
                var start = index;
                var hasDigit = false;
                var hasSeparator = false;
                while (index < source.Length && (char.IsDigit(source[index]) || source[index] is '.' or ','))
                {
                    if (char.IsDigit(source[index])) hasDigit = true;
                    else if (hasSeparator)
                        throw new CalculationException(CalculationErrorCode.InvalidToken, "Liczba zawiera więcej niż jeden separator dziesiętny.", index);
                    else hasSeparator = true;
                    index++;
                }

                if (!hasDigit)
                    throw new CalculationException(CalculationErrorCode.InvalidToken, "Separator dziesiętny musi należeć do liczby.", start);

                if (index < source.Length && source[index] is 'e' or 'E')
                {
                    var exponentPosition = index;
                    index++;
                    if (index < source.Length && source[index] is '+' or '-') index++;

                    var exponentStart = index;
                    while (index < source.Length && char.IsDigit(source[index])) index++;
                    if (index == exponentStart)
                        throw new CalculationException(
                            CalculationErrorCode.InvalidToken,
                            "Wykładnik notacji naukowej musi zawierać cyfry.",
                            exponentPosition);
                }

                var text = source[start..index];
                var normalized = text.Replace(',', '.');
                if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) || !double.IsFinite(number))
                    throw new CalculationException(CalculationErrorCode.NonFiniteResult, "Liczba wykracza poza obsługiwany zakres.", start);
                tokens.Add(new Token(TokenType.Number, text, start, number));
                continue;
            }

            if (char.IsLetter(character))
            {
                var start = index;
                while (index < source.Length && char.IsLetter(source[index])) index++;
                var identifier = source[start..index];
                var normalizedIdentifier = identifier.ToLowerInvariant();
                if (normalizedIdentifier is "inf" or "infinity")
                    throw new CalculationException(
                        CalculationErrorCode.NonFiniteResult,
                        "Nieskończoność nie jest obsługiwaną liczbą rzeczywistą.",
                        start);

                var type = normalizedIdentifier switch
                {
                    "sqrt" => TokenType.Sqrt,
                    "root" => TokenType.Root,
                    "ans" => TokenType.Ans,
                    _ => throw new CalculationException(CalculationErrorCode.InvalidToken, $"Nieznana nazwa: {identifier}.", start)
                };
                tokens.Add(new Token(type, identifier, start));
                continue;
            }

            if (character == '∞')
                throw new CalculationException(
                    CalculationErrorCode.NonFiniteResult,
                    "Symbol ∞ nie jest liczbą rzeczywistą. Kalkulator nie oblicza granic.",
                    index);

            var tokenType = character switch
            {
                '+' => TokenType.Plus,
                '-' => TokenType.Minus,
                '*' or '×' => TokenType.Multiply,
                '/' or '÷' or ':' => TokenType.Divide,
                '^' => TokenType.Power,
                '(' => TokenType.LeftParenthesis,
                ')' => TokenType.RightParenthesis,
                ';' => TokenType.Semicolon,
                '√' => TokenType.SquareRoot,
                '∛' => TokenType.CubeRoot,
                '∜' => TokenType.FourthRoot,
                _ => throw new CalculationException(CalculationErrorCode.InvalidToken, $"Nieobsługiwany znak: {character}.", index)
            };
            tokens.Add(new Token(tokenType, character.ToString(), index));
            index++;
        }

        tokens.Add(new Token(TokenType.End, string.Empty, source.Length));
        return tokens;
    }

    private enum TokenType
    {
        Number,
        Plus,
        Minus,
        Multiply,
        Divide,
        Power,
        LeftParenthesis,
        RightParenthesis,
        Semicolon,
        SquareRoot,
        CubeRoot,
        FourthRoot,
        Sqrt,
        Root,
        Ans,
        End
    }

    private sealed record Token(TokenType Type, string Text, int Position, double Number = 0d);

    private readonly record struct Evaluation(double Value, RepeatOperation? RepeatOperation = null);

    private sealed class Parser(IReadOnlyList<Token> tokens, double? ans)
    {
        private int _depth;
        private int _index;

        private Token Current => tokens[_index];

        public Evaluation Parse()
        {
            var evaluation = ParseAdditive();
            if (Current.Type != TokenType.End)
                throw new CalculationException(CalculationErrorCode.UnexpectedToken, $"Nieoczekiwany element: {Current.Text}.", Current.Position);
            return evaluation;
        }

        private Evaluation ParseAdditive()
        {
            var evaluation = ParseMultiplicative();
            var value = evaluation.Value;
            var repeatOperation = evaluation.RepeatOperation;
            while (Current.Type is TokenType.Plus or TokenType.Minus)
            {
                var operation = Advance();
                var right = ParseMultiplicative();
                value = EnsureFinite(operation.Type == TokenType.Plus ? value + right.Value : value - right.Value, operation.Position);
                repeatOperation = new RepeatOperation(
                    operation.Type == TokenType.Plus ? RepeatOperator.Add : RepeatOperator.Subtract,
                    right.Value);
            }
            return new Evaluation(value, repeatOperation);
        }

        private Evaluation ParseMultiplicative()
        {
            var evaluation = ParseUnary();
            var value = evaluation.Value;
            var repeatOperation = evaluation.RepeatOperation;
            while (Current.Type is TokenType.Multiply or TokenType.Divide || StartsImplicitProduct(Current.Type))
            {
                var operation = Current.Type is TokenType.Multiply or TokenType.Divide ? Advance() : null;
                var right = ParseUnary();
                if (operation?.Type == TokenType.Divide)
                {
                    if (right.Value == 0d)
                        throw new CalculationException(CalculationErrorCode.DivisionByZero, "Nie można dzielić przez zero.", operation.Position);
                    var quotient = value / right.Value;
                    EnsureNoUnderflow(quotient, operation.Position, value);
                    value = EnsureFinite(quotient, operation.Position);
                    repeatOperation = new RepeatOperation(RepeatOperator.Divide, right.Value);
                }
                else
                {
                    var position = operation?.Position ?? Current.Position;
                    var product = value * right.Value;
                    EnsureNoUnderflow(product, position, value, right.Value);
                    value = EnsureFinite(product, position);
                    repeatOperation = new RepeatOperation(RepeatOperator.Multiply, right.Value);
                }
            }
            return new Evaluation(value, repeatOperation);
        }

        private Evaluation ParseUnary()
        {
            if (Current.Type is TokenType.Plus or TokenType.Minus)
            {
                var operation = Advance();
                var value = ParseNested(operation.Position, ParseUnary);
                return new Evaluation(operation.Type == TokenType.Minus ? -value.Value : value.Value);
            }
            return ParsePower();
        }

        private Evaluation ParsePower()
        {
            var evaluation = ParsePrimary();
            var value = evaluation.Value;
            if (Current.Type != TokenType.Power) return evaluation;

            var operation = Advance();
            var exponent = ParseNested(operation.Position, ParseUnary);
            if (value == 0d && exponent.Value == 0d)
                throw new CalculationException(CalculationErrorCode.UndefinedPower, "Wyrażenie 0^0 jest nieokreślone.", operation.Position);
            if (value == 0d && exponent.Value < 0d)
                throw new CalculationException(CalculationErrorCode.DivisionByZero, "Zero nie może być podniesione do ujemnej potęgi.", operation.Position);

            var result = Math.Pow(value, exponent.Value);
            if (double.IsNaN(result))
                throw new CalculationException(CalculationErrorCode.UndefinedPower, "Ta potęga nie ma wyniku w zbiorze liczb rzeczywistych.", operation.Position);
            EnsureNoUnderflow(result, operation.Position, value);
            return new Evaluation(
                EnsureFinite(result, operation.Position),
                new RepeatOperation(RepeatOperator.Power, exponent.Value));
        }

        private Evaluation ParsePrimary()
        {
            var token = Advance();
            return token.Type switch
            {
                TokenType.Number => new Evaluation(token.Number),
                TokenType.Ans => new Evaluation(ans ?? throw new CalculationException(CalculationErrorCode.MissingAns, "Brak poprzedniego wyniku Ans.", token.Position)),
                TokenType.LeftParenthesis => ParseParenthesized(token.Position),
                TokenType.Sqrt => ParseFunctionRoot(token, 2),
                TokenType.Root => ParseGeneralRoot(token),
                TokenType.SquareRoot => new Evaluation(ApplyRoot(2, ParseNested(token.Position, ParseUnary).Value, token.Position)),
                TokenType.CubeRoot => new Evaluation(ApplyRoot(3, ParseNested(token.Position, ParseUnary).Value, token.Position)),
                TokenType.FourthRoot => new Evaluation(ApplyRoot(4, ParseNested(token.Position, ParseUnary).Value, token.Position)),
                TokenType.End => throw new CalculationException(CalculationErrorCode.UnexpectedToken, "Wyrażenie jest niepełne.", token.Position),
                _ => throw new CalculationException(CalculationErrorCode.UnexpectedToken, $"Nieoczekiwany element: {token.Text}.", token.Position)
            };
        }

        private Evaluation ParseParenthesized(int position)
        {
            var value = ParseNested(position, ParseAdditive);
            ExpectClosingParenthesis();
            return value;
        }

        private Evaluation ParseFunctionRoot(Token function, int degree)
        {
            Expect(TokenType.LeftParenthesis, "Po nazwie sqrt musi wystąpić nawias otwierający.");
            var radicand = ParseNested(function.Position, ParseAdditive);
            ExpectClosingParenthesis();
            return new Evaluation(ApplyRoot(degree, radicand.Value, function.Position));
        }

        private Evaluation ParseGeneralRoot(Token function)
        {
            Expect(TokenType.LeftParenthesis, "Po nazwie root musi wystąpić nawias otwierający.");
            var degree = ParseNested(function.Position, ParseAdditive);
            Expect(TokenType.Semicolon, "Funkcja root wymaga separatora ';' między stopniem i liczbą.");
            var radicand = ParseNested(function.Position, ParseAdditive);
            ExpectClosingParenthesis();
            return new Evaluation(ApplyRoot(degree.Value, radicand.Value, function.Position));
        }

        private Evaluation ParseNested(int position, Func<Evaluation> parse)
        {
            if (_depth >= MaxNestingDepth)
                throw new CalculationException(CalculationErrorCode.ExpressionTooDeep, $"Wyrażenie może mieć maksymalnie {MaxNestingDepth} poziomy zagnieżdżenia.", position);
            _depth++;
            try
            {
                return parse();
            }
            finally
            {
                _depth--;
            }
        }

        private static double ApplyRoot(double degreeValue, double radicand, int position)
        {
            var roundedDegree = Math.Round(degreeValue);
            if (!double.IsFinite(degreeValue) || degreeValue < 2d || roundedDegree > int.MaxValue || Math.Abs(degreeValue - roundedDegree) > 0.000000000001d)
                throw new CalculationException(CalculationErrorCode.InvalidRoot, "Stopień pierwiastka musi być dodatnią liczbą całkowitą nie mniejszą niż 2.", position);

            var degree = (int)roundedDegree;
            if (radicand < 0d && degree % 2 == 0)
                throw new CalculationException(CalculationErrorCode.InvalidRoot, "Pierwiastek parzystego stopnia z liczby ujemnej nie jest liczbą rzeczywistą.", position);

            var result = radicand < 0d
                ? -Math.Pow(-radicand, 1d / degree)
                : Math.Pow(radicand, 1d / degree);
            return EnsureFinite(result, position);
        }

        private static double EnsureFinite(double value, int position)
        {
            if (!double.IsFinite(value))
                throw new CalculationException(CalculationErrorCode.NonFiniteResult, "Wynik wykracza poza zakres obsługiwanych liczb.", position);
            return value;
        }

        private static void EnsureNoUnderflow(double result, int position, params double[] inputs)
        {
            if (result != 0d || inputs.Any(input => input == 0d)) return;

            throw new CalculationException(
                CalculationErrorCode.NonFiniteResult,
                "Wynik pośredni jest zbyt mały, aby przedstawić go jako liczbę double.",
                position);
        }

        private static bool StartsImplicitProduct(TokenType type) => type is
            TokenType.LeftParenthesis or TokenType.SquareRoot or TokenType.CubeRoot or TokenType.FourthRoot or
            TokenType.Sqrt or TokenType.Root or TokenType.Ans;

        private Token Advance()
        {
            var token = Current;
            if (_index < tokens.Count - 1) _index++;
            return token;
        }

        private void Expect(TokenType type, string message)
        {
            if (Current.Type != type)
                throw new CalculationException(CalculationErrorCode.UnexpectedToken, message, Current.Position);
            Advance();
        }

        private void ExpectClosingParenthesis()
        {
            if (Current.Type != TokenType.RightParenthesis)
                throw new CalculationException(CalculationErrorCode.MissingClosingParenthesis, "Brakuje nawiasu zamykającego.", Current.Position);
            Advance();
        }
    }

    public sealed class CalculationException(CalculationErrorCode code, string message, int position) : Exception(message)
    {
        public CalculationErrorCode Code { get; } = code;
        public int Position { get; } = position;
    }
}
