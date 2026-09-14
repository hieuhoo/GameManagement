using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
    public class UserGameWishlistRepository : Repository<UserGameWishlist>, IUserGameWishlistRepository
    {
        private readonly ApplicationDbContext _context;
        public UserGameWishlistRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<UserGameWishlist>> GetAllWithFilterAsync(IQueryable<UserGameWishlist> query, WishlistSearch search)
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

        public IQueryable<UserGameWishlist> GetQueryable()
        {
            return _context.UserGameWishlist.AsQueryable();
        }

        public async Task<bool> AddToWishlistAsync(UserGameWishlist data)
        {
            try
            {
                await AddAsync(data);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(UserGameWishlist data)
        {
            try
            {
                await DeleteAsync(data.Id);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<bool> CheckExistGameInWishlistAsync(string gameId, string userId)
        {
            var query = _context.UserGameWishlist
                    .Where(x => x.GameId == gameId && x.UserId == userId);

            var result = await query.AnyAsync();

            return result;
        }
    }
}
