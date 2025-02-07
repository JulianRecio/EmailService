using EmailService.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<EmailRecipt> EmailRecipts { get; set; }
    }

}
