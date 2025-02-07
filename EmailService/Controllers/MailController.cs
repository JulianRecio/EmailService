using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EmailService.Dtos;
using EmailService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [Route("mail/[controller]")]
    [ApiController]
    public class MailController(IMailService mailService) : ControllerBase
    {
        [Authorize]
        [HttpPost("send")]
        public IActionResult sendEmail(SendMailDto sendMailDto)
        {

            var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            try {
                mailService.SendEmailAsync(sendMailDto, email);
                return Ok("email sent!");
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("stats")]
        public async Task<IActionResult> recoverStats() {

            List<UserMailCountDto> mailsSent = await mailService.MailsSentToday();
            
            return Ok(mailsSent);
        }
    }
}
