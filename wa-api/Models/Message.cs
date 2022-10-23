using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wa_api.Models
{
	public class Message
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		public string Content { get; set; }

		[Required]
		public DateTime Time { get; set; }

		[Required]
		public User Author { get; set; }

		[Required]
		public Conversation Conversation { get; set; }
	}
}
