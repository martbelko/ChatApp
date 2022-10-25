using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wa_api.Models
{
	[Index(nameof(Username), IsUnique = true)]
	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		public string Username { get; set; } = null!;

		[Required]
		public ICollection<Message> Messages { get; set; } = new List<Message>();

		[Required]
		public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
	}
}
