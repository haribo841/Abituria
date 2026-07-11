using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abituria.Models;
using Abituria.Services;
using Abituria.Ui;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Layout;
using Avalonia.Media;

namespace Abituria.Views;

public sealed class LoginView : UserControl
{
    private readonly AccountService _accounts;
    private readonly Action<LocalProfile> _onLogin;
    private readonly ComboBox _profiles = new() { Name = "ProfileSelector" };
    private readonly TextBlock _status = new() { Name = "AccountStatusText", TextWrapping = TextWrapping.Wrap };
    private readonly TextBox _password = PasswordBox("Hasło");

    public LoginView(AccountService accounts, UiCopyCatalog copy, Action<LocalProfile> onLogin)
    {
        _accounts = accounts;
        _onLogin = onLogin;
        Content = Build(copy);
        AttachedToVisualTree += async (_, _) => await ReloadProfilesAsync();
    }

    private Grid Build(UiCopyCatalog copy)
    {
        var root = new Grid { ColumnDefinitions = new ColumnDefinitions("1.05*,0.95*"), ColumnSpacing = 24, Margin = new Thickness(30) };
        var intro = new StackPanel { Spacing = 18 };
        intro.Children.Add(UiFactory.AssetImage("img/abituria.png", 230, 92));
        intro.Children.Add(new TextBlock { Text = "Twój matematyczny korepetytor", Classes = { "h1" }, TextWrapping = TextWrapping.Wrap });
        intro.Children.Add(new TextBlock
        {
            Text = "Konta i postęp są przechowywane lokalnie. Profile zaimportowane ze starszej wersji pozostają profilami gościa.",
            Classes = { "muted" },
            TextWrapping = TextWrapping.Wrap
        });
        intro.Children.Add(UiFactory.InfoBand("Materiały", "18 tablic, 7 działów i 35 zadań z matury poprawkowej 2021."));
        intro.Children.Add(UiFactory.InfoBand("Prywatność", "Aplikacja działa offline i nie wysyła danych konta poza komputer."));
        root.Children.Add(UiFactory.Card(intro, new Thickness(30)));

        var forms = new StackPanel { Spacing = 12 };
        forms.Children.Add(new TextBlock { Text = "Logowanie", Classes = { "h2" } });
        _profiles.HorizontalAlignment = HorizontalAlignment.Stretch;
        _profiles.SelectionChanged += (_, _) => UpdateLoginMode();
        forms.Children.Add(_profiles);
        forms.Children.Add(_password);
        var login = new Button { Name = "LoginButton", Content = "Zaloguj", Classes = { "primary" }, HorizontalAlignment = HorizontalAlignment.Stretch };
        login.Click += async (_, _) => await LoginAsync();
        forms.Children.Add(login);

        forms.Children.Add(Separator());
        forms.Children.Add(new TextBlock { Text = "Nowe konto", Classes = { "h2" } });
        forms.Children.Add(UiFactory.InfoBand(copy.GetRequired("account.registration.rules")));
        var name = new TextBox
        {
            Name = "RegistrationNameBox",
            PlaceholderText = "Nazwa użytkownika (1-30 znaków)",
            MaxLength = AccountService.MaximumDisplayNameLength
        };
        var newPassword = PasswordBox("Hasło (minimum 15 znaków)");
        var confirmation = PasswordBox("Powtórz hasło");
        forms.Children.Add(name);
        forms.Children.Add(newPassword);
        forms.Children.Add(confirmation);
        var register = new Button { Name = "RegisterButton", Content = "Utwórz konto", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Stretch };
        register.Click += async (_, _) =>
        {
            var result = await _accounts.RegisterAsync(name.Text ?? string.Empty, newPassword.Text ?? string.Empty, confirmation.Text ?? string.Empty);
            ShowResult(result.Message, result.Success);
            ClearSecrets(newPassword, confirmation);
            if (result.Success)
            {
                await ReloadProfilesAsync(result.Profile?.Id);
                await ShowRecoveryCodeAsync(result.RecoveryCode!);
            }
        };
        forms.Children.Add(register);

        forms.Children.Add(Separator());
        forms.Children.Add(new TextBlock { Text = "Odzyskiwanie hasła", Classes = { "h2" } });
        var recoveryName = new TextBox { PlaceholderText = "Nazwa konta", MaxLength = AccountService.MaximumDisplayNameLength };
        var recoveryCode = new TextBox { PlaceholderText = "Kod odzyskiwania" };
        var recoveredPassword = PasswordBox("Nowe hasło");
        var recoveredConfirmation = PasswordBox("Powtórz nowe hasło");
        forms.Children.Add(recoveryName);
        forms.Children.Add(recoveryCode);
        forms.Children.Add(recoveredPassword);
        forms.Children.Add(recoveredConfirmation);
        var recover = new Button { Content = "Ustaw nowe hasło", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Stretch };
        recover.Click += async (_, _) =>
        {
            var result = await _accounts.RecoverPasswordAsync(recoveryName.Text ?? string.Empty, recoveryCode.Text ?? string.Empty, recoveredPassword.Text ?? string.Empty, recoveredConfirmation.Text ?? string.Empty);
            ShowResult(result.Message, result.Success);
            recoveryCode.Text = string.Empty;
            ClearSecrets(recoveredPassword, recoveredConfirmation);
            if (result.Success) await ShowRecoveryCodeAsync(result.RecoveryCode!);
        };
        forms.Children.Add(recover);
        forms.Children.Add(_status);

        var scroll = new ScrollViewer { Content = forms, VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto };
        var panel = UiFactory.Card(scroll, new Thickness(26), "#FAFCFD");
        Grid.SetColumn(panel, 1);
        root.Children.Add(panel);
        return root;
    }

