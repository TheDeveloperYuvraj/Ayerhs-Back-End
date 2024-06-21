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

        Task<Clients?> LoginClientAsync(InLoginClientDto inLoginClientDto);
    }
}
