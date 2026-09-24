namespace GameManagement.Share.ClassDB
{
	public class UserGameReviewReply
	{
		public string Id { get; set; }
		public string UserId { get; set; } // người phản hồi
		public string ReviewId { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string ReplyContent { get; set; }
		public bool IsAnonymous { get; set; } // phản hồi ản danh or công khai
		public bool IsHidden { get; set; } //admin có thể ẩn or ko
		public bool IsDeleted { get; set; } //đánh dấu xóa khi ng dùng xóa chứ ko del khỏi db
		public string? ParentId { get; set; }
	}
}
