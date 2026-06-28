using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Abituria.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using CSharpMath.Avalonia;

namespace Abituria.Ui;

public sealed class RichContentView : UserControl
{
    private static readonly Regex InlineMath = new(@"\\\((.*?)\\\)([,.!?;:]?)", RegexOptions.Compiled);

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

            var offset = 0;
            foreach (Match match in InlineMath.Matches(line))
            {
                AddPlainText(content, line[offset..match.Index]);
                content.Children.Add(new MathView
                {
                    LaTeX = match.Groups[1].Value + match.Groups[2].Value,
                    FontSize = 17,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 2)
                });
                offset = match.Index + match.Length;
            }
            AddPlainText(content, line[offset..]);
        }
        return content;
    }

    private static void AddPlainText(Panel content, string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        content.Children.Add(new TextBlock
        {
            Text = text.Trim(),
            FontSize = 17,
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Stretch
        });
    }
}
