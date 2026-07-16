using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Abituria.Data;
using Abituria.Models;
using Abituria.Services;
using Avalonia.Headless.XUnit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Abituria.Tests;

public sealed class ReleaseRuntimeTests
{
    [Fact]
    public void Build_info_exposes_one_release_version_and_project_identity()
    {
        var buildInfo = AppBuildInfo.FromAssembly(
            typeof(AppBuildInfo).Assembly,
            "0123456789abcdef0123456789abcdef01234567");

        Assert.Equal("0.9.0-beta.1", buildInfo.Version);
        Assert.Equal("0123456789abcdef0123456789abcdef01234567", buildInfo.Commit);
        Assert.Equal("MIT", buildInfo.License);
        Assert.Equal("Adam Kubiś", buildInfo.Author);
        Assert.Equal("https://github.com/haribo841/Abituria", buildInfo.RepositoryUrl);
    }

    [Fact]
    public void Build_info_extracts_release_version_and_commit_from_informational_version()
    {
        const string commit = "2957b43e7c9b04f1c0042fa887290832b5d82ad8";
        var assembly = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName("Abituria.BuildInfo.Test"),
            AssemblyBuilderAccess.Run);
        var constructor = typeof(AssemblyInformationalVersionAttribute)
            .GetConstructor([typeof(string)])!;
        assembly.SetCustomAttribute(new CustomAttributeBuilder(
            constructor,
            [$"0.9.0-beta.1+{commit}"]));

        var buildInfo = AppBuildInfo.FromAssembly(assembly);

