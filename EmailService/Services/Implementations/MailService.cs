using EmailService.Data;
using EmailService.Dtos;
using EmailService.Entities;
using EmailService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Services.Implementations
{
    public class MailService : IMailService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IEnumerable<IEmailProvider> providers;
        private readonly ILogger<MailService> logger;
        private readonly IConfiguration configuration;

        public MailService(
            IServiceScopeFactory scopeFactory,
            IEnumerable<IEmailProvider> providers,
            ILogger<MailService> logger,
            IConfiguration configuration)
        {
            this.scopeFactory = scopeFactory;
            this.providers = providers;
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task SendEmailAsync(SendMailDto request, string sender)
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();

            try
            {
                var senderUser = await dbContext.Users
                    .Include(u => u.EmailRecipts)
                    .FirstOrDefaultAsync(u => u.Email == sender);

                if (senderUser == null)
                {
                    throw new InvalidOperationException($"Sender user not found for email: {sender}");
                }

                int sentEmailsToday = await dbContext.EmailRecipts
                    .CountAsync(e =>
                        e.User.Id == senderUser.Id &&  
                        e.DateTime.Date == DateTime.UtcNow.Date);

                foreach (var provider in providers)
                {
                    try
                    {
                        if (await provider.SendEmailAsync(request))
                        {
                            var emailReceipt = new EmailRecipt
                            {
                                Id = Guid.NewGuid(),
                                User = senderUser,  
                                DateTime = DateTime.UtcNow,
                                To = request.To,
                                Subject = request.Subject,
                                Body = request.Body
                            };

                            dbContext.EmailRecipts.Add(emailReceipt);
                            await dbContext.SaveChangesAsync();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Error sending with {provider.GetType().Name}: {ex.Message}");
                    }
                }

                throw new InvalidOperationException("No email provider available");
            }
            finally
            {
                scope.Dispose();
            }
        }

        public async Task<int> MailsSentToday()
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            DateTime dateTime = DateTime.Now;

            return await dbContext.EmailRecipts.CountAsync(e => e.DateTime.Date == dateTime.Date);
        }


    }
}
