using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
    public interface IUserLockHistoryService
    {
        Task<bool> AddLockHistoryAsync(UserLockHistoryData data);
        Task<List<UserLockHistoryData>> GetAllWithFilterAsync(UserLockHistorySearch search);

    }

    [DataContract]
    public class UserLockHistorySearch
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public virtual string UserId { get; set; }
        [DataMember(Order = 3)]
        public virtual string Action { get; set; }
        [DataMember(Order = 4)]
        public virtual string Reason { get; set; }
        [DataMember(Order = 5)]
        public virtual string PerformedBy { get; set; }

        public IQueryable<UserLockHistory> CreateFilter(IQueryable<UserLockHistory> filter)
        {
            if (Id.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Id == Id);
            }
            if (UserId.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.UserId == UserId);
            }
            if (Action.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Action == Action);
            }
            if (Reason.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Reason == Reason);
            }
            if (PerformedBy.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.PerformedBy == PerformedBy);
            }
            return filter;
        }
    }
}
