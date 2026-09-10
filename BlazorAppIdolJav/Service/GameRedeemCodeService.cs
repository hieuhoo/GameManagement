using AutoMapper;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;

namespace GameManagement.Service
{
    public class GameRedeemCodeService : IGameRedeemCodeService
    {
        private readonly IGameRedeemCodeRepository _repo;
        readonly IMapper _mapper;

        public GameRedeemCodeService(IGameRedeemCodeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<GameRedeemCodeData>> GetAllWithFilterAsync(GameRedeemSearch search)
        {
            var filter = search.CreateFilter(_repo.GetQueryable());
            var result = await _repo.GetAllWithFilterAsync(filter, search);
            var data = _mapper.Map<List<GameRedeemCodeData>>(result);
            return data;
        }

        public async Task<bool> SaveRedeemAsync(GameRedeemCodeData data)
        {
            try
            {
                var game = _mapper.Map<GameRedeemCode>(data);
                var isSuccess = await _repo.SaveRedeemAsync(game);
                return isSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateRedeemAsync(GameRedeemCodeData data)
        {
            try
            {
                var game = _mapper.Map<GameRedeemCode>(data);
                var isSuccess = await _repo.UpdateRedeemAsync(game);
                return isSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteRedeemAsync(GameRedeemCodeData data)
        {
            try
            {
                var game = _mapper.Map<GameRedeemCode>(data);
                var isSuccess = await _repo.DeleteRedeemAsync(game);
                return isSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SaveListRedeemAsync(List<GameRedeemCodeData> data)
        {
            try
            {
                var game = _mapper.Map<List<GameRedeemCode>>(data);
                var isSuccess = await _repo.SaveListRedeemAsync(game);
                return isSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
