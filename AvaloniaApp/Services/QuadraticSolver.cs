using System;
using System.Collections.Generic;
using System.Globalization;

namespace Abituria.Services;

public sealed record QuadraticSolutionSection(string Title, IReadOnlyList<string> Lines);

public sealed record QuadraticSolution(bool Success, string Summary, IReadOnlyList<QuadraticSolutionSection> Sections);

public static class QuadraticSolver
{
    private const double Epsilon = 0.0000001;

    public static QuadraticSolution Solve(string? aText, string? bText, string? cText)
    {
        if (!TryRead(aText, out var a) || !TryRead(bText, out var b) || !TryRead(cText, out var c))
        {
            return new QuadraticSolution(false, "Współczynniki muszą być liczbami.", []);
        }
        if (Math.Abs(a) < Epsilon)
        {
            return new QuadraticSolution(false, "To nie jest funkcja kwadratowa, ponieważ a = 0.", []);
        }

        var delta = b * b - 4 * a * c;
        var p = -b / (2 * a);
        var q = -delta / (4 * a);
        var direction = a > 0 ? "w górę" : "w dół";
        var sections = new List<QuadraticSolutionSection>
        {
            new("Postać ogólna", [$"f(x) = {FormatPolynomial(a, b, c)}"]),
            new("Wyróżnik", [$"Δ = b² - 4ac = ({Format(b)})² - 4 · {Format(a)} · {Format(c)} = {Format(delta)}"])
        };

        if (delta > Epsilon)
        {
            var root = Math.Sqrt(delta);
            var x1 = (-b - root) / (2 * a);
            var x2 = (-b + root) / (2 * a);
            sections.Add(new("Miejsca zerowe",
            [
                $"x₁ = (-b - √Δ) / 2a = {Format(x1)}",
                $"x₂ = (-b + √Δ) / 2a = {Format(x2)}",
                $"Postać iloczynowa: f(x) = {FormatFactor(a)}{FormatBinomial(x1)}{FormatBinomial(x2)}"
            ]));
            AddVertexSection(sections, a, p, q, direction);
            return new QuadraticSolution(true, $"Dwa miejsca zerowe: x₁ = {Format(x1)}, x₂ = {Format(x2)}", sections);
        }
        if (Math.Abs(delta) <= Epsilon)
        {
            var x0 = -b / (2 * a);
            sections.Add(new("Miejsce zerowe",
            [
                $"x₀ = -b / 2a = {Format(x0)}",
                $"Postać iloczynowa: f(x) = {FormatFactor(a)}{FormatBinomial(x0)}²"
            ]));
            AddVertexSection(sections, a, p, q, direction);
            return new QuadraticSolution(true, $"Jedno miejsce zerowe: x₀ = {Format(x0)}", sections);
        }

        sections.Add(new("Miejsca zerowe", ["Delta jest ujemna, więc funkcja nie ma rzeczywistych miejsc zerowych ani postaci iloczynowej nad ℝ."]));
        AddVertexSection(sections, a, p, q, direction);
        return new QuadraticSolution(true, "Brak miejsc zerowych w zbiorze liczb rzeczywistych.", sections);
    }

    private static bool TryRead(string? value, out double result)
    {
        var normalized = (value ?? string.Empty).Trim().Replace(',', '.');
        return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
    }

    private static string Format(double value) => value.ToString("0.###", CultureInfo.GetCultureInfo("pl-PL"));

    private static void AddVertexSection(List<QuadraticSolutionSection> sections, double a, double p, double q, string direction)
    {
        var canonical = $"{FormatFactor(a)}{FormatBinomial(p)}²";
        if (q > Epsilon) canonical += $" + {Format(q)}";
        else if (q < -Epsilon) canonical += $" - {Format(Math.Abs(q))}";

        sections.Add(new("Wierzchołek i postać kanoniczna",
        [
            $"P = ({Format(p)}; {Format(q)})",
            $"f(x) = {canonical}",
            $"Ramiona paraboli są skierowane {direction}."
        ]));
    }

    private static string FormatPolynomial(double a, double b, double c)
    {
        var terms = new List<string> { FormatTerm(a, "x²", true) };
        if (Math.Abs(b) >= Epsilon) terms.Add(FormatTerm(b, "x", false));
        if (Math.Abs(c) >= Epsilon) terms.Add(FormatTerm(c, string.Empty, false));
        return string.Join(" ", terms);
    }

    private static string FormatTerm(double coefficient, string variable, bool first)
    {
        var sign = FormatSign(coefficient, first);
        var magnitude = Math.Abs(coefficient);
        var number = variable.Length > 0 && Math.Abs(magnitude - 1) < Epsilon ? string.Empty : Format(magnitude);
        if (first) return $"{sign}{number}{variable}";
        return $"{sign} {number}{variable}";
    }

    private static string FormatSign(double coefficient, bool first)
    {
        if (coefficient < 0) return "-";
        return first ? string.Empty : "+";
    }

    private static string FormatFactor(double value)
    {
        if (Math.Abs(value - 1) < Epsilon) return string.Empty;
        if (Math.Abs(value + 1) < Epsilon) return "-";
        return Format(value);
    }

    private static string FormatBinomial(double root)
    {
        if (Math.Abs(root) < Epsilon) return "x";
        return root > 0 ? $"(x - {Format(root)})" : $"(x + {Format(Math.Abs(root))})";
    }
}
