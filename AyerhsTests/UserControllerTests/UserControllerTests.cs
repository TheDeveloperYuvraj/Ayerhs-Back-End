using Ayerhs.Controllers;
using Ayerhs.Core.Entities.UserManagement;
using Ayerhs.Core.Entities.Utility;
using Ayerhs.Core.Interfaces.UserManagement;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Moq;

namespace AyerhsTests.UserControllerTests
{
    public class UserControllerTests
    {
        private readonly Mock<ILogger<UserManagementController>> _mockLogger;
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserManagementController _controller;
        private readonly Faker _faker;
        private readonly Faker<InAddGroupDto> _groupDtoFaker;
        private readonly Faker<Group> _groupFaker;
        private readonly Faker<InUpdateGroupDto> _groupUpdateDtoFaker;

        public UserControllerTests()
        {
            _mockLogger = new Mock<ILogger<UserManagementController>>();
            _mockUserService = new Mock<IUserService>();
            _controller = new UserManagementController(_mockLogger.Object, _mockUserService.Object);
            _faker = new Faker();

            _groupDtoFaker = new Faker<InAddGroupDto>()
                .RuleFor(g => g.GroupName, f => f.Company.CompanyName())
                .RuleFor(g => g.PartitionId, f => f.Random.Int(1, 100));

            _groupFaker = new Faker<Group>()
                .RuleFor(g => g.GroupName, f => f.Company.CompanyName())
                .RuleFor(g => g.PartitionId, f => f.Random.Int(1, 100))
                .RuleFor(g => g.IsActive, f => true)
                .RuleFor(g => g.IsDeleted, f => false)
                .RuleFor(g => g.GroupCreatedOn, f => DateTime.UtcNow)
                .RuleFor(g => g.GroupUpdatedOn, f => DateTime.UtcNow);

            _groupUpdateDtoFaker = new Faker<InUpdateGroupDto>()
                .RuleFor(g => g.NewGroupName, f => f.Company.CompanyName())
                .RuleFor(g => g.PartitionId, f => f.Random.Int(1, 100))
                .RuleFor(g => g.Id, f => f.Random.Int(1, 100));
        }

