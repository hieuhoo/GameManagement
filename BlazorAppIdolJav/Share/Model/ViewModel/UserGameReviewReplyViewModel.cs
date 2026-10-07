using GameManagement.Share.ClassData;
using System.ComponentModel.DataAnnotations;

namespace GameManagement.Share.Model.ViewModel
{
	public class UserGameReviewReplyViewModel
	{
		public string Id { get; set; }

		public int Stt { get; set; }
		public string ReviewId { get; set; }
		public DateTime CreateDate { get; set; }
		public string ReplyContent { get; set; }
		public string ParentId { get; set; }
		public string UserName { get; set; }
		public string UserId { get; set; }

	}
}
