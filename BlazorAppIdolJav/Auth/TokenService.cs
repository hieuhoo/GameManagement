using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GameManagement.Auth.Models;
using GameManagement.Share.ClassDB;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GameManagement.Auth
{
    public sealed class TokenService : ITokenService
    {
        private readonly JwtOptions _options;

        public TokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;

            if (string.IsNullOrWhiteSpace(_options.Key) || Encoding.UTF8.GetByteCount(_options.Key) < 32)
            {
                throw new InvalidOperationException(
                    "Jwt:Key phải có ít nhất 32 bytes. Không dùng key mặc định trong production.");
            }
        }

        public TokenResponse CreateTokens(User user, string refreshToken)
        {
            var now = DateTime.UtcNow;
            var accessExpires = now.AddMinutes(_options.AccessTokenMinutes);
            var refreshExpires = now.AddDays(_options.RefreshTokenDays);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName),
                new("display_name", user.Name ?? string.Empty),
                new(ClaimTypes.Role, user.Role ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: accessExpires,
                signingCredentials: credentials);

            return new TokenResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = accessExpires,
                RefreshTokenExpiresAt = refreshExpires,
                UserId = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                Role = user.Role ?? string.Empty
            };
        }

        public string CreateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public string HashRefreshToken(string refreshToken)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
            return Convert.ToHexString(hash);
        }
    }
}
