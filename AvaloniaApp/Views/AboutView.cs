using Abituria.Services;
using Abituria.Ui;
using Avalonia.Controls;

namespace Abituria.Views;

public sealed class AboutView : UserControl
{
    public AboutView(AppBuildInfo buildInfo)
    {
        var root = new StackPanel { Spacing = 16 };
        root.Children.Add(UiFactory.PageTitle(
            "O programie",
            "Informacje o bieżącej kompilacji i projekcie Abituria."));
        root.Children.Add(UiFactory.InfoBand("Wersja", buildInfo.Version));
        root.Children.Add(UiFactory.InfoBand("Commit", buildInfo.Commit));
        root.Children.Add(UiFactory.InfoBand("Licencja", buildInfo.License));
        root.Children.Add(UiFactory.InfoBand("Autor i opiekun", buildInfo.Author));
        root.Children.Add(UiFactory.InfoBand("Repozytorium", buildInfo.RepositoryUrl));
        Content = UiFactory.PageScroll(root);
    }
}
