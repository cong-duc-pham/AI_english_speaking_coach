using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_api.Data;
using backend_api.Models.Entities;

namespace backend_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/users/sync
        [HttpPost("sync")]
        public async Task<IActionResult> SyncUser([FromBody] SyncUserDto dto)
        {
            var user = await _context.Users.FindAsync(dto.Id);

            if (user == null)
            {
                user = new User
                {
                    Id = dto.Id,
                    Email = dto.Email,
                    DisplayName = dto.DisplayName,
                    PhotoUrl = dto.PhotoUrl,
                    Level = dto.Level ?? "Basic",
                    PreferredAccent = dto.PreferredAccent ?? "en-US",
                    CreatedAt = DateTime.UtcNow,
                    LastActiveDate = DateTime.UtcNow
                };

                _context.Users.Add(user);
            }
            else
            {
                user.DisplayName = dto.DisplayName;
                user.PhotoUrl = dto.PhotoUrl;
                user.LastActiveDate = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(dto.Level)) user.Level = dto.Level;
                if (!string.IsNullOrEmpty(dto.PreferredAccent)) user.PreferredAccent = dto.PreferredAccent;
            }

            await _context.SaveChangesAsync();
            return Ok(user);
        }

        // GET: api/users/profile/user123
        [HttpGet("profile/{userId}")]
        public async Task<IActionResult> GetProfile(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Achievements)
                    .ThenInclude(ua => ua.Achievement)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound(new { message = "User profile not found" });

            return Ok(new
            {
                user.Id,
                user.Email,
                user.DisplayName,
                user.PhotoUrl,
                user.Level,
                user.PreferredAccent,
                user.TotalPracticeMinutes,
                user.ExperienceXP,
                user.StreakDays,
                user.CreatedAt,
                achievements = user.Achievements.Select(a => new
                {
                    a.Achievement?.Code,
                    a.Achievement?.Title,
                    a.Achievement?.Description,
                    a.Achievement?.IconUrl,
                    a.UnlockedAt
                })
            });
        }
    }

    public class SyncUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string? Level { get; set; }
        public string? PreferredAccent { get; set; }
    }
}
