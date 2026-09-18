namespace GameManagement.Share.Model.ViewModel
{
	public class WalletTransactionHistoryViewModel
	{
		public string Id { get; set; }
		public DateTime CreateDate { get; set; }
		public int Stt { get; set; }
		public string ItemName { get; set; }
		public string Type { get; set; } // loại giao dịch: redeem nhập mã, mua game, nạp tiền
		public string RedeemType { get; set; }// loại redeem mã : game, money

		public string ReferenceId { get; set; }
		public decimal Amount { get; set; }
		public decimal BalanceAfter { get; set; }
		public int TotalMoney { get; set; }
		public string ChangeMoney { get; set; } // gán số thay đổi tiền = + hoặc - giá trị amounts
		public int PercentDiscount { get; set; }
		public int OriginalGamePrice { get; set; }
		public int PurchaseGamePrice { get; set; }
		public string UserId { get; set; }
		public string AccountName { get; set; }

	}
}
