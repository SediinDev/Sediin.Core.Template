using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Sediin.Core.Mvc.Helpers.Security
{
    public static class CryptoHelper
    {
        private static readonly string encryptionKey = "my_super_secure_key_1234"; // Deve essere di 16, 24 o 32 byte per AES

        public static string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);
            using (Aes aes = Aes.Create())
            {
                using (var key = new Rfc2898DeriveBytes(encryptionKey, Encoding.ASCII.GetBytes("SALT1234")))
                {
                    aes.Key = key.GetBytes(32);
                    aes.IV = key.GetBytes(16);
                }

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                using (var key = new Rfc2898DeriveBytes(encryptionKey, Encoding.ASCII.GetBytes("SALT1234")))
                {
                    aes.Key = key.GetBytes(32);
                    aes.IV = key.GetBytes(16);
                }

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
    }
}
