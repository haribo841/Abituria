using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Abituria.Ui;

public static class AdaptiveLayout
{
    private const double DialogMinWidth = 340;
    private const double DialogMinHeight = 220;
    private const double DialogMaxWidth = 720;
    private const double DialogMaxHeight = 640;

    public static void ObserveWidth(Control control, double compactBelow, Action<bool> updateLayout)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(updateLayout);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(compactBelow);

        bool? isCompact = null;

        void Update(double width)
        {
            if (width <= 0) return;

            var next = width < compactBelow;
            if (isCompact == next) return;

            isCompact = next;
            updateLayout(next);
        }

        control.SizeChanged += (_, eventArgs) => Update(eventArgs.NewSize.Width);

        if (control.Bounds.Width > 0)
        {
            Update(control.Bounds.Width);
        }
        else
        {
            isCompact = false;
            updateLayout(false);
        }
    }

    public static Window CreateDialog(
        Window owner,
        string title,
        Control content,
        double preferredWidth = 560,
        double preferredHeight = 300)
    {
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(content);

        var ownerWidth = owner.ClientSize.Width > 0 ? owner.ClientSize.Width : preferredWidth;
        var ownerHeight = owner.ClientSize.Height > 0 ? owner.ClientSize.Height : preferredHeight;
        var width = Math.Clamp(ownerWidth * 0.7, DialogMinWidth, Math.Min(preferredWidth, DialogMaxWidth));
        var height = Math.Clamp(ownerHeight * 0.55, DialogMinHeight, Math.Min(preferredHeight, DialogMaxHeight));

        var dialog = new Window
        {
            Title = title,
            Width = width,
            Height = height,
            MinWidth = DialogMinWidth,
            MinHeight = DialogMinHeight,
            MaxWidth = DialogMaxWidth,
            MaxHeight = DialogMaxHeight,
            CanResize = true,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new ScrollViewer
            {
                Content = UiFactory.Card(content, new Thickness(24)),
                Padding = new Thickness(16),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            }
        };
        UiFactory.UseResource(dialog, Window.BackgroundProperty, "AppBackgroundBrush");
        return dialog;
    }
}
