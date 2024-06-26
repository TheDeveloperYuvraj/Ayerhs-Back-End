using Ayerhs.Core.Entities.UserManagement;
using Ayerhs.Core.Entities.Utility;
using Ayerhs.Core.Interfaces.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ayerhs.Controllers
{
    /// <summary>
    /// Controller for User Management operations.
    /// </summary>
    [ApiController]
    [Route("ayerhs-security/[controller]")]
    [Authorize]
    public class UserManagementController(ILogger<UserManagementController> logger, IUserService userService) : ControllerBase
    {
        private readonly ILogger<UserManagementController> _logger = logger;
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Adds a partition to the user management system.
        /// </summary>
        /// <param name="partitionName">The name of the partition to add.</param>
        /// <returns>An IActionResult object indicating the outcome of the operation.</returns>
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [Route("AddPartition")]
        [HttpPost]
        public async Task<IActionResult> AddPartition(string partitionName)
        {
            try
            {
                var (success, message) = await _userService.AddPartitionAsync(partitionName);
                if (success)
                {
                    _logger.LogInformation("{Message}", message);
                    return Ok(new ApiResponse<string>(status: "Success", statusCode: 200, response: 1, successMessage: message, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                }
                else
                {
                    _logger.LogError("Error occurred while creating partition {Message}", message);
                    return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: message, errorCode: CustomErrorCodes.AddPartitionError, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding partition {Message}", ex.Message);
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UserManagementUnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a list of partitions.
        /// </summary>
        /// <returns>On success, a 200 OK response containing an ApiResponse object with a list of Partition objects and a success message.</returns>
        [ProducesResponseType(typeof(ApiResponse<List<Partition>>), 200)]
        [Route("GetPartitions")]
        [HttpGet]
        public async Task<IActionResult> GetPartitions()
        {
            try
            {
                var partitions = await _userService.GetPartitionsAsync();
                if (partitions != null)
                {
                    _logger.LogInformation("Partitions list gets successfully.");
                    return Ok(new ApiResponse<List<Partition>>(status: "Success", statusCode: 200, response: 1, successMessage: "Partitions list fetched successfully.", txn: ConstantData.GenerateTransactionId(), returnValue: partitions ));
                }
                else
                {
                    _logger.LogError("Partitions list is empty.");
                    return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: "Partitions are not there.", errorCode: CustomErrorCodes.GetPartitionsError, txn: ConstantData.GenerateTransactionId(), returnValue: null));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting partitions {Message}", ex.Message);
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UserManagementUnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: ex.Message));
            }
        }
    }
}
