using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_api.Data;
using backend_api.Models.Entities;

namespace backend_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SavedWordsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SavedWordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/savedwords/user/user123
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserWords(string userId)
        {
            var words = await _context.UserSavedWords
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            return Ok(words);
        }

        // GET: api/savedwords/review/user123 (Spaced Repetition Due Words)
        [HttpGet("review/{userId}")]
        public async Task<IActionResult> GetReviewDueWords(string userId)
        {
            var dueWords = await _context.UserSavedWords
                .Where(w => w.UserId == userId && !w.IsMastered && w.NextReviewDate <= DateTime.UtcNow)
                .OrderBy(w => w.NextReviewDate)
                .Take(10)
                .ToListAsync();

            return Ok(dueWords);
        }

        // POST: api/savedwords/save
        [HttpPost("save")]
        public async Task<IActionResult> SaveWord([FromBody] SaveWordDto dto)
        {
            var existingWord = await _context.UserSavedWords
                .FirstOrDefaultAsync(w => w.UserId == dto.UserId && w.Word.ToLower() == dto.Word.ToLower());

            if (existingWord != null)
            {
                return Ok(new { message = "Word already saved in vocabulary bank", word = existingWord });
            }

            var newWord = new UserSavedWord
            {
                UserId = dto.UserId,
                Word = dto.Word,
                MeaningVi = dto.MeaningVi,
                Phonetic = dto.Phonetic,
                ExampleSentence = dto.ExampleSentence,
                SourceMessageId = dto.SourceMessageId,
                NextReviewDate = DateTime.UtcNow.AddDays(1) // SuperMemo-2 Initial interval 1 day
            };

            _context.UserSavedWords.Add(newWord);
            await _context.SaveChangesAsync();

            return Ok(newWord);
        }

        // PUT: api/savedwords/review-result
        [HttpPut("review-result")]
        public async Task<IActionResult> UpdateReviewResult([FromBody] ReviewResultDto dto)
        {
            var word = await _context.UserSavedWords.FindAsync(dto.WordId);
            if (word == null) return NotFound();

            word.ReviewCount += 1;

            // Simple SuperMemo-2 Spaced Repetition Logic
            if (dto.IsCorrect)
            {
                int intervalDays = word.ReviewCount switch
                {
                    1 => 1,
                    2 => 3,
                    3 => 7,
                    4 => 14,
                    _ => (int)(word.ReviewCount * 6 * word.EaseFactor)
                };

                word.NextReviewDate = DateTime.UtcNow.AddDays(intervalDays);
                if (word.ReviewCount >= 5) word.IsMastered = true;
            }
            else
            {
                word.ReviewCount = 0;
                word.NextReviewDate = DateTime.UtcNow.AddHours(4); // Review again soon
            }

            await _context.SaveChangesAsync();
            return Ok(word);
        }
    }

    public class SaveWordDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Word { get; set; } = string.Empty;
        public string MeaningVi { get; set; } = string.Empty;
        public string? Phonetic { get; set; }
        public string? ExampleSentence { get; set; }
        public Guid? SourceMessageId { get; set; }
    }

    public class ReviewResultDto
    {
        public int WordId { get; set; }
        public bool IsCorrect { get; set; }
    }
}
