using System;
using System.IO;
using Abituria.Models;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Abituria.Ui;

public static class UiFactory
{
    public static T UseResource<T>(T element, AvaloniaProperty property, string resourceKey)
        where T : StyledElement
    {
        ArgumentNullException.ThrowIfNull(element);
        ArgumentNullException.ThrowIfNull(property);
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceKey);

        element[!property] = new DynamicResourceExtension(resourceKey);
        return element;
    }

    public static Border Card(Control content, Thickness? padding = null, string background = "SurfaceBrush")
    {
        var card = new Border
        {
            Child = content,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = padding ?? new Thickness(20)
        };
        UseResource(card, Border.BackgroundProperty, background);
        UseResource(card, Border.BorderBrushProperty, "BorderBrush");
        return card;
    }

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
        var heading = new TextBlock { Text = title, FontSize = 18, FontWeight = FontWeight.SemiBold };
        UseResource(heading, TextBlock.ForegroundProperty, "TextPrimaryBrush");
        panel.Children.Add(heading);
        panel.Children.Add(new TextBlock { Text = body, Classes = { "muted" }, TextWrapping = TextWrapping.Wrap });
        return Card(panel, new Thickness(16), "SurfaceAltBrush");
    }

    public static Border InfoBand(UiCopyEntry content) => InfoBand(content.Title, content.Body);

    public static ScrollViewer PageScroll(Control content)
    {
        var scrollViewer = new ScrollViewer
        {
            Content = content,
            Padding = new Thickness(28),
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto
        };
        UseResource(scrollViewer, ScrollViewer.BackgroundProperty, "AppBackgroundBrush");
        return scrollViewer;
    }

    public static Image AssetImage(
        string assetPath,
        double? maxWidth = null,
        double? maxHeight = null,
        string? alternativeText = null)
    {
        var image = new Image
        {
            Source = LoadBitmap(assetPath),
            MaxWidth = maxWidth ?? double.PositiveInfinity,
            MaxHeight = maxHeight ?? double.PositiveInfinity,
            Stretch = Stretch.Uniform,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        AutomationProperties.SetName(
            image,
            string.IsNullOrWhiteSpace(alternativeText)
                ? $"Ilustracja: {Path.GetFileNameWithoutExtension(assetPath)}"
                : alternativeText);
        return image;
    }

    public static Bitmap LoadBitmap(string assetPath)
    {
        var normalized = assetPath.TrimStart('/').Replace('\\', '/');
        return new Bitmap(AssetLoader.Open(new Uri($"avares://Abituria/{normalized}")));
    }
}
