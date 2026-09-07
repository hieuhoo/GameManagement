namespace GameManagement.Share.Model.ViewModel
{
    public class GameDiscountViewModel
    {
        public string Id { get; set; }
        public string GameId { get; set; }
        public int DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Stt { get; set; }
        public string GameName { get; set; }

    }
}
