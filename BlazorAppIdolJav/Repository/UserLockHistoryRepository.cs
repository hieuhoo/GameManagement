using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Share.ClassDB;

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
	}
}
