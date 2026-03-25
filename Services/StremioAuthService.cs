using GlanceStremioActivity.Models;
using GlanceStremioActivity.Services.Interfaces;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace GlanceStremioActivity.Services
{
    public class StremioAuthService(HttpClient _httpClient, ILogger<IStremioAuthService> _logger) : IStremioAuthService
    {
        private readonly ConcurrentDictionary<string, (string Token, DateTime ExpiresAt)> _tokenCache = new();
        private readonly string _stremioAuthUrl = "https://api.strem.io/api/login";
        private readonly TimeSpan _tokenLifeTime = TimeSpan.FromHours(1);

        public async Task<string?> GetAuthTokenAsync(string email, string password, string type = "Login", bool facebookLogin = false, CancellationToken cancellationToken = default)
        {
            if(_tokenCache.TryGetValue(email, out var cachedToken) && DateTime.UtcNow < cachedToken.ExpiresAt)
            {
                return cachedToken.Token;
            }

            StringContent content = new(JsonSerializer.Serialize(new
            {
                email,
                password,
                facebook = facebookLogin,
                type
            }),
            Encoding.UTF8,
            "application/json");

            var response = await _httpClient.PostAsync(_stremioAuthUrl, content, cancellationToken);

            if(!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to authenticate with Stremio API. Status Code: {StatusCode}", response.StatusCode);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var authResult = JsonSerializer.Deserialize<StremioAuthModel>(responseContent);

            var token = authResult?.Result?.Token;
            if(token != null)
            {
                _tokenCache[email] = (token, DateTime.UtcNow.Add(_tokenLifeTime));
            }

            return token;
        }

        public void InvalidateToken(string email)
        {
            _tokenCache.TryRemove(email, out _);
        }
    }
}
