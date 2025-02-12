using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using User.API.Controllers;
using User.API.Contracts;
using User.API.Data;
using User.API.ModelDtos;
using Xunit;

namespace User.API.Tests.Controller
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthManager> _mockAuthManager;
        private readonly AuthController _controller;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public AuthControllerTests()
        {
            _mockAuthManager = new Mock<IAuthManager>();
            _mockConfiguration = new Mock<IConfiguration>();
            _controller = new AuthController(_mockAuthManager.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task Register_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "Test123!",
                FirstName = "Test",
                LastName = "User",
                UserName = "testuser"
            };

            _mockAuthManager.Setup(x => x.Register(registerDto))
                .ReturnsAsync(new List<IdentityError>());

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Register_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "short",
                FirstName = "Test",
                LastName = "User",
                UserName = "testuser"
            };

            var errors = new List<IdentityError>
            {
                new IdentityError { Code = "PasswordTooShort", Description = "Password is too short" }
            };

            _mockAuthManager.Setup(x => x.Register(registerDto))
                .ReturnsAsync(errors);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("PasswordTooShort"));
        }

        [Theory]
        [InlineData("", "Password123!")]
        [InlineData("test@example.com", "")]
        [InlineData(null, "Password123!")]
        [InlineData("test@example.com", null)]
        [InlineData("invalid-email", "Password123!")]
        public async Task Register_InvalidInputData_ReturnsBadRequest(string email, string password)
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = email,
                Password = password,
                FirstName = "Test",
                LastName = "User",
                UserName = "testuser"
            };

            var errors = new List<IdentityError>
            {
                new IdentityError { Code = "InvalidInput", Description = "Invalid input data" }
            };

            _mockAuthManager.Setup(x => x.Register(registerDto))
                .ReturnsAsync(errors);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginDto = new AuthDto
            {
                Email = "test@example.com",
                Password = "Test123!"
            };

            var authResponse = new AuthResponseDto
            {
                UserId = "test-user-id",
                Token = "test-token"
            };

            _mockAuthManager.Setup(x => x.Login(loginDto))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.Equal(authResponse.UserId, returnValue.UserId);
            Assert.Equal(authResponse.Token, returnValue.Token);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new AuthDto
            {
                Email = "test@example.com",
                Password = "WrongPassword!"
            };

            _mockAuthManager.Setup(x => x.Login(loginDto))
                .ReturnsAsync((AuthResponseDto)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Login_ExceptionThrown_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new AuthDto
            {
                Email = "test@example.com",
                Password = "Test123!"
            };

            _mockAuthManager.Setup(x => x.Login(loginDto))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}