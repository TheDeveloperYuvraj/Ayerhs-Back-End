using Ayerhs.Core.Entities.UserManagement;
using Ayerhs.Core.Interfaces.UserManagement;
using Ayerhs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ayerhs.Application.Repositories.UserManagement
{
    /// <summary>
    /// This class provides functionalities to interact with User data in the application.
    /// Inherits from the ILogger interface and implements the IUserRepository interface.
    /// </summary>
    public class UserRepository(ILogger<UserRepository> logger, ApplicationDbContext context) : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger = logger;
        private readonly ApplicationDbContext _context = context;

        /// <summary>
        /// Adds a new partition asynchronously to the database.
        /// </summary>
        /// <param name="partition">The partition object to be added.</param>
        /// <returns>A task that returns true if the partition is added successfully, false otherwise.</returns>
        public async Task<bool?> AddPartitionAsync(Partition partition)
        {
            await _context.Partitions.AddAsync(partition);
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Partiton added successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding partition {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Asynchronously retrieves details of a partition based on its name.
        /// </summary>
        /// <param name="partitionName">The name of the partition to search for.</param>
        /// <returns>A task that returns the partition details if found, or null if not found.</returns>
        public async Task<bool> GetPartitionDetailsByName(string partitionName)
        {
            return await _context.Partitions.AnyAsync(n => n.PartitionName == partitionName);
        }

        /// <summary>
        /// Asynchronously retrieves a list of all Partition entities from the database.
        /// </summary>
        /// <returns>A task that resolves to a list of Partition objects.</returns>
        public async Task<List<Partition>> GetPartitionsAsync()
        {
            return await _context.Partitions.ToListAsync();
        }

        /// <summary>
        /// Asynchronously retrieves a partition by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the partition to retrieve.</param>
        /// <returns>A task that resolves to a Partition object if found, or null if not found.</returns>
        public async Task<Partition?> GetPartitionByIdAsync(int id)
        {
            Partition? partition = await _context.Partitions.FindAsync(id);
            return partition;
        }

        /// <summary>
        /// Updates a partition in the database based on the provided partition object.
        /// This method assumes the partition object has its identifier set for identification during update.
        /// </summary>
        /// <param name="partition">The Partition object containing the updated information.</param>
        /// <returns>A task that resolves to a nullable bool indicating success (true) or failure (null) upon update.</returns>
        public async Task<bool?> UpdatePartitionsByNameAsync(Partition partition)
        {
            _context.Partitions.Update(partition);
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Partition Updated Successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating partition {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Deletes a partition by ID asynchronously.
        /// </summary>
        /// <param name="id">Partition ID.</param>
        /// <returns>True on success, False otherwise.</returns>
        public async Task<bool> DeletePartitionByIdAsync(int id)
        {
            try
            {
                var partition = await _context.Partitions.FindAsync(id);
                if (partition != null)
                {
                    _context.Partitions.Remove(partition);
                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Partition {Partition} removed successfully.", partition.PartitionName);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "{Message}", ex.Message);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Asynchronously retrieve details of a group based on its name.
        /// </summary>
        /// <param name="groupName">The name of the group to search for.</param>
        /// <param name="partitionId">The ID of the partition to search for.</param>
        /// <returns>A task that returns the partition details if found, or null if not found.</returns>
        public async Task<bool> GetGroupDetailsByNameAndPartitionAsync(string groupName, int partitionId)
        {
            return await _context.Groups.AnyAsync(g => g.GroupName == groupName && g.PartitionId == partitionId);
        }

        /// <summary>
        /// Adds a new group asynchronously to the database.
        /// </summary>
        /// <param name="group">The group object to be added.</param>
        /// <returns>A task that returns true if the group is added successfully, false otherwise.</returns>
        public async Task<bool?> AddGroupAsync(Group group)
        {
            await _context.Groups.AddAsync(group);
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Group added successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding group {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Asynchronously retrieves a list of all groups from the data context.
        /// </summary>
        /// <returns>A task that resolves to a list of Group objects.</returns>
        public async Task<List<Group>> GetGroupsAsync()
        {
            return await _context.Groups.ToListAsync();
        }

        /// <summary>
        /// Asynchronously retrieves a list of groups for a specific partition from the data context.
        /// </summary>
        /// <param name="partitionId">The ID of the partition to retrieve groups for.</param>
        /// <returns>A task that resolves to a list of Group objects for the specified partition.</returns>
        public async Task<List<Group>> GetGroupsByPartitionAsync(int partitionId)
        {
            return await _context.Groups.Where(g => g.PartitionId == partitionId).ToListAsync();
        }

        /// <summary>
        /// Gets a group asynchronously by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the group to retrieve.</param>
        /// <returns>A Task that returns the group if found, otherwise null.</returns>
        public async Task<Group?> GetGroupByIdAsync(int id)
        {
            return await _context.Groups.FindAsync(id);
        }

        /// <summary>
        /// Updates a group asynchronously.
        /// </summary>
        /// <param name="group">The group object containing the updated information.</param>
        /// <returns>A Task that returns true if the update was successful, otherwise false.</returns>
        public async Task<bool> UpdateGroupAsync(Group group)
        {
            _context.Groups.Update(group);
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Group updated successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating group {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Asynchronously attempts to delete a group from the database based on the provided ID.
        /// </summary>
        /// <param name="id">The ID of the group to delete.</param>
        /// <returns>A task that returns true if the deletion was successful, false otherwise.</returns>
        /// <exception cref="Exception">An error occurred while deleting the group.</exception>
        public async Task<bool> DeleteGroupAsync(int id)
        {
            try
            {
                var group = await _context.Groups.FindAsync(id);
                if (group != null)
                {
                    _context.Groups.Remove(group);
                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Partition {Partition} removed successfully.", group.GroupName);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "{Message}", ex.Message);
                        return false;
                    }
                }
                else
                {
                    _logger.LogError("Group with Id {Id} not present in database.", id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);
                return false;
            }
        }
    }
}
