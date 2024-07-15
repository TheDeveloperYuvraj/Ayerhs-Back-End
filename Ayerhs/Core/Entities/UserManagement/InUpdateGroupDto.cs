namespace Ayerhs.Core.Entities.UserManagement
{
    /// <summary>
    /// This class represents a Data Transfer Object (DTO) used for updating groups.
    /// </summary>
    public class InUpdateGroupDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the group.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the new name of the group.
        /// </summary>
        public string? NewGroupName { get; set; }

        /// <summary>
        /// Gets or sets the partition identifier associated with the group.
        /// </summary>
        public int PartitionId { get; set; }
    }
}
