using EmailService.Dtos;
using EmailService.Services.Interfaces;

namespace EmailService.Services.Implementations
{
    public class MailService : IMailService
    {
        private readonly IEnumerable<IEmailProvider> providers;
        private readonly ILogger<MailService> logger;

        public MailService(IEnumerable<IEmailProvider> providers, ILogger<MailService> logger)
        {
            this.providers = providers;
            this.logger = logger;
        }

        public async Task SendEmailAsync(SendMailDto sendEmailDto)
        {
            foreach (var provider in providers)
            {
                try {
                    if (await provider.SendEmailAsync(sendEmailDto)) {
                        return;
                    }
                }catch (Exception e){
                    logger.LogError($"Error sending with {provider.GetType().Name}: {e.Message}");
                }
                
            }
            throw new Exception("No provider abvilable");

        }
    }
}
