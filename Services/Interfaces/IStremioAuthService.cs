namespace GlanceStremioActivity.Services.Interfaces
{
    public interface IStremioAuthService
    {
        public Task<string?> GetAuthTokenAsync(string email, string password, string type = "Login", bool facebookLogin = false, CancellationToken cancellationToken = default);
        void InvalidateToken(string email);
    }
}
