using System.Text;
using System.Text.Json;
using backend_api.Models.DTOs;

namespace backend_api.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GeminiService> _logger;

        public GeminiService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<GeminiResponseDto> ProcessSpeechConversationAsync(
            string systemPrompt,
            string userRole,
            string aiRole,
            string userMessage,
            List<(string Sender, string Content)> recentHistory)
        {
            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("Gemini API Key is missing in appsettings.json. Using mock AI response.");
                return GetMockResponse(userMessage);
            }

            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

            var jsonInstruction = @"
You are an expert native English speaking teacher and conversation coach.
Strictly return a valid JSON object matching this schema:
{
  ""aiResponse"": ""Your English reply as the AI role (natural, conversational, 1-3 sentences)"",
  ""aiResponseVi"": ""Bản dịch tiếng Việt của câu aiResponse"",
  ""phoneticIPA"": ""Phiên âm IPA cho câu aiResponse"",
  ""userCorrection"": {
    ""hasError"": true/false,
    ""originalText"": ""Original user input"",
    ""correctedText"": ""Corrected English sentence if user made grammar/vocab mistakes"",
    ""explanation"": ""Giải thích lỗi sai bằng tiếng Việt đơn giản, ngắn gọn"",
    ""betterWayToSay"": ""A more natural native way to say what user intended""
  },
  ""suggestedReplies"": [
    ""Suggested user reply option 1 in English"",
    ""Suggested user reply option 2 in English"",
    ""Suggested user reply option 3 in English""
  ]
}
";

            var fullSystemInstruction = $"{systemPrompt}\nAI Role: {aiRole}\nUser Role: {userRole}\n{jsonInstruction}";

            var contents = new List<object>();

            // Add history
            foreach (var msg in recentHistory)
            {
                contents.Add(new
                {
                    role = msg.Sender == "user" ? "user" : "model",
                    parts = new[] { new { text = msg.Content } }
                });
            }

            // Add current message
            contents.Add(new
            {
                role = "user",
                parts = new[] { new { text = userMessage } }
            });

            var requestBody = new
            {
                system_instruction = new
                {
                    parts = new[] { new { text = fullSystemInstruction } }
                },
                contents = contents,
                generationConfig = new
                {
                    response_mime_type = "application/json",
                    temperature = 0.7,
                    max_output_tokens = 1000
                }
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await _httpClient.PostAsync(endpoint, jsonContent);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseString);

                var textResponse = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                if (!string.IsNullOrEmpty(textResponse))
                {
                    var result = JsonSerializer.Deserialize<GeminiResponseDto>(textResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (result != null) return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Gemini API");
            }

            return GetMockResponse(userMessage);
        }

        private static GeminiResponseDto GetMockResponse(string userMessage)
        {
            return new GeminiResponseDto
            {
                AiResponse = "That sounds great! Tell me more about your thoughts on this.",
                AiResponseVi = "Nghe tuyệt đấy! Hãy chia sẻ thêm suy nghĩ của bạn về điều này nhé.",
                PhoneticIPA = "/ðæt saʊndz ɡreɪt/",
                UserCorrection = new UserCorrectionDto
                {
                    HasError = false,
                    OriginalText = userMessage,
                    CorrectedText = userMessage,
                    Explanation = "Câu của bạn chuẩn ngữ pháp!",
                    BetterWayToSay = userMessage
                },
                SuggestedReplies = new List<string>
                {
                    "I think it's very interesting because...",
                    "Could you explain that in more detail?",
                    "Let's move on to the next topic."
                }
            };
        }
    }
}
