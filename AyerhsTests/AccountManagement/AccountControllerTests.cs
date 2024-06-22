using Ayerhs.Controllers;
using Ayerhs.Core.Entities.AccountManagement;
using Ayerhs.Core.Entities.Utility;
using Ayerhs.Core.Interfaces.AccountManagement;
using Ayerhs.Core.Interfaces.Utility;
using Ayerhs.Infrastructure.External;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AyerhsTests.AccountManagement
{
    public class AccountControllerTests
    {
        private readonly Mock<ILogger<AccountController>> _mockLogger;
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly AccountController _controller;
        private readonly Faker<InRegisterClientDto> _fakerRegisterClientDto;
        private readonly Faker<InLoginClientDto> _fakerLoginClientDto;
        private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;

        public AccountControllerTests()
        {
            _mockLogger = new Mock<ILogger<AccountController>>();
            _mockAccountService = new Mock<IAccountService>();
            _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
            _mockAccountRepository = new Mock<IAccountRepository>();
            _controller = new AccountController(_mockLogger.Object, _mockJwtTokenGenerator.Object, _mockAccountService.Object, _mockAccountRepository.Object);

            _fakerRegisterClientDto = new Faker<InRegisterClientDto>()
                .RuleFor(r => r.ClientName, f => f.Name.FullName())
                .RuleFor(r => r.ClientEmail, f => f.Internet.Email())
                .RuleFor(r => r.ClientPassword, f => f.Internet.Password());

            _fakerLoginClientDto = new Faker<InLoginClientDto>()
                .RuleFor(r => r.ClientEmail, f => f.Internet.Email())
                .RuleFor(r => r.ClientPassword, f => f.Internet.Password());
        }

        private void SetModelStateValid()
        {
            _controller.ModelState.Clear();
        }

        private static string GenerateValidToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("10x5kWfclNAwNq3Ou04wiWArVWtIC+HuHhEg5PLI5aw=");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, "TestUser")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [Fact]
        public async Task RegisterClient_ValidModel_ReturnsOk()
        {
            // Arrange
            var inRegisterClientDto = _fakerRegisterClientDto.Generate();
            _mockAccountService.Setup(x => x.RegisterClientAsync(It.IsAny<InRegisterClientDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RegisterClient(inRegisterClientDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Success", apiResponse.Status);
        }

        [Fact]
        public async Task RegisterClient_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var inRegisterClientDto = new InRegisterClientDto(); // Invalid model
            _controller.ModelState.AddModelError("Error", "Model is invalid");

            // Act
            var result = await _controller.RegisterClient(inRegisterClientDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", apiResponse.Status);
        }

        [Fact]
        public async Task RegisterClient_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var inRegisterClientDto = _fakerRegisterClientDto.Generate();
            _mockAccountService.Setup(x => x.RegisterClientAsync(It.IsAny<InRegisterClientDto>())).Throws(new System.Exception("Error"));

            // Act
            var result = await _controller.RegisterClient(inRegisterClientDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", apiResponse.Status);
        }

        [Fact]
        public async Task LoginClient_ValidModel_ReturnsOk()
        {
            // Arrange
            SetModelStateValid();
            var inLoginClientDto = _fakerLoginClientDto.Generate();
            var client = new Clients { ClientId = "1", ClientEmail = inLoginClientDto.ClientEmail, ClientUsername = "TestUser" };

            _mockAccountService.Setup(x => x.LoginClientAsync(It.IsAny<InLoginClientDto>())).ReturnsAsync(client);
            var validToken = GenerateValidToken();
            _mockJwtTokenGenerator.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(validToken);

            // Act
            var result = await _controller.LoginClient(inLoginClientDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<LoginResponseDto>>(okResult.Value);
            Assert.Equal("Success", apiResponse.Status);
            Assert.NotNull(apiResponse.ReturnValue!.Token);
            Assert.NotNull(apiResponse.ReturnValue.Client);
            Assert.NotNull(apiResponse.ReturnValue.Claims);
        }

        [Fact]
        public async Task LoginClient_InvalidCredentials_ReturnsOkWithError()
        {
            // Arrange
            var inLoginClientDto = _fakerLoginClientDto.Generate();
            _mockAccountService.Setup(x => x.LoginClientAsync(It.IsAny<InLoginClientDto>())).ReturnsAsync((Clients)null!);

            // Act
            var result = await _controller.LoginClient(inLoginClientDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Error", apiResponse.Status);
            Assert.Equal("Invalid Credentials", apiResponse.ErrorMessage);
        }

        [Fact]
        public async Task LoginClient_AccountLocked_ReturnsOkWithError()
        {
            // Arrange
            var inLoginClientDto = _fakerLoginClientDto.Generate();
            var lockedClient = new Clients { ClientEmail = inLoginClientDto.ClientEmail, IsLocked = true, LockedUntil = System.DateTime.UtcNow.AddMinutes(5) };
            _mockAccountService.Setup(x => x.LoginClientAsync(It.IsAny<InLoginClientDto>())).ReturnsAsync((Clients)null!);
            _mockAccountRepository.Setup(x => x.GetClientByEmailAsync(It.IsAny<string>())).ReturnsAsync(lockedClient);

            // Act
            var result = await _controller.LoginClient(inLoginClientDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Error", apiResponse.Status);
            Assert.Contains("Account is locked until", apiResponse.ErrorMessage);
        }

        [Fact]
        public async Task LoginClient_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var inLoginClientDto = new InLoginClientDto(); // Invalid model
            _controller.ModelState.AddModelError("Error", "Model is invalid");

            // Act
            var result = await _controller.LoginClient(inLoginClientDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", apiResponse.Status);
        }

        [Fact]
        public async Task LoginClient_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var inLoginClientDto = _fakerLoginClientDto.Generate();
            _mockAccountService.Setup(x => x.LoginClientAsync(It.IsAny<InLoginClientDto>())).Throws(new System.Exception("Error"));

            // Act
            var result = await _controller.LoginClient(inLoginClientDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", apiResponse.Status);
        }
    }
}
