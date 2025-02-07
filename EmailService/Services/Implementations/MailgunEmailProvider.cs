using EmailService.Dtos;
using EmailService.Services.Interfaces;
using RestSharp;
using RestSharp.Authenticators;

namespace EmailService.Services.Implementations
{
    public class MailgunEmailProvider : IEmailProvider
    {
        public async Task<bool> SendEmailAsync(SendMailDto sendEmailDto)
        {
            RestClient client = new RestClient(new RestClientOptions("https://api.mailgun.net/v3/sandboxe5179c4a0d5944e7aaa7c0486e22e0cd.mailgun.org/messages")
            {
                Authenticator = new HttpBasicAuthenticator("api", Environment.GetEnvironmentVariable("MAILGUN_API_KEY")
                 ?? throw new Exception("Mailgun API key missing"))
            });


            RestRequest request = new RestRequest();

            request.AddParameter("from", "Excited User <mailgun@sandboxe5179c4a0d5944e7aaa7c0486e22e0cd.mailgun.org>");
            request.AddParameter("to", sendEmailDto.To);
            request.AddParameter("subject", sendEmailDto.Subject);
            request.AddParameter("text", sendEmailDto.Body);

            var response = await client.ExecuteAsync(request);
            return response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }
    }
}
