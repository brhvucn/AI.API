
using AI.ChatGPTIntegration.Models.Config;
using AI.ChatGPTIntegration.Plugins;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Runtime.CompilerServices;

namespace AI.ChatGPTIntegration.Agents
{
    public class SimpleAgent : BaseAgent<string>
    {
        private readonly ILogger<SimpleAgent> logger;        
        public SimpleAgent(IOptions<OpenAISettings> openAISettings, ILogger<SimpleAgent> logger) : base(openAISettings)
        {
            this.openAISettings = openAISettings?.Value ?? throw new ArgumentNullException(nameof(openAISettings));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //add the plugins, these are the available functions the agent can call
            this.Kernel.Plugins.AddFromType<JiraPlugin>();
            this.Kernel.Plugins.AddFromType<MonitoringPlugin>();
            this.Kernel.Plugins.AddFromType<SlackPlugin>();
        }

        // Example of overriding the abstract methods from BaseAgent
        public override string GetModelName()
        {
            return this.openAISettings.AgentModel;
        }

        // Example of overriding the abstract methods from BaseAgent
        public override string GetOpenAIApiKey()
        {
            return this.openAISettings.OpenAIKey;
        }

        public override async Task<string> RunAsync()
        {
            string prompt = @"
                Du er en SaaS-overvågningsagent.
                1. Brug get_error_summary til at hente fejl fra de sidste 24 timer.
                2. Hvis du finder kritiske fejl, brug create_jira_ticket for at oprette en sag.
                3. Afslut med en statusrapport til administratoren. Send denne med slack_send_message.
                ";

            // Lav execution settings
            var settings = new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions // Automatisk kald af kernel funktioner, hvis ikke skriver den kun hvad den vil gøre
            };

            // Lav kernel arguments med settings
            var args = new KernelArguments(settings);

            // Kør prompten
            var result = await this.Kernel.InvokePromptAsync(prompt, args);
            return result.ToString();
        }
    }
}
