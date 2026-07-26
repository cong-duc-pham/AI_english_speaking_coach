using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_api.Models.Entities
{
    [Table("SessionAssessments")]
    public class SessionAssessment
    {
        [Key]
        public int Id { get; set; }

        public Guid SessionId { get; set; }

        [ForeignKey("SessionId")]
        public ChatSession? Session { get; set; }

        public int GrammarScore { get; set; } = 0;

        public int VocabularyScore { get; set; } = 0;

        public int FluencyScore { get; set; } = 0;

        public int PronunciationScore { get; set; } = 0;

        public int OverallScore { get; set; } = 0;

        [StringLength(10)]
        public string EstimatedCEFR { get; set; } = "B1"; // A1, A2, B1, B2, C1

        public string? DetailedFeedback { get; set; }

        public string? KeyImprovementPoints { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
