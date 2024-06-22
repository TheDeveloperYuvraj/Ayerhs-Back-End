namespace Ayerhs.Core.Interfaces.Utility
{
    /// <summary>
    /// Interface that defines functionality for OTP (One-Time Password) generation.
    /// </summary>
    public interface IOtpHelper
    {
        /// <summary>
        /// Asynchronously generates a new One-Time Password.
        /// </summary>
        /// <returns>A string containing the generated OTP.</returns>
        Task<string> GenerateOtpAsync();
    }
}
