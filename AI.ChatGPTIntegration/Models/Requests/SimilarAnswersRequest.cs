using FluentValidation;

namespace AI.ChatGPTIntegration.Models.Requests
{
    /// <summary>
    /// This represents a request to find similar answers based on a question.
    /// This is used as input in the controller endpoint that finds similar FAQ answers.
    /// </summary>
    public class SimilarAnswersRequest
    {
        public string Question { get; set; } = string.Empty;
        public class SimilarAnswersRequestValidator : FluentValidation.AbstractValidator<SimilarAnswersRequest>
        {
            public SimilarAnswersRequestValidator()
            {
                RuleFor(x => x.Question).NotEmpty().WithMessage("Question is required.");
            }
        }
    }
}
