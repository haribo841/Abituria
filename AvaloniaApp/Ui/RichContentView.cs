using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Abituria.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media;
using CSharpMath.Avalonia;

namespace Abituria.Ui;

public sealed class RichContentView : UserControl
{
    private static readonly Regex InlineMath = new(@"\\\((.*?)\\\)", RegexOptions.Compiled);

    public RichContentView(IEnumerable<ContentBlock> blocks)
    {
        var stack = new StackPanel { Spacing = 14, HorizontalAlignment = HorizontalAlignment.Stretch };
        foreach (var block in blocks)
        {
            switch (block.Type)
            {
                case "richText" when !string.IsNullOrWhiteSpace(block.Text):
                    stack.Children.Add(CreateText(block.Text));
                    break;
                case "image" when !string.IsNullOrWhiteSpace(block.Asset):
                    stack.Children.Add(UiFactory.AssetImage(block.Asset, 920, 560));
                    break;
            }
        }
        Content = stack;
    }

    public static Control CreateText(string text)
    {
        var content = new StackPanel { Spacing = 8, HorizontalAlignment = HorizontalAlignment.Stretch };
        foreach (var line in text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
        {
            if (line.Length == 0)
            {
                content.Children.Add(new Border { Height = 5 });
                continue;
            }

            content.Children.Add(CreateInlineLine(line));
        }
        return content;
    }

    public static TextBlock CreateInlineLine(string line)
    {
        var textBlock = new TextBlock
        {
            FontSize = 17,
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ClipToBounds = false
        };

        var offset = 0;
        foreach (Match match in InlineMath.Matches(line))
        {
            AddPlainText(textBlock, line[offset..match.Index]);
            var latex = match.Groups[1].Value;
            if (IsListMarker(latex))
            {
                textBlock.Inlines!.Add(new Run("-"));
            }
            else
            {
                textBlock.Inlines!.Add(new InlineUIContainer(new MathView
                {
                    LaTeX = latex,
                    FontSize = 17,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    ClipToBounds = false
                })
                {
                    BaselineAlignment = BaselineAlignment.Baseline
                });
            }
            offset = match.Index + match.Length;
        }

        AddPlainText(textBlock, line[offset..]);
        return textBlock;
    }

    private static void AddPlainText(TextBlock content, string text)
    {
        if (text.Length > 0) content.Inlines!.Add(new Run(text));
    }

    private static bool IsListMarker(string latex) =>
        string.Equals(latex.Replace("\\:", string.Empty, StringComparison.Ordinal).Trim(), "-", StringComparison.Ordinal);
}
