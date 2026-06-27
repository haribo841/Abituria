using Abituria.Services;

namespace Abituria.Tests;

public sealed class QuadraticSolverTests
{
    private readonly QuadraticSolver _solver = new();

    [Fact]
    public void Returns_two_roots_and_all_forms()
    {
        var result = _solver.Solve("1", "-3", "2");
        Assert.True(result.Success);
        Assert.Contains("x₁ = 1", result.Summary);
        Assert.Contains("x₂ = 2", result.Summary);
        Assert.Contains(result.Steps, step => step.StartsWith("Postać kanoniczna"));
        Assert.Contains(result.Steps, step => step.StartsWith("Postać iloczynowa"));
    }

    [Fact]
    public void Handles_double_root_negative_delta_and_polish_decimal_separator()
    {
        Assert.Contains("Jedno miejsce zerowe", _solver.Solve("1", "2", "1").Summary);
        Assert.Contains("Brak miejsc zerowych", _solver.Solve("1", "0", "1").Summary);
        Assert.True(_solver.Solve("0,5", "-1", "0").Success);
    }

    [Fact]
    public void Rejects_invalid_coefficients()
    {
        Assert.False(_solver.Solve("0", "1", "2").Success);
        Assert.False(_solver.Solve("abc", "1", "2").Success);
    }
}
