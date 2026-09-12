using System.Runtime.Serialization;

namespace GameManagement.Share.ClassData
{
	public class UserWalletData
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
		public virtual decimal Balance
		{
			get;
			set;
		}
		[DataMember(Order = 4)]
		public virtual DateTime? UpdatedDate
		{
			get;
			set;
		}
		[DataMember(Order = 5)]
		public virtual DateTime CreateDate
		{
			get;
			set;
		}
	}
}
