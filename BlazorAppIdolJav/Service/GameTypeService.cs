using AutoMapper;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;

namespace GameManagement.Service
{
    public class GameTypeService : IGameTypeService
    {
        private readonly IGameTypeRepository _repo;
        readonly IMapper _mapper;

        public GameTypeService(IGameTypeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<GameTypeData>> GetAllWithFilterAsync(GameTypeSearch search)
        {
            var filter = search.CreateFilter(_repo.GetQueryable());
            var result = await _repo.GetAllWithFilterAsync(filter, search);
            var data = _mapper.Map<List<GameTypeData>>(result);
            return data;
        }

        public async Task<bool> SaveGameTypeAsync(GameTypeData data)
        {
            try
            {
                var game = _mapper.Map<GameType>(data);
                var isSuccess = await _repo.SaveGameTypeAsync(game);
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateGameTypeAsync(GameTypeData data)
        {
            try
            {
                var game = _mapper.Map<GameType>(data);
                var isSuccess = await _repo.UpdateGameTypeAsync(game);
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteGameTypeAsync(GameTypeData data)
        {
            try
            {
                var game = _mapper.Map<GameType>(data);
                var isSuccess = await _repo.DeleteGameTypeAsync(game);
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
