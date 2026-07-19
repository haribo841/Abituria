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
}
