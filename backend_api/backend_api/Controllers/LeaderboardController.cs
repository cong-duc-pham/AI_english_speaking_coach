using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_api.Data;

namespace backend_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeaderboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/leaderboard/top-xp
        [HttpGet("top-xp")]
        public async Task<IActionResult> GetTopXP([FromQuery] int limit = 10)
        {
            var topUsers = await _context.Users
                .OrderByDescending(u => u.ExperienceXP)
                .Take(limit)
                .Select(u => new
                {
                    u.Id,
                    u.DisplayName,
                    u.PhotoUrl,
                    u.Level,
                    u.ExperienceXP,
                    u.StreakDays,
                    u.TotalPracticeMinutes
                })
                .ToListAsync();

            return Ok(topUsers);
        }

        // GET: api/leaderboard/top-streak
        [HttpGet("top-streak")]
        public async Task<IActionResult> GetTopStreak([FromQuery] int limit = 10)
        {
            var topStreakUsers = await _context.Users
                .OrderByDescending(u => u.StreakDays)
                .ThenByDescending(u => u.ExperienceXP)
                .Take(limit)
                .Select(u => new
                {
                    u.Id,
                    u.DisplayName,
                    u.PhotoUrl,
                    u.Level,
                    u.StreakDays,
                    u.ExperienceXP
                })
                .ToListAsync();

            return Ok(topStreakUsers);
        }
    }
}
