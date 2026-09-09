namespace GameManagement.Auth.Models
{
    public sealed class TokenResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; init; }
        public DateTime RefreshTokenExpiresAt { get; init; }

        public string UserId { get; init; } = string.Empty;
        public string UserName { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
    }
}
