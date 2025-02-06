using EmailService.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<EmailRecipt> EmailRecipts { get; set; }
    }

}
