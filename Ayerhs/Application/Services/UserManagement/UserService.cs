using Ayerhs.Core.Entities.UserManagement;
using Ayerhs.Core.Interfaces.UserManagement;

namespace Ayerhs.Application.Services.UserManagement
{
    /// <summary>
    /// Implements the IUserService interface for user management operations.
    /// </summary>
    public class UserService(ILogger<UserService> logger, IUserRepository userRepository) : IUserService
    {
        private readonly ILogger<UserService> _logger = logger;
        private readonly IUserRepository _userRepository = userRepository;

        #region Private Helper Methods
        /// <summary>
        /// Generates a new GUID.
        /// </summary>
        /// <returns>A new GUID string.</returns>
        private static string GenerateNewGuid()
        {
            return Guid.NewGuid().ToString();
        } 
        #endregion

        #region Partitions related action methods
        /// <summary>
        /// Adds a partition to the user management system.
        /// </summary>
        /// <param name="partitionName">The name of the partition to add.</param>
        /// <returns>A task that returns a tuple indicating success and an optional error message.</returns>
        public async Task<(bool, string)> AddPartitionAsync(string partitionName)
        {
            var success = await _userRepository.GetPartitionDetailsByName(partitionName);
            if (!success)
            {
                Partition partition = new()
                {
                    PartitionId = GenerateNewGuid(),
                    PartitionName = partitionName,
                    PartitionCreatedOn = DateTime.UtcNow,
                    PartitionUpdatedOn = DateTime.UtcNow
                };
                var res = (bool)await _userRepository.AddPartitionAsync(partition);
                if (res)
                {
                    _logger.LogInformation("New Partion Created Successfully with name {PartitionName}", partitionName);
                    return (true, $"{partitionName} created successfully.");
                }
                else
                {
                    _logger.LogError("Error occurred while adding partition in database.");
                    return (false, "Error occurred while adding partition in database.");
                }
            }
            else
            {
                _logger.LogError("Duplicate partition name {PartitionName}", partitionName);
                return (false, $"Partition Name {partitionName} is used. Please provide unique partition name.");
            }
        }

