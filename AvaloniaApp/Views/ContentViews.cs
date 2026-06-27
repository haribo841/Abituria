using System;
using System.Collections.Generic;
using System.Linq;
using Abituria.Models;
using Abituria.Ui;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Abituria.Views;

public sealed class FormulaListView : UserControl
{
    public FormulaListView(IEnumerable<FormulaArticle> articles, Action<FormulaArticle> open)
    {
        var root = new StackPanel { Spacing = 14 };
        root.Children.Add(UiFactory.PageTitle("Tablice matematyczne", "Pełny zestaw 18 tematów ze starszych wersji projektu."));
        foreach (var article in articles.OrderBy(item => item.Order))
        {
            var button = new Button
            {
                Content = $"{article.Order}. {article.Title}",
                Classes = { "list" },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
            button.Click += (_, _) => open(article);
            root.Children.Add(button);
        }
        Content = UiFactory.PageScroll(root);
    }
}

public sealed class ArticleView : UserControl
{
    public ArticleView(string title, string subtitle, IReadOnlyList<ContentBlock> blocks, Action back)
    {
        var root = new StackPanel { Spacing = 18 };
        var backButton = new Button { Content = "← Wróć", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
        backButton.Click += (_, _) => back();
        root.Children.Add(backButton);
        root.Children.Add(UiFactory.PageTitle(title, subtitle));
        root.Children.Add(UiFactory.Card(new RichContentView(blocks), new Thickness(22)));
        Content = UiFactory.PageScroll(root);
    }
}

public sealed class ChapterListView : UserControl
{
    public ChapterListView(IEnumerable<ChapterArticle> chapters, Action<ChapterArticle> open)
    {
        var root = new StackPanel { Spacing = 14 };
        root.Children.Add(UiFactory.PageTitle("Działy", "Materiały działowe zachowane ze wszystkich wersji projektu."));
        foreach (var chapter in chapters)
        {
            var suffix = chapter.IsAvailable ? "" : " — treść w przygotowaniu";
            var button = new Button
            {
                Content = chapter.Title + suffix,
                Classes = { "list" },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
            button.Click += (_, _) => open(chapter);
            root.Children.Add(button);
        }
        Content = UiFactory.PageScroll(root);
    }
}

public sealed class PlaceholderView : UserControl
{
    public PlaceholderView(string title, string message, Action back)
    {
        var root = new StackPanel { Spacing = 18 };
        var backButton = new Button { Content = "← Wróć", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
        backButton.Click += (_, _) => back();
        root.Children.Add(backButton);
        root.Children.Add(UiFactory.PageTitle(title, "Zachowana pozycja ze starszej wersji aplikacji."));
        root.Children.Add(UiFactory.InfoBand("Status", message));
        Content = UiFactory.PageScroll(root);
    }
}
