using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_api.Models.Entities
{
    [Table("ChatSessions")]
    public class ChatSession
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(128)]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int TopicId { get; set; }

        [ForeignKey("TopicId")]
        public Topic? Topic { get; set; }

        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        public DateTime? EndTime { get; set; }

        public int DurationSeconds { get; set; } = 0;

        public int MessageCount { get; set; } = 0;

        [StringLength(20)]
        public string ClientPlatform { get; set; } = "Web"; // Web, MobileApp

        // Navigation properties
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
        public SessionAssessment? Assessment { get; set; }
    }
}
