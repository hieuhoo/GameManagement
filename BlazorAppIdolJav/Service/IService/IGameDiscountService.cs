using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
    public interface IGameDiscountService
    {
        Task<List<GameDiscountData>> GetAllWithFilterAsync(DiscountSearch search);
        Task<bool> SaveDiscountAsync(GameDiscountData data);
        Task<bool> UpdateDiscountAsync(GameDiscountData data);
        Task<bool> DeleteDiscountAsync(GameDiscountData data);
        Task<bool> CheckDiscountOverlapTimeAsync(string id, DateTime? start, DateTime? end);
    }

    [DataContract]
    public class DiscountSearch
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public virtual string GameId { get; set; }
        [DataMember(Order = 3)]
        public virtual string Status { get; set; }
        [DataMember(Order = 4)]
        public virtual DateTime StartDate { get; set; }
        [DataMember(Order = 5)]
        public virtual DateTime EndDate { get; set; }

        public IQueryable<GameDiscount> CreateFilter(IQueryable<GameDiscount> filter)
        {
            if (Id.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Id == Id);
            }
            if (GameId.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.GameId == GameId);
            }
            if (Status.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Status == Status);
            }
            //if (StartDate.)
            //{
            //    filter = filter.Where(x => x.SoldStatus == SoldStatus);
            //}
            //if (ReleaseStatus.IsNotNullOrEmpty())
            //{
            //    filter = filter.Where(x => x.ReleaseStatus == ReleaseStatus);
            //}
            return filter;
        }
    }
}
