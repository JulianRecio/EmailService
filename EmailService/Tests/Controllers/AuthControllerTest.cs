using EmailService.Controllers;
using EmailService.Dtos;
using EmailService.Entities;
using EmailService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EmailService.Tests.Controllers
{
    public class AuthControllerTest
    {
        public class AuthControllerTests
        {
            private readonly Mock<IAuthService> _authServiceMock;
            private readonly AuthController _authController;

            public AuthControllerTests()
            {
                _authServiceMock = new Mock<IAuthService>();
                _authController = new AuthController(_authServiceMock.Object);
            }

            [Fact]
            public async Task Register_ShouldReturnOk_WhenUserIsCreated()
            {
                // Arrange
                var request = new UserDto { Email = "user@example.com", Password = "password123" };
                var user = new User { Email = request.Email, PasswordHash = "hashedPassword" };

                _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(user);

                // Act
                var result = await _authController.Register(request);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnedUser = Assert.IsType<User>(okResult.Value);
                Assert.Equal(request.Email, returnedUser.Email);
            }

            [Fact]
            public async Task Register_ShouldReturnBadRequest_WhenEmailIsAlreadyRegistered()
            {
                // Arrange
                var request = new UserDto { Email = "user@example.com", Password = "password123" };

                _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync((User)null);

                // Act
                var result = await _authController.Register(request);

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
                Assert.Equal("Email already registered", badRequestResult.Value);
            }

            [Fact]
            public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
            {
                // Arrange
                var request = new UserDto { Email = "user@example.com", Password = "password123" };
                var tokenResponse = new TokenResponseDto { AccessToken = "jwtToken", RefreshToken = "refreshToken" };

                _authServiceMock.Setup(s => s.LoginAsync(request)).ReturnsAsync(tokenResponse);

                // Act
                var result = await _authController.Login(request);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnedTokens = Assert.IsType<TokenResponseDto>(okResult.Value);
                Assert.Equal("jwtToken", returnedTokens.AccessToken);
            }

            [Fact]
            public async Task Login_ShouldReturnBadRequest_WhenCredentialsAreInvalid()
            {
                // Arrange
                var request = new UserDto { Email = "user@example.com", Password = "wrongpassword" };

                _authServiceMock.Setup(s => s.LoginAsync(request)).ReturnsAsync((TokenResponseDto)null);

                // Act
                var result = await _authController.Login(request);

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
                Assert.Equal("Invalid email or password.", badRequestResult.Value);
            }

            [Fact]
            public async Task RefreshToken_ShouldReturnOk_WhenTokenIsValid()
            {
                // Arrange
                var request = new RefreshTokenRequestDto { UserId = Guid.NewGuid(), RefreshToken = "validRefreshToken" };
                var tokenResponse = new TokenResponseDto { AccessToken = "newAccessToken", RefreshToken = "newRefreshToken" };

                _authServiceMock.Setup(s => s.RefreshTokensAsync(request)).ReturnsAsync(tokenResponse);

                // Act
                var result = await _authController.RefreshToken(request);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnedTokens = Assert.IsType<TokenResponseDto>(okResult.Value);
                Assert.Equal("newAccessToken", returnedTokens.AccessToken);
            }

            [Fact]
            public async Task RefreshToken_ShouldReturnUnauthorized_WhenTokenIsInvalid()
            {
                // Arrange
                var request = new RefreshTokenRequestDto { UserId = Guid.NewGuid(), RefreshToken = "invalidToken" };

                _authServiceMock.Setup(s => s.RefreshTokensAsync(request)).ReturnsAsync((TokenResponseDto)null);

                // Act
                var result = await _authController.RefreshToken(request);

                // Assert
                var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
                Assert.Equal("Invalid refresh token.", unauthorizedResult.Value);
            }
        }
    }
}
