using Abituria.Services;

namespace Abituria.Tests;

public sealed class ExerciseScratchpadSessionTests
{
    [Fact]
    public void Text_is_isolated_by_profile_and_exercise_until_session_ends()
    {
        var session = new ExerciseScratchpadSession();
        var firstProfile = Guid.NewGuid();
        var secondProfile = Guid.NewGuid();

        session.SetText(firstProfile, "exercise-1", "pierwszy zapis");
        session.SetText(firstProfile, "exercise-2", "drugi zapis");
        session.SetText(secondProfile, "exercise-1", "inny profil");

        Assert.Equal("pierwszy zapis", session.GetText(firstProfile, "exercise-1"));
        Assert.Equal("drugi zapis", session.GetText(firstProfile, "exercise-2"));
        Assert.Equal("inny profil", session.GetText(secondProfile, "exercise-1"));
        Assert.Equal(string.Empty, session.GetText(secondProfile, "exercise-2"));

        session.SetText(firstProfile, "exercise-1", string.Empty);
        Assert.Equal(string.Empty, session.GetText(firstProfile, "exercise-1"));
    }

    [Fact]
    public void Invalid_keys_are_rejected()
    {
        var session = new ExerciseScratchpadSession();

        Assert.Throws<ArgumentException>(() => session.GetText(Guid.Empty, "exercise"));
        Assert.Throws<ArgumentException>(() => session.GetText(Guid.NewGuid(), " "));
        Assert.Throws<ArgumentException>(() => session.SetText(Guid.Empty, "exercise", "tekst"));
    }
}
