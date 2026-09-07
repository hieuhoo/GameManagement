using AutoMapper;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;

namespace GameManagement.Service
{
	public class GameService : IGameService
	{
		private readonly IGameRepository _repo;
		readonly IMapper _mapper;

		public GameService(IGameRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<List<GameData>> GetAllWithFilterAsync(GameSearch search)
		{
			var filter = search.CreateFilter(_repo.GetQueryable());
			var result = await _repo.GetAllWithFilterAsync(filter, search);
			var data = _mapper.Map<List<GameData>>(result);
			return data;
		}

		public async Task<bool> SaveGameAsync(GameData data)
		{
			try
			{
				var game = _mapper.Map<Game>(data);
				var isSuccess = await _repo.SaveGameAsync(game);
				return isSuccess;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<bool> UpdateGameAsync(GameData data)
		{
			try
			{
				var game = _mapper.Map<Game>(data);
				var isSuccess = await _repo.UpdateGameAsync(game);
				return isSuccess;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<bool> DeleteGameAsync(GameData data)
		{
			try
			{
				var game = _mapper.Map<Game>(data);
				var isSuccess = await _repo.DeleteGameAsync(game);
				return isSuccess;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
