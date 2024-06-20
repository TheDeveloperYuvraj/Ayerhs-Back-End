﻿using Ayerhs.Core.Entities.AccountManagement;

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
    }
}
