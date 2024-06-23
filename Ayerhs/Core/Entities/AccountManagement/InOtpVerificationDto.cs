namespace Ayerhs.Core.Entities.AccountManagement
{
    /// <summary>
    /// This class represents the data transfer object for InOtpVerification operation.
    /// </summary>
    public class InOtpVerificationDto
    {
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Gets or sets the one-time verification code (OTP).
        /// </summary>
        public string? Otp { get; set; }
    }
}
