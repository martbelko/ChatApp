using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wa_api.Models
{
	[NotMapped]
	[Index(nameof(Username), IsUnique = true)]
	public class GenericUser
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		public string Username { get; set; } = null!;
	}

	[Index(nameof(Email), IsUnique = true)]
	public class User : GenericUser
	{
		[Required]
		public ICollection<Message> Messages { get; set; } = new List<Message>();

		[Required]
		public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

		[Required]
		public string Email { get; set; } = null!;

		[Required]
		public bool EmailVerified { get; set; } = false;

		[Required]
		[DontShare]
		public Password Password { get; set; } = null!;

		[Required]
		[DontShare]
		public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
	}
}
