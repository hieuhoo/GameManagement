using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
    public interface IGameRepository
    {
        IQueryable<Game> GetQueryable();
        Task<List<Game>> GetAllWithFilterAsync(IQueryable<Game>  query , GameSearch search);
    }
}
