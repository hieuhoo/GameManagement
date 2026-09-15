using AutoMapper;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;

namespace GameManagement.Service
{
	public class UserGameLibraryService : IUserGameLibraryService

	{
		private readonly IUserGameLibraryRepository _repo;
		readonly IMapper _mapper;

		public UserGameLibraryService(IUserGameLibraryRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<List<UserGameLibraryData>> GetAllWithFilterAsync(LibrarySearch search)
		{
			try
			{
				var filter = search.CreateFilter(_repo.GetQueryable());
				var result = await _repo.GetAllWithFilterAsync(filter, search);
				var data = _mapper.Map<List<UserGameLibraryData>>(result);
				return data;
			}
			catch
			{
				return new List<UserGameLibraryData>();
			}
		}

		public async Task<bool> AddGameToLibraryAsync(UserGameLibraryData data)
		{
			try
			{
				var wishList = _mapper.Map<UserGameLibrary>(data);
				var isSuccess = await _repo.AddGameToLibraryAsync(wishList);
				return isSuccess;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

        public async Task<bool> CheckExistGameInLibraryAsync(string gameId, string userId)
        {
            try
            {
                return await _repo.CheckExistGameInLibraryAsync(gameId, userId);
            }
            catch
            {
                return false;
            }
        }
    }
}
