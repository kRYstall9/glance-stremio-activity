using GlanceStremioActivity.DTOs;
using GlanceStremioActivity.Enums;
using GlanceStremioActivity.Models;

namespace GlanceStremioActivity.Services.Interfaces
{
    public interface IStremioApiService
    {
        Task<List<UserActivityResponseDTO?>> GetUsersActivitiesAsync(List<GetActivityRequestDTO> requests, ActivityTypeEnum type, CancellationToken cancellationToken = default);
    }
}
