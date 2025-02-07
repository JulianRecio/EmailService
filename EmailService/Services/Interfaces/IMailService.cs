using EmailService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Services.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(SendMailDto sendEmailDto, string Sender);

        Task<List<UserMailCountDto>> MailsSentToday();
    }
}
