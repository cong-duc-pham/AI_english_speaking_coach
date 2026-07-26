using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_api.Models.Entities
{
    [Table("PathSteps")]
    public class PathStep
    {
        [Key]
        public int Id { get; set; }

        public int PathId { get; set; }

        [ForeignKey("PathId")]
        public LearningPath? Path { get; set; }

        public int TopicId { get; set; }

        [ForeignKey("TopicId")]
        public Topic? Topic { get; set; }

        public int StepOrder { get; set; }

        public int MinScoreToPass { get; set; } = 60;

        // Navigation properties
        public ICollection<UserPathProgress> UserProgresses { get; set; } = new List<UserPathProgress>();
    }
}
