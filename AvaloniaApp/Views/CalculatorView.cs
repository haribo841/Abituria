using System;
using Abituria.Services;
using Abituria.Ui;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Abituria.Views;

public sealed class CalculatorView : UserControl
{
    public CalculatorView(QuadraticSolver solver, Action openGeneralCalculator)
    {
        var root = new StackPanel { Spacing = 18 };
        root.Children.Add(UiFactory.PageTitle("Kalkulator funkcji kwadratowej", "Analiza równania ax² + bx + c = 0 krok po kroku."));
        var form = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*,*"), ColumnSpacing = 12 };
        var a = NumberBox("a", "1");
        var b = NumberBox("b", "-3");
        var c = NumberBox("c", "2");
        form.Children.Add(a);
        Grid.SetColumn(b, 1); form.Children.Add(b);
        Grid.SetColumn(c, 2); form.Children.Add(c);
        root.Children.Add(UiFactory.Card(form));

        var result = new StackPanel { Spacing = 9 };
        result.Children.Add(new TextBlock { Text = "Wynik pojawi się tutaj.", Classes = { "muted" } });
        var calculate = new Button { Content = "Oblicz", Classes = { "primary" }, HorizontalAlignment = HorizontalAlignment.Left, MinWidth = 150 };
        calculate.Click += (_, _) =>
        {
            var solution = solver.Solve(a.Text, b.Text, c.Text);
            result.Children.Clear();
            result.Children.Add(new TextBlock
            {
                Text = solution.Summary, FontSize = 20, FontWeight = Avalonia.Media.FontWeight.SemiBold,
                Foreground = UiFactory.Brush(solution.Success ? "#19733B" : "#B42318"), TextWrapping = TextWrapping.Wrap
            });
            foreach (var step in solution.Steps)
                result.Children.Add(new TextBlock { Text = step, TextWrapping = TextWrapping.Wrap, FontSize = 16 });
        };
        root.Children.Add(calculate);
        root.Children.Add(UiFactory.Card(result, new Thickness(20), "#F7FAFC"));
        var general = new Button { Content = "Kalkulator ogólny", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
        general.Click += (_, _) => openGeneralCalculator();
        root.Children.Add(general);
        root.Children.Add(UiFactory.InfoBand("Format liczb", "Możesz używać przecinka albo kropki jako separatora dziesiętnego."));
        Content = UiFactory.PageScroll(root);
    }

    private static TextBox NumberBox(string label, string value) => new()
    {
        PlaceholderText = label, Text = value, FontSize = 18, HorizontalAlignment = HorizontalAlignment.Stretch
    };
}
