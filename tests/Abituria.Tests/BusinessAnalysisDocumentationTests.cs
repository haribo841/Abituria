namespace Abituria.Tests;

public sealed class BusinessAnalysisDocumentationTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Business_analysis_covers_issue_38_with_current_product_facts()
    {
        var text = File.ReadAllText(Absolute("docs/BUSINESS_ANALYSIS.md"));
        var requiredHeadings = new[]
        {
            "## 1. Cel analizy",
            "## 2. Cele produktu i perspektywa klienta",
            "## 3. Użytkownicy, potrzeby i interesariusze",
            "## 4. Model udostępniania i wartość produktu",
            "## 5. Zakres produktu",
            "## 6. Harmonogram i kamienie milowe",
            "## 7. Kryteria akceptacji",
            "## 8. Model licencyjny, dane i prywatność",
            "## 9. Metodyka wymagań i kontrola zmian",
            "## 10. Architektura i ograniczenia technologiczne",
            "## 11. Ryzyka biznesowe i projektowe",
            "## 12. Śledzenie Issue #38"
        };

        Assert.All(requiredHeadings, heading => Assert.Contains(heading, text, StringComparison.Ordinal));
        Assert.Contains(".NET 10 LTS", text, StringComparison.Ordinal);
        Assert.Contains("AvaloniaUI 12", text, StringComparison.Ordinal);
        Assert.Contains("SQLite", text, StringComparison.Ordinal);
        Assert.Contains("licencją MIT", text, StringComparison.Ordinal);
        Assert.Contains("releaseEligible", text, StringComparison.Ordinal);
        Assert.Contains("publiczne prerelease `v0.9.0-beta.1` nie zostało jeszcze opublikowane", text, StringComparison.Ordinal);
        Assert.DoesNotContain("interfejs użytkownika: WPF", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("kalkulator może działać jako osobne okno", text, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Business_analysis_is_linked_from_active_project_documents()
    {
        var readme = File.ReadAllText(Absolute("README.md"));
        var toc = File.ReadAllText(Absolute("docs/toc.yml"));
        var requirements = File.ReadAllText(Absolute("docs/REQUIREMENTS.md"));
        var architecture = File.ReadAllText(Absolute("docs/ARCHITECTURE.md"));
        var roadmap = File.ReadAllText(Absolute("docs/ROADMAP.md"));

        Assert.Contains("docs/BUSINESS_ANALYSIS.md", readme, StringComparison.Ordinal);
        Assert.Contains("href: BUSINESS_ANALYSIS.md", toc, StringComparison.Ordinal);
        Assert.Contains("BUSINESS_ANALYSIS.md", requirements, StringComparison.Ordinal);
        Assert.Contains("docs/BUSINESS_ANALYSIS.md", architecture, StringComparison.Ordinal);
        Assert.Contains("BUSINESS_ANALYSIS.md", roadmap, StringComparison.Ordinal);
    }

    private static string Absolute(string relativePath) => Path.Combine(RepositoryRoot, relativePath);

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
}
