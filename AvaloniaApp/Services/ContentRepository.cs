using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Abituria.Models;
using Avalonia.Platform;

namespace Abituria.Services;

public sealed class ContentRepository
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ContentRepository()
    {
        Formulas = Load<FormulaCatalog>("Content/formulas.json");
        Diagrams = Load<DiagramCatalog>("Content/diagrams.json");
        DiagramCatalogValidator.Validate(Diagrams);
        MathCourse = Load<MathCourseCatalog>("Content/chapters.json");
        CourseExercises = Load<CourseExerciseCatalog>("Content/course-exercises.json");
        ExamIndex = Load<ExamIndexCatalog>("Content/exams.json");
        Exams = ExamIndex.Exams
            .Where(item => item.IsActive)
            .OrderBy(item => item.Order)
            .Select(LoadExam)
            .ToArray();
        ExamCatalogValidator.Validate(ExamIndex, Exams);
        Placeholders = Load<PlaceholderCatalog>("Content/placeholders.json");
        Roadmap = Load<RoadmapCatalog>("Content/roadmap.json");
        UiCopy = Load<UiCopyCatalog>("Content/ui-copy.json");
    }

    public FormulaCatalog Formulas { get; }
    public DiagramCatalog Diagrams { get; }
    public MathCourseCatalog MathCourse { get; }
    public CourseExerciseCatalog CourseExercises { get; }
    public ExamIndexCatalog ExamIndex { get; }
    public IReadOnlyList<ExamDefinition> Exams { get; }
    public IReadOnlyList<ExerciseTopicDefinition> ExamTopics => ExamIndex.Topics;
    public ExamDefinition Exam => GetExam("matura-poprawkowa-2021");
    public PlaceholderCatalog Placeholders { get; }
    public RoadmapCatalog Roadmap { get; }
    public UiCopyCatalog UiCopy { get; }

    public ExamDefinition GetExam(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return Exams.SingleOrDefault(item => string.Equals(item.Id, id, StringComparison.Ordinal))
            ?? throw new KeyNotFoundException($"Nie znaleziono arkusza '{id}'.");
    }

    public IReadOnlyList<LearningExercise> GetTopicExercises(string topicId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(topicId);
        if (!ExamTopics.Any(item => string.Equals(item.Id, topicId, StringComparison.Ordinal)))
            throw new KeyNotFoundException($"Nie znaleziono tematu '{topicId}'.");
        return Exams.SelectMany(item => item.Exercises)
            .Where(item => string.Equals(item.TopicId, topicId, StringComparison.Ordinal))
            .ToArray();
    }

    private ExamDefinition LoadExam(ExamIndexEntry entry)
    {
        if (string.IsNullOrWhiteSpace(entry.ContentPath) ||
            !entry.ContentPath.StartsWith("Content/", StringComparison.Ordinal) ||
            entry.ContentPath.Contains("..", StringComparison.Ordinal))
            throw new InvalidDataException($"Nieprawidłowa ścieżka arkusza: {entry.ContentPath}.");
        var catalog = Load<ExamCatalog>(entry.ContentPath);
        if (catalog.SchemaVersion is not (3 or 4))
            throw new InvalidDataException($"Nieobsługiwany schemat arkusza '{entry.Id}': {catalog.SchemaVersion}.");
        if (!string.Equals(catalog.Exam.Id, entry.Id, StringComparison.Ordinal))
            throw new InvalidDataException($"Identyfikator arkusza w pliku nie odpowiada indeksowi: {entry.Id}.");
        return catalog.Exam;
    }

    private T Load<T>(string relativePath)
    {
        var uri = new Uri($"avares://Abituria/{relativePath}");
        using Stream stream = AssetLoader.Open(uri);
        return JsonSerializer.Deserialize<T>(stream, _jsonOptions)
            ?? throw new InvalidDataException($"Nie udało się odczytać zasobu {relativePath}.");
    }
}
