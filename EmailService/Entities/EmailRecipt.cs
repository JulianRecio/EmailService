namespace EmailService.Entities
{
    public class EmailRecipt
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public User User { get; set; }
    }
}
