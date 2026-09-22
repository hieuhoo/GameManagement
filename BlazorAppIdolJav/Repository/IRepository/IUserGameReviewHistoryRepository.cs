using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
	public interface IUserGameReviewHistoryRepository
	{
		Task<List<UserGameReviewHistory>> GetAllWithFilterAsync(IQueryable<UserGameReviewHistory> query,
									ReviewHistorySearch search);
		IQueryable<UserGameReviewHistory> GetQueryable();

	}
}
