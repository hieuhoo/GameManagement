namespace GameManagement.Share.ClassDB
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public DateTime CreateDate { get; set; }
        public int? QuantityLoginCount { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string Role { get; set; }
        public DateTime? DateOfBirth { get; set; }


    }
}
