using System.Runtime.Serialization;

namespace GameManagement.Share.ClassData
{
	public class UserGameReviewReactionData
	{
		[DataMember(Order = 1)]
		public virtual String Id
		{
			get;
			set;
		}
		[DataMember(Order = 2)]
		public virtual String UserId
		{
			get;
			set;
		}
		[DataMember(Order = 3)]
		public virtual String ReviewId
		{
			get;
			set;
		}
		[DataMember(Order = 4)]
		public virtual DateTime CreateDate
		{
			get;
			set;
		}
		[DataMember(Order = 5)]
		public virtual string ReactionType
		{
			get;
			set;
		}
		[DataMember(Order = 6)]
		public virtual string? ReplyId
		{
			get;
			set;
		}
	}
}
