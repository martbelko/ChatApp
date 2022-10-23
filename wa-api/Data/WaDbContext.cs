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

		public DbSet<User> Users { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<Conversation> Conversation { get; set; }
	}
}
