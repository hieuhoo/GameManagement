using GameManagement.Share.ClassData;

namespace GameManagement.Service.IService
{
    public interface IUserPasswordHistoryService
    {
        Task<bool> AddPasswordHistoryAsync(UserPasswordHistoryData data);
        Task<bool> CheckPasswordRecentlyUsedAsync(string newPassword, string userId, int time);
    }
}
