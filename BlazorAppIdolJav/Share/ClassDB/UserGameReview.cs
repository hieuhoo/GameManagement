namespace GameManagement.Share.ClassDB
{
	public class UserGameReview
	{
		public string Id { get; set; }
		public string UserId { get; set; }
		public string GameId { get; set; }
		public DateTime CreateDate { get; set; }
		public decimal StarNumber { get; set; }
		public string Comment { get; set; }
		public bool IsHidden { get; set; } // ẩn hay ko do admin, mặc định tạo hidden =false;
		public bool? IsRecommend { get; set; }

		public DateTime UpdatedDate { get; set; }
		public int LikeCount { get; set; }
		public int HeartCount { get; set; }
		public bool IsAnonymous { get; set; }
        public int FunnyCount { get; set; }

    }
}
