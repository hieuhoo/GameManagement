using AntDesign;
using AntDesign.TableModels;
using AutoMapper;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Service;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using GameManagement.Share.Extension;
using GameManagement.Share.Model.EditModel;
using GameManagement.Share.Model.ViewModel;
using GameManagement.SpecialComponent;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using static GameManagement.Share.Extension.EnumExtension;
using static GameManagement.Share.Extension.MessageEnumExtension;

namespace GameManagement.WebInterface.GameInformation
{
	public partial class ListGame : ComponentBase
	{
		[Inject] IGameService GameService { get; set; }
		[Inject] IMapper Mapper { get; set; }
		[Inject] IGameCompanyService CompanyService { get; set; }
		[Inject] IGameTypeService GameTypeService { get; set; }
		[Inject] IGameDiscountService GameDiscountService { get; set; }

		[Inject] NotificationService Notice { get; set; }
        [Inject] AuthenticationStateProvider AuthProvider { get; set; }

        List<GameViewModel> ViewModels { get; set; }
		List<GameData> GameDatas { get; set; }
		List<GameTypeData> GameTypeDatas { get; set; } = new();
		List<GameCompanyData> CompanyDatas { get; set; } = new();
		Dictionary<string, DiscountInformationData> DiscountDict = new();
        List<GameData> FeatureGameDatas { get; set; }

        Table<GameViewModel> Table;
		GameDetail gameDetailRef;
		int width;
		int height;
		int totalFeature;

		bool loading;
		bool createVisible;
        bool isAdmin = false;

        string title;

