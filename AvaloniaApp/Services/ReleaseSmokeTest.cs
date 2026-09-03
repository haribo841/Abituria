using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abituria.Models;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;

namespace Abituria.Services;

public sealed record ReleaseSmokeTestArguments(string DataDirectory);

public sealed record ReleaseSmokeTestReport(
    string DatabasePath,
    string Version,
    string Commit,
    int FormulaCount,
    int CourseAreaCount,
    int CourseRequirementCount,
    int CourseExerciseCount,
    int ExamCount,
    int ExamExerciseCount,
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
            await Console.Error.WriteLineAsync(error);
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
            await Console.Out.WriteLineAsync($"ABITURIA_RELEASE_SMOKE version={report.Version} commit={report.Commit}");
            await Console.Out.WriteLineAsync(
                $"Abituria {report.Version}: smoke test zakończony powodzeniem " +
                $"({report.FormulaCount} tablic, {report.CourseAreaCount} obszarów, " +
                $"{report.CourseRequirementCount} wymagań, {report.CourseExerciseCount} ćwiczeń kursu, " +
                $"{report.ExamExerciseCount} jednostek postępu w {report.ExamCount} arkuszach).");
            return SuccessExitCode;
        }
        catch (Exception exception)
        {
            await Console.Error.WriteLineAsync($"Release smoke test nie powiódł się: {exception.Message}");
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
        var passwordHasher = services.GetRequiredService<PasswordHasher>();
        _ = services.GetRequiredService<CalculatorSession>();
        var buildInfo = services.GetRequiredService<AppBuildInfo>();

        EnsureContentIsAvailable(content);
        EnsureApplicationDiagramsAreAvailable(content);
        EnsureCalculatorIsOperational(calculator);
        var quadraticSummary = EnsureQuadraticCalculatorIsOperational();
        await EnsureGuestProfileIsAvailableAsync(accounts);
        await EnsureAccountLifecycleIsOperationalAsync(accounts, passwordHasher);

        if (!File.Exists(accounts.DatabasePath))
            throw new InvalidOperationException("Testowa baza danych nie została utworzona.");

        return new ReleaseSmokeTestReport(
            accounts.DatabasePath,
            buildInfo.Version,
            buildInfo.Commit,
            content.Formulas.Articles.Count,
            content.MathCourse.Areas.Count,
            content.MathCourse.Requirements.Count,
            content.CourseExercises.Exercises.Count,
            content.Exams.Count,
            content.Exams.Sum(exam => exam.Exercises.Count),
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
        if (content.Formulas.Articles.Count != 18 ||
            content.MathCourse.Groups.Count != 4 ||
            content.MathCourse.Areas.Count != 13 ||
            content.MathCourse.Requirements.Count != 119 ||
            content.MathCourse.Lessons.SelectMany(lesson => lesson.WorkedExamples).Count() != 238 ||
            content.CourseExercises.Exercises.Count != 357 ||
            content.Exams.Count != 32 ||
            content.GetExam("matura-maj-2026-podstawowa").Exercises.Count != 37 ||
            content.GetExam("matura-maj-2026-rozszerzona").Exercises.Count != 13 ||
            content.GetExam("matura-maj-2025-podstawowa").Exercises.Count != 35 ||
            content.GetExam("matura-maj-2025-rozszerzona").Exercises.Count != 13 ||
            content.GetExam("matura-poprawkowa-2025-podstawowa").Exercises.Count != 36 ||
            content.GetExam("matura-maj-2024-podstawowa").Exercises.Count != 35 ||
            content.GetExam("matura-maj-2024-rozszerzona").Exercises.Count != 14 ||
            content.GetExam("matura-poprawkowa-2024-podstawowa").Exercises.Count != 36 ||
            content.GetExam("matura-maj-2023-podstawowa").Exercises.Count != 34 ||
            content.GetExam("matura-poprawkowa-2023-podstawowa").Exercises.Count != 36 ||
            content.GetExam("matura-maj-2023-rozszerzona").Exercises.Count != 14 ||
            content.GetExam("matura-maj-2022-podstawowa").Exercises.Count != 35 ||
            content.GetExam("matura-maj-2022-rozszerzona").Exercises.Count != 15 ||
            content.GetExam("matura-poprawkowa-2022-podstawowa").Exercises.Count != 35 ||
            content.GetExam("matura-maj-2021-podstawowa").Exercises.Count != 35 ||
            content.GetExam("matura-maj-2021-rozszerzona").Exercises.Count != 15 ||
            content.GetExam("matura-poprawkowa-2021").Exercises.Count != 35 ||
            content.GetExam("matura-maj-2020-podstawowa").Exercises.Count != 34 ||
            content.GetExam("matura-maj-2020-rozszerzona").Exercises.Count != 15 ||
            content.GetExam("matura-poprawkowa-2020-podstawowa").Exercises.Count != 34 ||
            content.GetExam("matura-maj-2019-podstawowa").Exercises.Count != 34 ||
            content.GetExam("matura-maj-2019-rozszerzona").Exercises.Count != 15 ||
            content.GetExam("matura-poprawkowa-2019-podstawowa").Exercises.Count != 34 ||
            content.GetExam("matura-maj-2018-podstawowa").Exercises.Count != 34 ||
            content.GetExam("matura-maj-2018-rozszerzona").Exercises.Count != 15 ||
            content.GetExam("matura-poprawkowa-2018-podstawowa").Exercises.Count != 34 ||
            content.GetExam("matura-maj-2017-podstawowa").Exercises.Count != 34 ||
            content.GetExam("matura-maj-2017-rozszerzona").Exercises.Count != 15 ||
            content.GetExam("matura-poprawkowa-2017-podstawowa").Exercises.Count != 34 ||
            content.GetExam("matura-maj-2016-podstawowa").Exercises.Count != 34 ||
            content.GetExam("matura-maj-2016-rozszerzona").Exercises.Count != 16 ||
            content.GetExam("matura-poprawkowa-2016-podstawowa").Exercises.Count != 34 ||
            content.ExamTopics.Count != 17 ||
            content.UiCopy.Entries.Count == 0)
        {
            throw new InvalidDataException("Nie załadowano kompletu podstawowych treści aplikacji.");
        }

        if (!content.Formulas.Articles.Any(article => article.Id == "formula-2") ||
            !content.MathCourse.Requirements.Any(requirement => requirement.Id == "I.B.1") ||
            !content.CourseExercises.Exercises.Any(exercise => exercise.Id == "course-i-b01-1") ||
            !content.GetExam("matura-maj-2026-podstawowa").Exercises.Any(exercise => exercise.Id == "mm26-p0-z12-1") ||
            !content.GetExam("matura-maj-2026-rozszerzona").Exercises.Any(exercise => exercise.Id == "mm26-r0-z12-2") ||
            !content.GetExam("matura-maj-2025-podstawowa").Exercises.Any(exercise => exercise.Id == "mm25-p0-z12-1") ||
            !content.GetExam("matura-maj-2025-rozszerzona").Exercises.Any(exercise => exercise.Id == "mm25-r0-z12-2") ||
            !content.GetExam("matura-poprawkowa-2025-podstawowa").Exercises.Any(exercise => exercise.Id == "mm25-p0p-z30") ||
            !content.GetExam("matura-maj-2024-podstawowa").Exercises.Any(exercise => exercise.Id == "mm24-p0-z14-4") ||
            !content.GetExam("matura-maj-2024-rozszerzona").Exercises.Any(exercise => exercise.Id == "mm24-r0-z13-2") ||
            !content.GetExam("matura-poprawkowa-2024-podstawowa").Exercises.Any(exercise => exercise.Id == "mm24-p0p-z29") ||
            !content.GetExam("matura-maj-2023-podstawowa").Exercises.Any(exercise => exercise.Id == "mm23-p0-z31-2") ||
            !content.GetExam("matura-poprawkowa-2023-podstawowa").Exercises.Any(exercise => exercise.Id == "mm23-p0p-z29-2") ||
            !content.GetExam("matura-maj-2023-rozszerzona").Exercises.Any(exercise => exercise.Id == "mm23-r0-z12-2") ||
            !content.GetExam("matura-maj-2022-podstawowa").Exercises.Any(exercise => exercise.Id == "em22-p0-z35") ||
            !content.GetExam("matura-maj-2022-rozszerzona").Exercises.Any(exercise => exercise.Id == "em22-r0-z15") ||
            !content.GetExam("matura-poprawkowa-2022-podstawowa").Exercises.Any(exercise => exercise.Id == "em22-p0p-z35") ||
            !content.GetExam("matura-maj-2021-podstawowa").Exercises.Any(exercise => exercise.Id == "em21-p0-z35") ||
            !content.GetExam("matura-maj-2021-rozszerzona").Exercises.Any(exercise => exercise.Id == "em21-r0-z15") ||
            !content.GetExam("matura-maj-2020-podstawowa").Exercises.Any(exercise => exercise.Id == "mm20-p0-z34") ||
            !content.GetExam("matura-maj-2020-rozszerzona").Exercises.Any(exercise => exercise.Id == "mm20-r0-z15") ||
            !content.GetExam("matura-poprawkowa-2020-podstawowa").Exercises.Any(exercise => exercise.Id == "mm20-p0p-z34") ||
            !content.GetExam("matura-maj-2019-podstawowa").Exercises.Any(exercise => exercise.Id == "mm19-p0-z34") ||
            !content.GetExam("matura-maj-2019-rozszerzona").Exercises.Any(exercise => exercise.Id == "mm19-r0-z15") ||
            !content.GetExam("matura-poprawkowa-2019-podstawowa").Exercises.Any(exercise => exercise.Id == "mm19-p0p-z34") ||
            !content.GetExam("matura-maj-2018-podstawowa").Exercises.Any(exercise => exercise.Id == "mm18-p0-z34") ||
            !content.GetExam("matura-maj-2018-rozszerzona").Exercises.Any(exercise => exercise.Id == "mm18-r0-z15") ||
            !content.GetExam("matura-poprawkowa-2018-podstawowa").Exercises.Any(exercise => exercise.Id == "mm18-p0p-z34") ||
            !content.GetExam("matura-maj-2017-podstawowa").Exercises.Any(exercise => exercise.Id == "mm17-p0-z34") ||
            !content.GetExam("matura-maj-2017-rozszerzona").Exercises.Any(exercise => exercise.Id == "mm17-r0-z15") ||
            !content.GetExam("matura-poprawkowa-2017-podstawowa").Exercises.Any(exercise => exercise.Id == "mm17-p0p-z34") ||
            !content.GetExam("matura-maj-2016-podstawowa").Exercises.Any(exercise => exercise.Id == "mm16-p0-z34") ||
            !content.GetExam("matura-maj-2016-rozszerzona").Exercises.Any(exercise => exercise.Id == "mm16-r0-z16") ||
            !content.GetExam("matura-poprawkowa-2016-podstawowa").Exercises.Any(exercise => exercise.Id == "mm16-p0p-z34") ||
            !content.Exam.Exercises.Any(exercise => exercise.Id == "mp21-z9"))
        {
            throw new InvalidDataException("Nie załadowano reprezentatywnych materiałów wydania.");
        }
    }

    private static void EnsureApplicationDiagramsAreAvailable(ContentRepository content)
    {
        if (content.Diagrams.Diagrams.Count != 226)
            throw new InvalidDataException("Katalog diagramów aplikacji jest niekompletny.");

        var requiredDiagramIds = new[]
        {
            "formula-w9a", "exam-mp21-z9", "exam-em21-p0-z24", "exam-em21-r0-z14",
            "exam-mm20-p0-z34", "exam-mm20-r0-z15", "exam-mm20-p0p-z32", "exam-mm19-p0-z34",
            "exam-mm19-r0-z15", "exam-mm19-p0p-z34", "exam-mm18-p0-z34", "exam-mm18-r0-z15",
            "exam-mm18-p0p-z34", "exam-mm17-p0-z22", "exam-mm17-r0-z09", "exam-mm17-p0p-z21",
            "exam-mm16-p0-z29", "exam-mm16-r0-z16", "exam-mm16-p0p-z33", "exam-em22-p0-z26",
            "exam-em22-r0-z13", "exam-em22-p0p-z35", "exam-mm23-p0-z29", "exam-mm23-p0p-z24",
            "exam-mm23-r0-z13", "exam-mm24-p0-z14", "exam-mm24-r0-z09", "exam-mm24-p0p-z20",
            "exam-mm25-z06", "exam-mm25-p0p-z26", "exam-mm26-z12", "exam-mm26-r0-z11",
            "course-right-triangle"
        };

        foreach (var diagramId in requiredDiagramIds)
            _ = content.Diagrams.GetRequired(diagramId);
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

    private static async Task EnsureAccountLifecycleIsOperationalAsync(
        AccountService accounts,
        PasswordHasher passwordHasher)
    {
        var password = PasswordHasher.GenerateRecoveryCode();
        var registration = await accounts.RegisterAsync("Release smoke profile", password, password);
        if (!registration.Success || registration.Profile is null || string.IsNullOrWhiteSpace(registration.RecoveryCode))
            throw new InvalidOperationException("Rejestracja konta w smoke teście nie powiodła się.");

        if (!(await accounts.AuthenticateAsync(registration.Profile.Id, password)).Success)
            throw new InvalidOperationException("Logowanie konta w smoke teście nie powiodło się.");

        const string exerciseId = "mp21-z9";
        await accounts.MarkExerciseCompletedAsync(registration.Profile.Id, exerciseId);

        var restarted = new AccountService(new Abituria.Data.AppDbContextFactory(accounts.DatabasePath), passwordHasher);
        await restarted.InitializeAsync(importLegacyProfiles: false);
        if (!(await restarted.AuthenticateAsync(registration.Profile.Id, password)).Success ||
            !(await restarted.GetCompletedExerciseIdsAsync(registration.Profile.Id)).Contains(exerciseId))
        {
            throw new InvalidOperationException("Konto lub postęp nie przetrwały ponownego otwarcia bazy.");
        }
    }
}
