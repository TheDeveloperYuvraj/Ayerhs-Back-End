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
    }
}