		protected override async Task OnInitializedAsync()
		{
			try
			{
                var authState = await AuthProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                isAdmin = user.IsInRole(UserRole.Admin.ToString());
                width = ConfigTemplate.Width;
				height = ConfigTemplate.Height;
				await GetGameTypeDataAsync();
				await GetGameCompanyDataAsync();
				await LoadDataAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

		async Task LoadDataAsync()
		{
			try
			{
				await GetDiscountDataAsync();
				var result = await GameService.GetAllWithFilterAsync(new GameSearch
				{

				});
				totalFeature = result.Where(c => c.IsFeatured == true).Count();
				GameDatas = result ?? new List<GameData>();
				FeatureGameDatas = GameDatas.Where(c => c.IsFeatured).ToList();
				ViewModels = Mapper.Map<List<GameViewModel>>(GameDatas);
				var dict = CompanyDatas.ToDictionary(x => x.Id, x => x.Name);
				int stt = 1;
				foreach (var game in ViewModels)
				{
					game.Stt = stt++;

					if (game.GameCompanyId != null &&
						dict.TryGetValue(game.GameCompanyId, out var companyName))
					{
						game.GameCompanyName = companyName;
					}

					if (game.Id != null &&
						DiscountDict.TryGetValue(game.Id, out var currentPercent))
					{
						game.CurrentSalePercent = currentPercent.Percent;
					}
					game.CurrentPrice = $"{(game.Price * (100 - game.CurrentSalePercent)/100):N0} {game.Unit}";
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		async Task UpdateAsync(GameViewModel model)
		{
			try
			{
				createVisible = true;
				title = "Thông tin game";
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		async Task DeleteAsync(GameViewModel model)
		{
			try
			{

			}
			catch
			{

			}
		}

		void AddNewGame()
		{
			createVisible = true;
			title = "Thêm mới game";
		}

		public void ReSize(int size)
		{
			if (size == 12)
			{
				width = ConfigTemplate.Width;
			}
			else
			{
				width = ConfigTemplate.Width / 2;
			}
			StateHasChanged();
		}

		async Task GetGameTypeDataAsync()
		{
			try
			{
				var result = await GameTypeService.GetAllWithFilterAsync(new GameTypeSearch
				{

				});
				GameTypeDatas = result ?? new List<GameTypeData>();
			}
			catch (Exception)
			{
				throw;
			}
		}

		async Task GetGameCompanyDataAsync()
		{
			try
			{
				var result = await CompanyService.GetAllWithFilterAsync(new GameCompanySearch
				{

				});
				CompanyDatas = result ?? new List<GameCompanyData>();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		void CloseDetailGame()
		{
			createVisible = false;
		}

		void OnRowClick(RowData<GameViewModel> rowData)
		{
			try
			{
				//SelectModel = AccountDatas.FirstOrDefault(c => c.Id == rowData.Data.Id) ?? new UserData();
				//Mapper.Map(SelectModel, EditModel);
				//EditModel.ReadOnly = true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			StateHasChanged();
		}

		async Task GetDiscountDataAsync()
		{
			try
			{
				var data = await GameDiscountService.GetAllWithFilterAsync(new DiscountSearch
				{

				});
				var now = DateTime.Now;

				DiscountDict = data
					.Where(x =>
						x.StartDate <= now &&
						x.EndDate >= now)
					.ToDictionary(
						x => x.GameId,
						x => new DiscountInformationData
						{
							Percent = x.DiscountPercent,
							StartDate = x.StartDate,
							EndDate = x.EndDate
						});
			}
			catch (Exception)
			{

			}
		}

		async Task ViewDetailAsync(GameViewModel model)
		{
			try
			{
				createVisible = true;
				await gameDetailRef.LoadEditModelAsync(model);
            }
			catch
			{

			}
		}

		async Task DeleteGameAsync(GameViewModel model)
		{
			try
			{
				Notice.NotiWarning("Đang phát triển chưa làm tới ");
				return;
			}
			catch
			{

			}
		}

        private List<GameViewModel> TestUIGames = new()
    {
        new GameViewModel
        {
            Id = "1",
            Name = "Bánh mì Bách Khoa",
            Price = 50000,
            Unit = "VNĐ",
            CurrentSalePercent = 0,
            CurrentPrice = "50,000 VNĐ",
            IsFeatured = false,
            Status = "Hoạt động",
            SoldStatus = "Đang bán",
            ReleaseStatus = "Đã ra mắt",
            GameCompanyName = "Moba"
        },

        new GameViewModel
        {
            Id = "2",
            Name = "007 First Light",
            Price = 900000,
            Unit = "VNĐ",
            CurrentSalePercent = 15,
            CurrentPrice = "765,000 VNĐ",
            IsFeatured = true,
            OrderFeatured = 1,
            Status = "Hoạt động",
            SoldStatus = "Đang bán",
            ReleaseStatus = "Đã ra mắt",
            GameCompanyName = "Tencent Game"
        },

        new GameViewModel
        {
            Id = "3",
            Name = "God of War: Ragnarok",
            Price = 700000,
            Unit = "VNĐ",
            CurrentSalePercent = 30,
            CurrentPrice = "490,000 VNĐ",
            IsFeatured = false,
            Status = "Hoạt động",
            SoldStatus = "Đang bán",
            ReleaseStatus = "Đã ra mắt",
            GameCompanyName = "Santa Monica"
        },

        new GameViewModel
        {
            Id = "4",
            Name = "Mortal Kombat 1",
            Price = 500000,
            Unit = "VNĐ",
            CurrentSalePercent = 0,
            CurrentPrice = "500,000 VNĐ",
            IsFeatured = true,
            OrderFeatured = 2,
            Status = "Hoạt động",
            SoldStatus = "Đang bán",
            ReleaseStatus = "Đã ra mắt",
            GameCompanyName = "Netherrealm"
        },

        new GameViewModel
        {
            Id = "5",
            Name = "Uncharted 4: A Thief's End",
            Price = 1050000,
            Unit = "VNĐ",
            CurrentSalePercent = 40,
            CurrentPrice = "630,000 VNĐ",
            IsFeatured = true,
            OrderFeatured = 3,
            Status = "Hoạt động",
            SoldStatus = "Đang bán",
            ReleaseStatus = "Đã ra mắt",
            GameCompanyName = "Naughty Dog"
        }
    };

    }
}
