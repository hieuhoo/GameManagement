using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
	public interface IUserLockHistoryRepository
	{
		Task<bool> AddLockHistoryAsync(UserLockHistory data);
	}
}
