namespace AI.ChatGPTIntegration.Models.Config
{
    public class OpenAISettings
    {
        public string OpenAIKey { get; set; } = string.Empty;
        public string OpenAIChatUrl { get; set; } = string.Empty;
        public string OpenAIEmbeddingUrl { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
    }
}
