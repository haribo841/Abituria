using Abituria.Data;
using Abituria.Models;
using Abituria.Services;
using Abituria.ViewModels;
using Abituria.Views;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace Abituria.Tests;

public sealed class AboutViewTests
{
    private static readonly AppBuildInfo TestBuildInfo = new(
        "0.9.0-beta.1",
        "2957b43e7c9b04f1c0042fa887290832b5d82ad8",
        "MIT",
        "Adam Kubiś",
        "https://github.com/haribo841/Abituria");

    [AvaloniaFact]
    public void About_view_renders_exact_release_and_project_information_at_minimum_size()
    {
        var view = new AboutView(TestBuildInfo);
        var window = new Window { Width = 960, Height = 640, Content = view };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var text = view.GetLogicalDescendants()
                .OfType<TextBlock>()
                .Select(control => control.Text)
                .Where(value => value is not null)
                .ToArray();

            Assert.Contains("O programie", text);
            Assert.Contains(TestBuildInfo.Version, text);
            Assert.Contains(TestBuildInfo.Commit, text);
            Assert.Contains(TestBuildInfo.License, text);
            Assert.Contains(TestBuildInfo.Author, text);
            Assert.Contains(TestBuildInfo.RepositoryUrl, text);
            Assert.True(view.Bounds.Width > 0);
            Assert.True(view.Bounds.Height > 0);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task Main_navigation_opens_about_page_and_marks_its_button_as_active()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var accounts = new AccountService(
            new AppDbContextFactory(Path.Combine(directory, "about-routing.db")),
            new PasswordHasher(1_000));
        await accounts.InitializeAsync(importLegacyProfiles: false);
        var profile = (await accounts.GetProfilesAsync()).Single(item => item.Kind == ProfileKind.Guest);
        var viewModel = new AppViewModel();
        var content = new ContentRepository();
        var session = new CalculatorSession(new ExpressionCalculator());
        var window = new MainWindow(viewModel, accounts, content, session, TestBuildInfo);
        window.Width = 960;
        window.Height = 640;

        try
        {
            window.Show();
            viewModel.Login(profile);
            Dispatcher.UIThread.RunJobs();

            var aboutButton = window.GetLogicalDescendants()
                .OfType<Button>()
                .Single(button => string.Equals(button.Content as string, "O programie", StringComparison.Ordinal));
            aboutButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(AppPage.About, viewModel.CurrentPage);
            Assert.Single(window.GetLogicalDescendants().OfType<AboutView>());
            aboutButton = window.GetLogicalDescendants()
                .OfType<Button>()
                .Single(button => string.Equals(button.Content as string, "O programie", StringComparison.Ordinal));
            Assert.Contains("primary", aboutButton.Classes);
        }
        finally
        {
            window.Close();
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }
}
