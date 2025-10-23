using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
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

        public async Task<bool> CheckUserLoginAsync(User data)
        {
            try
            {
                var allUser = await GetAllAsync();
                foreach (var item in allUser)
                {
                    if (item.UserName == data.UserName && item.PassWord == data.PassWord)
                    {
                        return true; 
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<User> GetUserInfoAsync(UserSearch search)
        {
            try
            {
                var filter = search.CreateFilter(GetQueryable());
                var data = await filter.FirstOrDefaultAsync();
                return data ?? new User();
            }
            catch (Exception ex)
            {
                return new User();
            }
        }
    }
}