        Assert.Equal("0.9.0-beta.1", buildInfo.Version);
        Assert.Equal(commit, buildInfo.Commit);
    }

    [Fact]
    public void Release_smoke_command_accepts_only_the_documented_syntax()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", "smoke-parser");
        var valid = new[]
        {
            ReleaseSmokeTestCommand.CommandSwitch,
            ReleaseSmokeTestCommand.DataDirectorySwitch,
            directory
        };

        Assert.True(ReleaseSmokeTestCommand.IsRequested(valid));
        Assert.True(ReleaseSmokeTestCommand.TryParse(valid, out var parsed, out var error));
        Assert.Equal(Path.GetFullPath(directory), parsed!.DataDirectory);
        Assert.Empty(error);

        Assert.False(ReleaseSmokeTestCommand.TryParse(
            [ReleaseSmokeTestCommand.CommandSwitch],
            out _,
            out var missingDirectoryError));
        Assert.Contains(ReleaseSmokeTestCommand.DataDirectorySwitch, missingDirectoryError, StringComparison.Ordinal);

        Assert.False(ReleaseSmokeTestCommand.TryParse(
            [ReleaseSmokeTestCommand.CommandSwitch, "--unknown", directory],
            out _,
            out _));
        Assert.False(ReleaseSmokeTestCommand.TryParse(
            [ReleaseSmokeTestCommand.CommandSwitch, ReleaseSmokeTestCommand.DataDirectorySwitch, " "],
            out _,
            out _));
        Assert.False(ReleaseSmokeTestCommand.TryParse(
            [ReleaseSmokeTestCommand.CommandSwitch, ReleaseSmokeTestCommand.DataDirectorySwitch, "bad\0path"],
            out _,
            out var invalidPathError));
        Assert.Contains("nieprawidłową ścieżkę", invalidPathError, StringComparison.Ordinal);
    }

    [Fact]
    public void Release_smoke_options_reject_production_data_and_existing_smoke_database()
    {
        var productionDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Abituria");
        var directory = NewTemporaryDirectory();

        try
        {
            var productionError = Assert.Throws<ArgumentException>(() =>
                AppRuntimeOptions.ReleaseSmokeTest(productionDataDirectory));
            Assert.Contains("produkcyjnym katalogiem danych", productionError.Message, StringComparison.Ordinal);

            File.WriteAllText(Path.Combine(directory, AppRuntimeOptions.ReleaseSmokeDatabaseFileName), "occupied");
            var existingDatabaseError = Assert.Throws<ArgumentException>(() =>
                AppRuntimeOptions.ReleaseSmokeTest(directory));
            Assert.Contains("nowego pustego katalogu", existingDatabaseError.Message, StringComparison.Ordinal);
        }
        finally
        {
            DisposeSqliteAndDirectory(directory);
        }
    }

    [Fact]
    public void Release_smoke_options_reject_case_variant_of_production_data_on_case_insensitive_systems()
    {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsMacOS()) return;

        var productionDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Abituria");
        var caseVariant = Path.Combine(
            Path.GetDirectoryName(productionDataDirectory)!,
            "aBITURIA");

        var error = Assert.Throws<ArgumentException>(() => AppRuntimeOptions.ReleaseSmokeTest(caseVariant));
        Assert.Contains("produkcyjnym katalogiem danych", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Release_smoke_options_reject_symlink_alias_of_production_data_parent()
    {
        if (OperatingSystem.IsWindows()) return;

        var directory = NewTemporaryDirectory();
        var alias = Path.Combine(directory, "local-data-alias");
        var localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        Directory.CreateSymbolicLink(alias, localData);

        try
        {
            var candidate = Path.Combine(alias, "Abituria", "release-smoke");
            var error = Assert.Throws<ArgumentException>(() => AppRuntimeOptions.ReleaseSmokeTest(candidate));
            Assert.Contains("produkcyjnym katalogiem danych", error.Message, StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(alias);
            DisposeSqliteAndDirectory(directory);
        }
    }

    [Fact]
    public async Task Release_smoke_entrypoint_rejects_production_subdirectory_without_creating_it()
    {
        var productionDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Abituria");
        var candidate = Path.Combine(productionDataDirectory, "release-smoke-" + Guid.NewGuid().ToString("N"));
        Assert.False(Directory.Exists(candidate));

        var exitCode = await ReleaseSmokeTestCommand.ExecuteAsync([
            ReleaseSmokeTestCommand.CommandSwitch,
            ReleaseSmokeTestCommand.DataDirectorySwitch,
            candidate
        ]);

        Assert.Equal(ReleaseSmokeTestCommand.FailureExitCode, exitCode);
        Assert.False(Directory.Exists(candidate));
    }

    [Fact]
    public async Task Database_factory_treats_semicolon_as_part_of_the_file_path()
    {
        var directory = NewTemporaryDirectory();
        var databasePath = Path.Combine(directory, "name;with-semicolon.db");

        try
        {
            var accounts = new AccountService(new AppDbContextFactory(databasePath), new PasswordHasher(1_000));
            await accounts.InitializeAsync(importLegacyProfiles: false);

            Assert.True(File.Exists(databasePath));
            Assert.Single(await accounts.GetProfilesAsync());
        }
        finally
        {
            DisposeSqliteAndDirectory(directory);
        }
    }

    [AvaloniaFact]
    public async Task Release_smoke_runner_uses_an_isolated_database_and_checks_real_resources()
    {
        var directory = NewTemporaryDirectory();
        var options = AppRuntimeOptions.ReleaseSmokeTest(directory);
        await using var services = new ServiceCollection()
            .AddAbituriaServices(options)
            .BuildServiceProvider();

        try
        {
            await services.InitializeAbituriaAsync();
            var report = await ReleaseSmokeTestRunner.VerifyAsync(services);

            Assert.False(options.ImportLegacyProfiles);
            Assert.False(options.ShowMainWindow);
            Assert.True(options.IsReleaseSmokeTest);
            Assert.Equal(
                Path.Combine(Path.GetFullPath(directory), AppRuntimeOptions.ReleaseSmokeDatabaseFileName),
                report.DatabasePath);
            Assert.True(File.Exists(report.DatabasePath));
            Assert.Equal("0.9.0-beta.1", report.Version);
            Assert.Equal(AppBuildInfo.Current.Commit, report.Commit);
            Assert.True(report.FormulaCount > 0);
            Assert.True(report.ChapterCount > 0);
            Assert.True(report.ExerciseCount > 0);
            Assert.Equal("Dwa miejsca zerowe: x₁ = 1, x₂ = 2", report.QuadraticSummary);

            var profiles = await services.GetRequiredService<AccountService>().GetProfilesAsync();
            Assert.Equal(2, profiles.Count);
            Assert.Contains(profiles, profile => profile.DisplayName == "Maturzysta" && profile.Kind == ProfileKind.Guest);
            Assert.Contains(
                profiles,
                profile => profile.DisplayName == "Release smoke profile" && profile.Kind == ProfileKind.Password);
        }
        finally
        {
            DisposeSqliteAndDirectory(directory);
        }
    }

    [Fact]
    public async Task Release_smoke_entrypoint_finishes_without_opening_ui_and_uses_requested_directory()
    {
        var directory = NewTemporaryDirectory();
        var assemblyPath = typeof(App).Assembly.Location;
        var dotnetHost = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet";
        var startInfo = new ProcessStartInfo(dotnetHost)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };
        startInfo.ArgumentList.Add(assemblyPath);
        startInfo.ArgumentList.Add(ReleaseSmokeTestCommand.CommandSwitch);
        startInfo.ArgumentList.Add(ReleaseSmokeTestCommand.DataDirectorySwitch);
        startInfo.ArgumentList.Add(directory);

        try
        {
            using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Nie uruchomiono procesu smoke testu.");
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            await process.WaitForExitAsync(timeout.Token);
            var output = await process.StandardOutput.ReadToEndAsync(timeout.Token);
            var error = await process.StandardError.ReadToEndAsync(timeout.Token);

            Assert.True(
                process.ExitCode == ReleaseSmokeTestCommand.SuccessExitCode,
                $"Kod procesu: {process.ExitCode}. Standard output: {output}. Standard error: {error}");
            Assert.Contains("smoke test zakończony powodzeniem", output, StringComparison.Ordinal);
            Assert.Contains("ABITURIA_RELEASE_SMOKE version=0.9.0-beta.1 commit=", output, StringComparison.Ordinal);
            Assert.Empty(error);

            var databasePath = Path.Combine(directory, AppRuntimeOptions.ReleaseSmokeDatabaseFileName);
            Assert.True(File.Exists(databasePath));
            await using var context = new AppDbContextFactory(databasePath).CreateDbContext();
            var profiles = await context.Profiles
                .AsNoTracking()
                .ToListAsync(TestContext.Current.CancellationToken);
            Assert.Equal(2, profiles.Count);
            Assert.Contains(profiles, profile => profile.DisplayName == "Maturzysta" && profile.Kind == ProfileKind.Guest);
            Assert.Contains(
                profiles,
                profile => profile.DisplayName == "Release smoke profile" && profile.Kind == ProfileKind.Password);
        }
        finally
        {
            DisposeSqliteAndDirectory(directory);
        }
    }

    private static string NewTemporaryDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        return directory;
    }

    private static void DisposeSqliteAndDirectory(string directory)
    {
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        if (Directory.Exists(directory)) Directory.Delete(directory, true);
    }
}
