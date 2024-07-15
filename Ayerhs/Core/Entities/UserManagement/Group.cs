using System.ComponentModel.DataAnnotations;

namespace Ayerhs.Core.Entities.UserManagement
{
    /// <summary>
    /// Represents a group within the user management system.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Unique identifier for the group.
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Unique identifier for the group across the system. 
        /// </summary>
        [Required]
        public string? GroupId { get; set; }

        /// <summary>
        /// Name of the group.
        /// </summary>
        [Required]
        public string? GroupName { get; set; }

        /// <summary>
        /// Identifier of the partition the group belongs to.
        /// </summary>
        [Required]
        public int PartitionId { get; set; }

        /// <summary>
        /// Indicates whether the group is active.
        /// </summary>
        [Required]
        public bool? IsActive { get; set; }

        /// <summary>
        /// Indicates whether the group is deleted.
        /// </summary>
        [Required]
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Date and time the group was created.
        /// </summary>
        [Required]
        public DateTime? GroupCreatedOn { get; set; }

        /// <summary>
        /// Date and time the group information was last updated.
        /// </summary>
        [Required]
        public DateTime? GroupUpdatedOn { get; set; }

        /// <summary>
        /// Date and time the group was marked as deleted.
        /// </summary>
        public DateTime? GroupDeletedOn { get; set; }

        /// <summary>
        /// Navigation property to the partition the group belongs to.
        /// </summary>
        [Required]
        public Partition? Partition { get; set; }
    }
}
