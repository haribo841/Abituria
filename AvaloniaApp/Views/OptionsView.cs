using System;
using System.Threading.Tasks;
using Abituria.Models;
using Abituria.Ui;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Abituria.Views;

public sealed class OptionsView : UserControl
{
    private readonly Dictionary<CalculatorPipMode, RadioButton> _choices = [];
    private readonly TextBlock _status = new() { TextWrapping = TextWrapping.Wrap };
    private readonly Func<CalculatorPipMode, Task<bool>> _saveMode;
    private CalculatorPipMode _currentMode;
    private bool _isSaving;

    public OptionsView(
        CalculatorPipMode currentMode,
        Func<CalculatorPipMode, Task<bool>> saveMode)
    {
        _currentMode = currentMode;
        _saveMode = saveMode ?? throw new ArgumentNullException(nameof(saveMode));
        AutomationProperties.SetLiveSetting(_status, AutomationLiveSetting.Polite);
        AutomationProperties.SetName(_status, "Stan zapisu opcji");

        var root = new StackPanel { Spacing = 18 };
        root.Children.Add(UiFactory.PageTitle(
            "Opcje",
            "Ustawienia są zapisywane osobno dla aktywnego profilu."));
        root.Children.Add(UiFactory.InfoBand(
            "Kalkulator Picture in Picture",
            "Wybierz sposób wyświetlania kompaktowego kalkulatora. Zmiana jest stosowana również do już otwartego PiP."));

        var choices = new StackPanel { Spacing = 10 };
        AddChoice(choices, CalculatorPipMode.OwnedWindow, "Nad Abiturią", "Okno pozostaje nad głównym oknem aplikacji.");
        AddChoice(choices, CalculatorPipMode.AlwaysOnTopWindow, "Zawsze na wierzchu", "Okno pozostaje nad innymi aplikacjami.");
        AddChoice(choices, CalculatorPipMode.InAppPanel, "Panel w aplikacji", "Kalkulator jest panelem w prawym dolnym rogu Abiturii.");
        root.Children.Add(UiFactory.Card(choices));
        root.Children.Add(_status);
        Content = UiFactory.PageScroll(root);
    }

    private void AddChoice(StackPanel panel, CalculatorPipMode mode, string title, string description)
    {
        var content = new StackPanel { Spacing = 3 };
        content.Children.Add(new TextBlock { Text = title, FontSize = 17 });
        content.Children.Add(new TextBlock { Text = description, Classes = { "muted" }, TextWrapping = TextWrapping.Wrap });
        var choice = new RadioButton
        {
            GroupName = "calculator-pip-mode",
            Content = content,
            IsChecked = mode == _currentMode,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        AutomationProperties.SetName(choice, $"Tryb PiP: {title}");
        choice.IsCheckedChanged += async (_, _) => await SaveChoiceAsync(choice, mode);
        _choices.Add(mode, choice);
        panel.Children.Add(choice);
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
            _choices[previousMode].IsChecked = true;
            ShowStatus("Nie udało się zapisać ustawienia dla aktywnego profilu.", false);
        }

        SetChoicesEnabled(true);
        _isSaving = false;
    }

    private void SetChoicesEnabled(bool enabled)
    {
        foreach (var choice in _choices.Values) choice.IsEnabled = enabled;
    }

    private void ShowStatus(string message, bool success)
    {
        _status.Text = message;
        UiFactory.UseResource(_status, TextBlock.ForegroundProperty, success ? "SuccessBrush" : "ErrorBrush");
    }
}
