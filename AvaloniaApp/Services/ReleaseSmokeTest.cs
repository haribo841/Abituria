using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abituria.Models;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Microsoft.Extensions.DependencyInjection;

namespace Abituria.Services;

public sealed record ReleaseSmokeTestArguments(string DataDirectory);

public sealed record ReleaseSmokeTestReport(
    string DatabasePath,
    string Version,
    int FormulaCount,
    int ChapterCount,
    int ExerciseCount,
    string QuadraticSummary);

public static class ReleaseSmokeTestCommand
{
    public const string CommandSwitch = "--release-smoke-test";
    public const string DataDirectorySwitch = "--data-directory";
    public const int SuccessExitCode = 0;
    public const int FailureExitCode = 1;
    public const int UsageErrorExitCode = 2;

    public static bool IsRequested(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);
        return args.Contains(CommandSwitch, StringComparer.Ordinal);
    }

    public static bool TryParse(
        string[] args,
        out ReleaseSmokeTestArguments? commandArguments,
        out string error)
    {
        ArgumentNullException.ThrowIfNull(args);
        commandArguments = null;
        if (args.Length != 3 ||
            !string.Equals(args[0], CommandSwitch, StringComparison.Ordinal) ||
            !string.Equals(args[1], DataDirectorySwitch, StringComparison.Ordinal) ||
            string.IsNullOrWhiteSpace(args[2]))
        {
            error = $"Użycie: Abituria {CommandSwitch} {DataDirectorySwitch} <katalog-tymczasowy>";
            return false;
        }

        try
        {
            commandArguments = new ReleaseSmokeTestArguments(Path.GetFullPath(args[2]));
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            error = "Podany katalog tymczasowy ma nieprawidłową ścieżkę.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public static async Task<int> ExecuteAsync(string[] args)
    {
        Console.OutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        if (!TryParse(args, out var commandArguments, out var error))
        {
            Console.Error.WriteLine(error);
            return UsageErrorExitCode;
        }

        try
        {
            var runtimeOptions = AppRuntimeOptions.ReleaseSmokeTest(commandArguments!.DataDirectory);
            Directory.CreateDirectory(commandArguments.DataDirectory);
            if (File.Exists(runtimeOptions.DatabasePath))
                throw new InvalidOperationException("Katalog smoke testu zawiera już bazę danych i nie jest izolowany.");

            App.ConfigureRuntime(runtimeOptions);
            Program.BuildAvaloniaApp().SetupWithClassicDesktopLifetime(args);
            var report = await ReleaseSmokeTestRunner.VerifyAsync(App.Services);
            Console.WriteLine(
                $"Abituria {report.Version}: smoke test zakończony powodzeniem " +
                $"({report.FormulaCount} wzorów, {report.ChapterCount} działów, {report.ExerciseCount} zadań).");
            return SuccessExitCode;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Release smoke test nie powiódł się: {exception.Message}");
            return FailureExitCode;
        }
    }
}

public static class ReleaseSmokeTestRunner
{
    public static async Task<ReleaseSmokeTestReport> VerifyAsync(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);
        var options = services.GetRequiredService<AppRuntimeOptions>();
        EnsureIsolatedRuntime(options);
        EnsureNoWindowWasCreated();

        var accounts = services.GetRequiredService<AccountService>();
        var content = services.GetRequiredService<ContentRepository>();
        var calculator = services.GetRequiredService<ExpressionCalculator>();
        _ = services.GetRequiredService<CalculatorSession>();
        var buildInfo = services.GetRequiredService<AppBuildInfo>();

        EnsureContentIsAvailable(content);
        EnsureApplicationAssetIsAvailable();
        EnsureCalculatorIsOperational(calculator);
        var quadraticSummary = EnsureQuadraticCalculatorIsOperational();
        await EnsureGuestProfileIsAvailableAsync(accounts);

        if (!File.Exists(accounts.DatabasePath))
            throw new InvalidOperationException("Testowa baza danych nie została utworzona.");

        return new ReleaseSmokeTestReport(
            accounts.DatabasePath,
            buildInfo.Version,
            content.Formulas.Articles.Count,
            content.Chapters.Chapters.Count,
            content.Exam.Exercises.Count,
            quadraticSummary);
    }

    private static void EnsureIsolatedRuntime(AppRuntimeOptions options)
    {
        if (!options.IsReleaseSmokeTest ||
            options.ImportLegacyProfiles ||
            options.ShowMainWindow ||
            string.IsNullOrWhiteSpace(options.DatabasePath) ||
            !string.Equals(
                Path.GetFileName(options.DatabasePath),
                AppRuntimeOptions.ReleaseSmokeDatabaseFileName,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Smoke test nie został uruchomiony w izolowanym trybie danych.");
        }
    }

    private static void EnsureNoWindowWasCreated()
    {
        var application = Application.Current ??
            throw new InvalidOperationException("Środowisko Avalonia nie zostało zainicjalizowane.");

        if (application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: not null })
            throw new InvalidOperationException("Smoke test nie może tworzyć głównego okna aplikacji.");
    }

    private static void EnsureContentIsAvailable(ContentRepository content)
    {
        if (content.Formulas.Articles.Count == 0 ||
            content.Chapters.Chapters.Count == 0 ||
            content.Exam.Exercises.Count == 0 ||
            content.UiCopy.Entries.Count == 0)
        {
            throw new InvalidDataException("Nie załadowano kompletu podstawowych treści aplikacji.");
        }
    }

    private static void EnsureApplicationAssetIsAvailable()
    {
        using var stream = AssetLoader.Open(new Uri("avares://Abituria/img/icon.png"));
        if (stream.Length == 0) throw new InvalidDataException("Ikona aplikacji jest pusta.");
    }

    private static void EnsureCalculatorIsOperational(ExpressionCalculator calculator)
    {
        var result = calculator.Evaluate("sqrt(9)+1");
        if (!result.Success || result.Value is null || Math.Abs(result.Value.Value - 4d) > 1e-12)
            throw new InvalidOperationException("Kalkulator nie przeszedł testu diagnostycznego.");
    }

    private static string EnsureQuadraticCalculatorIsOperational()
    {
        var result = QuadraticSolver.Solve("1", "-3", "2");
        if (!result.Success ||
            !result.Summary.Contains("x₁ = 1", StringComparison.Ordinal) ||
            !result.Summary.Contains("x₂ = 2", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Kalkulator funkcji kwadratowej nie przeszedł testu diagnostycznego.");
        }

        return result.Summary;
    }

    private static async Task EnsureGuestProfileIsAvailableAsync(AccountService accounts)
    {
        var profiles = await accounts.GetProfilesAsync();
        if (!profiles.Any(profile => profile.Kind == ProfileKind.Guest))
            throw new InvalidOperationException("Nie utworzono lokalnego profilu gościa.");
    }
}
