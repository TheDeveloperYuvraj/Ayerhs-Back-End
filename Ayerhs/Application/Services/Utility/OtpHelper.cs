using Ayerhs.Core.Interfaces.Utility;

namespace Ayerhs.Application.Services.Utility
{
    /// <summary>
    /// Class that implements the IOtpHelper interface for OTP generation.
    /// </summary>
    public class OtpHelper : IOtpHelper
    {
        /// <summary>
        /// Generates a random OTP (One-Time Password) string of the specified length.
        /// </summary>
        /// <param name="length">Optional length of the OTP. Defaults to 6 characters.</param>
        /// <returns>A string containing the generated OTP.</returns>
        private static string GenerateOtp(int length = 6)
        {
            var random = new Random();
            var otp = new char[length];
            for (int i = 0; i < length; i++)
            {
                otp[i] = (char)('0' + random.Next(0, 10));
            }
            return new string(otp);
        }

        /// <summary>
        /// Asynchronously generates a new One-Time Password.
        /// </summary>
        /// <returns>A task that resolves to a string containing the generated OTP.</returns>
        public string GenerateOtpAsync()
        {
            var otp = GenerateOtp();
            return otp;
        }
    }
}
