using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_api.Data;
using backend_api.Models.Entities;

namespace backend_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuizController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/quiz/questions/user123
        [HttpGet("questions/{userId}")]
        public async Task<IActionResult> GetQuizQuestions(string userId)
        {
            var savedWords = await _context.UserSavedWords
                .Where(w => w.UserId == userId)
                .ToListAsync();

            if (savedWords.Count < 4)
            {
                // Fallback default sample questions if user has < 4 saved words
                return Ok(GetSampleQuizQuestions());
            }

            var random = new Random();
            var selectedWords = savedWords.OrderBy(_ => random.Next()).Take(5).ToList();

            var questions = selectedWords.Select(word =>
            {
                // Get 3 incorrect options
                var wrongOptions = savedWords
                    .Where(w => w.Id != word.Id)
                    .OrderBy(_ => random.Next())
                    .Take(3)
                    .Select(w => w.MeaningVi)
                    .ToList();

                var options = new List<string>(wrongOptions) { word.MeaningVi };
                options = options.OrderBy(_ => random.Next()).ToList();

                return new
                {
                    wordId = word.Id,
                    word = word.Word,
                    phonetic = word.Phonetic,
                    correctMeaning = word.MeaningVi,
                    options = options
                };
            }).ToList();

            return Ok(questions);
        }

        // POST: api/quiz/submit
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] SubmitQuizDto dto)
        {
            int xpEarned = dto.CorrectAnswers * 10;

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user != null)
            {
                user.ExperienceXP += xpEarned;
                user.LastActiveDate = DateTime.UtcNow;
            }

            var quizHistory = new QuizHistory
            {
                UserId = dto.UserId,
                TotalQuestions = dto.TotalQuestions,
                CorrectAnswers = dto.CorrectAnswers,
                XpEarned = xpEarned,
                CompletedAt = DateTime.UtcNow
            };

            _context.QuizHistories.Add(quizHistory);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                totalQuestions = dto.TotalQuestions,
                correctAnswers = dto.CorrectAnswers,
                xpEarned = xpEarned,
                totalXP = user?.ExperienceXP ?? 0
            });
        }

        private static List<object> GetSampleQuizQuestions()
        {
            return new List<object>
            {
                new { wordId = 1, word = "Barista", phonetic = "/bəˈriːstə/", correctMeaning = "Nhân viên pha chế cà phê", options = new[] { "Nhân viên pha chế cà phê", "Khách hàng", "Người quản lý", "Đầu bếp" } },
                new { wordId = 2, word = "Candidate", phonetic = "/ˈkændɪdət/", correctMeaning = "Ứng viên xin việc", options = new[] { "Ứng viên xin việc", "Nhà tuyển dụng", "Lập trình viên", "Giám đốc" } },
                new { wordId = 3, word = "Fluency", phonetic = "/ˈfluːənsi/", correctMeaning = "Độ trôi chảy", options = new[] { "Độ trôi chảy", "Ngữ pháp", "Từ vựng", "Phát âm" } },
                new { wordId = 4, word = "Recommendation", phonetic = "/ˌrekəmenˈdeɪʃn/", correctMeaning = "Lời gợi ý / Khuyên dùng", options = new[] { "Lời gợi ý / Khuyên dùng", "Câu hỏi", "Lỗi sai", "Bản dịch" } }
            };
        }
    }

    public class SubmitQuizDto
    {
        public string UserId { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
    }
}
