using GlanceStremioActivity.DTOs;
using GlanceStremioActivity.Enums;
using GlanceStremioActivity.Models;
using GlanceStremioActivity.Services.Interfaces;
using System.Text.Json;

namespace GlanceStremioActivity.Services
{
    public class StremioApiService(HttpClient _httpClient, IStremioAuthService _authService, ILogger<IStremioApiService> _logger) : IStremioApiService
    {
        private readonly string _stremioBaseApiUrl = "https://api.strem.io/api";
        private async Task<UserActivityResponseDTO?> GetActivityAsync(GetActivityRequestDTO request, ActivityTypeEnum type, CancellationToken cancellationToken = default)
        {
            string? token = await _authService.GetAuthTokenAsync(request.Email, request.Password, cancellationToken: cancellationToken);

            var payload = new
            {
                type = "DatastoreGet",
                authKey = token,
                collection = "libraryItem",
                ids = Array.Empty<string>(),
                all = true
            };

            var response = await _httpClient.PostAsJsonAsync($"{this._stremioBaseApiUrl}/datastoreGet", payload, cancellationToken);

            if(!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("Unauthorized access for user {Email}. Attempting to refresh token.", request.Email);
                _authService.InvalidateToken(request.Email);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);

            var items = JsonSerializer.Deserialize<List<LibraryItem>>(result.GetProperty("result"));

            var latestContentWatched = items?.Where(x => x?.State?.LastWatched != null).OrderByDescending(x => x?.State?.LastWatched).FirstOrDefault();

            if (latestContentWatched != null)
            {
                if ((type == ActivityTypeEnum.LastShowWatched) || ((DateTime.UtcNow - latestContentWatched.State?.LastWatched) <= TimeSpan.FromMinutes(2)))
                {
                    UserActivityResponseDTO userActivity = new()
                    {
                        DisplayName = request.DisplayName,
                        Success = true,
                        Duration = (latestContentWatched.State?.TotalShowDuration / 60000) ?? null,
                        PosterUrl = latestContentWatched.PosterUrl ?? null,
                        ShowTitle = latestContentWatched.Name ?? null,
                        TimeWatched = (latestContentWatched.State?.TimeOffset / 60000) ?? null
                    };
                    return userActivity;
                }
            }

            return null;
        }

        public async Task<List<UserActivityResponseDTO?>> GetUsersActivitiesAsync(List<GetActivityRequestDTO> requests, ActivityTypeEnum type, CancellationToken cancellationToken = default)
        {
            var tasks = requests.Select(r => GetActivityAsync(r, type, cancellationToken));
            var results = await Task.WhenAll(tasks);
            return [.. results.Where(a => a != null)];           
        }
    }
}
