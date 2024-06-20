using System.ComponentModel.DataAnnotations;

namespace Ayerhs.Core.Entities.AccountManagement
{
    public class ClientRoles
    {
        [Required]
        public int ClientId { get; set; }
        public Clients? Client { get; set; }

        [Required]
        public int RoleId { get; set; }
        public Roles? Role { get; set; }
    }
}
