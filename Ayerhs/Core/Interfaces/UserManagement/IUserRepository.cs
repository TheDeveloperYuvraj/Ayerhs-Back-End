using Ayerhs.Core.Entities.UserManagement;

namespace Ayerhs.Core.Interfaces.UserManagement
{
    /// <summary>
    /// This interface defines functionalities to interact with Partition data.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Adds a new partition asynchronously to the database.
        /// </summary>
        /// <param name="partition">The partition object to be added.</param>
        /// <returns>A task that returns true if the partition is added successfully, false otherwise.</returns>
        Task<bool?> AddPartitionAsync(Partition partition);

        /// <summary>
        /// Gets the details of a partition asynchronously based on its name.
        /// </summary>
        /// <param name="partitionName">The name of the partition to retrieve details for.</param>
        /// <returns>A task that returns a boolean indicating success or null if no partition is found. 
        ///  The task may also return additional details about the partition upon successful retrieval.</returns>
        Task<bool> GetPartitionDetailsByName(string partitionName);

        /// <summary>
        /// Retrieves a list of partitions asynchronously.
        /// </summary>
        /// <returns>A task that resolves to a list of Partition objects.</returns>
        Task<List<Partition>> GetPartitionsAsync();

        /// <summary>
        /// Asynchronously retrieves a partition by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the partition to retrieve.</param>
        /// <returns>A task that resolves to a Partition object if found, or null if not found.</returns>
        Task<Partition?> GetPartitionByIdAsync(int id);

        /// <summary>
        /// Updates partitions based on a provided name.
        /// </summary>
        /// <param name="partition">The entity of the partition to be updated.</param>
        /// <returns>A task that resolves to a nullable bool indicating success (true) or failure (null) upon update.</returns>
        Task<bool?> UpdatePartitionsByNameAsync(Partition partition);
    }
}
