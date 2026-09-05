using AutoMapper;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;

namespace GameManagement.Service
{
    public class UserOtpHistoryService : IUserOtpHistoryService

    {
        private readonly IUserOtpHistoryRepository _repo;
        readonly IMapper _mapper;

        public UserOtpHistoryService(IUserOtpHistoryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<bool> AddOtpHistoryAsync(UserOtpHistoryData data)
        {
            try
            {
                var history = _mapper.Map<UserOtpHistory>(data);
                var isSuccess = await _repo.AddOtpHistoryAsync(history);
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateOtpHistoryAsync(UserOtpHistoryData data)
        {
            try
            {
                var history = _mapper.Map<UserOtpHistory>(data);
                var isSuccess = await _repo.UpdateOtpHistoryAsync(history);
                return isSuccess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserOtpHistoryData> GetLatestOtpAsync(
            string email,
            string otpType)
        {
            var data = await _repo.GetLatestOtpAsync(email, otpType);
            var history = _mapper.Map<UserOtpHistoryData>(data);
            return history;
          
        }
    }
}
