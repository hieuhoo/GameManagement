namespace GameManagement.Share.ClassDB
{
    public class GameDiscount
    {
        public string Id { get; set; }
        public string GameId { get; set; }
        public int DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }
}
