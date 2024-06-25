using Ayerhs.Core.Interfaces.Utility;
using System.Security.Cryptography;

namespace Ayerhs.Application.Services.Utility
{
    /// <summary>
    /// Service for encrypting and decrypting data using AES algorithm.
    /// </summary>
    public class AesEncryptionDecryptionService(IConfiguration configuration) : IAesEncryptionDecryptionService
    {
        /// <summary>
        /// The AES key used for encryption and decryption.
        /// This value is retrieved from the configuration using the key "AESKey".
        /// </summary>
        private readonly byte[] _key = Convert.FromBase64String(configuration["AESKey"]!);

        /// <summary>
        /// Decrypts a base64 encoded string that was previously encrypted using AES.
        /// </summary>
        /// <param name="encryptedText">The base64 encoded string containing the encrypted data.</param>
        /// <returns>The decrypted string.</returns>
        public string Decrypt(string encryptedText)
        {
            var fullCipher = Convert.FromBase64String(encryptedText);
            var iv = new byte[16];
            var cipher = new byte[16];

            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            using var aesAlg = Aes.Create();
            using var decryptor = aesAlg.CreateDecryptor(_key, iv);
            using var msDecrypt = new MemoryStream(cipher);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }
}
