using System;
using System.Threading.Tasks;
using Abituria.Data;
using Abituria.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Abituria.Services;

public static class AppServiceRegistration
{
    public static IServiceCollection AddAbituriaServices(
        this IServiceCollection services,
        AppRuntimeOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        options ??= AppRuntimeOptions.Desktop;

        services.AddSingleton(options);
        services.AddSingleton(_ => new AppDbContextFactory(options.DatabasePath));
        services.AddSingleton<PasswordHasher>();
        services.AddSingleton<AccountService>();
        services.AddSingleton<ContentRepository>();
        services.AddSingleton<ExpressionCalculator>();
        services.AddSingleton<CalculatorSession>();
        services.AddSingleton(AppBuildInfo.Current);
        services.AddSingleton<AppViewModel>();
        services.AddSingleton<MainWindow>();
        return services;
    }

    public static async Task InitializeAbituriaAsync(this IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);
        var options = services.GetRequiredService<AppRuntimeOptions>();
        var accounts = services.GetRequiredService<AccountService>();
        await accounts.InitializeAsync(options.ImportLegacyProfiles);
    }
}
