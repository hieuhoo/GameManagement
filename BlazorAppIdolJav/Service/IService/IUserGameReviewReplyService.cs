using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
	public interface IUserGameReviewReplyService
	{
		Task<List<UserGameReviewReplyData>> GetAllWithFilterAsync(ReviewReplySearch search);
		Task<bool> AddReplyForCommentAsync(UserGameReviewReplyData data);
		Task<bool> UpdateReplyAsync(UserGameReviewReplyData data);
	}

	[DataContract]
	public class ReviewReplySearch
	{
		[DataMember(Order = 1)]
		public string Id { get; set; }

		[DataMember(Order = 2)]
		public virtual string UserId { get; set; }
		[DataMember(Order = 3)]
		public virtual string ReviewId { get; set; }
		[DataMember(Order = 4)]
		public virtual bool IsDeleted { get; set; }
		[DataMember(Order = 5)]
		public virtual string? ParentId { get; set; }
		[DataMember(Order = 6)]
		public virtual bool IsShowWithoutChild { get; set; }
		public IQueryable<UserGameReviewReply> CreateFilter(IQueryable<UserGameReviewReply> filter)
		{
			if (Id.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.Id == Id);
			}
			if (UserId.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.UserId == UserId);
			}
			if (ReviewId.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.ReviewId == ReviewId);
			}
			if (ParentId != null)
			{
				filter = filter.Where(x => x.ParentId == ParentId);
			}
			if (IsShowWithoutChild == true)
			{
				filter = filter.Where(x => x.ParentId == null);
			}
			//else
			//{
			//	filter = filter.Where(x => (x.ParentId == null || x.ParentId == ParentId));
			//}
			if (IsDeleted == false)
			{
				filter = filter.Where(x => x.IsDeleted == false);
			}
			//else
			//{
			//	filter = filter.Where(x => (x.IsDeleted == false || x.IsDeleted == true));
			//}
			//else
			//{
			//	filter = filter.Where(x => x.ParentId == null);
			//}
			return filter;
		}
	}
}
