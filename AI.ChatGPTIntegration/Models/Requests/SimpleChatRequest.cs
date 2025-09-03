using FluentValidation;

namespace AI.ChatGPTIntegration.Models.Requests
{
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
