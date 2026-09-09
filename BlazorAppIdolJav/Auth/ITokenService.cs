using GameManagement.Auth.Models;
using GameManagement.Share.ClassDB;

namespace GameManagement.Auth
{
    public interface ITokenService
    {
        TokenResponse CreateTokens(User user, string refreshToken);
        string CreateRefreshToken();
        string HashRefreshToken(string refreshToken);
    }
}
