using Ayerhs.Core.Entities.AccountManagement;
using Ayerhs.Core.Interfaces.AccountManagement;
using Ayerhs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ayerhs.Application.Repositories.AccountManagement
{
    /// <summary>
    /// This class implements the IAccountRepository interface and provides concrete methods
    /// for interacting with Client data using Entity Framework Core.
    /// </summary>
    public class AccountRepository(ApplicationDbContext context) : IAccountRepository
    {
        private readonly ApplicationDbContext _context = context;

        /// <summary>
        /// Asynchronously adds a new Client entity to the database.
        /// </summary>
        /// <param name="client">The Client entity to be added.</param>
        /// <returns>A Task that returns the newly added Client entity.</returns>
        public async Task<Clients> AddClientAsync(Clients client)
        {
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
            return client;
        }

        /// <summary>
        /// Asynchronously retrieves a Client entity by email address.
        /// </summary>
        /// <param name="email">The email address of the Client to retrieve.</param>
        /// <returns>A Task that returns the Client entity with the matching email address, or null if not found.</returns>
        public async Task<Clients?> GetClientByEmailAsync(string email)
        {
            return await _context.Clients.SingleOrDefaultAsync(c => c.ClientEmail == email);
        }

        /// <summary>
        /// Asynchronously retrieves a Client entity by username.
        /// </summary>
        /// <param name="username">The username of the Client to retrieve.</param>
        /// <returns>A Task that returns the Client entity with the matching username, or null if not found.</returns>
        public async Task<Clients?> GetClientByUsernameAsync(string username)
        {
            return await _context.Clients.SingleOrDefaultAsync(c => c.ClientUsername == username);
        }
    }
}
