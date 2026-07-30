using System.Text.Json;
using System.Text.RegularExpressions;
using Abituria.Models;
using Abituria.Services;
using Abituria.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CSharpMath.Avalonia;

namespace Abituria.Tests;

public sealed partial class Formula2023ContentTests
{
    private const string ExpectedDocumentUrl = "https://bip.cke.gov.pl/attachments/download/9944";
    private const string ExpectedDocumentHash = "57CFF1265A7E38C13ECB6A00F566A37CDFDA667ABF2D550BA65E19E166CC0D45";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Formula_catalog_has_verified_formula_2023_source_and_stable_eighteen_article_contract()
    {
        var catalog = Read<FormulaCatalog>("Content/formulas.json");

        Assert.Equal(4, catalog.SchemaVersion);
        Assert.Equal("Centralna Komisja Egzaminacyjna", catalog.Source.Publisher);
        Assert.Equal("Wybrane wzory matematyczne na egzamin maturalny z matematyki", catalog.Source.Title);
        Assert.Equal(ExpectedDocumentUrl, catalog.Source.DocumentUrl);
        Assert.Equal(ExpectedDocumentHash, catalog.Source.DocumentSha256);
        Assert.Equal("2024-08-26", catalog.Source.PublishedOn);
        Assert.Equal("2026-07-27", catalog.Source.VerifiedOn);
        Assert.Equal(
            Enumerable.Range(1, 18).Select(number => $"formula-{number}"),
            catalog.Articles.OrderBy(article => article.Order).Select(article => article.Id));
        Assert.Equal(Enumerable.Range(1, 18), catalog.Articles.OrderBy(article => article.Order).Select(article => article.Order));
    }

    [Fact]
    public void Coverage_manifest_maps_every_official_section_and_every_app_article()
    {
        using var manifest = JsonDocument.Parse(ReadText("tools/seeds/formula-2023-coverage.json"));
        var root = manifest.RootElement;
        var source = root.GetProperty("source");
        var sections = root.GetProperty("sections").EnumerateArray().ToArray();
        var catalog = Read<FormulaCatalog>("Content/formulas.json");

        Assert.Equal(1, root.GetProperty("schemaVersion").GetInt32());
        Assert.Equal(ExpectedDocumentUrl, source.GetProperty("documentUrl").GetString());
        Assert.Equal(ExpectedDocumentHash, source.GetProperty("documentSha256").GetString());
        Assert.Equal(17, sections.Length);
        Assert.Equal(Enumerable.Range(1, 17), sections.Select(section => section.GetProperty("number").GetInt32()));

        var articleIds = sections
            .SelectMany(section => section.GetProperty("articleIds").EnumerateArray())
            .Select(value => value.GetString()!)
            .ToHashSet(StringComparer.Ordinal);
        Assert.Equal(
            catalog.Articles.Select(article => article.Id).Order(StringComparer.Ordinal),
            articleIds.Order(StringComparer.Ordinal));

        var requiredItems = sections
            .SelectMany(section => section.GetProperty("requiredItems").EnumerateArray())
            .Select(value => value.GetString()!)
            .ToArray();
        Assert.InRange(requiredItems.Length, 100, int.MaxValue);
        Assert.All(requiredItems, item => Assert.False(string.IsNullOrWhiteSpace(item)));
        Assert.All(
            sections,
            section => Assert.NotEmpty(section.GetProperty("requiredItems").EnumerateArray()));
    }

