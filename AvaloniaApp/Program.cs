using System;
using System.Globalization;
using Abituria.Services;
using Avalonia;

namespace Abituria;

internal static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        var userInterfaceCulture = CultureInfo.GetCultureInfo("pl-PL");
        CultureInfo.DefaultThreadCurrentUICulture = userInterfaceCulture;
        CultureInfo.CurrentUICulture = userInterfaceCulture;

        if (ReleaseSmokeTestCommand.IsRequested(args))
            return ReleaseSmokeTestCommand.ExecuteAsync(args).GetAwaiter().GetResult();

        return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
    }
}
