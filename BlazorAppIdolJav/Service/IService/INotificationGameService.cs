using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
	public interface INotificationGameService
	{
		Task<bool> AddNotificationAsync(UserNotificationData data);
		Task<bool> UpdateNotificationAsync(UserNotificationData data); // update isread với tgian đọc
		Task<List<UserNotificationData>> GetAllWithFilterAsync(NotiSearch search);

	}

	[DataContract]
	public class NotiSearch
	{
		[DataMember(Order = 1)]
		public string Id { get; set; }

		[DataMember(Order = 2)]
		public virtual string ActorUserId { get; set; }
		[DataMember(Order = 3)]
		public virtual string ReceiveUserId { get; set; }
		[DataMember(Order = 4)]
		public virtual string Type { get; set; }

		public IQueryable<UserNotification> CreateFilter(IQueryable<UserNotification> filter)
		{
			if (Id.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.Id == Id);
			}
			if (ActorUserId.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.ActorUserId == ActorUserId);
			}
			if (ReceiveUserId.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.ReceiveUserId == ReceiveUserId);
			}
			if (Type.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.Type == Type);
			}
			return filter;
		}
	}
}
