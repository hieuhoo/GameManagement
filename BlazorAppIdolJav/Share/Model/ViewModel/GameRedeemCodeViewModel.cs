namespace GameManagement.Share.Model.ViewModel
{
	public class GameRedeemCodeViewModel
	{
		public string Id { get; set; }
		public string? GameId { get; set; }
		public int Stt { get; set; }
		public string Status { get; set; }
		public string GameName { get; set; }
		public DateTime ExpiredDate { get; set; }
		public string? Code { get; set; }
		public string RedeemType { get; set; }
		public int? Value { get; set; }
	}
}
