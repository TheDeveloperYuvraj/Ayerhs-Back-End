using System.ComponentModel.DataAnnotations;

namespace Ayerhs.Core.Entities.AccountManagement
{
    public class InLoginClientDto
    {
        [Required]
        public string? ClientEmail { get; set; }

        [Required]
        public string? ClientPassword { get; set; }
    }
}
