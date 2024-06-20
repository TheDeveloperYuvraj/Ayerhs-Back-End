using System.ComponentModel.DataAnnotations;

namespace Ayerhs.Core.Entities.AccountManagement
{
    public class Roles
    {
        [Key]
        [Required]
        public int? Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public ICollection<ClientRoles>? ClientRoles { get; set; }
    }
}
