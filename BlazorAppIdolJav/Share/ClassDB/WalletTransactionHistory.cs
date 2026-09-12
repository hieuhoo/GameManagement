namespace GameManagement.Share.ClassDB
{
	public class WalletTransactionHistory
	{
		public string Id { get; set; }
		public string UserId { get; set; }
		public decimal Amount { get; set; }
		public DateTime CreateDate { get; set; }
		public string ReferenceId { get; set; } //mã code thiết lập hoặc mã gdich
		public decimal BalanceBefore { get; set; } // sau = trước + amount
		public decimal BalanceAfter { get; set; }
		public string Type { get; set; }
	}
}
