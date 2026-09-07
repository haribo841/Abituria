using System;
using System.Threading.Tasks;
using Abituria.Models;
using Abituria.Services;
using Abituria.Ui;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Abituria.Views;

public sealed class OptionsView : UserControl
{
    private readonly Dictionary<CalculatorPipMode, RadioButton> _pipChoices = [];
    private readonly TextBlock _status = new() { TextWrapping = TextWrapping.Wrap };
    private readonly Func<CalculatorPipMode, Task<bool>> _saveMode;
    private readonly Action<AppThemeMode> _setTheme;
    private CalculatorPipMode _currentMode;
    private AppThemeMode _currentTheme;
    private bool _isSaving;

    public OptionsView(
        CalculatorPipMode currentMode,
        Func<CalculatorPipMode, Task<bool>> saveMode,
        AppThemeMode currentTheme,
        Action<AppThemeMode> setTheme)
    {
        _currentMode = currentMode;
        _saveMode = saveMode ?? throw new ArgumentNullException(nameof(saveMode));
        _currentTheme = currentTheme;
        _setTheme = setTheme ?? throw new ArgumentNullException(nameof(setTheme));
        AutomationProperties.SetLiveSetting(_status, AutomationLiveSetting.Polite);
        AutomationProperties.SetName(_status, "Stan opcji");

        var root = new StackPanel { Spacing = 18 };
        root.Children.Add(UiFactory.PageTitle(
            "Opcje",
            "Zmień motyw aplikacji i sposób wyświetlania kalkulatora Picture in Picture."));
        root.Children.Add(UiFactory.InfoBand(
            "Motyw aplikacji",
            "Wybierz wariant systemowy, jasny, ciemny albo wysoki kontrast. Zmiana działa natychmiast."));

        var themeChoices = new StackPanel { Spacing = 10 };
        AddThemeChoice(themeChoices, AppThemeMode.System, "Systemowy", "Dopasowuje jasny lub ciemny wariant do ustawień systemu.");
        AddThemeChoice(themeChoices, AppThemeMode.Light, "Jasny", "Używa jasnej palety niezależnie od ustawień systemu.");
        AddThemeChoice(themeChoices, AppThemeMode.Dark, "Ciemny", "Używa ciemnej palety niezależnie od ustawień systemu.");
        AddThemeChoice(themeChoices, AppThemeMode.HighContrast, "Wysoki kontrast", "Zwiększa kontrast tekstów, obramowań i fokusu klawiatury.");
        root.Children.Add(UiFactory.Card(themeChoices));
        root.Children.Add(UiFactory.InfoBand(
            "Kalkulator Picture in Picture",
            "Wybierz sposób wyświetlania kompaktowego kalkulatora. Tryb jest zapisywany osobno dla aktywnego profilu."));

        var choices = new StackPanel { Spacing = 10 };
        AddChoice(choices, CalculatorPipMode.OwnedWindow, "Nad Abiturią", "Okno pozostaje nad głównym oknem aplikacji.");
        AddChoice(choices, CalculatorPipMode.AlwaysOnTopWindow, "Zawsze na wierzchu", "Okno pozostaje nad innymi aplikacjami.");
        AddChoice(choices, CalculatorPipMode.InAppPanel, "Panel w aplikacji", "Kalkulator jest panelem w prawym dolnym rogu Abiturii.");
        root.Children.Add(UiFactory.Card(choices));
        root.Children.Add(_status);
        Content = UiFactory.PageScroll(root);
    }

    private void AddThemeChoice(StackPanel panel, AppThemeMode mode, string title, string description)
    {
        var choice = CreateChoice("application-theme", title, description, mode == _currentTheme);
        AutomationProperties.SetName(choice, $"Motyw: {title}");
        choice.IsCheckedChanged += (_, _) => ChangeTheme(choice, mode, title);
        panel.Children.Add(choice);
    }

    private void AddChoice(StackPanel panel, CalculatorPipMode mode, string title, string description)
    {
        var choice = CreateChoice("calculator-pip-mode", title, description, mode == _currentMode);
        AutomationProperties.SetName(choice, $"Tryb PiP: {title}");
        choice.IsCheckedChanged += async (_, _) => await SaveChoiceAsync(choice, mode);
        _pipChoices.Add(mode, choice);
        panel.Children.Add(choice);
    }

    private static RadioButton CreateChoice(string groupName, string title, string description, bool isChecked)
    {
        var content = new StackPanel { Spacing = 3 };
        content.Children.Add(new TextBlock { Text = title, FontSize = 17 });
        content.Children.Add(new TextBlock { Text = description, Classes = { "muted" }, TextWrapping = TextWrapping.Wrap });
        var choice = new RadioButton
        {
            GroupName = groupName,
            Content = content,
            IsChecked = isChecked,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        AutomationProperties.SetHelpText(choice, description);
        return choice;
    }

    private void ChangeTheme(RadioButton choice, AppThemeMode mode, string title)
    {
        if (choice.IsChecked != true || mode == _currentTheme) return;

        _setTheme(mode);
        _currentTheme = mode;
        ShowStatus($"Ustawiono motyw: {title}.", true);
    }

    private async Task SaveChoiceAsync(RadioButton choice, CalculatorPipMode mode)
    {
        if (_isSaving || choice.IsChecked != true || mode == _currentMode) return;

        _isSaving = true;
        SetChoicesEnabled(false);
        var previousMode = _currentMode;
        var saved = await _saveMode(mode);
        await Task.Yield();
        if (saved)
        {
            _currentMode = mode;
            ShowStatus("Zapisano tryb kalkulatora PiP.", true);
        }
        else
        {
            choice.IsChecked = false;
            _pipChoices[previousMode].IsChecked = true;
            ShowStatus("Nie udało się zapisać ustawienia dla aktywnego profilu.", false);
        }

        SetChoicesEnabled(true);
        _isSaving = false;
    }

    private void SetChoicesEnabled(bool enabled)
    {
        foreach (var choice in _pipChoices.Values) choice.IsEnabled = enabled;
    }

    private void ShowStatus(string message, bool success)
    {
        _status.Text = message;
        UiFactory.UseResource(_status, TextBlock.ForegroundProperty, success ? "SuccessBrush" : "ErrorBrush");
    }
}
