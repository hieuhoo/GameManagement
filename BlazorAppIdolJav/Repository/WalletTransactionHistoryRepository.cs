using GameManagement.CoreConfig.Extensions;
using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.Repository
{
	public class WalletTransactionHistoryRepository : Repository<WalletTransactionHistory>, IWalletTransactionHistoryRepository
	{
		private readonly ApplicationDbContext _context;

		public WalletTransactionHistoryRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<List<WalletTransactionHistory>> GetAllWithFilterAsync(IQueryable<WalletTransactionHistory> query, TransactionHistorySearch search)
		{
			try
			{
				var result = await query.ToListAsync();
				return result;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public IQueryable<WalletTransactionHistory> GetQueryable()
		{
			return _context.WalletTransactionHistory.AsQueryable();
		}

		public async Task<(int TotalSold, int? TotalRevenue)> GetTotalGameRevenue(string gameId)
		{
			try
			{
				var query = _context.WalletTransactionHistory
					.Where(x =>
						x.ReferenceId == gameId &&
						x.Type == WalletTransactionType.PurchaseGame.ToString());

				var totalSold = await query.CountAsync();

				var totalRevenue = await query
					.SumAsync(x => x.PurchaseGamePrice) ?? 0;
				return (totalSold, totalRevenue);
			}
			catch
			{
				return (0, 0);
			}
		}
	}
}
