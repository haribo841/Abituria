using System;
using System.Collections.Generic;
using System.Linq;
using Abituria.Models;
using Abituria.Ui;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Abituria.Views;

public sealed class FormulaListView : UserControl
{
    public FormulaListView(FormulaCatalog catalog, Action<FormulaArticle> open)
    {
        var root = new StackPanel { Spacing = 14 };
        root.Children.Add(UiFactory.PageTitle("Tablice matematyczne", "Pełny zestaw 18 tematów ze starszych wersji projektu."));
        if (catalog.Introduction.Count > 0)
            root.Children.Add(UiFactory.Card(new RichContentView(catalog.Introduction)));
        foreach (var article in catalog.Articles.OrderBy(item => item.Order))
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
    public ChapterListView(ChapterCatalog catalog, Action<ChapterArticle> open)
    {
        var root = new StackPanel { Spacing = 14 };
        root.Children.Add(UiFactory.PageTitle("Działy", "Materiały działowe zachowane ze wszystkich wersji projektu."));
        if (catalog.Introduction.Count > 0)
            root.Children.Add(UiFactory.Card(new RichContentView(catalog.Introduction)));
        foreach (var chapter in catalog.Chapters)
        {
            var suffix = chapter.IsAvailable ? "" : " - treść w przygotowaniu";
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
    public PlaceholderView(string title, string message, IReadOnlyList<ContentBlock> blocks, Action back, Action? openRoadmap = null)
    {
        var root = new StackPanel { Spacing = 18 };
        var backButton = new Button { Content = "← Wróć", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
        backButton.Click += (_, _) => back();
        root.Children.Add(backButton);
        root.Children.Add(UiFactory.PageTitle(title, "Zachowana pozycja ze starszej wersji aplikacji."));
        root.Children.Add(UiFactory.InfoBand("Status", message));
        if (blocks.Count > 0) root.Children.Add(UiFactory.Card(new RichContentView(blocks)));
        if (openRoadmap is not null)
        {
            var roadmap = new Button { Content = "Zobacz w planie rozwoju", Classes = { "ghost" }, HorizontalAlignment = HorizontalAlignment.Left };
            roadmap.Click += (_, _) => openRoadmap();
            root.Children.Add(roadmap);
        }
        Content = UiFactory.PageScroll(root);
    }
}

public sealed class RoadmapView : UserControl
{
    public RoadmapView(RoadmapCatalog catalog, string? focusedItemId)
    {
        var root = new StackPanel { Spacing = 18 };
        root.Children.Add(UiFactory.PageTitle("Plan rozwoju", "Stan migracji funkcji i treści ze wszystkich wersji projektu."));
        if (catalog.Introduction.Count > 0)
            root.Children.Add(UiFactory.Card(new RichContentView(catalog.Introduction)));

        AddGroup(root, catalog, RoadmapStatus.Migrated, "Przeniesione", "SuccessBrush", focusedItemId);
        AddGroup(root, catalog, RoadmapStatus.Planned, "Zaplanowane", "WarningBrush", focusedItemId);
        AddGroup(root, catalog, RoadmapStatus.Superseded, "Zastąpione", "TextMutedBrush", focusedItemId);
        Content = UiFactory.PageScroll(root);
    }

    private static void AddGroup(StackPanel root, RoadmapCatalog catalog, RoadmapStatus status, string title, string colorResource, string? focusedItemId)
    {
        root.Children.Add(new TextBlock { Text = title, Classes = { "h2" }, Margin = new Thickness(0, 8, 0, 0) });
        var items = catalog.Items.Where(item => item.Status == status)
            .OrderByDescending(item => string.Equals(item.Id, focusedItemId, StringComparison.Ordinal))
            .ThenBy(item => item.Title);
        foreach (var item in items)
        {
            var panel = new StackPanel { Spacing = 5 };
            panel.Children.Add(new TextBlock { Text = item.Title, FontSize = 18, FontWeight = Avalonia.Media.FontWeight.SemiBold, TextWrapping = TextWrapping.Wrap });
            panel.Children.Add(new TextBlock { Text = item.Summary, Classes = { "muted" } });
            var source = new TextBlock
            {
                Text = $"Obszar: {item.Context} · Źródła: {string.Join(", ", item.SourceRefs)}",
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap
            };
            UiFactory.UseResource(source, TextBlock.ForegroundProperty, "TextMutedBrush");
            panel.Children.Add(source);
            var focused = string.Equals(item.Id, focusedItemId, StringComparison.Ordinal);
            var card = UiFactory.Card(panel, new Thickness(16), focused ? "WarningSurfaceBrush" : "SurfaceBrush");
            UiFactory.UseResource(card, Border.BorderBrushProperty, focused ? colorResource : "BorderBrush");
            card.BorderThickness = new Thickness(focused ? 2 : 1);
            root.Children.Add(card);
        }
    }
}
