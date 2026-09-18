using AntDesign;
using AutoMapper;
using GameManagement.Components.State;
using GameManagement.Service;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using GameManagement.Share.Model.ViewModel;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace GameManagement.WebInterface.User
{
	public partial class UserGameWishlist : ComponentBase
	{
		[Inject] NavigationManager NavManager { get; set; }
		[Inject] AuthenticationStateProvider AuthProvider { get; set; }
		[Inject] IUserGameWishlistService WishlistService { get; set; }
		[Inject] IGameService GameService { get; set; }
		[Inject] IGameCompanyService CompanyService { get; set; }
		[Inject] IGameTypeService TypeService { get; set; }
		[Inject] IGameDiscountService DiscountService { get; set; }


		[Inject] IMapper Mapper { get; set; }
		[Inject] NotificationService Notice { get; set; }
		[Inject] WishlistState State { get; set; }
		public List<GameViewModel> ViewModels { get; set; } = new();
		List<GameData> GameDatas { get; set; } = new();
		List<GameData> WishlistGameDatas { get; set; } = new();
		List<GameCompanyData> CompanyDatas { get; set; } = new();
		List<GameTypeData> TypeDatas { get; set; } = new();
		List<GameDiscountData> DiscountDatas { get; set; } = new();
		List<UserGameWishlistData> UserWishlistDatas { get; set; } = new();


		Dictionary<string, string> CompanyDict = new Dictionary<string, string>();
		Dictionary<string, GameDiscountData> DiscountDict = new Dictionary<string, GameDiscountData>();

		string currentUserId;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				var authState = await AuthProvider.GetAuthenticationStateAsync();
				var user = authState.User;
				currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				await GetAllGameAsync();
				await GetAllCompanyGameAsync();
				await GetAllTypeGameAsync();
				await GetDiscountAsync();
				await GetWishlistGameDataAsync();
			}
			catch
			{

			}
		}

		async Task GetWishlistGameDataAsync()
		{
			try
			{
				UserWishlistDatas = await WishlistService.GetAllWithFilterAsync(new WishlistSearch
				{
					UserId = currentUserId
				}) ?? new List<UserGameWishlistData>();
				var listGameIds = UserWishlistDatas.Select(c => c.GameId).ToHashSet();
				WishlistGameDatas = GameDatas
					.Where(x => listGameIds.Contains(x.Id))
					.ToList(); //list cac game wlist
				var addDateDict = UserWishlistDatas.ToDictionary(x => x.GameId, x => x.CreateDate);
				ViewModels = Mapper.Map<List<GameViewModel>>(WishlistGameDatas);
				foreach (var item in ViewModels)
				{
					if (item.GameCompanyId != null && CompanyDict.TryGetValue(item.GameCompanyId, out var name))
					{
						item.GameCompanyName = name;
					}
					if (addDateDict.TryGetValue(item.Id, out var date))
					{
						item.DateAddedWishlist = date;
					}
					item.TypeIds = string.IsNullOrEmpty(item.GameTypeId)
								? new List<string>()
								: JsonSerializer.Deserialize<List<string>>(item.GameTypeId)
								  ?? new List<string>();
					item.TypeNames = TypeDatas
						.Where(x => item.TypeIds.Contains(x.Id))
						.Select(x => x.Name)
						.ToList();
					// lấy % sale hienj tại và giá hjienj tại
					if (DiscountDict.TryGetValue(item.Id, out var discount))
					{
						item.CurrentSalePercent = discount.DiscountPercent;
						item.CurrentPrice = $"{(item.Price * (100 - discount.DiscountPercent) / 100).ToString("N0")} {item.Unit}";
					}
				}
			}
			catch
			{

			}
		}

		async Task GetAllGameAsync()
		{
			try
			{
				GameDatas = await GameService.GetAllWithFilterAsync(new GameSearch
				{

				}) ?? new List<GameData>();
			}
			catch
			{

			}
		}

		async Task GetAllCompanyGameAsync()
		{
			try
			{
				CompanyDatas = await CompanyService.GetAllWithFilterAsync(new GameCompanySearch
				{

				}) ?? new List<GameCompanyData>();
				CompanyDict = CompanyDatas.ToDictionary(x => x.Id, x => x.Name);
			}
			catch
			{

			}
		}

		async Task GetAllTypeGameAsync()
		{
			try
			{
				TypeDatas = await TypeService.GetAllWithFilterAsync(new GameTypeSearch
				{

				}) ?? new List<GameTypeData>();
			}
			catch
			{

			}
		}

		async Task GetDiscountAsync()
		{
			try
			{
				DiscountDatas = await DiscountService.GetAllWithFilterAsync(new DiscountSearch
				{

				}) ?? new List<GameDiscountData>();
				var now = DateTime.Now;
				DiscountDatas = DiscountDatas
				  .Where(x => x.StartDate <= now &&
							  x.EndDate >= now)
				  .ToList();
				DiscountDict = DiscountDatas.ToDictionary(x => x.GameId, x => x);
			}
			catch
			{

			}
		}

		async Task RemoveGameAsync(string gameId)
		{
			try
			{
				var data = UserWishlistDatas.Where(c => c.GameId == gameId).FirstOrDefault() ?? new UserGameWishlistData();
				var isDeleted = await WishlistService.RemoveFromWishlistAsync(data);
				if (isDeleted)
				{
					Notice.NotiSuccess("Xóa game khỏi wishlist thành công");
					ViewModels.RemoveAll(c => c.Id == gameId);
					State.SetCount(ViewModels.Count);
				}
				else
				{
					Notice.NotiError("Chưa xóa được, game vẫn còn bên trong");
				}
			}
			catch
			{

			}
			//finally
			//{
			//	await GetWishlistGameDataAsync();
			//}
		}

		async Task BuyGameAsync()
		{
			try
			{
				Notice.NotiWarning("Bận , làm cái này sau");
			}
			catch
			{

			}
		}
	}
}
