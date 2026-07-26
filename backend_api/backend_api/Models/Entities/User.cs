using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_api.Models.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        [StringLength(128)]
        public string Id { get; set; } = string.Empty; // Firebase Auth UID

        [Required]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? PhotoUrl { get; set; }

        [StringLength(20)]
        public string Level { get; set; } = "Basic"; // Basic, Intermediate, Advanced

        [StringLength(10)]
        public string PreferredAccent { get; set; } = "en-US"; // en-US, en-GB

        public int TotalPracticeMinutes { get; set; } = 0;

        public int ExperienceXP { get; set; } = 0;

        public int StreakDays { get; set; } = 0;

        public DateTime? LastActiveDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
        public ICollection<UserSavedWord> SavedWords { get; set; } = new List<UserSavedWord>();
        public ICollection<UserAchievement> Achievements { get; set; } = new List<UserAchievement>();
        public ICollection<UserPathProgress> PathProgresses { get; set; } = new List<UserPathProgress>();
        public ICollection<QuizHistory> QuizHistories { get; set; } = new List<QuizHistory>();
    }
}
