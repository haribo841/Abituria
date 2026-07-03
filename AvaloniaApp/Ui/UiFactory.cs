using System;
using Abituria.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Abituria.Ui;

public static class UiFactory
{
    public static IBrush Brush(string color) => new SolidColorBrush(Color.Parse(color));

    public static Border Card(Control content, Thickness? padding = null, string background = "#FFFFFF") => new()
    {
        Child = content,
        Background = Brush(background),
        BorderBrush = Brush("#D8DEE4"),
        BorderThickness = new Thickness(1),
        CornerRadius = new CornerRadius(6),
        Padding = padding ?? new Thickness(20)
    };

    public static StackPanel PageTitle(string title, string subtitle)
    {
        var panel = new StackPanel { Spacing = 6 };
        panel.Children.Add(new TextBlock { Text = title, Classes = { "h1" }, TextWrapping = TextWrapping.Wrap });
        panel.Children.Add(new TextBlock { Text = subtitle, Classes = { "muted" }, TextWrapping = TextWrapping.Wrap });
        return panel;
    }

    public static Border InfoBand(string title, string body)
    {
        var panel = new StackPanel { Spacing = 4 };
        panel.Children.Add(new TextBlock { Text = title, FontSize = 18, FontWeight = FontWeight.SemiBold, Foreground = Brush("#18212B") });
        panel.Children.Add(new TextBlock { Text = body, Classes = { "muted" }, TextWrapping = TextWrapping.Wrap });
        return Card(panel, new Thickness(16), "#F7FAFC");
    }

    public static Border InfoBand(UiCopyEntry content) => InfoBand(content.Title, content.Body);

    public static ScrollViewer PageScroll(Control content) => new()
    {
        Content = content,
        Padding = new Thickness(28),
        Background = Brush("#F3F5F7"),
        HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
        VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto
    };

    public static Image AssetImage(string assetPath, double? maxWidth = null, double? maxHeight = null) => new()
    {
        Source = LoadBitmap(assetPath),
        MaxWidth = maxWidth ?? double.PositiveInfinity,
        MaxHeight = maxHeight ?? double.PositiveInfinity,
        Stretch = Stretch.Uniform,
        HorizontalAlignment = HorizontalAlignment.Left
    };

    public static Bitmap LoadBitmap(string assetPath)
    {
        var normalized = assetPath.TrimStart('/').Replace('\\', '/');
        return new Bitmap(AssetLoader.Open(new Uri($"avares://Abituria/{normalized}")));
    }
}
