namespace Abituria.Tests;

public sealed class ProjectRequirementsDocumentationTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Requirements_document_covers_issue_34_scope_and_acceptance_criteria()
    {
        var requirementsPath = Path.Combine(RepositoryRoot, "docs", "REQUIREMENTS.md");
        var text = File.ReadAllText(requirementsPath);
        var requiredHeadings = new[]
        {
            "## 1. Cel projektu",
            "## 2. Opis systemu",
            "## 3. Zakres funkcjonalny aplikacji",
            "## 4. Wymagania funkcjonalne",
            "## 5. Wymagania niefunkcjonalne",
            "## 6. Opis użytkowników systemu",
            "## 7. Opis głównych modułów aplikacji",
            "## 8. Przypadki użycia",
            "## 9. Wymagania dotyczące interfejsu użytkownika",
            "## 10. Ograniczenia technologiczne",
            "## 11. Kryteria akceptacji projektu",
            "## Macierz zgodności wymagań z implementacją",
            "## Status issue #34"
        };

        Assert.All(requiredHeadings, heading => Assert.Contains(heading, text, StringComparison.Ordinal));
        Assert.Contains("F-01", text, StringComparison.Ordinal);
        Assert.Contains("NF-01", text, StringComparison.Ordinal);
        Assert.Contains("UC-01", text, StringComparison.Ordinal);
        Assert.Contains("dotnet test Abituria.sln --configuration Release --no-build", text, StringComparison.Ordinal);
        Assert.Contains("SonarQube Cloud nie raportuje otwartych problemów", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Requirements_document_is_discoverable_from_primary_documentation()
    {
        var readme = File.ReadAllText(Path.Combine(RepositoryRoot, "README.md"));
        var architecture = File.ReadAllText(Path.Combine(RepositoryRoot, "docs", "ARCHITECTURE.md"));

        Assert.Contains("docs/REQUIREMENTS.md", readme, StringComparison.Ordinal);
        Assert.Contains("docs/REQUIREMENTS.md", architecture, StringComparison.Ordinal);
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
