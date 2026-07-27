using System.Diagnostics;
using System.Text;

namespace Abituria.Tests;

public sealed class ReleaseValidationScriptTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    public static TheoryData<string, byte[], string> BinarySecrets => new()
    {
        {
            "aws.bin",
            CrossChunkBoundary(AwsAccessKeyFixture()),
            "AWS access key"
        },
        {
            "session.cache",
            JwtFixture(),
            "JWT"
        },
        {
            "native.dat",
            ConnectionStringFixture(),
            "connection string credential"
        }
    };

    [Theory]
    [MemberData(nameof(BinarySecrets))]
    public void Package_secret_scanner_rejects_credentials_in_binary_files(
        string fileName,
        byte[] content,
        string expectedKind)
    {
        using var directory = new TemporaryDirectory();
        File.WriteAllBytes(Path.Combine(directory.Path, fileName), content);

        var result = RunPowerShell(
            "try { Import-Module $env:ABITURIA_MODULE -Force; " +
            "Test-PackagedSecrets -PackageDirectory $env:ABITURIA_FIXTURE; exit 0 } " +
            "catch { [Console]::Error.WriteLine($_.Exception.Message); exit 17 }",
            directory.Path);

        Assert.Equal(17, result.ExitCode);
        Assert.Contains(expectedKind, result.StandardError, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Package_secret_scanner_accepts_documented_placeholders_and_normal_binary_data()
    {
        using var directory = new TemporaryDirectory();
        File.WriteAllBytes(
            Path.Combine(directory.Path, "Abituria.bin"),
            DocumentedPlaceholderFixture());

        var result = RunPowerShell(
            "try { Import-Module $env:ABITURIA_MODULE -Force; " +
            "Test-PackagedSecrets -PackageDirectory $env:ABITURIA_FIXTURE; exit 0 } " +
            "catch { [Console]::Error.WriteLine($_.Exception.Message); exit 17 }",
            directory.Path);

        Assert.Equal(0, result.ExitCode);
    }

    [Theory]
    [InlineData("https://example.invalid/path", "not present")]
    [InlineData("http://github.com/haribo841/Abituria", "must use HTTPS")]
    [InlineData(
        "https://github.com/haribo841/Abituria/blob/main/definitely-missing-release-file.md",
        "does not resolve")]
    public void Documentation_validator_rejects_links_outside_the_deterministic_policy(
        string link,
        string expectedMessage)
    {
        using var directory = new TemporaryDirectory();
        File.WriteAllText(
            Path.Combine(directory.Path, "index.html"),
            $"<html><body><a href=\"{link}\">test</a></body></html>",
            Encoding.UTF8);

        var result = RunPowerShell(
            "try { & $env:ABITURIA_LINK_SCRIPT -SiteDirectory $env:ABITURIA_FIXTURE; exit 0 } " +
            "catch { [Console]::Error.WriteLine($_.Exception.Message); exit 17 }",
            directory.Path);

        Assert.Equal(17, result.ExitCode);
        Assert.Contains(expectedMessage, result.StandardError, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Documentation_validator_accepts_allowed_https_and_existing_local_targets_without_network()
    {
        using var directory = new TemporaryDirectory();
        File.WriteAllText(Path.Combine(directory.Path, "guide.html"), "guide", Encoding.UTF8);
        File.WriteAllText(
            Path.Combine(directory.Path, "index.html"),
            "<a href=\"guide.html\">guide</a>" +
            "<a href=\"https://github.com/haribo841/Abituria\">repo</a>" +
            "<a href=\"https://github.com/haribo841/Abituria/blob/main/README.md\">readme</a>",
            Encoding.UTF8);

        var result = RunPowerShell(
            "try { & $env:ABITURIA_LINK_SCRIPT -SiteDirectory $env:ABITURIA_FIXTURE; exit 0 } " +
            "catch { [Console]::Error.WriteLine($_.Exception.Message); exit 17 }",
            directory.Path);

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Documentation_validator_does_not_probe_contributor_covenant_from_ci()
    {
        var excludedLinks = new[]
        {
            "https://www.contributor-covenant.org/",
            "https://www.contributor-covenant.org/pl/version/1/4/code-of-conduct.html",
            "https://www.contributor-covenant.org/version/1/4/code-of-conduct.html"
        };

        AssertLinksAreExcludedFromOnlineProbe(excludedLinks);
    }

    [Fact]
    public void Documentation_validator_does_not_probe_nuget_package_pages_from_ci()
    {
        var excludedLinks = new[]
        {
            "https://www.nuget.org/packages/Avalonia/12.0.4",
            "https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/10.0.10",
            "https://www.nuget.org/packages/SQLitePCLRaw.bundle_e_sqlite3/2.1.12"
        };

        AssertLinksAreExcludedFromOnlineProbe(excludedLinks);
    }

    [Fact]
    public void Documentation_validator_uses_pinned_hash_instead_of_probing_cke_pdf()
    {
        AssertLinksAreExcludedFromOnlineProbe(
            ["https://bip.cke.gov.pl/attachments/download/9944"]);
    }

    private static void AssertLinksAreExcludedFromOnlineProbe(string[] excludedLinks)
    {
        using var directory = new TemporaryDirectory();
        File.WriteAllText(
            Path.Combine(directory.Path, "index.html"),
            string.Concat(excludedLinks.Select(link => $"<a href=\"{link}\">attribution</a>")),
            Encoding.UTF8);

        var result = RunPowerShell(
            "try { & $env:ABITURIA_LINK_SCRIPT -SiteDirectory $env:ABITURIA_FIXTURE " +
            "-CheckExternalLinks -ExternalLinkFailureAction Fail; exit 0 } " +
            "catch { [Console]::Error.WriteLine($_.Exception.Message); exit 17 }",
            directory.Path);

        Assert.Equal(0, result.ExitCode);
        Assert.All(excludedLinks, link =>
            Assert.Contains($"Skipping online probe for '{link}'", result.StandardOutput, StringComparison.Ordinal));
        Assert.DoesNotContain("External links were unavailable", result.StandardError, StringComparison.Ordinal);
    }

    [Fact]
    public void Documentation_validator_accepts_existing_same_page_and_cross_page_fragments()
    {
        using var directory = new TemporaryDirectory();
        File.WriteAllText(
            Path.Combine(directory.Path, "guide.html"),
            "<html><body><h2 ID=\"details\">Details</h2></body></html>",
            Encoding.UTF8);
        File.WriteAllText(
            Path.Combine(directory.Path, "index.html"),
            "<html><body><h2 id=\"overview\">Overview</h2>" +
            "<a href=\"#overview\">overview</a><a href=\"guide.html#details\">details</a></body></html>",
            Encoding.UTF8);

        var result = RunPowerShell(
            "try { & $env:ABITURIA_LINK_SCRIPT -SiteDirectory $env:ABITURIA_FIXTURE; exit 0 } " +
            "catch { [Console]::Error.WriteLine($_.Exception.Message); exit 17 }",
            directory.Path);

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Coverage_gate_accepts_combined_csharp_and_python_reports_above_thresholds()
    {
        using var directory = new TemporaryDirectory();
        WriteCoverageFixtures(directory.Path, covered: true);

        var result = RunPowerShell(
            "try { & $env:ABITURIA_COVERAGE_SCRIPT " +
            "-OpenCoverReport (Join-Path $env:ABITURIA_FIXTURE 'csharp.xml') " +
            "-PythonCoverageReport (Join-Path $env:ABITURIA_FIXTURE 'python.xml'); exit 0 } " +
            "catch { [Console]::Error.WriteLine($_.Exception.Message); exit 17 }",
            directory.Path);

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Coverage gate passed", result.StandardOutput, StringComparison.Ordinal);
    }

    [Fact]
    public void Coverage_gate_rejects_reports_below_overall_and_branch_thresholds()
    {
        using var directory = new TemporaryDirectory();
        WriteCoverageFixtures(directory.Path, covered: false);

        var result = RunPowerShell(
            "try { & $env:ABITURIA_COVERAGE_SCRIPT " +
            "-OpenCoverReport (Join-Path $env:ABITURIA_FIXTURE 'csharp.xml') " +
            "-PythonCoverageReport (Join-Path $env:ABITURIA_FIXTURE 'python.xml'); exit 0 } " +
            "catch { [Console]::Error.WriteLine($_.Exception.Message); exit 17 }",
            directory.Path);

        Assert.Equal(17, result.ExitCode);
        Assert.Contains("overall coverage", result.StandardError, StringComparison.Ordinal);
        Assert.Contains("branch coverage", result.StandardError, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("<h2 data-id=\"target\">Target</h2>", "#target")]
    [InlineData("<h2 id=\"target\">Target</h2>", "#TARGET")]
    public void Documentation_validator_requires_an_exact_case_sensitive_id_or_name_attribute(
        string targetHtml,
        string link)
    {
        using var directory = new TemporaryDirectory();
        File.WriteAllText(Path.Combine(directory.Path, "index.html"), targetHtml + $"<a href=\"{link}\">link</a>", Encoding.UTF8);

        var result = RunPowerShell(
            "try { & $env:ABITURIA_LINK_SCRIPT -SiteDirectory $env:ABITURIA_FIXTURE; exit 0 } " +
            "catch { [Console]::Error.WriteLine($_.Exception.Message); exit 17 }",
            directory.Path);

        Assert.Equal(17, result.ExitCode);
        Assert.Contains("missing fragment", result.StandardError, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("#missing")]
    [InlineData("guide.html#missing")]
    public void Documentation_validator_rejects_missing_local_fragments(string link)
    {
        using var directory = new TemporaryDirectory();
        File.WriteAllText(Path.Combine(directory.Path, "guide.html"), "<h2 id=\"present\">Guide</h2>", Encoding.UTF8);
        File.WriteAllText(
            Path.Combine(directory.Path, "index.html"),
            $"<html><body><a href=\"{link}\">broken</a></body></html>",
            Encoding.UTF8);

        var result = RunPowerShell(
            "try { & $env:ABITURIA_LINK_SCRIPT -SiteDirectory $env:ABITURIA_FIXTURE; exit 0 } " +
            "catch { [Console]::Error.WriteLine($_.Exception.Message); exit 17 }",
            directory.Path);

        Assert.Equal(17, result.ExitCode);
        Assert.Contains("missing fragment", result.StandardError, StringComparison.OrdinalIgnoreCase);
    }

    private static byte[] CrossChunkBoundary(byte[] secret)
    {
        var prefix = Enumerable.Repeat((byte)'x', (1024 * 1024) - 8).ToArray();
        return [.. prefix, .. secret, (byte)'y'];
    }

    private static byte[] AwsAccessKeyFixture() =>
        [65, 75, 73, 65, 49, 50, 51, 52, 53, 54, 55, 56, 57, 48, 65, 66, 67, 68, 69, 70];

    private static byte[] JwtFixture()
    {
        var segments = new[]
        {
            "eyJ" + "hbGciOiJIUzI1NiJ9",
            "c3ViamVjdC10ZXN0LXVzZXI",
            "c2lnbmF0dXJlLW5vdC1yZWFs"
        };
        return Encoding.ASCII.GetBytes("prefix " + string.Join('.', segments) + " suffix");
    }

    private static byte[] ConnectionStringFixture()
    {
        var credentialName = CredentialNameFixture();
        var credentialValue = new string(['n', 'o', 't', '-', 'a', '-', 'r', 'e', 'a', 'l', '-', 's', 'e', 'c', 'r', 'e', 't']);
        return Encoding.Unicode.GetBytes(
            $"Server=db.invalid;User Id=test;{credentialName}=\"{credentialValue}\";");
    }

    private static byte[] DocumentedPlaceholderFixture()
    {
        var credentialName = CredentialNameFixture();
        var environmentPlaceholder = "${DATABASE_" + "PASSWORD}";
        return Encoding.UTF8.GetBytes(
            $"Data Source=abituria.db;{credentialName}={environmentPlaceholder};" +
            $"{credentialName}=********;{credentialName}=\"{environmentPlaceholder}\";");
    }

    private static string CredentialNameFixture() =>
        new(['P', 'a', 's', 's', 'w', 'o', 'r', 'd']);

    private static void WriteCoverageFixtures(string directory, bool covered)
    {
        var sequenceVisitCount = covered ? 1 : 0;
        var coveredLineCount = covered ? 2 : 1;
        var coveredBranchCount = covered ? 2 : 0;
        File.WriteAllText(
            Path.Combine(directory, "csharp.xml"),
            $"""
            <CoverageSession>
              <Summary numBranchPoints="2" visitedBranchPoints="{coveredBranchCount}" />
              <Modules><Module>
                <Files><File uid="1" fullPath="Calculator.cs" /></Files>
                <Classes><Class><Methods><Method><SequencePoints>
                  <SequencePoint vc="1" fileid="1" sl="10" />
                  <SequencePoint vc="{sequenceVisitCount}" fileid="1" sl="11" />
                </SequencePoints></Method></Methods></Class></Classes>
              </Module></Modules>
            </CoverageSession>
            """,
            Encoding.UTF8);
        File.WriteAllText(
            Path.Combine(directory, "python.xml"),
            $"<coverage lines-valid=\"2\" lines-covered=\"{coveredLineCount}\" " +
            $"branches-valid=\"2\" branches-covered=\"{coveredBranchCount}\" />",
            Encoding.UTF8);
    }

    private static ProcessResult RunPowerShell(string command, string fixturePath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = OperatingSystem.IsWindows() ? "powershell.exe" : "pwsh",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        if (OperatingSystem.IsWindows())
        {
            startInfo.ArgumentList.Add("-ExecutionPolicy");
            startInfo.ArgumentList.Add("Bypass");
        }
        startInfo.ArgumentList.Add("-Command");
        startInfo.ArgumentList.Add(command);
        startInfo.Environment["ABITURIA_FIXTURE"] = fixturePath;
        startInfo.Environment["ABITURIA_MODULE"] = Absolute("tools/release/PackageSecurity.psm1");
        startInfo.Environment["ABITURIA_LINK_SCRIPT"] = Absolute("tools/release/Test-DocumentationSite.ps1");
        startInfo.Environment["ABITURIA_COVERAGE_SCRIPT"] = Absolute("tools/release/Test-CoverageThreshold.ps1");

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("PowerShell did not start.");
        var standardOutput = process.StandardOutput.ReadToEnd();
        var standardError = process.StandardError.ReadToEnd();
        process.WaitForExit();
        return new ProcessResult(process.ExitCode, standardOutput, standardError);
    }

    private static string Absolute(string relativePath) =>
        Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Abituria.sln")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Nie znaleziono repozytorium Abituria.");
    }

    private sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);

    private sealed class TemporaryDirectory : IDisposable
    {
        public TemporaryDirectory()
        {
            Path = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                "abituria-release-validation-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose() => Directory.Delete(Path, recursive: true);
    }
}
