using Ayerhs.Core.Entities.UserManagement;

namespace Ayerhs.Core.Interfaces.UserManagement
{
    /// <summary>
    /// Interface for user management services.
    /// </summary>
    public interface IUserService
    {
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
    }
}
