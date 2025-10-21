using BlazorAppIdolJav.Service.IService;
using BlazorAppIdolJav.Share.ClassDB;

namespace BlazorAppIdolJav.Repository.IRepository
{
    public interface IUserRepository
    {
        IQueryable<User> GetQueryable();
        Task<List<User>> GetAllWithFilterAsync(IQueryable<User>  query , UserSearch search);
        Task<bool> RegisterAccountAsync(User data);
        Task<bool> CheckExistUserInfoAsync(User data);

    }
}
