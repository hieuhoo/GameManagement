using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
	public interface IUserGameReviewHistoryService
	{
		Task<List<UserGameReviewHistoryData>> GetAllWithFilterAsync(ReviewHistorySearch search);
	}

	[DataContract]
	public class ReviewHistorySearch
	{
		[DataMember(Order = 1)]
		public string Id { get; set; }

		[DataMember(Order = 2)]
		public virtual string UserId { get; set; }
		[DataMember(Order = 3)]
		public virtual string GameId { get; set; }
		[DataMember(Order = 4)]
		public virtual string ReviewId { get; set; }

		public IQueryable<UserGameReviewHistory> CreateFilter(IQueryable<UserGameReviewHistory> filter)
		{
			if (Id.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.Id == Id);
			}
			if (UserId.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.UserId == UserId);
			}
			if (GameId.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.GameId == GameId);
			}
			if (ReviewId.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.ReviewId == ReviewId);
			}
			return filter;
		}
	}
}
