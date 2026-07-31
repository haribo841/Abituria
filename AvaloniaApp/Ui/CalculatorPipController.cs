using System;
using Abituria.Models;
using Abituria.Services;
using Abituria.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Abituria.Ui;

public sealed class CalculatorPipController : IDisposable
{
    public const double DefaultWidth = 360;
    public const double DefaultHeight = 480;
    public const double MinimumWidth = 320;
    public const double MinimumHeight = 400;

    private readonly Window _owner;
    private readonly Border _inAppHost;
    private readonly CalculatorSession _session;
    private readonly UiCopyCatalog _copy;
    private readonly CalculatorClipboardCoordinator _clipboardCoordinator;
    private GeneralCalculatorView? _view;
    private Window? _window;
    private bool _isDisposed;

    public CalculatorPipController(
        Window owner,
        Border inAppHost,
        CalculatorSession session,
        UiCopyCatalog copy,
        CalculatorClipboardCoordinator clipboardCoordinator,
        CalculatorPipMode initialMode)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _inAppHost = inAppHost ?? throw new ArgumentNullException(nameof(inAppHost));
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _copy = copy ?? throw new ArgumentNullException(nameof(copy));
        _clipboardCoordinator = clipboardCoordinator ?? throw new ArgumentNullException(nameof(clipboardCoordinator));
        Mode = ValidateMode(initialMode);
        _owner.SizeChanged += OwnerOnSizeChanged;
        UpdatePanelHeight();
    }

    public CalculatorPipMode Mode { get; private set; }
    public bool IsOpen => _view is not null;
    public Window? HostedWindow => _window;
    public GeneralCalculatorView? HostedView => _view;

    public void Open(CalculatorPipMode mode)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        ChangeMode(mode);
        if (_view is not null)
        {
            Activate();
            return;
        }

        _view = new GeneralCalculatorView(
            _session,
            _copy,
            Close,
            _clipboardCoordinator,
            GeneralCalculatorLayout.PictureInPicture);
        HostCurrentMode();
        Activate();
    }

    public void ChangeMode(CalculatorPipMode mode)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        var validated = ValidateMode(mode);
        if (Mode == validated) return;

        Mode = validated;
        if (_view is null) return;

        if (_window is not null && Mode != CalculatorPipMode.InAppPanel)
        {
            _window.Topmost = Mode == CalculatorPipMode.AlwaysOnTopWindow;
            Activate();
            return;
        }

        DetachCurrentHost();
        HostCurrentMode();
        Activate();
    }

    public void Close()
    {
        if (_view is null) return;
        DetachCurrentHost();
        _view = null;
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        Close();
        _owner.SizeChanged -= OwnerOnSizeChanged;
        _isDisposed = true;
    }

    private void HostCurrentMode()
    {
        var view = _view ?? throw new InvalidOperationException("Widok kalkulatora PiP nie został utworzony.");
        if (Mode == CalculatorPipMode.InAppPanel)
        {
            _inAppHost.Child = view;
            _inAppHost.IsVisible = true;
            return;
        }

        var window = new Window
        {
            Title = "Kalkulator PiP - Abituria",
            Width = DefaultWidth,
            Height = DefaultHeight,
            MinWidth = MinimumWidth,
            MinHeight = MinimumHeight,
            CanResize = true,
            ShowInTaskbar = false,
            Topmost = Mode == CalculatorPipMode.AlwaysOnTopWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = view
        };
        UiFactory.UseResource(window, Window.BackgroundProperty, "AppBackgroundBrush");
        window.Closed += WindowOnClosed;
        _window = window;
        window.Show(_owner);
    }

    private void DetachCurrentHost()
    {
        if (_window is not null)
        {
            var window = _window;
            _window = null;
            window.Closed -= WindowOnClosed;
            window.Content = null;
            window.Close();
        }

        if (_inAppHost.Child is not null)
            _inAppHost.Child = null;
        _inAppHost.IsVisible = false;
    }

    private void WindowOnClosed(object? sender, EventArgs eventArgs)
    {
        if (sender is Window window) window.Content = null;
        _window = null;
        _view = null;
    }

    private void Activate()
    {
        _window?.Activate();
        Dispatcher.UIThread.Post(() => _view?.FocusExpression());
    }

    private void OwnerOnSizeChanged(object? sender, SizeChangedEventArgs eventArgs) => UpdatePanelHeight();

    private void UpdatePanelHeight()
    {
        _inAppHost.MaxHeight = Math.Max(MinimumHeight, _owner.ClientSize.Height - 80);
        _inAppHost.Width = Math.Min(DefaultWidth, Math.Max(MinimumWidth, _owner.ClientSize.Width - 32));
    }

    private static CalculatorPipMode ValidateMode(CalculatorPipMode mode)
    {
        if (mode is CalculatorPipMode.OwnedWindow or
            CalculatorPipMode.AlwaysOnTopWindow or
            CalculatorPipMode.InAppPanel)
            return mode;

        throw new ArgumentOutOfRangeException(nameof(mode), mode, "Nieobsługiwany tryb kalkulatora PiP.");
    }
}
