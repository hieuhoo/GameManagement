using AutoMapper;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;

namespace GameManagement.Service
{
    public class UserPasswordHistoryService : IUserPasswordHistoryService

    {
        private readonly IUserPasswordHistoryRepository _repo;
        readonly IMapper _mapper;

        public UserPasswordHistoryService(IUserPasswordHistoryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<bool> AddPasswordHistoryAsync(UserPasswordHistoryData data)
        {
            try
            {
                var history = _mapper.Map<UserPasswordHistory>(data);
                var isSuccess = await _repo.AddPasswordHistoryAsync(history);
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CheckPasswordRecentlyUsedAsync(string newPassword, string userId, int time)
        {
            try
            {
                var result = await _repo.CheckPasswordRecentlyUsedAsync(newPassword, userId, time);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
