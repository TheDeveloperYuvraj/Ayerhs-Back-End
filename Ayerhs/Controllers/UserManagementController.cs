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

        #region Partitions Management Endpoints
        /// <summary>
        /// Adds a partition to the user management system.
        /// </summary>
        /// <param name="partitionName">The name of the partition to add.</param>
        /// <returns>An IActionResult object indicating the outcome of the operation.</returns>
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [Route("AddPartition/{partitionName}")]
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
                    return Ok(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: message, errorCode: CustomErrorCodes.AddPartitionError, txn: ConstantData.GenerateTransactionId(), returnValue: message));
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
                    return Ok(new ApiResponse<List<Partition>>(status: "Success", statusCode: 200, response: 1, successMessage: "Partitions list fetched successfully.", txn: ConstantData.GenerateTransactionId(), returnValue: partitions));
                }
                else
                {
                    _logger.LogError("Partitions list is empty.");
                    return Ok(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: "Partitions are not there.", errorCode: CustomErrorCodes.GetPartitionsError, txn: ConstantData.GenerateTransactionId(), returnValue: null));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting partitions {Message}", ex.Message);
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UserManagementUnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: ex.Message));
            }
        }

        /// <summary>
        /// Updates a partition asynchronously based on the provided name.
        /// </summary>
        /// <param name="inUpdatePartition">The entity contains id and name of the partition to be updated.</param>
        /// <returns>
        /// On success, a 200 OK response with a success message.
        /// On error, a 400 BadRequest response with an error message.
        /// </returns>
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [Route("UpdatePartition")]
        [HttpPut]
        public async Task<IActionResult> UpdatePartition(InUpdatePartition inUpdatePartition)
        {
            try
            {
                var (success, message) = await _userService.UpdatePartitionAsync(inUpdatePartition);
                if (success)
                {
                    _logger.LogInformation("{Message}", message);
                    return Ok(new ApiResponse<string>(status: "Success", statusCode: 200, response: 1, successMessage: message, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                }
                else
                {
                    _logger.LogError("An error occurred while updating partition {Message}", message);
                    return Ok(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: message, errorCode: CustomErrorCodes.UpdatePartitionError, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting partitions {Message}", ex.Message);
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UserManagementUnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: ex.Message));
            }
        }

        /// <summary>
        /// Deletes a partition asynchronously.
        /// </summary>
        /// <param name="id">The partition ID (must be positive).</param>
        /// <returns>Success/Failure response with details.</returns>
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [Route("DeletePartition/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeletePartition(int id)
        {
            try
            {
                if (id > 0)
                {
                    var (success, message) = await _userService.DeletePartitionAsync(id);
                    if (success)
                    {
                        _logger.LogInformation("{Message}", message);
                        return Ok(new ApiResponse<string>(status: "Success", statusCode: 200, response: 1, successMessage: message, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                    }
                    else
                    {
                        _logger.LogError("An error occurred while deleting partition {Message}", message);
                        return Ok(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: message, errorCode: CustomErrorCodes.DeletePartitionError, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                    }
                }
                else
                {
                    _logger.LogError("Negative ID value provide. {Id}", id);
                    string errMsg = $"Invalid ID Provided {id}";
                    return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: errMsg, errorCode: CustomErrorCodes.DeletePartitionNegativeIdError, txn: ConstantData.GenerateTransactionId(), returnValue: errMsg));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting partitions {Message}", ex.Message);
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UserManagementUnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: ex.Message));
            }
        }
        #endregion

        /// <summary>
        /// Adds a new group to the system.
        /// </summary>
        /// <param name="inAddGroupDto">An object containing details of the group to be added.</param>
        /// <returns>An IActionResult representing the HTTP response with details of the operation.</returns>
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [Route("AddGroup")]
        [HttpPost]
        public async Task<IActionResult> AddGroup(InAddGroupDto inAddGroupDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var (success, message) = await _userService.AddGroupAsync(inAddGroupDto);
                    if (success)
                    {
                        _logger.LogInformation("{Message}", message);
                        return Ok(new ApiResponse<string>(status: "Success", statusCode: 200, response: 1, successMessage: message, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                    }
                    else
                    {
                        _logger.LogError("Error occurred while adding group {Message}", message);
                        return Ok(new ApiResponse<string>(status: "Error", statusCode: 200, response: 0, errorMessage: message, errorCode: CustomErrorCodes.AddGroupError, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                    }
                }
                else
                {
                    _logger.LogError("Invalid Modal Sate");
                    string errMsg = "Invalid Modal Sate";
                    return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: errMsg, errorCode: CustomErrorCodes.UserManagementValidationError, txn: ConstantData.GenerateTransactionId(), returnValue: errMsg));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding partition {Message}", ex.Message);
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UserManagementUnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a list of groups based on the provided partition ID.
        /// </summary>
        /// <param name="partitionId">The ID of the partition to retrieve groups for. 0 retrieves all groups.</param>
        /// <returns>An IActionResult representing the HTTP response.</returns>
        [ProducesResponseType(typeof(ApiResponse<List<Group>>), 200)]
        [Route("GetGroups/{partitionId}")]
        [HttpGet]
        public async Task<IActionResult> GetGroups(int partitionId)
        {
            try
            {
                if (partitionId >= 0)
                {
                    var res = await _userService.GetGroupsAsync(partitionId);
                    if (res != null)
                    {
                        string message = "Group List Fetched Successfully.";
                        _logger.LogInformation("{Message}", message);
                        return Ok(new ApiResponse<List<Group>>(status: "Success", statusCode: 200, response: 1, successMessage: message, txn: ConstantData.GenerateTransactionId(), returnValue: res));
                    }
                    else
                    {
                        string message = "An error occurred while getting groups list.";
                        _logger.LogError("{Message}", message);
                        return Ok(new ApiResponse<string>(status: "Error", statusCode: 200, response: 0, errorMessage: message, errorCode: CustomErrorCodes.GetGroupsError, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                    }
                }
                else
                {
                    string message = $"Invalid Partition ID provided {partitionId}";
                    _logger.LogError("An error occurred while adding partition {Message}", message);
                    return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: message, errorCode: CustomErrorCodes.UserManagementValidationError, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding partition {Message}", ex.Message);
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UserManagementUnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: ex.Message));
            }
        }

        /// <summary>
        /// Updates a group asynchronously.
        /// </summary>
        /// <param name="inUpdateGroupDto">The DTO containing the updated group information.</param>
        /// <returns>An IActionResult containing the API response.</returns>
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [Route("UpdateGroup")]
        [HttpPut]
        public async Task<IActionResult> UpdateGroup(InUpdateGroupDto inUpdateGroupDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var (success, message) = await _userService.UpdateGroupAsync(inUpdateGroupDto);
                    if (success)
                    {
                        _logger.LogInformation("{Message}", message);
                        return Ok(new ApiResponse<string>(status: "Success", statusCode: 200, response: 1, successMessage: message, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                    }
                    else
                    {
                        _logger.LogError("Error occurred while updating group {Message}", message);
                        return Ok(new ApiResponse<string>(status: "Error", statusCode: 200, response: 0, errorMessage: message, errorCode: CustomErrorCodes.UpdateGroupError, txn: ConstantData.GenerateTransactionId(), returnValue: message));
                    }
                }
                else
                {
                    string errMsg = "Invalid Modal Sate";
                    _logger.LogError("{Message}", errMsg);
                    return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 400, response: 0, errorMessage: errMsg, errorCode: CustomErrorCodes.UserManagementValidationError, txn: ConstantData.GenerateTransactionId(), returnValue: errMsg));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding partition {Message}", ex.Message);
                return BadRequest(new ApiResponse<string>(status: "Error", statusCode: 500, response: 0, errorMessage: ex.Message, errorCode: CustomErrorCodes.UserManagementUnknownError, txn: ConstantData.GenerateTransactionId(), returnValue: ex.Message));
            }
        }
    }
}
