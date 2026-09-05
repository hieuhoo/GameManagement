namespace GameManagement.Share.ClassDB
{
    public class UserPasswordHistory
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string PreviousPassword { get; set; }
        public string CurrentPasswordHash { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
