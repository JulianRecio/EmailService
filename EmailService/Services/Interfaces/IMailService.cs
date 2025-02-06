using EmailService.Dtos;

namespace EmailService.Services.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(SendMailDto sendEmailDto);
    }
}
