using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
	public interface IUserGameReviewRepository
	{
		IQueryable<UserGameReview> GetQueryable();
		Task<List<UserGameReview>> GetAllWithFilterAsync(IQueryable<UserGameReview> query, ReviewSearch search);
		Task<StatisticReviewGameData> GetStatisticAboutGameAsync(string gameId);
	}
}
