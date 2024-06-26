namespace Ayerhs.Core.Entities.UserManagement
{
    /// <summary>
    /// Represents the data required to update a partition.
    /// </summary>
    public class InUpdatePartition
    {
        /// <summary>
        /// Unique identifier of the partition.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the partition to be updated. Can be null for scenarios where the name update might not be required.
        /// </summary>
        public string? PartitionName { get; set; }
    }
}
