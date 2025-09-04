using FluentValidation;

namespace AI.ChatGPTIntegration.Models.Requests
{
    /// <summary>
    /// This represents a request to save an FAQ answer.
    /// This will be supplied as input in the controlr endpoint that saves FAQ answers.
    /// </summary>
    public class FaqAnswerRequest
    {
        public string Answer { get; set; } = string.Empty;

        public class FaqAnswerRequestValidator : FluentValidation.AbstractValidator<FaqAnswerRequest>
        {
            public FaqAnswerRequestValidator()
            {
                RuleFor(x => x.Answer).NotEmpty().WithMessage("Answer is required.");
            }
        }
    }
}
