using GameManagement.Data;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Auth
{
    public sealed class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _context.Set<RefreshToken>().AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public Task<RefreshToken?> GetActiveAsync(string tokenHash)
        {
            return _context.Set<RefreshToken>()
                .FirstOrDefaultAsync(x =>
                    x.TokenHash == tokenHash &&
                    x.RevokedDate == null &&
                    x.ExpiredDate > DateTime.UtcNow);
        }

        public async Task RevokeAsync(RefreshToken token, string? replacedByTokenHash = null)
        {
            token.RevokedDate = DateTime.UtcNow;
            token.ReplacedByTokenHash = replacedByTokenHash;
            await _context.SaveChangesAsync();
        }

        public async Task RevokeAllForUserAsync(string userId)
        {
            var tokens = await _context.Set<RefreshToken>()
                .Where(x => x.UserId == userId && x.RevokedDate == null)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.RevokedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}
