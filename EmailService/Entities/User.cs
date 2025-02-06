namespace EmailService.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public String Email { get; set; }
        public String PasswordHash { get; set; } = string.Empty;
        public String Role { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiaryTime { get; set; }
        public ICollection<EmailRecipt> EmailRecipts { get; set; }
    }
}
    