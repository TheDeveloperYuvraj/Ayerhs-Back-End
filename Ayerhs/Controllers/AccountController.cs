using Ayerhs.Core.Entities.AccountManagement;
using Ayerhs.Core.Entities.Utility;
using Ayerhs.Core.Interfaces.AccountManagement;
using Ayerhs.Core.Interfaces.Utility;
using Ayerhs.Infrastructure.External;
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
        [Route("GetClients")]
        [HttpGet]
        [Authorize(Roles = "Admin, ClientManager")]
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
    }
}
