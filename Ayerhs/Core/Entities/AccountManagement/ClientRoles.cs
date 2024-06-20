using System.ComponentModel.DataAnnotations;

namespace Ayerhs.Core.Entities.AccountManagement
{
    /// <summary>
    /// This class represents the relationship between a Client and a Role.
    /// (Many-to-Many relationship)
    /// </summary>
    public class ClientRoles
    {
        /// <summary>
        /// Foreign key for the Client entity. (Required)
        /// </summary>
        [Required]
        public int ClientId { get; set; }

        /// <summary>
        /// Navigation property for the Client entity.
        /// </summary>
        public Clients? Client { get; set; }

        /// <summary>
        /// Foreign key for the Role entity. (Required)
        /// </summary>
        [Required]
        public int RoleId { get; set; }

        /// <summary>
        /// Navigation property for the Role entity.
        /// </summary>
        public Roles? Role { get; set; }
    }
}
