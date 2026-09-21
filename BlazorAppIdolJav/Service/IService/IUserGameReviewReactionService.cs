using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
	public interface IUserGameReviewReactionService
	{
		Task<bool> ChangeReactionAsync(string reviewId, string userId, string type);
		Task<string> CheckDisplayMyReactionAsync(string userId, string reviewId);
		Task<List<ReactionPersonInfoData>> GetListUsersReactAsync(string reviewId);
		Task<List<ReactionPersonInfoData>> GetAllWithFilterAsync(PersonReactionSearch search);

	}

	[DataContract]
	public class PersonReactionSearch
	{
		[DataMember(Order = 1)]
		public string Id { get; set; }

		[DataMember(Order = 2)]
		public virtual string UserId { get; set; }
		[DataMember(Order = 3)]
		public virtual string ReactionType { get; set; }
		[DataMember(Order = 4)]
		public virtual string ReviewId { get; set; }

		public IQueryable<UserGameReviewReaction> CreateFilter(IQueryable<UserGameReviewReaction> filter)
		{
			if (Id.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.Id == Id);
			}
			if (UserId.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.UserId == UserId);
			}
			if (ReactionType.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.ReactionType == ReactionType);
			}
			if (ReviewId.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.ReviewId == ReviewId);
			}
			return filter;
		}
	}
}
