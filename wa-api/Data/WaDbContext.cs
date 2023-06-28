using Microsoft.EntityFrameworkCore;
using wa_api.Models;
using wa_api.Security;

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

			var (hash, salt) = SecurityUtils.GeneratePassword("testpassword");

			var password = new Password
			{
				Id = 1,
				Hash = hash,
				Salt = salt
			};

			var user = new User
			{
				Id = 1,
				Email = "testemail@test.com",
				EmailVerified = true,
				Username = "testusername"
			};

			password.UserId = user.Id;
			modelBuilder.Entity<Password>().HasData(password);
			modelBuilder.Entity<User>().HasData(user);
		}
	}
}
