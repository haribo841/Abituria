using System.Diagnostics;
using System.Text.Json;

namespace Abituria.Tests;

public sealed class ReleaseSbomScopeTests
{
    private const string ReleaseVersion = "0.9.0-beta.1";
    private const string RuntimeIdentifier = "win-x64";
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public async Task Validator_accepts_exact_production_graph_with_current_runtime_and_host_packs()
    {
        var fixture = CreateFixture();
        try
        {
            var result = await ValidateAsync(fixture);

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("SBOM scope is correct for win-x64", result.StandardOutput, StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(fixture.Root, recursive: true);
        }
    }

    [Theory]
    [InlineData("xunit.v3", "3.2.2", "test packages")]
    [InlineData("coverlet.collector", "6.0.4", "test packages")]
    [InlineData("Microsoft.NET.Test.Sdk", "17.14.1", "test packages")]
    [InlineData("SkiaSharp.NativeAssets.Linux", "3.119.4", "another runtime")]
    [InlineData("Microsoft.NETCore.App.Runtime.linux-x64", "10.0.10", "another runtime")]
    [InlineData("Microsoft.NETCore.App.Host.osx-x64", "10.0.10", "another runtime")]
    public async Task Validator_rejects_test_packages_and_foreign_runtime_components(
        string packageId,
        string packageVersion,
        string expectedError)
    {
        var fixture = CreateFixture((packageId, packageVersion));
        try
        {
            var result = await ValidateAsync(fixture);

            Assert.NotEqual(0, result.ExitCode);
            Assert.Contains(expectedError, result.StandardError, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(fixture.Root, recursive: true);
        }
    }

    private static SbomFixture CreateFixture((string Id, string Version)? unexpectedPackage = null)
    {
        var root = Path.Combine(Path.GetTempPath(), "abituria-sbom-scope-" + Guid.NewGuid().ToString("N"));
        var publishedDirectory = Path.Combine(root, "published");
        Directory.CreateDirectory(publishedDirectory);

        var libraries = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [$"Abituria/{ReleaseVersion}"] = new { type = "project", serviceable = false },
            ["runtimepack.Microsoft.NETCore.App.Runtime.win-x64/10.0.10"] =
                new { type = "runtimepack", serviceable = false },
            ["Avalonia/12.0.4"] = new { type = "package", serviceable = true }
        };
        var dependencies = new
        {
            runtimeTarget = new { name = ".NETCoreApp,Version=v10.0/win-x64" },
            libraries
        };
        File.WriteAllText(
            Path.Combine(publishedDirectory, "Abituria.deps.json"),
            JsonSerializer.Serialize(dependencies));

        var packages = new List<object>
        {
            new { name = "Abituria-win-x64", versionInfo = ReleaseVersion },
            new { name = "Avalonia", versionInfo = "12.0.4" },
            new { name = "Microsoft.NETCore.App.Runtime.win-x64", versionInfo = "10.0.10" },
            new { name = "Microsoft.NETCore.App.Host.win-x64", versionInfo = "10.0.10" }
        };
        if (unexpectedPackage is { } unexpected)
            packages.Add(new { name = unexpected.Id, versionInfo = unexpected.Version });

        var sbomPath = Path.Combine(root, "manifest.spdx.json");
        File.WriteAllText(sbomPath, JsonSerializer.Serialize(new
        {
            spdxVersion = "SPDX-2.2",
            packages
        }));
        return new SbomFixture(root, publishedDirectory, sbomPath);
    }

    private static async Task<PowerShellResult> ValidateAsync(SbomFixture fixture)
    {
        var executable = OperatingSystem.IsWindows() ? "powershell" : "pwsh";
        var startInfo = new ProcessStartInfo(executable)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("-NoProfile");
        if (OperatingSystem.IsWindows())
        {
            startInfo.ArgumentList.Add("-ExecutionPolicy");
            startInfo.ArgumentList.Add("Bypass");
        }
        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(Absolute("tools/release/Test-ReleaseSbomScope.ps1"));
        startInfo.ArgumentList.Add("-SbomPath");
        startInfo.ArgumentList.Add(fixture.SbomPath);
        startInfo.ArgumentList.Add("-PublishedDirectory");
        startInfo.ArgumentList.Add(fixture.PublishedDirectory);
        startInfo.ArgumentList.Add("-RuntimeIdentifier");
        startInfo.ArgumentList.Add(RuntimeIdentifier);
        startInfo.ArgumentList.Add("-Version");
        startInfo.ArgumentList.Add(ReleaseVersion);

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("PowerShell did not start.");
        var standardOutput = process.StandardOutput.ReadToEndAsync();
        var standardError = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(30));
        return new PowerShellResult(process.ExitCode, await standardOutput, await standardError);
    }

    private static string Absolute(string relativePath) =>
        Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Abituria.sln")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Abituria repository was not found.");
    }

    private sealed record SbomFixture(string Root, string PublishedDirectory, string SbomPath);
    private sealed record PowerShellResult(int ExitCode, string StandardOutput, string StandardError);
}
