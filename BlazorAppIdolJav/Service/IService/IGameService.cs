using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
    public interface IGameService
    {
        Task<List<GameData>> GetAllWithFilterAsync(GameSearch search);
        Task<bool> SaveGameAsync(GameData data);
        Task<bool> UpdateGameAsync(GameData data);
        Task<bool> DeleteGameAsync(GameData data);

    }

    [DataContract]
    public class GameSearch
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public virtual string GameCompanyId { get; set; }
        [DataMember(Order = 3)]
        public virtual string Status { get; set; }
        [DataMember(Order = 4)]
        public virtual string Name { get; set; }
        [DataMember(Order = 5)]
        public virtual string SoldStatus { get; set; }
        [DataMember(Order = 6)]
        public virtual string ReleaseStatus { get; set; }

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
            if (Status.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Status == Status);
            }
            if (SoldStatus.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.SoldStatus == SoldStatus);
            }
            if (ReleaseStatus.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.ReleaseStatus == ReleaseStatus);
            }
            return filter;
        }
    }
}
