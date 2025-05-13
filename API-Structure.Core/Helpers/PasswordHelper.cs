using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace API_Structure.Core.Helpers
{
    public static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 64;
        private const int Iterations = 100000;
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA512,
                HashSize
                );
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";

        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var parts = hashedPassword.Split(':');
            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);

            var newHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA512,
                HashSize
                );

            return CryptographicOperations.FixedTimeEquals(hash, newHash);
        }

        public static bool IsValidPassword(string password)
        {
            string pattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*]).{8,}$";
            return Regex.IsMatch(password, pattern);
        }
    }
}
