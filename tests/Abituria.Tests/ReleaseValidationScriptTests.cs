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
