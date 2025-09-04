using System.ComponentModel;
using AI.ChatGPTIntegration.Extensions;
using Microsoft.SemanticKernel;

namespace AI.ChatGPTIntegration.Plugins
{
    /// <summary>
    /// This represents a plugin for integrating with Jira to create new Issues
    /// </summary>
    public class JiraPlugin
    {
        [KernelFunction("create_jira_ticket")]
        [Description("Creates a new Jira ticket with the provided summary and description.")]
        public string Execute(JiraCreateTicketInput input)
        {
            // In a real implementation, you would call the Jira API to create the ticket here.
            var ticketId = $"JIRA-{Random.Shared.Next(1000, 9999)}";
            string ticketInfo = $"Ticket '{input.Summary}' created with ID: {ticketId}\nDescription: {input.Description}";
            ColoredConsole.WriteLine("---Jira Plugin---", ConsoleColor.Green);
            Console.WriteLine(ticketInfo);            
            return ticketInfo;
        }
    }

    /// <summary>
    /// Represents the input required to create a Jira ticket.
    /// </summary>
    public class JiraCreateTicketInput
    {
        [Description("This is the detailed description of the issue to be created.")]
        public string Description { get; set; } = string.Empty;
        [Description("This is the summary or title of the issue to be created.")]
        public string Summary { get; set; } = string.Empty;
    }
}
