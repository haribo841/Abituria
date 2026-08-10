using Abituria.Services;

namespace Abituria.Tests;

public sealed class PasswordHasherBoundaryTests
{
    [Fact]
    public void Password_validation_rejects_only_an_empty_password()
    {
        var error = Assert.Throws<ArgumentException>(() => PasswordHasher.ValidatePassword(string.Empty));

        Assert.Equal("Hasło nie może być puste.", error.Message);
    }

    [Fact]
    public void Password_validation_and_hashing_accept_one_character_and_the_technical_maximum()
    {
        var hasher = new PasswordHasher(iterations: 1_000);

        foreach (var length in new[] { 1, PasswordHasher.MaximumPasswordLength })
        {
            var password = new string('a', length);
            PasswordHasher.ValidatePassword(password);
            var credential = hasher.HashPassword(password);

            Assert.True(PasswordHasher.VerifyPassword(
                password,
                credential.Hash,
                credential.Salt,
                credential.Iterations));
        }
    }

    [Fact]
    public void Password_validation_rejects_values_above_the_technical_maximum()
    {
        var password = new string('a', PasswordHasher.MaximumPasswordLength + 1);

        var error = Assert.Throws<ArgumentException>(() => PasswordHasher.ValidatePassword(password));

        Assert.Contains(PasswordHasher.MaximumPasswordLength.ToString(), error.Message, StringComparison.Ordinal);
    }
}
