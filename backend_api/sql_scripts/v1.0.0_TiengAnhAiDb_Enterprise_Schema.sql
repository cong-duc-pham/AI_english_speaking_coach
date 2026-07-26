/*******************************************************************************
 * PROJECT: AI English Speaking Coach (Multi-Client Edition)
 * MODULE: Database Schema & Seed Data Initialization
 * SCRIPT: v1.0.0_TiengAnhAiDb_Enterprise_Schema.sql
 * VERSION: v1.0.0
 * CREATED DATE: 2026-07-26
 * TARGET DBMS: Microsoft SQL Server 2019 / 2022 / Azure SQL / LocalDB
 * TOTAL TABLES: 15 Core Tables + 1 EF Migration History
 * AUTHOR: Cong Duc Pham
 *******************************************************************************/

USE [master];
GO

-- 1. Tạo Database nếu chưa tồn tại
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'TiengAnhAiDb')
BEGIN
    CREATE DATABASE [TiengAnhAiDb];
    PRINT N'✅ Đã tạo mới Database [TiengAnhAiDb]';
END;
GO

USE [TiengAnhAiDb];
GO

-- ============================================================================
-- PHẦN 1: XÓA BẢNG CŨ (NẾU CÓ) THEO ĐÚNG THỨ TỰ RÀNG BUỘC KHÓA NGOẠI
-- ============================================================================
IF OBJECT_ID(N'[dbo].[QuizHistory]', N'U') IS NOT NULL DROP TABLE [dbo].[QuizHistory];
IF OBJECT_ID(N'[dbo].[UserAchievements]', N'U') IS NOT NULL DROP TABLE [dbo].[UserAchievements];
IF OBJECT_ID(N'[dbo].[Achievements]', N'U') IS NOT NULL DROP TABLE [dbo].[Achievements];
IF OBJECT_ID(N'[dbo].[UserSavedWords]', N'U') IS NOT NULL DROP TABLE [dbo].[UserSavedWords];
IF OBJECT_ID(N'[dbo].[SessionAssessments]', N'U') IS NOT NULL DROP TABLE [dbo].[SessionAssessments];
IF OBJECT_ID(N'[dbo].[ChatMessages]', N'U') IS NOT NULL DROP TABLE [dbo].[ChatMessages];
IF OBJECT_ID(N'[dbo].[ChatSessions]', N'U') IS NOT NULL DROP TABLE [dbo].[ChatSessions];
IF OBJECT_ID(N'[dbo].[UserPathProgress]', N'U') IS NOT NULL DROP TABLE [dbo].[UserPathProgress];
IF OBJECT_ID(N'[dbo].[PathSteps]', N'U') IS NOT NULL DROP TABLE [dbo].[PathSteps];
IF OBJECT_ID(N'[dbo].[LearningPaths]', N'U') IS NOT NULL DROP TABLE [dbo].[LearningPaths];
IF OBJECT_ID(N'[dbo].[Topics]', N'U') IS NOT NULL DROP TABLE [dbo].[Topics];
IF OBJECT_ID(N'[dbo].[Categories]', N'U') IS NOT NULL DROP TABLE [dbo].[Categories];
IF OBJECT_ID(N'[dbo].[UserRoles]', N'U') IS NOT NULL DROP TABLE [dbo].[UserRoles];
IF OBJECT_ID(N'[dbo].[Roles]', N'U') IS NOT NULL DROP TABLE [dbo].[Roles];
IF OBJECT_ID(N'[dbo].[Users]', N'U') IS NOT NULL DROP TABLE [dbo].[Users];
GO

-- ============================================================================
-- PHẦN 2: PHÂN HỆ NGƯỜI DÙNG VÀ PHÂN QUYỀN (AUTH & RBAC)
-- ============================================================================

