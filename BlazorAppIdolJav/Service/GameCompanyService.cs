using AutoMapper;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;

namespace GameManagement.Service
{
    public class GameCompanyService : IGameCompanyService
    {
        private readonly IGameCompanyRepository _repo;
        readonly IMapper _mapper;

        public GameCompanyService(IGameCompanyRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<GameCompanyData>> GetAllWithFilterAsync(GameCompanySearch search)
        {
            var filter = search.CreateFilter(_repo.GetQueryable());
            var result = await _repo.GetAllWithFilterAsync(filter, search);
            var data = _mapper.Map<List<GameCompanyData>>(result);
            return data;
        }

        public async Task<bool> SaveCompanyAsync(GameCompanyData data)
        {
            try
            {
                var game = _mapper.Map<GameCompany>(data);
                var isSuccess = await _repo.SaveCompanyAsync(game);
                return isSuccess;
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateCompanyAsync(GameCompanyData data)
        {
            try
            {
                var game = _mapper.Map<GameCompany>(data);
                var isSuccess = await _repo.UpdateCompanyAsync(game);
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteCompanyAsync(GameCompanyData data)
        {
            try
            {
                var game = _mapper.Map<GameCompany>(data);
                var isSuccess = await _repo.DeleteCompanyAsync(game);
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
