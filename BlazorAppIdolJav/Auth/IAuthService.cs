using GameManagement.Auth.Models;

namespace GameManagement.Auth
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginRequest request);
        Task<TokenResponse?> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string refreshToken);
        Task LogoutAllAsync(string userId);
    }
}
