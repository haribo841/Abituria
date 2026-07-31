using Abituria.Data;
using Abituria.Models;
using Abituria.Services;
using Abituria.Ui;
using Abituria.Views;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Styling;
using Avalonia.Threading;

namespace Abituria.Tests;

public sealed class Issue5CalculatorPipTests
{
    [Fact]
    public async Task Successful_results_are_copied_exactly_in_order_and_failures_do_not_replace_clipboard()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        var clipboard = new RecordingClipboard(delayWrites: true);
        using var coordinator = new CalculatorClipboardCoordinator(session, clipboard);
        var statuses = new List<ClipboardWriteResult>();
        coordinator.StatusChanged += (_, args) => statuses.Add(args.Result);

        var first = session.Calculate("10/4");
        var repeated = session.RepeatLast();
        var originalHistoryItem = session.History[1];
        var restored = session.RestoreHistory(originalHistoryItem);
        var invalid = session.Calculate("2+");
        session.ClearHistory();

        await coordinator.FlushAsync();

        Assert.True(first.Success);
        Assert.True(repeated.Success);
        Assert.True(restored.Success);
        Assert.False(invalid.Success);
        Assert.Equal(
            [first.DisplayValue, repeated.DisplayValue, restored.DisplayValue],
            clipboard.Writes);
        Assert.Equal(1, clipboard.MaximumConcurrentWrites);
        Assert.Equal(3, statuses.Count);
        Assert.True(coordinator.LastResult?.Success);
        Assert.Equal("Ans skopiowano do schowka.", coordinator.LastResult?.Message);

