using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    private static readonly Regex InlineMath = new(
        @"\\\((.*?)\\\)",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(250));

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
        var lines = text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
        for (var index = 0; index < lines.Length;)
        {
            if (TryCreateTable(lines, ref index, out var table))
            {
                content.Children.Add(table);
                continue;
            }

            var line = lines[index++];
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

    private static bool TryCreateTable(string[] lines, ref int index, out Control table)
    {
        table = null!;
        if (index + 1 >= lines.Length) return false;

        var header = SplitTableRow(lines[index]);
        if (header.Length < 2 || !IsTableSeparator(lines[index + 1], header.Length)) return false;

        var rows = new List<string[]> { header };
        index += 2;
        while (index < lines.Length && !string.IsNullOrWhiteSpace(lines[index]))
        {
            var row = SplitTableRow(lines[index]);
            if (row.Length != header.Length) break;
            rows.Add(row);
            index++;
        }

        table = CreateTable(rows);
        return true;
    }

    private static Grid CreateTable(List<string[]> rows)
    {
        var grid = new Grid { HorizontalAlignment = HorizontalAlignment.Stretch };
        for (var column = 0; column < rows[0].Length; column++)
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        for (var row = 0; row < rows.Count; row++)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            for (var column = 0; column < rows[row].Length; column++)
            {
                var text = CreateInlineLine(rows[row][column]);
                if (row == 0) text.FontWeight = FontWeight.SemiBold;
                var cell = new Border
                {
                    Child = text,
                    Padding = new Thickness(8, 6),
                    Background = row == 0 ? UiFactory.Brush("#F1F5F9") : Brushes.White,
                    BorderBrush = UiFactory.Brush("#D8DEE4"),
                    BorderThickness = new Thickness(0, 0, 1, 1)
                };
                Grid.SetColumn(cell, column);
                Grid.SetRow(cell, row);
                grid.Children.Add(cell);
            }
        }

        return grid;
    }

    private static bool IsTableSeparator(string line, int expectedColumns)
    {
        var cells = SplitTableRow(line);
        return cells.Length == expectedColumns &&
               cells.All(IsTableSeparatorCell);
    }

    private static bool IsTableSeparatorCell(string cell)
    {
        var marker = cell.Trim();
        if (marker.StartsWith(':')) marker = marker[1..];
        if (marker.EndsWith(':')) marker = marker[..^1];
        return marker.Length >= 3 && marker.All(character => character == '-');
    }

    private static string[] SplitTableRow(string line)
    {
        var row = line.Trim();
        if (row.StartsWith('|')) row = row[1..];
        if (row.EndsWith('|')) row = row[..^1];

        var cells = new List<string>();
        var current = new StringBuilder();
        var insideMath = false;
        var index = 0;
        while (index < row.Length)
        {
            var character = row[index];
            if (character == '\\' && index + 1 < row.Length)
            {
                var next = row[index + 1];
                if (next == '(') insideMath = true;
                else if (next == ')') insideMath = false;
                else if (next == '|' && !insideMath)
                {
                    current.Append('|');
                    index += 2;
                    continue;
                }
            }

            if (character == '|' && !insideMath)
            {
                cells.Add(current.ToString().Trim());
                current.Clear();
                index++;
                continue;
            }

            current.Append(character);
            index++;
        }

        cells.Add(current.ToString().Trim());
        return cells.ToArray();
    }

    private static void AddPlainText(TextBlock content, string text)
    {
        if (text.Length > 0) content.Inlines!.Add(new Run(text));
    }

    private static bool IsListMarker(string latex) =>
        string.Equals(latex.Replace("\\:", string.Empty, StringComparison.Ordinal).Trim(), "-", StringComparison.Ordinal);
}
