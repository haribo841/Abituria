using System;
using System.Collections.Generic;
using System.Linq;
using Abituria.Models;

namespace Abituria.Services;

public static class DiagramCatalogValidator
{
    private static readonly HashSet<string> PrimitiveTypes =
        new(["line", "polyline", "polygon", "ellipse", "arc", "text"], StringComparer.Ordinal);

    private static readonly HashSet<string> ColorTokens =
        new(["none", "primary", "muted", "accent", "danger", "success", "surface"], StringComparer.Ordinal);

    public static void Validate(DiagramCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        if (catalog.SchemaVersion != 1)
            throw new InvalidOperationException($"Nieobsługiwany schemat diagramów: {catalog.SchemaVersion}.");

        var duplicate = catalog.Diagrams.GroupBy(item => item.Id, StringComparer.Ordinal).FirstOrDefault(group => group.Count() != 1);
        if (duplicate is not null)
            throw new InvalidOperationException($"Identyfikator diagramu nie jest unikalny: {duplicate.Key}.");

        foreach (var diagram in catalog.Diagrams)
            ValidateDiagram(diagram);
    }

    private static void ValidateDiagram(DiagramDefinition diagram)
    {
        if (string.IsNullOrWhiteSpace(diagram.Id) || string.IsNullOrWhiteSpace(diagram.SourceId))
            throw new InvalidOperationException("Diagram musi mieć identyfikator i źródło.");
        if (string.IsNullOrWhiteSpace(diagram.AlternativeText))
            throw new InvalidOperationException($"Diagram '{diagram.Id}' nie ma opisu alternatywnego.");
        if (diagram.SourcePage < 0)
            throw new InvalidOperationException($"Diagram '{diagram.Id}' ma nieprawidłową stronę źródłową.");
        if (!IsPositiveFinite(diagram.Width) || !IsPositiveFinite(diagram.Height))
            throw new InvalidOperationException($"Diagram '{diagram.Id}' ma nieprawidłowy obszar rysowania.");
        if (diagram.Primitives.Count == 0)
            throw new InvalidOperationException($"Diagram '{diagram.Id}' nie zawiera prymitywów.");

        foreach (var primitive in diagram.Primitives)
            ValidatePrimitive(diagram.Id, primitive);
    }

    private static void ValidatePrimitive(string diagramId, DiagramPrimitive primitive)
    {
        ValidatePrimitiveStyle(diagramId, primitive);
        ValidatePrimitiveCoordinates(diagramId, primitive);
        ValidatePrimitivePointList(diagramId, primitive);
        ValidatePrimitiveRadii(diagramId, primitive);
        ValidatePrimitiveArc(diagramId, primitive);
        ValidatePrimitiveText(diagramId, primitive);
    }

    private static void ValidatePrimitiveStyle(string diagramId, DiagramPrimitive primitive)
    {
        if (!PrimitiveTypes.Contains(primitive.Type))
            throw new InvalidOperationException($"Diagram '{diagramId}' zawiera nieznany prymityw '{primitive.Type}'.");
        if (!ColorTokens.Contains(primitive.Stroke) || !ColorTokens.Contains(primitive.Fill))
            throw new InvalidOperationException($"Diagram '{diagramId}' zawiera nieznany token koloru.");
        if (!IsPositiveFinite(primitive.StrokeThickness) || !IsPositiveFinite(primitive.FontSize))
            throw new InvalidOperationException($"Diagram '{diagramId}' zawiera nieprawidłowy rozmiar.");
    }

    private static void ValidatePrimitiveCoordinates(string diagramId, DiagramPrimitive primitive)
    {
        if (!AllFinite(primitive.Points) || !AllFinite([
                primitive.X, primitive.Y, primitive.X2, primitive.Y2,
                primitive.RadiusX, primitive.RadiusY, primitive.StartAngle, primitive.SweepAngle]))
            throw new InvalidOperationException($"Diagram '{diagramId}' zawiera współrzędną NaN lub nieskończoność.");
    }

    private static void ValidatePrimitivePointList(string diagramId, DiagramPrimitive primitive)
    {
        if (primitive.Type is "polyline" or "polygon" && (primitive.Points.Count < 4 || primitive.Points.Count % 2 != 0))
            throw new InvalidOperationException($"Diagram '{diagramId}' zawiera nieprawidłową listę punktów.");
    }

    private static void ValidatePrimitiveRadii(string diagramId, DiagramPrimitive primitive)
    {
        if (primitive.Type is "ellipse" or "arc" && (!IsPositiveFinite(primitive.RadiusX) || !IsPositiveFinite(primitive.RadiusY)))
            throw new InvalidOperationException($"Diagram '{diagramId}' zawiera nieprawidłowy promień.");
    }

    private static void ValidatePrimitiveArc(string diagramId, DiagramPrimitive primitive)
    {
        if (primitive.Type == "arc" && primitive.SweepAngle == 0)
            throw new InvalidOperationException($"Diagram '{diagramId}' zawiera pusty łuk.");
    }

    private static void ValidatePrimitiveText(string diagramId, DiagramPrimitive primitive)
    {
        if (primitive.Type == "text" && string.IsNullOrWhiteSpace(primitive.Text))
            throw new InvalidOperationException($"Diagram '{diagramId}' zawiera pustą etykietę.");
    }

    private static bool IsPositiveFinite(double value) => double.IsFinite(value) && value > 0;

    private static bool AllFinite(IEnumerable<double> values) => values.All(double.IsFinite);
}
