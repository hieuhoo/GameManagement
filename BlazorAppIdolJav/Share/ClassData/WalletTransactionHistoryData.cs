using System.Runtime.Serialization;

namespace GameManagement.Share.ClassData
{
	public class WalletTransactionHistoryData
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
		public virtual decimal Amount
		{
			get;
			set;
		}
		[DataMember(Order = 4)]
		public virtual string ReferenceId
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
		[DataMember(Order = 6)]
		public virtual string Type
		{
			get;
			set;
		}
		[DataMember(Order = 7)]
		public virtual decimal BalanceBefore
		{
			get;
			set;
		}
		[DataMember(Order = 8)]
		public virtual decimal BalanceAfter
		{
			get;
			set;
		}
	}
}
