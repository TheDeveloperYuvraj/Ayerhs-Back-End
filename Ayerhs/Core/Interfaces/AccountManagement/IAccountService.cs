using Ayerhs.Core.Entities.AccountManagement;

namespace Ayerhs.Core.Interfaces.AccountManagement
{
    public interface IAccountService
    {
        Task RegisterClientAsync(InRegisterClientDto inRegisterClientDto);
    }
}