-- 1. Bảng Users (Thông tin người học & Gamification Stats)
CREATE TABLE [dbo].[Users] (
    [Id] NVARCHAR(128) NOT NULL,
    [Email] NVARCHAR(256) NOT NULL,
    [DisplayName] NVARCHAR(100) NOT NULL,
    [PhotoUrl] NVARCHAR(500) NULL,
    [Level] NVARCHAR(20) NOT NULL DEFAULT N'Basic',              -- Basic, Intermediate, Advanced
    [PreferredAccent] NVARCHAR(10) NOT NULL DEFAULT N'en-US',   -- en-US, en-GB
    [TotalPracticeMinutes] INT NOT NULL DEFAULT 0,
    [ExperienceXP] INT NOT NULL DEFAULT 0,
    [StreakDays] INT NOT NULL DEFAULT 0,
    [LastActiveDate] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- 2. Bảng Roles (Danh sách vai trò)
CREATE TABLE [dbo].[Roles] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,                               -- Admin, Learner, Premium
    [Description] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- 3. Bảng UserRoles (Liên kết Người dùng - Vai trò)
CREATE TABLE [dbo].[UserRoles] (
    [UserId] NVARCHAR(128) NOT NULL,
    [RoleId] INT NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE
);
GO

-- ============================================================================
-- PHẦN 3: PHÂN HỆ KỊCH BẢN VÀ LỘ TRÌNH HỌC TẬP (LEARNING PATHWAYS)
-- ============================================================================

-- 4. Bảng Categories (Danh mục kịch bản)
CREATE TABLE [dbo].[Categories] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [NameVi] NVARCHAR(100) NOT NULL,
    [IconName] NVARCHAR(50) NOT NULL DEFAULT N'folder',
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- 5. Bảng Topics (Chi tiết kịch bản giao tiếp AI)
CREATE TABLE [dbo].[Topics] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [CategoryId] INT NOT NULL,
    [Title] NVARCHAR(150) NOT NULL,
    [TitleVi] NVARCHAR(150) NOT NULL,
    [Difficulty] NVARCHAR(20) NOT NULL DEFAULT N'Basic',          -- Basic, Intermediate, Advanced
    [AIRole] NVARCHAR(100) NOT NULL DEFAULT N'Native Speaker',
    [UserRole] NVARCHAR(100) NOT NULL DEFAULT N'Learner',
    [SystemPrompt] NVARCHAR(MAX) NOT NULL,
    [InitialGreeting] NVARCHAR(MAX) NOT NULL,
    [IconName] NVARCHAR(50) NOT NULL DEFAULT N'chat',
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Topics] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Topics_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id]) ON DELETE CASCADE
);

-- 6. Bảng LearningPaths (Lộ trình bài học theo tuyến)
CREATE TABLE [dbo].[LearningPaths] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(150) NOT NULL,
    [TitleVi] NVARCHAR(150) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [TargetLevel] NVARCHAR(20) NOT NULL DEFAULT N'Basic',
    [BannerUrl] NVARCHAR(500) NULL,
    [IsPublished] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_LearningPaths] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- 7. Bảng PathSteps (Các bước trong từng Lộ trình)
CREATE TABLE [dbo].[PathSteps] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [PathId] INT NOT NULL,
    [TopicId] INT NOT NULL,
    [StepOrder] INT NOT NULL,                                    -- Thứ tự 1, 2, 3 trong lộ trình
    [MinScoreToPass] INT NOT NULL DEFAULT 60,                    -- Điểm tối thiểu để mở khóa bước sau
    CONSTRAINT [PK_PathSteps] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PathSteps_LearningPaths] FOREIGN KEY ([PathId]) REFERENCES [dbo].[LearningPaths] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PathSteps_Topics] FOREIGN KEY ([TopicId]) REFERENCES [dbo].[Topics] ([Id]) ON DELETE NO ACTION
);

-- 8. Bảng UserPathProgress (Tiến độ của Người dùng trong Lộ trình)
CREATE TABLE [dbo].[UserPathProgress] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] NVARCHAR(128) NOT NULL,
    [StepId] INT NOT NULL,
    [IsCompleted] BIT NOT NULL DEFAULT 0,
    [BestScore] INT NOT NULL DEFAULT 0,
    [CompletedAt] DATETIME2 NULL,
    CONSTRAINT [PK_UserPathProgress] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserPathProgress_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserPathProgress_PathSteps] FOREIGN KEY ([StepId]) REFERENCES [dbo].[PathSteps] ([Id]) ON DELETE CASCADE
);
GO

-- ============================================================================
-- PHẦN 4: PHÂN HỆ LUYỆN NÓI VÀ ĐÁNH GIÁ CEFR (VOICE CHAT & ANALYTICS)
-- ============================================================================

-- 9. Bảng ChatSessions (Buổi luyện nói)
CREATE TABLE [dbo].[ChatSessions] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [UserId] NVARCHAR(128) NOT NULL,
    [TopicId] INT NOT NULL,
    [StartTime] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [EndTime] DATETIME2 NULL,
    [DurationSeconds] INT NOT NULL DEFAULT 0,
    [MessageCount] INT NOT NULL DEFAULT 0,
    [ClientPlatform] NVARCHAR(20) NOT NULL DEFAULT N'Web',       -- Web, MobileApp
    CONSTRAINT [PK_ChatSessions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ChatSessions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ChatSessions_Topics] FOREIGN KEY ([TopicId]) REFERENCES [dbo].[Topics] ([Id]) ON DELETE NO ACTION
);

