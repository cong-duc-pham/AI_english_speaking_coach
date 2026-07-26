using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_api.Data;
using backend_api.Models.DTOs;
using backend_api.Models.Entities;
using backend_api.Services;

namespace backend_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpeechController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IGeminiService _geminiService;

        public SpeechController(ApplicationDbContext context, IGeminiService geminiService)
        {
            _context = context;
            _geminiService = geminiService;
        }

        // POST: api/speech/start-session
        [HttpPost("start-session")]
        public async Task<IActionResult> StartSession([FromBody] StartSessionDto request)
        {
            var topic = await _context.Topics.FindAsync(request.TopicId);
            if (topic == null)
            {
                return NotFound(new { message = "Topic not found" });
            }

            var session = new ChatSession
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                TopicId = request.TopicId,
                StartTime = DateTime.UtcNow,
                ClientPlatform = request.ClientPlatform ?? "Web",
                MessageCount = 1
            };

            _context.ChatSessions.Add(session);

            // Add initial AI Greeting
            var initialMessage = new ChatMessage
            {
                Id = Guid.NewGuid(),
                SessionId = session.Id,
                Sender = "ai",
                Content = topic.InitialGreeting,
                Timestamp = DateTime.UtcNow
            };

            _context.ChatMessages.Add(initialMessage);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                sessionId = session.Id,
                topicTitle = topic.Title,
                aiRole = topic.AIRole,
                initialGreeting = topic.InitialGreeting
            });
        }

        // POST: api/speech/send
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SpeechRequestDto request)
        {
            var session = await _context.ChatSessions
                .Include(s => s.Topic)
                .Include(s => s.Messages)
                .FirstOrDefaultAsync(s => s.Id == request.SessionId);

            if (session == null || session.Topic == null)
            {
                return NotFound(new { message = "Session or topic not found" });
            }

            // Get recent history
            var history = session.Messages
                .OrderBy(m => m.Timestamp)
                .TakeLast(6)
                .Select(m => (m.Sender, m.Content))
                .ToList();

            // Call Gemini AI Proxy
            var aiResult = await _geminiService.ProcessSpeechConversationAsync(
                session.Topic.SystemPrompt,
                session.Topic.UserRole,
                session.Topic.AIRole,
                request.UserMessage,
                history
            );

            // Save User Message
            var userMsg = new ChatMessage
            {
                Id = Guid.NewGuid(),
                SessionId = session.Id,
                Sender = "user",
                Content = request.UserMessage,
                CorrectedText = aiResult.UserCorrection?.CorrectedText,
                BetterWayToSay = aiResult.UserCorrection?.BetterWayToSay,
                GrammarExplanation = aiResult.UserCorrection?.Explanation,
                Timestamp = DateTime.UtcNow
            };
            _context.ChatMessages.Add(userMsg);

            // Save AI Response Message
            var aiMsg = new ChatMessage
            {
                Id = Guid.NewGuid(),
                SessionId = session.Id,
                Sender = "ai",
                Content = aiResult.AiResponse,
                VietnameseTranslation = aiResult.AiResponseVi,
                PhoneticIPA = aiResult.PhoneticIPA,
                Timestamp = DateTime.UtcNow.AddMilliseconds(100)
            };
            _context.ChatMessages.Add(aiMsg);

            // Update Session Stats
            session.MessageCount += 2;
            session.EndTime = DateTime.UtcNow;
            session.DurationSeconds = (int)(session.EndTime.Value - session.StartTime).TotalSeconds;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                userMessage = userMsg,
                aiResponse = aiMsg,
                userCorrection = aiResult.UserCorrection,
                suggestedReplies = aiResult.SuggestedReplies
            });
        }
    }

    public class StartSessionDto
    {
        public string UserId { get; set; } = string.Empty;
        public int TopicId { get; set; }
        public string? ClientPlatform { get; set; }
    }
}
