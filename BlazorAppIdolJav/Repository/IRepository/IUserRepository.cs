using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
    public interface IUserRepository
    {
        IQueryable<User> GetQueryable();
        Task<List<User>> GetAllWithFilterAsync(IQueryable<User>  query , UserSearch search);
        Task<bool> RegisterAccountAsync(User data);
        Task<bool> CheckExistUserInfoAsync(User data);
        Task<bool> CheckUserLoginAsync(User data);
        Task<User> GetUserInfoAsync(UserSearch search);

    }
}
