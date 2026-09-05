using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
    public interface IUserPasswordHistoryRepository
    {
        Task<bool> AddPasswordHistoryAsync(UserPasswordHistory data);
        Task<bool> CheckPasswordRecentlyUsedAsync(string newPassword, string userId, int time);
    }
}
