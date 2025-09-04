using AI.ChatGPTIntegration.Extensions;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace AI.ChatGPTIntegration.Plugins
{
    /// <summary>
    /// This represents a plugin for integrating with Jira to create new Issues
    /// </summary>
    public class SlackPlugin
    {
        [KernelFunction("slack_send_message")]
        [Description("Will notify a user with the message contained in the input")]
        public string Execute(SlackSendMessageInput input)
        {
            // In a real implementation, you would send a webhook to slack here.            
            string detailedMessageInfo = $"Message '{input.Title}' with description: {input.Description} was sent";
            ColoredConsole.WriteLine("---Slack Plugin---", ConsoleColor.Green);
            Console.WriteLine(detailedMessageInfo);
            return detailedMessageInfo;
        }
    }

    /// <summary>
    /// Represents the input required to create a Jira ticket.
    /// </summary>
    public class SlackSendMessageInput
    {
        [Description("This is the detailed description for the slack message")]
        public string Description { get; set; } = string.Empty;
        [Description("This is the title/heading of the slack message")]
        public string Title { get; set; } = string.Empty;
    }
}
