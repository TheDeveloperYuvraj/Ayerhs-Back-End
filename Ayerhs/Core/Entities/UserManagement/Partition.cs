using System.ComponentModel.DataAnnotations;

namespace Ayerhs.Core.Entities.UserManagement
{
    /// <summary>
    /// Represents a partition within the user management system.
    /// </summary>
    public class Partition
    {
        /// <summary>
        /// Unique identifier for the partition.
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Unique identifier for each partition
        /// </summary>
        [Required]
        public string? PartitionId { get; set; }

        /// <summary>
        /// Name of the partition.
        /// </summary>
        [Required]
        public string? PartitionName { get; set; }

        /// <summary>
        /// Date and time when the partition was created.
        /// </summary>
        [Required]
        public DateTime? PartitionCreatedOn { get; set; }

        /// <summary>
        /// Date and time when the partition was last updated.
        /// </summary>
        [Required]
        public DateTime? PartitionUpdatedOn { get; set; }
    }
}
