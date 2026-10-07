namespace GameManagement.Share.ClassDB
{
	public class UserNotification
	{
		public string Id { get; set; }
		public string ReceiveUserId { get; set; }
		public string ActorUserId { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime? ReadAtTime { get; set; }
		public bool IsRead { get; set; }
		public string Type { get; set; } //loại noti thông báo
		public string TargetId { get; set; } // id cảu comment đc rep haowcj react
		public string GameId { get; set; }
	}
}
