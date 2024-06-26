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

        /// <summary>
        /// Generates a new GUID.
        /// </summary>
        /// <returns>A new GUID string.</returns>
        private static string GenerateNewGuid()
        {
            return Guid.NewGuid().ToString();
        }

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
    }
}
