using System;
using System.Collections.Generic;

namespace Abituria.Models;

public sealed class FormulaCatalog
{
    public int SchemaVersion { get; set; }
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
    public List<ChapterArticle> Chapters { get; set; } = [];
}

public sealed class ChapterArticle
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }
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
    public List<ExerciseDefinition> Exercises { get; set; } = [];
}

public sealed class ExerciseDefinition
{
    public string Id { get; set; } = string.Empty;
    public string ExamId { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
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
