using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_api.Models.Entities
{
    [Table("Topics")]
    public class Topic
    {
        [Key]
        public int Id { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(150)]
        public string TitleVi { get; set; } = string.Empty;

        [StringLength(20)]
        public string Difficulty { get; set; } = "Basic"; // Basic, Intermediate, Advanced

        [StringLength(100)]
        public string AIRole { get; set; } = "Native Speaker";

        [StringLength(100)]
        public string UserRole { get; set; } = "Learner";

        [Required]
        public string SystemPrompt { get; set; } = string.Empty;

        public string InitialGreeting { get; set; } = string.Empty;

        [StringLength(50)]
        public string IconName { get; set; } = "chat";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
        public ICollection<PathStep> PathSteps { get; set; } = new List<PathStep>();
    }
}
