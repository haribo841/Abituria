using System;
using System.Threading.Tasks;
using Abituria.Models;
using Abituria.Services;
using Abituria.Ui;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Abituria.Views;

public sealed class ProfileView : UserControl
{
    public const int BasicCourseExerciseTotal = 219;
    public const int ExtendedCourseExerciseTotal = 138;

    private readonly TextBlock _progress = new() { Classes = { "muted" } };
    private readonly TextBlock _status = new() { TextWrapping = TextWrapping.Wrap };

    public ProfileView(LocalProfile profile, AccountService accounts, Action logout)
        : this(profile, accounts, [], new CourseExerciseCatalog(), logout)
    {
    }

    public ProfileView(
        LocalProfile profile,
        AccountService accounts,
        CourseExerciseCatalog courseExercises,
        Action logout)
        : this(profile, accounts, [], courseExercises, logout)
    {
    }

    public ProfileView(
        LocalProfile profile,
        AccountService accounts,
        IReadOnlyList<ExamDefinition> exams,
        CourseExerciseCatalog courseExercises,
        Action logout)
    {
        AutomationProperties.SetName(_progress, "Postęp w zadaniach");
        AutomationProperties.SetLiveSetting(_progress, AutomationLiveSetting.Polite);
        AutomationProperties.SetName(_status, "Status profilu");
        AutomationProperties.SetLiveSetting(_status, AutomationLiveSetting.Polite);

        var root = new StackPanel { Spacing = 16 };
        root.Children.Add(UiFactory.PageTitle("Profil", "Konto i postęp zapisane lokalnie na tym urządzeniu."));
        root.Children.Add(UiFactory.InfoBand("Użytkownik", profile.DisplayName));
        root.Children.Add(UiFactory.InfoBand("Rodzaj profilu", profile.Kind == ProfileKind.Guest ? "Profil gościa bez hasła" : "Lokalne konto chronione hasłem"));
        root.Children.Add(UiFactory.InfoBand("Baza danych", accounts.DatabasePath));
        root.Children.Add(_progress);

        if (profile.Kind == ProfileKind.Password)
        {
            var change = new StackPanel { Spacing = 10 };
            change.Children.Add(new TextBlock { Text = "Zmiana hasła", Classes = { "h2" } });
            var current = PasswordBox("Bieżące hasło");
            var next = PasswordBox("Nowe hasło");
            var confirmation = PasswordBox("Powtórz nowe hasło");
            change.Children.Add(current);
            change.Children.Add(next);
            change.Children.Add(confirmation);
            var submit = new Button { Content = "Zmień hasło", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
            submit.Click += async (_, _) =>
            {
                var result = await accounts.ChangePasswordAsync(profile.Id, current.Text ?? string.Empty, next.Text ?? string.Empty, confirmation.Text ?? string.Empty);
                current.Text = next.Text = confirmation.Text = string.Empty;
                ShowStatus(result.Message, result.Success);
                if (result.Success && result.RecoveryCode is not null) await ShowRecoveryCodeAsync(result.RecoveryCode);
            };
            change.Children.Add(submit);
            root.Children.Add(UiFactory.Card(change));
        }

        var logoutButton = new Button { Content = "Wyloguj", Classes = { "primary" }, HorizontalAlignment = HorizontalAlignment.Left };
        logoutButton.Click += (_, _) => logout();
        root.Children.Add(logoutButton);
        root.Children.Add(_status);
        Content = UiFactory.PageScroll(root);
        AttachedToVisualTree += async (_, _) => await LoadProgressAsync(profile, accounts, exams, courseExercises);
    }

    private async Task LoadProgressAsync(
        LocalProfile profile,
        AccountService accounts,
        IReadOnlyList<ExamDefinition> exams,
        CourseExerciseCatalog courseExercises)
    {
        var completed = await accounts.GetCompletedExerciseIdsAsync(profile.Id);
        var courseById = courseExercises.Exercises.ToDictionary(item => item.Id, StringComparer.Ordinal);
        var basicCount = completed.Count(id =>
            courseById.TryGetValue(id, out var exercise) && exercise.Level == "basic");
        var extendedCount = completed.Count(id =>
            courseById.TryGetValue(id, out var exercise) && exercise.Level == "extended");
        var examLines = exams.Select(exam =>
        {
            var ids = exam.Exercises.Select(item => item.Id).ToHashSet(StringComparer.Ordinal);
            var count = completed.Count(ids.Contains);
            var label = string.IsNullOrWhiteSpace(exam.ProgressLabel) ? exam.Title : exam.ProgressLabel;
            return $"{label}: {count} / {exam.ProgressItemCount}";
        });
        _progress.Text = string.Join('\n', examLines.Append(
            $"Podstawa: {basicCount} / {BasicCourseExerciseTotal}").Append(
            $"Część rozszerzona: {extendedCount} / {ExtendedCourseExerciseTotal}"));
    }

    private async Task ShowRecoveryCodeAsync(string code)
    {
        if (TopLevel.GetTopLevel(this) is not Window owner) return;

        var panel = new StackPanel { Spacing = 12 };
        panel.Children.Add(new TextBlock { Text = "Poprzedni kod utracił ważność. Zapisz nowy kod:", TextWrapping = TextWrapping.Wrap });
        var codeBox = new TextBox { Text = code, IsReadOnly = true, HorizontalAlignment = HorizontalAlignment.Stretch };
        AutomationProperties.SetName(codeBox, "Nowy kod odzyskiwania");
        panel.Children.Add(codeBox);
        var dialog = AdaptiveLayout.CreateDialog(owner, "Nowy kod odzyskiwania", panel, preferredWidth: 560, preferredHeight: 280);
        var close = new Button { Content = "Zamknij", Classes = { "primary" }, HorizontalAlignment = HorizontalAlignment.Left };
        close.Click += (_, _) => dialog.Close();
        panel.Children.Add(close);
        await dialog.ShowDialog(owner);
    }

    private void ShowStatus(string message, bool success)
    {
        _status.Text = message;
        UiFactory.UseResource(_status, TextBlock.ForegroundProperty, success ? "SuccessBrush" : "ErrorBrush");
    }

    private static TextBox PasswordBox(string placeholder)
    {
        var box = new TextBox
        {
            PlaceholderText = placeholder,
            PasswordChar = '●',
            MaxLength = PasswordHasher.MaximumPasswordLength
        };
        AutomationProperties.SetName(box, placeholder);
        return box;
    }
}
