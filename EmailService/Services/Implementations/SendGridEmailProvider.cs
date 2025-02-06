using EmailService.Services.Interfaces;
using SendGrid.Helpers.Mail;
using SendGrid;
using EmailService.Dtos;


namespace EmailService.Services.Implementations
{
    public class SendGridEmailProvider : IEmailProvider
    {

        private readonly string apiKey;

        public SendGridEmailProvider() 
        {
            apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY")
                ?? throw new Exception("SendGrid API key missing");
        }

        public async Task<bool> SendEmailAsync(SendMailDto sendEmailDto)
        {
            var client = new SendGridClient(apiKey);
            var message = new SendGridMessage
            {
                From = new EmailAddress("julian.recio@ing.austral.edu.ar", "Email service"),
                Subject = sendEmailDto.Subject,
                HtmlContent = sendEmailDto.Body
            };
            message.AddTo(new EmailAddress(sendEmailDto.To));

            var response = await client.SendEmailAsync(message);
            return response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }

 
    }
}
