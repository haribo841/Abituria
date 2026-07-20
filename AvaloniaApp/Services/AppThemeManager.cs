using System;
using Avalonia;
using Avalonia.Platform;
using Avalonia.Styling;

namespace Abituria.Services;

public enum AppThemeMode
{
    System,
    Light,
    Dark,
    HighContrast
}

public sealed class AppThemeManager : IDisposable
{
    private static readonly string[] PaletteResourceKeys =
    [
        "AppBackgroundBrush",
        "SurfaceBrush",
        "SurfaceAltBrush",
        "BrandImageSurfaceBrush",
        "TextPrimaryBrush",
        "TextMutedBrush",
        "BorderBrush",
        "AccentBrush",
        "AccentHoverBrush",
        "AccentPressedBrush",
        "AccentTextBrush",
        "GhostHoverBrush",
        "GhostPressedBrush",
        "FocusBrush",
        "DangerBrush",
        "DangerTextBrush",
        "DangerHoverBrush",
        "DangerPressedBrush",
        "SuccessBrush",
        "ErrorBrush",
        "WarningBrush",
        "WarningSurfaceBrush"
    ];

    private readonly Application _application;
    private IPlatformSettings? _platformSettings;
    private bool _isDisposed;

    public AppThemeManager(Application application)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));
        ApplyCurrentMode();
    }

    public AppThemeMode Mode { get; private set; } = AppThemeMode.System;

    public string DisplayName => Mode switch
    {
        AppThemeMode.System => "Systemowy",
        AppThemeMode.Light => "Jasny",
        AppThemeMode.Dark => "Ciemny",
        AppThemeMode.HighContrast => "Wysoki kontrast",
        _ => "Systemowy"
    };

    public event EventHandler? ModeChanged;

    public void AttachPlatformSettings(IPlatformSettings? platformSettings)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (ReferenceEquals(_platformSettings, platformSettings))
            return;

        if (_platformSettings is not null)
            _platformSettings.ColorValuesChanged -= PlatformSettingsOnColorValuesChanged;

        _platformSettings = platformSettings;

        if (_platformSettings is not null)
            _platformSettings.ColorValuesChanged += PlatformSettingsOnColorValuesChanged;

        ApplyCurrentMode();
    }

    public void Cycle()
    {
        SetMode(Mode switch
        {
            AppThemeMode.System => AppThemeMode.Light,
            AppThemeMode.Light => AppThemeMode.Dark,
            AppThemeMode.Dark => AppThemeMode.HighContrast,
            _ => AppThemeMode.System
        });
    }

    public void SetMode(AppThemeMode mode)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (Mode == mode)
            return;

        Mode = mode;
        ApplyCurrentMode();
        ModeChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        if (_platformSettings is not null)
            _platformSettings.ColorValuesChanged -= PlatformSettingsOnColorValuesChanged;

        _platformSettings = null;
        _isDisposed = true;
    }

    private void PlatformSettingsOnColorValuesChanged(object? sender, PlatformColorValues e)
        => ApplyCurrentMode();

    private void ApplyCurrentMode()
    {
        _application.RequestedThemeVariant = Mode switch
        {
            AppThemeMode.Light => ThemeVariant.Light,
            AppThemeMode.Dark or AppThemeMode.HighContrast => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };

        if (Mode == AppThemeMode.HighContrast || IsSystemHighContrast())
            ApplyHighContrastPalette();
        else
            RemoveHighContrastPalette();
    }

    private bool IsSystemHighContrast() =>
        _platformSettings?.GetColorValues().ContrastPreference == ColorContrastPreference.High;

    private void ApplyHighContrastPalette()
    {
        foreach (var key in PaletteResourceKeys)
        {
            if (_application.TryGetResource($"HighContrast.{key}", ThemeVariant.Dark, out var value))
                _application.Resources[key] = value;
        }
    }

    private void RemoveHighContrastPalette()
    {
        foreach (var key in PaletteResourceKeys)
            _application.Resources.Remove(key);
    }
}
