using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Library.Authentication;

public static class Encryptor
{
    private const int SaltSize = 16; // 128 bits
    private const int KeySize = 32;  // 256 bits
    private const int Iterations = 100_000;

    public static string Hash(string text)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] hash = KeyDerivation.Pbkdf2(
            text,
            salt,
            KeyDerivationPrf.HMACSHA256,
            Iterations,
            KeySize
        );

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string text, string storedHash)
    {
        var parts = storedHash.Split('.', 3);
        int iterations = int.Parse(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] expectedHash = Convert.FromBase64String(parts[2]);

        byte[] actualHash = KeyDerivation.Pbkdf2(
            text,
            salt,
            KeyDerivationPrf.HMACSHA256,
            iterations,
            expectedHash.Length
        );

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    public static Task<bool> VerifyAsync(string text, string storedHash, CancellationToken ct = default)
    {
        return Task.Run(() => Verify(text, storedHash), ct);
    }
}
