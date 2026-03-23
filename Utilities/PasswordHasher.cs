using System.Security.Cryptography;

namespace TestUpscaleApp.Utilities;

public static class PasswordHasher
{
    public static byte[] HashPassword(string password, byte[] salt, int iterations, HashAlgorithmName algorithm, int outputLength)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, algorithm))
        {
            return pbkdf2.GetBytes(outputLength);
        }
    }

    public static (byte[], byte[]) GeneratePasswordHash(string password)
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        byte[] hash = HashPassword(password, salt, 10000, HashAlgorithmName.SHA256, 20);
        return (hash, salt);
    }

    public static bool VerifyPassword(string password, byte[] hash, byte[] salt)
    {
        try
        {
            byte[] computedHash = HashPassword(password, salt, 10000, HashAlgorithmName.SHA256, 20);
            return computedHash.SequenceEqual(hash);
        }
        catch
        {
            return false;
        }
    }
}
