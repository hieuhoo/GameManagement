using GameManagement.Share.ClassData;

namespace GameManagement.Service.IService
{
	public interface IUserLockHistoryService
	{
		Task<bool> AddLockHistoryAsync(UserLockHistoryData data);
		//Task<bool> UpdateLockHistoryAsync(UserLockHistoryData data);
	}
}
