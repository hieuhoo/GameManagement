namespace GameManagement.Share.Model.ViewModel
{
	public class ReactionPersonInfoViewModel
	{
		public string UserId { get; set; }
		public string? UserName { get; set; }
		public bool IsAnonymous { get; set; }
		public DateTime CreateDate { get; set; }
		public int Stt { get; set; }
		public string ReactionType { get; set; }
	}
}
