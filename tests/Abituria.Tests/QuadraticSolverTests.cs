using Abituria.Services;

namespace Abituria.Tests;

public sealed class QuadraticSolverTests
{
    [Fact]
    public void Returns_two_roots_and_all_forms()
    {
        var result = QuadraticSolver.Solve("1", "-3", "2");
        Assert.True(result.Success);
        Assert.Contains("x₁ = 1", result.Summary);
        Assert.Contains("x₂ = 2", result.Summary);
        Assert.Contains(result.Sections, section => section.Title == "Wierzchołek i postać kanoniczna");
        Assert.Contains(result.Sections.SelectMany(section => section.Lines), line => line == "Postać iloczynowa: f(x) = (x - 1)(x - 2)");
        Assert.Contains(result.Sections.SelectMany(section => section.Lines), line => line == "f(x) = x² - 3x + 2");
    }

    [Fact]
    public void Handles_double_root_negative_delta_and_polish_decimal_separator()
    {
        Assert.Contains("Jedno miejsce zerowe", QuadraticSolver.Solve("1", "2", "1").Summary);
        Assert.Contains("Brak miejsc zerowych", QuadraticSolver.Solve("1", "0", "1").Summary);
        Assert.True(QuadraticSolver.Solve("0,5", "-1", "0").Success);
    }

    [Fact]
    public void Rejects_invalid_coefficients()
    {
        Assert.False(QuadraticSolver.Solve("0", "1", "2").Success);
        Assert.False(QuadraticSolver.Solve("abc", "1", "2").Success);
    }

    [Fact]
    public void Formats_signs_unit_coefficients_and_negative_vertex_without_nested_parentheses()
    {
        var result = QuadraticSolver.Solve("-1", "-3", "-2");
        var lines = result.Sections.SelectMany(section => section.Lines).ToArray();

        Assert.Contains("f(x) = -x² - 3x - 2", lines);
        Assert.DoesNotContain(lines, line => line.Contains("+ -", StringComparison.Ordinal));
        Assert.DoesNotContain(lines, line => line.Contains("- (", StringComparison.Ordinal));
    }
}
