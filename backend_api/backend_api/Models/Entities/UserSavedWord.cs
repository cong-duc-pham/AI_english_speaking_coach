using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_api.Models.Entities
{
    [Table("UserSavedWords")]
    public class UserSavedWord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        [StringLength(100)]
        public string Word { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string MeaningVi { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Phonetic { get; set; }

        public string? ExampleSentence { get; set; }

        public Guid? SourceMessageId { get; set; }

        [ForeignKey("SourceMessageId")]
        public ChatMessage? SourceMessage { get; set; }

        public int ReviewCount { get; set; } = 0;

        public double EaseFactor { get; set; } = 2.5;

        public DateTime NextReviewDate { get; set; } = DateTime.UtcNow;

        public bool IsMastered { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