    private async Task ReloadProfilesAsync(Guid? selectedId = null)
    {
        var choices = (await _accounts.GetProfilesAsync()).Select(profile => new ProfileChoice(profile)).ToList();
        _profiles.ItemsSource = choices;
        _profiles.SelectedItem = choices.FirstOrDefault(choice => choice.Profile.Id == selectedId) ?? choices.FirstOrDefault();
        UpdateLoginMode();
    }

    private void UpdateLoginMode()
    {
        var isGuest = (_profiles.SelectedItem as ProfileChoice)?.Profile.Kind == ProfileKind.Guest;
        _password.IsVisible = !isGuest;
        _password.Text = string.Empty;
    }

    private async Task LoginAsync()
    {
        if (_profiles.SelectedItem is not ProfileChoice choice)
        {
            ShowResult("Wybierz profil.", false);
            return;
        }
        AuthenticationResult result = choice.Profile.Kind == ProfileKind.Guest
            ? await _accounts.LoginGuestAsync(choice.Profile.Id)
            : await _accounts.AuthenticateAsync(choice.Profile.Id, _password.Text ?? string.Empty);
        _password.Text = string.Empty;
        ShowResult(result.Message, result.Success);
        if (result.Success && result.Profile is not null) _onLogin(result.Profile);
    }

    private async Task ShowRecoveryCodeAsync(string code)
    {
        var box = new TextBox { Text = code, IsReadOnly = true, HorizontalAlignment = HorizontalAlignment.Stretch };
        var copy = new Button { Content = "Kopiuj kod", Classes = { "primary" } };
        var dialog = new Window { Title = "Kod odzyskiwania", Width = 520, Height = 230, CanResize = false };
        var panel = new StackPanel { Spacing = 14, Margin = new Thickness(24) };
        panel.Children.Add(new TextBlock { Text = "Zapisz ten kod. Po zamknięciu okna aplikacja nie pokaże go ponownie.", TextWrapping = TextWrapping.Wrap });
        panel.Children.Add(box);
        copy.Click += async (_, _) =>
        {
            if (dialog.Clipboard is not null) await dialog.Clipboard.SetTextAsync(code);
        };
        var close = new Button { Content = "Zamknij", Classes = { "ghost" } };
        close.Click += (_, _) => dialog.Close();
        var buttons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
        buttons.Children.Add(copy);
        buttons.Children.Add(close);
        panel.Children.Add(buttons);
        dialog.Content = panel;
        if (TopLevel.GetTopLevel(this) is Window owner) await dialog.ShowDialog(owner);
    }

    private void ShowResult(string message, bool success)
    {
        _status.Text = message;
        _status.Foreground = UiFactory.Brush(success ? "#19733B" : "#B42318");
    }

    private static TextBox PasswordBox(string placeholder) => new() { PlaceholderText = placeholder, PasswordChar = '●', MaxLength = PasswordHasher.MaximumPasswordLength };
    private static void ClearSecrets(params TextBox[] boxes) { foreach (var box in boxes) box.Text = string.Empty; }
    private static Border Separator() => new() { Height = 1, Background = UiFactory.Brush("#D8DEE4"), Margin = new Thickness(0, 8) };
    private sealed record ProfileChoice(LocalProfile Profile)
    {
        public override string ToString() => Profile.Kind == ProfileKind.Guest ? $"{Profile.DisplayName} (gość)" : Profile.DisplayName;
    }
}
