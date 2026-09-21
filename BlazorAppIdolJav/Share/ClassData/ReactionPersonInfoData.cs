namespace GameManagement.Share.ClassData
{
    public class ReactionPersonInfoData
    {
        public string UserId { get; set; }
        public string? UserName { get; set; }
        public bool IsAnonymous { get; set; }
        public string ReactionType { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
