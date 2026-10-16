using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
	public interface INotificationGameRepository
	{
		IQueryable<UserNotification> GetQueryable();
		Task<bool> AddNotificationAsync(UserNotification noti);
		Task<bool> UpdateNotificationAsync(UserNotification noti);
        Task<List<UserNotification>> GetAllWithFilterAsync(IQueryable<UserNotification> query, NotiSearch search);

	}
}
