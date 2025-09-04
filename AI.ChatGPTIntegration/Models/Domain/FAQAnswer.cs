namespace AI.ChatGPTIntegration.Models.Domain
{
    /// <summary>
    /// This is a domain model entity representing an FAQ answer along with its vector embedding.
    /// </summary>
    public class FAQAnswer
    {
        public FAQAnswer(string answer, float[] embedding) 
        {
            Answer = answer;
            Embedding = embedding;
        }

        public FAQAnswer(int id, string answer, float[] embedding)
        {
            Id = id;
            Answer = answer;
            Embedding = embedding;
        }
        public int Id { get; set; }
        public string Answer { get; private set; } = string.Empty;
        public float[] Embedding { get; private set; } = default!;
    }
}
