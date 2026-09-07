using System.ComponentModel.DataAnnotations;

namespace GameManagement.Share.Model.ViewModel
{
	public class GameViewModel
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public int Stt { get; set; }
		public string Status { get; set; }
		public string SoldStatus { get; set; }
		public string ReleaseStatus { get; set; }
		public string PriceUnit { get; set; }
		public bool IsFeatured { get; set; }
		public int? OrderFeatured { get; set; }
	}
}
