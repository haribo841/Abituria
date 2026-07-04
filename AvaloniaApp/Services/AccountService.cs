using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abituria.Data;
using Abituria.Models;
using Microsoft.EntityFrameworkCore;

namespace Abituria.Services;

public sealed class AccountService(AppDbContextFactory contextFactory, PasswordHasher passwordHasher)
{
    public const int MinimumDisplayNameLength = 1;
    public const int MaximumDisplayNameLength = 30;
    public const string DisplayNameValidationMessage =
        "Nazwa użytkownika musi mieć od 1 do 30 znaków i nie może składać się wyłącznie ze spacji.";

    private const string LegacyImportKey = "legacy-users-imported-v1";

    public string DatabasePath => contextFactory.DatabasePath;

    public async Task InitializeAsync()
    {
        await using var context = contextFactory.CreateDbContext();
        await context.Database.MigrateAsync();
        await ImportLegacyProfilesAsync(context);
        await EnsureDefaultGuestAsync(context);
    }

    public async Task<IReadOnlyList<LocalProfile>> GetProfilesAsync()
    {
        await using var context = contextFactory.CreateDbContext();
        return await context.Profiles
            .AsNoTracking()
            .OrderBy(profile => profile.DisplayName)
            .Select(profile => new LocalProfile(profile.Id, profile.DisplayName, profile.Kind))
            .ToListAsync();
    }

    public async Task<RegistrationResult> RegisterAsync(string rawName, string password, string confirmation)
    {
        var name = rawName.Trim();
        if (name.Length is < MinimumDisplayNameLength or > MaximumDisplayNameLength)
            return new RegistrationResult(false, DisplayNameValidationMessage);
        if (!string.Equals(password, confirmation, StringComparison.Ordinal))
            return new RegistrationResult(false, "Hasła nie są takie same.");

        PasswordCredential credential;
        try { credential = passwordHasher.HashPassword(password); }
        catch (ArgumentException error) { return new RegistrationResult(false, error.Message); }

        await using var context = contextFactory.CreateDbContext();
        var normalizedName = NormalizeName(name);
        if (await context.Profiles.AnyAsync(profile => profile.NormalizedName == normalizedName))
            return new RegistrationResult(false, "Profil o tej nazwie już istnieje.");

        var recoveryCode = PasswordHasher.GenerateRecoveryCode();
        var entity = new LocalProfileEntity
        {
            Id = Guid.NewGuid(),
            DisplayName = name,
            NormalizedName = normalizedName,
            Kind = ProfileKind.Password,
            PasswordHash = credential.Hash,
            PasswordSalt = credential.Salt,
            PasswordIterations = credential.Iterations,
            RecoveryCodeHash = PasswordHasher.HashRecoveryCode(recoveryCode),
            CreatedUtc = DateTime.UtcNow
        };
        context.Profiles.Add(entity);
        await context.SaveChangesAsync();
        return new RegistrationResult(true, "Konto zostało utworzone. Zapisz kod odzyskiwania.", ToProfile(entity), recoveryCode);
    }

    public async Task<AuthenticationResult> AuthenticateAsync(Guid profileId, string password)
    {
        await using var context = contextFactory.CreateDbContext();
        var entity = await context.Profiles.SingleOrDefaultAsync(profile => profile.Id == profileId);
        if (entity is null || entity.Kind != ProfileKind.Password || entity.PasswordHash is null ||
            entity.PasswordSalt is null || entity.PasswordIterations is null ||
            !PasswordHasher.VerifyPassword(password, entity.PasswordHash, entity.PasswordSalt, entity.PasswordIterations.Value))
        {
            return new AuthenticationResult(false, "Nieprawidłowy profil lub hasło.");
        }

        entity.LastLoginUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return new AuthenticationResult(true, "Zalogowano.", ToProfile(entity));
    }

    public async Task<AuthenticationResult> LoginGuestAsync(Guid profileId)
    {
        await using var context = contextFactory.CreateDbContext();
        var entity = await context.Profiles.SingleOrDefaultAsync(profile => profile.Id == profileId && profile.Kind == ProfileKind.Guest);
        if (entity is null) return new AuthenticationResult(false, "Nie znaleziono profilu gościa.");
        entity.LastLoginUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return new AuthenticationResult(true, "Zalogowano jako gość.", ToProfile(entity));
    }

    public async Task<PasswordUpdateResult> RecoverPasswordAsync(string rawName, string recoveryCode, string newPassword, string confirmation)
    {
        if (!string.Equals(newPassword, confirmation, StringComparison.Ordinal))
            return new PasswordUpdateResult(false, "Hasła nie są takie same.");

        PasswordCredential credential;
        try { credential = passwordHasher.HashPassword(newPassword); }
        catch (ArgumentException error) { return new PasswordUpdateResult(false, error.Message); }

        await using var context = contextFactory.CreateDbContext();
        var normalizedName = NormalizeName(rawName);
        var entity = await context.Profiles.SingleOrDefaultAsync(profile => profile.NormalizedName == normalizedName);
        if (entity is null || entity.Kind != ProfileKind.Password || entity.RecoveryCodeHash is null ||
            !PasswordHasher.VerifyRecoveryCode(recoveryCode, entity.RecoveryCodeHash))
        {
            return new PasswordUpdateResult(false, "Nieprawidłowa nazwa konta lub kod odzyskiwania.");
        }

        var nextRecoveryCode = PasswordHasher.GenerateRecoveryCode();
        ApplyCredential(entity, credential, nextRecoveryCode);
        await context.SaveChangesAsync();
        return new PasswordUpdateResult(true, "Hasło zmieniono. Zapisz nowy kod odzyskiwania.", nextRecoveryCode);
    }

