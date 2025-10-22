using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
    public interface IGameService
    {
        Task<List<GameData>> GetAllWithFilterAsync(GameSearch search);
    }

    [DataContract]
    public class GameSearch
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public virtual string GameCompanyId { get; set; }
        [DataMember(Order = 3)]
        public virtual string Country { get; set; }
        [DataMember(Order = 4)]
        public virtual string Name { get; set; }

        public IQueryable<Game> CreateFilter(IQueryable<Game> filter)
        {
            if (Id.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Id == Id);
            }
            if (GameCompanyId.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.GameCompanyId == GameCompanyId);
            }
            if (Country.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Country == Country);
            }
            return filter;
        }
    }
}
