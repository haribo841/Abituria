using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Abituria.Models;

namespace Abituria.Services;

public sealed record CompoundAnswerResult(bool IsComplete, bool IsCorrect, string Message);

public sealed class CompoundAnswerEvaluator
{
    private readonly NumericAnswerEvaluator _numericEvaluator;

    public CompoundAnswerEvaluator(NumericAnswerEvaluator numericEvaluator)
    {
        ArgumentNullException.ThrowIfNull(numericEvaluator);
        _numericEvaluator = numericEvaluator;
    }

    public CompoundAnswerResult Evaluate(
        LearningExercise exercise,
        IReadOnlyDictionary<string, string?> answers)
    {
        ArgumentNullException.ThrowIfNull(exercise);
        ArgumentNullException.ThrowIfNull(answers);
        if (!exercise.IsCompound || exercise.AnswerParts.Count < 2)
            return new CompoundAnswerResult(false, false, "Zadanie nie ma poprawnie skonfigurowanej odpowiedzi złożonej.");

        foreach (var part in exercise.AnswerParts)
        {
            if (!answers.TryGetValue(part.Id, out var answer) || string.IsNullOrWhiteSpace(answer))
                return new CompoundAnswerResult(false, false, "Uzupełnij wszystkie części odpowiedzi.");
            if (!IsCorrect(part, answer))
                return new CompoundAnswerResult(true, false, "Co najmniej jedna część odpowiedzi jest niepoprawna. Sprawdź obliczenia i spróbuj ponownie.");
        }

        return new CompoundAnswerResult(true, true, "Wszystkie części odpowiedzi są poprawne. Zadanie zapisano jako ukończone.");
    }

    private bool IsCorrect(LearningAnswerPart part, string answer)
    {
        if (part.IsMultipleChoice)
            return int.TryParse(answer, NumberStyles.None, CultureInfo.InvariantCulture, out var selected) &&
                selected == part.CorrectOption;
        if (part.IsNumeric)
            return _numericEvaluator.EvaluatePart(part, answer).IsCorrect;
        if (part.IsText)
        {
            var normalized = NormalizeTextAnswer(answer);
            return part.AcceptedAnswers.Any(item =>
                string.Equals(NormalizeTextAnswer(item), normalized, StringComparison.Ordinal));
        }

        return false;
    }

    public static string NormalizeTextAnswer(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var builder = new StringBuilder(value.Length);
        foreach (var character in value.Trim())
        {
            if (char.IsWhiteSpace(character))
                continue;
            builder.Append(character switch
            {
                '\u2212' or '\u2013' or '\u2014' => '-',
                ';' => ',',
                _ => char.ToLowerInvariant(character)
            });
        }
        return builder.ToString();
    }
}
