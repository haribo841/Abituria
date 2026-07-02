using System;
using System.Collections.Generic;
using System.Globalization;

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
    NonFiniteResult
}

public sealed record CalculationResult(
    bool Success,
    double? Value,
    string DisplayValue,
    CalculationErrorCode? ErrorCode,
    string Message,
    int? ErrorPosition);

public sealed class ExpressionCalculator
{
    public const int MaxExpressionLength = 512;
    public const int MaxNestingDepth = 64;

    private static readonly CultureInfo PolishCulture = CultureInfo.GetCultureInfo("pl-PL");

    public CalculationResult Evaluate(string? expression, double? ans = null)
    {
        var source = expression ?? string.Empty;
        if (string.IsNullOrWhiteSpace(source))
            return Error(CalculationErrorCode.EmptyExpression, "Wpisz wyrażenie do obliczenia.", 0);
        if (source.Length > MaxExpressionLength)
            return Error(CalculationErrorCode.ExpressionTooLong, $"Wyrażenie może mieć maksymalnie {MaxExpressionLength} znaków.", MaxExpressionLength);

        try
        {
            var tokens = Tokenize(source);
            var value = new Parser(tokens, ans).Parse();
            if (!double.IsFinite(value))
                throw new CalculationException(CalculationErrorCode.NonFiniteResult, "Wynik wykracza poza zakres obsługiwanych liczb.", source.Length);

            if (value == 0d) value = 0d;
            return new CalculationResult(true, value, value.ToString("G15", PolishCulture), null, "Wynik", null);
        }
        catch (CalculationException exception)
        {
            return Error(exception.Code, exception.Message, exception.Position);
        }
    }

    private static CalculationResult Error(CalculationErrorCode code, string message, int position) =>
        new(false, null, string.Empty, code, message, position);

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

