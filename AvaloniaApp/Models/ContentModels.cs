using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;

namespace Abituria.Models;

public sealed class UiCopyCatalog
{
    public int SchemaVersion { get; set; }
    public Dictionary<string, UiCopyEntry> Entries { get; set; } = new(StringComparer.Ordinal);

    public UiCopyEntry GetRequired(string key) => Entries.TryGetValue(key, out var entry)
        ? entry
        : throw new KeyNotFoundException($"Brak treści interfejsu: {key}.");

    public UiCopyEntry FormatRequired(string key, params object[] arguments)
    {
        var entry = GetRequired(key);
        return new UiCopyEntry
        {
            Title = entry.Title,
            Body = string.Format(CultureInfo.GetCultureInfo("pl-PL"), entry.Body, arguments)
        };
    }
}

public sealed class UiCopyEntry
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public sealed class FormulaCatalog
{
    public int SchemaVersion { get; set; }
    public FormulaSourceDocument Source { get; set; } = new();
    public List<ContentBlock> Introduction { get; set; } = [];
    public List<FormulaArticle> Articles { get; set; } = [];
}

public sealed class FormulaSourceDocument
{
    public string Publisher { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public string DocumentSha256 { get; set; } = string.Empty;
    public string PublishedOn { get; set; } = string.Empty;
    public string VerifiedOn { get; set; } = string.Empty;
}

public sealed class DiagramCatalog
{
    public int SchemaVersion { get; set; }
    public List<DiagramDefinition> Diagrams { get; set; } = [];

