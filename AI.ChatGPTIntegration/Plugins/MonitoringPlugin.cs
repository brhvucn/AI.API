using AI.ChatGPTIntegration.Extensions;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace AI.ChatGPTIntegration.Plugins
{
    /// <summary>
    /// This represents a plugin for monitoring a web service
    /// </summary>
    public class MonitoringPlugin
    {
        [KernelFunction("get_error_summary")]
        [Description("Retrieves errors from the web services a certain amount of hours back")]
        public async Task<ErrorSummary> Execute([Description("Get a list of errors in the system for the last x hours")] int sinceHours)
        {
            // In a real implementation, you would call the Jira API to create the ticket here.
            // Dummy data for test
            var errors = new List<ErrorItem>
            {
                new("SqlTimeoutException", 7, "Critical"),
                new("NullReferenceException", 2, "Warning")
            };
            ColoredConsole.WriteLine("---Monitoring Plugin---", ConsoleColor.Green);
            return await Task.FromResult(new ErrorSummary(9, errors));
        }
    }    

    [Description("Opsummering af fejl i systemet")]
    public class ErrorSummary
    {
        public ErrorSummary(int total, List<ErrorItem> items)
        {
            Errors = items;
            TotalCount = total;
        }

        [Description("Liste over fejltyper og deres antal")]
        public List<ErrorItem> Errors { get; set; } = new();

        [Description("Total antal fejl i systemet")]
        public int TotalCount { get; set; }
    }

    [Description("A single error in the system with type, count and severity")]
    public class ErrorItem
    {
        public ErrorItem(string type, int count, string severity)
        {
            ExceptionType = type;
            Count = count;
            Severity = severity;
        }

        [Description("This is the type of exception, e.g. NullReferenceException")]
        public string ExceptionType { get; set; }
        [Description("This is the number of occurrences of this exception type")]
        public int Count { get; set; }
        [Description("This is the severity of the exception, e.g. Critical, High, Medium, Low")]
        public string Severity { get; set; }
    }
}
