using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
	public interface IUserGameLibraryRepository
	{
		IQueryable<UserGameLibrary> GetQueryable();
		Task<List<UserGameLibrary>> GetAllWithFilterAsync(IQueryable<UserGameLibrary> query, LibrarySearch search);
		Task<bool> AddGameToLibraryAsync(UserGameLibrary data);
        Task<bool> CheckExistGameInLibraryAsync(string gameId, string userId);

    }
}
