using System.Runtime.Serialization;

namespace GameManagement.Share.ClassData
{
	public class EmailInformationData
	{
		[DataMember(Order = 1)]
		public virtual String Id
		{
			get;
			set;
		}
		[DataMember(Order = 2)]
		public virtual String Subject
		{
			get;
			set;
		}
		[DataMember(Order = 3)]
		public virtual String Content
		{
			get;
			set;
		}
		[DataMember(Order = 4)]
		public virtual String? Status
		{
			get;
			set;
		}
		[DataMember(Order = 5)]
		public virtual String MailSend
		{
			get;
			set;
		}
		[DataMember(Order = 6)]
		public virtual String MailReceipt
		{
			get;
			set;
		}
		[DataMember(Order = 7)]
		public virtual String? MailType
		{
			get;
			set;
		}
		[DataMember(Order = 8)]
		public virtual String? UserSendId
		{
			get;
			set;
		}

        [DataMember(Order = 9)]
		public virtual DateTime? CreateDate
		{
			get;
			set;
		}
		[DataMember(Order = 10)]
		public virtual String? UserReceiptId
		{
			get;
			set;
		}
	}
}
