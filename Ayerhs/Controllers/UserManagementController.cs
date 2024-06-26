using Ayerhs.Core.Entities.UserManagement;
using Ayerhs.Core.Entities.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ayerhs.Controllers
{
    [ApiController]
    [Route("ayerhs-security/[controller]")]
    [Authorize]
    public class UserManagementController(ILogger<UserManagementController> logger) : ControllerBase
    {
        private readonly ILogger<UserManagementController> _logger = logger;

        [ProducesResponseType(typeof(int), 200)]
        [Route("AddUser")]
        [HttpPost]
        public async Task<IActionResult> AddUser(InAddUser inAddUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Ok(inAddUser);
                }
                else
                {
                    _logger.LogError("Invalid Model State");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding user {Message}", ex.Message);
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UserManagementUnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: ex.Message));
            }
        }
    }
}
