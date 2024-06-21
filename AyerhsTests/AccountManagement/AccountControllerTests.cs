using Ayerhs.Controllers;
using Ayerhs.Core.Entities.AccountManagement;
using Ayerhs.Core.Entities.Utility;
using Ayerhs.Core.Interfaces.AccountManagement;
using Ayerhs.Infrastructure.External;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AyerhsTests.AccountManagement
{
    public class AccountControllerTests
    {
        private readonly Mock<ILogger<AccountController>> _mockLogger;
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly AccountController _controller;
        private readonly Faker<InRegisterClientDto> _faker;
        private readonly Mock<JwtTokenGenerator> _mockJwtTokenGenerator;

        public AccountControllerTests()
        {
            _mockLogger = new Mock<ILogger<AccountController>>();
            _mockAccountService = new Mock<IAccountService>();
            _mockJwtTokenGenerator = new Mock<JwtTokenGenerator>();
            _controller = new AccountController(_mockLogger.Object, _mockJwtTokenGenerator.Object, _mockAccountService.Object);

            _faker = new Faker<InRegisterClientDto>()
                .RuleFor(r => r.ClientName, f => f.Name.FullName())
                .RuleFor(r => r.ClientEmail, f => f.Internet.Email())
                .RuleFor(r => r.ClientPassword, f => f.Internet.Password());
        }

        [Fact]
        public async Task RegisterClient_ValidModel_ReturnsOk()
        {
            // Arrange
            var clientDto = _faker.Generate();
            _mockAccountService.Setup(s => s.RegisterClientAsync(clientDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RegisterClient(clientDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal(200, apiResponse.StatusCode);
            Assert.Equal("Success", apiResponse.Status);
            Assert.Equal(1, apiResponse.Response);
        }

        [Fact]
        public async Task RegisterClient_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var clientDto = _faker.Generate();
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.RegisterClient(clientDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal(400, apiResponse.StatusCode);
            Assert.Equal("Error", apiResponse.Status);
            Assert.Equal(0, apiResponse.Response);
            Assert.Equal(CustomErrorCodes.ValidationError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task RegisterClient_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var clientDto = _faker.Generate();
            var exceptionMessage = "An error occurred";
            _mockAccountService.Setup(s => s.RegisterClientAsync(clientDto)).ThrowsAsync(new System.Exception(exceptionMessage));

            // Act
            var result = await _controller.RegisterClient(clientDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal(500, apiResponse.StatusCode);
            Assert.Equal("Error", apiResponse.Status);
            Assert.Equal(0, apiResponse.Response);
            Assert.Equal(CustomErrorCodes.UnknownError, apiResponse.ErrorCode);
            Assert.Equal(exceptionMessage, apiResponse.ErrorMessage);
        }
    }
}
