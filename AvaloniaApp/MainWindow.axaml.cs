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
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;

namespace Abituria;

public partial class MainWindow : Window
{
    private readonly AppViewModel _viewModel;
    private readonly AccountService _accounts;
    private readonly ContentRepository _content;
    private readonly CalculatorSession _calculatorSession;
    private Border _shellHost = null!;

    public MainWindow() : this(
        App.Services.GetRequiredService<AppViewModel>(),
        App.Services.GetRequiredService<AccountService>(),
        App.Services.GetRequiredService<ContentRepository>(),
        App.Services.GetRequiredService<CalculatorSession>())
    {
    }

    public MainWindow(AppViewModel viewModel, AccountService accounts, ContentRepository content, CalculatorSession calculatorSession)
    {
        _viewModel = viewModel;
        _accounts = accounts;
        _content = content;
        _calculatorSession = calculatorSession;
        InitializeComponent();
        _shellHost = this.FindControl<Border>("ShellHost") ?? throw new InvalidOperationException("Nie znaleziono ShellHost.");
        _viewModel.PropertyChanged += ViewModelOnPropertyChanged;
        Render();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void ViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(AppViewModel.CurrentPage) or nameof(AppViewModel.ActiveProfile)) Render();
    }

    private void Render()
    {
        if (_viewModel.ActiveProfile is null || _viewModel.CurrentPage == AppPage.Login)
        {
            _shellHost.Child = new LoginView(_accounts, _content.UiCopy, _viewModel.Login);
            return;
        }

        var root = new Grid { RowDefinitions = new RowDefinitions("Auto,*"), Background = UiFactory.Brush("#F3F5F7") };
        root.Children.Add(BuildTopBar());
        var body = BuildPage();
        Grid.SetRow(body, 1);
        root.Children.Add(body);
        _shellHost.Child = root;
    }

    private Control BuildTopBar()
    {
        var header = new Border
        {
            Background = UiFactory.Brush("#FFFFFF"),
            BorderBrush = UiFactory.Brush("#D8DEE4"),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(22, 11)
        };
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"), ColumnSpacing = 18 };
        var brand = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, VerticalAlignment = VerticalAlignment.Center };
        brand.Children.Add(UiFactory.AssetImage("img/icon.png", 34, 34));
        brand.Children.Add(new TextBlock { Text = "Abituria", FontSize = 25, FontWeight = FontWeight.Bold, VerticalAlignment = VerticalAlignment.Center });
        grid.Children.Add(brand);

        var nav = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        AddNav(nav, "Start", AppPage.Home);
        AddNav(nav, "Wzory", AppPage.Formulas);
        AddNav(nav, "Zadania", AppPage.Exams);
        AddNav(nav, "Działy", AppPage.Chapters);
        AddNav(nav, "Kalkulator", AppPage.Calculator);
        AddNav(nav, "Plan rozwoju", AppPage.Roadmap);
        AddNav(nav, "Profil", AppPage.Profile);
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
        AppPage.Exams => _viewModel.CurrentPage is AppPage.Exams or AppPage.ExerciseList or AppPage.Exercise,
        AppPage.Chapters => _viewModel.CurrentPage is AppPage.Chapters or AppPage.ChapterDetail,
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
            () => _viewModel.Navigate(AppPage.Exams),
            () => _viewModel.Navigate(AppPage.Calculator),
            () => _viewModel.Navigate(AppPage.Chapters),
            () => _viewModel.OpenRoadmap()),
        AppPage.Formulas => new FormulaListView(_content.Formulas, _viewModel.OpenFormula),
        AppPage.FormulaDetail when _viewModel.SelectedFormula is not null => new ArticleView(
            _viewModel.SelectedFormula.Title, "Tablica matematyczna", _viewModel.SelectedFormula.Blocks,
            () => _viewModel.Navigate(AppPage.Formulas)),
        AppPage.Exams => new ExamOverviewView(_content.Exam, _content.Placeholders.Items, _viewModel.OpenExam, _viewModel.OpenTopic, _viewModel.OpenPlaceholder),
        AppPage.ExerciseList => new ExerciseListView(_content.Exam, _viewModel.SelectedTopicId, _viewModel.ActiveProfile!, _accounts, _viewModel.OpenExercise, () => _viewModel.Navigate(AppPage.Exams)),
        AppPage.Exercise when _viewModel.SelectedExercise is not null => new ExerciseView(
            _viewModel.SelectedExercise, CurrentExerciseContext(), _content.Exam.Source, _content.UiCopy, _viewModel.ActiveProfile!, _accounts,
            () => _viewModel.Navigate(AppPage.ExerciseList), _viewModel.OpenExercise),
        AppPage.Chapters => new ChapterListView(_content.Chapters, OpenChapter),
        AppPage.ChapterDetail when _viewModel.SelectedChapter is { IsAvailable: true } => new ArticleView(
            _viewModel.SelectedChapter.Title, "Materiał działowy", _viewModel.SelectedChapter.Blocks,
            () => _viewModel.Navigate(AppPage.Chapters)),
        AppPage.ChapterDetail when _viewModel.SelectedChapter is not null => new PlaceholderView(
            _viewModel.SelectedChapter.Title, _viewModel.SelectedChapter.Message ?? "Treść w przygotowaniu.",
            _viewModel.SelectedChapter.Blocks, () => _viewModel.Navigate(AppPage.Chapters),
            _viewModel.SelectedChapter.RoadmapId is null ? null : () => _viewModel.OpenRoadmap(_viewModel.SelectedChapter.RoadmapId)),
        AppPage.Calculator => new CalculatorView(_content.UiCopy, _viewModel.OpenGeneralCalculator, OpenPlannedCalculator),
        AppPage.GeneralCalculator => new GeneralCalculatorView(
            _calculatorSession, _content.UiCopy, () => _viewModel.Navigate(AppPage.Calculator)),
        AppPage.Roadmap => new RoadmapView(_content.Roadmap, _viewModel.SelectedRoadmapId),
        AppPage.Profile => new ProfileView(_viewModel.ActiveProfile!, _accounts, _viewModel.Logout),
        AppPage.Placeholder when _viewModel.SelectedPlaceholder is not null => new PlaceholderView(
            _viewModel.SelectedPlaceholder.Title, _viewModel.SelectedPlaceholder.Message,
            _viewModel.SelectedPlaceholder.Blocks,
            () => _viewModel.Navigate(_viewModel.SelectedPlaceholder.Category == "calculator" ? AppPage.Calculator : AppPage.Exams),
            _viewModel.SelectedPlaceholder.RoadmapId is null ? null : () => _viewModel.OpenRoadmap(_viewModel.SelectedPlaceholder.RoadmapId)),
        _ => new TextBlock { Text = "Nie udało się otworzyć strony.", Margin = new Thickness(30) }
    };

    private void OpenChapter(ChapterArticle chapter) => _viewModel.OpenChapter(chapter);

    private IReadOnlyList<ExerciseDefinition> CurrentExerciseContext() => _viewModel.SelectedTopicId is null
        ? _content.Exam.Exercises.OrderBy(item => item.Number).ToList()
        : _content.Exam.Exercises.Where(item => item.TopicId == _viewModel.SelectedTopicId).OrderBy(item => item.Number).ToList();

    private void OpenPlannedCalculator(string id)
    {
        var placeholder = _content.Placeholders.Items.Single(item => item.Id == id);
        _viewModel.OpenPlaceholder(placeholder);
    }
}
