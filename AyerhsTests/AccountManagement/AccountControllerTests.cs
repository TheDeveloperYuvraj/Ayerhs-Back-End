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
                Subject = new ClaimsIdentity(
                [
            new(ClaimTypes.Name, "TestUser")
                ]),
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
        public async Task LoginClient_InactiveAccount_ReturnsOkWithError()
        {
            // Arrange
            SetModelStateValid();
            var inLoginClientDto = _fakerLoginClientDto.Generate();
            var client = new Clients { ClientId = "1", ClientEmail = inLoginClientDto.ClientEmail, ClientUsername = "TestUser", IsActive = false, Status = ClientStatus.Inactive };

            _mockAccountService.Setup(x => x.LoginClientAsync(It.IsAny<InLoginClientDto>())).ReturnsAsync(client);

            // Act
            var result = await _controller.LoginClient(inLoginClientDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Error", apiResponse.Status);
            Assert.Contains("Account is not activated", apiResponse.ErrorMessage);
        }

        [Fact]
        public async Task LoginClient_JwtTokenGenerationFails_ReturnsBadRequest()
        {
            // Arrange
            SetModelStateValid();
            var inLoginClientDto = _fakerLoginClientDto.Generate();
            var client = new Clients { ClientId = "1", ClientEmail = inLoginClientDto.ClientEmail, ClientUsername = "TestUser", IsActive = true, Status = ClientStatus.Active };

            _mockAccountService.Setup(x => x.LoginClientAsync(It.IsAny<InLoginClientDto>())).ReturnsAsync(client);
            _mockJwtTokenGenerator.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Token generation failed"));

            // Act
            var result = await _controller.LoginClient(inLoginClientDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", apiResponse.Status);
            Assert.Contains("Token generation failed", apiResponse.ErrorMessage);
        }

        [Fact]
        public async Task LoginClient_AccountLockedWithoutLockedUntil_ReturnsOkWithError()
        {
            // Arrange
            var inLoginClientDto = _fakerLoginClientDto.Generate();
            var lockedClient = new Clients { ClientEmail = inLoginClientDto.ClientEmail, IsLocked = true, LockedUntil = null };
            _mockAccountService.Setup(x => x.LoginClientAsync(It.IsAny<InLoginClientDto>())).ReturnsAsync((Clients)null!);
            _mockAccountRepository.Setup(x => x.GetClientByEmailAsync(It.IsAny<string>())).ReturnsAsync(lockedClient);

            // Act
            var result = await _controller.LoginClient(inLoginClientDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Error", apiResponse.Status);
            Assert.Contains("Account is locked", apiResponse.ErrorMessage);
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

        [Fact]
        public async Task LoginClient_SuccessfulLogin_ReturnsOkWithToken()
        {
            // Arrange
            SetModelStateValid();
            var inLoginClientDto = _fakerLoginClientDto.Generate();
            var client = new Clients { ClientId = "1", ClientEmail = inLoginClientDto.ClientEmail, ClientUsername = "TestUser", IsActive = true, Status = ClientStatus.Active };
            var token = GenerateValidToken();

            _mockAccountService.Setup(x => x.LoginClientAsync(It.IsAny<InLoginClientDto>())).ReturnsAsync(client);
            _mockJwtTokenGenerator.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(token);

            // Act
            var result = await _controller.LoginClient(inLoginClientDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<LoginResponseDto>>(okResult.Value);
            Assert.Equal("Success", apiResponse.Status);
            Assert.NotNull(apiResponse.ReturnValue!.Token);
        }

        [Fact]
        public async Task LoginClient_InactiveAccountDifferentStatus_ReturnsOkWithError()
        {
            // Arrange
            SetModelStateValid();
            var inLoginClientDto = _fakerLoginClientDto.Generate();
            var client = new Clients { ClientId = "1", ClientEmail = inLoginClientDto.ClientEmail, ClientUsername = "TestUser", IsActive = false, Status = ClientStatus.Suspended };

            _mockAccountService.Setup(x => x.LoginClientAsync(It.IsAny<InLoginClientDto>())).ReturnsAsync(client);

            // Act
            var result = await _controller.LoginClient(inLoginClientDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Error", apiResponse.Status);
            Assert.Contains("Account is not activated", apiResponse.ErrorMessage);
        }

        [Fact]
        public async Task GetClients_AuthorizedRequest_ReturnsListOfClients()
        {
            // Arrange
            SetModelStateValid();
            var mockClients = new List<Clients>() { new() { ClientId = "1" }, new() { ClientId = "2" } };
            _mockAccountService.Setup(x => x.GetClientsAsync()).ReturnsAsync(mockClients);

            // Act
            var result = await _controller.GetClients();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<List<Clients>>>(okResult.Value);
            Assert.Equal("Success", apiResponse.Status);
            Assert.Equal(2, apiResponse.ReturnValue!.Count);
        }

        [Fact]
        public async Task GetClients_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            SetModelStateValid();
            _mockAccountService.Setup(x => x.GetClientsAsync()).Throws(new System.Exception("Error"));

            // Act
            var result = await _controller.GetClients();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", apiResponse.Status);
        }

        [Fact]
        public async Task GetClients_NoClientsFound_ReturnsNotFound()
        {
            // Arrange
            SetModelStateValid();
            _mockAccountService.Setup(x => x.GetClientsAsync())
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.GetClients();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Error", apiResponse.Status);
            Assert.Equal(404, apiResponse.StatusCode);
            Assert.Equal("No registered clients found.", apiResponse.ErrorMessage);
        }

        [Fact]
        public async Task OtpGenerationAndEmail_ValidEmail_ReturnsSuccess()
        {
            // Arrange
            var email = "test@example.com";
            _mockAccountService.Setup(x => x.OtpGenerationAndEmailAsync(It.Is<InOtpRequestDto>(dto => dto.Email == email)))
                .ReturnsAsync((true, "OTP generated and sent successfully"));

            // Act
            var result = await _controller.OtpGenerationAndEmail(new InOtpRequestDto { Email = email });

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Success", apiResponse.Status);
            Assert.Equal("OTP generated and sent successfully", apiResponse.SuccessMessage);
        }

        [Fact]
        public async Task OtpGenerationAndEmail_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.OtpGenerationAndEmail(new InOtpRequestDto());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", apiResponse.Status);
            Assert.Equal("Invalid Model State", apiResponse.ErrorMessage);
        }

        [Fact]
        public async Task OtpGenerationAndEmail_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var email = "test@example.com";
            _mockAccountService.Setup(x => x.OtpGenerationAndEmailAsync(It.Is<InOtpRequestDto>(dto => dto.Email == email)))
                .Throws(new Exception("Error"));

            // Act
            var result = await _controller.OtpGenerationAndEmail(new InOtpRequestDto { Email = email });

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", apiResponse.Status);
        }

        [Fact]
        public async Task OtpVerification_ValidModel_ValidOtp_ReturnsSuccess()
        {
            // Arrange
            SetModelStateValid();
            var inOtpVerificationDto = new InOtpVerificationDto { Email = "test@example.com", Otp = "123456" };
            _mockAccountService.Setup(x => x.OtpVerificationAsync(It.IsAny<InOtpVerificationDto>()))
                .ReturnsAsync((true, "OTP verification successful"));

            // Act
            var result = await _controller.OtpVerification(inOtpVerificationDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Success", apiResponse.Status);
            Assert.Equal("OTP verification successful", apiResponse.SuccessMessage);
        }

        [Fact]
        public async Task OtpVerification_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.OtpVerification(new InOtpVerificationDto());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", apiResponse.Status);
            Assert.Equal("Invalid Model State", apiResponse.ErrorMessage);
        }

        [Fact]
        public async Task OtpVerification_InvalidOtp_ReturnsBadRequest()
        {
            // Arrange
            SetModelStateValid();
            var inOtpVerificationDto = new InOtpVerificationDto { Email = "test@example.com", Otp = "invalid_otp" };
            _mockAccountService.Setup(x => x.OtpVerificationAsync(It.IsAny<InOtpVerificationDto>()))
                .ReturnsAsync((false, "Invalid OTP"));

            // Act
            var result = await _controller.OtpVerification(inOtpVerificationDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", apiResponse.Status);
            Assert.Equal("Invalid OTP", apiResponse.ErrorMessage);
        }

        [Fact]
        public async Task OtpVerification_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            SetModelStateValid();
            var inOtpVerificationDto = new InOtpVerificationDto { Email = "test@example.com", Otp = "123456" };
            _mockAccountService.Setup(x => x.OtpVerificationAsync(It.IsAny<InOtpVerificationDto>()))
                .Throws(new Exception("Error"));

            // Act
            var result = await _controller.OtpVerification(inOtpVerificationDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", apiResponse.Status);
        }

        [Fact]
        public async Task ForgotClientPassword_ValidModel_ReturnsOk()
        {
            // Arrange
            SetModelStateValid();
            var inForgotClientPassword = new InForgotClientPassword { ClientEmail = "test@email.com" };
            _mockAccountService.Setup(x => x.ForgotClientPasswordAsync(It.IsAny<InForgotClientPassword>()))
                .Returns(Task.FromResult((true, "Success message")));

            // Act
            var result = await _controller.ForgotClientPassword(inForgotClientPassword);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal("Success", apiResponse.Status);
        }

        [Fact]
        public async Task ForgotClientPassword_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var inForgotClientPassword = new InForgotClientPassword();
            _controller.ModelState.AddModelError("Error", "Model is invalid");

            // Act
            var result = await _controller.ForgotClientPassword(inForgotClientPassword);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", apiResponse.Status);
        }

        [Fact]
        public async Task ForgotClientPassword_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            SetModelStateValid();
            var inForgotClientPassword = new InForgotClientPassword { ClientEmail = "test@email.com" };
            _mockAccountService.Setup(x => x.ForgotClientPasswordAsync(It.IsAny<InForgotClientPassword>()))
              .Throws(new System.Exception("Error"));

            // Act
            var result = await _controller.ForgotClientPassword(inForgotClientPassword);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<string>>(badRequestResult.Value);
            Assert.Equal("Error", apiResponse.Status);
        }
    }
}
