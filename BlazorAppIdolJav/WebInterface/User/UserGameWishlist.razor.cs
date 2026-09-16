using GameManagement.Share.Model.ViewModel;
using Microsoft.AspNetCore.Components;

namespace GameManagement.WebInterface.User
{
	public partial class UserGameWishlist : ComponentBase
	{
		[Inject] NavigationManager NavManager { get; set; }
		public List<GameViewModel> ViewModels { get; set; } = new();

		protected override async Task OnInitializedAsync()
		{
			LoadMockWishlist();

			await base.OnInitializedAsync();
		}

		private void LoadMockWishlist()
		{
			ViewModels = new List<GameViewModel>
	{
		new()
		{
			Id = "1",
			Name = "God of War Ragnarök",
			ImagePathView = "Upload/GameURL/952fda00bf7742eaa8002d5006278a15.jpg",
			GameCompanyName = "Santa Monica Studio",
			ReleaseDate = new DateTime(2022, 11, 9),
			Price = 850000,
			CurrentPrice = 595000.ToString(),
			CurrentSalePercent = 30,
			DateAddedWishlist = new DateTime(2022, 11, 9),
		},

		new()
		{
			Id = "2",
			Name = "Black Myth: Wukong",
			ImagePathView = "Upload/GameURL/78897aada6e04eecbe2b5a6a1e13cfb1.jpg",
			GameCompanyName = "Game Science",
			ReleaseDate = new DateTime(2024, 8, 20),
			Price = 1290000,
			CurrentPrice = 1290000.ToString(),
			CurrentSalePercent = 0,
			DateAddedWishlist = new DateTime(2022, 11, 9),

		},

		new()
		{
			Id = "3",
			Name = "Uncharted 4: A Thief's End",
			ImagePathView = "Upload/GameURL/952fda00bf7742eaa8002d5006278a15.jpg",
			GameCompanyName = "Naughty Dog",
			ReleaseDate = new DateTime(2016, 5, 10),
			Price = 1000000,
			CurrentPrice = 700000.ToString(),
			CurrentSalePercent = 30,
			DateAddedWishlist = new DateTime(2022, 11, 9),
		},

		new()
		{
			Id = "4",
			Name = "Red Dead Redemption 2",
			ImagePathView = "Upload/GameURL/78897aada6e04eecbe2b5a6a1e13cfb1.jpg",
			GameCompanyName = "Rockstar Games",
			ReleaseDate = new DateTime(2018, 10, 26),
			Price = 1500000,
			CurrentPrice = 1125000.ToString(),
			CurrentSalePercent = 25,
			DateAddedWishlist = new DateTime(2022, 11, 9),

		}
	};
		}
	}
}
