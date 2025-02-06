using EmailService.Dtos;

namespace EmailService.Services.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(SendMailDto sendEmailDto, string Sender);

        Task<int> MailsSentToday();
    }
}
