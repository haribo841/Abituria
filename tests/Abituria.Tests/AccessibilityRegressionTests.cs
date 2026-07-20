using Abituria.Data;
using Abituria.Models;
using Abituria.Services;
using Abituria.Views;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.LogicalTree;

namespace Abituria.Tests;

public sealed class AccessibilityRegressionTests
{
    [AvaloniaFact]
    public void Login_and_calculator_controls_expose_names_and_live_results()
    {
        var repository = new ContentRepository();
        var accounts = new AccountService(
            new AppDbContextFactory(Path.Combine(Path.GetTempPath(), "Abituria.Tests", "accessibility-login.db")),
            new PasswordHasher(1_000));
        var login = new LoginView(accounts, repository.UiCopy, _ => { });
        var calculator = new GeneralCalculatorView(
            new CalculatorSession(new ExpressionCalculator()),
            repository.UiCopy,
            () => { });
        var quadraticCalculator = new CalculatorView(repository.UiCopy, () => { }, _ => { });

        var loginControls = login.GetLogicalDescendants().OfType<Control>().ToArray();
        Assert.Contains(loginControls, control => AutomationProperties.GetName(control) == "Profil użytkownika");
        Assert.Contains(loginControls, control => AutomationProperties.GetName(control) == "Nazwa nowego konta");
        var loginStatus = Assert.Single(loginControls, control => AutomationProperties.GetName(control) == "Status konta");
        Assert.Equal(AutomationLiveSetting.Polite, AutomationProperties.GetLiveSetting(loginStatus));

        var calculatorControls = calculator.GetLogicalDescendants().OfType<Control>().ToArray();
        Assert.Contains(calculatorControls, control => AutomationProperties.GetName(control) == "Wyrażenie matematyczne");
        Assert.Contains(calculatorControls, control => AutomationProperties.GetName(control) == "Usuń ostatni znak");
        Assert.Contains(calculatorControls, control => AutomationProperties.GetName(control) == "Oblicz wynik");
        var result = Assert.Single(calculatorControls, control => AutomationProperties.GetName(control) == "Wynik kalkulatora");
        Assert.Equal(AutomationLiveSetting.Polite, AutomationProperties.GetLiveSetting(result));

        var quadraticControls = quadraticCalculator.GetLogicalDescendants().OfType<Control>().ToArray();
        Assert.Contains(quadraticControls, control => AutomationProperties.GetName(control) == "Współczynnik a");
        Assert.Contains(quadraticControls, control => AutomationProperties.GetName(control) == "Współczynnik b");
        Assert.Contains(quadraticControls, control => AutomationProperties.GetName(control) == "Współczynnik c");
        var quadraticResult = Assert.Single(
            quadraticControls,
            control => AutomationProperties.GetName(control) == "Wynik kalkulatora funkcji kwadratowej");
        Assert.Equal(AutomationLiveSetting.Polite, AutomationProperties.GetLiveSetting(quadraticResult));
    }

    [AvaloniaFact]
    public void Exercise_navigation_and_feedback_have_descriptive_automation_metadata()
    {
        var exercises = new[]
        {
            Exercise("previous", 1, "Zadanie poprzednie"),
            Exercise("current", 2, "Zadanie bieżące"),
            Exercise("next", 3, "Zadanie następne")
        };
        var accounts = new AccountService(
            new AppDbContextFactory(Path.Combine(Path.GetTempPath(), "Abituria.Tests", "accessibility-exercise.db")),
            new PasswordHasher(1_000));
        var context = new ExerciseViewContext(
            exercises,
            new SourceDocument { VerifiedOn = "2026-07-19" },
            new ContentRepository().UiCopy,
            new LocalProfile(Guid.NewGuid(), "Tester", ProfileKind.Guest),
            accounts,
            () => { },
            _ => { });
        var view = new ExerciseView(exercises[1], context);
        var controls = view.GetLogicalDescendants().OfType<Control>().ToArray();

        Assert.Contains(controls, control => AutomationProperties.GetName(control) == "Poprzednie zadanie: Zadanie poprzednie");
        Assert.Contains(controls, control => AutomationProperties.GetName(control) == "Następne zadanie: Zadanie następne");
        Assert.Contains(controls, control => AutomationProperties.GetName(control) == "Brudnopis do zadania");
        var status = Assert.Single(controls, control => AutomationProperties.GetName(control) == "Wynik sprawdzania zadania");
        Assert.Equal(AutomationLiveSetting.Polite, AutomationProperties.GetLiveSetting(status));
        var check = Assert.Single(
            controls.OfType<Button>(),
            button => string.Equals(button.Content as string, "Sprawdź odpowiedź", StringComparison.Ordinal));
        Assert.Contains("zapisana", AutomationProperties.GetHelpText(check), StringComparison.OrdinalIgnoreCase);

        var openExercise = new ExerciseDefinition
        {
            Id = "open",
            Number = 4,
            Title = "Zadanie otwarte",
            Mode = "open",
            Prompt = "Treść testowa.",
            RevealedAnswer = "Odpowiedź testowa.",
            VerificationSource = "test",
            SourcePage = 4
        };
        var openView = new ExerciseView(openExercise, context);
        var reveal = Assert.Single(
            openView.GetLogicalDescendants().OfType<Button>(),
            button => string.Equals(
                button.Content as string,
                "Pokaż odpowiedź i oznacz jako ukończone",
                StringComparison.Ordinal));
        Assert.Contains("zapisze", AutomationProperties.GetHelpText(reveal), StringComparison.OrdinalIgnoreCase);
    }

    [AvaloniaFact]
    public void Profile_password_fields_progress_and_status_expose_accessibility_metadata()
    {
        var accounts = new AccountService(
            new AppDbContextFactory(Path.Combine(Path.GetTempPath(), "Abituria.Tests", "accessibility-profile.db")),
            new PasswordHasher(1_000));
        var profile = new LocalProfile(Guid.NewGuid(), "Tester", ProfileKind.Password);
        var view = new ProfileView(profile, accounts, () => { });
        var controls = view.GetLogicalDescendants().OfType<Control>().ToArray();

        foreach (var name in new[] { "Bieżące hasło", "Nowe hasło", "Powtórz nowe hasło" })
            Assert.Contains(controls, control => AutomationProperties.GetName(control) == name);

        var progress = Assert.Single(controls, control => AutomationProperties.GetName(control) == "Postęp w zadaniach");
        Assert.Equal(AutomationLiveSetting.Polite, AutomationProperties.GetLiveSetting(progress));
        var status = Assert.Single(controls, control => AutomationProperties.GetName(control) == "Status profilu");
        Assert.Equal(AutomationLiveSetting.Polite, AutomationProperties.GetLiveSetting(status));

        var profileSource = File.ReadAllText(
            Path.Combine(FindRepositoryRoot(), "AvaloniaApp", "Views", "ProfileView.cs"));
        Assert.Contains("Nowy kod odzyskiwania", profileSource, StringComparison.Ordinal);
    }

    private static ExerciseDefinition Exercise(string id, int number, string title) => new()
    {
        Id = id,
        Number = number,
        Title = title,
        Mode = "multipleChoice",
        Prompt = "Treść testowa.",
        Options = ["A", "B"],
        CorrectOption = 1,
        VerificationSource = "test",
        SourcePage = number
    };

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Abituria.sln"))) return directory.FullName;
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Nie znaleziono katalogu repozytorium Abituria.");
    }
}
