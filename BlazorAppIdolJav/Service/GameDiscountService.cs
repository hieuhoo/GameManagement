using AutoMapper;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;

namespace GameManagement.Service
{
    public class GameDiscountService : IGameDiscountService
    {
        private readonly IGameDiscountRepository _repo;
        readonly IMapper _mapper;

        public GameDiscountService(IGameDiscountRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<GameDiscountData>> GetAllWithFilterAsync(DiscountSearch search)
        {
            var filter = search.CreateFilter(_repo.GetQueryable());
            var result = await _repo.GetAllWithFilterAsync(filter, search);
            var data = _mapper.Map<List<GameDiscountData>>(result);
            return data;
        }

        public async Task<bool> SaveDiscountAsync(GameDiscountData data)
        {
            try
            {
                var discount = _mapper.Map<GameDiscount>(data);
                var isSuccess = await _repo.SaveDiscountAsync(discount);
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateDiscountAsync(GameDiscountData data)
        {
            try
            {
                var discount = _mapper.Map<GameDiscount>(data);
                var isSuccess = await _repo.UpdateDiscountAsync(discount);
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteDiscountAsync(GameDiscountData data)
        {
            try
            {
                var discount = _mapper.Map<GameDiscount>(data);
                var isSuccess = await _repo.DeleteDiscountAsync(discount);
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
