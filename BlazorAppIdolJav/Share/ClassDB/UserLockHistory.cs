namespace GameManagement.Share.ClassDB
{
	public class UserLockHistory
	{
		public string Id { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Reason { get; set; } 
        public string PerformedBy { get; set; }
	}
}
