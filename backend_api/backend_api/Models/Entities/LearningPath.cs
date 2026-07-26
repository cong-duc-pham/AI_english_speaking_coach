using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_api.Models.Entities
{
    [Table("LearningPaths")]
    public class LearningPath
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(150)]
        public string TitleVi { get; set; } = string.Empty;

        public string? Description { get; set; }

        [StringLength(20)]
        public string TargetLevel { get; set; } = "Basic";

        [StringLength(500)]
        public string? BannerUrl { get; set; }

        public bool IsPublished { get; set; } = true;

        // Navigation properties
        public ICollection<PathStep> Steps { get; set; } = new List<PathStep>();
    }
}
