using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wa_api.Models
{
	public class Password
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		public byte[] Salt { get; set; } = null!;

		[Required]
		public byte[] Hash { get; set; } = null!;

		[Required]
		public int UserId { get; set; }

		[Required]
		public User User { get; set; } = null!;
	}
}
