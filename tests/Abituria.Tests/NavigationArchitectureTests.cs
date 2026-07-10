using System.Xml.Linq;
using Abituria.Models;
using Abituria.ViewModels;
using Abituria.Views;
using Avalonia.Controls;

namespace Abituria.Tests;

public sealed class NavigationArchitectureTests
{
    [Fact]
    public void Every_live_view_is_an_avalonia_user_control_not_a_wpf_page()
    {
        var assembly = typeof(HomeView).Assembly;
        var viewTypes = assembly.GetTypes()
            .Where(type => type.Namespace == "Abituria.Views" && type.Name.EndsWith("View", StringComparison.Ordinal))
            .ToArray();

        Assert.NotEmpty(viewTypes);
        Assert.All(viewTypes, type => Assert.True(
            typeof(UserControl).IsAssignableFrom(type),
            $"{type.FullName} nie dziedziczy po Avalonia UserControl."));
        Assert.DoesNotContain(assembly.GetReferencedAssemblies(), reference =>
            reference.Name is "PresentationFramework" or "PresentationCore");
    }

    [Fact]
    public void Main_window_uses_one_unconstrained_shell_host_and_minimum_supported_size()
    {
        var root = FindRepositoryRoot();
        var document = XDocument.Load(Path.Combine(root, "AvaloniaApp", "MainWindow.axaml"));
        var window = Assert.IsType<XElement>(document.Root);
        XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";

        Assert.Equal("Window", window.Name.LocalName);
        Assert.Equal("960", window.Attribute("MinWidth")?.Value);
        Assert.Equal("640", window.Attribute("MinHeight")?.Value);
        var shellHosts = window.Descendants()
            .Where(element => element.Name.LocalName == "Border" && element.Attribute(x + "Name")?.Value == "ShellHost")
            .ToArray();
        Assert.Single(shellHosts);
        Assert.Null(shellHosts[0].Attribute("Width"));
        Assert.Null(shellHosts[0].Attribute("Height"));
        Assert.DoesNotContain(window.Descendants(), element =>
            element.Name.LocalName is "Page" or "Frame" or "NavigationWindow");
    }

    [Fact]
    public void Navigation_is_login_guarded_and_reaches_every_application_page()
    {
        var viewModel = new AppViewModel();
        foreach (var page in Enum.GetValues<AppPage>())
        {
            viewModel.Navigate(page);
            Assert.Equal(AppPage.Login, viewModel.CurrentPage);
        }

        var profile = new LocalProfile(Guid.NewGuid(), "Tester", ProfileKind.Guest);
        viewModel.Login(profile);
        Assert.Equal(AppPage.Home, viewModel.CurrentPage);

        foreach (var page in Enum.GetValues<AppPage>().Where(page => page != AppPage.Login))
        {
            viewModel.Navigate(page);
            Assert.Equal(page, viewModel.CurrentPage);
            Assert.Same(profile, viewModel.ActiveProfile);
        }

        viewModel.OpenGeneralCalculator();
        Assert.Equal(AppPage.GeneralCalculator, viewModel.CurrentPage);
        viewModel.Navigate(AppPage.Calculator);
        Assert.Equal(AppPage.Calculator, viewModel.CurrentPage);

        viewModel.Logout();
        Assert.Equal(AppPage.Login, viewModel.CurrentPage);
        Assert.Null(viewModel.ActiveProfile);
    }

    [Fact]
    public void Live_navigation_source_contains_no_legacy_wpf_page_or_navigation_window()
    {
        var root = FindRepositoryRoot();
        var files = Directory.EnumerateFiles(Path.Combine(root, "AvaloniaApp"), "*.*", SearchOption.AllDirectories)
            .Where(path => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) ||
                           path.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase));

        foreach (var file in files)
        {
            var source = File.ReadAllText(file);
            Assert.DoesNotContain("System.Windows.Controls.Page", source, StringComparison.Ordinal);
            Assert.DoesNotContain("NavigationWindow", source, StringComparison.Ordinal);
            Assert.DoesNotContain("<Page", source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Production_code_does_not_open_unbounded_non_modal_windows()
    {
        var root = FindRepositoryRoot();
        var files = Directory.EnumerateFiles(Path.Combine(root, "AvaloniaApp"), "*.cs", SearchOption.AllDirectories)
            .ToArray();
        var modalDialogFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Path.Combine(root, "AvaloniaApp", "Views", "LoginView.cs"),
            Path.Combine(root, "AvaloniaApp", "Views", "ProfileView.cs")
        };

        foreach (var file in files)
        {
            var source = File.ReadAllText(file);
            Assert.DoesNotContain(".Show(", source, StringComparison.Ordinal);
            if (modalDialogFiles.Contains(file))
            {
                Assert.Contains("ShowDialog(owner)", source, StringComparison.Ordinal);
                continue;
            }

            Assert.DoesNotContain("new Window", source, StringComparison.Ordinal);
        }
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Abituria.sln"))) return directory.FullName;
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Nie znaleziono katalogu repozytorium Abituria.");
    }
}
