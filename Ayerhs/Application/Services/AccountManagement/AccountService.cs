using Ayerhs.Core.Entities.AccountManagement;
using Ayerhs.Core.Entities.Utility;
using Ayerhs.Core.Interfaces.AccountManagement;
using Ayerhs.Core.Interfaces.Utility;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Ayerhs.Application.Services.AccountManagement
{
    /// <summary>
    /// This class implements the IAccountService interface and provides concrete methods
    /// for account management services.
    /// </summary>
    public class AccountService(ILogger<AccountService> logger, IAccountRepository accountRepository, IOtpHelper otpHelper, IEmailService emailService, IAesEncryptionDecryptionService aesEncryptionDecryptionService) : IAccountService
    {
        private readonly ILogger<AccountService> _logger = logger;
        private readonly IAccountRepository _accountRepository = accountRepository;
        private readonly IOtpHelper _otpHelper = otpHelper;
        private readonly IEmailService _emailService = emailService;
        private readonly IAesEncryptionDecryptionService _aesEncryptionDecryptionService = aesEncryptionDecryptionService;

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
            inRegisterClientDto.ClientPassword = _aesEncryptionDecryptionService.Decrypt(inRegisterClientDto.ClientPassword!);
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

            existingClient = await _accountRepository.GetClientByMobileNumberAsync(inRegisterClientDto.ClientMobileNumber!);
            if (existingClient != null)
            {
                throw new InvalidOperationException("Client mobile number already exists.");
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

                inLoginClientDto.ClientPassword = _aesEncryptionDecryptionService.Decrypt(inLoginClientDto.ClientPassword!);
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

        /// <summary>
        /// Asynchronously getting registered client list.
        /// </summary>
        /// <returns>A Task represents list of Clients entity.</returns>
        public async Task<List<Clients>?> GetClientsAsync()
        {
            var result = await _accountRepository.GetClientsAsync();
            return result;
        }

        /// <summary>
        /// Asynchronously generate a random OTP and send on email
        /// </summary>
        /// <param name="inOtpRequestDto">A DTO containing email related data.</param>
        /// <returns>A Task that contains a message.</returns>
        public async Task<(bool, string)> OtpGenerationAndEmailAsync(InOtpRequestDto inOtpRequestDto)
        {
            var client = await _accountRepository.GetClientByEmailAsync(inOtpRequestDto.Email!);
            if (client != null)
            {
                if (inOtpRequestDto.Use == OtpUse.AccountActivate)
                {
                    var otpStorageResponse = await _accountRepository.GetOtpStorageByEmailAsync(inOtpRequestDto.Email!);
                    if (otpStorageResponse == null)
                    {
                        var otp = _otpHelper.GenerateOtpAsync();
                        if (otp != null)
                        {
                            await _emailService.SendOtpEmailAsync(inOtpRequestDto.Email!, otp, "Ayerhs - Account Activation Code", "Your One Time Password (OTP) is: ", true);
                            var otpStorage = new OtpStorage
                            {
                                Email = inOtpRequestDto.Email,
                                GeneratedOn = DateTime.UtcNow,
                                ValidUpto = DateTime.UtcNow.AddMinutes(15),
                                Otp = otp,
                                Use = (int)OtpUse.AccountActivate
                            };

                            await _accountRepository.AddOtpAsync(otpStorage);
                            return (true, "OTP Genrate and send successfully.");
                        }
                        else
                        {
                            _logger.LogError("An error occurred while generation OTP for user email {Email}", inOtpRequestDto.Email);
                            return (false, "OTP generation failed.");
                        }
                    }
                    else
                    {
                        DateTime currentDateTime = DateTime.UtcNow;
                        if (otpStorageResponse.ValidUpto > currentDateTime)
                        {
                            await _emailService.SendOtpEmailAsync(inOtpRequestDto.Email!, otpStorageResponse!.Otp!, "Ayerhs - Account Activation Code", "Your One Time Password (OTP) is: ", true);
                            return (true, "OTP Genrate and send successfully.");
                        }
                        else
                        {
                            var otp = _otpHelper.GenerateOtpAsync();
                            if (otp != null)
                            {
                                await _emailService.SendOtpEmailAsync(inOtpRequestDto.Email!, otp, "Ayerhs - Account Activation Code", "Your One Time Password (OTP) is: ", true);
                                var otpStorage = new OtpStorage
                                {
                                    Email = inOtpRequestDto.Email,
                                    GeneratedOn = DateTime.UtcNow,
                                    ValidUpto = DateTime.UtcNow.AddMinutes(15),
                                    Otp = otp
                                };

                                await _accountRepository.UpdateOtpAsync(otpStorage);
                                return (true, "OTP Genrate and send successfully.");
                            }
                            else
                            {
                                _logger.LogError("An error occurred while generation OTP for user email {Email}", inOtpRequestDto.Email);
                                return (false, "OTP generation failed.");
                            }
                        }
                    } 
                }
                else if (inOtpRequestDto.Use == OtpUse.ForgotClientPassword)
                {
                    var otpStorageResponse = await _accountRepository.GetOtpStorageByEmailAsync(inOtpRequestDto.Email!);
                    if (otpStorageResponse == null)
                    {
                        var otp = _otpHelper.GenerateOtpAsync();
                        if (otp != null)
                        {
                            await _emailService.SendOtpEmailAsync(inOtpRequestDto.Email!, otp, "Ayerhs - Forgot Password Code", "Your One Time Password (OTP) is: ", true);
                            var otpStorage = new OtpStorage
                            {
                                Email = inOtpRequestDto.Email,
                                GeneratedOn = DateTime.UtcNow,
                                ValidUpto = DateTime.UtcNow.AddMinutes(15),
                                Otp = otp,
                                Use = (int)OtpUse.ForgotClientPassword
                            };

                            await _accountRepository.AddOtpAsync(otpStorage);
                            return (true, "OTP Genrate and send successfully.");
                        }
                        else
                        {
                            _logger.LogError("An error occurred while generation OTP for user email {Email}", inOtpRequestDto.Email);
                            return (false, "OTP generation failed.");
                        }
                    }
                    else
                    {
                        DateTime currentDateTime = DateTime.UtcNow;
                        if (otpStorageResponse.ValidUpto > currentDateTime)
                        {
                            await _emailService.SendOtpEmailAsync(inOtpRequestDto.Email!, otpStorageResponse!.Otp!, "Ayerhs - Forgot Password Code", "Your One Time Password (OTP) is: ", true);
                            return (true, "OTP Genrate and send successfully.");
                        }
                        else
                        {
                            var otp = _otpHelper.GenerateOtpAsync();
                            if (otp != null)
                            {
                                await _emailService.SendOtpEmailAsync(inOtpRequestDto.Email!, otp, "Ayerhs - Forgot Password Code", "Your One Time Password (OTP) is: ", true);
                                var otpStorage = new OtpStorage
                                {
                                    Email = inOtpRequestDto.Email,
                                    GeneratedOn = DateTime.UtcNow,
                                    ValidUpto = DateTime.UtcNow.AddMinutes(15),
                                    Otp = otp
                                };

                                await _accountRepository.UpdateOtpAsync(otpStorage);
                                return (true, "OTP Genrate and send successfully.");
                            }
                            else
                            {
                                _logger.LogError("An error occurred while generation OTP for user email {Email}", inOtpRequestDto.Email);
                                return (false, "OTP generation failed.");
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogError("Invalid OTP use provided.");
                    return (false, "Invalid OTP use provided.");
                }
            }
            else
            {
                _logger.LogError("{Email} is not registered with application.", inOtpRequestDto.Email);
                return (false, $"{inOtpRequestDto.Email} is not registered with application.");
            }
        }

        /// <summary>
        /// Verifies an OTP for a given email address.
        /// </summary>
        /// <param name="inOtpVerificationDto">Data for OTP verification (email and OTP).</param>
        /// <returns>True if OTP is valid, False otherwise (with error message).</returns>
        public async Task<(bool, string)> OtpVerificationAsync(InOtpVerificationDto inOtpVerificationDto)
        {
            var existingClient = await _accountRepository.GetClientByEmailAsync(inOtpVerificationDto.Email!);
            if (existingClient != null)
            {
                var existingOtp = await _accountRepository.GetOtpStorageByEmailAsync(inOtpVerificationDto.Email!);
                if (existingOtp != null)
                {
                    if (existingOtp.Otp == inOtpVerificationDto.Otp)
                    {
                        await _accountRepository.VerifyClientAsync(existingClient);
                        _logger.LogInformation("OTP verification successfull {Email}", inOtpVerificationDto.Email);
                        return (true, $"OTP verification successfull {inOtpVerificationDto.Email}");
                    }
                    else
                    {
                        _logger.LogError("Wrong OTP provided by client {Email}", inOtpVerificationDto.Email);
                        return (false, $"Wrong OTP provided by client {inOtpVerificationDto.Email}");
                    }
                }
                else
                {
                    _logger.LogError("Invalid OTP verification request for client {Email}", inOtpVerificationDto.Email);
                    return (false, $"Invalid OTP verification request for client {inOtpVerificationDto.Email}");
                }
            }
            else
            {
                _logger.LogError("{Email} is not registered with application.", inOtpVerificationDto.Email);
                return (false, $"{inOtpVerificationDto.Email} is not registered with application.");
            }
        }

        /// <summary>
        /// Resets a client's password based on the provided information in the `inForgotClientPassword` object.
        /// </summary>
        /// <param name="inForgotClientPassword">An object containing the client's email and potentially their new password.</param>
        /// <returns>A tuple indicating success (bool) and an optional message (string) related to the operation.</returns>
        public async Task<(bool, string)> ForgotClientPasswordAsync(InForgotClientPassword inForgotClientPassword)
        {
            try
            {
                var client = await _accountRepository.GetClientByEmailAsync(inForgotClientPassword.ClientEmail!);
                if (client != null)
                {
                    var salt = client.Salt;
                    var hashedNewPassword = HashPassword(inForgotClientPassword.ClientPassword!, salt!);
                    if (hashedNewPassword != null)
                    {
                        client.ClientPassword = hashedNewPassword;
                        await _accountRepository.UpdateClientAsync(client);
                        return (true, $"Password changed successfully for User {inForgotClientPassword.ClientEmail}");
                    }
                    else
                    {
                        _logger.LogError("An error occurred while hashing password.");
                        return (false, "An error occurred while hashing password.");
                    }
                }
                else
                {
                    _logger.LogError("User {Email} not found", inForgotClientPassword.ClientEmail);
                    return (false, $"User not registered with the application. {inForgotClientPassword.ClientEmail}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while forgoting client password {Message}", ex.Message);
                return (false, ex.Message);
            }
        }
    }
}
