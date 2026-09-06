using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
	public interface IUserLockHistoryRepository
	{
        IQueryable<UserLockHistory> GetQueryable();
        Task<List<UserLockHistory>> GetAllWithFilterAsync(IQueryable<UserLockHistory> query, UserLockHistorySearch search);

        Task<bool> AddLockHistoryAsync(UserLockHistory data);
	}
}
