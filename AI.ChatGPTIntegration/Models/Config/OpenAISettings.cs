namespace AI.ChatGPTIntegration.Models.Config
{
    /// <summary>
    /// Represents configuration settings for OpenAI API integration, including API keys, endpoint URLs, and model names.
    /// This is read from the "OpenAI" section of the appsettings.json file.
    /// </summary>
    public class OpenAISettings
    {
        public string OpenAIKey { get; set; } = string.Empty;
        public string OpenAIChatUrl { get; set; } = string.Empty;
        public string OpenAIEmbeddingUrl { get; set; } = string.Empty;
        public string ChatModel { get; set; } = string.Empty;
        public string EmbeddingModel { get; set; } = string.Empty;
        public string AgentModel { get; set; } = string.Empty;
    }
}
