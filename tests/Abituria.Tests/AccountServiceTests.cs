using System.Text;
using Abituria.Data;
using Abituria.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace Abituria.Tests;

public sealed class AccountServiceTests : IAsyncLifetime
{
    private const string ValidPassword = "bardzo-dlugie-haslo-1";
    private static readonly string[] CompletedExerciseIds = ["mp21-z1"];

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
        const string nextPassword = "drugie-bardzo-dlugie-haslo";
        var registration = await _accounts.RegisterAsync("Uczeń", ValidPassword, ValidPassword);
        Assert.True(registration.Success);
        Assert.NotNull(registration.Profile);
        Assert.NotNull(registration.RecoveryCode);
        Assert.False((await _accounts.AuthenticateAsync(registration.Profile!.Id, "złe-hasło")).Success);
        Assert.True((await _accounts.AuthenticateAsync(registration.Profile.Id, ValidPassword)).Success);

        var recovery = await _accounts.RecoverPasswordAsync("Uczeń", registration.RecoveryCode!, nextPassword, nextPassword);
        Assert.True(recovery.Success);
        Assert.False((await _accounts.RecoverPasswordAsync("Uczeń", registration.RecoveryCode!, ValidPassword, ValidPassword)).Success);
        Assert.True((await _accounts.AuthenticateAsync(registration.Profile.Id, nextPassword)).Success);
    }

    [Fact]
    public async Task Registration_accepts_a_nonempty_password_without_a_minimum_length()
    {
        var registration = await _accounts.RegisterAsync("KrótkieHasło", "a", "a");
        var empty = await _accounts.RegisterAsync("PusteHasło", string.Empty, string.Empty);

        Assert.True(registration.Success, registration.Message);
        Assert.True((await _accounts.AuthenticateAsync(registration.Profile!.Id, "a")).Success);
        Assert.False(empty.Success);
        Assert.Equal("Hasło nie może być puste.", empty.Message);
    }

    [Fact]
    public async Task Change_password_rejects_invalid_current_password()
    {
        var registration = await _accounts.RegisterAsync("ZmianaHasla", ValidPassword, ValidPassword);

        var result = await _accounts.ChangePasswordAsync(
            registration.Profile!.Id,
            "niepoprawne-biezace-haslo",
            "nowe-bardzo-dlugie-haslo",
            "nowe-bardzo-dlugie-haslo");

        Assert.False(result.Success);
        Assert.Equal("Bieżące hasło jest nieprawidłowe.", result.Message);
        Assert.Null(result.RecoveryCode);
        Assert.True((await _accounts.AuthenticateAsync(registration.Profile.Id, ValidPassword)).Success);
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
        Assert.Equal(CompletedExerciseIds, await _accounts.GetCompletedExerciseIdsAsync(profile.Id));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("     ")]
    [InlineData("\t\r\n")]
    public async Task Registration_rejects_empty_or_whitespace_only_name(string name)
    {
        var result = await _accounts.RegisterAsync(name, ValidPassword, ValidPassword);

        Assert.False(result.Success);
        Assert.Equal(AccountService.DisplayNameValidationMessage, result.Message);
        Assert.Null(result.Profile);
        Assert.Single(await _accounts.GetProfilesAsync());
    }

    [Fact]
    public async Task Registration_accepts_exactly_thirty_name_characters()
    {
        var name = new string('a', 30);

        var result = await _accounts.RegisterAsync(name, ValidPassword, ValidPassword);

        Assert.True(result.Success, result.Message);
        Assert.Equal(name, result.Profile?.DisplayName);
    }

    [Fact]
    public async Task Registration_rejects_thirty_one_name_characters()
    {
        var name = new string('a', 31);

        var result = await _accounts.RegisterAsync(name, ValidPassword, ValidPassword);

        Assert.False(result.Success);
        Assert.Equal(AccountService.DisplayNameValidationMessage, result.Message);
        Assert.Null(result.Profile);
        Assert.Single(await _accounts.GetProfilesAsync());
    }

    [Fact]
    public async Task Empty_database_gets_one_idempotent_default_guest()
    {
        await _accounts.InitializeAsync();
        var profiles = await _accounts.GetProfilesAsync();
        var guest = Assert.Single(profiles, profile => profile.Kind == Models.ProfileKind.Guest);

        Assert.Equal("Maturzysta", guest.DisplayName);
        Assert.Equal(Models.CalculatorPipMode.OwnedWindow, guest.CalculatorPipMode);
    }

    [Fact]
    public async Task Calculator_pip_mode_is_validated_and_stored_per_profile()
    {
        var first = (await _accounts.RegisterAsync("PierwszyPip", ValidPassword, ValidPassword)).Profile!;
        var second = (await _accounts.RegisterAsync("DrugiPip", ValidPassword, ValidPassword)).Profile!;

        Assert.True(await _accounts.SetCalculatorPipModeAsync(first.Id, Models.CalculatorPipMode.AlwaysOnTopWindow));
        Assert.True(await _accounts.SetCalculatorPipModeAsync(second.Id, Models.CalculatorPipMode.InAppPanel));
        Assert.False(await _accounts.SetCalculatorPipModeAsync(Guid.NewGuid(), Models.CalculatorPipMode.OwnedWindow));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            _accounts.SetCalculatorPipModeAsync(first.Id, (Models.CalculatorPipMode)99));

        var profiles = await _accounts.GetProfilesAsync();
        Assert.Equal(
            Models.CalculatorPipMode.AlwaysOnTopWindow,
            profiles.Single(profile => profile.Id == first.Id).CalculatorPipMode);
        Assert.Equal(
            Models.CalculatorPipMode.InAppPanel,
            profiles.Single(profile => profile.Id == second.Id).CalculatorPipMode);

        await using (var context = _factory.CreateDbContext())
        {
            var stored = await context.Profiles.SingleAsync(
                profile => profile.Id == first.Id,
                TestContext.Current.CancellationToken);
            stored.CalculatorPipMode = (Models.CalculatorPipMode)99;
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var normalized = (await _accounts.GetProfilesAsync()).Single(profile => profile.Id == first.Id);
        Assert.Equal(Models.CalculatorPipMode.OwnedWindow, normalized.CalculatorPipMode);
        var authenticated = await _accounts.AuthenticateAsync(first.Id, ValidPassword);
        Assert.True(authenticated.Success);
        Assert.Equal(Models.CalculatorPipMode.OwnedWindow, authenticated.Profile?.CalculatorPipMode);
    }
}
