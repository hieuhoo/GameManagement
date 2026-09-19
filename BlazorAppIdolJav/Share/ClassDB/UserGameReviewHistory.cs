namespace GameManagement.Share.ClassDB
{
	public class UserGameReviewHistory
	{
		public string Id { get; set; }
		public string UserId { get; set; } // người bình loạn
		public string GameId { get; set; } //game tương ứng
		public DateTime CreateDate { get; set; }
		public string ReviewId { get; set; }  // tham chiếu tới bình luận nào
		public string? PrevComment { get; set; }
		public string CurrentComment { get; set; }
		public decimal? PrevStarNumber { get; set; } // sao sau
		public decimal CurrentStarNumber { get; set; } // sao trước
		public bool? PrevIsRecommend { get; set; }
		public bool? CurrentIsRecommend { get; set; }
	}
}
