using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Abituria.Services;

public sealed record PasswordCredential(byte[] Hash, byte[] Salt, int Iterations);

public sealed class PasswordHasher(int iterations = PasswordHasher.DefaultIterations)
{
    public const int DefaultIterations = 600_000;
    public const int MinimumPasswordLength = 15;
    public const int MaximumPasswordLength = 128;
    private const int SaltLength = 16;
    private const int HashLength = 32;

    public int Iterations { get; } = iterations;

    public PasswordCredential HashPassword(string password)
    {
        ValidatePassword(password);
        var salt = RandomNumberGenerator.GetBytes(SaltLength);
        var hash = Derive(password, salt, Iterations);
        return new PasswordCredential(hash, salt, Iterations);
    }

    public static bool VerifyPassword(string password, byte[] expectedHash, byte[] salt, int iterations)
    {
        if (password.Length > MaximumPasswordLength) return false;
        var actual = Derive(password, salt, iterations);
        return CryptographicOperations.FixedTimeEquals(actual, expectedHash);
    }

    public static void ValidatePassword(string password)
    {
        if (password.Length < MinimumPasswordLength || password.Length > MaximumPasswordLength)
        {
            throw new ArgumentException($"Hasło musi mieć od {MinimumPasswordLength} do {MaximumPasswordLength} znaków.");
        }
    }

    public static string GenerateRecoveryCode()
    {
        var raw = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
        return string.Join('-', Enumerable.Range(0, 4).Select(index => raw.Substring(index * 8, 8)));
    }

    public static byte[] HashRecoveryCode(string recoveryCode)
    {
        var normalized = recoveryCode.Replace("-", string.Empty, StringComparison.Ordinal).Trim().ToUpperInvariant();
        return SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
    }

    public static bool VerifyRecoveryCode(string recoveryCode, byte[] expectedHash)
    {
        var actual = HashRecoveryCode(recoveryCode);
        return CryptographicOperations.FixedTimeEquals(actual, expectedHash);
    }

    private static byte[] Derive(string password, byte[] salt, int iterations) =>
        Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, HashLength);
}
