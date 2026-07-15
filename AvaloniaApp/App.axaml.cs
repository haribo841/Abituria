using Abituria.Services;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace Abituria;

public partial class App : Application
{
    private static AppRuntimeOptions _runtimeOptions = AppRuntimeOptions.Desktop;

    public static ServiceProvider Services { get; private set; } = null!;

    internal static void ConfigureRuntime(AppRuntimeOptions options) =>
        _runtimeOptions = options ?? throw new ArgumentNullException(nameof(options));

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        Services = new ServiceCollection()
            .AddAbituriaServices(_runtimeOptions)
            .BuildServiceProvider();

        Task.Run(Services.InitializeAbituriaAsync).GetAwaiter().GetResult();
        if (_runtimeOptions.ShowMainWindow &&
            ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Services.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
