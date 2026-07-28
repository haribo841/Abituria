using Abituria.Services;

namespace Abituria.Tests;

public sealed class PasswordHasherBoundaryTests
{
    [Theory]
    [InlineData(PasswordHasher.MinimumPasswordLength - 1)]
    [InlineData(PasswordHasher.MaximumPasswordLength + 1)]
    public void Password_validation_rejects_both_length_boundaries(int length)
    {
        var password = new string('a', length);

        var error = Assert.Throws<ArgumentException>(() => PasswordHasher.ValidatePassword(password));

        Assert.Contains(PasswordHasher.MinimumPasswordLength.ToString(), error.Message, StringComparison.Ordinal);
        Assert.Contains(PasswordHasher.MaximumPasswordLength.ToString(), error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Password_validation_and_hashing_accept_exact_boundaries()
    {
        var hasher = new PasswordHasher(iterations: 1_000);

        foreach (var length in new[] { PasswordHasher.MinimumPasswordLength, PasswordHasher.MaximumPasswordLength })
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
}
