using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace wa_api.Models
{
	public class Conversation
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public ICollection<User> Members { get; set; } = new List<User>();

		public ICollection<Message> Messages { get; set; } = new List<Message>();
	}
}
