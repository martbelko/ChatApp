using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace wa_api.Models
{
	[Index(nameof(Content), IsUnique = true)]
	public class RefreshToken
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		public string Content { get; set; } = null!;

		[Required]
		public User Owner { get; set; } = null!;
	}
}
