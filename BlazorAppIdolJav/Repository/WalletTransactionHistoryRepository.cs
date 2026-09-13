using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

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
	}
}
