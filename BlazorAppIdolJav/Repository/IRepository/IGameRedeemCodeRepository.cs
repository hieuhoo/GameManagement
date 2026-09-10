using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
    public interface IGameRedeemCodeRepository
    {
        IQueryable<GameRedeemCode> GetQueryable();
        Task<List<GameRedeemCode>> GetAllWithFilterAsync(IQueryable<GameRedeemCode> query, GameRedeemSearch search);
        Task<bool> SaveRedeemAsync(GameRedeemCode data);
        Task<bool> UpdateRedeemAsync(GameRedeemCode data);
        Task<bool> DeleteRedeemAsync(GameRedeemCode data);
        Task<bool> SaveListRedeemAsync(List<GameRedeemCode> data);

    }
}
