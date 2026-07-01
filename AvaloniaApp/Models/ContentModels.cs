using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Abituria.Models;

public sealed class FormulaCatalog
{
    public int SchemaVersion { get; set; }
    public List<ContentBlock> Introduction { get; set; } = [];
    public List<FormulaArticle> Articles { get; set; } = [];
}

public sealed class FormulaArticle
{
    public string Id { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<ContentBlock> Blocks { get; set; } = [];
}

public sealed class ChapterCatalog
{
    public int SchemaVersion { get; set; }
    public List<ContentBlock> Introduction { get; set; } = [];
    public List<ChapterArticle> Chapters { get; set; } = [];
}

public sealed class ChapterArticle
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string? RoadmapId { get; set; }
    public List<ContentBlock> Blocks { get; set; } = [];
    public bool IsAvailable => string.Equals(Status, "available", StringComparison.OrdinalIgnoreCase);
}

public sealed class ContentBlock
{
    public string Type { get; set; } = string.Empty;
    public string? Text { get; set; }
    public string? Asset { get; set; }
}

public sealed class ExamCatalog
{
    public int SchemaVersion { get; set; }
    public ExamDefinition Exam { get; set; } = new();
}

public sealed class ExamDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Session { get; set; } = string.Empty;
    public List<ContentBlock> Introduction { get; set; } = [];
    public List<ContentBlock> TopicIntroduction { get; set; } = [];
    public SourceDocument Source { get; set; } = new();
    public List<ExerciseTopicDefinition> Topics { get; set; } = [];
    public List<ExerciseDefinition> Exercises { get; set; } = [];
}

public sealed class SourceDocument
{
    public string Publisher { get; set; } = string.Empty;
    public string DocumentCode { get; set; } = string.Empty;
    public string ExamDate { get; set; } = string.Empty;
    public string QuestionPaperUrl { get; set; } = string.Empty;
    public string AnswerKeyUrl { get; set; } = string.Empty;
    public string VerifiedOn { get; set; } = string.Empty;
}

public sealed class ExerciseTopicDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<int> ExerciseNumbers { get; set; } = [];
}

public sealed class ExerciseDefinition
{
    public string Id { get; set; } = string.Empty;
    public string ExamId { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TopicId { get; set; } = string.Empty;
    public int SourcePage { get; set; }
    public string VerificationSource { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public List<string> Options { get; set; } = [];
    public int? CorrectOption { get; set; }
    public List<string> Hints { get; set; } = [];
    public string? RevealedAnswer { get; set; }
    public List<string> Assets { get; set; } = [];
    public bool IsMultipleChoice => string.Equals(Mode, "multipleChoice", StringComparison.OrdinalIgnoreCase);
}

public sealed class PlaceholderCatalog
{
    public int SchemaVersion { get; set; }
    public List<PlaceholderItem> Items { get; set; } = [];
}

public sealed class PlaceholderItem
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? RoadmapId { get; set; }
    public List<ContentBlock> Blocks { get; set; } = [];
}

public sealed class RoadmapCatalog
{
    public int SchemaVersion { get; set; }
    public List<ContentBlock> Introduction { get; set; } = [];
    public List<RoadmapItem> Items { get; set; } = [];
}

public sealed class RoadmapItem
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public RoadmapStatus Status { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public List<string> SourceRefs { get; set; } = [];
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoadmapStatus
{
    Migrated,
    Planned,
    Superseded
}

public enum ProfileKind
{
    Guest = 0,
    Password = 1
}

public sealed record LocalProfile(Guid Id, string DisplayName, ProfileKind Kind);

public sealed record RegistrationResult(bool Success, string Message, LocalProfile? Profile = null, string? RecoveryCode = null);

public sealed record AuthenticationResult(bool Success, string Message, LocalProfile? Profile = null);

public sealed record PasswordUpdateResult(bool Success, string Message, string? RecoveryCode = null);
