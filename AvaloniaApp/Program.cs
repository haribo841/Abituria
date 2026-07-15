using System;
using Abituria.Services;
using Avalonia;

namespace Abituria;

internal static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        if (ReleaseSmokeTestCommand.IsRequested(args))
            return ReleaseSmokeTestCommand.ExecuteAsync(args).GetAwaiter().GetResult();

        return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
