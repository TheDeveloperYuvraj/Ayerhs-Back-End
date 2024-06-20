using System.ComponentModel.DataAnnotations;

namespace Ayerhs.Core.Entities.AccountManagement
{
    /// <summary>
    /// This class represents a Data Transfer Object (DTO) used for client registration data.
    /// </summary>
    public class InRegisterClientDto
    {
        /// <summary>
        /// Name of the Client. (Required, Max Length 100)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? ClientName { get; set; }

        /// <summary>
        /// Username for the Client account. (Required, Max Length 20)
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string? ClientUsername { get; set; }

        /// <summary>
        /// Email address of the Client. (Required, Email format)
        /// </summary>
        [Required]
        [EmailAddress]
        public string? ClientEmail { get; set; }

        /// <summary>
        /// Client's password (for registration). (Required, Max Length 50)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? ClientPassword { get; set; }

        /// <summary>
        /// Mobile phone number of the Client. (Required, Phone format)
        /// </summary>
        [Required]
        [Phone]
        public string? ClientMobileNumber { get; set; }
    }
}
