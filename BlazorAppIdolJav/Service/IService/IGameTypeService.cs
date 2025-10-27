using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
    public interface IGameTypeService
    {
        Task<bool> SaveGameTypeAsync(GameTypeData data);
        Task<bool> UpdateGameTypeAsync(GameTypeData data);
        Task<bool> DeleteGameTypeAsync(GameTypeData data);

        Task<List<GameTypeData>> GetAllWithFilterAsync(GameTypeSearch search);
    }

    [DataContract]
    public class GameTypeSearch
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public virtual string Name { get; set; }
        [DataMember(Order = 3)]
        public virtual string EnglishName { get; set; }
        [DataMember(Order = 4)]
        public virtual string Status { get; set; }

        public IQueryable<GameType> CreateFilter(IQueryable<GameType> filter)
        {
            if (Id.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Id == Id);
            }
            if (Name.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Name == Name);
            }
            if (EnglishName.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.EnglishName == EnglishName);
            }
            if (Status.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Status == Status);
            }
            return filter;
        }
    }
}
