using System.Runtime.InteropServices;
using Abituria.Services;
using Abituria.Ui;
using Abituria.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CSharpMath.Avalonia;

namespace Abituria.Tests;

public sealed class Discussion10VisualRegressionTests
{
    [AvaloniaFact]
    public void Mathematical_list_uses_one_inline_text_flow_per_source_line()
    {
        var sourceLines = ReadMathematicalListLines();
        var content = Assert.IsType<StackPanel>(RichContentView.CreateText(string.Join('\n', sourceLines)));
        var renderedLines = content.Children.Cast<Control>().ToArray();

        Assert.Equal(sourceLines.Length, renderedLines.Length);
        foreach (var line in renderedLines)
        {
            var textView = Assert.IsType<TextView>(line);
            Assert.StartsWith("-", textView.LaTeX);
            Assert.DoesNotContain("\\(-\\)", textView.LaTeX, StringComparison.Ordinal);
            Assert.Contains("\\(", textView.LaTeX, StringComparison.Ordinal);
            Assert.True(string.IsNullOrWhiteSpace(textView.ErrorMessage), textView.ErrorMessage);
        }
    }

    [AvaloniaFact]
    public void Inline_formulas_are_arranged_inside_their_text_line_and_wrap_at_narrow_width()
    {
        var content = RichContentView.CreateText(string.Join('\n', ReadMathematicalListLines()));
        var window = ShowInWindow(content, 320, 420);
        try
        {
            var renderedLines = content.GetVisualDescendants().OfType<TextView>().ToArray();

            Assert.NotEmpty(renderedLines);
            Assert.Contains(renderedLines, line => line.Bounds.Height > 30d);
            foreach (var line in renderedLines)
            {
                Assert.InRange(line.Bounds.Width, 1d, content.Bounds.Width + 0.1d);
                Assert.True(string.IsNullOrWhiteSpace(line.ErrorMessage), line.ErrorMessage);
            }
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaTheory]
    [InlineData(960, 640, "discussion-10-math-list-960x640.png")]
    [InlineData(1280, 820, "discussion-10-math-list-1280x820.png")]
    public void Mathematical_list_matches_the_reviewed_visual_baseline(int width, int height, string baselineFileName)
    {
        var content = RichContentView.CreateText(string.Join('\n', ReadMathematicalListLines()));
        var window = ShowInWindow(content, width, height);
        try
        {
            using var actual = Assert.IsType<WriteableBitmap>(window.CaptureRenderedFrame());
            var baselinePath = Path.Combine(FindRepositoryRoot(), "tests", "Abituria.Tests", "VisualBaselines", baselineFileName);

            if (string.Equals(Environment.GetEnvironmentVariable("UPDATE_VISUAL_BASELINES"), "1", StringComparison.Ordinal))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(baselinePath)!);
                actual.Save(baselinePath);
                return;
            }

            Assert.True(File.Exists(baselinePath), $"Brak obrazu wzorcowego: {baselinePath}");
            using var expected = new Bitmap(baselinePath);
            AssertImagesMatch(expected, actual, 0.03d, 18);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Full_calculator_history_can_be_reached_with_the_vertical_scroll_viewer()
    {
        var repository = new ContentRepository();
        var session = new CalculatorSession(new ExpressionCalculator());
        for (var value = 1; value <= CalculatorSession.HistoryLimit; value++)
            Assert.True(session.Calculate($"{value}+1").Success);

        var view = new GeneralCalculatorView(session, repository.UiCopy, () => { });
        var window = ShowInWindow(view, 960, 640);
        try
        {
            var scroll = view.GetVisualDescendants().OfType<ScrollViewer>().Single(item => item.Content is StackPanel);

            Assert.Equal(Avalonia.Controls.Primitives.ScrollBarVisibility.Auto, scroll.VerticalScrollBarVisibility);
            Assert.True(scroll.Extent.Height > scroll.Viewport.Height);
            scroll.Offset = new Vector(0d, scroll.Extent.Height - scroll.Viewport.Height);
            Dispatcher.UIThread.RunJobs();
            Assert.True(scroll.Offset.Y > 0d);
        }
        finally
        {
            window.Close();
        }
    }

    private static string[] ReadMathematicalListLines()
    {
        var article = new ContentRepository().Formulas.Articles.Single(item => item.Id == "formula-2");
        var text = Assert.Single(article.Blocks).Text!;
        return text.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Split('\n')
            .Where(line => line.StartsWith("\\(-\\)", StringComparison.Ordinal))
            .ToArray();
    }

    private static Window ShowInWindow(Control content, double width, double height)
    {
        var window = new Window
        {
            Width = width,
            Height = height,
            Background = Brushes.White,
            Content = new Border
            {
                Padding = new Thickness(24),
                Background = Brushes.White,
                Child = content
            }
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static void AssertImagesMatch(Bitmap expected, Bitmap actual, double maximumDifferentPixelRatio, byte channelTolerance)
    {
        Assert.Equal(expected.PixelSize, actual.PixelSize);
        var expectedPixels = ReadPixels(expected);
        var actualPixels = ReadPixels(actual);
        var comparisonArea = FindInkBounds(expectedPixels, expected.PixelSize, 8);
        var expectedInkPixels = CountInkPixels(expectedPixels, expected.PixelSize, comparisonArea);
        var actualInkPixels = CountInkPixels(actualPixels, actual.PixelSize, comparisonArea);
        var differentPixels = 0;

        for (var y = comparisonArea.Y; y < comparisonArea.Bottom; y++)
            for (var x = comparisonArea.X; x < comparisonArea.Right; x++)
            {
                var index = ((y * expected.PixelSize.Width) + x) * 4;
                var pixelIsDifferent = false;
                for (var channel = 0; channel < 4; channel++)
                {
                    if (Math.Abs(expectedPixels[index + channel] - actualPixels[index + channel]) <= channelTolerance) continue;
                    pixelIsDifferent = true;
                    break;
                }

                if (pixelIsDifferent) differentPixels++;
            }

        Assert.InRange(actualInkPixels, expectedInkPixels * 0.95d, expectedInkPixels * 1.05d);
        var pixelCount = comparisonArea.Width * comparisonArea.Height;
        Assert.True(
            differentPixels / pixelCount <= maximumDifferentPixelRatio,
            $"Obraz różni się na {differentPixels / pixelCount:P2} pikseli, limit wynosi {maximumDifferentPixelRatio:P2}.");
    }

    private static PixelRect FindInkBounds(byte[] pixels, PixelSize size, int margin)
    {
        var left = size.Width;
        var top = size.Height;
        var right = 0;
        var bottom = 0;

        for (var y = 0; y < size.Height; y++)
            for (var x = 0; x < size.Width; x++)
            {
                var index = ((y * size.Width) + x) * 4;
                if (!IsInk(pixels, index)) continue;
                left = Math.Min(left, x);
                top = Math.Min(top, y);
                right = Math.Max(right, x + 1);
                bottom = Math.Max(bottom, y + 1);
            }

        Assert.True(right > left && bottom > top, "Obraz wzorcowy nie zawiera widocznej treści.");
        return new PixelRect(
            Math.Max(0, left - margin),
            Math.Max(0, top - margin),
            Math.Min(size.Width, right + margin) - Math.Max(0, left - margin),
            Math.Min(size.Height, bottom + margin) - Math.Max(0, top - margin));
    }

    private static int CountInkPixels(byte[] pixels, PixelSize size, PixelRect area)
    {
        var count = 0;
        for (var y = area.Y; y < area.Bottom; y++)
            for (var x = area.X; x < area.Right; x++)
            {
                var index = ((y * size.Width) + x) * 4;
                if (IsInk(pixels, index)) count++;
            }

        return count;
    }

    private static bool IsInk(byte[] pixels, int index) =>
        pixels[index] < 245 || pixels[index + 1] < 245 || pixels[index + 2] < 245;

    private static byte[] ReadPixels(Bitmap bitmap)
    {
        const int bytesPerPixel = 4;
        var stride = bitmap.PixelSize.Width * bytesPerPixel;
        var pixels = new byte[stride * bitmap.PixelSize.Height];
        var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(
                new PixelRect(bitmap.PixelSize),
                handle.AddrOfPinnedObject(),
                pixels.Length,
                stride);
            return pixels;
        }
        finally
        {
            handle.Free();
        }
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Abituria.sln"))) return current.FullName;
            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Nie znaleziono katalogu repozytorium Abituria.");
    }
}
