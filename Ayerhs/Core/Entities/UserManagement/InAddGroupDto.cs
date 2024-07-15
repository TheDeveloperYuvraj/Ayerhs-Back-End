using System.ComponentModel.DataAnnotations;

namespace Ayerhs.Core.Entities.UserManagement
{
    /// <summary>
    /// Data Transfer Object (DTO) used for adding a new group.
    /// </summary>
    public class InAddGroupDto
    {
        /// <summary>
        /// Identifier of the partition the group belongs to.
        /// </summary>
        [Required]
        public int PartitionId { get; set; }

        /// <summary>
        /// Name of the group to be created.
        /// </summary>
        [Required]
        public string? GroupName { get; set; }
    }
}
