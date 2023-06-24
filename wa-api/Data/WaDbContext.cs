using Microsoft.EntityFrameworkCore;
using wa_api.Models;

namespace wa_api.Data
{
    public class WaDbContext : DbContext
    {
        public WaDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Password> Passwords { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<Conversation> Conversations { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(p => p.Password)
                .WithOne(p => p.User)
                .HasForeignKey<Password>(p => p.UserId);
        }
    }
}
