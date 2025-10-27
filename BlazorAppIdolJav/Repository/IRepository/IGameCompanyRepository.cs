using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
    public interface IGameCompanyRepository
    {
        IQueryable<GameCompany> GetQueryable();
        Task<List<GameCompany>> GetAllWithFilterAsync(IQueryable<GameCompany> query, GameCompanySearch search);
        Task<bool> SaveCompanyAsync(GameCompany data);
        Task<bool> UpdateCompanyAsync(GameCompany data);
        Task<bool> DeleteCompanyAsync(GameCompany data);


    }
}
