using Ayerhs.Core.Entities.AccountManagement;
using Ayerhs.Core.Interfaces.AccountManagement;
using System.Security.Cryptography;
using System.Text;

namespace Ayerhs.Application.Services.AccountManagement
{
    public class AccountService(IAccountRepository accountRepository) : IAccountService
    {
        private readonly IAccountRepository _accountRepository = accountRepository;

        #region Private Methods for Support
        private static string GetGenerateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var buffer = new byte[16];
            rng.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        private static string HashPassword(string password, string salt)
        {
            var saltedPassword = password + salt;
            return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(saltedPassword)));
        }

        private static string GenerateClientId()
        {
            return Guid.NewGuid().ToString();
        }
        #endregion

        public async Task RegisterClientAsync(InRegisterClientDto inRegisterClientDto)
        {
            var existingClient = await _accountRepository.GetClientByEmailAsync(inRegisterClientDto.ClientEmail!);
            if (existingClient != null)
            {
                throw new InvalidOperationException("Client email already exists.");
            }

            existingClient = await _accountRepository.GetClientByUsernameAsync(inRegisterClientDto.ClientUsername!);
            if (existingClient != null)
            {
                throw new InvalidOperationException("Client username already exists.");
            }

            var salt = GetGenerateSalt();
            var hashedPassword = HashPassword(inRegisterClientDto.ClientPassword!, salt);

            var client = new Clients
            {
                ClientId = GenerateClientId(),
                ClientName = inRegisterClientDto.ClientName,
                ClientUsername = inRegisterClientDto.ClientUsername,
                ClientEmail = inRegisterClientDto.ClientEmail,
                ClientPassword = hashedPassword,
                ClientMobileNumber = inRegisterClientDto.ClientMobileNumber,
                IsAdmin = true,
                IsActive = false,
                Status = ClientStatus.Inactive,
                DeletedState = DeletedState.NotDeleted,
                Salt = salt,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                DeletedOn = null,
                AutoDeletedOn = null
            };

            await _accountRepository.AddClientAsync(client);
        }
    }
}
