using Abituria.Models;
using Abituria.Services;
using Abituria.Ui;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CSharpMath.Avalonia;

namespace Abituria.Tests;

public sealed class RichContentDelimiterRegressionTests
{
    [AvaloniaFact]
    public void Balanced_line_uses_text_view_for_all_inline_formulas()
    {
        const string source = @"Tekst \(x+1\), potem \(y^2\).";

        var textView = Assert.IsType<TextView>(RichContentView.CreateInlineLine(source));

        Assert.Contains("x+1", textView.LaTeX, StringComparison.Ordinal);
        Assert.Contains("y^2", textView.LaTeX, StringComparison.Ordinal);
        Assert.True(string.IsNullOrWhiteSpace(textView.ErrorMessage), textView.ErrorMessage);
    }

    [AvaloniaTheory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("\r")]
    public void Delimiter_split_across_lines_never_reaches_text_view(string lineEnding)
    {
        var content = Assert.IsType<StackPanel>(
            RichContentView.CreateText($@"Tekst \(x^2.{lineEnding}\) Dalszy tekst."));

        var lines = content.Children.Cast<Control>().ToArray();
        Assert.Equal(2, lines.Length);
        Assert.All(lines, line => Assert.IsType<TextBlock>(line));
        Assert.Equal(@"Tekst \(x^2.", Assert.IsType<TextBlock>(lines[0]).Text);
        Assert.Equal(@"\) Dalszy tekst.", Assert.IsType<TextBlock>(lines[1]).Text);
    }

    [AvaloniaFact]
    public void Delimiter_split_across_content_blocks_never_reaches_text_view()
    {
        var view = new RichContentView(
        [
            new ContentBlock { Type = "richText", Text = @"Pierwszy \(x" },
            new ContentBlock { Type = "richText", Text = @"y\) drugi" }
        ]);

        var blocks = Assert.IsType<StackPanel>(view.Content).Children.Cast<StackPanel>().ToArray();
        Assert.Equal(2, blocks.Length);
        Assert.All(blocks, block => Assert.IsType<TextBlock>(Assert.Single(block.Children)));
        Assert.Equal(@"Pierwszy \(x", Assert.IsType<TextBlock>(Assert.Single(blocks[0].Children)).Text);
        Assert.Equal(@"y\) drugi", Assert.IsType<TextBlock>(Assert.Single(blocks[1].Children)).Text);
    }

    [AvaloniaTheory]
    [InlineData(@"\(x\) tekst \(y")]
    [InlineData(@"\)x\(")]
    [InlineData(@"\(outer \(nested\)\)")]
    [InlineData(@"\(\)")]
    [InlineData(@"\(   \)")]
    public void Malformed_line_is_kept_entirely_as_plain_text(string source)
    {
        var text = Assert.IsType<TextBlock>(RichContentView.CreateInlineLine(source));

        Assert.Equal(source, text.Text);
    }

    [AvaloniaFact]
    public void Adjacent_formulas_and_list_marker_keep_one_text_flow()
    {
        var textView = Assert.IsType<TextView>(
            RichContentView.CreateInlineLine(@"\(-\) \(x\)\(y\)"));

        Assert.StartsWith("-", textView.LaTeX);
        Assert.Contains(@"\(x\)\(y\)", textView.LaTeX, StringComparison.Ordinal);
        Assert.True(string.IsNullOrWhiteSpace(textView.ErrorMessage), textView.ErrorMessage);
    }

    [AvaloniaFact]
    public void Historical_multiline_exam_hint_is_now_one_balanced_renderer_line()
    {
        var hint = new ContentRepository().Exam.Exercises.Single(exercise => exercise.Number == 11).Hints[1];

        Assert.DoesNotContain('\r', hint);
        Assert.DoesNotContain('\n', hint);
        Assert.True(RichContentView.HasBalancedInlineMathDelimiters(hint));
        var textView = Assert.IsType<TextView>(RichContentView.CreateInlineLine(hint));
        Assert.True(string.IsNullOrWhiteSpace(textView.ErrorMessage), textView.ErrorMessage);
    }
}
