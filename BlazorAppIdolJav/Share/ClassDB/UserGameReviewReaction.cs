namespace GameManagement.Share.ClassDB
{
	public class UserGameReviewReaction
	{
		public string Id { get; set; }
		public string UserId { get; set; } // người thả reaction
		public string ReviewId { get; set; }
		public DateTime CreateDate { get; set; }
		public string ReactionType { get; set; } // tym hoặc like , có thể sau này là share
		public string? ReplyId { get; set; }
	}
}
