using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Abituria.Tests;

public sealed class NuGetLicenseBundleTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Bundle_is_deterministic_and_preserves_nuspec_license_and_notice_evidence()
    {
        using var fixture = new TemporaryDirectory();
        var packageRoot = fixture.CreateDirectory("packages");
        CreatePackage(
            packageRoot,
            "Example.Package",
            "1.2.3",
            "<license type=\"expression\">MIT</license><copyright>Example Authors</copyright>",
            ("LICENSE.txt", "MIT license"),
            ("legal/ThirdPartyNotices.txt", "Dependency notice"));
        CreatePackage(
            packageRoot,
            "File.Licensed",
            "2.0.0",
            "<license type=\"file\">legal/COPYING.custom</license>",
            ("legal/COPYING.custom", "File license"));
        var componentsPath = WriteComponents(
            fixture.Path,
            ("File.Licensed", "2.0.0"),
            ("Example.Package", "1.2.3"));

        var firstOutput = Path.Combine(fixture.Path, "bundle-a");
        var secondOutput = Path.Combine(fixture.Path, "bundle-b");
        AssertSuccess(RunScript(componentsPath, firstOutput, packageRoot));
        AssertSuccess(RunScript(componentsPath, secondOutput, packageRoot));

        AssertBundlesEqual(firstOutput, secondOutput);
        using var manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(firstOutput, "manifest.json")));
        var root = manifest.RootElement;
        Assert.Equal(1, root.GetProperty("schemaVersion").GetInt32());
        Assert.Equal(2, root.GetProperty("componentCount").GetInt32());
        var components = root.GetProperty("components").EnumerateArray().ToArray();
        Assert.Equal("Example.Package", components[0].GetProperty("id").GetString());
        Assert.Equal("expression", components[0].GetProperty("declaredLicense").GetProperty("kind").GetString());
        Assert.Equal(2, components[0].GetProperty("evidence").GetArrayLength());
        Assert.Equal("File.Licensed", components[1].GetProperty("id").GetString());
        Assert.Equal("file", components[1].GetProperty("declaredLicense").GetProperty("kind").GetString());
        Assert.All(components, component =>
        {
            Assert.False(component.GetProperty("externallyHandled").GetBoolean());
            Assert.Equal(64, component.GetProperty("nuspec").GetProperty("sha256").GetString()!.Length);
        });
    }

    [Fact]
    public void Bundle_rejects_a_component_without_license_evidence_and_leaves_no_partial_output()
    {
        using var fixture = new TemporaryDirectory();
        var packageRoot = fixture.CreateDirectory("packages");
        CreatePackage(packageRoot, "Missing.License", "1.0.0", string.Empty);
        var componentsPath = WriteComponents(fixture.Path, ("Missing.License", "1.0.0"));
        var output = Path.Combine(fixture.Path, "bundle");

        var result = RunScript(componentsPath, output, packageRoot);

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("does not provide license evidence", result.StandardError, StringComparison.OrdinalIgnoreCase);
        Assert.False(Directory.Exists(output));
        Assert.Empty(Directory.GetDirectories(fixture.Path, "bundle.tmp-*"));
    }

    [Fact]
    public void Bundle_rejects_unsafe_component_identity_before_accessing_the_cache()
    {
        using var fixture = new TemporaryDirectory();
        var packageRoot = fixture.CreateDirectory("packages");
        var componentsPath = WriteComponents(fixture.Path, ("../escape", "1.0.0"));

        var result = RunScript(componentsPath, Path.Combine(fixture.Path, "bundle"), packageRoot);

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("Component id", result.StandardError, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("unsafe", result.StandardError, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Bundle_rejects_an_intermediate_package_directory_link_outside_the_cache()
    {
        using var fixture = new TemporaryDirectory();
        var packageRoot = fixture.CreateDirectory("packages");
        var externalRoot = fixture.CreateDirectory("external-packages");
        CreatePackage(
            externalRoot,
            "Example.Package",
            "1.2.3",
            "<license type=\"expression\">MIT</license>");
        var packageLink = Path.Combine(packageRoot, "example.package");
        CreateDirectoryLink(packageLink, Path.Combine(externalRoot, "example.package"));
        var componentsPath = WriteComponents(fixture.Path, ("Example.Package", "1.2.3"));

        ProcessResult result;
        try
        {
            result = RunScript(componentsPath, Path.Combine(fixture.Path, "bundle"), packageRoot);
        }
        finally
        {
            Directory.Delete(packageLink);
        }

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("reparse point", result.StandardError, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Runtime_and_host_are_the_only_components_allowed_without_embedded_license_evidence()
    {
        using var fixture = new TemporaryDirectory();
        var packageRoot = fixture.CreateDirectory("packages");
        const string version = "10.0.10";
        CreatePackage(packageRoot, "Microsoft.NETCore.App.Runtime.win-x64", version, string.Empty);
        var componentsPath = WriteComponents(
            fixture.Path,
            ("Microsoft.NETCore.App.Runtime.win-x64", version),
            ("Microsoft.NETCore.App.Host.win-x64", version));
        var output = Path.Combine(fixture.Path, "bundle");

        AssertSuccess(RunScript(componentsPath, output, packageRoot));

        using var manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(output, "manifest.json")));
        var components = manifest.RootElement.GetProperty("components").EnumerateArray().ToArray();
        Assert.All(components, component =>
        {
            Assert.True(component.GetProperty("externallyHandled").GetBoolean());
            Assert.Equal("dotnet-runtime-host-notices", component.GetProperty("externalHandler").GetString());
            Assert.Equal(JsonValueKind.Null, component.GetProperty("declaredLicense").ValueKind);
            Assert.Empty(component.GetProperty("evidence").EnumerateArray());
        });
        var host = Assert.Single(components, component =>
            component.GetProperty("id").GetString() == "Microsoft.NETCore.App.Host.win-x64");
        Assert.Equal(JsonValueKind.Null, host.GetProperty("nuspec").ValueKind);
    }

    [Fact]
    public void Release_validator_accepts_the_exact_bundle_and_rejects_component_identity_tampering()
    {
        using var fixture = new TemporaryDirectory();
        var packageRoot = fixture.CreateDirectory("packages");
        CreatePackage(
            packageRoot,
            "Example.Package",
            "1.2.3",
            "<license type=\"expression\">MIT</license>",
            ("LICENSE.txt", "MIT license"));
        var publishedDirectory = fixture.CreateDirectory("published");
        WritePublishedDependencies(publishedDirectory);
        var packageDirectory = fixture.CreateDirectory("release-package");
        var licensesDirectory = Directory.CreateDirectory(Path.Combine(packageDirectory, "licenses")).FullName;
        var output = Path.Combine(licensesDirectory, "nuget");

        AssertSuccess(RunPublishedScript(publishedDirectory, output, packageRoot));
        AssertSuccess(RunValidator(packageDirectory, publishedDirectory));

        var manifestPath = Path.Combine(output, "manifest.json");
        var manifest = JsonNode.Parse(File.ReadAllText(manifestPath))!.AsObject();
        var components = manifest["components"]!.AsArray();
        components[0]!["id"] = "Tampered.Package";
        File.WriteAllText(manifestPath, manifest.ToJsonString(), new UTF8Encoding(false));

        var tampered = RunValidator(packageDirectory, publishedDirectory);
        Assert.NotEqual(0, tampered.ExitCode);
        Assert.Contains("differ from the published dependency graph", tampered.StandardError, StringComparison.Ordinal);
    }

    private static void CreatePackage(
        string packageRoot,
        string id,
        string version,
        string licenseMetadata,
        params (string Path, string Content)[] evidenceFiles)
    {
        var directory = Path.Combine(packageRoot, id.ToLowerInvariant(), version.ToLowerInvariant());
        Directory.CreateDirectory(directory);
        var nuspec = $"""
            <?xml version="1.0" encoding="utf-8"?>
            <package xmlns="http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd">
              <metadata>
                <id>{id}</id>
                <version>{version}</version>
                <authors>Fixture Author</authors>
                <description>Fixture package.</description>
                {licenseMetadata}
              </metadata>
            </package>
            """;
        File.WriteAllText(Path.Combine(directory, id + ".nuspec"), nuspec, new UTF8Encoding(false));
        foreach (var (relativePath, content) in evidenceFiles)
        {
            var path = Path.Combine(directory, relativePath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, content, new UTF8Encoding(false));
        }
    }

    private static string WriteComponents(
        string directory,
        params (string Id, string Version)[] components)
    {
        var path = Path.Combine(directory, "components-" + Guid.NewGuid().ToString("N") + ".json");
        File.WriteAllText(
            path,
            JsonSerializer.Serialize(components.Select(component => new
            {
                component.Id,
                component.Version
            })),
            new UTF8Encoding(false));
        return path;
    }

    private static void WritePublishedDependencies(string publishedDirectory)
    {
        var document = new Dictionary<string, object>
        {
            ["runtimeTarget"] = new Dictionary<string, object>
            {
                ["name"] = ".NETCoreApp,Version=v10.0/win-x64"
            },
            ["libraries"] = new Dictionary<string, object>
            {
                ["Example.Package/1.2.3"] = new { type = "package" },
                ["runtimepack.Microsoft.NETCore.App.Runtime.win-x64/10.0.10"] =
                    new { type = "runtimepack" }
            }
        };
        File.WriteAllText(
            Path.Combine(publishedDirectory, "Abituria.deps.json"),
            JsonSerializer.Serialize(document),
            new UTF8Encoding(false));
    }

    private static void CreateDirectoryLink(string linkPath, string targetPath)
    {
        if (!OperatingSystem.IsWindows())
        {
            Directory.CreateSymbolicLink(linkPath, targetPath);
            return;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        startInfo.ArgumentList.Add("-ExecutionPolicy");
        startInfo.ArgumentList.Add("Bypass");
        startInfo.ArgumentList.Add("-Command");
        startInfo.ArgumentList.Add(
            "$ErrorActionPreference = 'Stop'; " +
            "New-Item -ItemType Junction -Path $env:ABITURIA_LINK -Target $env:ABITURIA_TARGET | Out-Null");
        startInfo.Environment["ABITURIA_LINK"] = linkPath;
        startInfo.Environment["ABITURIA_TARGET"] = targetPath;
        AssertSuccess(RunProcess(startInfo));
    }

    private static ProcessResult RunScript(string componentsPath, string output, string packageRoot)
    {
        var startInfo = CreatePowerShellStartInfo("tools/release/New-NuGetLicenseBundle.ps1");
        startInfo.ArgumentList.Add("-ComponentsPath");
        startInfo.ArgumentList.Add(componentsPath);
        startInfo.ArgumentList.Add("-OutputDirectory");
        startInfo.ArgumentList.Add(output);
        startInfo.ArgumentList.Add("-PackageRoot");
        startInfo.ArgumentList.Add(packageRoot);

        return RunProcess(startInfo);
    }

    private static ProcessResult RunPublishedScript(
        string publishedDirectory,
        string output,
        string packageRoot)
    {
        var startInfo = CreatePowerShellStartInfo("tools/release/New-NuGetLicenseBundle.ps1");
        startInfo.ArgumentList.Add("-PublishedDirectory");
        startInfo.ArgumentList.Add(publishedDirectory);
        startInfo.ArgumentList.Add("-RuntimeIdentifier");
        startInfo.ArgumentList.Add("win-x64");
        startInfo.ArgumentList.Add("-OutputDirectory");
        startInfo.ArgumentList.Add(output);
        startInfo.ArgumentList.Add("-PackageRoot");
        startInfo.ArgumentList.Add(packageRoot);

        return RunProcess(startInfo);
    }

    private static ProcessResult RunValidator(string packageDirectory, string publishedDirectory)
    {
        var startInfo = CreatePowerShellStartInfo("tools/release/Test-ReleaseAssets.ps1");
        startInfo.ArgumentList.Add("-LicensePackageDirectory");
        startInfo.ArgumentList.Add(packageDirectory);
        startInfo.ArgumentList.Add("-LicensePublishedPayloadDirectory");
        startInfo.ArgumentList.Add(publishedDirectory);
        startInfo.ArgumentList.Add("-LicenseRuntimeIdentifier");
        startInfo.ArgumentList.Add("win-x64");

        return RunProcess(startInfo);
    }

    private static ProcessStartInfo CreatePowerShellStartInfo(string scriptPath)
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
        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(Absolute(scriptPath));
        return startInfo;
    }

    private static ProcessResult RunProcess(ProcessStartInfo startInfo)
    {
        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("PowerShell did not start.");
        var standardOutput = process.StandardOutput.ReadToEndAsync();
        var standardError = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(30_000))
        {
            process.Kill(entireProcessTree: true);
            process.WaitForExit();
            throw new TimeoutException("NuGet license bundle script did not finish within 30 seconds.");
        }
        return new ProcessResult(
            process.ExitCode,
            standardOutput.GetAwaiter().GetResult(),
            standardError.GetAwaiter().GetResult());
    }

    private static void AssertSuccess(ProcessResult result)
    {
        Assert.True(
            result.ExitCode == 0,
            $"Script failed with exit code {result.ExitCode}.{Environment.NewLine}" +
            $"stdout: {result.StandardOutput}{Environment.NewLine}stderr: {result.StandardError}");
    }

    private static void AssertBundlesEqual(string first, string second)
    {
        var firstFiles = GetBundleFiles(first);
        var secondFiles = GetBundleFiles(second);
        Assert.Equal(firstFiles.Keys, secondFiles.Keys);
        foreach (var relativePath in firstFiles.Keys)
            Assert.Equal(firstFiles[relativePath], secondFiles[relativePath]);
    }

    private static SortedDictionary<string, byte[]> GetBundleFiles(string directory)
    {
        var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
            .ToDictionary(
                path => Path.GetRelativePath(directory, path).Replace(Path.DirectorySeparatorChar, '/'),
                File.ReadAllBytes,
                StringComparer.Ordinal);
        return new SortedDictionary<string, byte[]>(files, StringComparer.Ordinal);
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
                "abituria-nuget-license-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public string CreateDirectory(string name)
        {
            var path = System.IO.Path.Combine(Path, name);
            Directory.CreateDirectory(path);
            return path;
        }

        public void Dispose() => Directory.Delete(Path, recursive: true);
    }
}
