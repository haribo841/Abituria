using System.Security.Cryptography;
using Abituria.Data;
using Abituria.Models;
using Abituria.Services;
using Microsoft.Data.Sqlite;

namespace Abituria.Tests;

public sealed class ReleaseDatabaseCompatibilityTests
{
    private const string FixtureRelativePath =
        "tests/Abituria.Tests/Fixtures/net9-efcore-9.0.17.db";
    private const string FixtureSha256 =
        "AA006B46A3E7384EB87A92DD4164B9364D50CF78C873322A870CC5582923E1ED";
    private const string HistoricalPassword = "historyczne-haslo-testowe";
    private const string HistoricalMigration = "202606270001_InitialLocalAccounts";
    private const string HistoricalProfileName = "Uczeń z .NET 9";

    [Fact]
    public async Task Real_ef9_database_keeps_login_and_progress_after_net10_upgrade_and_restart()
    {
        var fixturePath = Absolute(FixtureRelativePath);
        Assert.Equal(
            FixtureSha256,
            Convert.ToHexString(SHA256.HashData(await File.ReadAllBytesAsync(
                fixturePath,
                TestContext.Current.CancellationToken))));
        Assert.Equal("9.0.17", await ReadMigrationProductVersionAsync(fixturePath));
        Assert.Equal(PasswordHasher.DefaultIterations, await ReadPasswordIterationsAsync(fixturePath));

        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var databasePath = Path.Combine(directory, "net9-existing.db");
        Directory.CreateDirectory(directory);
        File.Copy(fixturePath, databasePath);

        try
        {
            var hasher = new PasswordHasher();
            var firstRun = new AccountService(new AppDbContextFactory(databasePath), hasher);
            await firstRun.InitializeAsync(importLegacyProfiles: false);
            var profile = Assert.Single(await firstRun.GetProfilesAsync());
            Assert.Equal(HistoricalProfileName, profile.DisplayName);
            Assert.Equal(ProfileKind.Password, profile.Kind);
            Assert.True((await firstRun.AuthenticateAsync(profile.Id, HistoricalPassword)).Success);
            Assert.Equal(["mp21-z7"], await firstRun.GetCompletedExerciseIdsAsync(profile.Id));
            await firstRun.MarkExerciseCompletedAsync(profile.Id, "mp21-z35");

            SqliteConnection.ClearAllPools();
            var restarted = new AccountService(new AppDbContextFactory(databasePath), hasher);
            await restarted.InitializeAsync(importLegacyProfiles: false);

            Assert.True((await restarted.AuthenticateAsync(profile.Id, HistoricalPassword)).Success);
            Assert.Equal(
                ["mp21-z35", "mp21-z7"],
                (await restarted.GetCompletedExerciseIdsAsync(profile.Id)).Order(StringComparer.Ordinal));
            Assert.Equal("9.0.17", await ReadMigrationProductVersionAsync(databasePath));
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory)) Directory.Delete(directory, recursive: true);
        }
    }

    private static async Task<string> ReadMigrationProductVersionAsync(string databasePath)
    {
        await using var connection = new SqliteConnection(ConnectionString(databasePath));
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT \"ProductVersion\" FROM \"__EFMigrationsHistory\" WHERE \"MigrationId\" = $id;";
        command.Parameters.AddWithValue("$id", HistoricalMigration);
        return (string)(await command.ExecuteScalarAsync(TestContext.Current.CancellationToken)
            ?? throw new InvalidDataException("Brak historycznego wpisu migracji."));
    }

    private static async Task<long> ReadPasswordIterationsAsync(string databasePath)
    {
        await using var connection = new SqliteConnection(ConnectionString(databasePath));
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT \"PasswordIterations\" FROM \"Profiles\" WHERE \"DisplayName\" = $name;";
        command.Parameters.AddWithValue("$name", HistoricalProfileName);
        return (long)(await command.ExecuteScalarAsync(TestContext.Current.CancellationToken)
            ?? throw new InvalidDataException("Historyczny profil nie zawiera liczby iteracji hasła."));
    }

    private static string ConnectionString(string databasePath) =>
        new SqliteConnectionStringBuilder { DataSource = databasePath }.ToString();

    private static string Absolute(string relativePath) =>
        Path.Combine(FindRepositoryRoot(), relativePath.Replace('/', Path.DirectorySeparatorChar));

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Abituria.sln")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Nie znaleziono repozytorium Abituria.");
    }
}
