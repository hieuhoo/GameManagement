using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
	public interface IUserGameWishlistRepository
	{
		IQueryable<UserGameWishlist> GetQueryable();
		Task<List<UserGameWishlist>> GetAllWithFilterAsync(IQueryable<UserGameWishlist> query, WishlistSearch search);
		Task<bool> AddToWishlistAsync(UserGameWishlist data);
		Task<bool> RemoveFromWishlistAsync(UserGameWishlist data);
	}
}