        /// <summary>
        /// Asynchronously retrieves a list of all Partition entities from the database.
        /// </summary>
        /// <returns> A task that resolves to a list of Partition objects, or null if an error occurs.</returns>
        public async Task<List<Partition>?> GetPartitionsAsync()
        {
            try
            {
                List<Partition> partitions = await _userRepository.GetPartitionsAsync();
                return partitions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Updates a partition asynchronously and returns success/failure with message.
        /// </summary>
        /// <param name="inUpdatePartition">The entity contains id and name of the partition to be updated.</param>
        /// <returns>A task resolving to (success, message) tuple.</returns>
        public async Task<(bool, string)> UpdatePartitionAsync(InUpdatePartition inUpdatePartition)
        {
            var duplicatePartitionName = await _userRepository.GetPartitionDetailsByName(inUpdatePartition.PartitionName!);
            if (!duplicatePartitionName)
            {
                var existingPartition = await _userRepository.GetPartitionByIdAsync(inUpdatePartition.Id);
                if (existingPartition != null)
                {
                    existingPartition.PartitionName = inUpdatePartition.PartitionName;
                    existingPartition.PartitionUpdatedOn = DateTime.UtcNow;

                    var res = (bool)await _userRepository.UpdatePartitionsByNameAsync(existingPartition);
                    if (res)
                    {
                        _logger.LogInformation("{PartitionName} is successfully updated.", inUpdatePartition.PartitionName);
                        return (true, $"{inUpdatePartition.PartitionName} is successfully updated.");
                    }
                    else
                    {
                        _logger.LogError("Error occurred while updating partition. {PartitionName}", inUpdatePartition.PartitionName);
                        return (false, $"Error occurred while updating partition. {inUpdatePartition.PartitionName}");
                    }
                }
                else
                {
                    _logger.LogError("Invalid partition Id provided. {PartitionName}", inUpdatePartition.Id);
                    return (false, $"Invalid partition Id provided. {inUpdatePartition.Id}");
                }
            }
            else
            {
                _logger.LogError("Duplicate partition name found. {PartitionName}", inUpdatePartition.PartitionName);
                return (false, $"Duplicate partition name found. {inUpdatePartition.PartitionName}");
            }
        }

        /// <summary>
        /// Deletes a partition by ID asynchronously. Logs info/errors.
        /// </summary>
        /// <param name="id">Partition ID.</param>
        /// <returns>(success, error message).</returns>
        public async Task<(bool, string)> DeletePartitionAsync(int id)
        {
            var existingPartition = await _userRepository.GetPartitionByIdAsync(id);
            if (existingPartition != null)
            {
                var res = await _userRepository.DeletePartitionByIdAsync(id);
                if (res)
                {
                    _logger.LogInformation("Partition {Partition} removed successfully.", existingPartition.PartitionName);
                    return (true, $"Partition {existingPartition.PartitionName} removed successfully.");
                }
                else
                {
                    _logger.LogError("Error occurred while removing partition from database {Partition}", existingPartition.PartitionName);
                    return (false, $"Error occurred while removing partition {existingPartition.PartitionName}");
                }
            }
            else
            {
                _logger.LogError("Invalid ID {Id} Provided", id);
                return (false, "Invalid ID provided.");
            }
        }
        #endregion

        /// <summary>
        /// Adds a new group to the system under a specified partition.
        /// </summary>
        /// <param name="inAddGroupDto">An object containing details of the group to be added.</param>
        /// <returns>(bool, string): A tuple indicating success (bool) and a message (string) describing the outcome.</returns>
        public async Task<(bool, string)> AddGroupAsync(InAddGroupDto inAddGroupDto)
        {
            var isPartitionPresent = await _userRepository.GetPartitionByIdAsync(inAddGroupDto.PartitionId);
            if (isPartitionPresent != null)
            {
                var isGroupPresent = await _userRepository.GetGroupDetailsByNameAndPartitionAsync(inAddGroupDto.GroupName!, inAddGroupDto.PartitionId);
                if (!isGroupPresent)
                {
                    Group group = new()
                    {
                        GroupId = GenerateNewGuid(),
                        GroupName = inAddGroupDto.GroupName,
                        PartitionId = inAddGroupDto.PartitionId,
                        IsActive = true,
                        IsDeleted = false,
                        GroupCreatedOn = DateTime.UtcNow,
                        GroupUpdatedOn = DateTime.UtcNow,
                        GroupDeletedOn = null,
                        Partition = isPartitionPresent,
                    };
                    var res = (bool)await _userRepository.AddGroupAsync(group);
                    if (res)
                    {
                        _logger.LogInformation("Group {GroupName} successfully added under partition {PartitionName}", inAddGroupDto.GroupName, isPartitionPresent.PartitionName);
                        return (true, $"Group {inAddGroupDto.GroupName} successfully added under partition {isPartitionPresent.PartitionName}");
                    }
                    else
                    {
                        _logger.LogError("Error occurred while adding group {GroupName} under partition {PartitionName}", inAddGroupDto.GroupName, isPartitionPresent.PartitionName);
                        return (false, $"Internal error occurred while adding group {inAddGroupDto.GroupName} under partition {isPartitionPresent.PartitionName}");
                    }
                }
                else
                {
                    _logger.LogError("Duplicate group name under same partition. GroupName : {GroupName} => PartitionName : {Partition}", inAddGroupDto.GroupName, isPartitionPresent.PartitionName);
                    return (false, "You can not create a group with duplicate group name under same partition. Please choose group name wisely and unique.");
                }
            }
            else
            {
                _logger.LogError("Invalid Partition ID {Id} provided.", inAddGroupDto.PartitionId);
                return (false, "Partition not present.");
            }
        }

        /// <summary>
        /// Retrieves groups asynchronously based on partition ID:
        /// </summary>
        /// <param name="partitionId">The partition ID (0 for all groups).</param>
        /// <returns>A task that resolves to a list of Group objects or null (on error).</returns>
        public async Task<List<Group>?> GetGroupsAsync(int partitionId)
        {
            if (partitionId >= 0)
            {
                _ = new List<Group>();
                List<Group> groups;
                if (partitionId == 0)
                {
                    groups = await _userRepository.GetGroupsAsync();
                }
                else
                {
                    groups = await _userRepository.GetGroupsByPartitionAsync(partitionId);
                }

                foreach (var group in groups)
                {
                    group.Partition = await _userRepository.GetPartitionByIdAsync(group.PartitionId);
                }

                return groups;
            }
            else
            {
                _logger.LogError("Invalid Partition ID provided {Id}", partitionId);
                return null;
            }
        }
    }
}
