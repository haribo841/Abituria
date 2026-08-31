using System.Text.Json;
using Abituria.Models;
using Abituria.Ui;
using CSharpMath.Avalonia;

namespace Abituria.Tests;

public sealed class ContentInventoryTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();
    private static readonly string[] ExpectedPlaceholderIds =
        ["exercise-set-e", "graph-generator", "matura-2019", "matura-2020", "matura-2021", "trigonometric-calculator"];
    private static readonly string[] RetainedLessonIds =
        ["equations-and-inequalities", "greek-alphabet", "logarithms", "natural-numbers", "number-sequences", "prime-numbers", "quadratic-function", "sets-and-logic", "vectors"];
    private static readonly string[] ExpectedContentBlockTypes = ["richText", "diagram"];
    private static readonly string[] ExpectedTask7Options =
        ["\\(g(x)=-2x+2\\)", "\\(g(x)=-2x\\)", "\\(g(x)=-2x+6\\)", "\\(g(x)=-2x+8\\)"];

    [Fact]
    public void Migrated_content_has_expected_inventory()
    {
        var formulas = Read<FormulaCatalog>("Content/formulas.json");
        var course = Read<MathCourseCatalog>("Content/chapters.json");
        var courseExercises = Read<CourseExerciseCatalog>("Content/course-exercises.json");
        var examIndex = Read<ExamIndexCatalog>("Content/exams.json");
        var examCatalog = Read<ExamCatalog>("Content/exam-2021-correction.json");
        var exam = examCatalog.Exam;
        var currentExamCatalog = Read<ExamCatalog>("Content/exam-2026-main-basic.json");
        var currentExam = currentExamCatalog.Exam;
        var extendedExamCatalog = Read<ExamCatalog>("Content/exam-2026-main-extended.json");
        var extendedExam = extendedExamCatalog.Exam;
        var basic2025Catalog = Read<ExamCatalog>("Content/exam-2025-main-basic.json");
        var basic2025 = basic2025Catalog.Exam;
        var extended2025Catalog = Read<ExamCatalog>("Content/exam-2025-main-extended.json");
        var extended2025 = extended2025Catalog.Exam;
        var correction2025Catalog = Read<ExamCatalog>("Content/exam-2025-correction-basic.json");
        var correction2025 = correction2025Catalog.Exam;
        var basic2024Catalog = Read<ExamCatalog>("Content/exam-2024-main-basic.json");
        var basic2024 = basic2024Catalog.Exam;
        var extended2024Catalog = Read<ExamCatalog>("Content/exam-2024-main-extended.json");
        var extended2024 = extended2024Catalog.Exam;
        var correction2024Catalog = Read<ExamCatalog>("Content/exam-2024-correction-basic.json");
        var correction2024 = correction2024Catalog.Exam;
        var basic2023Catalog = Read<ExamCatalog>("Content/exam-2023-main-basic.json");
        var basic2023 = basic2023Catalog.Exam;
        var correction2023Catalog = Read<ExamCatalog>("Content/exam-2023-correction-basic.json");
        var correction2023 = correction2023Catalog.Exam;
        var extended2023Catalog = Read<ExamCatalog>("Content/exam-2023-main-extended.json");
        var extended2023 = extended2023Catalog.Exam;
        var basic2022Catalog = Read<ExamCatalog>("Content/exam-2022-main-basic.json");
        var basic2022 = basic2022Catalog.Exam;
        var extended2022Catalog = Read<ExamCatalog>("Content/exam-2022-main-extended.json");
        var extended2022 = extended2022Catalog.Exam;
        var correction2022Catalog = Read<ExamCatalog>("Content/exam-2022-correction-basic.json");
        var correction2022 = correction2022Catalog.Exam;
        var basic2021Catalog = Read<ExamCatalog>("Content/exam-2021-main-basic.json");
        var basic2021 = basic2021Catalog.Exam;
        var extended2021Catalog = Read<ExamCatalog>("Content/exam-2021-main-extended.json");
        var extended2021 = extended2021Catalog.Exam;
        var placeholders = Read<PlaceholderCatalog>("Content/placeholders.json");
        var roadmap = Read<RoadmapCatalog>("Content/roadmap.json");

        Assert.Equal(18, formulas.Articles.Count);
        Assert.NotEmpty(formulas.Introduction);
        Assert.Equal(Enumerable.Range(1, 18), formulas.Articles.Select(item => item.Order));
        Assert.All(formulas.Articles, item => Assert.NotEmpty(item.Blocks));
        Assert.Equal(4, formulas.SchemaVersion);
        Assert.Equal(4, course.SchemaVersion);
        Assert.Equal(3, examCatalog.SchemaVersion);
        Assert.Equal(4, currentExamCatalog.SchemaVersion);
        Assert.Equal(4, extendedExamCatalog.SchemaVersion);
        Assert.Equal(4, basic2025Catalog.SchemaVersion);
        Assert.Equal(4, extended2025Catalog.SchemaVersion);
        Assert.Equal(4, correction2025Catalog.SchemaVersion);
        Assert.Equal(4, basic2024Catalog.SchemaVersion);
        Assert.Equal(4, extended2024Catalog.SchemaVersion);
        Assert.Equal(4, correction2024Catalog.SchemaVersion);
        Assert.Equal(4, basic2023Catalog.SchemaVersion);
        Assert.Equal(4, correction2023Catalog.SchemaVersion);
        Assert.Equal(4, extended2023Catalog.SchemaVersion);
        Assert.Equal(4, basic2022Catalog.SchemaVersion);
        Assert.Equal(4, extended2022Catalog.SchemaVersion);
        Assert.Equal(4, correction2022Catalog.SchemaVersion);
        Assert.Equal(4, basic2021Catalog.SchemaVersion);
        Assert.Equal(4, extended2021Catalog.SchemaVersion);
        Assert.Equal(1, examIndex.SchemaVersion);
        Assert.Equal(17, examIndex.Topics.Count);
        Assert.Equal(
            [
                "matura-maj-2026-podstawowa",
                "matura-maj-2026-rozszerzona",
                "matura-maj-2025-podstawowa",
                "matura-maj-2025-rozszerzona",
                "matura-poprawkowa-2025-podstawowa",
                "matura-maj-2024-podstawowa",
                "matura-maj-2024-rozszerzona",
                "matura-poprawkowa-2024-podstawowa",
                "matura-maj-2023-podstawowa",
                "matura-poprawkowa-2023-podstawowa",
                "matura-maj-2023-rozszerzona",
                "matura-maj-2022-podstawowa",
                "matura-maj-2022-rozszerzona",
                "matura-poprawkowa-2022-podstawowa",
                "matura-maj-2021-podstawowa",
                "matura-maj-2021-rozszerzona",
                "matura-poprawkowa-2021"
            ],
            examIndex.Exams.OrderBy(item => item.Order).Select(item => item.Id));
        Assert.Equal(4, course.Groups.Count);
        Assert.Equal(13, course.Areas.Count);
        Assert.Equal(34, course.Lessons.Count);
        Assert.NotEmpty(course.Introduction);
        Assert.All(RetainedLessonIds, id => Assert.Contains(course.Lessons, lesson => lesson.Id == id));
        Assert.All(course.Lessons, item => Assert.NotEmpty(item.Blocks));
        var vectorChapter = course.Lessons.Single(item => item.Id == "vectors");
        Assert.Equal(8, vectorChapter.Blocks.Count(block => block.Type == "diagram"));
        Assert.Equal(course.Lessons.Count, course.Lessons.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.All(
            course.Lessons.SelectMany(item => item.Blocks),
            block => Assert.Contains(block.Type, ExpectedContentBlockTypes));
        Assert.Equal(357, courseExercises.Exercises.Count);
        Assert.Equal(6, placeholders.Items.Count);
        Assert.Equal(
            ExpectedPlaceholderIds,
            placeholders.Items.Select(item => item.Id).Order());
        Assert.Equal(17, exam.Topics.Count);
        Assert.NotEmpty(exam.Introduction);
        Assert.NotEmpty(exam.TopicIntroduction);
        Assert.Equal(35, exam.Exercises.Count);
        Assert.Equal(28, exam.Exercises.Count(item => item.IsMultipleChoice));
        Assert.Equal(7, exam.Exercises.Count(item => !item.IsMultipleChoice));
        Assert.Equal(Enumerable.Range(1, 35), exam.Exercises.Select(item => item.Number));
        Assert.Equal(37, currentExam.Exercises.Count);
        Assert.Equal(33, currentExam.OfficialTaskCount);
        Assert.Equal(50, currentExam.MaximumPoints);
        Assert.Equal(13, extendedExam.Exercises.Count);
        Assert.Equal(12, extendedExam.OfficialTaskCount);
        Assert.Equal(50, extendedExam.MaximumPoints);
        Assert.Equal(35, basic2025.Exercises.Count);
        Assert.Equal(31, basic2025.OfficialTaskCount);
        Assert.Equal(50, basic2025.MaximumPoints);
        Assert.Equal(13, extended2025.Exercises.Count);
        Assert.Equal(12, extended2025.OfficialTaskCount);
        Assert.Equal(50, extended2025.MaximumPoints);
        Assert.Equal(36, correction2025.Exercises.Count);
        Assert.Equal(31, correction2025.OfficialTaskCount);
        Assert.Equal(50, correction2025.MaximumPoints);
        Assert.Equal(35, basic2024.Exercises.Count);
        Assert.Equal(31, basic2024.OfficialTaskCount);
        Assert.Equal(46, basic2024.MaximumPoints);
        Assert.Equal(14, extended2024.Exercises.Count);
        Assert.Equal(13, extended2024.OfficialTaskCount);
        Assert.Equal(50, extended2024.MaximumPoints);
        Assert.Equal(36, correction2024.Exercises.Count);
        Assert.Equal(30, correction2024.OfficialTaskCount);
        Assert.Equal(46, correction2024.MaximumPoints);
        Assert.Equal(34, basic2023.Exercises.Count);
        Assert.Equal(31, basic2023.OfficialTaskCount);
        Assert.Equal(46, basic2023.MaximumPoints);
        Assert.Equal(36, correction2023.Exercises.Count);
        Assert.Equal(33, correction2023.OfficialTaskCount);
        Assert.Equal(46, correction2023.MaximumPoints);
        Assert.Equal(14, extended2023.Exercises.Count);
        Assert.Equal(13, extended2023.OfficialTaskCount);
        Assert.Equal(50, extended2023.MaximumPoints);
        Assert.Equal(35, basic2022.Exercises.Count);
        Assert.Equal(35, basic2022.OfficialTaskCount);
        Assert.Equal(45, basic2022.MaximumPoints);
        Assert.Equal(15, extended2022.Exercises.Count);
        Assert.Equal(15, extended2022.OfficialTaskCount);
        Assert.Equal(50, extended2022.MaximumPoints);
        Assert.Equal(35, correction2022.Exercises.Count);
        Assert.Equal(35, correction2022.OfficialTaskCount);
        Assert.Equal(45, correction2022.MaximumPoints);
        Assert.Equal(35, basic2021.Exercises.Count);
        Assert.Equal(35, basic2021.OfficialTaskCount);
        Assert.Equal(45, basic2021.MaximumPoints);
        Assert.Equal(15, extended2021.Exercises.Count);
        Assert.Equal(15, extended2021.OfficialTaskCount);
        Assert.Equal(50, extended2021.MaximumPoints);
        Assert.Contains(roadmap.Items, item => item.Status == RoadmapStatus.Migrated);
        Assert.Contains(roadmap.Items, item => item.Status == RoadmapStatus.Planned);
        Assert.Contains(roadmap.Items, item => item.Status == RoadmapStatus.Superseded);
        Assert.Contains(roadmap.Items, item => item.Id == "general-calculator" && item.Status == RoadmapStatus.Migrated);
        Assert.Contains(roadmap.Items, item => item.Id == "graph-generator" && item.Status == RoadmapStatus.Planned);
        Assert.Contains(roadmap.Items, item => item.Id == "trigonometric-calculator" && item.Status == RoadmapStatus.Planned);
        Assert.Contains(roadmap.Items, item => item.Id == "formula-editor-prototype" && item.Status == RoadmapStatus.Superseded);
        Assert.Contains(roadmap.Items, item => item.Id == "natural-numbers" && item.Status == RoadmapStatus.Migrated);
        Assert.Contains(roadmap.Items, item => item.Id == "greek-alphabet" && item.Status == RoadmapStatus.Migrated);
        Assert.Contains(roadmap.Items, item => item.Id == "chapters-expansion" && item.Status == RoadmapStatus.Migrated);
        Assert.Contains(placeholders.Items.Single(item => item.Id == "matura-2021").Blocks,
            block => block.Text is not null && block.Text.Contains("79%", StringComparison.Ordinal) && block.Text.Contains("56%", StringComparison.Ordinal));

        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        Assert.Equal(1, diagrams.SchemaVersion);
        Assert.Equal(139, diagrams.Diagrams.Count);
        Assert.Equal(139, diagrams.Diagrams.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Topic_mapping_covers_every_exercise_exactly_once()
    {
        var exam = Read<ExamCatalog>("Content/exam-2021-correction.json").Exam;
        var mappedNumbers = exam.Topics.SelectMany(topic => topic.ExerciseNumbers).ToArray();

        Assert.Equal(35, mappedNumbers.Length);
        Assert.Equal(Enumerable.Range(1, 35), mappedNumbers.Order());
        Assert.Equal(35, mappedNumbers.Distinct().Count());
        Assert.All(exam.Exercises, exercise => Assert.Contains(exam.Topics, topic => topic.Id == exercise.TopicId && topic.ExerciseNumbers.Contains(exercise.Number)));
    }

    [Fact]
    public void Cke_contract_and_known_legacy_defects_are_corrected()
    {
        var exam = Read<ExamCatalog>("Content/exam-2021-correction.json").Exam;
        var expectedKey = new[] { 4, 3, 1, 1, 4, 3, 2, 3, 1, 3, 4, 4, 3, 2, 3, 2, 4, 1, 4, 2, 4, 1, 3, 3, 1, 2, 3, 2 };

        Assert.Equal("EMAP-P0-100-2108", exam.Source.DocumentCode);
        Assert.Equal("2026-06-30", exam.Source.VerifiedOn);
        Assert.Equal(expectedKey, exam.Exercises.Take(28).Select(item => item.CorrectOption!.Value));
        Assert.All(exam.Exercises, item =>
        {
            Assert.Equal("CKE EMAP-P0-100-2108", item.VerificationSource);
            Assert.InRange(item.SourcePage, 2, 22);
        });

        var task7 = exam.Exercises.Single(item => item.Number == 7);
        Assert.Equal(ExpectedTask7Options, task7.Options);
        var task17 = exam.Exercises.Single(item => item.Number == 17);
        Assert.Contains("BSC", task17.Prompt);
        Assert.Equal("\\(40^{\\circ}\\)", task17.Options[3]);
        Assert.Contains("exam-mp21-z17", task17.DiagramIds);
        Assert.DoesNotContain("x=-2", string.Join(' ', task17.Hints), StringComparison.Ordinal);
        var task18 = exam.Exercises.Single(item => item.Number == 18);
        Assert.Contains("\\angle BOC", task18.Prompt, StringComparison.Ordinal);
        var task19 = exam.Exercises.Single(item => item.Number == 19);
        Assert.Contains("\\angle ACB", task19.Prompt, StringComparison.Ordinal);
        var task22 = exam.Exercises.Single(item => item.Number == 22);
        Assert.Contains(task22.Hints, hint => hint.Contains("prostej prostopadłej", StringComparison.Ordinal));
        var task28 = exam.Exercises.Single(item => item.Number == 28);
        Assert.Contains(task28.Hints, hint => hint.Contains("35x+40", StringComparison.Ordinal));
        var task29 = exam.Exercises.Single(item => item.Number == 29);
        Assert.Equal("\\(\\text{Odpowiedź: } x\\in(-\\infty,-1]\\cup[5,+\\infty)\\)", task29.RevealedAnswer);
        Assert.Contains("x=-\\frac{1}{2}", exam.Exercises.Single(item => item.Number == 30).RevealedAnswer, StringComparison.Ordinal);
        Assert.Contains("(a-2b)^2+b^2", exam.Exercises.Single(item => item.Number == 31).RevealedAnswer, StringComparison.Ordinal);
        Assert.Equal("\\(|BD|=12\\)", exam.Exercises.Single(item => item.Number == 32).RevealedAnswer);
        Assert.Contains("\\frac{16}{3}", exam.Exercises.Single(item => item.Number == 33).RevealedAnswer, StringComparison.Ordinal);
        Assert.Contains("\\frac{1}{9}", exam.Exercises.Single(item => item.Number == 34).RevealedAnswer, StringComparison.Ordinal);
        Assert.Contains("x=0", exam.Exercises.Single(item => item.Number == 35).RevealedAnswer, StringComparison.Ordinal);
        Assert.Contains("q =-2", exam.Exercises.Single(item => item.Number == 35).RevealedAnswer, StringComparison.Ordinal);

        var allHints = string.Join(' ', exam.Exercises.SelectMany(item => item.Hints));
        Assert.DoesNotContain("Rozwiąż równanie. Zacznij", allHints, StringComparison.Ordinal);
        Assert.DoesNotContain("kawadratowej", allHints, StringComparison.Ordinal);
        Assert.DoesNotContain("czegokąty", allHints, StringComparison.Ordinal);
        Assert.DoesNotContain("Stosunek ku białych", allHints, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("Analiza biznesowa.txt", "7216E37D9565480CE220590FDAC7B7BCAA509650D749A68777A78E3BC534E3CC")]
    [InlineData("Definicja projektu.txt", "8D690148DDE6EDE54EA53C6CEBDBD5A3365C38C3AD85FD2C54813756BFB804E4")]
    [InlineData("Implementacja.txt", "500D2EB2695DDC18842441916D97CC3ECD98151DFC612044E493805E38BB798F")]
    [InlineData("Opis struktury systemu.txt", "66E6B14916D1DE78A53B36F0FC24CA791D6DF5C80A0FE3260FFB105237E508F3")]
    [InlineData("Uzupełnić Treść działów matematyki.txt", "8EBD73B3FC283F683C1D0A40C850D70874EE4AACE6B67BE792AB57D7F81F59CF")]
    [InlineData("LICENSE-2022-Ich-Troje.txt", "F36B81A276CF8EC8889310086D7DF99667AA0A06BB348328E99F65A09A7DDCC5")]
    public void Legacy_originals_are_preserved_byte_for_byte(string fileName, string expectedHash)
    {
        var path = Path.Combine(RepositoryRoot, "docs", "legacy", "originals", fileName);
        var actualHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(path)));

        Assert.Equal(expectedHash, actualHash);
    }

    [Fact]
    public void Every_exercise_has_complete_answer_contract()
    {
        var exercises = Read<ExamCatalog>("Content/exam-2021-correction.json").Exam.Exercises;
        foreach (var exercise in exercises)
        {
            Assert.False(string.IsNullOrWhiteSpace(exercise.Prompt));
            Assert.InRange(exercise.Hints.Count, 2, int.MaxValue);
            Assert.All(exercise.Hints, hint => Assert.False(string.IsNullOrWhiteSpace(hint)));
            if (exercise.IsMultipleChoice)
            {
                Assert.Equal(4, exercise.Options.Count);
                Assert.InRange(exercise.CorrectOption ?? 0, 1, 4);
                Assert.All(exercise.Options, option => Assert.False(string.IsNullOrWhiteSpace(option)));
            }
            else
            {
                Assert.Empty(exercise.Options);
                Assert.Null(exercise.CorrectOption);
                Assert.False(string.IsNullOrWhiteSpace(exercise.RevealedAnswer));
            }
        }
    }

    [Fact]
    public void Referenced_diagrams_exist_are_all_used_and_legacy_latex_typos_are_removed()
    {
        var formulas = Read<FormulaCatalog>("Content/formulas.json");
        var course = Read<MathCourseCatalog>("Content/chapters.json");
        var courseExercises = Read<CourseExerciseCatalog>("Content/course-exercises.json");
        var exam = Read<ExamCatalog>("Content/exam-2021-correction.json").Exam;
        var currentExam = Read<ExamCatalog>("Content/exam-2026-main-basic.json").Exam;
        var extendedExam = Read<ExamCatalog>("Content/exam-2026-main-extended.json").Exam;
        var basic2025 = Read<ExamCatalog>("Content/exam-2025-main-basic.json").Exam;
        var extended2025 = Read<ExamCatalog>("Content/exam-2025-main-extended.json").Exam;
        var correction2025 = Read<ExamCatalog>("Content/exam-2025-correction-basic.json").Exam;
        var basic2024 = Read<ExamCatalog>("Content/exam-2024-main-basic.json").Exam;
        var extended2024 = Read<ExamCatalog>("Content/exam-2024-main-extended.json").Exam;
        var correction2024 = Read<ExamCatalog>("Content/exam-2024-correction-basic.json").Exam;
        var basic2023 = Read<ExamCatalog>("Content/exam-2023-main-basic.json").Exam;
        var correction2023 = Read<ExamCatalog>("Content/exam-2023-correction-basic.json").Exam;
        var extended2023 = Read<ExamCatalog>("Content/exam-2023-main-extended.json").Exam;
        var basic2022 = Read<ExamCatalog>("Content/exam-2022-main-basic.json").Exam;
        var extended2022 = Read<ExamCatalog>("Content/exam-2022-main-extended.json").Exam;
        var correction2022 = Read<ExamCatalog>("Content/exam-2022-correction-basic.json").Exam;
        var basic2021 = Read<ExamCatalog>("Content/exam-2021-main-basic.json").Exam;
        var extended2021 = Read<ExamCatalog>("Content/exam-2021-main-extended.json").Exam;
        var diagrams = Read<DiagramCatalog>("Content/diagrams.json");
        var referencedDiagramIds = formulas.Articles.SelectMany(item => item.Blocks)
            .Concat(course.Lessons.SelectMany(item => item.Blocks))
            .Where(block => block.Type == "diagram")
            .Select(block => block.DiagramId!)
            .Concat(exam.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(currentExam.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(extendedExam.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(basic2025.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(extended2025.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(correction2025.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(basic2024.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(extended2024.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(correction2024.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(basic2023.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(correction2023.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(extended2023.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(basic2022.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(extended2022.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(correction2022.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(basic2021.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(extended2021.Exercises.SelectMany(item => item.DiagramIds))
            .Concat(courseExercises.Exercises.SelectMany(item => item.DiagramIds))
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var catalogIds = diagrams.Diagrams.Select(item => item.Id).Order(StringComparer.Ordinal).ToArray();
        Assert.Equal(catalogIds, referencedDiagramIds);
        Assert.All(diagrams.Diagrams, diagram =>
        {
            Assert.False(string.IsNullOrWhiteSpace(diagram.SourceId));
            Assert.False(string.IsNullOrWhiteSpace(diagram.AlternativeText));
            Assert.NotEmpty(diagram.Primitives);
        });

        var allRenderedText = string.Join('\n', ReadRichTexts());
        Assert.DoesNotContain("/cdot", allRenderedText, StringComparison.Ordinal);
        Assert.DoesNotContain("/text", allRenderedText, StringComparison.Ordinal);
        Assert.DoesNotContain("\\tg", allRenderedText, StringComparison.Ordinal);
        Assert.DoesNotContain("1=-\\frac{\\Delta}", allRenderedText, StringComparison.Ordinal);
        Assert.DoesNotContain("x_1+x_1=", allRenderedText, StringComparison.Ordinal);
        Assert.DoesNotContain("x_1\\cdot{x_1}=", allRenderedText, StringComparison.Ordinal);
        Assert.DoesNotContain("B=(x_1,y_1)", allRenderedText, StringComparison.Ordinal);
    }

    [Fact]
    public void Every_rendered_rich_text_line_is_supported_by_text_renderer()
    {
        var errors = ReadRichTextLines()
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => (Line: line, Painter: new TextPainter { LaTeX = line }))
            .Where(result => !string.IsNullOrWhiteSpace(result.Painter.ErrorMessage))
            .Select(result => $"{result.Line}: {result.Painter.ErrorMessage}")
            .ToArray();

        Assert.True(errors.Length == 0, string.Join(Environment.NewLine, errors));
    }

    [Fact]
    public void Every_rendered_rich_text_line_uses_balanced_single_line_math_delimiters()
    {
        foreach (var line in ReadRichTextLines())
        {
            Assert.True(RichContentView.HasBalancedInlineMathDelimiters(line), line);
            Assert.DoesNotContain('$', line);
            Assert.DoesNotContain("\\[", line, StringComparison.Ordinal);
            Assert.DoesNotContain("\\]", line, StringComparison.Ordinal);
        }
    }

    private static IEnumerable<string> ReadRichTexts()
    {
        var formulas = Read<FormulaCatalog>("Content/formulas.json");
        var course = Read<MathCourseCatalog>("Content/chapters.json");
        var courseExercises = Read<CourseExerciseCatalog>("Content/course-exercises.json");
        var exams = new[]
        {
            Read<ExamCatalog>("Content/exam-2026-main-basic.json").Exam,
            Read<ExamCatalog>("Content/exam-2026-main-extended.json").Exam,
            Read<ExamCatalog>("Content/exam-2025-main-basic.json").Exam,
            Read<ExamCatalog>("Content/exam-2025-main-extended.json").Exam,
            Read<ExamCatalog>("Content/exam-2025-correction-basic.json").Exam,
            Read<ExamCatalog>("Content/exam-2024-main-basic.json").Exam,
            Read<ExamCatalog>("Content/exam-2024-main-extended.json").Exam,
            Read<ExamCatalog>("Content/exam-2024-correction-basic.json").Exam,
            Read<ExamCatalog>("Content/exam-2023-main-basic.json").Exam,
            Read<ExamCatalog>("Content/exam-2023-correction-basic.json").Exam,
            Read<ExamCatalog>("Content/exam-2023-main-extended.json").Exam,
            Read<ExamCatalog>("Content/exam-2022-main-basic.json").Exam,
            Read<ExamCatalog>("Content/exam-2022-main-extended.json").Exam,
            Read<ExamCatalog>("Content/exam-2022-correction-basic.json").Exam,
            Read<ExamCatalog>("Content/exam-2021-main-basic.json").Exam,
            Read<ExamCatalog>("Content/exam-2021-main-extended.json").Exam,
            Read<ExamCatalog>("Content/exam-2021-correction.json").Exam
        };
        var placeholders = Read<PlaceholderCatalog>("Content/placeholders.json");
        var roadmap = Read<RoadmapCatalog>("Content/roadmap.json");
        var examExercises = exams.SelectMany(item => item.Exercises);
        var exercises = examExercises.Concat(courseExercises.Exercises);
        foreach (var text in formulas.Introduction.Concat(formulas.Articles.SelectMany(item => item.Blocks))
                     .Concat(course.Introduction).Concat(course.Lessons.SelectMany(item => item.Blocks))
                     .Concat(exams.SelectMany(item => item.Introduction))
                     .Concat(exams.SelectMany(item => item.TopicIntroduction))
                     .Concat(placeholders.Items.SelectMany(item => item.Blocks))
                     .Concat(roadmap.Introduction)
                     .Select(block => block.Text).Where(text => !string.IsNullOrWhiteSpace(text)))
            yield return text!;
        foreach (var requirement in course.Requirements)
            yield return requirement.Text;
        foreach (var example in course.Lessons.SelectMany(lesson => lesson.WorkedExamples))
        {
            yield return example.Prompt;
            yield return example.Solution;
        }
        foreach (var exercise in exercises)
        {
            yield return exercise.Prompt;
            foreach (var option in exercise.Options) yield return option;
            foreach (var hint in exercise.Hints) yield return hint;
            if (!string.IsNullOrWhiteSpace(exercise.RevealedAnswer)) yield return exercise.RevealedAnswer;
            if (!string.IsNullOrWhiteSpace(exercise.Solution)) yield return exercise.Solution;
            if (!string.IsNullOrWhiteSpace(exercise.ScoringCriteria)) yield return exercise.ScoringCriteria;
            foreach (var part in exercise.AnswerParts)
            {
                yield return part.Prompt;
                foreach (var option in part.Options) yield return option;
            }
        }
    }

    private static IEnumerable<string> ReadRichTextLines() => ReadRichTexts().SelectMany(text =>
        text.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n'));

    private static T Read<T>(string relativePath) => JsonSerializer.Deserialize<T>(
        File.ReadAllText(Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar))), JsonOptions)!;

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Abituria.csproj"))) directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Nie znaleziono katalogu repozytorium.");
    }
}
