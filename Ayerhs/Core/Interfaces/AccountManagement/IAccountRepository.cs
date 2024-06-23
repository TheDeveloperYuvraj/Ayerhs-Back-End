using Ayerhs.Core.Entities.AccountManagement;
using Ayerhs.Core.Entities.Utility;

namespace Ayerhs.Core.Interfaces.AccountManagement
{
    /// <summary>
    /// This interface defines methods for interacting with Client data.
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Asynchronously adds a new Client entity to the repository.
        /// </summary>
        /// <param name="client">The Client entity to be added.</param>
        /// <returns>A Task that returns the newly added Client entity.</returns>
        Task<Clients> AddClientAsync(Clients client);

        /// <summary>
        /// Asynchronously retrieves a Client entity by email address.
        /// </summary>
        /// <param name="email">The email address of the Client to retrieve.</param>
        /// <returns>A Task that returns the Client entity with the matching email address, or null if not found.</returns>
        Task<Clients?> GetClientByEmailAsync(string email);

        /// <summary>
        /// Asynchronously retrieves a Client entity by username.
        /// </summary>
        /// <param name="username">The username of the Client to retrieve.</param>
        /// <returns>A Task that returns the Client entity with the matching username, or null if not found.</returns>
        Task<Clients?> GetClientByUsernameAsync(string username);

        /// <summary>
        /// Asynchronously retrieve a role by ID.
        /// </summary>
        /// <param name="roleId">The ID of role.</param>
        /// <returns>A task that returns the </returns>
        Task<string?> GetRoleByName(int roleId);

        /// <summary>
        /// Asynchronously adds a new Client Role entity to the repository.
        /// </summary>
        /// <param name="clientRoles">The entity which decide about the role.</param>
        /// <returns>A Task that returns assigned Client Role</returns>
        Task <ClientRoles?> AddClientRolesAsync(ClientRoles? clientRoles);

        /// <summary>
        /// Asynchronously updates an existing Client entity in the repository.
        /// </summary>
        /// <param name="clients">The Client entity with the updated data.</param>
        /// <returns>A Task representing the asynchronous update operation.</returns>
        Task UpdateClientAsync(Clients clients);

        /// <summary>
        /// Asynchronously getting registered clients
        /// </summary>
        /// <returns>A Task represents List of Client entity.</returns>
        Task<List<Clients>?> GetClientsAsync();

        /// <summary>
        /// Asynchronously adds OTP details in table.
        /// </summary>
        /// <param name="otpStorage">The entity to be added.</param>
        /// <returns>A Task of entity which added into database.</returns>
        Task<OtpStorage?> AddOtpAsync(OtpStorage otpStorage);

        /// <summary>
        /// Asynchronously getting OTP stored in database.
        /// </summary>
        /// <param name="email">An email of client with received.</param>
        /// <returns>A Task with entity of OtpStorage</returns>
        Task<OtpStorage?> GetOtpStorageByEmailAsync(string? email);

        /// <summary>
        /// Asynchronously update OTP details in database.
        /// </summary>
        /// <param name="otpStorage">The entity to be updated.</param>
        /// <returns>A Task of entity which Updated into database.</returns>
        Task<OtpStorage?> UpdateOtpAsync(OtpStorage otpStorage);
    }
}
