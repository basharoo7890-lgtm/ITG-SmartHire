using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Employment.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<GeminiService> _logger;

        public GeminiService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeminiApiKey"] ?? "";
            _logger = logger;
        }

        public async Task<string?> GenerateAsync(string prompt)
        {
            try
            {
                var requestBody = new
                {
            model = "openai/gpt-oss-120b:free",
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var response = await _httpClient.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);
                var responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("OpenRouter API Error: {StatusCode} - {Response}", response.StatusCode, responseJson);
                    return null;
                }

                var doc = JsonDocument.Parse(responseJson);
                return doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OpenRouter request failed");
                return null;
            }
        }
    }
}