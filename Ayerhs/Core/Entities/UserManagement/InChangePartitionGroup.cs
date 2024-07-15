namespace Ayerhs.Core.Entities.UserManagement
{
    /// <summary>
    /// Represents a group associated with a partition for change management purposes.
    /// </summary>
    public class InChangePartitionGroup
    {
        /// <summary>
        /// Gets or sets the identifier of the partition.
        /// </summary>
        public int PartitionId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the group.
        /// </summary>
        public int GroupId { get; set; }
    }
}
