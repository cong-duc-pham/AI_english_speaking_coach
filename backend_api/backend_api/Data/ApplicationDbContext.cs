using Microsoft.EntityFrameworkCore;
using backend_api.Models.Entities;

namespace backend_api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<LearningPath> LearningPaths { get; set; }
        public DbSet<PathStep> PathSteps { get; set; }
        public DbSet<UserPathProgress> UserPathProgresses { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<SessionAssessment> SessionAssessments { get; set; }
        public DbSet<UserSavedWord> UserSavedWords { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<QuizHistory> QuizHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite Key for UserRole
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // Composite Key for UserAchievement
            modelBuilder.Entity<UserAchievement>()
                .HasKey(ua => new { ua.UserId, ua.AchievementId });

            // Avoid Multiple Cascade Paths in SQL Server
            modelBuilder.Entity<PathStep>()
                .HasOne(ps => ps.Topic)
                .WithMany(t => t.PathSteps)
                .HasForeignKey(ps => ps.TopicId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ChatSession>()
                .HasOne(cs => cs.Topic)
                .WithMany(t => t.ChatSessions)
                .HasForeignKey(cs => cs.TopicId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserSavedWord>()
                .HasOne(sw => sw.SourceMessage)
                .WithMany()
                .HasForeignKey(sw => sw.SourceMessageId)
                .OnDelete(DeleteBehavior.NoAction);

            // Seed Initial Data
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Quản trị viên hệ thống" },
                new Role { Id = 2, Name = "Learner", Description = "Học viên luyện nói" }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Daily Life", NameVi = "Giao tiếp Hàng ngày", IconName = "coffee", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Work & Career", NameVi = "Tiếng Anh Công sở", IconName = "briefcase", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Travel & Culture", NameVi = "Du lịch & Văn hóa", IconName = "plane", DisplayOrder = 3 },
                new Category { Id = 4, Name = "Free Talk", NameVi = "Trò chuyện Tự do", IconName = "chat", DisplayOrder = 4 }
            );

            modelBuilder.Entity<Topic>().HasData(
                new Topic
                {
                    Id = 1,
                    CategoryId = 1,
                    Title = "Ordering Coffee at Starbucks",
                    TitleVi = "Gọi cà phê tại Starbucks",
                    Difficulty = "Basic",
                    AIRole = "Barista at Starbucks",
                    UserRole = "Customer",
                    SystemPrompt = "You are a friendly and polite barista at a Starbucks coffee shop. Greet the customer and help them place their order. Keep responses natural, conversational, and under 3 sentences.",
                    InitialGreeting = "Hi there! Welcome to Starbucks. What can I get started for you today?",
                    IconName = "coffee",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Topic
                {
                    Id = 2,
                    CategoryId = 2,
                    Title = "Job Interview for Software Developer",
                    TitleVi = "Phỏng vấn xin việc Lập trình viên",
                    Difficulty = "Intermediate",
                    AIRole = "Tech Lead Hiring Manager",
                    UserRole = "Job Candidate",
                    SystemPrompt = "You are a Tech Lead hiring manager interviewing a software developer candidate. Ask professional questions about their experience, problem solving, and software projects.",
                    InitialGreeting = "Hello! Thanks for coming in today. Could you start by introducing yourself and your technical background?",
                    IconName = "briefcase",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Topic
                {
                    Id = 3,
                    CategoryId = 4,
                    Title = "Free Conversation & Friendly Chat",
                    TitleVi = "Trò chuyện tự do",
                    Difficulty = "All Levels",
                    AIRole = "Native English Friend",
                    UserRole = "Friend",
                    SystemPrompt = "You are a friendly, encouraging native English speaker having a casual conversation with your friend who is practicing English. Discuss hobbies, daily life, weather, or movies.",
                    InitialGreeting = "Hey there! Great to chat with you today. How has your day been so far?",
                    IconName = "chat",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<Achievement>().HasData(
                new Achievement { Id = 1, Code = "FIRST_TALK", Title = "Lời Chào Đầu Tiên", Description = "Hoàn thành buổi luyện nói đầu tiên với AI", XpReward = 50 },
                new Achievement { Id = 2, Code = "STREAK_7", Title = "Chiến Binh Chăm Chỉ", Description = "Đạt chuỗi 7 ngày luyện tập liên tục", XpReward = 200 },
                new Achievement { Id = 3, Code = "GRAMMAR_PRO", Title = "Bậc Thầy Ngữ Pháp", Description = "Đạt 90+ điểm ngữ pháp trong 3 buổi học", XpReward = 150 },
                new Achievement { Id = 4, Code = "WORD_COLLECTOR", Title = "Nhà Sưu Tầm Từ Vựng", Description = "Lưu 20 từ vựng vào Sổ tay cá nhân", XpReward = 100 }
            );
        }
    }
}
