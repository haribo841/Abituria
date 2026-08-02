using Abituria.Models;
using Abituria.Services;

namespace Abituria.Tests;

public sealed class CompoundAnswerEvaluatorTests
{
    private readonly CompoundAnswerEvaluator _evaluator = new(
        new NumericAnswerEvaluator(new ExpressionCalculator()));

    [Fact]
    public void Choice_numeric_and_text_parts_must_all_be_correct()
    {
        var exercise = ValidExercise();
        var result = _evaluator.Evaluate(exercise, new Dictionary<string, string?>
        {
            ["choice"] = "2",
            ["numeric"] = "9/25",
            ["text"] = " <−2 ; 4> "
        });

        Assert.True(result.IsComplete);
        Assert.True(result.IsCorrect);
        Assert.Contains("Wszystkie części", result.Message, StringComparison.Ordinal);

        var decimalResult = _evaluator.Evaluate(exercise, new Dictionary<string, string?>
        {
            ["choice"] = "2",
            ["numeric"] = "0,36",
            ["text"] = "[-2,4]"
        });
        Assert.True(decimalResult.IsCorrect);
    }

    [Fact]
    public void Missing_blank_wrong_and_malformed_answers_do_not_complete_successfully()
    {
        var exercise = ValidExercise();

        var missing = _evaluator.Evaluate(exercise, new Dictionary<string, string?>
        {
            ["choice"] = "2"
        });
        Assert.False(missing.IsComplete);
        Assert.False(missing.IsCorrect);

        var blank = _evaluator.Evaluate(exercise, new Dictionary<string, string?>
        {
            ["choice"] = "2",
            ["numeric"] = " ",
            ["text"] = "[-2,4]"
        });
        Assert.False(blank.IsComplete);

        AssertIncorrect(exercise, "choice", "1");
        AssertIncorrect(exercise, "choice", "nie-liczba");
        AssertIncorrect(exercise, "numeric", "błąd");
        AssertIncorrect(exercise, "numeric", "NaN");
        AssertIncorrect(exercise, "text", "(-2,4)");
    }

    [Fact]
    public void Invalid_compound_contract_and_unknown_part_mode_are_rejected_without_throwing()
    {
        var ordinary = ValidExercise();
        ordinary.Mode = "numeric";
        var ordinaryResult = _evaluator.Evaluate(ordinary, new Dictionary<string, string?>());
        Assert.False(ordinaryResult.IsComplete);
        Assert.False(ordinaryResult.IsCorrect);

        var tooShort = ValidExercise();
        tooShort.AnswerParts.RemoveAt(2);
        tooShort.AnswerParts.RemoveAt(1);
        var shortResult = _evaluator.Evaluate(tooShort, new Dictionary<string, string?>());
        Assert.False(shortResult.IsComplete);

        var unknown = ValidExercise();
        unknown.AnswerParts[0].Mode = "unknown";
        var unknownResult = _evaluator.Evaluate(unknown, CorrectAnswers());
        Assert.True(unknownResult.IsComplete);
        Assert.False(unknownResult.IsCorrect);
    }

    [Fact]
    public void Text_normalization_handles_whitespace_case_semicolon_and_unicode_minus_variants()
    {
        Assert.Equal("[-2,4]", CompoundAnswerEvaluator.NormalizeTextAnswer(" [ −2 ; 4 ] "));
        Assert.Equal("[-2,4]", CompoundAnswerEvaluator.NormalizeTextAnswer("[–2,4]"));
        Assert.Equal("[-2,4]", CompoundAnswerEvaluator.NormalizeTextAnswer("[\u20142,4]"));
        Assert.Equal("prawda", CompoundAnswerEvaluator.NormalizeTextAnswer(" PRAWDA "));
        Assert.Throws<ArgumentNullException>(() => CompoundAnswerEvaluator.NormalizeTextAnswer(null!));
    }

    [Fact]
    public void Null_dependencies_and_arguments_are_rejected()
    {
        Assert.Throws<ArgumentNullException>(() => new CompoundAnswerEvaluator(null!));
        Assert.Throws<ArgumentNullException>(() => _evaluator.Evaluate(null!, CorrectAnswers()));
        Assert.Throws<ArgumentNullException>(() => _evaluator.Evaluate(ValidExercise(), null!));
    }

    private void AssertIncorrect(LearningExercise exercise, string key, string value)
    {
        var answers = CorrectAnswers();
        answers[key] = value;
        var result = _evaluator.Evaluate(exercise, answers);
        Assert.True(result.IsComplete);
        Assert.False(result.IsCorrect);
        Assert.Contains("niepoprawna", result.Message, StringComparison.Ordinal);
    }

    private static Dictionary<string, string?> CorrectAnswers() => new(StringComparer.Ordinal)
    {
        ["choice"] = "2",
        ["numeric"] = "0.36",
        ["text"] = "[-2,4]"
    };

    private static LearningExercise ValidExercise() => new()
    {
        Id = "compound-test",
        Mode = "compound",
        AnswerParts =
        [
            new LearningAnswerPart
            {
                Id = "choice",
                Prompt = "Wybierz.",
                Mode = "multipleChoice",
                Options = ["P", "F"],
                CorrectOption = 2
            },
            new LearningAnswerPart
            {
                Id = "numeric",
                Prompt = "Oblicz.",
                Mode = "numeric",
                ExpectedValue = 0.36
            },
            new LearningAnswerPart
            {
                Id = "text",
                Prompt = "Podaj przedział.",
                Mode = "text",
                AcceptedAnswers = ["[-2,4]", "<-2,4>"]
            }
        ]
    };
}
