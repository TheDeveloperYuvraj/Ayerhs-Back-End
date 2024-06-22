using System.ComponentModel.DataAnnotations;

namespace Ayerhs.Core.Entities.AccountManagement
{
    /// <summary>
    /// This class represents the InLoginClientDto object used for login requests.
    /// </summary>
    public class InLoginClientDto
    {
        /// <summary>
        /// Gets or sets the email address of the client attempting to login.
        /// </summary>
        [Required]
        public string? ClientEmail { get; set; }

        /// <summary>
        /// Gets or sets the password of the client attempting to login.
        /// </summary>
        [Required]
        public string? ClientPassword { get; set; }
    }
}
