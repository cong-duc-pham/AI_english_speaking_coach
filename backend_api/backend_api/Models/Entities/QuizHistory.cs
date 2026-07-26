using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_api.Models.Entities
{
    [Table("QuizHistory")]
    public class QuizHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int TotalQuestions { get; set; } = 5;

        public int CorrectAnswers { get; set; } = 0;

        public int XpEarned { get; set; } = 0;

        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }
}
