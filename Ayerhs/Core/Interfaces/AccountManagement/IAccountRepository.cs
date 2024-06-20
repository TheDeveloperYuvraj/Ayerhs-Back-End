using Ayerhs.Core.Entities.AccountManagement;

namespace Ayerhs.Core.Interfaces.AccountManagement
{
    public interface IAccountRepository
    {
        Task<Clients> AddClientAsync(Clients client);

        Task<Clients?> GetClientByEmailAsync(string email);

        Task<Clients?> GetClientByUsernameAsync(string username);
    }
}
