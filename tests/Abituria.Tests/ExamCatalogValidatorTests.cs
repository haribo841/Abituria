using System.Text.Json;
using Abituria.Models;
using Abituria.Services;

namespace Abituria.Tests;

public sealed class ExamCatalogValidatorTests
{
    private static readonly JsonSerializerOptions JsonOptions = new();

    [Fact]
    public void Validator_accepts_a_complete_catalog_and_rejects_null_or_invalid_index_contracts()
    {
        var (index, exams) = ValidCatalog();
        ExamCatalogValidator.Validate(index, exams);

        Assert.Throws<ArgumentNullException>(() => ExamCatalogValidator.Validate(null!, exams));
        Assert.Throws<ArgumentNullException>(() => ExamCatalogValidator.Validate(index, null!));
        AssertInvalid((candidate, _) => candidate.SchemaVersion = 2);
        AssertInvalid((candidate, _) => candidate.Exams.Clear());
        AssertInvalid((candidate, _) => candidate.Topics.Clear());
        AssertInvalid((candidate, _) => candidate.Exams[0].Id = "");
        AssertInvalid((candidate, _) => candidate.Exams[0].ContentPath = "");
        AssertInvalid((candidate, _) => candidate.Exams[0].Level = "");
        AssertInvalid((candidate, _) => candidate.Exams[0].Order = 0);
        AssertInvalid((candidate, _) => candidate.Topics[0].Id = "");
        AssertInvalid((candidate, _) => candidate.Topics[0].Title = "");
        AssertInvalid((candidate, _) => candidate.Topics[0].Order = 0);
        AssertInvalid((candidate, _) => candidate.Exams.Add(Clone(candidate.Exams[0])));
        AssertInvalid((candidate, _) =>
        {
            var duplicate = Clone(candidate.Exams[0]);
            duplicate.Id = "other";
            candidate.Exams.Add(duplicate);
        });
        AssertInvalid((candidate, _) =>
        {
            var duplicate = Clone(candidate.Exams[0]);
            duplicate.Id = "other";
            duplicate.ContentPath = "Content/other.json";
            candidate.Exams.Add(duplicate);
        });
        AssertInvalid((candidate, _) => candidate.Topics.Add(Clone(candidate.Topics[0])));
        AssertInvalid((candidate, _) =>
        {
            var duplicateOrder = Clone(candidate.Topics[0]);
            duplicateOrder.Id = "other-topic";
            candidate.Topics.Add(duplicateOrder);
        });
        AssertInvalid((candidate, loaded) => loaded.Clear());
        AssertInvalid((candidate, loaded) =>
        {
            candidate.Exams[0].IsActive = false;
            loaded.Clear();
        });
        AssertInvalid((candidate, loaded) =>
        {
            candidate.Exams[0].IsActive = false;
            loaded.Clear();
            loaded.Add(ValidExam("unexpected"));
        });
        AssertInvalid((candidate, _) => candidate.Exams[0].Level = "extended");
    }

    [Fact]
    public void Validator_rejects_duplicate_loaded_exams_and_active_order_mismatches()
    {
        AssertInvalid((index, exams) =>
        {
            index.Exams.Add(new ExamIndexEntry
            {
                Id = "second",
                ContentPath = "Content/second.json",
                Level = "basic",
                Order = 2,
                IsActive = true
            });
            exams.Add(Clone(exams[0]));
        });

        AssertInvalid((index, exams) =>
        {
            index.Exams.Add(new ExamIndexEntry
            {
                Id = "second",
                ContentPath = "Content/second.json",
                Level = "basic",
                Order = 2,
                IsActive = true
            });
            exams.Add(ValidExam("second"));
            exams.Reverse();
        });
    }

    [Fact]
    public void Validator_requires_complete_exam_metadata_source_and_progress_count()
    {
        Action<ExamDefinition>[] mutations =
        [
            exam => exam.Id = "",
            exam => exam.Title = " ",
            exam => exam.Session = "",
            exam => exam.Formula = "",
            exam => exam.Level = "",
            exam => exam.Year = 0,
            exam => exam.DurationMinutes = 0,
            exam => exam.MaximumPoints = 0,
            exam => exam.OfficialTaskCount = 0,
            exam => exam.ProgressItemCount = 0,
            exam => exam.Exercises.Clear(),
            exam => exam.Source.Publisher = "",
            exam => exam.Source.DocumentCode = "",
            exam => exam.Source.QuestionPaperUrl = "",
            exam => exam.Source.AnswerKeyUrl = ""
        ];

        foreach (var mutation in mutations)
            AssertInvalid((_, exams) => mutation(exams[0]));

        AssertInvalid((_, exams) => exams[0].OfficialTaskCount = 2);
    }

    [Fact]
    public void Validator_rejects_invalid_exercise_identity_mapping_order_and_source()
    {
        AssertInvalidExercise(exercise => exercise.Id = "");
        AssertInvalidExercise(exercise => exercise.Id = new string('x', 80));
        AssertInvalidExercise(exercise => exercise.ExamId = "other");
        AssertInvalidExercise(exercise => exercise.TopicId = "other");
        AssertInvalidExercise(exercise => exercise.Mode = "other");
        AssertInvalidExercise(exercise => exercise.Prompt = "");
        AssertInvalidExercise(exercise => exercise.VerificationSource = "");
        AssertInvalidExercise(exercise => exercise.SourcePage = 0);

        AssertInvalid((_, exams) =>
        {
            var duplicate = Clone(exams[0].Exercises[0]);
            duplicate.Order = 2;
            duplicate.Number = 2;
            exams[0].Exercises.Add(duplicate);
            exams[0].ProgressItemCount = 2;
        });
        AssertInvalid((_, exams) =>
        {
            var duplicateOrder = Clone(exams[0].Exercises[0]);
            duplicateOrder.Id = "second-exercise";
            exams[0].Exercises.Add(duplicateOrder);
            exams[0].ProgressItemCount = 2;
        });
    }

