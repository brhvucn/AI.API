using AI.ChatGPTIntegration.Models.Config;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace AI.ChatGPTIntegration.Agents
{
    public abstract class BaseAgent<Toutput>
    {
        protected readonly Kernel Kernel;
        protected OpenAISettings openAISettings;

        protected BaseAgent(IOptions<OpenAISettings> openAISettings)
        {
            this.openAISettings = openAISettings?.Value ?? throw new ArgumentNullException(nameof(openAISettings));
            var builder = Kernel.CreateBuilder();
            string modelName = GetModelName();
            string openAiApiKey = GetOpenAIApiKey();
            builder.Services.AddOpenAIChatCompletion(modelName, openAiApiKey);
            builder.Services.AddLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Information));
            Kernel = builder.Build();
        }

        public abstract string GetOpenAIApiKey();
        public abstract string GetModelName();

        public abstract Task<Toutput> RunAsync();
    }
}
