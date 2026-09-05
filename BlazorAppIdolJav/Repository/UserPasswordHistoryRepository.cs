using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
    public class UserPasswordHistoryRepository : Repository<UserPasswordHistory> , IUserPasswordHistoryRepository
    {
        private readonly ApplicationDbContext _context;
        public UserPasswordHistoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> AddPasswordHistoryAsync(UserPasswordHistory data)
        {
            await AddAsync(data);
            return true;
        }

        public async Task<bool> CheckPasswordRecentlyUsedAsync(string newPassword, string userId, int time)
        {
            try
            {
                var histories = await _context.UserPasswordHistory
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.CreateDate)
                    .Take(time)
                    .ToListAsync();

                foreach (var history in histories)
                {
                    if (history.CurrentPassword == newPassword ||
                        history.PreviousPassword == newPassword)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }
    }
}
