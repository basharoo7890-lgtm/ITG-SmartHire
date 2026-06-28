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
            const int maxAttempts = 3;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var requestBody = new
                    {
                        model = "gpt-4o-mini",
                        messages = new[]
                        {
                            new { role = "user", content = prompt }
                        }
                    };

                    var json = JsonSerializer.Serialize(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                    var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                    var responseJson = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning(
                            "OpenAI API attempt {Attempt}/{Max} failed: {StatusCode} - {Response}",
                            attempt, maxAttempts, response.StatusCode, responseJson);

                        if (IsTransientStatus(response.StatusCode) && attempt < maxAttempts)
                        {
                            await Task.Delay(GetBackoffDelay(attempt));
                            continue;
                        }

                        return null;
                    }

                    var doc = JsonDocument.Parse(responseJson);

                    if (!doc.RootElement.TryGetProperty("choices", out var choices) ||
                        choices.GetArrayLength() == 0)
                    {
                        _logger.LogWarning(
                            "OpenAI attempt {Attempt}/{Max} returned no choices. Raw: {Raw}",
                            attempt, maxAttempts, responseJson);

                        if (attempt < maxAttempts)
                        {
                            await Task.Delay(GetBackoffDelay(attempt));
                            continue;
                        }

                        return null;
                    }

                    var text = choices[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        _logger.LogWarning(
                            "OpenAI attempt {Attempt}/{Max} returned empty content. Retrying if possible.",
                            attempt, maxAttempts);

                        if (attempt < maxAttempts)
                        {
                            await Task.Delay(GetBackoffDelay(attempt));
                            continue;
                        }

                        return null;
                    }

                    return text;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OpenAI request attempt {Attempt}/{Max} threw an exception", attempt, maxAttempts);

                    if (attempt < maxAttempts)
                    {
                        await Task.Delay(GetBackoffDelay(attempt));
                        continue;
                    }

                    return null;
                }
            }

            return null;
        }

        private static bool IsTransientStatus(System.Net.HttpStatusCode statusCode)
        {
            return statusCode == System.Net.HttpStatusCode.TooManyRequests
                || (int)statusCode >= 500;
        }

        private static TimeSpan GetBackoffDelay(int attempt)
        {
            return TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
        }
    }
}
