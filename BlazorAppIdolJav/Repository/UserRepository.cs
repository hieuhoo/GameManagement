using BlazorAppIdolJav.CoreConfig.Repository;
using BlazorAppIdolJav.Data;
using BlazorAppIdolJav.Repository.IRepository;
using BlazorAppIdolJav.Service.IService;
using BlazorAppIdolJav.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppIdolJav.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllWithFilterAsync(IQueryable<User> query, UserSearch search)
        {
            try
            {
                var result = await query.ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<User> GetQueryable()
        {
            return _context.User.AsQueryable();
        }

        public async Task<bool> RegisterAccountAsync(User data)
        {
            try
            {
                await AddAsync(data);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CheckExistUserInfoAsync(User data)
        {
            try
            {
                var existData = await GetAllAsync();
                bool isExist = existData.Any(u =>
                   u.UserName.Equals(data.UserName, StringComparison.OrdinalIgnoreCase) ||
                   u.Email.Equals(data.Email, StringComparison.OrdinalIgnoreCase));
                return isExist;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
