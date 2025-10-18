using BlazorAppIdolJav.Service.IService;
using BlazorAppIdolJav.Share.ClassDB;

namespace BlazorAppIdolJav.Repository.IRepository
{
    public interface IActressRepository
    {
        IQueryable<Actress> GetQueryable();
        Task<List<Actress>> GetAllWithFilterAsync(IQueryable<Actress>  query , ActressSearch search);
    }
}
