using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_api.Data;

namespace backend_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PathwaysController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PathwaysController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/pathways
        [HttpGet]
        public async Task<IActionResult> GetPathways()
        {
            var pathways = await _context.LearningPaths
                .Where(p => p.IsPublished)
                .Include(p => p.Steps)
                    .ThenInclude(s => s.Topic)
                .ToListAsync();

            return Ok(pathways);
        }

        // GET: api/pathways/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPathwayDetails(int id, [FromQuery] string? userId)
        {
            var pathway = await _context.LearningPaths
                .Include(p => p.Steps.OrderBy(s => s.StepOrder))
                    .ThenInclude(s => s.Topic)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pathway == null) return NotFound();

            List<int> completedStepIds = new();
            if (!string.IsNullOrEmpty(userId))
            {
                completedStepIds = await _context.UserPathProgresses
                    .Where(up => up.UserId == userId && up.IsCompleted)
                    .Select(up => up.StepId)
                    .ToListAsync();
            }

            var stepsResult = pathway.Steps.Select(s => new
            {
                s.Id,
                s.StepOrder,
                s.MinScoreToPass,
                topic = s.Topic,
                isCompleted = completedStepIds.Contains(s.Id)
            });

            return Ok(new
            {
                pathway.Id,
                pathway.Title,
                pathway.TitleVi,
                pathway.Description,
                pathway.TargetLevel,
                pathway.BannerUrl,
                steps = stepsResult
            });
        }
    }
}
