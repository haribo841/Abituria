using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Abituria.Tests;

public sealed class ReadmeProductPageTests
{
    private const string ArchivedReadmeSha256 = "C36820EF7C8BFCF989C165AA57D16985920302F26969C41EEBEDD860BF3D09B4";
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Readme_is_a_concise_bilingual_product_page()
    {
        var readme = File.ReadAllText(Absolute("README.md"));
        var requiredSections = new[]
        {
            "# 🍀 Abituria",
            "## Pobieranie",
            "## Najprostszy start",
            "## Najważniejsze funkcje",
            "## Jak wygląda nauka",
            "## Technologie i architektura",
            "## Uruchomienie ze źródeł",
            "## Dokumentacja i projekt",
            "## Kontakt i zgłoszenia"
        };

        Assert.InRange(readme.Length, 3_000, 7_500);
        Assert.All(requiredSections, section => Assert.Contains(section, readme, StringComparison.Ordinal));
        Assert.Contains(
            "Offline desktop mathematics tutor for Polish high-school students preparing for the matura exam.",
            readme,
            StringComparison.Ordinal);
        Assert.Contains("```mermaid", readme, StringComparison.Ordinal);
        Assert.Contains("flowchart LR", readme, StringComparison.Ordinal);
        Assert.DoesNotContain('\u2014', readme);
        Assert.DoesNotContain('\u2013', readme);
    }

    [Fact]
    public void Readme_leads_to_verified_downloads_and_support_information()
    {
        var readme = File.ReadAllText(Absolute("README.md"));
        var requiredFragments = new[]
        {
            "releases/download/v0.9.3/Abituria-v0.9.3-win-x64.exe",
            "releases/download/v0.9.3/Abituria-v0.9.3-linux-x64.tar.gz",
            "releases/download/v0.9.3/Abituria-v0.9.3-osx-x64.zip",
            "releases/download/v0.9.3/SHA256SUMS.txt",
            "Windows 11 24H2 x64",
            "Ubuntu 24.04 x64",
            "macOS 15 Intel x64",
            "docs/INSTALLATION.md",
            "docs/USER_GUIDE.md",
            "docs/KNOWN_LIMITATIONS.md",
            "docs/BUSINESS_ANALYSIS.md",
            "docs/REQUIREMENTS.md",
            "docs/ARCHITECTURE.md",
            "docs/TESTING.md",
            "docs/CONTENT_PROVENANCE.md",
            "docs/acceptance/README.md",
            "docs/DEFENSE_PROTOCOL.md",
            "docs/EVALUATION_PROTOCOL.md",
            "[Współtworzenie](CONTRIBUTING.md)",
            "[MIT](LICENSE)",
            "SUPPORT.md",
            "SECURITY.md",
            "issues/new/choose"
        };

        Assert.All(requiredFragments, fragment => Assert.Contains(fragment, readme, StringComparison.Ordinal));
        Assert.Contains("Najnowszym publicznym wydaniem jest `v0.9.3`", readme, StringComparison.Ordinal);
        Assert.Contains("gałęzi `main`", readme, StringComparison.Ordinal);
        Assert.Contains("`releaseEligible=false`", readme, StringComparison.Ordinal);
        Assert.DoesNotContain("Start-Process", readme, StringComparison.Ordinal);
    }

    [Fact]
    public void Readme_uses_repository_owned_workflow_badges()
    {
        var readme = File.ReadAllText(Absolute("README.md"));

        foreach (var workflow in new[] { "build.yml", "sonarcloud.yml", "codeql.yml", "pages.yml" })
        {
            Assert.Contains(
                $"https://github.com/haribo841/Abituria/actions/workflows/{workflow}/badge.svg?branch=main",
                readme,
                StringComparison.Ordinal);
        }

        Assert.DoesNotContain("shields.io", readme, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Readme_screenshots_are_versioned_accessible_and_consistent()
    {
        var readme = File.ReadAllText(Absolute("README.md"));
        var screenshots = new Dictionary<string, string>
        {
            ["docs/assets/readme/home.png"] = "Ekran główny Abiturii z sześcioma modułami aplikacji",
            ["docs/assets/readme/learning.png"] = "Autorskie ćwiczenie kursowe z widoczną stopniowaną podpowiedzią",
            ["docs/assets/readme/calculator-pip.png"] = "Brudnopis zadania i kalkulator Picture in Picture z wartością Ans"
        };

        foreach (var (relativePath, alternativeText) in screenshots)
        {
            Assert.Contains($"![{alternativeText}]({relativePath})", readme, StringComparison.Ordinal);
            AssertPngHasExpectedShape(relativePath, expectedWidth: 1280, expectedHeight: 820);
        }

        Assert.Contains("nie zawierają transkrybowanych treści CKE", readme, StringComparison.Ordinal);
    }

    [Fact]
    public void Previous_readme_is_preserved_as_a_normalized_snapshot()
    {
        var archive = File.ReadAllText(Absolute("docs/legacy/README-2026-09-05.md"));
        var legacyIndex = File.ReadAllText(Absolute("docs/legacy/README.md"));

        Assert.Equal(ArchivedReadmeSha256, HashNormalized(archive));
        Assert.Contains("README-2026-09-05.md", legacyIndex, StringComparison.Ordinal);
        Assert.Contains("archiwum README z 2026-09-05", File.ReadAllText(Absolute("README.md")), StringComparison.Ordinal);
    }

    private static void AssertPngHasExpectedShape(string relativePath, int expectedWidth, int expectedHeight)
    {
        var bytes = File.ReadAllBytes(Absolute(relativePath));
        ReadOnlySpan<byte> signature = [137, 80, 78, 71, 13, 10, 26, 10];

        Assert.InRange(bytes.Length, 100, 1_500_000);
        Assert.True(bytes.AsSpan(0, signature.Length).SequenceEqual(signature), $"{relativePath} nie jest plikiem PNG.");
        Assert.Equal(expectedWidth, BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(16, 4)));
        Assert.Equal(expectedHeight, BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(20, 4)));
    }

    private static string HashNormalized(string text)
    {
        var normalized = text.Replace("\r\n", "\n", StringComparison.Ordinal);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalized)));
    }

    private static string Absolute(string relativePath) =>
        Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

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
