namespace EmailService.Dtos
{
    public class SendMailDto
    {
        public required string To { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
    }
}
