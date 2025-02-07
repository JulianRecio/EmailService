using System.Linq.Expressions;
using EmailService.Data;
using EmailService.Dtos;
using EmailService.Entities;
using EmailService.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EmailService.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly UserDbContext _dbContext;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new UserDbContext(options);
            _configurationMock = new Mock<IConfiguration>();

            _configurationMock.Setup(c => c["AppSettings:Token"])
                .Returns("supersecretkeythatisverysecure");
            _configurationMock.Setup(c => c["AppSettings:Issuer"])
                .Returns("testIssuer");
            _configurationMock.Setup(c => c["AppSettings:Audience"])
                .Returns("testAudience");

            _authService = new AuthService(_dbContext, _configurationMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateNewUser_WhenEmailIsNotTaken()
        {
            // Arrange
            var userDto = new UserDto { Email = "newuser@example.com", Password = "password123" };

            // Act
            var result = await _authService.RegisterAsync(userDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newuser@example.com", result.Email);
            Assert.False(string.IsNullOrEmpty(result.PasswordHash));
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnNull_WhenEmailIsAlreadyRegistered()
        {
            // Arrange
            var userDto = new UserDto { Email = "existinguser@example.com", Password = "password123" };
            _dbContext.Users.Add(new User { Email = "existinguser@example.com", PasswordHash = "hash" });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _authService.RegisterAsync(userDto);

            // Assert
            Assert.Null(result);
        }

    }
}
