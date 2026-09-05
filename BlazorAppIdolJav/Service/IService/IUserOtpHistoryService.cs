using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;

namespace GameManagement.Service.IService
{
    public interface IUserOtpHistoryService
    {
        Task<bool> AddOtpHistoryAsync(UserOtpHistoryData data);
        Task<bool> UpdateOtpHistoryAsync(UserOtpHistoryData data);

        Task<UserOtpHistoryData> GetLatestOtpAsync(string email, string otpType);
    }
}