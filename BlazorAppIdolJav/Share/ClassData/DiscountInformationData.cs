using System.Runtime.Serialization;

namespace GameManagement.Share.ClassData
{
	public class DiscountInformationData
	{
		[DataMember(Order = 1)]
		public virtual int Percent
		{
			get;
			set;
		}
		[DataMember(Order = 2)]
		public virtual DateTime StartDate
		{
			get;
			set;
		}
		[DataMember(Order = 3)]
		public virtual DateTime EndDate
		{
			get;
			set;
		}
	}
}
