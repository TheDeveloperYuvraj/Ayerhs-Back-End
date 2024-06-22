using Ayerhs.Core.Entities.AccountManagement;

namespace Ayerhs.Core.Interfaces.AccountManagement
{
    /// <summary>
    /// This interface defines methods for account management services.
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Asynchronously registers a new Client using the provided data.
        /// </summary>
        /// <param name="inRegisterClientDto">A DTO containing data for client registration.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task RegisterClientAsync(InRegisterClientDto inRegisterClientDto);

        /// <summary>
        /// Asynchronously attempts to login a client using the provided credentials.
        /// </summary>
        /// <param name="inLoginClientDto">A DTO containing login credentials.</param>
        /// <returns>A Task that resolves to a Clients object containing user information on success, or null on failure.</returns>
        Task<Clients?> LoginClientAsync(InLoginClientDto inLoginClientDto);
    }
}
