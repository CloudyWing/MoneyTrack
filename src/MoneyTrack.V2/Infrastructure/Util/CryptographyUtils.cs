using System;
using System.Security.Cryptography;
using System.Text;

namespace CloudyWing.MoneyTrack.Infrastructure.Util {
    /// <summary>
    /// Provides utility methods for cryptography operations.
    /// </summary>
    public static class CryptographyUtils {
        private const string PasswordParameter = "Password";
        private const string SaltParameter = "SaltParameter";

        /// <summary>
        /// Encrypts the specified string using AES encryption.
        /// </summary>
        /// <param name="input">The string to encrypt.</param>
        /// <param name="replaceIllegalUrlChars">Whether to replace illegal URL characters in the output.</param>
        /// <returns>The encrypted string.</returns>
        public static string Encrypt(string input, bool replaceIllegalUrlChars = false) {
            if (string.IsNullOrEmpty(input)) {
                return input;
            }

            using (Aes aes = Aes.Create()) {
                (aes.Key, aes.IV) = GenerateKeyFromPassword(aes.KeySize, aes.BlockSize);
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);

                using (ICryptoTransform encryptor = aes.CreateEncryptor()) {
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    string encryptedString = Convert.ToBase64String(encryptedBytes);

                    if (replaceIllegalUrlChars) {
                        encryptedString = ReplaceIllegalUrlChars(encryptedString);
                    }

                    return encryptedString;

                }
            }
        }

        /// <summary>
        /// Decrypts the specified encrypted string using AES decryption.
        /// </summary>
        /// <param name="encryptedInput">The encrypted string to decrypt.</param>
        /// <param name="restoreIllegalUrlChars">Whether to replace illegal URL characters in the input.</param>
        /// <returns>The decrypted string.</returns>
        public static string Decrypt(string encryptedInput, bool restoreIllegalUrlChars = false) {
            if (string.IsNullOrEmpty(encryptedInput)) {
                return encryptedInput;
            }

            if (restoreIllegalUrlChars) {
                encryptedInput = RestoreIllegalUrlChars(encryptedInput);
            }

            using (Aes aes = Aes.Create()) {
                (aes.Key, aes.IV) = GenerateKeyFromPassword(aes.KeySize, aes.BlockSize);
                byte[] encryptedBytes = Convert.FromBase64String(encryptedInput);

                using (ICryptoTransform decryptor = aes.CreateDecryptor()) {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        private static (byte[] Key, byte[] IV) GenerateKeyFromPassword(int keySize, int blockSize) {
            byte[] salt = Encoding.UTF8.GetBytes(SaltParameter);
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(PasswordParameter, salt);

            return (deriveBytes.GetBytes(keySize / 8), deriveBytes.GetBytes(blockSize / 8));
        }

        private static string ReplaceIllegalUrlChars(string str) {
            return str.Replace('+', '-')
                      .Replace('/', '_');
        }

        private static string RestoreIllegalUrlChars(string str) {
            return str.Replace('-', '+')
                      .Replace('_', '/');
        }
    }
}
