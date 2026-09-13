using AntDesign;
using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
	public class UserWalletRepository : Repository<UserWallet> , IUserWalletRepository
	{
        private readonly ApplicationDbContext _context;

        public UserWalletRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<UserWallet>> GetAllWithFilterAsync(IQueryable<UserWallet> query, UserWalletSearch search)
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

        public IQueryable<UserWallet> GetQueryable()
        {
            return _context.UserWallet.AsQueryable();

        }
    }
}
