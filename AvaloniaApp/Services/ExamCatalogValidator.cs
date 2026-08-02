using System;
using System.Collections.Generic;
using System.Linq;
using Abituria.Models;

namespace Abituria.Services;

public static class ExamCatalogValidator
{
    private static readonly HashSet<string> SupportedModes =
        new(["multipleChoice", "numeric", "revealOnly", "compound"], StringComparer.Ordinal);

    private static readonly HashSet<string> SupportedPartModes =
        new(["multipleChoice", "numeric", "text"], StringComparer.Ordinal);

    public static void Validate(ExamIndexCatalog index, IReadOnlyList<ExamDefinition> exams)
    {
        ArgumentNullException.ThrowIfNull(index);
        ArgumentNullException.ThrowIfNull(exams);
        if (index.SchemaVersion != 1)
            throw new InvalidOperationException($"Nieobsługiwany schemat indeksu arkuszy: {index.SchemaVersion}.");
        if (index.Exams.Count == 0 || index.Topics.Count == 0)
            throw new InvalidOperationException("Indeks arkuszy musi zawierać arkusze i tematy.");

        ValidateIndexEntries(index.Exams);
        ValidateTopics(index.Topics);

        RequireUnique(index.Exams.Select(item => item.Id), "arkusza w indeksie");
        RequireUnique(index.Exams.Select(item => item.ContentPath), "ścieżki arkusza");
        RequireUnique(index.Exams.Select(item => item.Order), "kolejności arkusza");
        RequireUnique(index.Topics.Select(item => item.Id), "tematu");
        RequireUnique(index.Topics.Select(item => item.Order), "kolejności tematu");
        RequireUnique(exams.Select(item => item.Id), "załadowanego arkusza");

        var activeEntries = index.Exams.Where(item => item.IsActive).OrderBy(item => item.Order).ToArray();
        if (activeEntries.Length == 0)
            throw new InvalidOperationException("Indeks nie zawiera aktywnego arkusza.");
        if (activeEntries.Length != exams.Count)
            throw new InvalidOperationException("Liczba aktywnych wpisów indeksu nie odpowiada liczbie załadowanych arkuszy.");
        if (!activeEntries.Select(item => item.Id).SequenceEqual(exams.Select(item => item.Id), StringComparer.Ordinal))
            throw new InvalidOperationException("Załadowane arkusze nie zachowują kolejności aktywnego indeksu.");
        if (!activeEntries.Select(item => item.Level).SequenceEqual(exams.Select(item => item.Level), StringComparer.Ordinal))
            throw new InvalidOperationException("Poziomy załadowanych arkuszy nie odpowiadają aktywnemu indeksowi.");

        var topicIds = index.Topics.Select(item => item.Id).ToHashSet(StringComparer.Ordinal);
        var allExerciseIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var exam in exams)
            ValidateExam(exam, topicIds, allExerciseIds);
    }

    private static void ValidateIndexEntries(IEnumerable<ExamIndexEntry> entries)
    {
        if (entries.Any(item => string.IsNullOrWhiteSpace(item.Id) ||
                string.IsNullOrWhiteSpace(item.ContentPath) || string.IsNullOrWhiteSpace(item.Level) ||
                item.Order <= 0))
            throw new InvalidOperationException("Wpis indeksu arkuszy ma niepełne dane.");
    }

    private static void ValidateTopics(IEnumerable<ExerciseTopicDefinition> topics)
    {
        if (topics.Any(item => string.IsNullOrWhiteSpace(item.Id) ||
                string.IsNullOrWhiteSpace(item.Title) || item.Order <= 0))
            throw new InvalidOperationException("Temat arkuszy ma niepełne dane.");
    }

    private static void ValidateExam(
        ExamDefinition exam,
        HashSet<string> topicIds,
        HashSet<string> allExerciseIds)
    {
        if (string.IsNullOrWhiteSpace(exam.Id) || string.IsNullOrWhiteSpace(exam.Title) ||
            string.IsNullOrWhiteSpace(exam.Session) || string.IsNullOrWhiteSpace(exam.Formula) ||
            string.IsNullOrWhiteSpace(exam.Level))
            throw new InvalidOperationException("Arkusz musi mieć identyfikator i tytuł.");
        if (exam.Year <= 0 || exam.DurationMinutes <= 0 || exam.MaximumPoints <= 0 ||
            exam.OfficialTaskCount <= 0 || exam.ProgressItemCount <= 0)
            throw new InvalidOperationException($"Arkusz '{exam.Id}' ma niepełne metadane egzaminu.");
        if (exam.Exercises.Count != exam.ProgressItemCount)
            throw new InvalidOperationException($"Arkusz '{exam.Id}' nie ma oczekiwanej liczby jednostek postępu.");
        if (string.IsNullOrWhiteSpace(exam.Source.Publisher) || string.IsNullOrWhiteSpace(exam.Source.DocumentCode) ||
            string.IsNullOrWhiteSpace(exam.Source.QuestionPaperUrl) || string.IsNullOrWhiteSpace(exam.Source.AnswerKeyUrl))
            throw new InvalidOperationException($"Arkusz '{exam.Id}' ma niepełne źródło.");

        var orders = new HashSet<int>();
        foreach (var exercise in exam.Exercises)
        {
            if (string.IsNullOrWhiteSpace(exercise.Id) || exercise.Id.Length >= 80 ||
                !allExerciseIds.Add(exercise.Id))
                throw new InvalidOperationException($"Arkusz '{exam.Id}' zawiera nieprawidłowy albo powtórzony identyfikator zadania.");
            if (!string.Equals(exercise.ExamId, exam.Id, StringComparison.Ordinal))
                throw new InvalidOperationException($"Zadanie '{exercise.Id}' wskazuje niewłaściwy arkusz.");
            if (!topicIds.Contains(exercise.TopicId))
                throw new InvalidOperationException($"Zadanie '{exercise.Id}' wskazuje nieznany temat.");
            if (!orders.Add(exercise.EffectiveOrder))
                throw new InvalidOperationException($"Arkusz '{exam.Id}' zawiera powtórzoną kolejność zadania.");
            ValidateExercise(exercise);
        }

        var officialGroups = exam.Exercises.Select(item => string.IsNullOrWhiteSpace(item.GroupId) ? item.Id : item.GroupId);
        if (officialGroups.Distinct(StringComparer.Ordinal).Count() != exam.OfficialTaskCount)
            throw new InvalidOperationException($"Arkusz '{exam.Id}' nie ma oczekiwanej liczby oficjalnych zadań.");
    }

    private static void ValidateExercise(LearningExercise exercise)
    {
        if (!SupportedModes.Contains(exercise.Mode))
            throw new InvalidOperationException($"Zadanie '{exercise.Id}' ma nieznany tryb odpowiedzi.");
        if (string.IsNullOrWhiteSpace(exercise.Prompt) || string.IsNullOrWhiteSpace(exercise.VerificationSource) ||
            exercise.SourcePage <= 0)
            throw new InvalidOperationException($"Zadanie '{exercise.Id}' ma niepełną treść albo źródło.");
        if (exercise.IsMultipleChoice &&
            (exercise.Options.Count < 2 || exercise.CorrectOption is null ||
             exercise.CorrectOption < 1 || exercise.CorrectOption > exercise.Options.Count))
            throw new InvalidOperationException($"Zadanie '{exercise.Id}' ma niepoprawną odpowiedź wyboru.");
        if (exercise.IsNumeric &&
            (exercise.ExpectedValue is not double expected || !double.IsFinite(expected)))
            throw new InvalidOperationException($"Zadanie '{exercise.Id}' ma niepoprawną odpowiedź liczbową.");
        if (exercise.IsRevealOnly && string.IsNullOrWhiteSpace(exercise.EffectiveSolution))
            throw new InvalidOperationException($"Zadanie '{exercise.Id}' nie ma rozwiązania do ujawnienia.");
        if (exercise.IsCompound)
            ValidateAnswerParts(exercise);
    }

    private static void ValidateAnswerParts(LearningExercise exercise)
    {
        if (exercise.AnswerParts.Count < 2)
            throw new InvalidOperationException($"Zadanie '{exercise.Id}' nie ma złożonej odpowiedzi.");
        RequireUnique(exercise.AnswerParts.Select(item => item.Id), "części odpowiedzi");
        foreach (var part in exercise.AnswerParts)
        {
            if (string.IsNullOrWhiteSpace(part.Id) || string.IsNullOrWhiteSpace(part.Prompt) ||
                !SupportedPartModes.Contains(part.Mode))
                throw new InvalidOperationException($"Zadanie '{exercise.Id}' ma niepełną część odpowiedzi.");
            if (part.IsMultipleChoice &&
                (part.Options.Count < 2 || part.CorrectOption is null ||
                 part.CorrectOption < 1 || part.CorrectOption > part.Options.Count))
                throw new InvalidOperationException($"Część '{part.Id}' ma niepoprawny wybór.");
            if (part.IsNumeric &&
                (part.ExpectedValue is not double expected || !double.IsFinite(expected)))
                throw new InvalidOperationException($"Część '{part.Id}' ma niepoprawną wartość liczbową.");
            if (part.IsText && (part.AcceptedAnswers.Count == 0 || part.AcceptedAnswers.Any(string.IsNullOrWhiteSpace)))
                throw new InvalidOperationException($"Część '{part.Id}' nie ma akceptowanych odpowiedzi tekstowych.");
        }
    }

    private static void RequireUnique<T>(IEnumerable<T> values, string label)
        where T : notnull
    {
        var duplicate = values.GroupBy(value => value).FirstOrDefault(group => group.Count() != 1);
        if (duplicate is not null)
            throw new InvalidOperationException($"Powtórzony identyfikator {label}: {duplicate.Key}.");
    }
}
