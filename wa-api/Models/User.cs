using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wa_api.Models
{
    [NotMapped]
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class GenericUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        [Personal]
        public ICollection<Message> Messages { get; set; } = new List<Message>();

        [Required]
        [Personal]
        public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

        [Required]
        [Internal]
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }

    public class User : GenericUser
    {
        [Required]
        [Personal]
        public Password Password { get; set; } = null!;
    }
}
