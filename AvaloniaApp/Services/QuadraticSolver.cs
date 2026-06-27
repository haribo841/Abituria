using System;
using System.Collections.Generic;
using System.Globalization;

namespace Abituria.Services;

public sealed record QuadraticSolution(bool Success, string Summary, IReadOnlyList<string> Steps);

public sealed class QuadraticSolver
{
    private const double Epsilon = 0.0000001;

    public QuadraticSolution Solve(string? aText, string? bText, string? cText)
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
        var steps = new List<string>
        {
            $"Postać ogólna: f(x) = {Term(a, "x²", true)} {Term(b, "x", false)} {Term(c, string.Empty, false)}",
            $"Delta: Δ = b² - 4ac = ({Format(b)})² - 4 · {Format(a)} · {Format(c)} = {Format(delta)}",
            $"Wierzchołek: P = ({Format(p)}; {Format(q)})",
            $"Postać kanoniczna: f(x) = {Format(a)}(x - ({Format(p)}))² + {Format(q)}",
            $"Ramiona paraboli są skierowane {direction}."
        };

        if (delta > Epsilon)
        {
            var root = Math.Sqrt(delta);
            var x1 = (-b - root) / (2 * a);
            var x2 = (-b + root) / (2 * a);
            steps.Add($"x₁ = (-b - √Δ) / 2a = {Format(x1)}");
            steps.Add($"x₂ = (-b + √Δ) / 2a = {Format(x2)}");
            steps.Add($"Postać iloczynowa: f(x) = {Format(a)}(x - ({Format(x1)}))(x - ({Format(x2)}))");
            return new QuadraticSolution(true, $"Dwa miejsca zerowe: x₁ = {Format(x1)}, x₂ = {Format(x2)}", steps);
        }
        if (Math.Abs(delta) <= Epsilon)
        {
            var x0 = -b / (2 * a);
            steps.Add($"x₀ = -b / 2a = {Format(x0)}");
            steps.Add($"Postać iloczynowa: f(x) = {Format(a)}(x - ({Format(x0)}))²");
            return new QuadraticSolution(true, $"Jedno miejsce zerowe: x₀ = {Format(x0)}", steps);
        }

        steps.Add("Delta jest ujemna, więc funkcja nie ma rzeczywistych miejsc zerowych ani postaci iloczynowej nad ℝ.");
        return new QuadraticSolution(true, "Brak miejsc zerowych w zbiorze liczb rzeczywistych.", steps);
    }

    private static bool TryRead(string? value, out double result)
    {
        var normalized = (value ?? string.Empty).Trim().Replace(',', '.');
        return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
    }

    private static string Format(double value) => value.ToString("0.###", CultureInfo.GetCultureInfo("pl-PL"));

    private static string Term(double coefficient, string variable, bool first)
    {
        var sign = coefficient < 0 ? "-" : first ? string.Empty : "+";
        var magnitude = Math.Abs(coefficient);
        return $"{sign} {Format(magnitude)}{variable}".Trim();
    }
}
