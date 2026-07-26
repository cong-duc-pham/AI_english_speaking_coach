using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_api.Models.Entities
{
    [Table("ChatMessages")]
    public class ChatMessage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SessionId { get; set; }

        [ForeignKey("SessionId")]
        public ChatSession? Session { get; set; }

        [Required]
        [StringLength(10)]
        public string Sender { get; set; } = "user"; // "user" or "ai"

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? CorrectedText { get; set; }

        public string? BetterWayToSay { get; set; }

        public string? GrammarExplanation { get; set; }

        public string? VietnameseTranslation { get; set; }

        public string? PhoneticIPA { get; set; }

        [StringLength(500)]
        public string? AudioUrl { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
