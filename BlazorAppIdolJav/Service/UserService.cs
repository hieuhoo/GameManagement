using AutoMapper;
using BlazorAppIdolJav.Repository.IRepository;
using BlazorAppIdolJav.Service.IService;
using BlazorAppIdolJav.Share.ClassData;
using BlazorAppIdolJav.Share.ClassDB;

namespace BlazorAppIdolJav.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        readonly IMapper _mapper;

        public UserService(IUserRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<UserData>> GetAllWithFilterAsync(UserSearch search)
        {
            try
            {
                var filter = search.CreateFilter(_repo.GetQueryable());
                var result = await _repo.GetAllWithFilterAsync(filter, search);
                var data = _mapper.Map<List<UserData>>(result);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RegisterAccountAsync(UserData data)
        {
            try
            {
                var user = _mapper.Map<User>(data);
                var result = await _repo.RegisterAccountAsync(user);
                return result;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CheckExistUserInfoAsync(UserData data)
        {
            try
            {
                var user = _mapper.Map<User>(data);
                var result = await _repo.CheckExistUserInfoAsync(user);
                return result;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
