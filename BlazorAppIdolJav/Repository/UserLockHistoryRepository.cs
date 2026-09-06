using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
	public class UserLockHistoryRepository : Repository<UserLockHistory>, IUserLockHistoryRepository
	{
		private readonly ApplicationDbContext _context;
		public UserLockHistoryRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<bool> AddLockHistoryAsync(UserLockHistory data)
		{
			try
			{
				await AddAsync(data);
				return true;
			}
			catch
			{
				return false;
			}
		}

        public IQueryable<UserLockHistory> GetQueryable()
        {
            return _context.UserLockHistory.AsQueryable();
        }

        public async Task<List<UserLockHistory>> GetAllWithFilterAsync(IQueryable<UserLockHistory> query, UserLockHistorySearch search)
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
    }
}
