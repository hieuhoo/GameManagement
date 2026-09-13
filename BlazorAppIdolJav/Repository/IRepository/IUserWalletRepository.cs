using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
	public interface IUserWalletRepository
	{
        IQueryable<UserWallet> GetQueryable();
        Task<List<UserWallet>> GetAllWithFilterAsync(IQueryable<UserWallet> query, UserWalletSearch search);
    }
}
