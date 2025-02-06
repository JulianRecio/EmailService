using EmailService.Dtos;
using EmailService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [Route("mail/[controller]")]
    [ApiController]
    public class MailController(IMailService mailService) : ControllerBase
    {

        [HttpPost("send")]
        public async Task<IActionResult> sendEmail(SendMailDto sendMailDto) { 
            mailService.SendEmailAsync(sendMailDto);
            return Ok("email sent!");
        }
    }
}
