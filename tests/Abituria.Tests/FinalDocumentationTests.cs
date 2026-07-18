namespace Abituria.Tests;

public sealed class FinalDocumentationTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Commission_package_contains_the_required_current_documents()
    {
        var required = new Dictionary<string, string[]>
        {
            ["docs/TESTING.md"] = ["## Cel i zakres", "## Wyniki wydajności, pamięci i obciążenia"],
            ["docs/USABILITY_TEST_PROTOCOL.md"] = ["## Scenariusze", "## Arkusz wyników"],
            ["docs/ACCEPTANCE_PROTOCOL.md"] = ["## Test instalacji na niezależnych komputerach", "## Decyzja odbiorowa"],
            ["docs/COMMISSION_PACKAGE.md"] = ["## Zawartość techniczna", "## PDF"]
        };

        foreach (var (relativePath, headings) in required)
        {
            var path = Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(path), $"Brak dokumentu: {relativePath}");
            var text = File.ReadAllText(path);
            Assert.All(headings, heading => Assert.Contains(heading, text, StringComparison.Ordinal));
        }
    }

    [Fact]
    public void Commission_pdf_is_versioned_and_has_a_valid_pdf_header()
    {
        var pdfPath = Path.Combine(
            RepositoryRoot,
            "output",
            "pdf",
            "Abituria-Technical-Documentation-0.9.0-beta.1.pdf");

        Assert.True(File.Exists(pdfPath), "Brak wygenerowanego PDF dla komisji.");
        var header = File.ReadAllBytes(pdfPath).Take(5).ToArray();
        Assert.Equal("%PDF-", System.Text.Encoding.ASCII.GetString(header));
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Abituria.sln"))) return directory.FullName;
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Nie znaleziono repozytorium Abituria.");
    }
}