                var text = source[start..index];
                var normalized = text.Replace(',', '.');
                if (!double.TryParse(normalized, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var number) || !double.IsFinite(number))
                    throw new CalculationException(CalculationErrorCode.NonFiniteResult, "Liczba wykracza poza obsługiwany zakres.", start);
                tokens.Add(new Token(TokenType.Number, text, start, number));
                continue;
            }

            if (char.IsLetter(character))
            {
                var start = index;
                while (index < source.Length && char.IsLetter(source[index])) index++;
                var identifier = source[start..index];
                var type = identifier.ToLowerInvariant() switch
                {
                    "sqrt" => TokenType.Sqrt,
                    "root" => TokenType.Root,
                    "ans" => TokenType.Ans,
                    _ => throw new CalculationException(CalculationErrorCode.InvalidToken, $"Nieznana nazwa: {identifier}.", start)
                };
                tokens.Add(new Token(type, identifier, start));
                continue;
            }

            var tokenType = character switch
            {
                '+' => TokenType.Plus,
                '-' => TokenType.Minus,
                '*' or '×' => TokenType.Multiply,
                '/' or '÷' => TokenType.Divide,
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

    private sealed class Parser(IReadOnlyList<Token> tokens, double? ans)
    {
        private int _depth;
        private int _index;

        private Token Current => tokens[_index];

        public double Parse()
        {
            var value = ParseAdditive();
            if (Current.Type != TokenType.End)
                throw new CalculationException(CalculationErrorCode.UnexpectedToken, $"Nieoczekiwany element: {Current.Text}.", Current.Position);
            return value;
        }

        private double ParseAdditive()
        {
            var value = ParseMultiplicative();
            while (Current.Type is TokenType.Plus or TokenType.Minus)
            {
                var operation = Advance();
                var right = ParseMultiplicative();
                value = EnsureFinite(operation.Type == TokenType.Plus ? value + right : value - right, operation.Position);
            }
            return value;
        }

        private double ParseMultiplicative()
        {
            var value = ParseUnary();
            while (Current.Type is TokenType.Multiply or TokenType.Divide || StartsImplicitProduct(Current.Type))
            {
                var operation = Current.Type is TokenType.Multiply or TokenType.Divide ? Advance() : null;
                var right = ParseUnary();
                if (operation?.Type == TokenType.Divide)
                {
                    if (right == 0d)
                        throw new CalculationException(CalculationErrorCode.DivisionByZero, "Nie można dzielić przez zero.", operation.Position);
                    value = EnsureFinite(value / right, operation.Position);
                }
                else
                {
                    value = EnsureFinite(value * right, operation?.Position ?? Current.Position);
                }
            }
            return value;
        }

        private double ParseUnary()
        {
            if (Current.Type is TokenType.Plus or TokenType.Minus)
            {
                var operation = Advance();
                var value = ParseNested(operation.Position, ParseUnary);
                return operation.Type == TokenType.Minus ? -value : value;
            }
            return ParsePower();
        }

        private double ParsePower()
        {
            var value = ParsePrimary();
            if (Current.Type != TokenType.Power) return value;

            var operation = Advance();
            var exponent = ParseNested(operation.Position, ParseUnary);
            if (value == 0d && exponent == 0d)
                throw new CalculationException(CalculationErrorCode.UndefinedPower, "Wyrażenie 0^0 jest nieokreślone.", operation.Position);
            if (value == 0d && exponent < 0d)
                throw new CalculationException(CalculationErrorCode.DivisionByZero, "Zero nie może być podniesione do ujemnej potęgi.", operation.Position);

            var result = Math.Pow(value, exponent);
            if (double.IsNaN(result))
                throw new CalculationException(CalculationErrorCode.UndefinedPower, "Ta potęga nie ma wyniku w zbiorze liczb rzeczywistych.", operation.Position);
            return EnsureFinite(result, operation.Position);
        }

        private double ParsePrimary()
        {
            var token = Advance();
            return token.Type switch
            {
                TokenType.Number => token.Number,
                TokenType.Ans => ans ?? throw new CalculationException(CalculationErrorCode.MissingAns, "Brak poprzedniego wyniku Ans.", token.Position),
                TokenType.LeftParenthesis => ParseParenthesized(token.Position),
                TokenType.Sqrt => ParseFunctionRoot(token, 2),
                TokenType.Root => ParseGeneralRoot(token),
                TokenType.SquareRoot => ApplyRoot(2, ParseNested(token.Position, ParseUnary), token.Position),
                TokenType.CubeRoot => ApplyRoot(3, ParseNested(token.Position, ParseUnary), token.Position),
                TokenType.FourthRoot => ApplyRoot(4, ParseNested(token.Position, ParseUnary), token.Position),
                TokenType.End => throw new CalculationException(CalculationErrorCode.UnexpectedToken, "Wyrażenie jest niepełne.", token.Position),
                _ => throw new CalculationException(CalculationErrorCode.UnexpectedToken, $"Nieoczekiwany element: {token.Text}.", token.Position)
            };
        }

        private double ParseParenthesized(int position)
        {
            var value = ParseNested(position, ParseAdditive);
            ExpectClosingParenthesis();
            return value;
        }

        private double ParseFunctionRoot(Token function, int degree)
        {
            Expect(TokenType.LeftParenthesis, "Po nazwie sqrt musi wystąpić nawias otwierający.");
            var radicand = ParseNested(function.Position, ParseAdditive);
            ExpectClosingParenthesis();
            return ApplyRoot(degree, radicand, function.Position);
        }

        private double ParseGeneralRoot(Token function)
        {
            Expect(TokenType.LeftParenthesis, "Po nazwie root musi wystąpić nawias otwierający.");
            var degree = ParseNested(function.Position, ParseAdditive);
            Expect(TokenType.Semicolon, "Funkcja root wymaga separatora ';' między stopniem i liczbą.");
            var radicand = ParseNested(function.Position, ParseAdditive);
            ExpectClosingParenthesis();
            return ApplyRoot(degree, radicand, function.Position);
        }

        private double ParseNested(int position, Func<double> parse)
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

    private sealed class CalculationException(CalculationErrorCode code, string message, int position) : Exception(message)
    {
        public CalculationErrorCode Code { get; } = code;
        public int Position { get; } = position;
    }
}
