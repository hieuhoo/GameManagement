using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
    public interface IUserOtpHistoryRepository
    {
        IQueryable<UserOtpHistory> GetQueryable();
        Task<bool> AddOtpHistoryAsync(UserOtpHistory data);
        Task<bool> UpdateOtpHistoryAsync(UserOtpHistory data);

        Task<UserOtpHistory> GetLatestOtpAsync(string email, string otpType);

    }
}
