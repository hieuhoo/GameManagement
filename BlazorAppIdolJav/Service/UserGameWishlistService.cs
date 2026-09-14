using AutoMapper;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;

namespace GameManagement.Service
{
	public class UserGameWishlistService : IUserGameWishlistService
	{
		private readonly IUserGameWishlistRepository _repo;
		readonly IMapper _mapper;

		public UserGameWishlistService(IUserGameWishlistRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<List<UserGameWishlistData>> GetAllWithFilterAsync(WishlistSearch search)
		{
			var filter = search.CreateFilter(_repo.GetQueryable());
			var result = await _repo.GetAllWithFilterAsync(filter, search);
			var data = _mapper.Map<List<UserGameWishlistData>>(result);
			return data;
		}

		public async Task<bool> AddToWishlistAsync(UserGameWishlistData data)
		{
			try
			{
				var wishList = _mapper.Map<UserGameWishlist>(data);
				var isSuccess = await _repo.AddToWishlistAsync(wishList);
				return isSuccess;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		
		public async Task<bool> RemoveFromWishlistAsync(UserGameWishlistData data)
		{
			try
			{
				var wishList = _mapper.Map<UserGameWishlist>(data);
				var isSuccess = await _repo.RemoveFromWishlistAsync(wishList);
				return isSuccess;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

        public async Task<bool> CheckExistGameInWishlistAsync(string gameId, string userId)
        {
			try
			{
				return await _repo.CheckExistGameInWishlistAsync(gameId, userId);
            }
			catch
			{
				return false;
			}
        }
    }
}
