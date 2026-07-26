using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_api.Models.Entities
{
    [Table("UserPathProgress")]
    public class UserPathProgress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int StepId { get; set; }

        [ForeignKey("StepId")]
        public PathStep? Step { get; set; }

        public bool IsCompleted { get; set; } = false;

        public int BestScore { get; set; } = 0;

        public DateTime? CompletedAt { get; set; }
    }
}
