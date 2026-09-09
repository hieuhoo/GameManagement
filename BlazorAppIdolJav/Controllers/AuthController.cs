using GameManagement.Auth;
using GameManagement.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameManagement.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (!result.Succeeded || result.Tokens is null)
                return Unauthorized(new { message = result.ErrorMessage });

            return Ok(result.Tokens);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponse>> Refresh([FromBody] RefreshRequest request)
        {
            var tokens = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (tokens is null)
                return Unauthorized(new { message = "Refresh token không hợp lệ hoặc đã hết hạn." });

            return Ok(tokens);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequest request)
        {
            await _authService.LogoutAsync(request.RefreshToken);
            return NoContent();
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            await _authService.LogoutAllAsync(userId);
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                UserName = User.Identity?.Name,
                Role = User.FindFirstValue(ClaimTypes.Role)
            });
        }
    }

    public sealed class RefreshRequest
    {
        public string RefreshToken { get; init; } = string.Empty;
    }
}
