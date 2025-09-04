using AI.ChatGPTIntegration.Models.Domain;
using Npgsql;
using Pgvector;

namespace AI.ChatGPTIntegration.DAL
{
    /// <summary>
    /// Simple repository class for managing FAQ entries with vector embeddings in a PostgreSQL database.
    /// Please note that this implementation does not include advanced error handling or connection pooling for brevity.
    /// This type of repository is typically used with interfaces and dependency injection in a real-world application.
    /// </summary>
    public class FAQRepository
    {
        private readonly string connectionString;
        private readonly ILogger<FAQRepository> logger;
        public FAQRepository(IConfiguration configuration, ILogger<FAQRepository> logger)
        {
            NpgsqlConnection.GlobalTypeMapper.UseVector(); //register vector type globally, should be done once in the application
            connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("DefaultConnection is not configured.");
            this.logger = logger ?? throw new ArgumentException("Logger cannot be null");
        }

        public void Save(FAQAnswer answer)
        {
            using var conn = new NpgsqlConnection(this.connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("INSERT INTO faq (content, embedding) VALUES (@c, @e)", conn);
            
            cmd.Parameters.AddWithValue("c", answer.Answer);            
            cmd.Parameters.AddWithValue("e", new Vector(answer.Embedding));

            int rows = cmd.ExecuteNonQuery();
            this.logger.LogInformation($"{rows} row(s) inserted into faq.");
        }

        public List<FAQAnswer> GetAll()
        {
            var results = new List<FAQAnswer>();

            using var conn = new NpgsqlConnection(this.connectionString);
            conn.Open();

            //<-> er distance operatoren, alternativt er cosine similarity hvis vi selv vil beregne det. <#> det er "simpel matematik" og nemt for en computer at beregne
            using var cmd = new NpgsqlCommand(@"SELECT id, content, embedding FROM faq", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var faq = ReadAnswer(reader);
                results.Add(faq);
            }

            return results;
        }

        public List<FAQAnswer> SearchSimilar(float[] embedding, int limit = 5)
        {
            var results = new List<FAQAnswer>();

            using var conn = new NpgsqlConnection(this.connectionString);
            conn.Open();

            //<-> er distance operatoren, alternativt er cosine similarity hvis vi selv vil beregne det. <#> det er "simpel matematik" og nemt for en computer at beregne
            using var cmd = new NpgsqlCommand(@"
            SELECT id, content, embedding
            FROM faq
            ORDER BY embedding <-> @e
            LIMIT @limit;", conn);

            cmd.Parameters.AddWithValue("e", new Vector(embedding));
            cmd.Parameters.AddWithValue("limit", limit);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var faq = ReadAnswer(reader);        
                results.Add(faq);
            }

            return results;
        }

        private FAQAnswer ReadAnswer(NpgsqlDataReader reader)
        {
            int id = reader.GetInt32(0);
            string answer = reader.GetString(1);
            var v = reader.GetFieldValue<Vector>(2);
            float[] answerembedding = v.ToArray();
            var faq = new FAQAnswer(id, answer, answerembedding);
            return faq;
        }
    }
}
