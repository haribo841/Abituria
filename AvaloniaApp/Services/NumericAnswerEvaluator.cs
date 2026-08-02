using Abituria.Models;

namespace Abituria.Services;

public sealed record NumericAnswerResult(bool IsValidInput, bool IsCorrect, string Message);

public sealed class NumericAnswerEvaluator
{
    private readonly ExpressionCalculator _calculator;

    public NumericAnswerEvaluator(ExpressionCalculator calculator)
    {
        ArgumentNullException.ThrowIfNull(calculator);
        _calculator = calculator;
    }

    public NumericAnswerResult Evaluate(LearningExercise exercise, string? answer)
    {
        ArgumentNullException.ThrowIfNull(exercise);
        if (!exercise.IsNumeric || exercise.ExpectedValue is not double expected || !double.IsFinite(expected))
            return new NumericAnswerResult(false, false, "Zadanie nie ma poprawnie skonfigurowanej odpowiedzi liczbowej.");
        return Evaluate(expected, exercise.AbsoluteTolerance, exercise.RelativeTolerance, answer);
    }

    public NumericAnswerResult EvaluatePart(LearningAnswerPart part, string? answer)
    {
        ArgumentNullException.ThrowIfNull(part);
        if (!part.IsNumeric || part.ExpectedValue is not double expected || !double.IsFinite(expected))
            return new NumericAnswerResult(false, false, "Część zadania nie ma poprawnie skonfigurowanej odpowiedzi liczbowej.");
        return Evaluate(expected, part.AbsoluteTolerance, part.RelativeTolerance, answer);
    }

    private NumericAnswerResult Evaluate(
        double expected,
        double absoluteTolerance,
        double relativeTolerance,
        string? answer)
    {
        if (!ValidTolerance(absoluteTolerance) || !ValidTolerance(relativeTolerance))
            return new NumericAnswerResult(false, false, "Zadanie ma niepoprawnie skonfigurowaną tolerancję.");

        var calculation = _calculator.Evaluate(answer);
        if (!calculation.Success || calculation.Value is not double actual || !double.IsFinite(actual))
            return new NumericAnswerResult(false, false, calculation.Message);

        var difference = Math.Abs(actual - expected);
        var tolerance = Math.Max(
            absoluteTolerance,
            relativeTolerance * Math.Abs(expected));
        return difference <= tolerance
            ? new NumericAnswerResult(true, true, "Poprawny wynik. Zadanie zapisano jako ukończone.")
            : new NumericAnswerResult(true, false, "Wynik nie mieści się w dopuszczalnej tolerancji. Sprawdź obliczenia.");
    }

    private static bool ValidTolerance(double tolerance) => double.IsFinite(tolerance) && tolerance >= 0d;
}
