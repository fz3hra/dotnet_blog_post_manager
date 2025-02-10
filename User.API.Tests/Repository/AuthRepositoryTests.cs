using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using User.API.Data;
using User.API.ModelDtos;
using User.API.Repository;
using Xunit;

namespace User.API.Tests.Repository
{
    public class AuthRepositoryTests
    {
        private readonly Mock<UserManager<UserApi>> _mockUserManager;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthRepository _repository;

        public AuthRepositoryTests()
        {
            var userStoreMock = new Mock<IUserStore<UserApi>>();
            _mockUserManager = new Mock<UserManager<UserApi>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _mockMapper = new Mock<IMapper>();

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["JwtSettings:Key"]).Returns("your-test-jwt-key-that-is-long-enough-for-signing");
            _mockConfiguration.Setup(x => x["JwtSettings:Issuer"]).Returns("test-issuer");
            _mockConfiguration.Setup(x => x["JwtSettings:Audience"]).Returns("test-audience");
            _mockConfiguration.Setup(x => x["JwtSettings:DurationInMinutes"]).Returns("60");

            _repository = new AuthRepository(_mockUserManager.Object, _mockMapper.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task Register_ValidInput_ReturnsNoErrors()
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

            var user = new UserApi
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };

            _mockMapper.Setup(x => x.Map<UserApi>(registerDto)).Returns(user);
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<UserApi>(), registerDto.Password))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<UserApi>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _repository.Register(registerDto);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsError()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "existing@example.com",
                Password = "Test123!"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(registerDto.Email))
                .ReturnsAsync(new UserApi());

            // Act
            var result = await _repository.Register(registerDto);

            // Assert
            Assert.Contains(result, e => e.Code == "DuplicateEmail");
        }

        [Theory]
        [InlineData("", "password")]
        [InlineData("email@test.com", "")]
        public async Task Register_EmptyCredentials_ReturnsError(string email, string password)
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

            // Act
            var result = await _repository.Register(registerDto);

            // Assert
            Assert.Contains(result, e => e.Code == "InvalidInput");
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsAuthResponse()
        {
            // Arrange
            var loginDto = new AuthDto
            {
                Email = "test@example.com",
                Password = "Test123!"
            };

            var user = new UserApi
            {
                Id = "test-user-id",
                Email = loginDto.Email,
                UserName = "testuser"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
                .ReturnsAsync(true);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "User" });
            _mockUserManager.Setup(x => x.GetClaimsAsync(user))
                .ReturnsAsync(new List<Claim>());

            // Act
            var result = await _repository.Login(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.UserId);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsNull()
        {
            // Arrange
            var loginDto = new AuthDto
            {
                Email = "test@example.com",
                Password = "WrongPassword!"
            };

            var user = new UserApi { Email = loginDto.Email };

            _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
                .ReturnsAsync(false);

            // Act
            var result = await _repository.Login(loginDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshToken_ThrowsNotImplementedException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotImplementedException>(
                () => _repository.RefreshToken("refresh-token")
            );
        }
    }
}