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

        public UserControllerTests()
        {
            _mockLogger = new Mock<ILogger<UserManagementController>>();
            _mockUserService = new Mock<IUserService>();
            _controller = new UserManagementController(_mockLogger.Object, _mockUserService.Object);
            _faker = new Faker();
        }

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
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
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
            new Partition { PartitionId = _faker.Random.Guid().ToString(), PartitionName = _faker.Lorem.Word(), PartitionCreatedOn = DateTime.UtcNow, PartitionUpdatedOn = DateTime.UtcNow },
            new Partition { PartitionId = _faker.Random.Guid().ToString(), PartitionName = _faker.Lorem.Word(), PartitionCreatedOn = DateTime.UtcNow, PartitionUpdatedOn = DateTime.UtcNow }
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
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
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
    }
}
