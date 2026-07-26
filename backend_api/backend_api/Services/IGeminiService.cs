using backend_api.Models.DTOs;

namespace backend_api.Services
{
    public interface IGeminiService
    {
        Task<GeminiResponseDto> ProcessSpeechConversationAsync(
            string systemPrompt, 
            string userRole, 
            string aiRole, 
            string userMessage, 
            List<(string Sender, string Content)> recentHistory);
    }
}
