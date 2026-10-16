using GameManagement.Share.ClassData;
using System.ComponentModel.DataAnnotations;

namespace GameManagement.Share.Model.ViewModel
{
	public class EmailInformationViewModel
	{
		public string Id { get; set; }

		public string MailSend { get; set; }
		public string MailReceipt { get; set; }
		public string Content { get; set; }
		public DateTime CreateDate { get; set; }

		public string Subject { get; set; }

		public string MailType { get; set; }
		public string Status { get; set; }
		public string UserSendId { get; set; }
		public string UserReceiptId { get; set; }
		public int Stt { get; set; }
	}
}
