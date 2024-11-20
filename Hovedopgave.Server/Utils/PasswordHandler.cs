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

        //PBKDF2 hashing algorithm
        private static string ComputeHash(byte[] bytesToHash, byte[] salt)
        {
            var byteResult = new Rfc2898DeriveBytes(bytesToHash, salt, 10000);
            return Convert.ToBase64String(byteResult.GetBytes(24));
        }

        public static string GenerateSalt()
        {
            var bytes = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
