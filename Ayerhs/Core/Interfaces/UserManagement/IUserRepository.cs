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
    }
}