    [Fact]
    public void Validator_rejects_invalid_single_answer_contracts()
    {
        AssertInvalidExercise(exercise => exercise.Options = ["A"]);
        AssertInvalidExercise(exercise => exercise.CorrectOption = null);
        AssertInvalidExercise(exercise => exercise.CorrectOption = 3);
        AssertInvalidExercise(exercise =>
        {
            exercise.Mode = "numeric";
            exercise.ExpectedValue = null;
        });
        AssertInvalidExercise(exercise =>
        {
            exercise.Mode = "numeric";
            exercise.ExpectedValue = double.PositiveInfinity;
        });
        AssertInvalidExercise(exercise =>
        {
            exercise.Mode = "revealOnly";
            exercise.Solution = null;
            exercise.RevealedAnswer = null;
        });
    }

    [Fact]
    public void Validator_rejects_invalid_compound_answer_parts()
    {
        AssertInvalidCompound(exercise => exercise.AnswerParts.RemoveAt(1));
        AssertInvalidCompound(exercise => exercise.AnswerParts[1].Id = exercise.AnswerParts[0].Id);
        AssertInvalidCompound(exercise => exercise.AnswerParts[0].Id = "");
        AssertInvalidCompound(exercise => exercise.AnswerParts[0].Prompt = "");
        AssertInvalidCompound(exercise => exercise.AnswerParts[0].Mode = "other");
        AssertInvalidCompound(exercise => exercise.AnswerParts[0].Options = ["P"]);
        AssertInvalidCompound(exercise => exercise.AnswerParts[0].CorrectOption = null);
        AssertInvalidCompound(exercise => exercise.AnswerParts[0].CorrectOption = 3);
        AssertInvalidCompound(exercise =>
        {
            exercise.AnswerParts[1].Mode = "numeric";
            exercise.AnswerParts[1].ExpectedValue = null;
        });
        AssertInvalidCompound(exercise =>
        {
            exercise.AnswerParts[1].Mode = "numeric";
            exercise.AnswerParts[1].ExpectedValue = double.NaN;
        });
        AssertInvalidCompound(exercise =>
        {
            exercise.AnswerParts[1].Mode = "text";
            exercise.AnswerParts[1].AcceptedAnswers.Clear();
        });
        AssertInvalidCompound(exercise =>
        {
            exercise.AnswerParts[1].Mode = "text";
            exercise.AnswerParts[1].AcceptedAnswers = [" "];
        });
    }

    private static void AssertInvalidExercise(Action<LearningExercise> mutation) =>
        AssertInvalid((_, exams) => mutation(exams[0].Exercises[0]));

    private static void AssertInvalidCompound(Action<LearningExercise> mutation) => AssertInvalid((_, exams) =>
    {
        var exercise = exams[0].Exercises[0];
        exercise.Mode = "compound";
        exercise.Options.Clear();
        exercise.CorrectOption = null;
        exercise.AnswerParts =
        [
            new LearningAnswerPart
            {
                Id = "a",
                Prompt = "Pierwsze zdanie.",
                Mode = "multipleChoice",
                Options = ["P", "F"],
                CorrectOption = 1
            },
            new LearningAnswerPart
            {
                Id = "b",
                Prompt = "Drugie zdanie.",
                Mode = "multipleChoice",
                Options = ["P", "F"],
                CorrectOption = 2
            }
        ];
        mutation(exercise);
    });

    private static void AssertInvalid(Action<ExamIndexCatalog, List<ExamDefinition>> mutation)
    {
        var (index, exams) = ValidCatalog();
        mutation(index, exams);
        Assert.Throws<InvalidOperationException>(() => ExamCatalogValidator.Validate(index, exams));
    }

    private static (ExamIndexCatalog Index, List<ExamDefinition> Exams) ValidCatalog()
    {
        var index = new ExamIndexCatalog
        {
            SchemaVersion = 1,
            Topics = [new ExerciseTopicDefinition { Id = "topic", Order = 1, Title = "Temat" }],
            Exams =
            [
                new ExamIndexEntry
                {
                    Id = "exam",
                    ContentPath = "Content/exam.json",
                    Level = "basic",
                    Order = 1,
                    IsActive = true
                }
            ]
        };
        return (index, [ValidExam("exam")]);
    }

    private static ExamDefinition ValidExam(string id) => new()
    {
        Id = id,
        Title = "Arkusz",
        Year = 2026,
        Session = "główna",
        Formula = "2023",
        Level = "basic",
        DurationMinutes = 180,
        MaximumPoints = 1,
        OfficialTaskCount = 1,
        ProgressItemCount = 1,
        Source = new SourceDocument
        {
            Publisher = "CKE",
            DocumentCode = "CODE",
            QuestionPaperUrl = "https://example.test/paper.pdf",
            AnswerKeyUrl = "https://example.test/rules.pdf"
        },
        Exercises =
        [
            new LearningExercise
            {
                Id = $"{id}-exercise",
                ExamId = id,
                Number = 1,
                Order = 1,
                Points = 1,
                Title = "Zadanie 1",
                TopicId = "topic",
                SourcePage = 1,
                VerificationSource = "test",
                Mode = "multipleChoice",
                Prompt = "Wybierz odpowiedź.",
                Options = ["A", "B"],
                CorrectOption = 1
            }
        ]
    };

    private static T Clone<T>(T value) => JsonSerializer.Deserialize<T>(
        JsonSerializer.Serialize(value, JsonOptions),
        JsonOptions) ?? throw new InvalidDataException("Nie można sklonować danych testowych.");
}
