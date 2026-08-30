using Abituria.Models;
using Abituria.Services;
using Abituria.Ui;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.LogicalTree;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace Abituria.Tests;

public sealed class DiagramCatalogTests
{
    private static readonly Size[] SupportedSizes = [new(720, 520), new(960, 640), new(1280, 820)];
    private static readonly AppThemeMode[] SupportedThemes =
        [AppThemeMode.Light, AppThemeMode.Dark, AppThemeMode.HighContrast];

    [AvaloniaFact]
    public void Catalog_contains_exactly_the_required_unique_and_used_primitive_inventory()
    {
        var catalog = new ContentRepository().Diagrams;

        DiagramCatalogValidator.Validate(catalog);
        Assert.Equal(1, catalog.SchemaVersion);
        Assert.Equal(100, catalog.Diagrams.Count);
        Assert.Equal(100, catalog.Diagrams.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(36, catalog.Diagrams.Count(item => item.SourceId == "cke-formula-2023"));
        Assert.Equal(9, catalog.Diagrams.Count(item => item.SourceId == "cke-2021-correction"));
        Assert.Equal(4, catalog.Diagrams.Count(item => item.SourceId == "cke-2023-main-extended"));
        Assert.Equal(8, catalog.Diagrams.Count(item => item.SourceId == "cke-2023-main-basic"));
        Assert.Equal(11, catalog.Diagrams.Count(item => item.SourceId == "cke-2024-main-basic"));
        Assert.Equal(1, catalog.Diagrams.Count(item => item.SourceId == "cke-2024-main-extended"));
        Assert.Equal(7, catalog.Diagrams.Count(item => item.SourceId == "cke-2026-main-basic"));
        Assert.Equal(3, catalog.Diagrams.Count(item => item.SourceId == "cke-2026-main-extended"));
        Assert.Equal(9, catalog.Diagrams.Count(item => item.SourceId == "cke-2025-main-basic"));
        Assert.Equal(4, catalog.Diagrams.Count(item => item.SourceId == "adam-course"));
        Assert.Equal(8, catalog.Diagrams.Count(item => item.SourceId == "legacy-vectors"));
        Assert.All(
            catalog.Diagrams.Where(item => item.SourceId == "cke-2026-main-basic"),
            item => Assert.InRange(item.SourcePage, 1, 35));
        Assert.All(
            catalog.Diagrams.Where(item => item.SourceId == "cke-2026-main-extended"),
            item => Assert.InRange(item.SourcePage, 1, 33));
        Assert.All(
            catalog.Diagrams.Where(item => item.SourceId == "cke-2025-main-basic"),
            item => Assert.InRange(item.SourcePage, 1, 28));
        Assert.All(
            catalog.Diagrams.Where(item => item.SourceId == "cke-2024-main-basic"),
            item => Assert.InRange(item.SourcePage, 1, 28));
        Assert.All(
            catalog.Diagrams.Where(item => item.SourceId == "cke-2024-main-extended"),
            item => Assert.InRange(item.SourcePage, 1, 24));
        Assert.All(
            catalog.Diagrams.Where(item => item.SourceId == "cke-2023-main-extended"),
            item => Assert.InRange(item.SourcePage, 1, 22));
        Assert.All(
            catalog.Diagrams.Where(item => item.SourceId == "cke-2023-main-basic"),
            item => Assert.InRange(item.SourcePage, 1, 27));
        Assert.Equal(
            ["arc", "ellipse", "line", "polygon", "polyline", "text"],
            catalog.Diagrams.SelectMany(item => item.Primitives)
                .Select(item => item.Type)
                .Distinct(StringComparer.Ordinal)
                .Order(StringComparer.Ordinal));
        Assert.Contains(catalog.Diagrams.SelectMany(item => item.Primitives), item => item.ArrowStart);
        Assert.Contains(catalog.Diagrams.SelectMany(item => item.Primitives), item => item.ArrowEnd);
        Assert.Contains(catalog.Diagrams.SelectMany(item => item.Primitives), item => item.Dashed);
    }

    [Fact]
    public void Catalog_validator_rejects_invalid_schema_identity_descriptions_coordinates_styles_and_primitives()
    {
        Assert.Throws<ArgumentNullException>(() => DiagramCatalogValidator.Validate(null!));
        AssertInvalidCatalog(catalog => catalog.SchemaVersion = 2);
        AssertInvalidCatalog(catalog => catalog.Diagrams.Add(catalog.Diagrams[0]));
        AssertInvalidDiagram(diagram => diagram.Id = "");
        AssertInvalidDiagram(diagram => diagram.SourceId = " ");
        AssertInvalidDiagram(diagram => diagram.AlternativeText = "");
        AssertInvalidDiagram(diagram => diagram.SourcePage = -1);
        AssertInvalidDiagram(diagram => diagram.Width = 0);
        AssertInvalidDiagram(diagram => diagram.Height = double.PositiveInfinity);
        AssertInvalidDiagram(diagram => diagram.Primitives.Clear());
        AssertInvalidPrimitive(primitive => primitive.Type = "spline");
        AssertInvalidPrimitive(primitive => primitive.Stroke = "unknown");
        AssertInvalidPrimitive(primitive => primitive.Fill = "unknown");
        AssertInvalidPrimitive(primitive => primitive.StrokeThickness = 0);
        AssertInvalidPrimitive(primitive => primitive.FontSize = double.NaN);
        AssertInvalidPrimitive(primitive => primitive.X = double.PositiveInfinity);
        AssertInvalidPrimitive(primitive => primitive.Points = [double.NaN]);
        AssertInvalidPrimitive(primitive =>
        {
            primitive.Type = "polyline";
            primitive.Points = [0, 0, 1];
        });
        AssertInvalidPrimitive(primitive =>
        {
            primitive.Type = "polygon";
            primitive.Points = [0, 0];
        });
        AssertInvalidPrimitive(primitive =>
        {
            primitive.Type = "ellipse";
            primitive.RadiusX = 0;
            primitive.RadiusY = 2;
        });
        AssertInvalidPrimitive(primitive =>
        {
            primitive.Type = "arc";
            primitive.RadiusX = 2;
            primitive.RadiusY = 2;
            primitive.SweepAngle = 0;
        });
        AssertInvalidPrimitive(primitive =>
        {
            primitive.Type = "text";
            primitive.Text = " ";
        });
    }

    [Fact]
    public void Required_lookup_rejects_blank_missing_and_duplicate_identifiers()
    {
        var catalog = ValidCatalog();

        Assert.Same(catalog.Diagrams[0], catalog.GetRequired("test-diagram"));
        Assert.Throws<ArgumentException>(() => catalog.GetRequired(" "));
        Assert.Throws<KeyNotFoundException>(() => catalog.GetRequired("missing"));
        catalog.Diagrams.Add(ValidDiagram());
        Assert.Throws<KeyNotFoundException>(() => catalog.GetRequired("test-diagram"));
    }

    [AvaloniaFact]
    public void Renderer_materializes_every_primitive_arrow_dash_and_accessibility_description()
    {
        var definition = new DiagramDefinition
        {
            Id = "all-primitives",
            SourceId = "test",
            AlternativeText = "Diagram testowy wszystkich prymitywów.",
            Width = 100,
            Height = 100,
            Primitives =
            [
                new() { Type = "line", X = 10, Y = 10, X2 = 90, Y2 = 10, Dashed = true, ArrowStart = true, ArrowEnd = true },
                new() { Type = "polyline", Points = [10, 20, 50, 30, 90, 20], Stroke = "accent" },
                new() { Type = "polygon", Points = [10, 40, 35, 40, 20, 60], Stroke = "danger", Fill = "surface" },
                new() { Type = "ellipse", X = 45, Y = 70, RadiusX = 10, RadiusY = 8, Stroke = "success" },
                new() { Type = "arc", X = 70, Y = 70, RadiusX = 15, RadiusY = 10, StartAngle = 0, SweepAngle = 180, Stroke = "muted" },
                new() { Type = "text", X = 75, Y = 45, Text = "A", Stroke = "primary", FontSize = 12 }
            ]
        };
        DiagramCatalogValidator.Validate(new DiagramCatalog { SchemaVersion = 1, Diagrams = [definition] });

        var view = new DiagramView(definition);
        var canvas = Assert.Single(view.GetLogicalDescendants().OfType<Canvas>());

        Assert.Equal(definition.AlternativeText, AutomationProperties.GetName(view));
        Assert.Equal(5, canvas.Children.OfType<Line>().Count());
        Assert.Equal(2, canvas.Children.OfType<Polyline>().Count());
        Assert.Single(canvas.Children.OfType<Polygon>());
        Assert.Single(canvas.Children.OfType<Ellipse>());
        Assert.Single(canvas.Children.OfType<TextBlock>());
        Assert.All(canvas.Children.OfType<Line>(), line => Assert.NotNull(line.StrokeDashArray));
        Assert.Throws<InvalidOperationException>(() => new DiagramView(new DiagramDefinition
        {
            AlternativeText = "Nieznany prymityw",
            Width = 10,
            Height = 10,
            Primitives = [new DiagramPrimitive { Type = "unknown" }]
        }));
        Assert.Throws<InvalidOperationException>(() => new DiagramView(new DiagramDefinition
        {
            AlternativeText = "Nieznany kolor",
            Width = 10,
            Height = 10,
            Primitives = [new DiagramPrimitive { Type = "line", X2 = 1, Y2 = 1, Stroke = "unknown" }]
        }));
    }

    [AvaloniaFact]
    public void Main_exam_figures_use_only_Avalonia_vector_controls()
    {
        var definitions = new ContentRepository().Diagrams.Diagrams
            .Where(item => item.SourceId is "cke-2023-main-basic" or "cke-2023-main-extended" or "cke-2024-main-basic" or "cke-2024-main-extended" or "cke-2026-main-basic" or "cke-2026-main-extended")
            .ToArray();

        Assert.Equal(34, definitions.Length);
        foreach (var definition in definitions)
        {
            var view = new DiagramView(definition);
            var canvas = Assert.Single(view.GetLogicalDescendants().OfType<Canvas>());
            Assert.NotEmpty(canvas.Children);
            Assert.All(canvas.Children, child =>
                Assert.True(child is Shape or TextBlock, $"{definition.Id} zawiera kontrolkę rastrową {child.GetType().Name}."));
            Assert.Empty(view.GetLogicalDescendants().OfType<Image>());
        }
    }

    [AvaloniaFact]
    public void Every_diagram_renders_in_all_themes_and_supported_window_sizes()
    {
        var application = Assert.IsType<TestApplication>(Application.Current);
        using var themeManager = new AppThemeManager(application);
        var catalog = new ContentRepository().Diagrams;
        var window = new Window();

        try
        {
            window.Show();
            foreach (var theme in SupportedThemes)
            {
                themeManager.SetMode(theme);
                foreach (var size in SupportedSizes)
                {
                    window.Width = size.Width;
                    window.Height = size.Height;
                    foreach (var definition in catalog.Diagrams)
                    {
                        var view = new DiagramView(definition);
                        window.Content = view;
                        Dispatcher.UIThread.RunJobs();
                        Assert.Equal(definition.AlternativeText, AutomationProperties.GetName(view));
                        Assert.InRange(view.Bounds.Width, 1, size.Width);
                        Assert.InRange(view.Bounds.Height, 1, size.Height);
                        using var frame = Assert.IsType<WriteableBitmap>(window.CaptureRenderedFrame());
                    }
                }
            }
        }
        finally
        {
            window.Close();
            themeManager.SetMode(AppThemeMode.System);
        }
    }

    [Fact]
    public void Rich_content_rejects_missing_and_unknown_diagram_references()
    {
        var block = new ContentBlock { Type = "diagram", DiagramId = "missing" };

        Assert.Throws<InvalidOperationException>(() => new RichContentView([block]));
        Assert.Throws<KeyNotFoundException>(() => new RichContentView([block], ValidCatalog()));
    }

    private static void AssertInvalidCatalog(Action<DiagramCatalog> mutation)
    {
        var catalog = ValidCatalog();
        mutation(catalog);
        Assert.Throws<InvalidOperationException>(() => DiagramCatalogValidator.Validate(catalog));
    }

    private static void AssertInvalidDiagram(Action<DiagramDefinition> mutation)
    {
        var catalog = ValidCatalog();
        mutation(catalog.Diagrams[0]);
        Assert.Throws<InvalidOperationException>(() => DiagramCatalogValidator.Validate(catalog));
    }

    private static void AssertInvalidPrimitive(Action<DiagramPrimitive> mutation)
    {
        var catalog = ValidCatalog();
        mutation(catalog.Diagrams[0].Primitives[0]);
        Assert.Throws<InvalidOperationException>(() => DiagramCatalogValidator.Validate(catalog));
    }

    private static DiagramCatalog ValidCatalog() => new()
    {
        SchemaVersion = 1,
        Diagrams = [ValidDiagram()]
    };

    private static DiagramDefinition ValidDiagram() => new()
    {
        Id = "test-diagram",
        SourceId = "test",
        AlternativeText = "Poprawny diagram testowy.",
        Width = 100,
        Height = 100,
        Primitives = [new DiagramPrimitive { Type = "line", X = 1, Y = 1, X2 = 99, Y2 = 99 }]
    };
}