    [Fact]
    public void Known_formula_defects_are_removed_and_formula_2023_additions_are_present()
    {
        var text = string.Join(
            '\n',
            Read<FormulaCatalog>("Content/formulas.json").Articles
                .SelectMany(article => article.Blocks)
                .Where(block => block.Type == "richText")
                .Select(block => block.Text));

        Assert.Contains(@"| x\cdot y|=| x|\cdot| y|", text, StringComparison.Ordinal);
        Assert.Contains(@"(a+b)^n=\binom{n}{0}a^n", text, StringComparison.Ordinal);
        Assert.Contains(@"(a-b)^n=\binom{n}{0}a^n", text, StringComparison.Ordinal);
        Assert.Contains(@"a^3+1=(a+1)(a^2-a+1)", text, StringComparison.Ordinal);
        Assert.Contains(@"\frac{| Ax_0+By_0+C|}{\sqrt{A^2+B^2}}", text, StringComparison.Ordinal);
        Assert.Contains(@"a_1a_2=-1", text, StringComparison.Ordinal);
        Assert.Contains("Twierdzenie o trzech ciągach", text, StringComparison.Ordinal);
        Assert.Contains("Schemat Bernoullego", text, StringComparison.Ordinal);
        Assert.Contains("Twierdzenie Bayesa", text, StringComparison.Ordinal);
        Assert.Contains("Wartość oczekiwana", text, StringComparison.Ordinal);
        Assert.Contains("Średnia kwadratowa", text, StringComparison.Ordinal);
        Assert.Contains(@"\binom{n}{k}+\binom{n}{k+1}", text, StringComparison.Ordinal);
        Assert.DoesNotContain(@"| x\cdot y|\geq", text, StringComparison.Ordinal);
        Assert.DoesNotContain(@"(a+b)^n=\binom{n}{0}=", text, StringComparison.Ordinal);
        Assert.DoesNotContain(@"a^3-1=(a+1)", text, StringComparison.Ordinal);
        Assert.DoesNotContain(@"Ax_0+By_0+C=0}{", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Trigonometric_table_contains_every_angle_from_zero_through_ninety_exactly_once()
    {
        var table = Read<FormulaCatalog>("Content/formulas.json").Articles.Single(article => article.Id == "formula-18");
        var rows = table.Blocks
            .Where(block => block.Type == "richText")
            .SelectMany(block => block.Text!.Split('\n'))
            .Select(line => TrigonometricRow().Match(line))
            .Where(match => match.Success)
            .Select(match => int.Parse(match.Groups["angle"].Value, System.Globalization.CultureInfo.InvariantCulture))
            .ToArray();

        Assert.Equal(91, rows.Length);
        Assert.Equal(Enumerable.Range(0, 91), rows);
        Assert.Equal(91, rows.Distinct().Count());
        Assert.Contains("| 0 | 0,0000 | 1,0000 | 0,0000 |", string.Join('\n', table.Blocks.Select(block => block.Text)));
        Assert.Contains("| 90 | 1,0000 | 0,0000 | nie istnieje |", string.Join('\n', table.Blocks.Select(block => block.Text)));
        Assert.DoesNotContain(table.Blocks, block => block.Type == "image");
    }

    [Fact]
    public void Legacy_importer_copies_curated_formula_catalog_instead_of_parsing_old_formula_pages()
    {
        var importer = ReadText("tools/Import-LegacyContent.ps1");

        Assert.Contains("$FormulaCatalogPath", importer, StringComparison.Ordinal);
        Assert.Contains("schemaVersion -ne 4", importer, StringComparison.Ordinal);
        Assert.Contains("Write-Json 'formulas.json' $formulaCatalog", importer, StringComparison.Ordinal);
        Assert.DoesNotContain("Get-FormulaArticles", importer, StringComparison.Ordinal);
        Assert.DoesNotContain("pages\\equations", importer, StringComparison.Ordinal);
    }

    [Fact]
    public void Formula_2023_source_and_rights_are_documented_and_routed_through_docfx()
    {
        var rights = ReadText("docs/ASSET_RIGHTS_DECLARATION.md");
        var provenance = ReadText("Content/provenance.json");
        var toc = ReadText("docs/toc.yml");
        var viewSource = ReadText("AvaloniaApp/Views/ContentViews.cs");

        Assert.Contains("Rozszerzenie deklaracji z 27 lipca 2026 r.", rights, StringComparison.Ordinal);
        Assert.Contains(ExpectedDocumentHash, rights, StringComparison.Ordinal);
        Assert.Contains("\"id\": \"cke-formula-2023-transcription\"", provenance, StringComparison.Ordinal);
        Assert.Contains("FORMULA_2023_COVERAGE.md", toc, StringComparison.Ordinal);
        Assert.Contains("zgodnych zakresem z tablicami CKE dla Formuły 2023", viewSource, StringComparison.Ordinal);
    }

    [AvaloniaTheory]
    [InlineData(960, 640)]
    [InlineData(1280, 820)]
    public void Every_formula_article_renders_without_math_errors_or_horizontal_overflow(int width, int height)
    {
        var repository = new ContentRepository();
        var catalog = repository.Formulas;
        foreach (var article in catalog.Articles)
        {
            var view = new ArticleView(
                article.Title,
                "Tablice CKE - Formuła 2023",
                article.Blocks,
                () => { },
                repository.Diagrams);
            var window = ShowInWindow(view, width, height);
            try
            {
                var scroll = Assert.Single(view.GetVisualDescendants().OfType<ScrollViewer>());
                Assert.True(
                    scroll.Extent.Width <= scroll.Viewport.Width + 1d,
                    $"{article.Id}: extent={scroll.Extent.Width}, viewport={scroll.Viewport.Width}");
                Assert.All(
                    view.GetVisualDescendants().OfType<TextView>(),
                    text => Assert.True(
                        string.IsNullOrWhiteSpace(text.ErrorMessage),
                        $"{article.Id}: {text.LaTeX}: {text.ErrorMessage}"));
            }
            finally
            {
                window.Close();
            }
        }
    }

    [GeneratedRegex(@"^\|\s*(?<angle>\d{1,2})\s*\|", RegexOptions.CultureInvariant)]
    private static partial Regex TrigonometricRow();

    private static T Read<T>(string relativePath) =>
        JsonSerializer.Deserialize<T>(ReadText(relativePath), JsonOptions)!;

    private static string ReadText(string relativePath) =>
        File.ReadAllText(Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar)));

    private static Window ShowInWindow(Control content, double width, double height)
    {
        var window = new Window
        {
            Width = width,
            Height = height,
            Background = Brushes.White,
            Content = content
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Abituria.csproj")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Nie znaleziono katalogu repozytorium.");
    }
}
