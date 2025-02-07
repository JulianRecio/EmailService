using EmailService.Controllers;
using EmailService.Dtos;
using EmailService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace EmailService.Tests.Controllers
{
    public class MailControllerTest
    {
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly MailController _controller;

        public MailControllerTest()
        {
            _mailServiceMock = new Mock<IMailService>();
            _controller = new MailController(_mailServiceMock.Object);
        }

        private void SetAuthorizationHeader(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) }),
            };

            var token = tokenHandler.CreateEncodedJwt(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) })
            });

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task SendEmail_ShouldReturnOk_WhenEmailSentSuccessfully()
        {
            // Arrange
            SetAuthorizationHeader("test@example.com");

            var sendMailDto = new SendMailDto
            {
                To = "recipient@example.com",
                Subject = "Test Subject",
                Body = "Test Body"
            };

            _mailServiceMock.Setup(m => m.SendEmailAsync(It.IsAny<SendMailDto>(), "test@example.com"))
                .Returns(Task.CompletedTask);

            // Act
            var result = _controller.sendEmail(sendMailDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("email sent!", okResult.Value);
        }


        [Fact]
        public async Task RecoverStats_ShouldReturnOk_WithMailStats()
        {
            // Arrange
            var stats = new List<UserMailCountDto>
        {
            new UserMailCountDto { UserId = Guid.NewGuid(), Email = "user@example.com", SentCount = 5 }
        };

            _mailServiceMock.Setup(m => m.MailsSentToday()).ReturnsAsync(stats);

            // Act
            var result = await _controller.recoverStats();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStats = Assert.IsType<List<UserMailCountDto>>(okResult.Value);
            Assert.Single(returnedStats);
            Assert.Equal(5, returnedStats[0].SentCount);
        }
    }
}
