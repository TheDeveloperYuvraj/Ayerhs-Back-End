using System.ComponentModel.DataAnnotations;

namespace Ayerhs.Core.Entities.AccountManagement
{
    public class InRegisterClientDto
    {
        [Required]
        [MaxLength(100)]
        public string? ClientName { get; set; }

        [Required]
        [MaxLength(20)]
        public string? ClientUsername { get; set; }

        [Required]
        [EmailAddress]
        public string? ClientEmail { get; set; }

        [Required]
        [MaxLength(50)]
        public string? ClientPassword { get; set; }

        [Required]
        [Phone]
        public string? ClientMobileNumber { get; set; }
    }
}
