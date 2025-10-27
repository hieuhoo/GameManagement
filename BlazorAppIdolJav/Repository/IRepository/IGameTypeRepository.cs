using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
    public interface IGameTypeRepository
    {
        IQueryable<GameType> GetQueryable();
        Task<List<GameType>> GetAllWithFilterAsync(IQueryable<GameType> query, GameTypeSearch search);
        Task<bool> SaveGameTypeAsync(GameType data);
        Task<bool> UpdateGameTypeAsync(GameType data);
        Task<bool> DeleteGameTypeAsync(GameType data);
    }
}
