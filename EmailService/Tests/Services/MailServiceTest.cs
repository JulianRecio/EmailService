using EmailService.Data;
using EmailService.Dtos;
using EmailService.Entities;
using EmailService.Services.Implementations;
using EmailService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EmailService.Tests.Services
{
    public class MailServiceTests
    {
        private readonly MailService _mailService;
        private readonly UserDbContext _dbContext;

        public MailServiceTests()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new UserDbContext(options);
            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var scopeMock = new Mock<IServiceScope>();
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock.Setup(sp => sp.GetService(typeof(UserDbContext)))
                .Returns(_dbContext);
            scopeMock.Setup(s => s.ServiceProvider).Returns(serviceProviderMock.Object);
            scopeFactoryMock.Setup(sf => sf.CreateScope()).Returns(scopeMock.Object);

            var loggerMock = new Mock<ILogger<MailService>>();
            var emailProviderMock = new Mock<IEmailProvider>();

            _mailService = new MailService(
                scopeFactoryMock.Object,
                new List<IEmailProvider> { emailProviderMock.Object },
                loggerMock.Object,
                new ConfigurationBuilder().Build()
            );
        }

        [Fact]
        public async Task SendEmailAsync_ShouldLogError_WhenNoProvidersWork()
        {
            // Arrange
            _dbContext.Users.Add(new User { Id = Guid.NewGuid(), Email = "test@example.com" });
            await _dbContext.SaveChangesAsync();

            var request = new SendMailDto
            {
                To = "recipient@example.com",
                Subject = "Test Subject",
                Body = "Test Body"
            };

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _mailService.SendEmailAsync(request, "test@example.com"));

            // Assert
            Assert.Equal("No email provider available", exception.Message);
        }

        [Fact]
        public async Task SendEmailAsync_ShouldThrowException_WhenSenderDoesNotExist()
        {
            // Arrange
            var sender = "nonexistent@example.com";
            var request = new SendMailDto
            {
                To = "recipient@example.com",
                Subject = "Test Subject",
                Body = "Test Body"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _mailService.SendEmailAsync(request, sender));

            Assert.Equal($"Sender user not found for email: {sender}", exception.Message);
        }

        [Fact]
        public async Task MailsSentToday_ShouldReturnCorrectCount()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "user@example.com" };
            _dbContext.Users.Add(user);

            _dbContext.EmailRecipts.Add(new EmailRecipt
            {
                Id = Guid.NewGuid(),
                User = user,
                DateTime = DateTime.UtcNow,
                To = "recipient@example.com",
                Subject = "Test",
                Body = "Body"
            });

            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _mailService.MailsSentToday();

            // Assert
            Assert.Single(result);
            Assert.Equal(user.Id, result[0].UserId);
            Assert.Equal(user.Email, result[0].Email);
            Assert.Equal(1, result[0].SentCount);
        }

    }
}
