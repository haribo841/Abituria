using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Input.Platform;

namespace Abituria.Services;

public readonly record struct ClipboardWriteResult(bool Success, string Message);

public readonly record struct ClipboardReadResult(bool Success, string? Text, string Message);

public interface ITextClipboard
{
    Task<ClipboardWriteResult> WriteTextAsync(string text);
    Task<ClipboardReadResult> ReadTextAsync();
}

public sealed class AvaloniaTextClipboard : ITextClipboard
{
    private IClipboard? _clipboard;

    public void Attach(IClipboard? clipboard) => _clipboard = clipboard;

    public async Task<ClipboardWriteResult> WriteTextAsync(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var clipboard = _clipboard;
        if (clipboard is null)
            return UnavailableWriteResult();

        try
        {
            await clipboard.SetTextAsync(text);
            return new ClipboardWriteResult(true, "Ans skopiowano do schowka.");
        }
        catch (Exception exception) when (IsUnavailableException(exception))
        {
            return UnavailableWriteResult();
        }
    }

    public async Task<ClipboardReadResult> ReadTextAsync()
    {
        var clipboard = _clipboard;
        if (clipboard is null)
            return UnavailableReadResult();

        try
        {
            var text = await clipboard.TryGetTextAsync();
            return string.IsNullOrEmpty(text)
                ? new ClipboardReadResult(false, null, "Schowek nie zawiera tekstu.")
                : new ClipboardReadResult(true, text, string.Empty);
        }
        catch (Exception exception) when (IsUnavailableException(exception))
        {
            return UnavailableReadResult();
        }
    }

    private static bool IsUnavailableException(Exception exception) =>
        exception is InvalidOperationException or
            PlatformNotSupportedException or
            UnauthorizedAccessException or
            ExternalException;

    private static ClipboardWriteResult UnavailableWriteResult() =>
        new(false, "Nie udało się skopiować Ans. Schowek systemowy jest niedostępny.");

    private static ClipboardReadResult UnavailableReadResult() =>
        new(false, null, "Nie udało się odczytać schowka systemowego.");
}
