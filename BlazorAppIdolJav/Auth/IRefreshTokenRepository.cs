using GameManagement.Share.ClassDB;

namespace GameManagement.Auth
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetActiveAsync(string tokenHash);
        Task RevokeAsync(RefreshToken token, string? replacedByTokenHash = null);
        Task RevokeAllForUserAsync(string userId);
    }
}
