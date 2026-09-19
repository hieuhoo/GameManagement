using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
	public interface IUserGameReviewService
	{
		Task<bool> CreateReviewAsync(UserGameReviewData data);
		Task<bool> UpdateReviewAsync(UserGameReviewData data);
		Task<List<UserGameReviewData>> GetAllWithFilterAsync(ReviewSearch search);
		Task<StatisticReviewGameData> GetStatisticAboutGameAsync(string gameId);

	}

	[DataContract]
	public class ReviewSearch
	{
		[DataMember(Order = 1)]
		public string Id { get; set; }

		[DataMember(Order = 2)]
		public virtual string UserId { get; set; }
		[DataMember(Order = 3)]
		public virtual string GameId { get; set; }
		[DataMember(Order = 4)]
		public virtual bool? IsPositiveComment { get; set; }
		//[DataMember(Order = 3)]
		//public virtual string GameId { get; set; }

		public IQueryable<UserGameReview> CreateFilter(IQueryable<UserGameReview> filter)
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
			if (IsPositiveComment != null && IsPositiveComment == true)
			{
				filter = filter.Where(x => x.StarNumber >= 3);
			}
			if (IsPositiveComment != null && IsPositiveComment == false)
			{
				filter = filter.Where(x => x.StarNumber < 3);
			}
			return filter;
		}
	}
}
