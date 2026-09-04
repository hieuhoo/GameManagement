namespace GameManagement.Share.ClassDB
{
    public class UserOtpHistory
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string OtpType { get; set; }
        public DateTime CreateDate { get; set; }
        public string OtpCode { get; set; } 
        public string OtpCodeHash { get; set; }
        public DateTime ExpiredDate { get; set; }

        public string Email { get; set; }

        public bool IsUsed { get; set; }
    }
}
