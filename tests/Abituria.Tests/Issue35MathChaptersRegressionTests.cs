using System.Diagnostics;
using System.Text.Json;
using Abituria.Models;
using Abituria.Ui;
using Abituria.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using CSharpMath.Avalonia;

namespace Abituria.Tests;

public sealed class Issue35MathChaptersRegressionTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();
    private static readonly string[] AvailableChapterIds =
    [
        "equations-and-inequalities",
        "greek-alphabet",
        "logarithms",
        "natural-numbers",
        "quadratic-function",
        "sets-and-logic",
        "vectors"
    ];
    private static readonly string[] GreekRows =
    [
        "Α | α | alfa",
        "Β | β | beta",
        "Γ | γ | gamma",
        "Δ | δ | delta",
        "Ε | ε | epsilon",
        "Ζ | ζ | dzeta",
        "Η | η | eta",
        "Θ | θ | theta",
        "Ι | ι | jota",
        "Κ | κ | kappa",
        "Λ | λ | lambda",
        "Μ | μ | mi",
        "Ν | ν | ni",
        "Ξ | ξ | ksi",
        "Ο | ο | omikron",
        "Π | π | pi",
        "Ρ | ρ | ro",
        "Σ | σ, ς | sigma",
        "Τ | τ | tau",
        "Υ | υ | ypsilon",
        "Φ | φ | fi",
        "Χ | χ | chi",
        "Ψ | ψ | psi",
        "Ω | ω | omega"
    ];
    private static readonly IReadOnlyDictionary<string, string[]> RequiredSections =
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["natural-numbers"] =
            [
                "Aksjomatyka liczb naturalnych",
                "Indukcja matematyczna krok po kroku",
                "Działania na liczbach naturalnych",
                "Zadanie.",
                "Wskazówka.",
                "Odpowiedź."
            ],
            ["greek-alphabet"] =
            [
                "Tabela alfabetu greckiego",
                "Najczęstsze zastosowania symboli"
            ],
            ["sets-and-logic"] =
            [
                "Zbiory liczbowe",
                "Działania na zbiorach",
                "Przedziały liczbowe",
                "Podstawy logiki"
            ],
            ["equations-and-inequalities"] =
            [
                "Wyrażenia algebraiczne - część teoretyczna",
                "Obliczenia liczbowe, ułamki i przybliżenia",
                "błąd bezwzględny",
                "Wzory skróconego mnożenia",
                "Działania na potęgach i pierwiastkach",
                "Obliczenia procentowe",
                "Równania i nierówności algebraiczne",
                "Wymagania przed zadaniami wprowadzającymi",
                "Zadanie wprowadzające",
                "Zadanie maturalne"
            ],
            ["quadratic-function"] =
            [
                "Funkcja i równanie kwadratowe",
                "\\(\\Delta{}<0\\)",
                "nie ma rozwiązań w zbiorze liczb rzeczywistych",
                "Zadanie maturalne"
            ],
            ["logarithms"] =
            [
                "Pojęcie logarytmu",
                "Prawa działań na logarytmach",
                "\\(b>0\\)",
                "\\(b\\neq1\\)",
                "Obliczenia logarytmiczne",
                "Zadanie maturalne"
            ]
        };

    [Fact]
    public void Issue_35_chapters_are_available_and_cover_every_historical_topic()
    {
        var catalog = Read<ChapterCatalog>("Content/chapters.json");

        Assert.Equal(9, catalog.Chapters.Count);
        Assert.Equal(
            AvailableChapterIds,
            catalog.Chapters.Where(chapter => chapter.IsAvailable).Select(chapter => chapter.Id).Order());
        Assert.Equal(
            ["number-sequences", "prime-numbers"],
            catalog.Chapters.Where(chapter => !chapter.IsAvailable).Select(chapter => chapter.Id).Order());

        foreach (var requirement in RequiredSections)
        {
            var chapter = catalog.Chapters.Single(item => item.Id == requirement.Key);
            var text = string.Join('\n', chapter.Blocks.Select(block => block.Text));

            Assert.True(chapter.IsAvailable, requirement.Key);
            Assert.NotEmpty(chapter.Blocks);
            Assert.All(
                requirement.Value,
                section => Assert.Contains(section, text, StringComparison.Ordinal));
        }
    }

    [Fact]
    public void Greek_alphabet_table_contains_all_24_letters_in_canonical_order()
    {
        var catalog = Read<ChapterCatalog>("Content/chapters.json");
        var chapter = catalog.Chapters.Single(item => item.Id == "greek-alphabet");
        var table = chapter.Blocks.Single(block => block.Text?.StartsWith("Tabela alfabetu greckiego", StringComparison.Ordinal) == true).Text!;
        var rows = table.Split('\n')
            .Where(line => line.Contains('|'))
            .ToArray();

        Assert.Equal(26, rows.Length);
        Assert.Equal("Wielka | Mała | Nazwa polska", rows[0]);
        Assert.Equal("--- | --- | ---", rows[1]);
        Assert.Equal(
            GreekRows,
            rows.Skip(2));
    }

    [Fact]
    public void Issue_35_roadmap_entries_are_migrated_and_remaining_chapters_stay_planned()
    {
        var roadmap = Read<RoadmapCatalog>("Content/roadmap.json");
        var migratedIds = new[]
        {
            "algebra-equations-content",
            "greek-alphabet",
            "logarithms-content",
            "natural-numbers",
            "quadratic-function-content",
            "sets-and-logic-content"
        };

        foreach (var id in migratedIds)
        {
            var item = roadmap.Items.Single(entry => entry.Id == id);
            Assert.Equal(RoadmapStatus.Migrated, item.Status);
            Assert.Contains("Projekt-Inzynierski issue #35", item.SourceRefs);
        }

        var remaining = roadmap.Items.Single(item => item.Id == "chapters-expansion");
        Assert.Equal(RoadmapStatus.Planned, remaining.Status);
        Assert.Contains("ciągach liczbowych", remaining.Summary, StringComparison.Ordinal);
        Assert.Contains("liczbach pierwszych", remaining.Summary, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Legacy_importer_generates_complete_catalogs_from_independent_issue_35_seed()
    {
        var temporaryRoot = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var outputRoot = Path.Combine(temporaryRoot, "output");
        var vectorPath = Path.Combine(temporaryRoot, "vectors.json");
        Directory.CreateDirectory(temporaryRoot);

        try
        {
            var activeChapters = Read<ChapterCatalog>("Content/chapters.json");
            var activeRoadmap = Read<RoadmapCatalog>("Content/roadmap.json");
            var vector = activeChapters.Chapters.Single(chapter => chapter.Id == "vectors");
            var cancellationToken = TestContext.Current.CancellationToken;
            await File.WriteAllTextAsync(vectorPath, JsonSerializer.Serialize(vector, JsonOptions), cancellationToken);

            var importResult = await RunPowerShellAsync(
                Path.Combine(RepositoryRoot, "tools", "Import-LegacyContent.ps1"),
                cancellationToken,
                "-CuratedChaptersOnly",
                "-VectorChapterPath",
                vectorPath,
                "-OutputRoot",
                outputRoot);
            Assert.True(importResult.ExitCode == 0, $"{importResult.StandardOutput}\n{importResult.StandardError}");
            var generatedChapters = ReadFromPath<ChapterCatalog>(Path.Combine(outputRoot, "chapters.json"));
            var generatedRoadmap = ReadFromPath<RoadmapCatalog>(Path.Combine(outputRoot, "roadmap.json"));

            Assert.Equal(JsonSerializer.Serialize(activeChapters, JsonOptions), JsonSerializer.Serialize(generatedChapters, JsonOptions));
            Assert.Equal(JsonSerializer.Serialize(activeRoadmap, JsonOptions), JsonSerializer.Serialize(generatedRoadmap, JsonOptions));
            Assert.Equal(9, generatedChapters.Chapters.Select(chapter => chapter.Id).Distinct(StringComparer.Ordinal).Count());
            Assert.Equal(
                generatedRoadmap.Items.Count,
                generatedRoadmap.Items.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count());

            var importer = await File.ReadAllTextAsync(
                Path.Combine(RepositoryRoot, "tools", "Import-LegacyContent.ps1"),
                cancellationToken);
            Assert.Contains("seeds\\issue-35-content.json", importer, StringComparison.Ordinal);
            Assert.DoesNotContain("$canonicalChapterCatalog", importer, StringComparison.Ordinal);
            Assert.DoesNotContain("$canonicalRoadmapCatalog", importer, StringComparison.Ordinal);

            var guardedResult = await RunPowerShellAsync(
                Path.Combine(RepositoryRoot, "tools", "Import-LegacyContent.ps1"),
                cancellationToken,
                "-CuratedChaptersOnly",
                "-OutputRoot",
                Path.Combine(RepositoryRoot, "Content") + Path.DirectorySeparatorChar);
            Assert.NotEqual(0, guardedResult.ExitCode);
            Assert.Contains(
                "poza aktywnym katalogiem Content",
                guardedResult.StandardOutput + guardedResult.StandardError,
                StringComparison.Ordinal);

            var syncResult = await RunPowerShellAsync(
                Path.Combine(RepositoryRoot, "tools", "Sync-Issue35Content.ps1"),
                cancellationToken,
                "-ContentRoot",
                outputRoot);
            Assert.True(syncResult.ExitCode == 0, $"{syncResult.StandardOutput}\n{syncResult.StandardError}");
            Assert.Equal(
                JsonSerializer.Serialize(activeChapters, JsonOptions),
                JsonSerializer.Serialize(ReadFromPath<ChapterCatalog>(Path.Combine(outputRoot, "chapters.json")), JsonOptions));
            Assert.Equal(
                JsonSerializer.Serialize(activeRoadmap, JsonOptions),
                JsonSerializer.Serialize(ReadFromPath<RoadmapCatalog>(Path.Combine(outputRoot, "roadmap.json")), JsonOptions));
        }
        finally
        {
            if (Directory.Exists(temporaryRoot)) Directory.Delete(temporaryRoot, true);
        }
    }

    [Fact]
    public void Issue_35_completion_is_documented_in_primary_project_documents()
    {
        var paths = new[]
        {
            "README.md",
            "docs/MIGRATION_INVENTORY.md",
            "docs/REQUIREMENTS.md",
            "docs/ROADMAP.md"
        };

        Assert.All(
            paths,
            path => Assert.Contains(
                "issue #35",
                File.ReadAllText(Path.Combine(RepositoryRoot, path.Replace('/', Path.DirectorySeparatorChar))),
                StringComparison.OrdinalIgnoreCase));
    }

    [AvaloniaFact]
    public void Rich_text_table_parser_preserves_math_pipes_and_rejects_invalid_separators()
    {
        const string validTable = "Wzór | Opis\n--- | ---\n\\(|x|\\) | wartość bezwzględna\nlewa \\| prawa | znak pionowy";
        const string invalidTable = "Kolumna A | Kolumna B\n::: | :::\nwartość 1 | wartość 2";
        var window = new Window { Width = 960, Height = 640, Content = RichContentView.CreateText(validTable) };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var validGrid = Assert.Single(
                window.GetLogicalDescendants().OfType<Grid>(),
                grid => grid.RowDefinitions.Count == 3 && grid.ColumnDefinitions.Count == 2);

            Assert.Equal(6, validGrid.Children.Count);
            var mathCell = validGrid.Children.OfType<Border>().Single(
                cell => Grid.GetRow(cell) == 1 && Grid.GetColumn(cell) == 0);
            var mathText = Assert.IsType<TextView>(mathCell.Child);
            Assert.NotNull(mathText.LaTeX);
            Assert.Contains('|', mathText.LaTeX);
            Assert.Contains('x', mathText.LaTeX);
            Assert.True(string.IsNullOrWhiteSpace(mathText.ErrorMessage), mathText.ErrorMessage);
            Assert.Equal("lewa | prawa", TableCellText(validGrid, 2, 0));

            window.Content = RichContentView.CreateText(invalidTable);
            Dispatcher.UIThread.RunJobs();
            Assert.DoesNotContain(
                window.GetLogicalDescendants().OfType<Grid>(),
                grid => grid.RowDefinitions.Count > 0 && grid.ColumnDefinitions.Count == 2);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Chapter_list_opens_and_renders_all_issue_35_content_at_supported_window_sizes()
    {
        var catalog = Read<ChapterCatalog>("Content/chapters.json");
        var opened = new List<ChapterArticle>();
        var view = new ChapterListView(catalog, opened.Add);
        var window = new Window { Width = 960, Height = 640, Content = view };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var buttons = view.GetLogicalDescendants().OfType<Button>().ToArray();

            foreach (var chapter in catalog.Chapters.Where(item => item.IsAvailable))
            {
                var button = buttons.Single(item => string.Equals(item.Content as string, chapter.Title, StringComparison.Ordinal));
                button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }

            Assert.Equal(AvailableChapterIds, opened.Select(chapter => chapter.Id).Order());
            Assert.All(
                catalog.Chapters.Where(item => !item.IsAvailable),
                chapter => Assert.Contains(
                    buttons,
                    button => string.Equals(button.Content as string, chapter.Title + " - treść w przygotowaniu", StringComparison.Ordinal)));

            AssertIssue35ArticleLayouts(window, catalog, new Size(960, 640));
            AssertIssue35ArticleLayouts(window, catalog, new Size(1280, 820));

            var home = new HomeView(
                "Tester",
                Read<UiCopyCatalog>("Content/ui-copy.json"),
                () => { },
                () => { },
                () => { },
                () => { },
                () => { });
            window.Content = home;
            Dispatcher.UIThread.RunJobs();
            Assert.Contains(
                home.GetLogicalDescendants().OfType<TextBlock>(),
                text => text.Text == "7 działów: teoria, przykłady i zadania");
        }
        finally
        {
            window.Close();
        }
    }

    private static void AssertIssue35ArticleLayouts(Window window, ChapterCatalog catalog, Size size)
    {
        window.Width = size.Width;
        window.Height = size.Height;
        foreach (var id in RequiredSections.Keys)
        {
            var chapter = catalog.Chapters.Single(item => item.Id == id);
            var article = new ArticleView(chapter.Title, "Materiał działowy", chapter.Blocks, () => { });
            window.Content = article;
            Dispatcher.UIThread.RunJobs();

            var scroll = Assert.Single(article.GetLogicalDescendants().OfType<ScrollViewer>());
            Assert.InRange(article.Bounds.Width, 1d, window.Bounds.Width);
            Assert.InRange(article.Bounds.Height, 1d, window.Bounds.Height);
            Assert.InRange(scroll.Bounds.Width, 1d, article.Bounds.Width);
            Assert.InRange(scroll.Bounds.Height, 1d, article.Bounds.Height);
            Assert.Equal(Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled, scroll.HorizontalScrollBarVisibility);
            Assert.True(
                scroll.Extent.Width <= scroll.Viewport.Width + 1,
                $"{id} przy {size.Width}x{size.Height}: szerokość treści {scroll.Extent.Width}, viewport {scroll.Viewport.Width}.");
            Assert.NotEmpty(article.GetLogicalDescendants().OfType<TextView>());

            if (id == "greek-alphabet") AssertGreekTable(article, scroll);
        }
    }

    private static void AssertGreekTable(Control article, ScrollViewer scroll)
    {
        var table = Assert.Single(
            article.GetLogicalDescendants().OfType<Grid>(),
            grid => grid.RowDefinitions.Count == 25 && grid.ColumnDefinitions.Count == 3);

        Assert.Equal(75, table.Children.Count);
        Assert.InRange(table.Bounds.Width, 1d, scroll.Bounds.Width);
        Assert.True(table.Bounds.Height > scroll.Bounds.Height);
        Assert.All(table.Children, cell =>
        {
            Assert.True(cell.Bounds.Width > 0);
            Assert.True(cell.Bounds.Height > 0);
        });
        Assert.Equal("Wielka", TableCellText(table, 0, 0));
        Assert.Equal("Mała", TableCellText(table, 0, 1));
        Assert.Equal("Nazwa polska", TableCellText(table, 0, 2));
        Assert.Equal("Α", TableCellText(table, 1, 0));
        Assert.Equal("α", TableCellText(table, 1, 1));
        Assert.Equal("alfa", TableCellText(table, 1, 2));
        Assert.Equal("Ω", TableCellText(table, 24, 0));
        Assert.Equal("ω", TableCellText(table, 24, 1));
        Assert.Equal("omega", TableCellText(table, 24, 2));
    }

    private static string TableCellText(Grid table, int row, int column)
    {
        var border = table.Children.OfType<Border>().Single(cell =>
            Grid.GetRow(cell) == row && Grid.GetColumn(cell) == column);
        var text = Assert.IsType<TextBlock>(border.Child);
        return text.Text ?? string.Empty;
    }

    private static async Task<PowerShellResult> RunPowerShellAsync(
        string scriptPath,
        CancellationToken cancellationToken,
        params string[] arguments)
    {
        var startInfo = new ProcessStartInfo("powershell")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-ExecutionPolicy");
        startInfo.ArgumentList.Add("Bypass");
        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(scriptPath);
        foreach (var argument in arguments) startInfo.ArgumentList.Add(argument);

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Nie uruchomiono PowerShell.");
        var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        return new PowerShellResult(process.ExitCode, await outputTask, await errorTask);
    }

    private static T Read<T>(string relativePath) => JsonSerializer.Deserialize<T>(
        File.ReadAllText(Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar))),
        JsonOptions)!;

    private static T ReadFromPath<T>(string path) => JsonSerializer.Deserialize<T>(
        File.ReadAllText(path),
        JsonOptions)!;

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Abituria.sln"))) return directory.FullName;
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Nie znaleziono katalogu repozytorium Abituria.");
    }

    private sealed record PowerShellResult(int ExitCode, string StandardOutput, string StandardError);
}
