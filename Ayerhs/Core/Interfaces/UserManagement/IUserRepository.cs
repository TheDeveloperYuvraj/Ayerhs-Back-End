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

        /// <summary>
        /// Deletes a partition asynchronously based on its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the partition to be deleted.</param>
        /// <returns>A task that returns a nullable boolean value indicating the success of the deletion.</returns>
        Task<bool> DeletePartitionByIdAsync(int id);

        /// <summary>
        /// Asynchronously retrieve details of a group based on its name.
        /// </summary>
        /// <param name="groupName">The name of the group to search for.</param>
        /// <param name="partitionId">The ID of the partition to search for.</param>
        /// <returns>A task that returns the partition details if found, or null if not found.</returns>
        Task<bool> GetGroupDetailsByNameAndPartitionAsync(string groupName, int partitionId);

        /// <summary>
        /// Adds a new group asynchronously to the database.
        /// </summary>
        /// <param name="group">The group object to be added.</param>
        /// <returns>A task that returns true if the group is added successfully, false otherwise.</returns>
        Task<bool?> AddGroupAsync(Group group);

        /// <summary>
        /// Asynchronously retrieves a list of all groups.
        /// </summary>
        /// <returns>A task that resolves to a list of Group objects.</returns>
        Task<List<Group>> GetGroupsAsync();

        /// <summary>
        /// Asynchronously retrieves a list of groups for a specific partition.
        /// </summary>
        /// <param name="partitionId">The ID of the partition to retrieve groups for.</param>
        /// <returns>A task that resolves to a list of Group objects for the specified partition.</returns>
        Task<List<Group>> GetGroupsByPartitionAsync(int partitionId);

        /// <summary>
        /// Gets a group asynchronously by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the group to retrieve.</param>
        /// <returns>A Task that returns the group if found, otherwise null.</returns>
        Task<Group?> GetGroupByIdAsync(int id);

        /// <summary>
        /// Updates a group asynchronously.
        /// </summary>
        /// <param name="group">The group object containing the updated information.</param>
        /// <returns>A Task that returns true if the update was successful, otherwise false.</returns>
        Task<bool> UpdateGroupAsync(Group group);
    }
}
