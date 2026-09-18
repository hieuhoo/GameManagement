namespace GameManagement.Share.ClassDB
{
	public class UserGameReview
	{
		public string Id { get; set; }
		public string UserId { get; set; }
		public string GameId { get; set; }
		public DateTime CreateDate { get; set; }
		public int StarNumber { get; set; }
		public string Comment { get; set; }
		public bool IsPositive { get; set; }
		public DateTime UpdatedDate { get; set; }
	}
}
