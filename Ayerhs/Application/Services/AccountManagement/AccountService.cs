using Ayerhs.Core.Entities.AccountManagement;
using Ayerhs.Core.Interfaces.AccountManagement;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Ayerhs.Application.Services.AccountManagement
{
    /// <summary>
    /// This class implements the IAccountService interface and provides concrete methods
    /// for account management services.
    /// </summary>
    public class AccountService(ILogger<AccountService> logger, IAccountRepository accountRepository) : IAccountService
    {
        private readonly ILogger<AccountService> _logger = logger;
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
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = Convert.FromBase64String(salt),
                DegreeOfParallelism = 8,
                MemorySize = 65536,
                Iterations = 4
            };

            var hash = argon2.GetBytes(16);
            return Convert.ToBase64String(hash);
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
                AutoDeletedOn = null,
                AttemptCount = 0,
                LastLoginDateTime = DateTime.UtcNow,
                IsLocked = false,
            };

            await _accountRepository.AddClientAsync(client);

            var role = await _accountRepository.GetRoleByName(inRegisterClientDto.RoleId);
            if (role != null)
            {
                var clientRole = new ClientRoles
                {
                    ClientId = client.Id,
                    RoleId = inRegisterClientDto.RoleId
                };
                await _accountRepository.AddClientRolesAsync(clientRole);
            }
        }

        /// <summary>
        /// Asynchronously logs in a client user using the provided email address and password.
        /// 
        /// Throws exceptions if the client account is locked due to failed login attempts.
        /// 
        /// Returns the Client entity if login is successful, otherwise null.
        /// </summary>
        /// <param name="inLoginClientDto">A DTO containing email address and password for login.</param>
        /// <returns>A Task that returns the Client entity if login is successful, otherwise null.</returns>
        public async Task<Clients?> LoginClientAsync(InLoginClientDto inLoginClientDto)
        {
            var client = await _accountRepository.GetClientByEmailAsync(inLoginClientDto.ClientEmail!);
            if (client != null)
            {
                if (client.IsLocked)
                {
                    if (client.LockedUntil > DateTime.UtcNow)
                    {
                        _logger.LogError("Client account is locked until {LockedUntil}", client.LockedUntil);
                        return null;
                    }
                    else
                    {
                        client.IsLocked = false;
                        client.AttemptCount = 0;
                        await _accountRepository.UpdateClientAsync(client);
                    }
                }

                var hashedLoginPassword = HashPassword(inLoginClientDto.ClientPassword!, client.Salt!);
                if (hashedLoginPassword == client.ClientPassword)
                {
                    _logger.LogInformation("Login Successful");

                    client.AttemptCount = 0;
                    client.LastLoginDateTime = DateTime.UtcNow;
                    client.LockedUntil = null;
                    await _accountRepository.UpdateClientAsync(client);

                    return client;
                }
                else
                {
                    client.AttemptCount++;
                    client.LastLoginDateTime = DateTime.UtcNow;
                    if (client.AttemptCount >= 3)
                    {
                        client.IsLocked = true;
                        client.LockedUntil = DateTime.UtcNow.AddMinutes(15);
                        _logger.LogError("Client account locked due to multiple failed login attempts. Locked until {LockedUntil}", client.LockedUntil);
                    }
                    await _accountRepository.UpdateClientAsync(client);
                    _logger.LogError("Invalid Credentials");
                    return null;
                }
            }
            else
            {
                _logger.LogError("Invalid Email address provided.");
                return null;
            }
        }
    }
}
