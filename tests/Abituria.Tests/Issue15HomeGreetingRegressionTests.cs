using Abituria.Data;
using Abituria.Models;
using Abituria.Services;
using Abituria.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Abituria.Tests;

public sealed class Issue15HomeGreetingRegressionTests
{
    [AvaloniaFact]
    public void Logged_in_profile_name_is_rendered_in_home_greeting_and_top_bar()
    {
        const string profileName = "Alicja Kowalska";
        const string expectedGreeting = "Cześć, Alicja Kowalska. Wybierz obszar nauki.";
        var databasePath = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"), "issue15.db");
        var viewModel = new AppViewModel();
        var accounts = new AccountService(new AppDbContextFactory(databasePath), new PasswordHasher(1_000));
        var content = new ContentRepository();
        var calculatorSession = new CalculatorSession(new ExpressionCalculator());
        var profile = new LocalProfile(Guid.NewGuid(), profileName, ProfileKind.Password);
        var window = new MainWindow(viewModel, accounts, content, calculatorSession);

        try
        {
            viewModel.Login(profile);
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var textBlocks = window.GetVisualDescendants().OfType<TextBlock>().ToArray();
            var greeting = Assert.Single(textBlocks, control => control.Text == expectedGreeting);

            Assert.Equal(AppPage.Home, viewModel.CurrentPage);
            Assert.Same(profile, viewModel.ActiveProfile);
            Assert.True(greeting.IsVisible);
            Assert.InRange(greeting.Bounds.Width, 1d, window.Bounds.Width);
            Assert.InRange(greeting.Bounds.Height, 1d, window.Bounds.Height);
            Assert.NotNull(greeting.TranslatePoint(default, window));
            Assert.Contains(textBlocks, control => control.Text == profileName);
            Assert.DoesNotContain(textBlocks, control => control.Text == "Cześć, . Wybierz obszar nauki.");
        }
        finally
        {
            window.Close();
        }
    }
}
