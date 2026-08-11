using System.Security.Cryptography;
using System.Text;

namespace Abituria.Tests;

public sealed class BusinessAnalysisDocumentationTests
{
    private const string HistoricalAnalysisSha256 = "A25F986182E8E8CD82E00D0A1508B42E065CF0F454EFB985FCF10C9888684928";
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Business_analysis_covers_issue_9_with_current_product_facts()
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
            "## 12. Śledzenie Issue #9"
        };

        Assert.All(requiredHeadings, heading => Assert.Contains(heading, text, StringComparison.Ordinal));
        Assert.Contains(".NET 10 LTS", text, StringComparison.Ordinal);
        Assert.Contains("AvaloniaUI 12", text, StringComparison.Ordinal);
        Assert.Contains("SQLite", text, StringComparison.Ordinal);
        Assert.Contains("licencją MIT", text, StringComparison.Ordinal);
        Assert.Contains("releaseEligible", text, StringComparison.Ordinal);
        Assert.Contains("https://github.com/haribo841/Abituria/issues/9", text, StringComparison.Ordinal);
        Assert.Contains(
            "https://github.com/Projekt-Inzynierski-AK-AS-FD/Projekt-Inzynierski/issues/38",
            text,
            StringComparison.Ordinal);
        Assert.Contains("legacy/analiza-biznesowa-pelna.md", text, StringComparison.Ordinal);
        Assert.DoesNotContain("Issue #38", text, StringComparison.Ordinal);
        Assert.Contains("razem 7 arkuszy i 182 jednostki postępu", text, StringComparison.Ordinal);
        Assert.Contains("releases/tag/v0.9.2", text, StringComparison.Ordinal);
        Assert.DoesNotContain("interfejs użytkownika: WPF", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("kalkulator może działać jako osobne okno", text, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Full_historical_analysis_is_archived_separately_from_current_product_facts()
    {
        var text = File.ReadAllText(Absolute("docs/legacy/analiza-biznesowa-pelna.md"));
        var legacyIndex = File.ReadAllText(Absolute("docs/legacy/README.md"));

        Assert.StartsWith("# Historyczna analiza biznesowa", text, StringComparison.Ordinal);
        Assert.Contains("nie jest źródłem prawdy dla bieżącej implementacji", text, StringComparison.Ordinal);
        Assert.Contains("## 1. Cel analizy biznesowej", text, StringComparison.Ordinal);
        Assert.Contains("# 16. Podsumowanie analizy biznesowej", text, StringComparison.Ordinal);
        Assert.Contains("**WPF**", text, StringComparison.Ordinal);
        Assert.Contains("analiza-biznesowa-pelna.md", legacyIndex, StringComparison.Ordinal);
        Assert.Equal(HistoricalAnalysisSha256, HashNormalized(text));
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

    private static string HashNormalized(string text)
    {
        var normalized = text.Replace("\r\n", "\n", StringComparison.Ordinal);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalized)));
    }

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
