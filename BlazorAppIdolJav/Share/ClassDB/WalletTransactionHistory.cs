namespace GameManagement.Share.ClassDB
{
	public class WalletTransactionHistory
	{
		public string Id { get; set; }
		public string UserId { get; set; }
		public decimal Amount { get; set; }
		public DateTime CreateDate { get; set; }
		public string ReferenceId { get; set; } //mã code thiết lập hoặc mã gdich hoặc mã game
		public decimal BalanceBefore { get; set; } // sau = trước + amount
		public decimal BalanceAfter { get; set; }
		public string Type { get; set; } // loại giao dịch : là dùng voucher hoặc ấn nút nạp tiền hoặc mua game
		public string? RedeemType { get; set; } // loại mã redeem :money hoặc game , hiện tại case redeem game khá phức tạp

		public int? PercentDiscount { get; set; } // phần trăm giảm giá game lúc mua (nếu có ?? 0)	
		public int? OriginalGamePrice { get; set; } //giá gốc (nếu là giao dịch mua game)
		public int? PurchaseGamePrice { get; set; } // giá khi mua (nếu là giao dịch mua game)
	}
}
