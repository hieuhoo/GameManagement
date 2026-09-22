using System.Runtime.Serialization;

namespace GameManagement.Share.ClassData
{
	public class UserGameReviewHistoryData
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
		public virtual String GameId
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
		public virtual string ReviewId
		{
			get;
			set;
		}
		[DataMember(Order = 6)]
		public virtual string PrevComment
		{
			get;
			set;
		}
		[DataMember(Order = 7)]
		public virtual string CurrentComment
		{
			get;
			set;
		}
		[DataMember(Order = 8)]
		public virtual decimal PrevStarNumber
		{
			get;
			set;
		}
		[DataMember(Order = 9)]
		public virtual decimal CurrentStarNumber
		{
			get;
			set;
		}
		[DataMember(Order = 10)]
		public virtual int PrevIsRecommend
		{
			get;
			set;
		}
		[DataMember(Order = 11)]
		public virtual int CurrentIsRecommend
		{
			get;
			set;
		}

		[DataMember(Order = 12)]
		public virtual string? UserName
		{
			get;
			set;
		}
	}
}
