using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
	public interface IGameRepository
	{
		IQueryable<Game> GetQueryable();
		Task<List<Game>> GetAllWithFilterAsync(IQueryable<Game> query, GameSearch search);
		Task<bool> SaveGameAsync(Game data);
		Task<bool> UpdateGameAsync(Game data);
		Task<bool> DeleteGameAsync(Game data);
	}
}
