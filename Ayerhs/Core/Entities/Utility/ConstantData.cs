namespace Ayerhs.Core.Entities.Utility
{
    /// <summary>
    /// Provides utility methods for generating constant data.
    /// </summary>
    public static class ConstantData
    {
        /// <summary>
        /// Generates a unique transaction ID based on current UTC time.
        /// </summary>
        /// <returns>A string representing the transaction ID in the format "yyyyMMddHHmmssfff".</returns>
        public static string GenerateTransactionId()
        {
            return DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        }
    }
}
