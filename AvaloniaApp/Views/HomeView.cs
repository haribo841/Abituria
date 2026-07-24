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
        var tiles = new[]
        {
            AddTile(grid, new HomeTile("Wzory", "18 pełnych tablic matematycznych", "img/wzory.png", showFormulas)),
            AddTile(grid, new HomeTile("Zadania", "Matura poprawkowa 2021 i archiwalne zestawy", "img/zadania.png", showExams)),
            AddTile(grid, new HomeTile("Kalkulator", "Wyrażenia i pełna analiza funkcji kwadratowej", "img/kalkulator.png", showCalculator)),
            AddTile(grid, new HomeTile("Działy", "7 działów: teoria, przykłady i zadania", "img/dzialy.png", showChapters)),
            AddTile(grid, new HomeTile("Plan rozwoju", "Przeniesione, zaplanowane i zastąpione elementy starych wersji", "img/abituria.png", showRoadmap))
        };

        AdaptiveLayout.ObserveWidth(this, 780, isCompact => ApplyTileLayout(grid, tiles, isCompact));

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

    private static void ApplyTileLayout(Grid grid, IReadOnlyList<Control> tiles, bool isCompact)
    {
        if (isCompact)
        {
            grid.ColumnDefinitions = new ColumnDefinitions("*");
            grid.RowDefinitions = new RowDefinitions("Auto,Auto,Auto,Auto,Auto");
            grid.ColumnSpacing = 0;
            for (var index = 0; index < tiles.Count; index++)
                Position(tiles[index], 0, index);
            return;
        }

        grid.ColumnDefinitions = new ColumnDefinitions("*,*");
        grid.RowDefinitions = new RowDefinitions("Auto,Auto,Auto");
        grid.ColumnSpacing = 16;
        Position(tiles[0], 0, 0);
        Position(tiles[1], 1, 0);
        Position(tiles[2], 0, 1);
        Position(tiles[3], 1, 1);
        Position(tiles[4], 0, 2, 2);
    }

    private static void Position(Control control, int column, int row, int columnSpan = 1)
    {
        Grid.SetColumn(control, column);
        Grid.SetRow(control, row);
        Grid.SetColumnSpan(control, columnSpan);
    }

    private sealed record HomeTile(string Title, string Description, string Asset, Action Action);
}
