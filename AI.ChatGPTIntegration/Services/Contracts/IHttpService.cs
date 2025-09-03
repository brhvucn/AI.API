using System.Text.Json;

namespace AI.ChatGPTIntegration.Services.Contracts
{
    /// <summary>
    /// Helpers for making HTTP requests.
    /// </summary>
    public interface IHttpService
    {
        Task<JsonDocument> GetJsonAsync(string url, Dictionary<string, string>? headers = null);
        Task<JsonDocument> PostAsync(string url, object data, Dictionary<string, string>? headers = null);
        Task<JsonDocument> PutAsync(string url, object data, Dictionary<string, string>? headers = null);
        Task<bool> DeleteAsync(string url, Dictionary<string, string>? headers = null);
    }
}
