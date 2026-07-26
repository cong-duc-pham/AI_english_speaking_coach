namespace backend_api.Models.DTOs
{
    public class SpeechRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public Guid SessionId { get; set; }
        public int TopicId { get; set; }
        public string UserMessage { get; set; } = string.Empty;
    }

    public class GeminiResponseDto
    {
        public string AiResponse { get; set; } = string.Empty;
        public string AiResponseVi { get; set; } = string.Empty;
        public string PhoneticIPA { get; set; } = string.Empty;
        public UserCorrectionDto? UserCorrection { get; set; }
        public List<string> SuggestedReplies { get; set; } = new();
    }

    public class UserCorrectionDto
    {
        public bool HasError { get; set; } = false;
        public string OriginalText { get; set; } = string.Empty;
        public string CorrectedText { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public string BetterWayToSay { get; set; } = string.Empty;
    }
}
