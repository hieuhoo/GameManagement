using System.Runtime.Serialization;

namespace GameManagement.Share.ClassData
{
	public class UserGameReviewData
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
		public virtual int StarNumber
		{
			get;
			set;
		}
		[DataMember(Order = 6)]
		public virtual string Comment
		{
			get;
			set;
		}
		[DataMember(Order = 7)]
		public virtual DateTime UpdatedDate
		{
			get;
			set;
		}
		[DataMember(Order = 8)]
		public virtual bool IsPositive
		{
			get;
			set;
		}
	}
}
