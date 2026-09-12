namespace GameManagement.Share.ClassDB
{
	public class UserWallet
	{
		public string Id { get; set; }
		public string UserId { get; set; }
		public decimal Balance { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime? UpdatedDate { get; set; }

	}
}
