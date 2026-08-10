using Abituria.Models;
using Abituria.Services;
using Avalonia.Headless.XUnit;

namespace Abituria.Tests;

public sealed class ExerciseHintProviderTests
{
    [Fact]
    public void Authored_hints_take_precedence_without_rewriting_content()
    {
        var exercise = new LearningExercise
        {
            TopicId = "powers",
            Hints = ["Autorska pierwsza wskazówka.", "Autorska druga wskazówka."]
        };

        var hints = ExerciseHintProvider.GetHints(exercise);

        Assert.Same(exercise.Hints, hints);
        Assert.Equal(exercise.Hints, hints);
    }

    [AvaloniaFact]
    public void Every_active_exam_exercise_has_two_nonempty_displayed_hints()
    {
        var repository = new ContentRepository();
        var exercises = repository.Exams.SelectMany(exam => exam.Exercises).ToArray();

        Assert.NotEmpty(exercises);
        Assert.All(exercises, exercise =>
        {
            var hints = ExerciseHintProvider.GetHints(exercise);
            Assert.True(hints.Count >= 2, exercise.Id);
            Assert.All(hints, hint => Assert.False(string.IsNullOrWhiteSpace(hint)));
        });
    }

    [AvaloniaFact]
    public void Every_declared_exam_topic_receives_topic_specific_fallback_guidance()
    {
        var repository = new ContentRepository();

        Assert.All(repository.ExamTopics, topic =>
        {
            var hints = ExerciseHintProvider.GetHints(new LearningExercise { TopicId = topic.Id });
            Assert.Equal(2, hints.Count);
            Assert.DoesNotContain("Zapisz dane, oznacz niewiadomą", hints[0], StringComparison.Ordinal);
        });
    }

    [Fact]
    public void Unknown_topic_uses_safe_general_guidance()
    {
        var hints = ExerciseHintProvider.GetHints(new LearningExercise { TopicId = "nieznany-temat", Mode = "numeric" });

        Assert.Equal(2, hints.Count);
        Assert.Contains("Zapisz dane", hints[0], StringComparison.Ordinal);
        Assert.Contains("warunki", hints[1], StringComparison.Ordinal);
    }
}
