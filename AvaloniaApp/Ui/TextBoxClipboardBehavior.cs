using System;
using System.Threading.Tasks;
using Abituria.Services;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Abituria.Ui;

public static class TextBoxClipboardBehavior
{
    public static void Attach(TextBox textBox, ITextClipboard clipboard, Action<string> showStatus)
    {
        ArgumentNullException.ThrowIfNull(textBox);
        ArgumentNullException.ThrowIfNull(clipboard);
        ArgumentNullException.ThrowIfNull(showStatus);

        var paste = new MenuItem { Header = "Wklej" };
        paste.Click += async (_, _) => await PasteAsync(textBox, clipboard, showStatus);
        textBox.ContextMenu = new ContextMenu { Items = { paste } };
        textBox.AddHandler(InputElement.KeyDownEvent, async (_, eventArgs) =>
        {
            var modifier = OperatingSystem.IsMacOS() ? KeyModifiers.Meta : KeyModifiers.Control;
            if (eventArgs.Key != Key.V || !eventArgs.KeyModifiers.HasFlag(modifier)) return;

            eventArgs.Handled = true;
            await PasteAsync(textBox, clipboard, showStatus);
        }, RoutingStrategies.Tunnel);
    }

    internal static async Task PasteAsync(TextBox textBox, ITextClipboard clipboard, Action<string> showStatus)
    {
        var result = await clipboard.ReadTextAsync();
        if (!result.Success || result.Text is null)
        {
            showStatus(result.Message);
            return;
        }

        var source = textBox.Text ?? string.Empty;
        var selectionStart = Math.Clamp(Math.Min(textBox.SelectionStart, textBox.SelectionEnd), 0, source.Length);
        var selectionEnd = Math.Clamp(Math.Max(textBox.SelectionStart, textBox.SelectionEnd), selectionStart, source.Length);
        var updated = string.Concat(source.AsSpan(0, selectionStart), result.Text, source.AsSpan(selectionEnd));
        var caret = selectionStart + result.Text.Length;
        textBox.Text = updated;
        textBox.CaretIndex = caret;
        textBox.SelectionStart = caret;
        textBox.SelectionEnd = caret;
        textBox.Focus();
    }
}
