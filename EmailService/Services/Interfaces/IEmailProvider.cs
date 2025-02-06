using EmailService.Dtos;

namespace EmailService.Services.Interfaces
{
    public interface IEmailProvider
    {
        Task<bool> SendEmailAsync(SendMailDto sendEmailDto);
    }
}
