using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.Share.ClassDB
{
	public class Game
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Country { get; set; }
		public string? ImageName { get; set; }
		public DateTime ReleaseDate { get; set; }
		public DateTime CreateDate { get; set; }
		public string Status { get; set; }
		public string SoldStatus { get; set; }
		public string ReleaseStatus { get; set; }
		public int Price { get; set; }
		public string Unit { get; set; }
		public string Description { get; set; }
		public string SystemSupport { get; set; }

		public string GameCompanyId { get; set; }
		public string? ImagePath { get; set; }
		public string GameTypeId { get; set; }
		public bool IsFeatured { get; set; }
		public int? OrderFeatured { get; set; }

		public string? MinOS { get; set; } //win 10  -11-12
		public string? MinCPU { get; set; }
		public int? MinRAM { get; set; } // option
		public string? MinGPU { get; set; }

		public string? RecommendOS { get; set; }
		public string? RecommendCPU { get; set; }
		public int? RecommendRAM { get; set; }
		public string? RecommendGPU { get; set; }

		public string? DirectX { get; set; }
		public int? StorageRequired { get; set; } // GB
		public string? SystemNote { get; set; } //ghi chú

	}
}
