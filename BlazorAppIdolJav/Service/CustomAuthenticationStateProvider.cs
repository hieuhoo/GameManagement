using GameManagement.Auth;
using GameManagement.Auth.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GameManagement.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private const string AccessTokenKey = "accessToken";
        private const string RefreshTokenKey = "refreshToken";

        private readonly ProtectedLocalStorage _localStorage;
        private readonly IAuthService _authService;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(
            ProtectedLocalStorage localStorage,
            IAuthService authService)
        {
            _localStorage = localStorage;
            _authService = authService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var access = await _localStorage.GetAsync<string>(AccessTokenKey);
                var refresh = await _localStorage.GetAsync<string>(RefreshTokenKey);

                var accessToken = access.Success ? access.Value : null;
                var refreshToken = refresh.Success ? refresh.Value : null;

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    var principal = CreatePrincipalFromToken(accessToken);

                    if (principal.Identity?.IsAuthenticated == true)
                    {
                        var expiresAt = GetExpiration(accessToken);

                        if (expiresAt > DateTime.UtcNow.AddSeconds(30))
                            return new AuthenticationState(principal);
                    }
                }

                // Access token hết hạn nhưng refresh token còn hiệu lực -> rotate token.
                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    var tokens = await _authService.RefreshTokenAsync(refreshToken);

                    if (tokens is not null)
                    {
                        await StoreTokensAsync(tokens);
                        return new AuthenticationState(CreatePrincipalFromToken(tokens.AccessToken));
                    }
                }

                await ClearTokensAsync();
            }
            catch
            {
                // ProtectedLocalStorage có thể chưa sẵn sàng trong prerender/lần render đầu.
            }

            return new AuthenticationState(_anonymous);
        }

        public async Task MarkUserAsAuthenticatedAsync(TokenResponse tokens)
        {
            await StoreTokensAsync(tokens);

            var principal = CreatePrincipalFromToken(tokens.AccessToken);
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(principal)));
        }

        public async Task MarkUserAsLoggedOutAsync()
        {
            try
            {
                var refresh = await _localStorage.GetAsync<string>(RefreshTokenKey);
                if (refresh.Success && !string.IsNullOrWhiteSpace(refresh.Value))
                {
                    await _authService.LogoutAsync(refresh.Value);
                }
            }
            catch
            {
            }

            await ClearTokensAsync();
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(_anonymous)));
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            var result = await _localStorage.GetAsync<string>(AccessTokenKey);
            return result.Success ? result.Value : null;
        }

        private async Task StoreTokensAsync(TokenResponse tokens)
        {
            await _localStorage.SetAsync(AccessTokenKey, tokens.AccessToken);
            await _localStorage.SetAsync(RefreshTokenKey, tokens.RefreshToken);
        }

        private async Task ClearTokensAsync()
        {
            try
            {
                await _localStorage.DeleteAsync(AccessTokenKey);
                await _localStorage.DeleteAsync(RefreshTokenKey);
            }
            catch
            {
            }
        }

        private static ClaimsPrincipal CreatePrincipalFromToken(string accessToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(accessToken);

                var claims = jwt.Claims.ToList();

                // JwtRegisteredClaimNames.Sub đã có trong token; Name/Role được tạo
                // bởi TokenService nên ClaimsPrincipal dùng được với AuthorizeView/IsInRole.
                var identity = new ClaimsIdentity(
                    claims,
                    authenticationType: "Bearer",
                    nameType: ClaimTypes.Name,
                    roleType: ClaimTypes.Role);

                return new ClaimsPrincipal(identity);
            }
            catch
            {
                return new ClaimsPrincipal(new ClaimsIdentity());
            }
        }

        private static DateTime GetExpiration(string accessToken)
        {
            try
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
                return jwt.ValidTo;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }
}
