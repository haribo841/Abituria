using System;
using System.Threading.Tasks;

namespace Abituria.Services;

public sealed class CalculatorClipboardStatusChangedEventArgs(ClipboardWriteResult result) : EventArgs
{
    public ClipboardWriteResult Result { get; } = result;
}

public sealed class CalculatorClipboardCoordinator : IDisposable
{
    private readonly CalculatorSession _session;
    private readonly ITextClipboard _clipboard;
    private readonly object _sync = new();
    private Task _pendingWrite = Task.CompletedTask;
    private bool _isDisposed;

    public CalculatorClipboardCoordinator(CalculatorSession session, ITextClipboard clipboard)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _clipboard = clipboard ?? throw new ArgumentNullException(nameof(clipboard));
        _session.ResultAvailable += SessionOnResultAvailable;
    }

    public ClipboardWriteResult? LastResult { get; private set; }

    public event EventHandler<CalculatorClipboardStatusChangedEventArgs>? StatusChanged;

    public Task FlushAsync()
    {
        lock (_sync)
            return _pendingWrite;
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _session.ResultAvailable -= SessionOnResultAvailable;
        _isDisposed = true;
    }

    private void SessionOnResultAvailable(object? sender, CalculatorResultAvailableEventArgs eventArgs)
    {
        lock (_sync)
        {
            if (_isDisposed) return;
            _pendingWrite = CopyAfterAsync(_pendingWrite, eventArgs.DisplayValue);
        }
    }

    private async Task CopyAfterAsync(Task previousWrite, string text)
    {
        await previousWrite;
        var result = await _clipboard.WriteTextAsync(text);
        LastResult = result;
        StatusChanged?.Invoke(this, new CalculatorClipboardStatusChangedEventArgs(result));
    }
}
