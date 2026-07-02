using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Abituria.Ui;

namespace Abituria.Views;

public sealed class HomeView : UserControl
{
    public HomeView(string profileName, Action showFormulas, Action showExams, Action showCalculator, Action showChapters, Action showRoadmap)
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
        AddTile(grid, 0, 0, "Wzory", "18 pełnych tablic matematycznych", "img/wzory.png", showFormulas);
        AddTile(grid, 1, 0, "Zadania", "Matura poprawkowa 2021 i archiwalne zestawy", "img/zadania.png", showExams);
        AddTile(grid, 0, 1, "Kalkulator", "Wyrażenia i pełna analiza funkcji kwadratowej", "img/kalkulator.png", showCalculator);
        AddTile(grid, 1, 1, "Działy", "Wektory i zachowane materiały działowe", "img/dzialy.png", showChapters);
        AddTile(grid, 0, 2, "Plan rozwoju", "Przeniesione, zaplanowane i zastąpione elementy starych wersji", "img/abituria.png", showRoadmap, 2);
        root.Children.Add(grid);
        root.Children.Add(UiFactory.InfoBand("Tryb pracy", "Najpierw powtórz materiał, następnie rozwiązuj zadania. Poprawne odpowiedzi i ujawnione rozwiązania są zapisywane w profilu."));
        Content = UiFactory.PageScroll(root);
    }

    private static void AddTile(Grid grid, int column, int row, string title, string description, string asset, Action action, int columnSpan = 1)
    {
        var content = new StackPanel { Spacing = 10 };
        content.Children.Add(UiFactory.AssetImage(asset, 52, 52));
        content.Children.Add(new TextBlock { Text = title, Classes = { "h2" } });
        content.Children.Add(new TextBlock { Text = description, Classes = { "muted" }, TextWrapping = TextWrapping.Wrap });
        var button = new Button
        {
            Content = UiFactory.Card(content),
            Padding = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent
        };
        button.Click += (_, _) => action();
        Grid.SetColumn(button, column);
        Grid.SetRow(button, row);
        Grid.SetColumnSpan(button, columnSpan);
        grid.Children.Add(button);
    }
}
