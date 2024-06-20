using Ayerhs.Core.Entities.AccountManagement;
using Ayerhs.Core.Interfaces.AccountManagement;
using System.Security.Cryptography;
using System.Text;

namespace Ayerhs.Application.Services.AccountManagement
{
    /// <summary>
    /// This class implements the IAccountService interface and provides concrete methods
    /// for account management services.
    /// </summary>
    public class AccountService(IAccountRepository accountRepository) : IAccountService
    {
        private readonly IAccountRepository _accountRepository = accountRepository;

        #region Private Methods for Support
        /// <summary>
        /// Generates a random base64 encoded salt string of length 16 bytes.
        /// </summary>
        /// <returns>A base64 encoded string representing the random salt.</returns>
        private static string GetGenerateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var buffer = new byte[16];
            rng.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// Hashes a password using SHA256 with a provided salt.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <param name="salt">The salt to be used for hashing.</param>
        /// <returns>A base64 encoded string representing the hashed password.</returns>
        private static string HashPassword(string password, string salt)
        {
            var saltedPassword = password + salt;
            return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(saltedPassword)));
        }

        /// <summary>
        /// Generates a new unique client identifier using a Guid.
        /// </summary>
        /// <returns>A string representing the newly generated client ID.</returns>
        private static string GenerateClientId()
        {
            return Guid.NewGuid().ToString();
        }
        #endregion

        /// <summary>
        /// Asynchronously registers a new Client using the provided data in the InRegisterClientDto object.
        /// Throws exceptions if the provided email or username already exists.
        /// </summary>
        /// <param name="inRegisterClientDto">A DTO containing data for client registration.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
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
