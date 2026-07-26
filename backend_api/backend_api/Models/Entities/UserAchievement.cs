using System.ComponentModel.DataAnnotations.Schema;

namespace backend_api.Models.Entities
{
    [Table("UserAchievements")]
    public class UserAchievement
    {
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int AchievementId { get; set; }

        [ForeignKey("AchievementId")]
        public Achievement? Achievement { get; set; }

        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    }
}
