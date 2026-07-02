using Abituria.Data;
using Abituria.Services;
using Abituria.ViewModels;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace Abituria;

public partial class App : Application
{
    public static ServiceProvider Services { get; private set; } = null!;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddSingleton<AppDbContextFactory>();
        collection.AddSingleton<PasswordHasher>();
        collection.AddSingleton<AccountService>();
        collection.AddSingleton<ContentRepository>();
        collection.AddSingleton<ExpressionCalculator>();
        collection.AddSingleton<CalculatorSession>();
        collection.AddSingleton<QuadraticSolver>();
        collection.AddSingleton<AppViewModel>();
        collection.AddSingleton<MainWindow>();
        Services = collection.BuildServiceProvider();

        var accounts = Services.GetRequiredService<AccountService>();
        Task.Run(accounts.InitializeAsync).GetAwaiter().GetResult();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = Services.GetRequiredService<MainWindow>();
        base.OnFrameworkInitializationCompleted();
    }
}
