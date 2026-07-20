using System;
using Abituria.Models;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Abituria.Ui;

namespace Abituria.Views;

public sealed class HomeView : UserControl
{
    public HomeView(string profileName, UiCopyCatalog copy, Action showFormulas, Action showExams, Action showCalculator, Action showChapters, Action showRoadmap)
    {
        var root = new StackPanel { Spacing = 20 };
        root.Children.Add(UiFactory.PageTitle("Start", $"Cześć, {profileName}. Wybierz obszar nauki."));

        var grid = new Grid
        {
            Name = "HomeLayoutRoot",
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto"),
            ColumnSpacing = 16,
            RowSpacing = 16
        };
        var formulas = AddTile(grid, new HomeTile("Wzory", "18 pełnych tablic matematycznych", "img/wzory.png", showFormulas));
        var exams = AddTile(grid, new HomeTile("Zadania", "Matura poprawkowa 2021 i archiwalne zestawy", "img/zadania.png", showExams));
        var calculator = AddTile(grid, new HomeTile("Kalkulator", "Wyrażenia i pełna analiza funkcji kwadratowej", "img/kalkulator.png", showCalculator));
        var chapters = AddTile(grid, new HomeTile("Działy", "7 działów: teoria, przykłady i zadania", "img/dzialy.png", showChapters));
        var roadmap = AddTile(grid, new HomeTile("Plan rozwoju", "Przeniesione, zaplanowane i zastąpione elementy starych wersji", "img/abituria.png", showRoadmap));

        AdaptiveLayout.ObserveWidth(this, 780, isCompact =>
        {
            grid.ColumnDefinitions = new ColumnDefinitions(isCompact ? "*" : "*,*");
            grid.RowDefinitions = new RowDefinitions(isCompact ? "Auto,Auto,Auto,Auto,Auto" : "Auto,Auto,Auto");
            grid.ColumnSpacing = isCompact ? 0 : 16;

            Position(formulas, 0, 0);
            Position(exams, isCompact ? 0 : 1, isCompact ? 1 : 0);
            Position(calculator, 0, isCompact ? 2 : 1);
            Position(chapters, isCompact ? 0 : 1, isCompact ? 3 : 1);
            Position(roadmap, 0, isCompact ? 4 : 2, isCompact ? 1 : 2);
        });

        root.Children.Add(grid);
        root.Children.Add(UiFactory.InfoBand(copy.GetRequired("home.work-mode")));
        Content = UiFactory.PageScroll(root);
    }

    private static Button AddTile(Grid grid, HomeTile tile)
    {
        var content = new StackPanel { Spacing = 10 };
        content.Children.Add(UiFactory.AssetImage(tile.Asset, 52, 52, $"Ikona sekcji {tile.Title}"));
        content.Children.Add(new TextBlock { Text = tile.Title, Classes = { "h2" } });
        content.Children.Add(new TextBlock { Text = tile.Description, Classes = { "muted" }, TextWrapping = TextWrapping.Wrap });
        var button = new Button
        {
            Content = UiFactory.Card(content),
            Classes = { "home-tile" },
            Padding = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch
        };
        AutomationProperties.SetName(button, tile.Title);
        AutomationProperties.SetHelpText(button, tile.Description);
        button.Click += (_, _) => tile.Action();
        grid.Children.Add(button);
        return button;
    }

    private static void Position(Control control, int column, int row, int columnSpan = 1)
    {
        Grid.SetColumn(control, column);
        Grid.SetRow(control, row);
        Grid.SetColumnSpan(control, columnSpan);
    }

    private sealed record HomeTile(string Title, string Description, string Asset, Action Action);
}
