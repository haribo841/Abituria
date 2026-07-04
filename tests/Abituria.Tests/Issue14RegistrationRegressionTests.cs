using Abituria.Data;
using Abituria.Models;
using Abituria.Services;
using Abituria.Views;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Microsoft.Data.Sqlite;

namespace Abituria.Tests;

public sealed class Issue14RegistrationRegressionTests
{
    [AvaloniaFact]
    public async Task Failed_registration_keeps_login_available_and_explains_all_limits()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var databasePath = Path.Combine(directory, "issue14.db");
        var accounts = new AccountService(new AppDbContextFactory(databasePath), new PasswordHasher(1_000));
        await accounts.InitializeAsync();
        var copy = new ContentRepository().UiCopy;
        LocalProfile? loggedInProfile = null;
        var view = new LoginView(accounts, copy, profile => loggedInProfile = profile);
        var originalContent = view.Content;
        var window = new Window { Width = 960, Height = 640, Content = view };
        window.Show();

        try
        {
            var controls = view.GetLogicalDescendants().ToArray();
            var profiles = controls.OfType<ComboBox>().Single(control => control.Name == "ProfileSelector");
            var login = controls.OfType<Button>().Single(control => control.Name == "LoginButton");
            var register = controls.OfType<Button>().Single(control => control.Name == "RegisterButton");
            var name = controls.OfType<TextBox>().Single(control => control.Name == "RegistrationNameBox");
            var status = controls.OfType<TextBlock>().Single(control => control.Name == "AccountStatusText");
            var rules = copy.GetRequired("account.registration.rules");

            await WaitUntilAsync(() => profiles.SelectedItem is not null);
            Assert.Contains(controls.OfType<TextBlock>(), control => control.Text == rules.Title);
            Assert.Contains(controls.OfType<TextBlock>(), control => control.Text == rules.Body);
            Assert.Contains(AccountService.MinimumDisplayNameLength.ToString(), rules.Body, StringComparison.Ordinal);
            Assert.Contains(AccountService.MaximumDisplayNameLength.ToString(), rules.Body, StringComparison.Ordinal);
            Assert.Contains(PasswordHasher.MinimumPasswordLength.ToString(), rules.Body, StringComparison.Ordinal);
            Assert.Contains(PasswordHasher.MaximumPasswordLength.ToString(), rules.Body, StringComparison.Ordinal);
            Assert.Equal(1, AccountService.MinimumDisplayNameLength);
            Assert.Equal(30, AccountService.MaximumDisplayNameLength);
            Assert.Equal(30, name.MaxLength);

            name.Text = "   ";
            register.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            Assert.Same(originalContent, view.Content);
            Assert.Contains(login, view.GetLogicalDescendants().OfType<Button>());
            Assert.True(login.IsVisible);
            Assert.True(login.IsEnabled);
            Assert.Equal(AccountService.DisplayNameValidationMessage, status.Text);
            Assert.Null(loggedInProfile);

            login.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            await WaitUntilAsync(() => loggedInProfile is not null);
            Assert.Equal("Maturzysta", loggedInProfile!.DisplayName);
            Assert.Equal(ProfileKind.Guest, loggedInProfile.Kind);
        }
        finally
        {
            window.Close();
            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            Dispatcher.UIThread.RunJobs();
            if (condition()) return;
            await Task.Delay(10);
        }

        Assert.Fail("Warunek interfejsu nie został spełniony w wyznaczonym czasie.");
    }
}
