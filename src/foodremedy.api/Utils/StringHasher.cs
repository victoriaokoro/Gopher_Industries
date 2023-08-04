using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace foodremedy.api.Utils;

public static class StringHasher
{
    public static string GenerateSalt()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(128 / 8));
    }
    
    public static string Hash(string input, string salt)
    {
        byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(input, saltBytes, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8));
    }
}