        coordinator.Dispose();
        session.Calculate("7");
        await coordinator.FlushAsync();
        Assert.Equal(3, clipboard.Writes.Count);
    }

    [Fact]
    public async Task Clipboard_failure_is_reported_without_changing_the_calculation_result()
    {
        var session = new CalculatorSession(new ExpressionCalculator());
        var clipboard = new RecordingClipboard
        {
            WriteResult = new ClipboardWriteResult(false, "Schowek jest chwilowo niedostępny.")
        };
        using var coordinator = new CalculatorClipboardCoordinator(session, clipboard);
        ClipboardWriteResult? reported = null;
        coordinator.StatusChanged += (_, args) => reported = args.Result;

        var result = session.Calculate("6*7");
        await coordinator.FlushAsync();

        Assert.True(result.Success);
        Assert.Equal(42, session.LastResult);
        Assert.Equal(result.DisplayValue, Assert.Single(clipboard.Writes));
        Assert.False(reported?.Success);
        Assert.Equal("Schowek jest chwilowo niedostępny.", reported?.Message);
    }

    [AvaloniaFact]
    public async Task Avalonia_clipboard_bridge_handles_unattached_and_real_headless_clipboards()
    {
        var bridge = new AvaloniaTextClipboard();
        var unavailableWrite = await bridge.WriteTextAsync("42");
        var unavailableRead = await bridge.ReadTextAsync();
        Assert.False(unavailableWrite.Success);
        Assert.False(unavailableRead.Success);

        var window = Show(new TextBlock { Text = "Schowek" }, 420, 240);
        try
        {
            bridge.Attach(window.Clipboard);
            var write = await bridge.WriteTextAsync("dokładne 2,5");
            var read = await bridge.ReadTextAsync();

            Assert.True(write.Success);
            Assert.Equal("Ans skopiowano do schowka.", write.Message);
            Assert.True(read.Success);
            Assert.Equal("dokładne 2,5", read.Text);
        }
        finally
        {
            bridge.Attach(null);
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task Scratchpad_and_numeric_answer_paste_at_selection_and_keep_profile_scoped_text()
    {
        var directory = CreateTestDirectory();
        var accounts = CreateAccounts(directory);
        await accounts.InitializeAsync();
        var profiles = await accounts.GetProfilesAsync();
        var profile = profiles.Single(item => item.Kind == ProfileKind.Guest);
        var otherProfile = new LocalProfile(Guid.NewGuid(), "Inny", ProfileKind.Guest);
        var exercise = NumericExercise("course-paste-1");
        var otherExercise = NumericExercise("course-paste-2");
        var scratchpads = new ExerciseScratchpadSession();
        var clipboard = new RecordingClipboard
        {
            ReadResult = new ClipboardReadResult(true, "XY", string.Empty)
        };
        var launcherCalls = 0;
        var view = CreateExerciseView(exercise, profile, accounts, scratchpads, clipboard, () => launcherCalls++);
        var window = Show(view, 960, 640);

        try
        {
            var scratchpad = FindTextBox(view, "Brudnopis do zadania");
            scratchpad.Text = "abcd";
            Select(scratchpad, 1, 3);
            RaisePasteShortcut(scratchpad);
            await DrainAsync();

            Assert.Equal("aXYd", scratchpad.Text);
            Assert.Equal(3, scratchpad.CaretIndex);
            Assert.Equal("aXYd", scratchpads.GetText(profile.Id, exercise.Id));
            var scratchpadMenu = Assert.IsType<ContextMenu>(scratchpad.ContextMenu);
            Assert.Equal("Wklej", Assert.IsType<MenuItem>(Assert.Single(scratchpadMenu.Items)).Header);

            var answer = FindTextBox(view, "Odpowiedź liczbowa");
            answer.Text = "12";
            Select(answer, 1, 1);
            var contextMenu = Assert.IsType<ContextMenu>(answer.ContextMenu);
            var paste = Assert.IsType<MenuItem>(Assert.Single(contextMenu.Items));
            Assert.Equal("Wklej", paste.Header);
            paste.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            await DrainAsync();

            Assert.Equal("1XY2", answer.Text);
            Assert.Equal(3, answer.CaretIndex);
            answer.Text = "34";
            Select(answer, 0, 2);
            RaisePasteShortcut(answer);
            await DrainAsync();
            Assert.Equal("XY", answer.Text);

            Click(view, "Otwórz kalkulator PiP");
            Assert.Equal(1, launcherCalls);

            window.Content = CreateExerciseView(exercise, profile, accounts, scratchpads, clipboard, () => { });
            Render();
            var restored = FindTextBox((Control)window.Content, "Brudnopis do zadania");
            Assert.Equal("aXYd", restored.Text);

            Assert.Equal(string.Empty, scratchpads.GetText(otherProfile.Id, exercise.Id));
            Assert.Equal(string.Empty, scratchpads.GetText(profile.Id, otherExercise.Id));
        }
        finally
        {
            window.Close();
            DeleteTestDirectory(directory);
        }
    }

    [AvaloniaFact]
    public async Task Paste_failure_keeps_text_and_announces_warning()
    {
        var directory = CreateTestDirectory();
        var accounts = CreateAccounts(directory);
        await accounts.InitializeAsync();
        var profile = (await accounts.GetProfilesAsync()).Single(item => item.Kind == ProfileKind.Guest);
        var clipboard = new RecordingClipboard
        {
            ReadResult = new ClipboardReadResult(false, null, "Schowek nie zawiera tekstu.")
        };
        var view = CreateExerciseView(
            NumericExercise("course-paste-failure"),
            profile,
            accounts,
            new ExerciseScratchpadSession(),
            clipboard,
            () => { });
        var window = Show(view, 720, 520);

        try
        {
            var scratchpad = FindTextBox(view, "Brudnopis do zadania");
            scratchpad.Text = "bez zmian";
            Select(scratchpad, 2, 5);
            RaisePasteShortcut(scratchpad);
            await DrainAsync();

            Assert.Equal("bez zmian", scratchpad.Text);
            Assert.Contains(
                view.GetLogicalDescendants().OfType<TextBlock>(),
                text => text.Text == "Schowek nie zawiera tekstu.");
        }
        finally
        {
            window.Close();
            DeleteTestDirectory(directory);
        }
    }

    [AvaloniaFact]
    public void Pip_controller_reuses_one_view_and_moves_it_between_all_hosts()
    {
        var owner = new Window { Width = 960, Height = 640 };
        var host = new Border { IsVisible = false };
        var root = new Grid();
        root.Children.Add(host);
        owner.Content = root;
        var session = new CalculatorSession(new ExpressionCalculator());
        using var coordinator = new CalculatorClipboardCoordinator(session, new RecordingClipboard());
        using var controller = new CalculatorPipController(
            owner,
            host,
            session,
            new ContentRepository().UiCopy,
            coordinator,
            CalculatorPipMode.OwnedWindow);
        owner.Show();
        Render();

        try
        {
            controller.Open(CalculatorPipMode.OwnedWindow);
            Render();
            var view = Assert.IsType<GeneralCalculatorView>(controller.HostedView);
            var ownedWindow = Assert.IsType<Window>(controller.HostedWindow);
            Assert.False(ownedWindow.ShowInTaskbar);
            Assert.False(ownedWindow.Topmost);
            Assert.Equal(CalculatorPipController.DefaultWidth, ownedWindow.Width);
            Assert.Equal(CalculatorPipController.DefaultHeight, ownedWindow.Height);

            FindTextBox(view, "Wyrażenie matematyczne").Text = "123+456";
            controller.Open(CalculatorPipMode.OwnedWindow);
            Assert.Same(view, controller.HostedView);
            Assert.Same(ownedWindow, controller.HostedWindow);

            controller.ChangeMode(CalculatorPipMode.AlwaysOnTopWindow);
            Assert.Same(view, controller.HostedView);
            Assert.Same(ownedWindow, controller.HostedWindow);
            Assert.True(ownedWindow.Topmost);

            controller.ChangeMode(CalculatorPipMode.InAppPanel);
            Render();
            Assert.Null(controller.HostedWindow);
            Assert.Same(view, host.Child);
            Assert.True(host.IsVisible);
            Assert.Equal("123+456", FindTextBox(view, "Wyrażenie matematyczne").Text);

            owner.Width = 720;
            owner.Height = 520;
            Render();
            Assert.InRange(host.Width, CalculatorPipController.MinimumWidth, CalculatorPipController.DefaultWidth);
            Assert.True(host.MaxHeight >= CalculatorPipController.MinimumHeight);

            controller.ChangeMode(CalculatorPipMode.OwnedWindow);
            Render();
            Assert.Same(view, controller.HostedView);
            Assert.NotNull(controller.HostedWindow);
            Assert.Null(host.Child);
            Assert.False(host.IsVisible);
            Assert.Equal("123+456", FindTextBox(view, "Wyrażenie matematyczne").Text);

            var reopenedWindow = Assert.IsType<Window>(controller.HostedWindow);
            reopenedWindow.Close();
            Render();
            Assert.False(controller.IsOpen);
            Assert.Null(controller.HostedView);
            Assert.Null(controller.HostedWindow);
            Assert.Throws<ArgumentOutOfRangeException>(() => controller.ChangeMode((CalculatorPipMode)99));
        }
        finally
        {
            controller.Close();
            owner.Close();
        }

        controller.Dispose();
        Assert.Throws<ObjectDisposedException>(() => controller.Open(CalculatorPipMode.OwnedWindow));
    }

    [AvaloniaFact]
    public async Task Options_save_each_profile_mode_and_restore_previous_choice_after_failure()
    {
        var savedModes = new List<CalculatorPipMode>();
        var shouldSave = true;
        var view = new OptionsView(CalculatorPipMode.OwnedWindow, mode =>
        {
            savedModes.Add(mode);
            return Task.FromResult(shouldSave);
        });
        var window = Show(view, 720, 520);

        try
        {
            var owned = FindChoice(view, "Tryb PiP: Nad Abiturią");
            var topmost = FindChoice(view, "Tryb PiP: Zawsze na wierzchu");
            var panel = FindChoice(view, "Tryb PiP: Panel w aplikacji");
            Assert.True(owned.IsChecked);

            topmost.IsChecked = true;
            await DrainAsync();
            Assert.Equal([CalculatorPipMode.AlwaysOnTopWindow], savedModes);
            Assert.True(topmost.IsChecked);
            Assert.Contains(StatusTexts(view), text => text == "Zapisano tryb kalkulatora PiP.");

            shouldSave = false;
            panel.IsChecked = true;
            await DrainAsync();
            Assert.Equal(
                [CalculatorPipMode.AlwaysOnTopWindow, CalculatorPipMode.InAppPanel],
                savedModes);
            Assert.True(topmost.IsChecked);
            Assert.False(panel.IsChecked);
            Assert.Contains(StatusTexts(view), text => text == "Nie udało się zapisać ustawienia dla aktywnego profilu.");
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Calculator_page_has_independent_pip_launcher()
    {
        var pipCalls = 0;
        var view = new CalculatorView(new ContentRepository().UiCopy, () => { }, () => pipCalls++, _ => { });
        var window = Show(view, 720, 520);

        try
        {
            Click(view, "Otwórz kalkulator PiP");
            Assert.Equal(1, pipCalls);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task Main_window_closes_in_app_pip_when_profile_logs_out()
    {
        var directory = CreateTestDirectory();
        var accounts = CreateAccounts(directory);
        await accounts.InitializeAsync();
        var storedProfile = (await accounts.GetProfilesAsync()).Single(item => item.Kind == ProfileKind.Guest);
        await accounts.SetCalculatorPipModeAsync(storedProfile.Id, CalculatorPipMode.InAppPanel);
        var profile = (await accounts.GetProfilesAsync()).Single(item => item.Id == storedProfile.Id);
        var viewModel = new Abituria.ViewModels.AppViewModel();
        var window = new MainWindow(
            viewModel,
            accounts,
            new ContentRepository(),
            new CalculatorSession(new ExpressionCalculator()),
            AppBuildInfo.Current,
            new AvaloniaTextClipboard(),
            new ExerciseScratchpadSession())
        {
            Width = 720,
            Height = 520
        };

        try
        {
            window.Show();
            viewModel.Login(profile);
            viewModel.Navigate(Abituria.ViewModels.AppPage.Calculator);
            Render();
            Click(window, "Otwórz kalkulator PiP");

            var host = window.FindControl<Border>("PipOverlayHost");
            Assert.NotNull(host);
            Assert.True(host.IsVisible);
            Assert.IsType<GeneralCalculatorView>(host.Child);

            viewModel.Logout();
            Render();
            Assert.False(host.IsVisible);
            Assert.Null(host.Child);
        }
        finally
        {
            window.Close();
            DeleteTestDirectory(directory);
        }
    }

    [AvaloniaFact]
    public void Pip_view_renders_at_supported_sizes_in_all_application_themes()
    {
        var application = Assert.IsType<TestApplication>(Avalonia.Application.Current);
        using var themeManager = new AppThemeManager(application);
        var clipboard = new RecordingClipboard();
        var session = new CalculatorSession(new ExpressionCalculator());
        using var coordinator = new CalculatorClipboardCoordinator(session, clipboard);
        var view = new GeneralCalculatorView(
            session,
            new ContentRepository().UiCopy,
            () => { },
            coordinator,
            GeneralCalculatorLayout.PictureInPicture);
        var window = Show(view, 360, 480);

        try
        {
            foreach (var mode in new[] { AppThemeMode.Light, AppThemeMode.Dark, AppThemeMode.HighContrast })
            {
                themeManager.SetMode(mode);
                foreach (var size in new[] { (720d, 520d), (960d, 640d), (1280d, 820d) })
                {
                    window.Width = Math.Min(CalculatorPipController.DefaultWidth, size.Item1 - 32);
                    window.Height = Math.Min(CalculatorPipController.DefaultHeight, size.Item2 - 80);
                    Render();
                    Assert.True(view.Bounds.Width <= window.ClientSize.Width);
                    Assert.True(view.Bounds.Height <= window.ClientSize.Height);
                    Assert.Single(view.GetLogicalDescendants().OfType<ScrollViewer>());
                    Assert.NotEmpty(view.GetLogicalDescendants().OfType<Button>());
                    Assert.Equal(
                        mode == AppThemeMode.Light ? ThemeVariant.Light : ThemeVariant.Dark,
                        application.RequestedThemeVariant);
                }
            }
        }
        finally
        {
            window.Close();
            themeManager.SetMode(AppThemeMode.System);
        }
    }

    private static ExerciseView CreateExerciseView(
        LearningExercise exercise,
        LocalProfile profile,
        AccountService accounts,
        ExerciseScratchpadSession scratchpads,
        ITextClipboard clipboard,
        Action openPip)
    {
        var context = new ExerciseViewContext(
            [exercise],
            new SourceDocument(),
            new ContentRepository().UiCopy,
            profile,
            accounts,
            new DiagramCatalog(),
            () => { },
            _ => { })
        {
            Scratchpads = scratchpads,
            Clipboard = clipboard,
            OpenCalculatorPip = openPip
        };
        return new ExerciseView(exercise, context);
    }

    private static LearningExercise NumericExercise(string id) => new()
    {
        Id = id,
        Title = "Wklejanie wyniku",
        Mode = "numeric",
        Prompt = "Podaj wynik.",
        ExpectedValue = 42,
        Hints = ["Oblicz wartość."]
    };

    private static AccountService CreateAccounts(string directory) => new(
        new AppDbContextFactory(Path.Combine(directory, "issue-5.db")),
        new PasswordHasher(1_000));

    private static string CreateTestDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Abituria.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        return directory;
    }

    private static void DeleteTestDirectory(string directory)
    {
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        if (Directory.Exists(directory)) Directory.Delete(directory, true);
    }

    private static Window Show(Control content, double width, double height)
    {
        var window = new Window { Width = width, Height = height, Content = content };
        window.Show();
        Render();
        return window;
    }

    private static TextBox FindTextBox(Control root, string name) =>
        root.GetLogicalDescendants()
            .OfType<TextBox>()
            .Single(control => AutomationProperties.GetName(control) == name);

    private static RadioButton FindChoice(Control root, string name) =>
        root.GetLogicalDescendants()
            .OfType<RadioButton>()
            .Single(control => AutomationProperties.GetName(control) == name);

    private static IEnumerable<string> StatusTexts(Control root) =>
        root.GetLogicalDescendants()
            .OfType<TextBlock>()
            .Where(control => control.Text is not null)
            .Select(control => control.Text!);

    private static void Select(TextBox textBox, int start, int end)
    {
        textBox.CaretIndex = end;
        textBox.SelectionStart = start;
        textBox.SelectionEnd = end;
    }

    private static void RaisePasteShortcut(TextBox textBox)
    {
        textBox.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.V,
            KeyModifiers = OperatingSystem.IsMacOS() ? KeyModifiers.Meta : KeyModifiers.Control
        });
        Render();
    }

    private static void Click(Control root, string content)
    {
        var button = root.GetLogicalDescendants()
            .OfType<Button>()
            .Single(control => string.Equals(control.Content as string, content, StringComparison.Ordinal));
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Render();
    }

    private static async Task DrainAsync()
    {
        await Task.Yield();
        Render();
        await Task.Yield();
        Render();
    }

    private static void Render() => Dispatcher.UIThread.RunJobs();

    private sealed class RecordingClipboard(bool delayWrites = false) : ITextClipboard
    {
        private int _activeWrites;

        public List<string> Writes { get; } = [];
        public int MaximumConcurrentWrites { get; private set; }
        public ClipboardWriteResult WriteResult { get; set; } = new(true, "Ans skopiowano do schowka.");
        public ClipboardReadResult ReadResult { get; set; } = new(false, null, "Schowek nie zawiera tekstu.");

        public async Task<ClipboardWriteResult> WriteTextAsync(string text)
        {
            var concurrentWrites = Interlocked.Increment(ref _activeWrites);
            MaximumConcurrentWrites = Math.Max(MaximumConcurrentWrites, concurrentWrites);
            if (delayWrites) await Task.Yield();
            Writes.Add(text);
            Interlocked.Decrement(ref _activeWrites);
            return WriteResult;
        }

        public Task<ClipboardReadResult> ReadTextAsync() => Task.FromResult(ReadResult);
    }

}
