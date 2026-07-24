using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml.Linq;
using Abituria.Data;
using Abituria.Models;
using Abituria.Services;
using Abituria.Ui;
using Abituria.ViewModels;
using Abituria.Views;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.Data.Sqlite;

namespace Abituria.Tests;

public sealed class Discussion49StyleRegressionTests
{
    [Fact]
    public void Custom_chrome_font_theme_and_interaction_contract_is_present_in_sources()
    {
        var root = FindRepositoryRoot();
        var mainWindowPath = Path.Combine(root, "AvaloniaApp", "MainWindow.axaml");
        var mainWindowSourcePath = Path.Combine(root, "AvaloniaApp", "MainWindow.axaml.cs");
        var appPath = Path.Combine(root, "AvaloniaApp", "App.axaml");
        var stylesPath = Path.Combine(root, "AvaloniaApp", "Styles", "AppStyles.axaml");
        var programPath = Path.Combine(root, "AvaloniaApp", "Program.cs");
        var projectPath = Path.Combine(root, "Abituria.csproj");

        var document = XDocument.Load(mainWindowPath);
        var window = Assert.IsType<XElement>(document.Root);
        XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";
        Assert.Equal("None", window.Attribute("WindowDecorations")?.Value);
        Assert.Null(window.Attribute("SystemDecorations"));
        Assert.Equal("True", window.Attribute("ExtendClientAreaToDecorationsHint")?.Value);
        Assert.Equal("720", window.Attribute("MinWidth")?.Value);
        Assert.Equal("520", window.Attribute("MinHeight")?.Value);
        Assert.Single(window.Descendants(), element => element.Attribute(x + "Name")?.Value == "TitleBarDragArea");
        Assert.Single(window.Descendants(), element => element.Attribute(x + "Name")?.Value == "ThemeButton");
        Assert.Single(window.Descendants(), element => element.Attribute(x + "Name")?.Value == "MinimizeButton");
        Assert.Single(window.Descendants(), element => element.Attribute(x + "Name")?.Value == "MaximizeButton");
        Assert.Single(window.Descendants(), element => element.Attribute(x + "Name")?.Value == "CloseButton");
        Assert.Equal(
            8,
            window.Descendants().Count(element =>
                element.Attribute("PointerPressed")?.Value.StartsWith("Resize", StringComparison.Ordinal) == true));

        var mainWindowSource = File.ReadAllText(mainWindowSourcePath);
        Assert.Contains("BeginMoveDrag(e)", mainWindowSource, StringComparison.Ordinal);
        Assert.Contains("BeginResizeDrag(edge, e)", mainWindowSource, StringComparison.Ordinal);
        Assert.Contains("WindowState.Minimized", mainWindowSource, StringComparison.Ordinal);
        Assert.Contains("WindowState.Maximized", mainWindowSource, StringComparison.Ordinal);

        var appSource = File.ReadAllText(appPath);
        var styleSource = File.ReadAllText(stylesPath);
        var programSource = File.ReadAllText(programPath);
        var projectSource = File.ReadAllText(projectPath);
        Assert.DoesNotContain("RequestedThemeVariant=\"Light\"", appSource, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("fonts#Mulish", styleSource, StringComparison.Ordinal);
        Assert.Contains("PlaceholderForeground", styleSource, StringComparison.Ordinal);
        Assert.Contains("TextControlPlaceholderOpacity", styleSource, StringComparison.Ordinal);
        Assert.Contains("BrandImageSurfaceBrush", styleSource, StringComparison.Ordinal);
        Assert.DoesNotContain("Avalonia.Fonts.Inter", projectSource, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("WithInterFont", programSource, StringComparison.Ordinal);
        Assert.Contains("DefaultThreadCurrentUICulture", programSource, StringComparison.Ordinal);
        Assert.Contains("pl-PL", programSource, StringComparison.Ordinal);

        foreach (var selector in new[]
                 {
                     "Button:pointerover",
                     "Button:pressed",
                     "Button:focus",
                     "Button:focus-visible",
                     "Button.primary:pointerover",
                     "Button.primary:pressed",
                     "Button.primary:focus",
                     "Button.primary:focus-visible",
                     "Button.home-tile:pointerover",
                     "Button.home-tile:pressed",
                     "Button.home-tile:focus",
                     "Button.home-tile:focus-visible",
                     "TextBox:pointerover",
                     "TextBox:pressed",
                     "TextBox:focus",
                     "TextBox:focus-visible",
                     "ComboBox:pointerover",
                     "ComboBox:pressed",
                     "ComboBox:focus",
                     "ComboBox:focus-visible"
                 })
        {
            Assert.Contains(selector, styleSource, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Every_palette_meets_the_declared_text_focus_and_non_text_contrast_budgets()
    {
        var palettes = ReadPalettes();
        foreach (var (name, palette) in palettes)
        {
            AssertContrast(name, palette, "TextPrimaryBrush", "SurfaceBrush", 4.5d);
            AssertContrast(name, palette, "TextMutedBrush", "SurfaceBrush", 4.5d);
            AssertContrast(name, palette, "AccentTextBrush", "AccentBrush", 4.5d);
            AssertContrast(name, palette, "AccentTextBrush", "AccentHoverBrush", 4.5d);
            AssertContrast(name, palette, "AccentTextBrush", "AccentPressedBrush", 4.5d);
            AssertContrast(name, palette, "SuccessBrush", "SurfaceBrush", 4.5d);
            AssertContrast(name, palette, "ErrorBrush", "SurfaceBrush", 4.5d);
            AssertContrast(name, palette, "WarningBrush", "SurfaceBrush", 4.5d);
            AssertContrast(name, palette, "BorderBrush", "SurfaceBrush", 3d);
            AssertContrast(name, palette, "FocusBrush", "SurfaceBrush", 3d);
            AssertContrast(name, palette, "DangerTextBrush", "DangerBrush", 4.5d);
            AssertContrast(name, palette, "DangerTextBrush", "DangerHoverBrush", 4.5d);
            AssertContrast(name, palette, "DangerTextBrush", "DangerPressedBrush", 4.5d);
        }
    }

    [AvaloniaFact]
    public void Theme_manager_switches_light_dark_high_contrast_and_system_resources_at_runtime()
    {
        var application = Assert.IsType<TestApplication>(Application.Current);
        using var manager = new AppThemeManager(application);

        try
        {
            manager.SetMode(AppThemeMode.Light);
            Assert.Same(ThemeVariant.Light, application.RequestedThemeVariant);
            Assert.Equal(ReadPalettes()["Light"]["AppBackgroundBrush"], CurrentColor(application, "AppBackgroundBrush", ThemeVariant.Light));

            manager.SetMode(AppThemeMode.Dark);
            Assert.Same(ThemeVariant.Dark, application.RequestedThemeVariant);
            Assert.Equal(ReadPalettes()["Dark"]["AppBackgroundBrush"], CurrentColor(application, "AppBackgroundBrush", ThemeVariant.Dark));

            manager.SetMode(AppThemeMode.HighContrast);
            Assert.Same(ThemeVariant.Dark, application.RequestedThemeVariant);
            Assert.Equal(ReadPalettes()["HighContrast"]["AppBackgroundBrush"], CurrentColor(application, "AppBackgroundBrush", ThemeVariant.Dark));

            manager.SetMode(AppThemeMode.System);
            Assert.Same(ThemeVariant.Default, application.RequestedThemeVariant);
            Assert.DoesNotContain("AppBackgroundBrush", application.Resources.Keys.Cast<object>());
        }
        finally
        {
            manager.SetMode(AppThemeMode.System);
        }
    }

    [AvaloniaFact]
    public void Keyboard_focus_is_visible_in_properties_and_in_the_rendered_frame()
    {
        var application = Assert.IsType<TestApplication>(Application.Current);
        using var manager = new AppThemeManager(application);
        manager.SetMode(AppThemeMode.Light);
        var first = new Button { Content = "Pierwszy", Classes = { "primary" }, Width = 150 };
        var focused = new Button { Content = "Drugi", Classes = { "home-tile" }, Width = 150 };
        var window = ShowInWindow(
            new StackPanel { Margin = new Thickness(30), Spacing = 20, Children = { first, focused } },
            420,
            240);

        try
        {
            using var before = Assert.IsType<WriteableBitmap>(window.CaptureRenderedFrame());
            Assert.True(focused.Focus(NavigationMethod.Tab));
            Dispatcher.UIThread.RunJobs();
            using var after = Assert.IsType<WriteableBitmap>(window.CaptureRenderedFrame());

            Assert.True(focused.IsKeyboardFocusWithin);
            Assert.Equal(new Thickness(3), focused.BorderThickness);
            var actualFocus = Assert.IsType<ISolidColorBrush>(focused.BorderBrush, exactMatch: false).Color;
            Assert.Equal(CurrentColor(application, "FocusBrush", ThemeVariant.Light), actualFocus);
            Assert.NotEqual(HashBitmap(before), HashBitmap(after));
        }
        finally
        {
            window.Close();
            manager.SetMode(AppThemeMode.System);
        }
    }

    [AvaloniaFact]
    public void The_same_view_renders_visibly_different_light_dark_and_high_contrast_frames()
    {
        var application = Assert.IsType<TestApplication>(Application.Current);
        using var manager = new AppThemeManager(application);
        var content = new ContentRepository();
        var view = new HomeView("Tester", content.UiCopy, () => { }, () => { }, () => { }, () => { }, () => { });
        var window = ShowInWindow(view, 960, 640);

        try
        {
            var hashes = new HashSet<string>(StringComparer.Ordinal);
            foreach (var mode in new[] { AppThemeMode.Light, AppThemeMode.Dark, AppThemeMode.HighContrast })
            {
                manager.SetMode(mode);
                Dispatcher.UIThread.RunJobs();
                using var frame = Assert.IsType<WriteableBitmap>(window.CaptureRenderedFrame());
                hashes.Add(HashBitmap(frame));
            }

            Assert.Equal(3, hashes.Count);
        }
        finally
        {
            window.Close();
            manager.SetMode(AppThemeMode.System);
        }
    }

    [AvaloniaFact]
    public async Task Login_home_and_calculator_switch_between_wide_and_compact_layouts()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var accounts = new AccountService(
            new AppDbContextFactory(Path.Combine(directory, "discussion49-responsive.db")),
            new PasswordHasher(1_000));
        await accounts.InitializeAsync(importLegacyProfiles: false);
        var content = new ContentRepository();

        try
        {
            var login = new LoginView(accounts, content.UiCopy, _ => { });
            AssertResponsiveColumns(login, "LoginLayoutRoot", 1100, 720, 2, 1);

            var home = new HomeView("Tester", content.UiCopy, () => { }, () => { }, () => { }, () => { }, () => { });
            AssertResponsiveColumns(home, "HomeLayoutRoot", 1100, 720, 2, 1);

            var calculator = new GeneralCalculatorView(
                new CalculatorSession(new ExpressionCalculator()),
                content.UiCopy,
                () => { });
            AssertResponsiveColumns(calculator, "CalculatorLayoutRoot", 1100, 820, 2, 1);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory)) Directory.Delete(directory, recursive: true);
        }
    }

    [AvaloniaFact]
    public async Task Login_logo_and_placeholders_remain_readable_in_dark_and_high_contrast_modes()
    {
        var application = Assert.IsType<TestApplication>(Application.Current);
        using var manager = new AppThemeManager(application);
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var accounts = new AccountService(
            new AppDbContextFactory(Path.Combine(directory, "discussion49-login-contrast.db")),
            new PasswordHasher(1_000));
        await accounts.InitializeAsync(importLegacyProfiles: false);
        var view = new LoginView(accounts, new ContentRepository().UiCopy, _ => { });
        var window = ShowInWindow(view, 960, 640);

        try
        {
            var nameBox = view.GetLogicalDescendants()
                .OfType<TextBox>()
                .Single(textBox => textBox.Name == "RegistrationNameBox");
            var logoSurface = view.GetLogicalDescendants()
                .OfType<Border>()
                .Single(border => border.Name == "LoginBrandLogoSurface");
            var placeholder = nameBox.GetVisualDescendants()
                .OfType<TextBlock>()
                .Single(textBlock => textBlock.Name == "PART_Placeholder");

            foreach (var mode in new[] { AppThemeMode.Dark, AppThemeMode.HighContrast })
            {
                manager.SetMode(mode);
                Dispatcher.UIThread.RunJobs();
                var variant = ThemeVariant.Dark;
                Assert.Equal(
                    CurrentColor(application, "TextMutedBrush", variant),
                    Assert.IsType<ISolidColorBrush>(nameBox.PlaceholderForeground, exactMatch: false).Color);
                Assert.Equal(
                    CurrentColor(application, "BrandImageSurfaceBrush", variant),
                    Assert.IsType<ISolidColorBrush>(logoSurface.Background, exactMatch: false).Color);
                Assert.Equal(Colors.White, Assert.IsType<ISolidColorBrush>(logoSurface.Background, exactMatch: false).Color);
                Assert.Equal(1d, placeholder.Opacity);
            }
        }
        finally
        {
            window.Close();
            manager.SetMode(AppThemeMode.System);
            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory)) Directory.Delete(directory, recursive: true);
        }
    }

