using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
    public interface IGameCompanyService
    {
        Task<bool> SaveCompanyAsync(GameCompanyData data);
        Task<bool> UpdateCompanyAsync(GameCompanyData data);
        Task<bool> DeleteCompanyAsync(GameCompanyData data);

        Task<List<GameCompanyData>> GetAllWithFilterAsync(GameCompanySearch search);

    }

    [DataContract]
    public class GameCompanySearch
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public virtual string Name { get; set; }
        [DataMember(Order = 3)]
        public virtual string Country { get; set; }
        [DataMember(Order = 4)]
        public virtual string Status { get; set; }

        public IQueryable<GameCompany> CreateFilter(IQueryable<GameCompany> filter)
        {
            if (Id.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Id == Id);
            }
            if (Name.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Name == Name);
            }
            if (Country.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Country == Country);
            }
            return filter;
        }
    }
}
