using System;
using System.Threading.Tasks;
using Abituria.Models;
using Abituria.Services;
using Abituria.Ui;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Abituria.Views;

public sealed class ProfileView : UserControl
{
    private readonly TextBlock _progress = new() { Classes = { "muted" } };
    private readonly TextBlock _status = new() { TextWrapping = TextWrapping.Wrap };

    public ProfileView(LocalProfile profile, AccountService accounts, Action logout)
    {
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
        AttachedToVisualTree += async (_, _) => await LoadProgressAsync(profile, accounts);
    }

    private async Task LoadProgressAsync(LocalProfile profile, AccountService accounts)
    {
        var completed = await accounts.GetCompletedExerciseIdsAsync(profile.Id);
        _progress.Text = $"Ukończone zadania: {completed.Count} / 35";
    }

    private async Task ShowRecoveryCodeAsync(string code)
    {
        var dialog = new Window { Title = "Nowy kod odzyskiwania", Width = 520, Height = 220, CanResize = false };
        var panel = new StackPanel { Spacing = 12, Margin = new Thickness(24) };
        panel.Children.Add(new TextBlock { Text = "Poprzedni kod utracił ważność. Zapisz nowy kod:", TextWrapping = TextWrapping.Wrap });
        panel.Children.Add(new TextBox { Text = code, IsReadOnly = true });
        var close = new Button { Content = "Zamknij", Classes = { "primary" }, HorizontalAlignment = HorizontalAlignment.Left };
        close.Click += (_, _) => dialog.Close();
        panel.Children.Add(close);
        dialog.Content = panel;
        if (TopLevel.GetTopLevel(this) is Window owner) await dialog.ShowDialog(owner);
    }

    private void ShowStatus(string message, bool success)
    {
        _status.Text = message;
        _status.Foreground = UiFactory.Brush(success ? "#19733B" : "#B42318");
    }

    private static TextBox PasswordBox(string placeholder) => new()
    {
        PlaceholderText = placeholder, PasswordChar = '●', MaxLength = PasswordHasher.MaximumPasswordLength
    };
}
