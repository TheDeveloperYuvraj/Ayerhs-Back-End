using System.ComponentModel.DataAnnotations;

namespace Ayerhs.Core.Entities.AccountManagement
{
    /// <summary>
    /// This class represents a Role entity within the system.
    /// </summary>
    public class Roles
    {
        /// <summary>
        /// Unique identifier for the Role (can be null for future extensibility). (Primary Key)
        /// </summary>
        [Key]
        [Required]
        public int? Id { get; set; }

        /// <summary>
        /// Name of the Role. (Required, Max Length 50)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        /// <summary>
        /// Description of the Role (optional).
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Collection of client roles associated with the Role.
        /// </summary>
        public ICollection<ClientRoles>? ClientRoles { get; set; }
    }
}
