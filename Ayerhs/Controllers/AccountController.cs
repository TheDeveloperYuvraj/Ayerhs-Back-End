using Ayerhs.Core.Entities.AccountManagement;
using Ayerhs.Core.Entities.Utility;
using Ayerhs.Core.Interfaces.AccountManagement;
using Microsoft.AspNetCore.Mvc;

namespace Ayerhs.Controllers
{
    /// <summary>
    /// Controller class for handling account management related operations.
    /// </summary>
    [ApiController]
    [Route("ayerhs-security/[controller]")]
    public class AccountController(IAccountService accountService) : ControllerBase
    {
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
                    await _accountService.RegisterClientAsync(inRegisterClientDto);
                    return Ok(new ApiResponse<string>(status: "Success", statusCode: 200, response: 1, successMessage: "Client registered successfully", txn: ConstantData.GenerateTransactionId(), returnValue: null)); 
                }
                else
                {
                    return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: "Invalid Model State", errorCode: CustomErrorCodes.ValidationError, txn: ConstantData.GenerateTransactionId(), returnValue: null));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: null));
            }
        }
    }
}
