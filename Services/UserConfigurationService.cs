using GlanceStremioActivity.Models;
using GlanceStremioActivity.Services.Interfaces;
using System.Text.Json;

namespace GlanceStremioActivity.Services
{
    public class UserConfigurationService : IUserConfigurationService
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        private readonly string _usersFilePath;
        private readonly ILogger<UserConfigurationService> _logger;

        public UserConfigurationService(ILogger<UserConfigurationService> logger)
        {
            _usersFilePath = Environment.GetEnvironmentVariable("UsersFilePath") ?? "users.json";
            _logger = logger;
        }

        public async Task<List<StremioUser>> GetUsersAsync()
        {
            // 1. Try to load from the STREMIO_USERS environment variable
            var users = LoadFromEnvironmentVariable();
            if (users.Count > 0)
                return users;

            // 2. Fallback: load from JSON file
            users = await LoadFromFileAsync();
            if (users.Count > 0)
                return users;

            _logger.LogWarning("No users configured. Set the STREMIO_USERS environment variable or mount a users.json file.");
            return [];
        }

        private List<StremioUser> LoadFromEnvironmentVariable()
        {
            var envValue = Environment.GetEnvironmentVariable("STREMIO_USERS");
            if (string.IsNullOrWhiteSpace(envValue))
                return [];

            try
            {
                var usersConfig = JsonSerializer.Deserialize<UsersConfiguration>(envValue, _jsonOptions);
                if (usersConfig?.Users is { Count: > 0 })
                {
                    _logger.LogInformation("Loaded {Count} user(s) from STREMIO_USERS environment variable.", usersConfig.Users.Count);
                    return usersConfig.Users;
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse STREMIO_USERS environment variable.");
            }

            return [];
        }

        private async Task<List<StremioUser>> LoadFromFileAsync()
        {
            if (!File.Exists(_usersFilePath))
            {
                _logger.LogWarning("Users file not found at: {FilePath}.", _usersFilePath);
                return [];
            }

            try
            {
                var jsonContent = await File.ReadAllTextAsync(_usersFilePath);
                var usersConfig = JsonSerializer.Deserialize<UsersConfiguration>(jsonContent, _jsonOptions);

                if (usersConfig?.Users is { Count: > 0 })
                {
                    _logger.LogInformation("Loaded {Count} user(s) from file: {FilePath}.", usersConfig.Users.Count, _usersFilePath);
                    return usersConfig.Users;
                }

                _logger.LogWarning("No users found in configuration file: {FilePath}.", _usersFilePath);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse users JSON file: {FilePath}.", _usersFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading users configuration from: {FilePath}.", _usersFilePath);
            }

            return [];
        }
    }
}
