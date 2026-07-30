using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Abituria.Models;
using Abituria.Services;
using Abituria.Ui;
using Abituria.ViewModels;
using Abituria.Views;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Microsoft.Extensions.DependencyInjection;

namespace Abituria;

public partial class MainWindow : Window
{
    private readonly AppViewModel _viewModel;
    private readonly AccountService _accounts;
    private readonly ContentRepository _content;
    private readonly CalculatorSession _calculatorSession;
    private readonly AppBuildInfo _buildInfo;
    private readonly AppThemeManager _themeManager;
    private Border _shellHost = null!;
    private Button? _themeButton;
    private Button? _maximizeButton;
    private Grid? _resizeGrips;

    public MainWindow() : this(
        App.Services.GetRequiredService<AppViewModel>(),
        App.Services.GetRequiredService<AccountService>(),
        App.Services.GetRequiredService<ContentRepository>(),
        App.Services.GetRequiredService<CalculatorSession>(),
        App.Services.GetRequiredService<AppBuildInfo>())
    {
    }

    public MainWindow(AppViewModel viewModel, AccountService accounts, ContentRepository content, CalculatorSession calculatorSession)
        : this(viewModel, accounts, content, calculatorSession, AppBuildInfo.Current)
    {
    }

    public MainWindow(
        AppViewModel viewModel,
        AccountService accounts,
        ContentRepository content,
        CalculatorSession calculatorSession,
        AppBuildInfo buildInfo)
    {
        _viewModel = viewModel;
        _accounts = accounts;
        _content = content;
        _calculatorSession = calculatorSession;
        _buildInfo = buildInfo;
        InitializeComponent();
        _shellHost = this.FindControl<Border>("ShellHost") ?? throw new InvalidOperationException("Nie znaleziono ShellHost.");
        _themeButton = this.FindControl<Button>("ThemeButton") ?? throw new InvalidOperationException("Nie znaleziono ThemeButton.");
        _maximizeButton = this.FindControl<Button>("MaximizeButton") ?? throw new InvalidOperationException("Nie znaleziono MaximizeButton.");
        _resizeGrips = this.FindControl<Grid>("ResizeGrips") ?? throw new InvalidOperationException("Nie znaleziono ResizeGrips.");
        _themeManager = new AppThemeManager(Application.Current ?? throw new InvalidOperationException("Aplikacja nie została zainicjalizowana."));
        _themeManager.ModeChanged += ThemeManagerOnModeChanged;
        Opened += MainWindowOnOpened;
        Closed += MainWindowOnClosed;
        ConfigureWindowControlAccessibility();
        UpdateThemeButton();
        UpdateWindowChromeState();
        _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        Render();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void ViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(AppViewModel.CurrentPage) or
            nameof(AppViewModel.ActiveProfile) or
            nameof(AppViewModel.SelectedCourseLevel))
            Render();
    }

    private void Render()
    {
        if (_viewModel.ActiveProfile is null || _viewModel.CurrentPage == AppPage.Login)
        {
            _shellHost.Child = new LoginView(_accounts, _content.UiCopy, _viewModel.Login);
            return;
        }

        var root = new Grid { RowDefinitions = new RowDefinitions("Auto,*"), Classes = { "app-shell" } };
        root.Children.Add(BuildTopBar());
        var body = BuildPage();
        Grid.SetRow(body, 1);
        root.Children.Add(body);
        _shellHost.Child = root;
    }

    private Border BuildTopBar()
    {
        var header = new Border
        {
            Padding = new Thickness(22, 11)
        };
        header.Classes.Add("app-header");
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"), ColumnSpacing = 18 };
        var brand = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, VerticalAlignment = VerticalAlignment.Center };
        AutomationProperties.SetName(brand, "🍀 Abituria");
        brand.Children.Add(UiFactory.Glyph("🍀", 30, "Koniczyna Abituria"));
        brand.Children.Add(new TextBlock { Text = "Abituria", Classes = { "brand-text" }, VerticalAlignment = VerticalAlignment.Center });
        grid.Children.Add(brand);

        var nav = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        AddNav(nav, "Start", AppPage.Home);
        AddNav(nav, "Wzory", AppPage.Formulas);
        AddNav(nav, "Matura", AppPage.Matura);
        AddNav(nav, "Zadania", AppPage.Tasks);
        AddNav(nav, "Działy", AppPage.Chapters);
        AddNav(nav, "Kalkulator", AppPage.Calculator);
        AddNav(nav, "Plan rozwoju", AppPage.Roadmap);
        AddNav(nav, "Profil", AppPage.Profile);
        AddNav(nav, "O programie", AppPage.About);
        Grid.SetColumn(nav, 1);
        grid.Children.Add(nav);

        var account = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, VerticalAlignment = VerticalAlignment.Center };
        account.Children.Add(new TextBlock { Text = _viewModel.ActiveProfile!.DisplayName, Classes = { "muted" }, VerticalAlignment = VerticalAlignment.Center });
        var logout = new Button { Content = "Wyloguj", Classes = { "ghost" } };
        logout.Click += (_, _) => _viewModel.Logout();
        account.Children.Add(logout);
        Grid.SetColumn(account, 2);
        grid.Children.Add(account);
        header.Child = grid;
        return header;
    }

    private void AddNav(Panel panel, string title, AppPage page)
    {
        var selected = IsSelected(page);
        var button = new Button { Content = title, Classes = { selected ? "primary" : "ghost" }, Margin = new Thickness(3) };
        button.Click += (_, _) => _viewModel.Navigate(page);
        panel.Children.Add(button);
    }

    private bool IsSelected(AppPage page) => page switch
    {
        AppPage.Formulas => _viewModel.CurrentPage is AppPage.Formulas or AppPage.FormulaDetail,
        AppPage.Matura => _viewModel.CurrentPage == AppPage.Matura ||
            (_viewModel.ExamNavigationOrigin == ExamNavigationOrigin.Matura &&
             (_viewModel.CurrentPage is AppPage.ExerciseList or AppPage.Exercise) &&
             _viewModel.SelectedExercise?.IsCourseExercise != true),
        AppPage.Tasks => _viewModel.CurrentPage == AppPage.Tasks ||
            (_viewModel.ExamNavigationOrigin == ExamNavigationOrigin.Tasks &&
             (_viewModel.CurrentPage is AppPage.ExerciseList or AppPage.Exercise) &&
             _viewModel.SelectedExercise?.IsCourseExercise != true),
        AppPage.Chapters => _viewModel.CurrentPage is AppPage.Chapters or AppPage.CourseArea or AppPage.CourseLesson ||
            (_viewModel.CurrentPage == AppPage.Exercise && _viewModel.SelectedExercise?.IsCourseExercise == true),
        AppPage.Calculator => _viewModel.CurrentPage is AppPage.Calculator or AppPage.GeneralCalculator,
        AppPage.Roadmap => _viewModel.CurrentPage == AppPage.Roadmap,
        _ => _viewModel.CurrentPage == page
    };

    private Control BuildPage() => _viewModel.CurrentPage switch
    {
        AppPage.Home => new HomeView(
            _viewModel.ActiveProfile!.DisplayName,
            _content.UiCopy,
            () => _viewModel.Navigate(AppPage.Formulas),
            () => _viewModel.Navigate(AppPage.Matura),
            () => _viewModel.Navigate(AppPage.Tasks),
            () => _viewModel.Navigate(AppPage.Calculator),
            () => _viewModel.Navigate(AppPage.Chapters),
            () => _viewModel.OpenRoadmap()),
        AppPage.Formulas => new FormulaListView(_content.Formulas, _viewModel.OpenFormula),
        AppPage.FormulaDetail when _viewModel.SelectedFormula is not null => new ArticleView(
            _viewModel.SelectedFormula.Title, "Tablica matematyczna", _viewModel.SelectedFormula.Blocks,
            () => _viewModel.Navigate(AppPage.Formulas), _content.Diagrams),
        AppPage.Matura => new MaturaView(
            _content.Exam,
            _content.Placeholders.Items,
            _viewModel.OpenExam,
            _viewModel.OpenPlaceholder,
            _viewModel.OpenRandomExercise),
        AppPage.Tasks => new TaskTopicsView(
            _content.Exam,
            _content.Placeholders.Items,
            _viewModel.OpenTopic,
            _viewModel.OpenPlaceholder,
            _viewModel.OpenRandomExercise),
        AppPage.ExerciseList => new ExerciseListView(
            _content.Exam,
            _viewModel.SelectedTopicId,
            _viewModel.ActiveProfile!,
            _accounts,
            _viewModel.OpenExercise,
            _viewModel.ExamNavigationOrigin == ExamNavigationOrigin.Matura ? "← Matura" : "← Zadania",
            () => _viewModel.Navigate(
                _viewModel.ExamNavigationOrigin == ExamNavigationOrigin.Matura ? AppPage.Matura : AppPage.Tasks)),
        AppPage.Exercise when _viewModel.SelectedExercise is not null => new ExerciseView(
            _viewModel.SelectedExercise, CreateExerciseViewContext()),
        AppPage.Chapters => new ChapterListView(
            _content.MathCourse,
            _viewModel.SelectedCourseLevel,
            _viewModel.SetCourseLevel,
            _viewModel.OpenCourseArea),
        AppPage.CourseArea when _viewModel.SelectedCourseArea is not null => new CourseAreaView(
            _content.MathCourse,
            _viewModel.SelectedCourseArea,
            _viewModel.SelectedCourseLevel,
            _viewModel.SetCourseLevel,
            _viewModel.OpenCourseLesson,
            () => _viewModel.Navigate(AppPage.Chapters)),
        AppPage.CourseLesson when _viewModel.SelectedCourseLesson is not null => new CourseLessonView(
            _content.MathCourse,
            _content.CourseExercises,
            _viewModel.SelectedCourseLesson,
            _viewModel.SelectedCourseLevel,
            _viewModel.OpenCourseExercise,
            () => _viewModel.Navigate(AppPage.CourseArea),
            _content.Diagrams),
        AppPage.Calculator => new CalculatorView(_content.UiCopy, _viewModel.OpenGeneralCalculator, OpenPlannedCalculator),
        AppPage.GeneralCalculator => new GeneralCalculatorView(
            _calculatorSession, _content.UiCopy, () => _viewModel.Navigate(AppPage.Calculator)),
        AppPage.Roadmap => new RoadmapView(_content.Roadmap, _viewModel.SelectedRoadmapId),
        AppPage.About => new AboutView(_buildInfo),
        AppPage.Profile => new ProfileView(
            _viewModel.ActiveProfile!,
            _accounts,
            _content.CourseExercises,
            _viewModel.Logout),
        AppPage.Placeholder when _viewModel.SelectedPlaceholder is not null => new PlaceholderView(
            _viewModel.SelectedPlaceholder.Title, _viewModel.SelectedPlaceholder.Message,
            _viewModel.SelectedPlaceholder.Blocks,
            () => _viewModel.Navigate(_viewModel.SelectedPlaceholder.Category switch
            {
                "calculator" => AppPage.Calculator,
                "exam" => AppPage.Matura,
                _ => AppPage.Tasks
            }),
            _viewModel.SelectedPlaceholder.RoadmapId is null ? null : () => _viewModel.OpenRoadmap(_viewModel.SelectedPlaceholder.RoadmapId)),
        _ => new TextBlock { Text = "Nie udało się otworzyć strony.", Margin = new Thickness(30) }
    };

    private List<LearningExercise> CurrentExerciseContext()
    {
        if (_viewModel.SelectedExercise?.IsCourseExercise == true && _viewModel.SelectedCourseLesson is not null)
        {
            var exerciseIds = _viewModel.SelectedCourseLesson.ExerciseIds.ToHashSet(StringComparer.Ordinal);
            return _content.CourseExercises.Exercises
                .Where(item => exerciseIds.Contains(item.Id))
                .OrderBy(item => item.Number)
                .ToList();
        }

        return _viewModel.SelectedTopicId is null
            ? _content.Exam.Exercises.OrderBy(item => item.Number).ToList()
            : _content.Exam.Exercises
                .Where(item => item.TopicId == _viewModel.SelectedTopicId)
                .OrderBy(item => item.Number)
                .ToList();
    }

    private ExerciseViewContext CreateExerciseViewContext()
    {
        var courseExercise = _viewModel.SelectedExercise?.IsCourseExercise == true;
        var legalSource = _content.MathCourse.Sources.Single(source => source.Id == "legal-basis-2024");
        Action back;
        string backLabel;
        if (courseExercise)
        {
            back = () => _viewModel.Navigate(AppPage.Chapters);
            backLabel = "Działy";
        }
        else if (_viewModel.ExamNavigationOrigin == ExamNavigationOrigin.Matura)
        {
            back = () => _viewModel.Navigate(AppPage.Matura);
            backLabel = "Matura";
        }
        else
        {
            back = () => _viewModel.Navigate(AppPage.Tasks);
            backLabel = "Zadania";
        }

        return new ExerciseViewContext(
            CurrentExerciseContext(),
            courseExercise
                ? new SourceDocument { VerifiedOn = legalSource.VerifiedOn }
                : _content.Exam.Source,
            _content.UiCopy,
            _viewModel.ActiveProfile!,
            _accounts,
            _content.Diagrams,
            back,
            courseExercise ? _viewModel.OpenCourseExercise : _viewModel.OpenExercise)
        {
            BackLabel = backLabel,
            SourceUrl = courseExercise ? legalSource.DocumentUrl : null
        };
    }

    private void OpenPlannedCalculator(string id)
    {
        var placeholder = _content.Placeholders.Items.Single(item => item.Id == id);
        _viewModel.OpenPlaceholder(placeholder);
    }

    private void MainWindowOnOpened(object? sender, EventArgs e) =>
        _themeManager.AttachPlatformSettings(Application.Current?.TryGetFeature(typeof(IPlatformSettings)) as IPlatformSettings);

    private void MainWindowOnClosed(object? sender, EventArgs e)
    {
        _viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
        _themeManager.ModeChanged -= ThemeManagerOnModeChanged;
        _themeManager.Dispose();
    }

    private void ConfigureWindowControlAccessibility()
    {
        var themeButton = _themeButton ?? throw new InvalidOperationException("Nie znaleziono ThemeButton.");
        var maximizeButton = _maximizeButton ?? throw new InvalidOperationException("Nie znaleziono MaximizeButton.");
        var minimizeButton = this.FindControl<Button>("MinimizeButton") ?? throw new InvalidOperationException("Nie znaleziono MinimizeButton.");
        var closeButton = this.FindControl<Button>("CloseButton") ?? throw new InvalidOperationException("Nie znaleziono CloseButton.");

        AutomationProperties.SetName(themeButton, "Zmień motyw aplikacji");
        AutomationProperties.SetAutomationId(themeButton, "ThemeButton");
        AutomationProperties.SetName(minimizeButton, "Minimalizuj okno");
        AutomationProperties.SetAutomationId(minimizeButton, "MinimizeButton");
        AutomationProperties.SetAutomationId(maximizeButton, "MaximizeButton");
        AutomationProperties.SetName(closeButton, "Zamknij okno");
        AutomationProperties.SetAutomationId(closeButton, "CloseButton");
    }

    private void ThemeManagerOnModeChanged(object? sender, EventArgs e) => UpdateThemeButton();

    private void ThemeButtonOnClick(object? sender, RoutedEventArgs e) => _themeManager.Cycle();

    private void UpdateThemeButton()
    {
        if (_themeButton is null)
            return;

        _themeButton.Content = $"Motyw: {_themeManager.DisplayName}";
        AutomationProperties.SetHelpText(_themeButton, $"Aktualny motyw: {_themeManager.DisplayName}. Aktywuj, aby wybrać następny motyw.");
    }

    private void MinimizeButtonOnClick(object? sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void MaximizeButtonOnClick(object? sender, RoutedEventArgs e) => ToggleMaximize();

    private void CloseButtonOnClick(object? sender, RoutedEventArgs e) => Close();

    private void TitleBarOnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        if (e.ClickCount == 2)
        {
            ToggleMaximize();
            e.Handled = true;
            return;
        }

        BeginMoveDrag(e);
        e.Handled = true;
    }

    private void ToggleMaximize() =>
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void ResizeNorthOnPointerPressed(object? sender, PointerPressedEventArgs e) => BeginResize(WindowEdge.North, e);

    private void ResizeSouthOnPointerPressed(object? sender, PointerPressedEventArgs e) => BeginResize(WindowEdge.South, e);

    private void ResizeWestOnPointerPressed(object? sender, PointerPressedEventArgs e) => BeginResize(WindowEdge.West, e);

    private void ResizeEastOnPointerPressed(object? sender, PointerPressedEventArgs e) => BeginResize(WindowEdge.East, e);

    private void ResizeNorthWestOnPointerPressed(object? sender, PointerPressedEventArgs e) => BeginResize(WindowEdge.NorthWest, e);

    private void ResizeNorthEastOnPointerPressed(object? sender, PointerPressedEventArgs e) => BeginResize(WindowEdge.NorthEast, e);

    private void ResizeSouthWestOnPointerPressed(object? sender, PointerPressedEventArgs e) => BeginResize(WindowEdge.SouthWest, e);

    private void ResizeSouthEastOnPointerPressed(object? sender, PointerPressedEventArgs e) => BeginResize(WindowEdge.SouthEast, e);

    private void BeginResize(WindowEdge edge, PointerPressedEventArgs e)
    {
        if (!CanResize || WindowState != WindowState.Normal || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        BeginResizeDrag(edge, e);
        e.Handled = true;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == WindowStateProperty)
            UpdateWindowChromeState();
    }

    private void UpdateWindowChromeState()
    {
        if (_maximizeButton is null || _resizeGrips is null)
            return;

        var isMaximized = WindowState == WindowState.Maximized;
        _resizeGrips.IsVisible = WindowState == WindowState.Normal;
        _maximizeButton.Content = isMaximized ? "❐" : "□";
        ToolTip.SetTip(_maximizeButton, isMaximized ? "Przywróć" : "Maksymalizuj");
        AutomationProperties.SetName(_maximizeButton, isMaximized ? "Przywróć okno" : "Maksymalizuj okno");
    }
}
