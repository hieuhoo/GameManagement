namespace GameManagement.Share.ClassData
{
	public class RedeemTransactionResultData
	{
		public bool IsSuccess { get; set; }
		public decimal BalanceAfter { get; set; }
		public int Amount { get; set; }
		public string Message {get; set;}
	}
}
