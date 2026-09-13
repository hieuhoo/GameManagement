namespace GameManagement.Share.ClassData
{
	public class RedeemTransactionResultData
	{
		public bool IsSuccess { get; set; }
		public decimal BalanceAfter { get; set; }
        public decimal BalanceBefore { get; set; } // reload vẫn hiện gtri cũ  khi nhập code báo ko hợp lệ
        public int Amount { get; set; }
		public string Message {get; set;}
	}
}
