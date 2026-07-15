using Abituria.Data;
using Abituria.Models;
using Abituria.Services;
using Microsoft.Data.Sqlite;

namespace Abituria.Tests;

public sealed class ReleaseDatabaseCompatibilityTests
{
    private const string HistoricalPassword = "historyczne-haslo-testowe";
    private const string HistoricalMigration = "202606270001_InitialLocalAccounts";

    [Fact]
    public async Task Net9_database_keeps_login_and_progress_after_net10_upgrade_and_restart()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var databasePath = Path.Combine(directory, "net9-existing.db");
        var profileId = Guid.NewGuid();
        var hasher = new PasswordHasher(1_000);
        var credential = hasher.HashPassword(HistoricalPassword);
        Directory.CreateDirectory(directory);

        try
        {
            await CreateNet9DatabaseAsync(databasePath, profileId, credential);

            var firstRun = new AccountService(new AppDbContextFactory(databasePath), hasher);
            await firstRun.InitializeAsync(importLegacyProfiles: false);
            var profile = Assert.Single(await firstRun.GetProfilesAsync());
            Assert.Equal(profileId, profile.Id);
            Assert.Equal("Uczeń z .NET 9", profile.DisplayName);
            Assert.Equal(ProfileKind.Password, profile.Kind);
            Assert.True((await firstRun.AuthenticateAsync(profileId, HistoricalPassword)).Success);
            Assert.Equal(["mp21-z7"], await firstRun.GetCompletedExerciseIdsAsync(profileId));
            await firstRun.MarkExerciseCompletedAsync(profileId, "mp21-z35");

            SqliteConnection.ClearAllPools();
            var restarted = new AccountService(new AppDbContextFactory(databasePath), hasher);
            await restarted.InitializeAsync(importLegacyProfiles: false);

            Assert.True((await restarted.AuthenticateAsync(profileId, HistoricalPassword)).Success);
            Assert.Equal(
                ["mp21-z35", "mp21-z7"],
                (await restarted.GetCompletedExerciseIdsAsync(profileId)).Order(StringComparer.Ordinal));
            Assert.Equal("9.0.17", await ReadMigrationProductVersionAsync(databasePath));
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory)) Directory.Delete(directory, recursive: true);
        }
    }

    private static async Task CreateNet9DatabaseAsync(
        string databasePath,
        Guid profileId,
        PasswordCredential credential)
    {
        await using var connection = new SqliteConnection($"Data Source={databasePath}");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        await using var schema = connection.CreateCommand();
        schema.CommandText = """
            PRAGMA foreign_keys = ON;
            CREATE TABLE "__EFMigrationsHistory" (
                "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
                "ProductVersion" TEXT NOT NULL
            );
            CREATE TABLE "AppMetadata" (
                "Key" TEXT NOT NULL CONSTRAINT "PK_AppMetadata" PRIMARY KEY,
                "Value" TEXT NOT NULL
            );
            CREATE TABLE "Profiles" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_Profiles" PRIMARY KEY,
                "DisplayName" TEXT NOT NULL,
                "NormalizedName" TEXT NOT NULL,
                "Kind" INTEGER NOT NULL,
                "PasswordHash" BLOB NULL,
                "PasswordSalt" BLOB NULL,
                "PasswordIterations" INTEGER NULL,
                "RecoveryCodeHash" BLOB NULL,
                "CreatedUtc" TEXT NOT NULL,
                "LastLoginUtc" TEXT NULL
            );
            CREATE TABLE "ExerciseProgress" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_ExerciseProgress" PRIMARY KEY AUTOINCREMENT,
                "ProfileId" TEXT NOT NULL,
                "ExerciseId" TEXT NOT NULL,
                "CompletedUtc" TEXT NOT NULL,
                CONSTRAINT "FK_ExerciseProgress_Profiles_ProfileId"
                    FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
            );
            CREATE UNIQUE INDEX "IX_Profiles_NormalizedName" ON "Profiles" ("NormalizedName");
            CREATE UNIQUE INDEX "IX_ExerciseProgress_ProfileId_ExerciseId"
                ON "ExerciseProgress" ("ProfileId", "ExerciseId");
            INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                VALUES ('202606270001_InitialLocalAccounts', '9.0.17');
            """;
        await schema.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);

        await using var insertProfile = connection.CreateCommand();
        insertProfile.CommandText = """
            INSERT INTO "Profiles"
                ("Id", "DisplayName", "NormalizedName", "Kind", "PasswordHash", "PasswordSalt",
                 "PasswordIterations", "RecoveryCodeHash", "CreatedUtc", "LastLoginUtc")
            VALUES
                ($id, 'Uczeń z .NET 9', 'UCZEŃ Z .NET 9', 1, $hash, $salt, $iterations, NULL, $created, NULL);
            """;
        insertProfile.Parameters.AddWithValue("$id", profileId);
        insertProfile.Parameters.AddWithValue("$hash", credential.Hash);
        insertProfile.Parameters.AddWithValue("$salt", credential.Salt);
        insertProfile.Parameters.AddWithValue("$iterations", credential.Iterations);
        insertProfile.Parameters.AddWithValue("$created", new DateTime(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc));
        await insertProfile.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);

        await using var insertProgress = connection.CreateCommand();
        insertProgress.CommandText = """
            INSERT INTO "ExerciseProgress" ("ProfileId", "ExerciseId", "CompletedUtc")
            VALUES ($profileId, 'mp21-z7', $completed);
            """;
        insertProgress.Parameters.AddWithValue("$profileId", profileId);
        insertProgress.Parameters.AddWithValue("$completed", new DateTime(2026, 7, 1, 12, 30, 0, DateTimeKind.Utc));
        await insertProgress.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);
    }

    private static async Task<string> ReadMigrationProductVersionAsync(string databasePath)
    {
        await using var connection = new SqliteConnection($"Data Source={databasePath}");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT \"ProductVersion\" FROM \"__EFMigrationsHistory\" WHERE \"MigrationId\" = $id;";
        command.Parameters.AddWithValue("$id", HistoricalMigration);
        return (string)(await command.ExecuteScalarAsync(TestContext.Current.CancellationToken)
            ?? throw new InvalidDataException("Brak historycznego wpisu migracji."));
    }
}
