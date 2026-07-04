using System;
using Abituria.Models;
using Abituria.Services;
using Abituria.Ui;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Abituria.Views;

public sealed class CalculatorView : UserControl
{
    public CalculatorView(UiCopyCatalog copy, Action openGeneralCalculator, Action<string> openPlannedCalculator)
    {
        var root = new StackPanel { Spacing = 18 };
        root.Children.Add(UiFactory.PageTitle("Kalkulator funkcji kwadratowej", "Poznaj sposób obliczania delty, miejsc zerowych i postaci funkcji krok po kroku."));
        root.Children.Add(UiFactory.InfoBand(copy.GetRequired("calculator.quadratic.usage")));
        root.Children.Add(UiFactory.InfoBand(copy.GetRequired("calculator.quadratic.definition")));
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
        var calculate = new Button { Content = "Oblicz", Classes = { "primary" }, MinWidth = 150 };
        calculate.Click += (_, _) =>
        {
            var solution = QuadraticSolver.Solve(a.Text, b.Text, c.Text);
            result.Children.Clear();
            result.Children.Add(new TextBlock
            {
                Text = solution.Summary,
                FontSize = 20,
                FontWeight = Avalonia.Media.FontWeight.SemiBold,
                Foreground = UiFactory.Brush(solution.Success ? "#19733B" : "#B42318"),
                TextWrapping = TextWrapping.Wrap
            });
            foreach (var section in solution.Sections)
            {
                result.Children.Add(new TextBlock { Text = section.Title, FontSize = 17, FontWeight = Avalonia.Media.FontWeight.SemiBold, Margin = new Thickness(0, 6, 0, 0) });
                foreach (var line in section.Lines)
                    result.Children.Add(new TextBlock { Text = line, TextWrapping = TextWrapping.Wrap, FontSize = 16 });
            }
        };
        var clear = new Button { Content = "Wyczyść", Classes = { "ghost" }, MinWidth = 120 };
        clear.Click += (_, _) =>
        {
            a.Text = string.Empty;
            b.Text = string.Empty;
            c.Text = string.Empty;
            result.Children.Clear();
            result.Children.Add(new TextBlock { Text = "Wynik pojawi się tutaj.", Classes = { "muted" } });
            a.Focus();
        };
        var actions = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, HorizontalAlignment = HorizontalAlignment.Left };
        actions.Children.Add(calculate);
        actions.Children.Add(clear);
        root.Children.Add(actions);
        root.Children.Add(UiFactory.Card(result, new Thickness(20), "#F7FAFC"));
        root.Children.Add(new TextBlock { Text = "Pozostałe narzędzia", Classes = { "h2" } });
        var plannedTools = new WrapPanel { Orientation = Orientation.Horizontal };
        AddTool(plannedTools, "Kalkulator ogólny", openGeneralCalculator);
        AddPlannedTool(plannedTools, "Generator wykresów", "graph-generator", openPlannedCalculator);
        AddPlannedTool(plannedTools, "Funkcje trygonometryczne", "trigonometric-calculator", openPlannedCalculator);
        root.Children.Add(plannedTools);
        root.Children.Add(UiFactory.InfoBand(copy.GetRequired("calculator.quadratic.forms")));
        root.Children.Add(UiFactory.InfoBand("Format liczb", "Możesz używać przecinka albo kropki jako separatora dziesiętnego."));
        Content = UiFactory.PageScroll(root);
    }

    private static TextBox NumberBox(string label, string value) => new()
    {
        PlaceholderText = label,
        Text = value,
        FontSize = 18,
        HorizontalAlignment = HorizontalAlignment.Stretch
    };

    private static void AddPlannedTool(Panel panel, string label, string id, Action<string> open)
        => AddTool(panel, label, () => open(id));

    private static void AddTool(Panel panel, string label, Action open)
    {
        var button = new Button { Content = label, Classes = { "ghost" }, Margin = new Thickness(0, 0, 10, 10) };
        button.Click += (_, _) => open();
        panel.Children.Add(button);
    }
}
