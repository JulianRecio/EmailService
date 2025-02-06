namespace EmailService.Dtos
{
    public class SendMailDto
    {
        public string to { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
    }
}
