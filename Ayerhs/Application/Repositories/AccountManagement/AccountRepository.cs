using Ayerhs.Core.Entities.AccountManagement;
using Ayerhs.Core.Entities.Utility;
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

        /// <summary>
        /// Asynchronously retrieves the name of a role by its ID.
        /// </summary>
        /// <param name="roleId">The ID of the role to retrieve.</param>
        /// <returns>A Task that returns the name of the role with the matching ID, or null if not found.</returns>
        public async Task<string?> GetRoleByName(int roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role != null)
            {
                return role.Name;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Asynchronously adds a new Client Role entity to the database.
        /// </summary>
        /// <param name="clientRoles">An entity that decides Client Role.</param>
        /// <returns>A Task that returns Client Roles entity.</returns>
        public async Task<ClientRoles?> AddClientRolesAsync(ClientRoles? clientRoles)
        {
            await _context.ClientRoles.AddAsync(clientRoles!);
            await _context.SaveChangesAsync();
            return clientRoles;
        }

        /// <summary>
        /// Asynchronously updates an existing Client entity in the database.
        /// </summary>
        /// <param name="clients">The Client entity to be updated.</param>
        /// <returns>A Task that completes the update operation.</returns>
        public async Task UpdateClientAsync(Clients clients)
        {
            _context.Clients.Update(clients);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Asynchronously getting registered clients
        /// </summary>
        /// <returns>A Task represents List of Client entity.</returns>
        public async Task<List<Clients>?> GetClientsAsync()
        {
            List<Clients> clients = await _context.Clients.ToListAsync();
            return clients;
        }

        /// <summary>
        /// Asynchronously adds OTP details in table.
        /// </summary>
        /// <param name="otpStorage">The entity to be added.</param>
        /// <returns>A Task of entity which added into database.</returns>
        public async Task<OtpStorage?> AddOtpAsync(OtpStorage otpStorage)
        {
            await _context.OtpStorages.AddAsync(otpStorage);
            await _context.SaveChangesAsync();
            return otpStorage;
        }

        /// <summary>
        /// Asynchronously getting OTP stored in database.
        /// </summary>
        /// <param name="email">An email of client with received.</param>
        /// <returns>A Task with entity of OtpStorage</returns>
        public async Task<OtpStorage?> GetOtpStorageByEmailAsync(string? email)
        {
            var otpStorage =  await _context.OtpStorages.FirstOrDefaultAsync(x => x.Email == email);
            return otpStorage;
        }

        /// <summary>
        /// Asynchronously update OTP details in database.
        /// </summary>
        /// <param name="otpStorage">The entity to be updated.</param>
        /// <returns>A Task of entity which Updated into database.</returns>
        public async Task<OtpStorage?> UpdateOtpAsync(OtpStorage otpStorage)
        {
            var existingEmailRecord = await _context.OtpStorages.FirstOrDefaultAsync(x => x.Email == otpStorage.Email);
            if (existingEmailRecord != null)
            {
                existingEmailRecord.GeneratedOn = otpStorage.GeneratedOn;
                existingEmailRecord.ValidUpto = otpStorage.ValidUpto;
                existingEmailRecord.Otp = otpStorage.Otp;
                _context.OtpStorages.Update(existingEmailRecord);
                await _context.SaveChangesAsync();
                return otpStorage; 
            }
            return otpStorage;
        }

        /// <summary>
        /// Performs verification on a client object.
        /// </summary>
        /// <param name="client">The client object to be verified.</param>
        /// <returns>A task indicating the completion of verification.</returns>
        public async Task VerifyClientAsync(Clients client)
        {
            var existingClient = await _context.Clients.FirstOrDefaultAsync(x => x.ClientEmail == client.ClientEmail );
            if (existingClient != null)
            {
                existingClient.IsActive = true;
                existingClient.Status = ClientStatus.Active;
                _context.Clients.Update(existingClient);
                await _context.SaveChangesAsync();
            }
        }
    }
}
