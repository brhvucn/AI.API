using FluentValidation;

namespace AI.ChatGPTIntegration.Models.Requests
{
    /// <summary>
    /// This represents a request to find similar answers based on a question.
    /// This is used as input in the controller endpoint that finds similar FAQ answers.    
    /// </summary>
    public class SimpleChatRequest
    {
        public string Message { get; set; } = string.Empty;

        public class SimpleChatRequestValidator : AbstractValidator<SimpleChatRequest>
        {
            public SimpleChatRequestValidator()
            {
                RuleFor(x => x.Message).NotEmpty().WithMessage("Message is required.");
            }
        }
    }
}
