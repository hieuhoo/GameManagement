using GameManagement.Auth.Models;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.Extension;
using GameManagement.Share.ClassDB;
using static GameManagement.Share.Extension.EnumExtension;
using AutoMapper;

namespace GameManagement.Auth
{
    public sealed class AuthService : IAuthService
    {
        private const int MaxLoginAttempts = 3;
        private readonly IMapper _mapper;

        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IUserLockHistoryService _lockHistoryService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly JwtOptions _jwtOptions;

        public AuthService(
            IUserRepository userRepository,
            IUserService userService,
            IUserLockHistoryService lockHistoryService,
            IRefreshTokenRepository refreshTokenRepository,
            ITokenService tokenService,
            IMapper mapper,
            Microsoft.Extensions.Options.IOptions<JwtOptions> jwtOptions)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _userService = userService;
            _lockHistoryService = lockHistoryService;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return Fail("Vui lòng nhập tên đăng nhập và mật khẩu.");
            }

            var user = await _userRepository.GetUserInfoAsync(new UserSearch
            {
                UserName = request.UserName.Trim()
            });

            if (string.IsNullOrWhiteSpace(user.Id))
            {
                return Fail("Sai tên đăng nhập hoặc mật khẩu.");
            }

            if (string.Equals(user.Status, AccountStatus.Lock.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var reason = user.LockReason.GetEnumDescription<AccountLockReason>();
                return Fail($"Tài khoản đã bị {reason}. Vui lòng liên hệ quản trị viên để được mở khóa.");
            }

            // PasswordHash là nguồn xác thực chính. PassWord chỉ được giữ để tương thích
            // với schema cũ của project.
            var passwordValid = !string.IsNullOrWhiteSpace(user.PasswordHash)
                ? BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)
                : user.PassWord == request.Password;
            var userData = new UserData();
            if (!passwordValid)
            {
                user.FailedLoginCount = (user.FailedLoginCount ?? 0) + 1;
                user.UpdatedDate = DateTime.Now;

                if (user.FailedLoginCount >= MaxLoginAttempts)
                {
                    user.Status = AccountStatus.Lock.ToString();
                    user.LockReason = AccountLockReason.ByIncorrectPassword.ToString();
                    user.LockBy = LockPerson.System.ToString();
                    userData = _mapper.Map<UserData>(user);
                    await _userService.UpdateAccountAsync(userData);

                    if (!string.IsNullOrWhiteSpace(user.Id))
                    {
                        var history = user.Id.FillLockHistoryData(
                            AccountOperation.Lock,
                            LockPerson.System,
                            AccountLockReason.ByIncorrectPassword);

                        await _lockHistoryService.AddLockHistoryAsync(history);
                    }

                    return Fail($"Bạn đã đăng nhập sai {MaxLoginAttempts} lần. Tài khoản đã bị khóa.");
                }
                userData = _mapper.Map<UserData>(user);
                await _userService.UpdateAccountAsync(userData);

                var remaining = MaxLoginAttempts - user.FailedLoginCount.Value;
                return Fail($"Sai tên đăng nhập hoặc mật khẩu. Tài khoản sẽ bị khóa sau {remaining} lần sai nữa.");
            }

            // Hỗ trợ migrate tài khoản cũ chưa có PasswordHash.
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            if ((user.FailedLoginCount ?? 0) > 0)
            {
                user.FailedLoginCount = 0;
            }

            user.UpdatedDate = DateTime.Now;
            userData = _mapper.Map<UserData>(user);
            await _userService.UpdateAccountAsync(userData);

            var refreshToken = _tokenService.CreateRefreshToken();
            var tokenHash = _tokenService.HashRefreshToken(refreshToken);

            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                TokenHash = tokenHash,
                CreateDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
            });

            var tokens = _tokenService.CreateTokens(user, refreshToken);

            return new LoginResult
            {
                Succeeded = true,
                Tokens = tokens
            };
        }

        public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            var oldHash = _tokenService.HashRefreshToken(refreshToken);
            var storedToken = await _refreshTokenRepository.GetActiveAsync(oldHash);

            if (storedToken is null)
                return null;

            var user = await _userRepository.GetUserInfoAsync(new UserSearch
            {
                Id = storedToken.UserId
            });

            if (string.IsNullOrWhiteSpace(user.Id) ||
                string.Equals(user.Status, AccountStatus.Lock.ToString(), StringComparison.OrdinalIgnoreCase) ||
                string.Equals(user.Status, AccountStatus.Inactive.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                await _refreshTokenRepository.RevokeAsync(storedToken);
                return null;
            }

            var newRefreshToken = _tokenService.CreateRefreshToken();
            var newHash = _tokenService.HashRefreshToken(newRefreshToken);

            await _refreshTokenRepository.RevokeAsync(storedToken, newHash);

            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                TokenHash = newHash,
                CreateDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
            });

            return _tokenService.CreateTokens(user, newRefreshToken);
        }

        public async Task LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return;

            var hash = _tokenService.HashRefreshToken(refreshToken);
            var token = await _refreshTokenRepository.GetActiveAsync(hash);

            if (token is not null)
                await _refreshTokenRepository.RevokeAsync(token);
        }

        public Task LogoutAllAsync(string userId)
        {
            return _refreshTokenRepository.RevokeAllForUserAsync(userId);
        }

        private static LoginResult Fail(string message)
        {
            return new LoginResult
            {
                Succeeded = false,
                ErrorMessage = message
            };
        }
    }
}
