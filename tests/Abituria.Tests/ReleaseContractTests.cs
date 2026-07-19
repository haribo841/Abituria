using System.Text.Json;
using System.Xml.Linq;

namespace Abituria.Tests;

public sealed class ReleaseContractTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void Canonical_version_is_consistent_across_assembly_and_release_documents()
    {
        var properties = XDocument.Load(Absolute("Directory.Build.props"));
        var versionElement = properties.Descendants("Version").Single();
        var version = versionElement.Value;
        var expectedTag = "v" + version;
        var requiredDocuments = new[]
        {
            "README.md",
            "CHANGELOG.md",
            "docs/INSTALLATION.md",
            "docs/RELEASE_PROCESS.md",
            "docs/KNOWN_LIMITATIONS.md",
            "docs/BUSINESS_ANALYSIS.md",
            "docs/REQUIREMENTS.md"
        };

        Assert.Equal("0.9.0-beta.1", version);
        Assert.Empty(properties.Descendants("VersionPrefix"));
        Assert.Empty(properties.Descendants("VersionSuffix"));
        Assert.Equal(version, Services.AppBuildInfo.Current.Version);
        Assert.All(requiredDocuments, path =>
            Assert.Contains(version, File.ReadAllText(Absolute(path)), StringComparison.Ordinal));
        Assert.Contains(expectedTag, File.ReadAllText(Absolute("README.md")), StringComparison.Ordinal);
        Assert.Contains(expectedTag, File.ReadAllText(Absolute(".github/workflows/release.yml")), StringComparison.Ordinal);
        Assert.Contains(
            $"Abituria-v{version}-<rid>",
            File.ReadAllText(Absolute("README.md")),
            StringComparison.Ordinal);
    }

    [Fact]
    public void Release_configuration_pins_sdk_dependencies_audit_and_tools()
    {
        using var globalJson = JsonDocument.Parse(File.ReadAllText(Absolute("global.json")));
        using var tools = JsonDocument.Parse(File.ReadAllText(Absolute(".config/dotnet-tools.json")));
        var project = XDocument.Load(Absolute("Abituria.csproj"));
        var testProject = XDocument.Load(Absolute("tests/Abituria.Tests/Abituria.Tests.csproj"));
        var properties = XDocument.Load(Absolute("Directory.Build.props"));
        var dependencyDocumentation = File.ReadAllText(Absolute("docs/DEPENDENCIES.md"));
        var packages = project.Descendants("PackageReference")
            .ToDictionary(
                node => node.Attribute("Include")!.Value,
                node => node.Attribute("Version")!.Value,
                StringComparer.Ordinal);

        Assert.Equal("10.0.302", globalJson.RootElement.GetProperty("sdk").GetProperty("version").GetString());
        Assert.Equal("disable", globalJson.RootElement.GetProperty("sdk").GetProperty("rollForward").GetString());
        Assert.Equal("net10.0", project.Descendants("TargetFramework").Single().Value);
        Assert.Equal("net10.0", testProject.Descendants("TargetFramework").Single().Value);
        Assert.Equal("10.0.10", properties.Descendants("RuntimeFrameworkVersion").Single().Value);
        Assert.Equal("10.0.10", packages["Microsoft.EntityFrameworkCore.Sqlite"]);
        Assert.Equal("10.0.10", packages["Microsoft.Extensions.DependencyInjection"]);
        Assert.Equal("2.1.12", packages["SQLitePCLRaw.bundle_e_sqlite3"]);
        Assert.DoesNotContain("Avalonia.BuildServices", packages.Keys);
        Assert.DoesNotContain(project.Descendants(), node => node.Name.LocalName == "NuGetAudit" && node.Value == "false");
        Assert.Equal("true", properties.Descendants("RestorePackagesWithLockFile").Single().Value);
        Assert.Equal("all", properties.Descendants("NuGetAuditMode").Single().Value);
        Assert.Equal("low", properties.Descendants("NuGetAuditLevel").Single().Value);
        Assert.Contains("NU1901;NU1902;NU1903;NU1904", properties.Descendants("WarningsAsErrors").Single().Value, StringComparison.Ordinal);
        Assert.Equal(
            "2.78.5",
            tools.RootElement.GetProperty("tools").GetProperty("docfx").GetProperty("version").GetString());
        Assert.Equal(
            "4.1.5",
            tools.RootElement.GetProperty("tools").GetProperty("microsoft.sbom.dotnettool").GetProperty("version").GetString());
        Assert.Contains("| `docfx` | `2.78.5` |", dependencyDocumentation, StringComparison.Ordinal);
        Assert.Contains("| `Microsoft.Sbom.DotNetTool` | `4.1.5` |", dependencyDocumentation, StringComparison.Ordinal);
        Assert.Contains("| `dotnet-sonarscanner` | `11.2.1` |", dependencyDocumentation, StringComparison.Ordinal);
        Assert.Contains("| LGPL-3.0 |", dependencyDocumentation, StringComparison.Ordinal);
        Assert.True(File.Exists(Absolute("packages.lock.json")));
        Assert.True(File.Exists(Absolute("tests/Abituria.Tests/packages.lock.json")));
    }

    [Fact]
    public void Release_workflow_uses_native_runners_and_required_supply_chain_gates()
    {
        var workflow = File.ReadAllText(Absolute(".github/workflows/release.yml"));
        var draftJob = workflow[workflow.IndexOf("\n  draft-release:", StringComparison.Ordinal)..];
        var requiredFragments = new[]
        {
            "windows-2025",
            "ubuntu-24.04",
            "macos-15-intel",
            "--locked-mode",
            "Test-NuGetVulnerabilities.ps1",
            "Test-ContentProvenance.ps1 -RequireReleaseEligible",
            "Generate-DependencyDocumentation.ps1 -Verify",
            "dotnet format",
            "git diff --check",
            "actions/attest@v4",
            "concurrency:",
            "cancel-in-progress: false",
            "refs/remotes/origin/main",
            "Restore solution in locked mode for dependency documentation",
            "--draft",
            "--prerelease"
        };

        Assert.All(requiredFragments, fragment => Assert.Contains(fragment, workflow, StringComparison.Ordinal));
        Assert.True(
            workflow.IndexOf(
                "Restore solution in locked mode for dependency documentation",
                StringComparison.Ordinal) <
            workflow.IndexOf(
                "Verify generated dependency and license documentation",
                StringComparison.Ordinal));
        Assert.Contains("sonar.qualitygate.wait=true", workflow, StringComparison.Ordinal);
        Assert.Contains(
            "xvfb-run -a dotnet test Abituria.sln --configuration Release --no-build --no-restore",
            workflow,
            StringComparison.Ordinal);
        Assert.Contains("artifact-metadata: write", draftJob, StringComparison.Ordinal);
        Assert.Contains("if (-not $existingRelease.isDraft)", draftJob, StringComparison.Ordinal);
        Assert.Contains("nie może zostać nadpisane ani cofnięte do draftu", draftJob, StringComparison.Ordinal);
        Assert.Equal(7, CountOccurrences(draftJob, "uses: actions/attest@v4"));
        Assert.Equal(3, CountOccurrences(draftJob, "sbom-path:"));
        Assert.Equal(2, CountOccurrences(draftJob, "subject-path: artifacts/release/Abituria-v${{ needs.preflight.outputs.version }}-win-x64.zip"));
        Assert.Equal(2, CountOccurrences(draftJob, "subject-path: artifacts/release/Abituria-v${{ needs.preflight.outputs.version }}-linux-x64.tar.gz"));
        Assert.Equal(2, CountOccurrences(draftJob, "subject-path: artifacts/release/Abituria-v${{ needs.preflight.outputs.version }}-osx-x64.zip"));
        Assert.Contains("Attest Windows archive build provenance", draftJob, StringComparison.Ordinal);
        Assert.Contains("Attest Linux archive build provenance", draftJob, StringComparison.Ordinal);
        Assert.Contains("Attest macOS archive build provenance", draftJob, StringComparison.Ordinal);
    }

    [Fact]
    public void Pages_build_uses_pinned_docfx_and_does_not_publish_image_directory()
    {
        var workflow = File.ReadAllText(Absolute(".github/workflows/pages.yml"))
            .Replace("\r\n", "\n", StringComparison.Ordinal);
        var buildJob = workflow[..workflow.IndexOf("\n  deploy:", StringComparison.Ordinal)];
        var docfx = File.ReadAllText(Absolute("docfx.json"));

        Assert.Contains("dotnet tool run docfx", workflow, StringComparison.Ordinal);
        Assert.Contains("--warningsAsErrors", workflow, StringComparison.Ordinal);
        Assert.Contains("actions/deploy-pages@v4", workflow, StringComparison.Ordinal);
        Assert.Contains(
            "    permissions:\n      contents: read\n      pages: write\n      id-token: write",
            buildJob,
            StringComparison.Ordinal);
        Assert.Contains("actions/configure-pages@v5", buildJob, StringComparison.Ordinal);
        Assert.Contains("https://haribo841.github.io/Abituria/", docfx, StringComparison.Ordinal);
        Assert.DoesNotContain("\"resource\"", docfx, StringComparison.Ordinal);
        Assert.DoesNotContain("img", docfx, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Every_archive_uses_the_short_platform_specific_start_guide()
    {
        var guide = File.ReadAllText(Absolute("tools/release/RELEASE-README.md"));
        var packagingScript = File.ReadAllText(Absolute("tools/release/Publish-ReleaseArtifact.ps1"));
        var installation = File.ReadAllText(Absolute("docs/INSTALLATION.md"));
        var readme = File.ReadAllText(Absolute("README.md"));

        Assert.InRange(guide.Length, 500, 4_000);
        Assert.Contains("Windows 11 x64", guide, StringComparison.Ordinal);
        Assert.Contains("Ubuntu 24.04 x64", guide, StringComparison.Ordinal);
        Assert.Contains("macOS 15 Intel x64", guide, StringComparison.Ordinal);
        Assert.Contains("SHA-256", guide, StringComparison.Ordinal);
        Assert.Contains("Abituria-v0.9.0-beta.1-win-x64", guide, StringComparison.Ordinal);
        Assert.Contains("Abituria-v0.9.0-beta.1-linux-x64", guide, StringComparison.Ordinal);
        Assert.Contains("Abituria-v0.9.0-beta.1-osx-x64", guide, StringComparison.Ordinal);
        Assert.Contains("tools/release/RELEASE-README.md", packagingScript, StringComparison.Ordinal);
        Assert.Contains("dotnet-runtime-LICENSE.txt", packagingScript, StringComparison.Ordinal);
        Assert.Contains("dotnet-runtime-THIRD-PARTY-NOTICES.txt", packagingScript, StringComparison.Ordinal);
        Assert.Contains("New-NuGetLicenseBundle.ps1", packagingScript, StringComparison.Ordinal);
        Assert.Contains("licenses/nuget", guide, StringComparison.Ordinal);
        Assert.Contains("Start-Process", installation, StringComparison.Ordinal);
        Assert.Contains("-Wait", installation, StringComparison.Ordinal);
        Assert.Contains("$process.ExitCode", installation, StringComparison.Ordinal);
        Assert.Contains("Start-Process", readme, StringComparison.Ordinal);
        Assert.Contains("-Wait -PassThru", readme, StringComparison.Ordinal);
        Assert.DoesNotContain("docs/INSTALLATION.md", packagingScript, StringComparison.Ordinal);
    }

    [Fact]
    public void Native_release_jobs_smoke_test_the_extracted_final_archive()
    {
        var workflow = File.ReadAllText(Absolute(".github/workflows/release.yml"));
        var smokeScript = File.ReadAllText(Absolute("tools/release/Invoke-ReleaseSmokeTest.ps1"));

        Assert.Contains("Smoke-test final archive after extraction", workflow, StringComparison.Ordinal);
        Assert.Contains("-ArchivePath $archivePath", workflow, StringComparison.Ordinal);
        Assert.DoesNotContain("-PublishedDirectory", workflow, StringComparison.Ordinal);
        Assert.Contains("Abituria.app/Contents/MacOS/Abituria", smokeScript, StringComparison.Ordinal);
        Assert.Contains("Abituria-v$Version-$RuntimeIdentifier", smokeScript, StringComparison.Ordinal);
        Assert.Contains("Assert-SafeArchiveEntry", smokeScript, StringComparison.Ordinal);
        Assert.Contains("TimeoutSeconds = 120", smokeScript, StringComparison.Ordinal);
        Assert.Contains("$process.Kill($true)", smokeScript, StringComparison.Ordinal);
        Assert.Contains("ABITURIA_RELEASE_SMOKE", smokeScript, StringComparison.Ordinal);
        Assert.Contains("exactly one top-level directory", smokeScript, StringComparison.Ordinal);
    }

    [Fact]
    public void Release_identity_is_bound_to_main_tag_and_never_allows_unknown_commit()
    {
        var versionValidator = File.ReadAllText(Absolute("tools/release/Test-ReleaseVersion.ps1"));
        var publisher = File.ReadAllText(Absolute("tools/release/Publish-ReleaseArtifact.ps1"));
        var assetValidator = File.ReadAllText(Absolute("tools/release/Test-ReleaseAssets.ps1"));

        Assert.Contains("refs/remotes/origin/main", versionValidator, StringComparison.Ordinal);
        Assert.Contains("TryParseExact", versionValidator, StringComparison.Ordinal);
        Assert.Contains("taggerdate:unix", versionValidator, StringComparison.Ordinal);
        Assert.Contains("FromUnixTimeSeconds", versionValidator, StringComparison.Ordinal);
        Assert.Contains("musi być adnotowany", versionValidator, StringComparison.Ordinal);
        Assert.Contains("przedwydaniowy komunikat", versionValidator, StringComparison.Ordinal);
        Assert.Contains("^[0-9a-f]{40}$", publisher, StringComparison.Ordinal);
        Assert.DoesNotContain("commit = \"unknown\"", publisher, StringComparison.Ordinal);
        Assert.DoesNotContain("|unknown", assetValidator, StringComparison.Ordinal);
        Assert.Contains("$releaseMetadata.commit -cne $expectedCommit", assetValidator, StringComparison.Ordinal);
        Assert.Contains("exactly one top-level directory", assetValidator, StringComparison.Ordinal);
        Assert.DoesNotContain("--sequesterRsrc", publisher, StringComparison.Ordinal);
    }

    [Fact]
    public void Documentation_link_policy_is_explicit_retried_and_non_flaky_in_ci()
    {
        using var policy = JsonDocument.Parse(File.ReadAllText(
            Absolute("tools/release/external-links-policy.json")));
        var root = policy.RootElement;
        var script = File.ReadAllText(Absolute("tools/release/Test-DocumentationSite.ps1"));
        var pages = File.ReadAllText(Absolute(".github/workflows/pages.yml"));

        Assert.Equal(1, root.GetProperty("schemaVersion").GetInt32());
        Assert.Equal(3, root.GetProperty("retryCount").GetInt32());
        Assert.Equal(
            "https://github.com/haribo841/Abituria/blob/main/",
            root.GetProperty("localRepositoryBlobPrefix").GetString());
        Assert.Contains(
            root.GetProperty("allowedHosts").EnumerateArray(),
            item => item.GetString() == "github.com");
        Assert.Contains(
            root.GetProperty("allowedHosts").EnumerateArray(),
            item => item.GetString() == "www.youtube.com");
        Assert.All(
            root.GetProperty("onlineCheckExclusions").EnumerateArray(),
            item =>
            {
                Assert.StartsWith("https://", item.GetProperty("urlPrefix").GetString(), StringComparison.Ordinal);
                Assert.False(string.IsNullOrWhiteSpace(item.GetProperty("reason").GetString()));
            });
        Assert.Contains("-CheckExternalLinks", pages, StringComparison.Ordinal);
        Assert.Contains("-ExternalLinkFailureAction Fail", pages, StringComparison.Ordinal);
        Assert.Contains("steps.deployment.outputs.page_url", pages, StringComparison.Ordinal);
        Assert.Contains("Deployed Pages entry point failed after 6 attempts", pages, StringComparison.Ordinal);
        Assert.Contains("Start-Sleep", script, StringComparison.Ordinal);
        Assert.Contains("External link must use HTTPS", script, StringComparison.Ordinal);
        Assert.Contains("RequirePublishedReleaseLinks", script, StringComparison.Ordinal);
        Assert.Contains(
            "-RequirePublishedReleaseLinks",
            File.ReadAllText(Absolute("docs/RELEASE_PROCESS.md")),
            StringComparison.Ordinal);
    }

    [Fact]
    public void Release_asset_validation_scans_secrets_in_every_file_type()
    {
        var validator = File.ReadAllText(Absolute("tools/release/Test-ReleaseAssets.ps1"));
        var scanner = File.ReadAllText(Absolute("tools/release/PackageSecurity.psm1"));

        Assert.Contains("Test-PackagedSecrets -PackageDirectory $packageDirectory", validator, StringComparison.Ordinal);
        Assert.Contains("[IO.File]::Open", scanner, StringComparison.Ordinal);
        Assert.Contains("AWS access key", scanner, StringComparison.Ordinal);
        Assert.Contains("JWT", scanner, StringComparison.Ordinal);
        Assert.Contains("connection string credential", scanner, StringComparison.Ordinal);
        Assert.Contains("[Text.Encoding]::Unicode", scanner, StringComparison.Ordinal);
    }

    private static string Absolute(string relativePath) =>
        Path.Combine(RepositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

    private static int CountOccurrences(string value, string fragment) =>
        (value.Length - value.Replace(fragment, string.Empty, StringComparison.Ordinal).Length) / fragment.Length;

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Abituria.sln")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Nie znaleziono repozytorium Abituria.");
    }
}
