namespace GameManagement.Auth.Models
{
    public sealed class LoginResult
    {
        public bool Succeeded { get; init; }
        public string ErrorMessage { get; init; } = string.Empty;
        public TokenResponse? Tokens { get; init; }
    }
}