-- 10. Bảng ChatMessages (Tin nhắn & Nhận xét Sửa lỗi chi tiết)
CREATE TABLE [dbo].[ChatMessages] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [SessionId] UNIQUEIDENTIFIER NOT NULL,
    [Sender] NVARCHAR(10) NOT NULL,                              -- 'user' hoặc 'ai'
    [Content] NVARCHAR(MAX) NOT NULL,
    [CorrectedText] NVARCHAR(MAX) NULL,
    [BetterWayToSay] NVARCHAR(MAX) NULL,
    [GrammarExplanation] NVARCHAR(MAX) NULL,
    [VietnameseTranslation] NVARCHAR(MAX) NULL,
    [PhoneticIPA] NVARCHAR(MAX) NULL,                            -- Phiên âm IPA
    [AudioUrl] NVARCHAR(500) NULL,
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_ChatMessages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ChatMessages_ChatSessions] FOREIGN KEY ([SessionId]) REFERENCES [dbo].[ChatSessions] ([Id]) ON DELETE CASCADE
);

-- 11. Bảng SessionAssessments (Đánh giá Báo cáo Đa tiêu chí & CEFR)
CREATE TABLE [dbo].[SessionAssessments] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [SessionId] UNIQUEIDENTIFIER NOT NULL,
    [GrammarScore] INT NOT NULL DEFAULT 0,                       -- Thang điểm 0 - 100
    [VocabularyScore] INT NOT NULL DEFAULT 0,
    [FluencyScore] INT NOT NULL DEFAULT 0,
    [PronunciationScore] INT NOT NULL DEFAULT 0,
    [OverallScore] INT NOT NULL DEFAULT 0,
    [EstimatedCEFR] NVARCHAR(10) NOT NULL DEFAULT N'B1',         -- A1, A2, B1, B2, C1
    [DetailedFeedback] NVARCHAR(MAX) NULL,
    [KeyImprovementPoints] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_SessionAssessments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SessionAssessments_ChatSessions] FOREIGN KEY ([SessionId]) REFERENCES [dbo].[ChatSessions] ([Id]) ON DELETE CASCADE
);
GO

-- ============================================================================
-- PHẦN 5: PHÂN HỆ TỪ VỰNG SPACED REPETITION, GAME HÓA VÀ QUIZ
-- ============================================================================

-- 12. Bảng UserSavedWords (Sổ tay Từ vựng & Thuật toán Spaced Repetition)
CREATE TABLE [dbo].[UserSavedWords] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] NVARCHAR(128) NOT NULL,
    [Word] NVARCHAR(100) NOT NULL,
    [MeaningVi] NVARCHAR(255) NOT NULL,
    [Phonetic] NVARCHAR(100) NULL,
    [ExampleSentence] NVARCHAR(MAX) NULL,
    [SourceMessageId] UNIQUEIDENTIFIER NULL,                     -- Từ này rút ra từ tin nhắn nào
    [ReviewCount] INT NOT NULL DEFAULT 0,
    [EaseFactor] FLOAT NOT NULL DEFAULT 2.5,                     -- Hệ số lặp lại ngắt quãng (SuperMemo-2)
    [NextReviewDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),    -- Ngày cần ôn tập lại
    [IsMastered] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_UserSavedWords] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserSavedWords_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserSavedWords_ChatMessages] FOREIGN KEY ([SourceMessageId]) REFERENCES [dbo].[ChatMessages] ([Id]) ON DELETE NO ACTION
);

-- 13. Bảng Achievements (Danh mục Huy hiệu Thưởng)
CREATE TABLE [dbo].[Achievements] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Code] NVARCHAR(50) NOT NULL,                                -- STREAK_7, FIRST_TALK, GRAMMAR_MASTER
    [Title] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(255) NOT NULL,
    [IconUrl] NVARCHAR(500) NULL,
    [XpReward] INT NOT NULL DEFAULT 50,
    CONSTRAINT [PK_Achievements] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- 14. Bảng UserAchievements (Huy hiệu Người dùng Đã Mở khóa)
