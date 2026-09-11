using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
    public interface IGameRedeemCodeService
    {
        Task<List<GameRedeemCodeData>> GetAllWithFilterAsync(GameRedeemSearch search);
        Task<bool> SaveRedeemAsync(GameRedeemCodeData data);
        Task<bool> UpdateRedeemAsync(GameRedeemCodeData data);
        Task<bool> DeleteRedeemAsync(GameRedeemCodeData data);
        //cần hàm add list
        Task<bool> SaveListRedeemAsync(List<GameRedeemCodeData> data);
        Task<bool> IsRedeemCodeExistsAsync(string code);

    }

    [DataContract]
    public class GameRedeemSearch
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public virtual string Status { get; set; }
        [DataMember(Order = 3)]
        public virtual string BatchId { get; set; }

        public IQueryable<GameRedeemCode> CreateFilter(IQueryable<GameRedeemCode> filter)
        {
            if (Id.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Id == Id);
            }
            if (Status.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Status == Status);
            }
            if (BatchId.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.BatchId == BatchId);
            }
            return filter;
        }
    }
}
