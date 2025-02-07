namespace EmailService.Dtos
{
    public class UserMailCountDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public int SentCount { get; set; }
    }
}