    [AvaloniaFact]
    public void Dialog_factory_creates_bounded_resizable_modal_content()
    {
        var owner = new Window { Width = 900, Height = 700 };
        var dialog = AdaptiveLayout.CreateDialog(owner, "Test", new TextBlock { Text = "Treść" });

        Assert.True(dialog.CanResize);
        Assert.Equal(WindowStartupLocation.CenterOwner, dialog.WindowStartupLocation);
        Assert.InRange(dialog.MinWidth, 320d, dialog.Width);
        Assert.InRange(dialog.MinHeight, 200d, dialog.Height);
        Assert.True(dialog.MaxWidth >= dialog.Width);
        Assert.True(dialog.MaxHeight >= dialog.Height);
        var scroll = Assert.IsType<ScrollViewer>(dialog.Content);
        Assert.Equal(Avalonia.Controls.Primitives.ScrollBarVisibility.Auto, scroll.VerticalScrollBarVisibility);
    }

    [AvaloniaFact]
    public async Task Custom_window_buttons_are_accessible_and_control_the_actual_window_state()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var accounts = new AccountService(
            new AppDbContextFactory(Path.Combine(directory, "discussion49-chrome.db")),
            new PasswordHasher(1_000));
        await accounts.InitializeAsync(importLegacyProfiles: false);
        var window = new MainWindow(
            new AppViewModel(),
            accounts,
            new ContentRepository(),
            new CalculatorSession(new ExpressionCalculator()),
            AppBuildInfo.Current);

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var minimize = Assert.IsType<Button>(window.FindControl<Button>("MinimizeButton"));
            var maximize = Assert.IsType<Button>(window.FindControl<Button>("MaximizeButton"));
            var close = Assert.IsType<Button>(window.FindControl<Button>("CloseButton"));
            var grips = Assert.IsType<Grid>(window.FindControl<Grid>("ResizeGrips"));
            Assert.Equal("Minimalizuj okno", AutomationProperties.GetName(minimize));
            Assert.Equal("Maksymalizuj okno", AutomationProperties.GetName(maximize));
            Assert.Equal("Zamknij okno", AutomationProperties.GetName(close));

