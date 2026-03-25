using GlanceStremioActivity.Models;

namespace GlanceStremioActivity.Services.Interfaces
{
    public interface IUserConfigurationService
    {
        Task<List<StremioUser>> GetUsersAsync();
    }
}
