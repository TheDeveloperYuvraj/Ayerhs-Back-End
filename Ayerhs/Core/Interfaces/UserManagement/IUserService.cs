using Ayerhs.Core.Entities.UserManagement;

namespace Ayerhs.Core.Interfaces.UserManagement
{
    /// <summary>
    /// Interface for user management services.
    /// </summary>
    public interface IUserService
    {
        #region Partitions related action methods
        /// <summary>
        /// Adds a partition to the user management system.
        /// </summary>
        /// <param name="partitionName">The name of the partition to add.</param>
        /// <returns>A task that returns a tuple indicating success and an optional error message.</returns>
        Task<(bool, string)> AddPartitionAsync(string partitionName);

        /// <summary>
        /// Retrieves a list of partitions asynchronously.
        /// </summary>
        /// <returns>A task that resolves to a list of Partition objects.</returns>
        Task<List<Partition>?> GetPartitionsAsync();

        /// <summary>
        /// Updates a partition asynchronously based on the provided name.
        /// </summary>
        /// <param name="inUpdatePartition">The entity contains id and name of the partition to be updated.</param>
        /// <returns>A task that resolves to a tuple containing a boolean indicating success and a string message describing the outcome.</returns>
        Task<(bool, string)> UpdatePartitionAsync(InUpdatePartition inUpdatePartition);

        /// <summary>
        /// Deletes a partition by ID asynchronously.
        /// </summary>
        /// <param name="id">Partition ID.</param>
        /// <returns>(success, error message).</returns>
        Task<(bool, string)> DeletePartitionAsync(int id); 
        #endregion
    }
}
