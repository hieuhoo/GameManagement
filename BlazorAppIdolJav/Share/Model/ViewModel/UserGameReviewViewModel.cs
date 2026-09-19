using System.ComponentModel.DataAnnotations;

namespace GameManagement.Share.Model.ViewModel
{
	public class UserGameReviewViewModel
	{
		public string Id { get; set; }

		public decimal StarNumber { get; set; }
		public string Comment { get; set; }
		public string UserId { get; set; }
		public string GameId { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdatedDate { get; set; }

		public bool IsRecommend { get; set; }

		public bool IsHidden { get; set; }
		public bool IsAnonymous { get; set; }

		public int LikeCount { get; set; } // tổng số like comment
		public int HeartCount { get; set; } // tổng số tym comment
		public string UserName { get; set; }
		public string MyReaction { get; set; }
	}
}
