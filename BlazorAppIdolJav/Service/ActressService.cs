using AutoMapper;
using BlazorAppIdolJav.Repository.IRepository;
using BlazorAppIdolJav.Service.IService;
using BlazorAppIdolJav.Share.ClassData;
using BlazorAppIdolJav.Share.ClassDB;

namespace BlazorAppIdolJav.Service
{
    public class ActressService : IActressService
    {
        private readonly IActressRepository _repo;
        readonly IMapper _mapper;

        public ActressService(IActressRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<ActressData>> GetAllWithFilterAsync(ActressSearch search)
        {
            var filter = search.CreateFilter(_repo.GetQueryable());
            var result  = await _repo.GetAllWithFilterAsync(filter, search);
            var data = _mapper.Map<List<ActressData>>(result);
            return data;
        }
    }
}