            maximize.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.Equal(WindowState.Maximized, window.WindowState);
            Assert.False(grips.IsVisible);
            Assert.Equal("Przywróć okno", AutomationProperties.GetName(maximize));

            maximize.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.Equal(WindowState.Normal, window.WindowState);
            Assert.True(grips.IsVisible);

            minimize.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Assert.Equal(WindowState.Minimized, window.WindowState);
            window.WindowState = WindowState.Normal;

            close.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Dispatcher.UIThread.RunJobs();
            Assert.False(window.IsVisible);
        }
        finally
        {
            if (window.IsVisible) window.Close();
            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory)) Directory.Delete(directory, recursive: true);
        }
    }

    [AvaloniaFact]
    [Trait("Category", "Performance")]
    [Trait("Category", "Accessibility")]
    public async Task Theme_changes_navigation_and_rendering_stay_within_style_impact_budget()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        var accounts = new AccountService(
            new AppDbContextFactory(Path.Combine(directory, "discussion49-navigation.db")),
            new PasswordHasher(1_000));
        await accounts.InitializeAsync(importLegacyProfiles: false);
        var profile = (await accounts.GetProfilesAsync()).Single(item => item.Kind == ProfileKind.Guest);
        var viewModel = new AppViewModel();
        var content = new ContentRepository();
        var window = new MainWindow(
            viewModel,
            accounts,
            content,
            new CalculatorSession(new ExpressionCalculator()),
            AppBuildInfo.Current)
        {
            Width = 960,
            Height = 640
        };

        try
        {
            window.Show();
            viewModel.Login(profile);
            Dispatcher.UIThread.RunJobs();
            var themeButton = window.GetLogicalDescendants()
                .OfType<Button>()
                .Single(button => AutomationProperties.GetAutomationId(button) == "ThemeButton");
            var pages = new[] { AppPage.Home, AppPage.Formulas, AppPage.Exams, AppPage.Calculator, AppPage.About };
            var hashes = new HashSet<string>(StringComparer.Ordinal);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
            var stopwatch = Stopwatch.StartNew();
            for (var index = 0; index < pages.Length; index++)
            {
                if (index > 0)
                    themeButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                viewModel.Navigate(pages[index]);
                window.Width = index % 2 == 0 ? 960 : 1280;
                window.Height = index % 2 == 0 ? 640 : 820;
                Dispatcher.UIThread.RunJobs();
                using var frame = Assert.IsType<WriteableBitmap>(window.CaptureRenderedFrame());
                hashes.Add(HashBitmap(frame));
                Assert.NotNull(window.FindControl<Border>("ShellHost")?.Child);
            }
            stopwatch.Stop();
            var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

            TestContext.Current.TestOutputHelper?.WriteLine(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "METRIC discussion49-style-render: navigations={0}; elapsedMs={1:F1}; allocatedBytes={2}; distinctFrames={3}",
                    pages.Length,
                    stopwatch.Elapsed.TotalMilliseconds,
                    allocatedBytes,
                    hashes.Count));

            Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(10), $"Renderowanie i nawigacja trwały {stopwatch.Elapsed}.");
            Assert.True(allocatedBytes <= 128L * 1024L * 1024L, $"Renderowanie zaalokowało {allocatedBytes} B.");
            Assert.True(hashes.Count >= 3, $"Oczekiwano co najmniej 3 różnych klatek, uzyskano {hashes.Count}.");
            Assert.Contains("Systemowy", themeButton.Content as string, StringComparison.Ordinal);
        }
        finally
        {
            window.Close();
            SqliteConnection.ClearAllPools();
            if (Directory.Exists(directory)) Directory.Delete(directory, recursive: true);
        }
    }

    private static void AssertResponsiveColumns(
        Control view,
        string layoutName,
        double wideWidth,
        double compactWidth,
        int wideColumns,
        int compactColumns)
    {
        var window = ShowInWindow(view, wideWidth, 760);
        try
        {
            var layout = view.GetLogicalDescendants().OfType<Grid>().Single(grid => grid.Name == layoutName);
            Assert.Equal(wideColumns, layout.ColumnDefinitions.Count);

            window.Width = compactWidth;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(compactColumns, layout.ColumnDefinitions.Count);

            window.Width = wideWidth;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(wideColumns, layout.ColumnDefinitions.Count);
        }
        finally
        {
            window.Close();
        }
    }

    private static Window ShowInWindow(Control content, double width, double height)
    {
        var window = new Window { Width = width, Height = height, Content = content };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static Dictionary<string, IReadOnlyDictionary<string, Color>> ReadPalettes()
    {
        var document = XDocument.Load(Path.Combine(FindRepositoryRoot(), "AvaloniaApp", "Styles", "AppStyles.axaml"));
        XNamespace avalonia = "https://github.com/avaloniaui";
        XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";
        var palettes = new Dictionary<string, IReadOnlyDictionary<string, Color>>(StringComparer.Ordinal);

        foreach (var name in new[] { "Light", "Dark" })
        {
            var dictionary = document.Descendants(avalonia + "ResourceDictionary")
                .Single(element => element.Attribute(x + "Key")?.Value == name);
            palettes[name] = dictionary.Elements(avalonia + "SolidColorBrush")
                .ToDictionary(
                    element => element.Attribute(x + "Key")?.Value ?? string.Empty,
                    element => Color.Parse(element.Attribute("Color")?.Value ?? throw new InvalidDataException("Brak koloru.")),
                    StringComparer.Ordinal);
        }

        palettes["HighContrast"] = document.Descendants(avalonia + "SolidColorBrush")
            .Where(element => element.Attribute(x + "Key")?.Value.StartsWith("HighContrast.", StringComparison.Ordinal) == true)
            .ToDictionary(
                element => element.Attribute(x + "Key")!.Value["HighContrast.".Length..],
                element => Color.Parse(element.Attribute("Color")?.Value ?? throw new InvalidDataException("Brak koloru.")),
                StringComparer.Ordinal);
        return palettes;
    }

    private static void AssertContrast(
        string paletteName,
        IReadOnlyDictionary<string, Color> palette,
        string foregroundKey,
        string backgroundKey,
        double minimum)
    {
        var ratio = ContrastRatio(palette[foregroundKey], palette[backgroundKey]);
        Assert.True(
            ratio >= minimum,
            $"{paletteName}: {foregroundKey} / {backgroundKey} ma kontrast {ratio:F2}:1, wymagane jest {minimum:F1}:1.");
    }

    private static double ContrastRatio(Color first, Color second)
    {
        var firstLuminance = RelativeLuminance(first);
        var secondLuminance = RelativeLuminance(second);
        return (Math.Max(firstLuminance, secondLuminance) + 0.05d) /
               (Math.Min(firstLuminance, secondLuminance) + 0.05d);
    }

    private static double RelativeLuminance(Color color) =>
        (0.2126d * LinearChannel(color.R)) +
        (0.7152d * LinearChannel(color.G)) +
        (0.0722d * LinearChannel(color.B));

    private static double LinearChannel(byte channel)
    {
        var normalized = channel / 255d;
        return normalized <= 0.04045d
            ? normalized / 12.92d
            : Math.Pow((normalized + 0.055d) / 1.055d, 2.4d);
    }

    private static Color CurrentColor(Application application, string resourceKey, ThemeVariant variant)
    {
        Assert.True(application.TryGetResource(resourceKey, variant, out var value), $"Brak zasobu {resourceKey}.");
        return Assert.IsType<ISolidColorBrush>(value, exactMatch: false).Color;
    }

    private static string HashBitmap(Bitmap bitmap)
    {
        const int bytesPerPixel = 4;
        var stride = bitmap.PixelSize.Width * bytesPerPixel;
        var pixels = new byte[stride * bitmap.PixelSize.Height];
        var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(new PixelRect(bitmap.PixelSize), handle.AddrOfPinnedObject(), pixels.Length, stride);
            return Convert.ToHexString(SHA256.HashData(pixels));
        }
        finally
        {
            handle.Free();
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
