using System;
using System.Security.Cryptography;
using System.Text;

namespace CloudyWing.MoneyTrack.Infrastructure.Util {
    public static class CryptographyUtils {
        private const string Password = "MoneyTrack";

        public static string Encrypt(string str) {
            if (string.IsNullOrWhiteSpace(str)) {
                return str;
            }

            using (Aes aes = Aes.Create()) {
                (aes.Key, aes.IV) = GenerateKey(aes.KeySize, aes.BlockSize);

                using (ICryptoTransform encryptor = aes.CreateEncryptor()) {
                    byte[] strBytes = Encoding.UTF8.GetBytes(str);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public static string Decrypt(string str) {
            if (string.IsNullOrWhiteSpace(str)) {
                return str;
            }

            using (Aes aes = Aes.Create()) {
                (aes.Key, aes.IV) = GenerateKey(aes.KeySize, aes.BlockSize);
                byte[] strBytes = Convert.FromBase64String(str);

                using (ICryptoTransform decryptor = aes.CreateDecryptor()) {
                    byte[] outputData = decryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);
                    return Encoding.UTF8.GetString(outputData);
                }
            }
        }

        private static (byte[] Key, byte[] IV) GenerateKey(int keySize, int blockSize) {
            byte[] slat = Encoding.UTF8.GetBytes(Password);
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(Password, slat, 1000);
            return (deriveBytes.GetBytes(keySize / 8), deriveBytes.GetBytes(blockSize / 8));
        }
    }
}
