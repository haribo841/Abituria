using System;
using Abituria.Models;
using Avalonia;
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
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto"),
            ColumnSpacing = 16,
            RowSpacing = 16
        };
        AddTile(grid, 0, 0, new HomeTile("Wzory", "18 pełnych tablic matematycznych", "img/wzory.png", showFormulas));
        AddTile(grid, 1, 0, new HomeTile("Zadania", "Matura poprawkowa 2021 i archiwalne zestawy", "img/zadania.png", showExams));
        AddTile(grid, 0, 1, new HomeTile("Kalkulator", "Wyrażenia i pełna analiza funkcji kwadratowej", "img/kalkulator.png", showCalculator));
        AddTile(grid, 1, 1, new HomeTile("Działy", "Wektory i zachowane materiały działowe", "img/dzialy.png", showChapters));
        AddTile(grid, 0, 2, new HomeTile("Plan rozwoju", "Przeniesione, zaplanowane i zastąpione elementy starych wersji", "img/abituria.png", showRoadmap, 2));
        root.Children.Add(grid);
        root.Children.Add(UiFactory.InfoBand(copy.GetRequired("home.work-mode")));
        Content = UiFactory.PageScroll(root);
    }

    private static void AddTile(Grid grid, int column, int row, HomeTile tile)
    {
        var content = new StackPanel { Spacing = 10 };
        content.Children.Add(UiFactory.AssetImage(tile.Asset, 52, 52));
        content.Children.Add(new TextBlock { Text = tile.Title, Classes = { "h2" } });
        content.Children.Add(new TextBlock { Text = tile.Description, Classes = { "muted" }, TextWrapping = TextWrapping.Wrap });
        var button = new Button
        {
            Content = UiFactory.Card(content),
            Padding = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent
        };
        button.Click += (_, _) => tile.Action();
        Grid.SetColumn(button, column);
        Grid.SetRow(button, row);
        Grid.SetColumnSpan(button, tile.ColumnSpan);
        grid.Children.Add(button);
    }

    private sealed record HomeTile(string Title, string Description, string Asset, Action Action, int ColumnSpan = 1);
}
