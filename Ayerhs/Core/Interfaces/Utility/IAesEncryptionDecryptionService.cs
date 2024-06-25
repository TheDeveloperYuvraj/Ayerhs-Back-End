namespace Ayerhs.Core.Interfaces.Utility
{
    /// <summary>
    /// Interface for a service that encrypts and decrypts data using the Advanced Encryption Standard (AES) algorithm.
    /// </summary>
    public interface IAesEncryptionDecryptionService
    {
        /// <summary>
        /// Decrypts a string that was previously encrypted using the service's encryption method.
        /// </summary>
        /// <param name="encryptedText">The encrypted text to decrypt.</param>
        /// <returns>The decrypted plain text.</returns>
        string Decrypt(string encryptedText);
    }
}
