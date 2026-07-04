using System.Text.Json;
using System.Text.RegularExpressions;
using Abituria.Models;

namespace Abituria.Tests;

public sealed partial class ContentSeparationTests
{
    private const int MaximumInlineLiteralLength = 120;

    [Fact]
    public void Ui_copy_file_contains_every_externalized_long_description()
    {
        var root = FindRepositoryRoot();
        var path = Path.Combine(root, "Content", "ui-copy.json");
        var catalog = JsonSerializer.Deserialize<UiCopyCatalog>(
            File.ReadAllText(path),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(catalog);
        Assert.Equal(1, catalog.SchemaVersion);
        Assert.Equal(7, catalog.Entries.Count);
        Assert.Contains("account.registration.rules", catalog.Entries.Keys);
        Assert.All(catalog.Entries, entry =>
        {
            Assert.False(string.IsNullOrWhiteSpace(entry.Key));
            Assert.False(string.IsNullOrWhiteSpace(entry.Value.Title));
            Assert.True(entry.Value.Body.Length >= 50, $"Treść {entry.Key} jest zbyt krótka, aby należeć do katalogu długich opisów.");
        });
        Assert.Equal(
            "CKE, strona 12. Treść i klucz odpowiedzi zweryfikowano 3 lipca 2026.",
            catalog.FormatRequired("exam.source", "CKE", 12, "3 lipca 2026").Body);
    }

    [Fact]
    public void Production_csharp_contains_no_long_static_text_or_formula_literals()
    {
        var sourceRoot = Path.Combine(FindRepositoryRoot(), "AvaloniaApp");
        var violations = new List<string>();

        foreach (var path in Directory.EnumerateFiles(sourceRoot, "*.cs", SearchOption.AllDirectories))
        {
            var lineNumber = 0;
            foreach (var line in File.ReadLines(path))
            {
                lineNumber++;
                foreach (Match match in CSharpStringLiteral().Matches(line))
                {
                    if (match.Length - 2 <= MaximumInlineLiteralLength) continue;
                    violations.Add($"{Path.GetRelativePath(sourceRoot, path)}:{lineNumber} ({match.Length - 2} znaków)");
                }
            }
        }

        Assert.Empty(violations);
    }

    [GeneratedRegex("\"(?:\\\\.|[^\"\\\\])*\"", RegexOptions.CultureInvariant)]
    private static partial Regex CSharpStringLiteral();

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Abituria.sln"))) return current.FullName;
            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Nie znaleziono katalogu repozytorium Abituria.");
    }
}