    public async Task<PasswordUpdateResult> ChangePasswordAsync(Guid profileId, string currentPassword, string newPassword, string confirmation)
    {
        if (!string.Equals(newPassword, confirmation, StringComparison.Ordinal))
            return new PasswordUpdateResult(false, "Nowe hasła nie są takie same.");

        await using var context = contextFactory.CreateDbContext();
        var entity = await context.Profiles.SingleOrDefaultAsync(profile => profile.Id == profileId);
        if (entity is null || entity.Kind != ProfileKind.Password || entity.PasswordHash is null || entity.PasswordSalt is null ||
            entity.PasswordIterations is null || !PasswordHasher.VerifyPassword(currentPassword, entity.PasswordHash, entity.PasswordSalt, entity.PasswordIterations.Value))
        {
            return new PasswordUpdateResult(false, "Bieżące hasło jest nieprawidłowe.");
        }

        PasswordCredential credential;
        try { credential = passwordHasher.HashPassword(newPassword); }
        catch (ArgumentException error) { return new PasswordUpdateResult(false, error.Message); }

        var recoveryCode = PasswordHasher.GenerateRecoveryCode();
        ApplyCredential(entity, credential, recoveryCode);
        await context.SaveChangesAsync();
        return new PasswordUpdateResult(true, "Hasło zmieniono. Zapisz nowy kod odzyskiwania.", recoveryCode);
    }

    public async Task MarkExerciseCompletedAsync(Guid profileId, string exerciseId)
    {
        await using var context = contextFactory.CreateDbContext();
        if (await context.ExerciseProgress.AnyAsync(item => item.ProfileId == profileId && item.ExerciseId == exerciseId)) return;
        context.ExerciseProgress.Add(new ExerciseProgressEntity
        {
            ProfileId = profileId,
            ExerciseId = exerciseId,
            CompletedUtc = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlySet<string>> GetCompletedExerciseIdsAsync(Guid profileId)
    {
        await using var context = contextFactory.CreateDbContext();
        var ids = await context.ExerciseProgress.AsNoTracking()
            .Where(item => item.ProfileId == profileId)
            .Select(item => item.ExerciseId)
            .ToListAsync();
        return ids.ToHashSet(StringComparer.Ordinal);
    }

    private static void ApplyCredential(LocalProfileEntity entity, PasswordCredential credential, string recoveryCode)
    {
        entity.PasswordHash = credential.Hash;
        entity.PasswordSalt = credential.Salt;
        entity.PasswordIterations = credential.Iterations;
        entity.RecoveryCodeHash = PasswordHasher.HashRecoveryCode(recoveryCode);
    }

    private static LocalProfile ToProfile(LocalProfileEntity entity) => new(entity.Id, entity.DisplayName, entity.Kind);
    private static string NormalizeName(string value) => value.Trim().ToUpperInvariant();

    private static async Task ImportLegacyProfilesAsync(AppDbContext context)
    {
        if (await context.Metadata.AnyAsync(item => item.Key == LegacyImportKey)) return;

        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Abituria", "users.txt");
        if (File.Exists(filePath))
        {
            var names = (await File.ReadAllLinesAsync(filePath))
                .Select(name => name.Trim())
                .Where(name => name.Length is > 0 and <= 30)
                .Distinct(StringComparer.OrdinalIgnoreCase);
            foreach (var name in names)
            {
                var normalized = NormalizeName(name);
                if (await context.Profiles.AnyAsync(profile => profile.NormalizedName == normalized)) continue;
                context.Profiles.Add(new LocalProfileEntity
                {
                    Id = Guid.NewGuid(),
                    DisplayName = name,
                    NormalizedName = normalized,
                    Kind = ProfileKind.Guest,
                    CreatedUtc = DateTime.UtcNow
                });
            }
        }

        context.Metadata.Add(new AppMetadataEntity { Key = LegacyImportKey, Value = DateTime.UtcNow.ToString("O") });
        await context.SaveChangesAsync();
    }

    private static async Task EnsureDefaultGuestAsync(AppDbContext context)
    {
        if (await context.Profiles.AnyAsync()) return;

        context.Profiles.Add(new LocalProfileEntity
        {
            Id = Guid.NewGuid(),
            DisplayName = "Maturzysta",
            NormalizedName = NormalizeName("Maturzysta"),
            Kind = ProfileKind.Guest,
            CreatedUtc = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }
}
