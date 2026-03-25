using GlanceStremioActivity.DTOs;
using GlanceStremioActivity.Enums;
using GlanceStremioActivity.Models;
using GlanceStremioActivity.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static GlanceStremioActivity.Utils.Utils;

namespace GlanceStremioActivity.Controllers
{
    [ApiController]
    [Route("")]
    public class WidgetController(IStremioApiService _stremioApiService, IUserConfigurationService _userConfigService, Config _config) : Controller
    {

        [HttpGet("activity")]
        public async Task<IActionResult> GetUsersActivities([FromHeader] string token, [FromQuery] ActivityTypeEnum type, CancellationToken cancellationToken = default)
        {

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is required.");
            }

            if (!string.Equals(token, _config.ApiKey))
            {
                return Unauthorized("Invalid token.");
            }

            // Load users from configuration
            var users = await _userConfigService.GetUsersAsync();

            if (users.Count == 0)
            {
                return BadRequest("No users configured. Please add users to users.json file.");
            }

            // Convert users to GetActivityRequestDTO
            List<GetActivityRequestDTO> requests = [.. users.Select(u => new GetActivityRequestDTO
            {
                Email = u.Email,
                Password = u.Password,
                DisplayName = u.DisplayName ?? u.Email
            })];

            List<UserActivityResponseDTO?> items = await _stremioApiService.GetUsersActivitiesAsync(requests, type, cancellationToken);

            Response.Headers["Widget-Title"] = type == Enums.ActivityTypeEnum.Watching ? "Stremio - Watching" : "Stremio - Last Show Watched";
            Response.Headers["Widget-Title-URL"] = "https://web.strem.io/";
            Response.Headers["Widget-Content-Type"] = "html";

            if (items.Count == 0)
            {
                return Content(NoActivityHtml(type), "text/html");
            }

            return Content(BuildHtml(items, type));
        }
    }
}
