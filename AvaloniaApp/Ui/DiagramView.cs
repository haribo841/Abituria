using System;
using System.Collections.Generic;
using Abituria.Models;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;

namespace Abituria.Ui;

public sealed class DiagramView : UserControl
{
    private const int ArcSegments = 32;

    public DiagramView(DiagramDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        AutomationProperties.SetName(this, definition.AlternativeText);

        var canvas = new Canvas
        {
            Width = definition.Width,
            Height = definition.Height,
            ClipToBounds = true
        };
        foreach (var primitive in definition.Primitives)
            AddPrimitive(canvas, primitive);

        Content = new Viewbox
        {
            Child = canvas,
            Stretch = Stretch.Uniform,
            StretchDirection = StretchDirection.DownOnly,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MaxHeight = 560
        };
    }

    private static void AddPrimitive(Canvas canvas, DiagramPrimitive primitive)
    {
        switch (primitive.Type)
        {
            case "line":
                AddLine(canvas, primitive.X, primitive.Y, primitive.X2, primitive.Y2, primitive);
                break;
            case "polyline":
                AddPolyline(canvas, primitive, false);
                break;
            case "polygon":
                AddPolyline(canvas, primitive, true);
                break;
            case "ellipse":
                AddEllipse(canvas, primitive);
                break;
            case "arc":
                AddArc(canvas, primitive);
                break;
            case "text":
                AddText(canvas, primitive);
                break;
            default:
                throw new InvalidOperationException($"Nieobsługiwany prymityw diagramu: {primitive.Type}.");
        }
    }

    private static void AddLine(Canvas canvas, double x1, double y1, double x2, double y2, DiagramPrimitive primitive)
    {
        var start = new Point(x1, y1);
        var end = new Point(x2, y2);
        var line = new Line { StartPoint = start, EndPoint = end };
        StyleShape(line, primitive);
        canvas.Children.Add(line);
        if (primitive.ArrowStart) AddArrowHead(canvas, end, start, primitive);
        if (primitive.ArrowEnd) AddArrowHead(canvas, start, end, primitive);
    }

    private static void AddArrowHead(Canvas canvas, Point from, Point tip, DiagramPrimitive primitive)
    {
        var angle = Math.Atan2(tip.Y - from.Y, tip.X - from.X);
        const double length = 14;
        const double spread = 0.55;
        AddArrowSegment(canvas, tip.X, tip.Y, angle + Math.PI - spread, length, primitive);
        AddArrowSegment(canvas, tip.X, tip.Y, angle + Math.PI + spread, length, primitive);
    }

    private static void AddArrowSegment(Canvas canvas, double x, double y, double angle, double length, DiagramPrimitive primitive)
    {
        var arrow = new Line
        {
            StartPoint = new Point(x, y),
            EndPoint = new Point(x + Math.Cos(angle) * length, y + Math.Sin(angle) * length)
        };
        StyleShape(arrow, primitive);
        canvas.Children.Add(arrow);
    }

    private static void AddPolyline(Canvas canvas, DiagramPrimitive primitive, bool closed)
    {
        var points = ToPoints(primitive.Points);
        Shape shape = closed
            ? new Polygon { Points = points }
            : new Polyline { Points = points };
        StyleShape(shape, primitive);
        canvas.Children.Add(shape);
    }

    private static void AddEllipse(Canvas canvas, DiagramPrimitive primitive)
    {
        var ellipse = new Ellipse
        {
            Width = primitive.RadiusX * 2,
            Height = primitive.RadiusY * 2
        };
        StyleShape(ellipse, primitive);
        Canvas.SetLeft(ellipse, primitive.X - primitive.RadiusX);
        Canvas.SetTop(ellipse, primitive.Y - primitive.RadiusY);
        canvas.Children.Add(ellipse);
    }

    private static void AddArc(Canvas canvas, DiagramPrimitive primitive)
    {
        var values = new List<double>((ArcSegments + 1) * 2);
        for (var index = 0; index <= ArcSegments; index++)
        {
            var fraction = index / (double)ArcSegments;
            var angle = (primitive.StartAngle + primitive.SweepAngle * fraction) * Math.PI / 180;
            values.Add(primitive.X + Math.Cos(angle) * primitive.RadiusX);
            values.Add(primitive.Y + Math.Sin(angle) * primitive.RadiusY);
        }

        var arc = new Polyline { Points = ToPoints(values) };
        StyleShape(arc, primitive);
        canvas.Children.Add(arc);
    }

    private static void AddText(Canvas canvas, DiagramPrimitive primitive)
    {
        var text = new TextBlock
        {
            Text = primitive.Text,
            FontSize = primitive.FontSize,
            FontFamily = FontFamily.Default
        };
        UiFactory.UseResource(text, TextBlock.ForegroundProperty, BrushResource(primitive.Stroke));
        Canvas.SetLeft(text, primitive.X);
        Canvas.SetTop(text, primitive.Y);
        canvas.Children.Add(text);
    }

    private static Points ToPoints(List<double> values)
    {
        var points = new Points();
        for (var index = 0; index < values.Count; index += 2)
            points.Add(new Point(values[index], values[index + 1]));
        return points;
    }

    private static void StyleShape(Shape shape, DiagramPrimitive primitive)
    {
        if (primitive.Stroke != "none")
            UiFactory.UseResource(shape, Shape.StrokeProperty, BrushResource(primitive.Stroke));
        if (primitive.Fill != "none")
            UiFactory.UseResource(shape, Shape.FillProperty, BrushResource(primitive.Fill));
        shape.StrokeThickness = primitive.StrokeThickness;
        if (primitive.Dashed)
            shape.StrokeDashArray = [8, 6];
    }

    private static string BrushResource(string token) => token switch
    {
        "primary" => "TextPrimaryBrush",
        "muted" => "TextMutedBrush",
        "accent" => "AccentBrush",
        "danger" => "DangerBrush",
        "success" => "SuccessBrush",
        "surface" => "SurfaceAltBrush",
        _ => throw new InvalidOperationException($"Nieobsługiwany token koloru: {token}.")
    };
}
