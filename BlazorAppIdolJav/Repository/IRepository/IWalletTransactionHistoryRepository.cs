using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
	public interface IWalletTransactionHistoryRepository
	{
		IQueryable<WalletTransactionHistory> GetQueryable();
		Task<List<WalletTransactionHistory>> GetAllWithFilterAsync(IQueryable<WalletTransactionHistory> query, TransactionHistorySearch search);
	}
}
