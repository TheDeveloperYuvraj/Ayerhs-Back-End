using Ayerhs.Core.Entities.AccountManagement;
using Ayerhs.Core.Entities.Utility;
using Ayerhs.Core.Interfaces.AccountManagement;
using Ayerhs.Core.Interfaces.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Ayerhs.Controllers
{
    /// <summary>
    /// Controller class for handling account management related operations.
    /// </summary>
    [ApiController]
    [Route("ayerhs-security/[controller]")]
    [Authorize]
    public class AccountController(ILogger<AccountController> logger, IJwtTokenGenerator jwtTokenGenerator, IAccountService accountService, IAccountRepository accountRepository) : ControllerBase
    {
        private readonly ILogger<AccountController> _logger = logger;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly IAccountService _accountService = accountService;
        private readonly IAccountRepository _accountRepository = accountRepository;

        #region Private Helper Methods for Account Controller
        /// <summary>
        /// Private method to extract claims from a JWT token.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        /// <returns>A dictionary containing claim types and their values.</returns>
        private static Dictionary<string, string> ExtractClaimsFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);

            var claims = new Dictionary<string, string>();
            foreach (var claim in jsonToken.Claims)
            {
                claims[claim.Type] = claim.Value;
            }

            return claims;
        }
        #endregion

        /// <summary>
        /// This endpoint allows a client user to log in to the system using their email address and password.
        /// </summary>
        /// <param name="inLoginClientDto">The data transfer object containing client login information (email and password).</param>
        /// <returns>An IActionResult representing the HTTP response for the login request. The response object may contain a JWT token, client user object, and claims if login is successful, or an error message otherwise.</returns>
        [ProducesResponseType(typeof(Clients), 200)]
        [Route("LoginClient")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginClient(InLoginClientDto inLoginClientDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var client = await _accountService.LoginClientAsync(inLoginClientDto);
                    if (client != null)
                    {
                        if (client.IsActive == true && client.Status == ClientStatus.Active)
                        {
                            var token = _jwtTokenGenerator.GenerateToken(client.ClientId!.ToString(), client.ClientEmail!, client.ClientUsername!);

                            var claims = ExtractClaimsFromToken(token);

                            var responseDto = new LoginResponseDto
                            {
                                Token = token,
                                Client = client,
                                Claims = claims
                            };
                            return Ok(new ApiResponse<LoginResponseDto>(status: "Success", statusCode: 200, response: 1, successMessage: "Login Successful", txn: ConstantData.GenerateTransactionId(), returnValue: responseDto));
                        }
                        else
                        {
                            return Ok(new ApiResponse<string>(status: "Error", statusCode: 200, response: 0, errorMessage: $"Account is not activated for user {inLoginClientDto.ClientEmail}. Please activate your account.", errorCode: CustomErrorCodes.AccountActivation, txn: ConstantData.GenerateTransactionId(), returnValue: null));
                        }
                    }
                    else
                    {
                        var lockedClient = await _accountRepository.GetClientByEmailAsync(inLoginClientDto.ClientEmail!);
                        if (lockedClient != null && lockedClient.IsLocked)
                        {
                            return Ok(new ApiResponse<string>(status: "Error", statusCode: 200, response: 0, errorMessage: $"Account is locked until {lockedClient.LockedUntil}", errorCode: CustomErrorCodes.AccountLocked, txn: ConstantData.GenerateTransactionId(), returnValue: null));
                        }
                        else
                        {
                            return Ok(new ApiResponse<string>(status: "Error", statusCode: 200, response: 0, errorMessage: "Invalid Credentials", errorCode: CustomErrorCodes.InvalidCredentials, txn: ConstantData.GenerateTransactionId(), returnValue: null));
                        }
                    }
                }
                else
                {
                    string validationErrors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                    _logger.LogError("Model validation failed for client registration. Validation errors: {ValidationErrors}", validationErrors);
                    return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: "Invalid Model State", errorCode: CustomErrorCodes.ValidationError, txn: ConstantData.GenerateTransactionId(), returnValue: null));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while login client: {Message}", ex.Message);
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: null));
            }
        }

        /// <summary>
        /// Registers a new client user using the provided information.
        /// </summary>
        /// <param name="inRegisterClientDto">The data transfer object containing client registration information.</param>
        /// <returns>An IActionResult representing the HTTP response for the registration request.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("RegisterClient")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterClient(InRegisterClientDto inRegisterClientDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogDebug("Starting client registration process with data: {InRegisterClientDto}", inRegisterClientDto);
                    await _accountService.RegisterClientAsync(inRegisterClientDto);
                    _logger.LogInformation("Client registered successfully.");
                    return Ok(new ApiResponse<string>(status: "Success", statusCode: 200, response: 1, successMessage: "Client registered successfully", txn: ConstantData.GenerateTransactionId(), returnValue: null));
                }
                else
                {
                    string validationErrors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                    _logger.LogError("Model validation failed for client registration. Validation errors: {ValidationErrors}", validationErrors);
                    return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: "Invalid Model State", errorCode: CustomErrorCodes.ValidationError, txn: ConstantData.GenerateTransactionId(), returnValue: null));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering client: {Message}", ex.Message);
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: null));
            }
        }

        /// <summary>
        /// Retrieves a list of all registered clients in the system.
        /// This endpoint requires authorization (bearer token) to be accessed.
        /// </summary>
        /// <returns>An `IActionResult` containing the list of retrieved clients or an error message if unsuccessful.</returns>
        [ProducesResponseType(typeof(List<Clients>), 200)]
        [Route("GetClients")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetClients()
        {
            try
            {
                _logger.LogInformation("Getting client list.");
                var result = await _accountService.GetClientsAsync();
                if (result != null)
                {
                    return Ok(new ApiResponse<List<Clients>>(status: "Success", statusCode: 200, response: 1, successMessage: "Clients fetched successfully.", txn: ConstantData.GenerateTransactionId(), returnValue: result));
                }
                else
                {
                    return Ok(new ApiResponse<string>(status: "Error", statusCode: 404, response: 0, errorMessage: "No registered clients found.", errorCode: CustomErrorCodes.NullRegisteredClients, txn: ConstantData.GenerateTransactionId(), returnValue: null));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting clients list. {Message}", ex.Message);
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: null));
            }
        }

        /// <summary>
        /// Generates an OTP (One-Time Password) and sends it to the specified email address.
        /// </summary>
        /// <param name="inOtpRequestDto">The request object containing the email address for which to generate OTP.</param>
        /// <returns>An ApiResponse object indicating the success or failure of the operation.</returns>
        [ProducesResponseType(typeof(string), 200)]
        [Route("OtpGenerationAndEmail")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> OtpGenerationAndEmail(InOtpRequestDto inOtpRequestDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var (success, message) = await _accountService.OtpGenerationAndEmailAsync(inOtpRequestDto);
                    if (success)
                    {
                        _logger.LogInformation("OTP generated and sent successfully on {Email}", inOtpRequestDto.Email);
                        return Ok(new ApiResponse<string>(status: "Success", statusCode: 200, response: 1, successMessage: message, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                    }
                    else
                    {
                        _logger.LogError("An error occurred while generating OTP for client {Email}", inOtpRequestDto.Email);
                        return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 200, response: 0, errorMessage: message, errorCode: CustomErrorCodes.OtpGeneration, txn: ConstantData.GenerateTransactionId(), returnValue: null));
                    }
                }
                else
                {
                    string validationErrors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                    _logger.LogError("Model validation failed for OTP generation. Validation errors: {ValidationErrors}", validationErrors);
                    return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: "Invalid Model State", errorCode: CustomErrorCodes.ValidationError, txn: ConstantData.GenerateTransactionId(), returnValue: null));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating OTP: {Message}", ex.Message);
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: null));
            }
        }

        /// <summary>
        /// Verifies an OTP for a given email address.
        /// </summary>
        /// <param name="inOtpVerificationDto">Data for OTP verification (email and OTP).</param>
        /// <returns>API response containing success/failure status and message.</returns>
        [ProducesResponseType(typeof(string), 200)]
        [Route("OtpVerification")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> OtpVerification(InOtpVerificationDto inOtpVerificationDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var (success, message) = await _accountService.OtpVerificationAsync(inOtpVerificationDto);
                    if (success)
                    {
                        _logger.LogInformation("OTP verification successfully on {Email}", inOtpVerificationDto.Email);
                        return Ok(new ApiResponse<string>(status: "Success", statusCode: 200, response: 1, successMessage: message, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                    }
                    else
                    {
                        _logger.LogError("An error occurred while verifying OTP for client {Email}", inOtpVerificationDto.Email);
                        return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 200, response: 0, errorMessage: message, errorCode: CustomErrorCodes.OtpVerification, txn: ConstantData.GenerateTransactionId(), returnValue: null));
                    }
                }
                else
                {
                    string validationErrors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                    _logger.LogError("Model validation failed for OTP generation. Validation errors: {ValidationErrors}", validationErrors);
                    return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: "Invalid Model State", errorCode: CustomErrorCodes.ValidationError, txn: ConstantData.GenerateTransactionId(), returnValue: null));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating OTP: {Message}", ex.Message);
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: null));
            }
        }

        /// <summary>
        /// Resets a client's password based on their email.
        /// </summary>
        /// <param name="inForgotClientPassword">Email address of the client requesting password reset.</param>
        /// <returns>An HTTP response indicating success or failure.</returns>
        [ProducesResponseType(typeof(string), 200)]
        [Route("ForgotClientPassword")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotClientPassword(InForgotClientPassword inForgotClientPassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var (success, message) = await _accountService.ForgotClientPasswordAsync(inForgotClientPassword);
                    if (success)
                    {
                        _logger.LogInformation("Password changed for Client {Email}", inForgotClientPassword.ClientEmail);
                        return Ok(new ApiResponse<string>(status: "Success", statusCode: 200, response: 1, successMessage: message, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                    }
                    else
                    {
                        _logger.LogError("An error occurred while forgoting password of client {Email}", inForgotClientPassword.ClientEmail);
                        return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 200, response: 0, errorMessage: message, errorCode: CustomErrorCodes.ForgotClientPassword, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                    }
                }
                else
                {
                    string validationErrors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                    _logger.LogError("Model validation failed for OTP generation. Validation errors: {ValidationErrors}", validationErrors);
                    return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: "Invalid Model State", errorCode: CustomErrorCodes.ValidationError, txn: ConstantData.GenerateTransactionId(), returnValue: null));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating OTP: {Message}", ex.Message);
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: null));
            }
        }
    }
}
