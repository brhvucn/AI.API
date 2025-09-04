using AI.ChatGPTIntegration.Services.Contracts;
using System.Text.Json;

namespace AI.ChatGPTIntegration.Services
{
    /// <summary>
    /// implements methods for making HTTP requests.
    /// </summary>
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpService> logger;
        public HttpService(ILogger<HttpService> logger, HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            this.logger = logger;
        }
       
        public Task<bool> DeleteAsync(string url, Dictionary<string, string>? headers = null)
        {
            return ExecuteRequest(url, HttpMethod.Delete, headers).ContinueWith(t => t.Result != null);
        }

        public Task<JsonDocument> GetJsonAsync(string url, Dictionary<string, string>? headers = null)
        {
            return ExecuteRequest(url, HttpMethod.Get, headers);
        }

        public Task<JsonDocument> PostAsync(string url, object data, Dictionary<string, string>? headers = null)
        {
            return ExecuteRequest(url, HttpMethod.Post, data, headers);
        }

        public async Task<JsonDocument> PutAsync(string url, object data, Dictionary<string, string>? headers = null)
        {
            return await ExecuteRequest(url, HttpMethod.Put, data, headers);
        }

        public async Task<JsonDocument> ExecuteRequest(string url, HttpMethod method, object? payload = null, Dictionary<string, string>? headers = null)
        {
            string errormsg = string.Empty;
            try
            {
                using var request = new HttpRequestMessage(method, url);
                if(payload != null) //if we have data, post it
                {
                    var jsonString = JsonSerializer.Serialize(payload);
                    request.Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
                }
                AddHeaders(request, headers);
                var response = await _httpClient.SendAsync(request);
                errormsg = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                //return the result
                var json = await response.Content.ReadAsStringAsync();
                return JsonDocument.Parse(json);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                this.logger.LogError(errormsg);
                return null;
            }
        }

        public void SetDefaultHeader(string name, string value)
        {
            if (_httpClient.DefaultRequestHeaders.Contains(name))
                _httpClient.DefaultRequestHeaders.Remove(name);
            _httpClient.DefaultRequestHeaders.Add(name, value);
        }

        private void AddHeaders(HttpRequestMessage request, Dictionary<string, string>? headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }
    }
}