    public DiagramDefinition GetRequired(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var matches = Diagrams.Where(item => string.Equals(item.Id, id, StringComparison.Ordinal)).Take(2).ToArray();
        return matches.Length == 1
            ? matches[0]
            : throw new KeyNotFoundException($"Diagram '{id}' nie istnieje albo jego identyfikator nie jest unikalny.");
    }
}

public sealed class DiagramDefinition
{
    public string Id { get; set; } = string.Empty;
    public string SourceId { get; set; } = string.Empty;
    public int SourcePage { get; set; }
    public string AlternativeText { get; set; } = string.Empty;
    public double Width { get; set; }
    public double Height { get; set; }
    public List<DiagramPrimitive> Primitives { get; set; } = [];
}

public sealed class DiagramPrimitive
{
    public string Type { get; set; } = string.Empty;
    public List<double> Points { get; set; } = [];
    public double X { get; set; }
    public double Y { get; set; }
    public double X2 { get; set; }
    public double Y2 { get; set; }
    public double RadiusX { get; set; }
    public double RadiusY { get; set; }
    public double StartAngle { get; set; }
    public double SweepAngle { get; set; }
    public string? Text { get; set; }
    public string Stroke { get; set; } = "primary";
    public string Fill { get; set; } = "none";
    public double StrokeThickness { get; set; } = 2;
    public double FontSize { get; set; } = 22;
    public bool Dashed { get; set; }
    public bool ArrowStart { get; set; }
    public bool ArrowEnd { get; set; }
}

public sealed class FormulaArticle
{
    public string Id { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<ContentBlock> Blocks { get; set; } = [];
}

public sealed class MathCourseCatalog
{
    public int SchemaVersion { get; set; }
    public string Author { get; set; } = string.Empty;
    public List<CourseSourceDocument> Sources { get; set; } = [];
    public List<ContentBlock> Introduction { get; set; } = [];
    public List<CourseGroup> Groups { get; set; } = [];
    public List<CourseArea> Areas { get; set; } = [];
    public List<CourseRequirement> Requirements { get; set; } = [];
    public List<MathCourseLesson> Lessons { get; set; } = [];
}

public sealed class CourseSourceDocument
{
    public string Id { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public string DocumentSha256 { get; set; } = string.Empty;
    public string PublishedOn { get; set; } = string.Empty;
    public string VerifiedOn { get; set; } = string.Empty;
}

public sealed class CourseGroup
{
    public string Id { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<string> AreaIds { get; set; } = [];
}

public sealed class CourseArea
{
    public string Id { get; set; } = string.Empty;
    public int Order { get; set; }
    public string OfficialNumber { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<string> LessonIds { get; set; } = [];
}

public sealed class CourseRequirement
{
    public string Id { get; set; } = string.Empty;
    public string AreaId { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Text { get; set; } = string.Empty;
    public string SourceId { get; set; } = string.Empty;
    public string LessonId { get; set; } = string.Empty;
    public List<string> WorkedExampleIds { get; set; } = [];
    public List<string> ExerciseIds { get; set; } = [];
}

public sealed class MathCourseLesson
{
    public string Id { get; set; } = string.Empty;
    public string AreaId { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public bool AlwaysVisible { get; set; }
    public List<string> RequirementIds { get; set; } = [];
    public List<ContentBlock> Blocks { get; set; } = [];
    public List<WorkedExample> WorkedExamples { get; set; } = [];
    public List<string> ExerciseIds { get; set; } = [];
}

public sealed class WorkedExample
{
    public string Id { get; set; } = string.Empty;
    public string RequirementId { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public string Solution { get; set; } = string.Empty;
}

public sealed class OfficialCourseExampleCatalog
{
    public int SchemaVersion { get; set; }
    public List<OfficialCourseExampleSource> Sources { get; set; } = [];
    public List<OfficialCourseExample> Examples { get; set; } = [];
}

public sealed class OfficialCourseExampleSource
{
    public string Id { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public string DocumentSha256 { get; set; } = string.Empty;
    public string VerifiedOn { get; set; } = string.Empty;
    public int FirstExamplePage { get; set; }
    public int LastExamplePage { get; set; }
    public int ExampleCount { get; set; }
}

public sealed class OfficialCourseExample
{
    public string Id { get; set; } = string.Empty;
    public string SourceId { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string OfficialNumber { get; set; } = string.Empty;
    public int Order { get; set; }
    public int MaximumPoints { get; set; }
    public List<int> SourcePages { get; set; } = [];
    public List<string> RequirementIds { get; set; } = [];
    public List<OfficialCourseVisualReference> VisualReferences { get; set; } = [];
    public string Transcription { get; set; } = string.Empty;
}

public sealed class OfficialCourseVisualReference
{
    public int SourcePage { get; set; }
    public string AlternativeText { get; set; } = string.Empty;
}

public sealed class ContentBlock
{
    public string Type { get; set; } = string.Empty;
    public string? Text { get; set; }
    public string? DiagramId { get; set; }
}

public sealed class ExamCatalog
{
    public int SchemaVersion { get; set; }
    public ExamDefinition Exam { get; set; } = new();
}

public sealed class ExamIndexCatalog
{
    public int SchemaVersion { get; set; }
    public List<ContentBlock> TopicIntroduction { get; set; } = [];
    public List<ExerciseTopicDefinition> Topics { get; set; } = [];
    public List<ExamIndexEntry> Exams { get; set; } = [];
}

public sealed class ExamIndexEntry
{
    public string Id { get; set; } = string.Empty;
    public string ContentPath { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsActive { get; set; }
}

public sealed class ExamDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ProgressLabel { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Session { get; set; } = string.Empty;
    public string Formula { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int MaximumPoints { get; set; }
    public int OfficialTaskCount { get; set; }
    public int ProgressItemCount { get; set; }
    public List<ContentBlock> Introduction { get; set; } = [];
    public List<ContentBlock> TopicIntroduction { get; set; } = [];
    public SourceDocument Source { get; set; } = new();
    public List<ExerciseTopicDefinition> Topics { get; set; } = [];
    public List<LearningExercise> Exercises { get; set; } = [];
}

public sealed class SourceDocument
{
    public string Publisher { get; set; } = string.Empty;
    public string DocumentCode { get; set; } = string.Empty;
    public string ExamDate { get; set; } = string.Empty;
    public string QuestionPaperUrl { get; set; } = string.Empty;
    public string QuestionPaperSha256 { get; set; } = string.Empty;
    public string AnswerKeyUrl { get; set; } = string.Empty;
    public string AnswerKeySha256 { get; set; } = string.Empty;
    public string VerifiedOn { get; set; } = string.Empty;
}

public sealed class ExerciseTopicDefinition
{
    public string Id { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<int> ExerciseNumbers { get; set; } = [];
}

public sealed class LearningExercise
{
    public string Id { get; set; } = string.Empty;
    public string ExamId { get; set; } = string.Empty;
    public int Number { get; set; }
    public string DisplayNumber { get; set; } = string.Empty;
    public int Order { get; set; }
    public string? GroupId { get; set; }
    public int Points { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string TopicId { get; set; } = string.Empty;
    public int SourcePage { get; set; }
    public int SolutionSourcePage { get; set; }
    public string VerificationSource { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public List<string> Options { get; set; } = [];
    public int? CorrectOption { get; set; }
    public List<string> Hints { get; set; } = [];
    public string? RevealedAnswer { get; set; }
    public string? Solution { get; set; }
    public string? ScoringCriteria { get; set; }
    public List<LearningAnswerPart> AnswerParts { get; set; } = [];
    public List<string> DiagramIds { get; set; } = [];
    public string? RequirementId { get; set; }
    public string? Level { get; set; }
    public double? ExpectedValue { get; set; }
    public double AbsoluteTolerance { get; set; } = 1e-9;
    public double RelativeTolerance { get; set; } = 1e-9;
    public bool IsMultipleChoice => string.Equals(Mode, "multipleChoice", StringComparison.OrdinalIgnoreCase);
    public bool IsNumeric => string.Equals(Mode, "numeric", StringComparison.OrdinalIgnoreCase);
    public bool IsRevealOnly => string.Equals(Mode, "revealOnly", StringComparison.OrdinalIgnoreCase);
    public bool IsCompound => string.Equals(Mode, "compound", StringComparison.OrdinalIgnoreCase);
    public bool IsCourseExercise => Id.StartsWith("course-", StringComparison.Ordinal);
    public string EffectiveDisplayNumber => string.IsNullOrWhiteSpace(DisplayNumber)
        ? Number.ToString(CultureInfo.InvariantCulture)
        : DisplayNumber;
    public int EffectiveOrder => Order > 0 ? Order : Number;
    public string? EffectiveSolution => string.IsNullOrWhiteSpace(Solution) ? RevealedAnswer : Solution;
}

public sealed class LearningAnswerPart
{
    public string Id { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public List<string> Options { get; set; } = [];
    public int? CorrectOption { get; set; }
    public double? ExpectedValue { get; set; }
    public double AbsoluteTolerance { get; set; } = 1e-9;
    public double RelativeTolerance { get; set; } = 1e-9;
    public List<string> AcceptedAnswers { get; set; } = [];
    public bool IsMultipleChoice => string.Equals(Mode, "multipleChoice", StringComparison.OrdinalIgnoreCase);
    public bool IsNumeric => string.Equals(Mode, "numeric", StringComparison.OrdinalIgnoreCase);
    public bool IsText => string.Equals(Mode, "text", StringComparison.OrdinalIgnoreCase);
}

public sealed class CourseExerciseCatalog
{
    public int SchemaVersion { get; set; }
    public string Author { get; set; } = string.Empty;
    public List<LearningExercise> Exercises { get; set; } = [];
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

public enum CalculatorPipMode
{
    OwnedWindow = 0,
    AlwaysOnTopWindow = 1,
    InAppPanel = 2
}

public sealed record LocalProfile(
    Guid Id,
    string DisplayName,
    ProfileKind Kind,
    CalculatorPipMode CalculatorPipMode = CalculatorPipMode.OwnedWindow);

public sealed record RegistrationResult(bool Success, string Message, LocalProfile? Profile = null, string? RecoveryCode = null);

public sealed record AuthenticationResult(bool Success, string Message, LocalProfile? Profile = null);

public sealed record PasswordUpdateResult(bool Success, string Message, string? RecoveryCode = null);
