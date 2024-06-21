using Ayerhs.Core.Entities.AccountManagement;
using Ayerhs.Core.Entities.Utility;
using Ayerhs.Core.Interfaces.AccountManagement;
using Ayerhs.Infrastructure.External;
using Microsoft.AspNetCore.Mvc;

namespace Ayerhs.Controllers
{
    /// <summary>
    /// Controller class for handling account management related operations.
    /// </summary>
    [ApiController]
    [Route("ayerhs-security/[controller]")]
    public class AccountController(ILogger<AccountController> logger, JwtTokenGenerator jwtTokenGenerator, IAccountService accountService) : ControllerBase
    {
        private readonly ILogger<AccountController> _logger = logger;
        private readonly JwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly IAccountService _accountService = accountService;

        /// <summary>
        /// Registers a new client user using the provided information.
        /// </summary>
        /// <param name="inRegisterClientDto">The data transfer object containing client registration information.</param>
        /// <returns>An IActionResult representing the HTTP response for the registration request.</returns>
        [Route("RegisterClient")]
        [HttpPost]
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

        [Route("LoginClient")]
        [HttpPost]
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
                        var responseDto = new LoginResponseDto
                        {
                            Token = token,
                            Client = client
                        };
                        return Ok(new ApiResponse<LoginResponseDto>(status: "Success", statusCode: 200, response: 1, successMessage: "Login Successful", txn: ConstantData.GenerateTransactionId(), returnValue: responseDto));
                    }
                    else
                    {
                        _logger.LogError("Invalid credentials provided.");
                        return Ok(new ApiResponse<string>(status: "Error", statusCode: 200, response: 0, errorMessage: "Invalid Credentials", errorCode: CustomErrorCodes.InvalidCredentials, txn: ConstantData.GenerateTransactionId(), returnValue: null));
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
    }
}