        #region Unit Test Cases for Partitions
        [Fact]
        public async Task AddPartition_Success()
        {
            // Arrange
            var partitionName = _faker.Lorem.Word();
            var successMessage = $"{partitionName} created successfully.";
            _mockUserService.Setup(s => s.AddPartitionAsync(partitionName)).ReturnsAsync((true, successMessage));

            // Act
            var result = await _controller.AddPartition(partitionName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Success", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(1, response.Response);
            Assert.Equal(successMessage, response.ReturnValue);
        }

        [Fact]
        public async Task AddPartition_DuplicateName_Error()
        {
            // Arrange
            var partitionName = _faker.Lorem.Word();
            var errorMessage = $"Partition Name {partitionName} is used. Please provide unique partition name.";
            _mockUserService.Setup(s => s.AddPartitionAsync(partitionName)).ReturnsAsync((false, errorMessage));

            // Act
            var result = await _controller.AddPartition(partitionName);

            // Assert
            var badRequestResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(400, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errorMessage, response.ReturnValue);
        }

        [Fact]
        public async Task AddPartition_UnknownError()
        {
            // Arrange
            var partitionName = _faker.Lorem.Word();
            var exceptionMessage = "An unexpected error occurred.";
            _mockUserService.Setup(s => s.AddPartitionAsync(partitionName)).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.AddPartition(partitionName);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(500, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(exceptionMessage, response.ReturnValue);
        }

        [Fact]
        public async Task GetPartitions_Success()
        {
            // Arrange
            var partitions = new List<Partition>
        {
            new() { PartitionId = _faker.Random.Guid().ToString(), PartitionName = _faker.Lorem.Word(), PartitionCreatedOn = DateTime.UtcNow, PartitionUpdatedOn = DateTime.UtcNow },
            new() { PartitionId = _faker.Random.Guid().ToString(), PartitionName = _faker.Lorem.Word(), PartitionCreatedOn = DateTime.UtcNow, PartitionUpdatedOn = DateTime.UtcNow }
        };
            _mockUserService.Setup(s => s.GetPartitionsAsync()).ReturnsAsync(partitions);

            // Act
            var result = await _controller.GetPartitions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<List<Partition>>>(okResult.Value);
            Assert.Equal("Success", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(1, response.Response);
            Assert.Equal(partitions, response.ReturnValue);
        }

        [Fact]
        public async Task GetPartitions_EmptyList()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetPartitionsAsync()).ReturnsAsync((List<Partition>)null!);

            // Act
            var result = await _controller.GetPartitions();

            // Assert
            var badRequestResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(400, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(null!, response.ReturnValue);
        }

        [Fact]
        public async Task GetPartitions_UnknownError()
        {
            // Arrange
            var exceptionMessage = "An unexpected error occurred.";
            _mockUserService.Setup(s => s.GetPartitionsAsync()).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetPartitions();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(500, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(exceptionMessage, response.ReturnValue);
        }

        [Fact]
        public async Task UpdatePartition_Success()
        {
            // Arrange
            var inUpdatePartition = new InUpdatePartition
            {
                Id = _faker.Random.Int(),
                PartitionName = _faker.Lorem.Word()
            };
            var successMessage = $"{inUpdatePartition.PartitionName} is successfully updated.";
            _mockUserService.Setup(s => s.UpdatePartitionAsync(inUpdatePartition)).ReturnsAsync((true, successMessage));

            // Act
            var result = await _controller.UpdatePartition(inUpdatePartition);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Success", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(1, response.Response);
            Assert.Equal(successMessage, response.ReturnValue);
        }

        [Fact]
        public async Task UpdatePartition_DuplicateName_Error()
        {
            // Arrange
            var inUpdatePartition = new InUpdatePartition
            {
                Id = _faker.Random.Int(),
                PartitionName = _faker.Lorem.Word()
            };
            var errorMessage = $"Duplicate partition name found. {inUpdatePartition.PartitionName}";
            _mockUserService.Setup(s => s.UpdatePartitionAsync(inUpdatePartition)).ReturnsAsync((false, errorMessage));

            // Act
            var result = await _controller.UpdatePartition(inUpdatePartition);

            // Assert
            var badRequestResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(400, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errorMessage, response.ReturnValue);
        }

        [Fact]
        public async Task UpdatePartition_InvalidId_Error()
        {
            // Arrange
            var inUpdatePartition = new InUpdatePartition
            {
                Id = _faker.Random.Int(),
                PartitionName = _faker.Lorem.Word()
            };
            var errorMessage = $"Invalid partition Id provided. {inUpdatePartition.Id}";
            _mockUserService.Setup(s => s.UpdatePartitionAsync(inUpdatePartition)).ReturnsAsync((false, errorMessage));

            // Act
            var result = await _controller.UpdatePartition(inUpdatePartition);

            // Assert
            var badRequestResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(400, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errorMessage, response.ReturnValue);
        }

        [Fact]
        public async Task UpdatePartition_UnknownError()
        {
            // Arrange
            var inUpdatePartition = new InUpdatePartition
            {
                Id = _faker.Random.Int(),
                PartitionName = _faker.Lorem.Word()
            };
            var exceptionMessage = "An unexpected error occurred.";
            _mockUserService.Setup(s => s.UpdatePartitionAsync(inUpdatePartition)).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.UpdatePartition(inUpdatePartition);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(500, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(exceptionMessage, response.ReturnValue);
        }

        [Fact]
        public async Task DeletePartition_Success()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var successMessage = "Partition removed successfully.";
            _mockUserService.Setup(s => s.DeletePartitionAsync(id)).ReturnsAsync((true, successMessage));

            // Act
            var result = await _controller.DeletePartition(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Success", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(1, response.Response);
            Assert.Equal(successMessage, response.ReturnValue);
        }

        [Fact]
        public async Task DeletePartition_InvalidId()
        {
            // Arrange
            var id = -1;
            var errorMessage = $"Invalid ID Provided {id}";

            // Act
            var result = await _controller.DeletePartition(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(400, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errorMessage, response.ReturnValue);
        }

        [Fact]
        public async Task DeletePartition_InvalidId_Error()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var errorMessage = "Invalid ID provided.";
            _mockUserService.Setup(s => s.DeletePartitionAsync(id)).ReturnsAsync((false, errorMessage));

            // Act
            var result = await _controller.DeletePartition(id);

            // Assert
            var badRequestResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(400, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errorMessage, response.ReturnValue);
        }

        [Fact]
        public async Task DeletePartition_UnknownError()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var exceptionMessage = "An unexpected error occurred.";
            _mockUserService.Setup(s => s.DeletePartitionAsync(id)).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.DeletePartition(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(500, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(exceptionMessage, response.ReturnValue);
        }
        #endregion

        #region Unit Test Cases for Groups
        [Fact]
        public async Task AddGroup_ValidModel_ReturnsSuccess()
        {
            var groupDto = _groupDtoFaker.Generate();
            _mockUserService.Setup(s => s.AddGroupAsync(It.IsAny<InAddGroupDto>())).ReturnsAsync((true, "Group added successfully"));

            var result = await _controller.AddGroup(groupDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Success", apiResponse.Status);
        }

        [Fact]
        public async Task AddGroup_InvalidModel_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("GroupName", "Required");
            var groupDto = _groupDtoFaker.Generate();

            var result = await _controller.AddGroup(groupDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", apiResponse.Status);
        }

        [Fact]
        public async Task AddGroup_ServiceFailure_ReturnsError()
        {
            var groupDto = _groupDtoFaker.Generate();
            _mockUserService.Setup(s => s.AddGroupAsync(It.IsAny<InAddGroupDto>())).ReturnsAsync((false, "Error adding group"));

            var result = await _controller.AddGroup(groupDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Error", apiResponse.Status);
        }

        [Fact]
        public async Task GetGroups_ValidPartitionId_ReturnsGroupList()
        {
            var partitionId = 1;
            var groups = _groupFaker.Generate(3);
            _mockUserService.Setup(s => s.GetGroupsAsync(partitionId)).ReturnsAsync(groups);

            var result = await _controller.GetGroups(partitionId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<List<Group>>>(okResult.Value);
            Assert.Equal("Success", apiResponse.Status);
            Assert.Equal(3, apiResponse.ReturnValue!.Count);
        }

        [Fact]
        public async Task GetGroups_InvalidPartitionId_ReturnsBadRequest()
        {
            var partitionId = -1;

            var result = await _controller.GetGroups(partitionId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", apiResponse.Status);
        }

        [Fact]
        public async Task GetGroups_ServiceFailure_ReturnsError()
        {
            var partitionId = 1;
            _mockUserService.Setup(s => s.GetGroupsAsync(partitionId)).ReturnsAsync((List<Group>)null!);

            var result = await _controller.GetGroups(partitionId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Error", apiResponse.Status);
        }

        [Fact]
        public async Task UpdateGroup_SuccessfulUpdate_ReturnsOk()
        {
            // Arrange
            var groupDto = _groupUpdateDtoFaker.Generate();
            _mockUserService.Setup(s => s.UpdateGroupAsync(It.IsAny<InUpdateGroupDto>()))
                .ReturnsAsync((true, "Group successfully updated."));

            // Act
            var result = await _controller.UpdateGroup(groupDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Success", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(1, response.Response);
            Assert.Equal("Group successfully updated.", response.ReturnValue);
        }

        [Fact]
        public async Task UpdateGroup_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var groupDto = _groupUpdateDtoFaker.Generate();
            _controller.ModelState.AddModelError("NewGroupName", "Required");

            // Act
            var result = await _controller.UpdateGroup(groupDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(400, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal("Invalid Modal Sate", response.ReturnValue);
        }

        [Fact]
        public async Task UpdateGroup_PartitionNotFound_ReturnsOkWithError()
        {
            // Arrange
            var groupDto = _groupUpdateDtoFaker.Generate();
            _mockUserService.Setup(s => s.UpdateGroupAsync(It.IsAny<InUpdateGroupDto>()))
                .ReturnsAsync((false, "Invalid Partition Provided"));

            // Act
            var result = await _controller.UpdateGroup(groupDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal("Invalid Partition Provided", response.ReturnValue);
        }

        [Fact]
        public async Task UpdateGroup_GroupNotFound_ReturnsOkWithError()
        {
            // Arrange
            var groupDto = _groupUpdateDtoFaker.Generate();
            _mockUserService.Setup(s => s.UpdateGroupAsync(It.IsAny<InUpdateGroupDto>()))
                .ReturnsAsync((false, "Invalid Group Id Provided"));

            // Act
            var result = await _controller.UpdateGroup(groupDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal("Invalid Group Id Provided", response.ReturnValue);
        }

        [Fact]
        public async Task UpdateGroup_DuplicateGroupName_ReturnsOkWithError()
        {
            // Arrange
            var groupDto = _groupUpdateDtoFaker.Generate();
            _mockUserService.Setup(s => s.UpdateGroupAsync(It.IsAny<InUpdateGroupDto>()))
                .ReturnsAsync((false, "Duplicate Group Name under Same Partition. Please Choose Unique Name."));

            // Act
            var result = await _controller.UpdateGroup(groupDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal("Duplicate Group Name under Same Partition. Please Choose Unique Name.", response.ReturnValue);
        }

        [Fact]
        public async Task UpdateGroup_UpdateFailure_ReturnsOkWithError()
        {
            // Arrange
            var groupDto = _groupUpdateDtoFaker.Generate();
            _mockUserService.Setup(s => s.UpdateGroupAsync(It.IsAny<InUpdateGroupDto>()))
                .ReturnsAsync((false, "Error occurred while updating group."));

            // Act
            var result = await _controller.UpdateGroup(groupDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal("Error occurred while updating group.", response.ReturnValue);
        }

        [Fact]
        public async Task UpdateGroup_ExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            var groupDto = _groupUpdateDtoFaker.Generate();
            _mockUserService.Setup(s => s.UpdateGroupAsync(It.IsAny<InUpdateGroupDto>()))
                .ThrowsAsync(new Exception("Exception occurred"));

            // Act
            var result = await _controller.UpdateGroup(groupDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(500, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal("Exception occurred", response.ReturnValue);
        }

        [Fact]
        public async Task SoftDeleteGroup_ValidId_ReturnsSuccess()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var message = $"Group with ID {id} successfully removed temporary.";
            _mockUserService.Setup(s => s.SoftDeleteGroupAsync(id)).ReturnsAsync((true, message));

            // Act
            var result = await _controller.SoftDeleteGroup(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Success", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(1, response.Response);
            Assert.Equal(message, response.ReturnValue);
        }

        [Fact]
        public async Task SoftDeleteGroup_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var id = 0;
            var errMsg = $"Invalid Group ID {id} provided.";

            // Act
            var result = await _controller.SoftDeleteGroup(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(400, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errMsg, response.ReturnValue);
        }

        [Fact]
        public async Task SoftDeleteGroup_GroupDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var errMsg = $"Group with ID {id} not found.";
            _mockUserService.Setup(s => s.SoftDeleteGroupAsync(id)).ReturnsAsync((false, errMsg));

            // Act
            var result = await _controller.SoftDeleteGroup(id);

            // Assert
            var notFoundResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(notFoundResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errMsg, response.ReturnValue);
        }

        [Fact]
        public async Task SoftDeleteGroup_GroupAlreadyDeleted_ReturnsConflict()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var errMsg = $"Group with ID {id} is already soft deleted.";
            _mockUserService.Setup(s => s.SoftDeleteGroupAsync(id)).ReturnsAsync((false, errMsg));

            // Act
            var result = await _controller.SoftDeleteGroup(id);

            // Assert
            var conflictResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(conflictResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errMsg, response.ReturnValue);
        }

        [Fact]
        public async Task SoftDeleteGroup_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var errMsg = $"Unauthorized access for Group ID {id}.";
            _mockUserService.Setup(s => s.SoftDeleteGroupAsync(id)).ThrowsAsync(new UnauthorizedAccessException(errMsg));

            // Act
            var result = await _controller.SoftDeleteGroup(id);

            // Assert
            var unauthorizedResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(unauthorizedResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(500, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errMsg, response.ReturnValue);
        }

        [Fact]
        public async Task SoftDeleteGroup_Exception_ReturnsServerError()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var errorMessage = "Exception error.";
            _mockUserService.Setup(s => s.SoftDeleteGroupAsync(id)).ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await _controller.SoftDeleteGroup(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(500, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errorMessage, response.ReturnValue);
        }

        [Fact]
        public async Task RecoverDeletedGroup_ValidId_ReturnsSuccess()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var message = $"Group with ID {id} successfully recovered.";
            _mockUserService.Setup(s => s.RecoverDeletedGroupAsync(id)).ReturnsAsync((true, message));

            // Act
            var result = await _controller.RecoverDeletedGroup(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Success", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(1, response.Response);
            Assert.Equal(message, response.ReturnValue);
        }

        [Fact]
        public async Task RecoverDeletedGroup_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var id = 0;
            var errMsg = $"Invalid Group ID {id} provided.";

            // Act
            var result = await _controller.RecoverDeletedGroup(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(400, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errMsg, response.ReturnValue);
        }

        [Fact]
        public async Task RecoverDeletedGroup_GroupDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var errMsg = $"Group with ID {id} not found.";
            _mockUserService.Setup(s => s.RecoverDeletedGroupAsync(id)).ReturnsAsync((false, errMsg));

            // Act
            var result = await _controller.RecoverDeletedGroup(id);

            // Assert
            var notFoundResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(notFoundResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errMsg, response.ReturnValue);
        }

        [Fact]
        public async Task RecoverDeletedGroup_GroupNotDeleted_ReturnsConflict()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var errMsg = $"Group with ID {id} is not deleted.";
            _mockUserService.Setup(s => s.RecoverDeletedGroupAsync(id)).ReturnsAsync((false, errMsg));

            // Act
            var result = await _controller.RecoverDeletedGroup(id);

            // Assert
            var conflictResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(conflictResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errMsg, response.ReturnValue);
        }

        [Fact]
        public async Task RecoverDeletedGroup_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var errMsg = $"Unauthorized access for Group ID {id}.";
            _mockUserService.Setup(s => s.RecoverDeletedGroupAsync(id)).ThrowsAsync(new UnauthorizedAccessException(errMsg));

            // Act
            var result = await _controller.RecoverDeletedGroup(id);

            // Assert
            var unauthorizedResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(unauthorizedResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(500, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errMsg, response.ReturnValue);
        }

        [Fact]
        public async Task RecoverDeletedGroup_Exception_ReturnsServerError()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var errorMessage = "Exception error.";
            _mockUserService.Setup(s => s.RecoverDeletedGroupAsync(id)).ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await _controller.RecoverDeletedGroup(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(500, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errorMessage, response.ReturnValue);
        }

        [Fact]
        public async Task DeleteGroup_ValidId_ReturnsSuccess()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var message = $"Group with ID {id} successfully removed.";
            _mockUserService.Setup(s => s.DeleteGroupAsync(id)).ReturnsAsync((true, message));

            // Act
            var result = await _controller.DeleteGroup(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Success", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(1, response.Response);
            Assert.Equal(message, response.ReturnValue);
        }

        [Fact]
        public async Task DeleteGroup_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var id = 0;
            var errMsg = $"Invalid Group ID {id} provided.";

            // Act
            var result = await _controller.DeleteGroup(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(400, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errMsg, response.ReturnValue);
        }

        [Fact]
        public async Task DeleteGroup_GroupDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var errMsg = $"Group with ID {id} not found.";
            _mockUserService.Setup(s => s.DeleteGroupAsync(id)).ReturnsAsync((false, errMsg));

            // Act
            var result = await _controller.DeleteGroup(id);

            // Assert
            var notFoundResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(notFoundResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errMsg, response.ReturnValue);
        }

        [Fact]
        public async Task DeleteGroup_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var errMsg = $"Unauthorized access for Group ID {id}.";
            _mockUserService.Setup(s => s.DeleteGroupAsync(id)).ThrowsAsync(new UnauthorizedAccessException(errMsg));

            // Act
            var result = await _controller.DeleteGroup(id);

            // Assert
            var unauthorizedResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(unauthorizedResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(500, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errMsg, response.ReturnValue);
        }

        [Fact]
        public async Task DeleteGroup_Exception_ReturnsServerError()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var errorMessage = "Exception error.";
            _mockUserService.Setup(s => s.DeleteGroupAsync(id)).ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await _controller.DeleteGroup(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", response.Status);
            Assert.Equal(500, response.StatusCode);
            Assert.Equal(0, response.Response);
            Assert.Equal(errorMessage, response.ReturnValue);
        } 
        #endregion
    }
}
