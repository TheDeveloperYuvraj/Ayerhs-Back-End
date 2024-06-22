namespace Ayerhs.Core.Entities.AccountManagement
{
    /// <summary>
    /// This class represents the InOtpRequestDto data transfer object.
    /// It contains the email address for which an OTP is requested.
    /// </summary>
    public class InOtpRequestDto
    {
        /// <summary>
        /// Gets or sets the email address for which an OTP is requested.
        /// </summary>
        public string? Email { get; set; }
    }
}
