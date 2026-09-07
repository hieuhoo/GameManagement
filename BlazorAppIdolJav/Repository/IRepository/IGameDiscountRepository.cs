using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
    public interface IGameDiscountRepository
    {
        IQueryable<GameDiscount> GetQueryable();
        Task<List<GameDiscount>> GetAllWithFilterAsync(IQueryable<GameDiscount> query, DiscountSearch search);
        Task<bool> SaveDiscountAsync(GameDiscount data);
        Task<bool> UpdateDiscountAsync(GameDiscount data);
        Task<bool> DeleteDiscountAsync(GameDiscount data);
    }
}
