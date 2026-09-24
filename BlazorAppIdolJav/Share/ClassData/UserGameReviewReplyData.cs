using System.Runtime.Serialization;

namespace GameManagement.Share.ClassData
{
	public class UserGameReviewReplyData
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
		public virtual DateTime UpdatedDate
		{
			get;
			set;
		}
		[DataMember(Order = 6)]
		public virtual string ReplyContent
		{
			get;
			set;
		}
		[DataMember(Order = 7)]
		public virtual bool IsAnonymous
		{
			get;
			set;
		}
		[DataMember(Order = 8)]
		public virtual bool IsHidden
		{
			get;
			set;
		}
		[DataMember(Order = 9)]
		public virtual bool IsDeleted
		{
			get;
			set;
		}
		[DataMember(Order = 10)]
		public virtual String UserName
		{
			get;
			set;
		}
		[DataMember(Order = 11)]
		public virtual int LikeCount
		{
			get;
			set;
		}
		[DataMember(Order = 12)]
		public virtual string? ParentId
		{
			get;
			set;
		}
		[DataMember(Order = 13)]
		public List<UserGameReviewReplyData> Children { get; set; } = new();

		[DataMember(Order = 14)]
		public bool IsLikedByMe
		{
			get;
			set;
		}
		//cái này sẽ chỉ có 1 loại reaction là like, ko hỗ trợ cái khác
	}
}
