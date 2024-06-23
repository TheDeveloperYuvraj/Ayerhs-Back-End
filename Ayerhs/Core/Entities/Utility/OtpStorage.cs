using System.ComponentModel.DataAnnotations;

namespace Ayerhs.Core.Entities.Utility
{
    /// <summary>
    /// This class represents a record for storing One-Time Passwords (OTPs).
    /// </summary>
    public class OtpStorage
    {
        /// <summary>
        /// Unique identifier for the OTP record.
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Email address associated with the OTP.
        /// </summary>
        [Required]
        public string? Email { get; set; }

        /// <summary>
        /// Randomally generated OTP for respective Email address.
        /// </summary>
        [Required]
        public string? Otp { get; set; }

        /// <summary>
        /// Date and time when the OTP was generated.
        /// </summary>
        [Required]
        public DateTime GeneratedOn { get; set; }

        /// <summary>
        /// Date and time until which the OTP is valid.
        /// </summary>
        [Required]
        public DateTime ValidUpto { get; set; }
    }
}
