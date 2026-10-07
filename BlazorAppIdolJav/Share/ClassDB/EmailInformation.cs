namespace GameManagement.Share.ClassDB
{
	public class EmailInformation
	{
		public string Id { get; set; }
		public string Content { get; set; }
		public string Subject { get; set; } // tiêu đề
		public string? Status { get; set; }
		public string MailSend { get; set; }
		public string MailReceipt { get; set; }
		public string? MailType { get; set; }
		public string? UserSendId { get; set; }
		public string? UserReceiptId { get; set; }

		public DateTime? CreateDate { get; set; }
	}
}
