using System;
using System.IO;
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
        Chapters = Load<ChapterCatalog>("Content/chapters.json");
        Exam = Load<ExamCatalog>("Content/exam-2021-correction.json").Exam;
        Placeholders = Load<PlaceholderCatalog>("Content/placeholders.json");
        Roadmap = Load<RoadmapCatalog>("Content/roadmap.json");
    }

    public FormulaCatalog Formulas { get; }
    public ChapterCatalog Chapters { get; }
    public ExamDefinition Exam { get; }
    public PlaceholderCatalog Placeholders { get; }
    public RoadmapCatalog Roadmap { get; }

    private T Load<T>(string relativePath)
    {
        var uri = new Uri($"avares://Abituria/{relativePath}");
        using Stream stream = AssetLoader.Open(uri);
        return JsonSerializer.Deserialize<T>(stream, _jsonOptions)
            ?? throw new InvalidDataException($"Nie udało się odczytać zasobu {relativePath}.");
    }
}
