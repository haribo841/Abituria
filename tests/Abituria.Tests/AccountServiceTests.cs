using System.Text;
using Abituria.Data;
using Abituria.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace Abituria.Tests;

public sealed class AccountServiceTests : IAsyncLifetime
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
    private AccountService _accounts = null!;
    private AppDbContextFactory _factory = null!;

    public async ValueTask InitializeAsync()
    {
        Directory.CreateDirectory(_directory);
        _factory = new AppDbContextFactory(Path.Combine(_directory, "test.db"));
        _accounts = new AccountService(_factory, new PasswordHasher(1_000));
        await _accounts.InitializeAsync();
    }

    public ValueTask DisposeAsync()
    {
        SqliteConnection.ClearAllPools();
        if (Directory.Exists(_directory)) Directory.Delete(_directory, true);
        return ValueTask.CompletedTask;
    }

    [Fact]
    public async Task Registration_authentication_and_recovery_are_functional()
    {
        const string firstPassword = "bardzo-dlugie-haslo-1";
        const string nextPassword = "drugie-bardzo-dlugie-haslo";
        var registration = await _accounts.RegisterAsync("Uczeń", firstPassword, firstPassword);
        Assert.True(registration.Success);
        Assert.NotNull(registration.Profile);
        Assert.NotNull(registration.RecoveryCode);
        Assert.False((await _accounts.AuthenticateAsync(registration.Profile!.Id, "złe-hasło")).Success);
        Assert.True((await _accounts.AuthenticateAsync(registration.Profile.Id, firstPassword)).Success);

        var recovery = await _accounts.RecoverPasswordAsync("Uczeń", registration.RecoveryCode!, nextPassword, nextPassword);
        Assert.True(recovery.Success);
        Assert.False((await _accounts.RecoverPasswordAsync("Uczeń", registration.RecoveryCode!, firstPassword, firstPassword)).Success);
        Assert.True((await _accounts.AuthenticateAsync(registration.Profile.Id, nextPassword)).Success);
    }

    [Fact]
    public async Task Passwords_use_unique_salts_and_are_not_stored_as_plain_text()
    {
        const string password = "jednakowe-bardzo-dlugie-haslo";
        await _accounts.RegisterAsync("Pierwszy", password, password);
        await _accounts.RegisterAsync("Drugi", password, password);
        await using var context = _factory.CreateDbContext();
        var profiles = await context.Profiles
            .Where(item => item.Kind == Models.ProfileKind.Password)
            .ToListAsync(TestContext.Current.CancellationToken);
        Assert.Equal(2, profiles.Count);
        Assert.NotEqual(profiles[0].PasswordSalt, profiles[1].PasswordSalt);
        Assert.NotEqual(profiles[0].PasswordHash, profiles[1].PasswordHash);
        Assert.All(profiles, profile => Assert.NotEqual(Encoding.UTF8.GetBytes(password), profile.PasswordHash));
    }

    [Fact]
    public async Task Exercise_progress_is_idempotent()
    {
        const string password = "haslo-do-testu-postepu";
        var profile = (await _accounts.RegisterAsync("Postęp", password, password)).Profile!;
        await _accounts.MarkExerciseCompletedAsync(profile.Id, "mp21-z1");
        await _accounts.MarkExerciseCompletedAsync(profile.Id, "mp21-z1");
        Assert.Equal(new[] { "mp21-z1" }, await _accounts.GetCompletedExerciseIdsAsync(profile.Id));
    }

    [Fact]
    public async Task Empty_database_gets_one_idempotent_default_guest()
    {
        await _accounts.InitializeAsync();
        var profiles = await _accounts.GetProfilesAsync();
        var guest = Assert.Single(profiles, profile => profile.Kind == Models.ProfileKind.Guest);

        Assert.Equal("Maturzysta", guest.DisplayName);
    }
}
