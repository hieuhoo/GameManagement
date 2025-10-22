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
            var result  = await _repo.GetAllWithFilterAsync(filter, search);
            var data = _mapper.Map<List<GameData>>(result);
            return data;
        }
    }
}
