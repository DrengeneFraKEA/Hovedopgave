using System.Security.Cryptography;
using System.Text;

namespace Hovedopgave.Server.Utils
{
    public class PasswordHandler
    {
        public static string GetHashedPassword(string password, string salt) 
        {
            return ComputeHash(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(salt));
        }
        
        public static string GenerateSaltAndHashedPassword(string password, out string salt) 
        {
            salt = GenerateSalt();
            return ComputeHash(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(salt));
        }

        // SHA-512 hashing algorithm with hash secret 
        private static string ComputeHash(byte[] bytesToHash, byte[] salt)
        {
            using (var sha512 = SHA512.Create())
            {
                // hash secret 
                //string hashSecret = Environment.GetEnvironmentVariable("HASH_SECRET");
                string hashSecret = "12345678";
                byte[] secretBytes = Encoding.UTF8.GetBytes(hashSecret);
                // Combine password bytes, salt, and secret
                byte[] combinedBytes = new byte[bytesToHash.Length + salt.Length + secretBytes.Length];
                Buffer.BlockCopy(bytesToHash, 0, combinedBytes, 0, bytesToHash.Length);
                Buffer.BlockCopy(salt, 0, combinedBytes, bytesToHash.Length, salt.Length);
                Buffer.BlockCopy(secretBytes, 0, combinedBytes, bytesToHash.Length + salt.Length, secretBytes.Length);
                // Compute hash
                byte[] hashBytes = sha512.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static string GenerateSalt()
        {
            var bytes = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder result = new StringBuilder();
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    result.Append(validChars[(int)(num % (uint)validChars.Length)]);
                }
            }
            return result.ToString();
        }

    }
}