CREATE TABLE [dbo].[UserAchievements] (
    [UserId] NVARCHAR(128) NOT NULL,
    [AchievementId] INT NOT NULL,
    [UnlockedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_UserAchievements] PRIMARY KEY CLUSTERED ([UserId] ASC, [AchievementId] ASC),
    CONSTRAINT [FK_UserAchievements_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserAchievements_Achievements] FOREIGN KEY ([AchievementId]) REFERENCES [dbo].[Achievements] ([Id]) ON DELETE CASCADE
);

-- 15. Bảng QuizHistory (Lưu lịch sử chơi Mini-Game Ôn từ vựng qua Âm thanh)
CREATE TABLE [dbo].[QuizHistory] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] NVARCHAR(128) NOT NULL,
    [TotalQuestions] INT NOT NULL DEFAULT 5,
    [CorrectAnswers] INT NOT NULL DEFAULT 0,
    [XpEarned] INT NOT NULL DEFAULT 0,
    [CompletedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_QuizHistory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_QuizHistory_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);
GO

-- ============================================================================
-- PHẦN 6: DỮ LIỆU MẪU BAN ĐẦU (SEED DATA)
-- ============================================================================

-- Roles Seed
INSERT INTO [dbo].[Roles] ([Name], [Description]) VALUES 
(N'Admin', N'Quản trị viên hệ thống'),
(N'Learner', N'Học viên luyện nói');

-- Categories Seed
INSERT INTO [dbo].[Categories] ([Name], [NameVi], [IconName], [DisplayOrder]) VALUES
(N'Daily Life', N'Giao tiếp Hàng ngày', N'coffee', 1),
(N'Work & Career', N'Tiếng Anh Công sở', N'briefcase', 2),
(N'Travel & Culture', N'Du lịch & Văn hóa', N'plane', 3),
(N'Free Talk', N'Trò chuyện Tự do', N'chat', 4);

-- Topics Seed
INSERT INTO [dbo].[Topics] ([CategoryId], [Title], [TitleVi], [Difficulty], [AIRole], [UserRole], [SystemPrompt], [InitialGreeting], [IconName], [IsActive]) VALUES
(1, N'Ordering Coffee at Starbucks', N'Gọi cà phê tại Starbucks', N'Basic', N'Barista at Starbucks', N'Customer', 
 N'You are a friendly and polite barista at a Starbucks coffee shop. Greet the customer and help them place their order. Keep responses natural, conversational, and under 3 sentences.', 
 N'Hi there! Welcome to Starbucks. What can I get started for you today?', N'coffee', 1),

(2, N'Job Interview for Software Developer', N'Phỏng vấn xin việc Lập trình viên', N'Intermediate', N'Tech Lead Hiring Manager', N'Job Candidate', 
 N'You are a Tech Lead hiring manager interviewing a software developer candidate. Ask professional questions about their experience, problem solving, and software projects.', 
 N'Hello! Thanks for coming in today. Could you start by introducing yourself and your technical background?', N'briefcase', 1),

(4, N'Free Conversation & Friendly Chat', N'Trò chuyện tự do', N'All Levels', N'Native English Friend', N'Friend', 
 N'You are a friendly, encouraging native English speaker having a casual conversation with your friend who is practicing English. Discuss hobbies, daily life, weather, or movies.', 
 N'Hey there! Great to chat with you today. How has your day been so far?', N'chat', 1);

-- Achievements Seed
INSERT INTO [dbo].[Achievements] ([Code], [Title], [Description], [XpReward]) VALUES
(N'FIRST_TALK', N'Lời Chào Đầu Tiên', N'Hoàn thành buổi luyện nói đầu tiên với AI', 50),
(N'STREAK_7', N'Chiến Binh Chăm Chỉ', N'Đạt chuỗi 7 ngày luyện tập liên tục', 200),
(N'GRAMMAR_PRO', N'Bậc Thầy Ngữ Pháp', N'Đạt 90+ điểm ngữ pháp trong 3 buổi học', 150),
(N'WORD_COLLECTOR', N'Nhà Sưu Tầm Từ Vựng', N'Lưu 20 từ vựng vào Sổ tay cá nhân', 100);
GO

-- Synchronize EF Migrations History
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory] (
        [MigrationId] NVARCHAR(150) NOT NULL,
        [ProductVersion] NVARCHAR(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC)
    );
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20260726132830_InitialCreate')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260726132830_InitialCreate', N'8.0.12');
END;
GO

PRINT N'========================================================================';
PRINT N'✅ THÀNH CÔNG: Đã khởi tạo hoàn chỉnh CSDL v1.0.0 (15 Bảng) cho TiengAnhAiDb!';
PRINT N'========================================================================';
