
using GameManagement.Components.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GameManagement.Components.Pages
{
	public partial class Home : ComponentBase
	{
		[Inject]
		public NavigationManager NavigationManager { get; set; } = default!;

		[Inject]
		public AuthenticationStateProvider AuthProvider { get; set; } = default!;

		[CascadingParameter]
		public MainLayout MainLayout { get; set; } = default!;
		string? redirectAfterLogin;

		async Task HandleBrowseGame()
		{
			try
			{
				var authState = await AuthProvider.GetAuthenticationStateAsync();

				if (authState.User.Identity?.IsAuthenticated == true)
				{
					NavigationManager.NavigateTo("/danh-sach-game");
					return;
				}

				await MainLayout.ShowLoginModalAsync("/danh-sach-game");
			}
			catch
			{

			}
		}

		public List<(string Name,
			 string CategoryName,
			 string CompanyName,
			 string Status,
			 string ImageUrl)> FeaturedGames
		{ get; set; } = new()
{
	(
		"Cyber Adventure",
		"Action • Adventure",
		"Game Studio",
		"Active",
		"/image/game1.jpg"
	),
	(
		"Dark Kingdom",
		"RPG • Fantasy",
		"Moon Studio",
		"Active",
		"/image/game2.jpg"
	),
	(
		"GOW:Ragnarok",
		"Action • Singleplayer",
		"SOny Interactive",
		"Active",
		"/image/game3.jpg"
	)
};
	}
}
