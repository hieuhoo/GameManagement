using AntDesign;
using AntDesign.TableModels;
using AutoMapper;
using GameManagement.Components.State;
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
using System.Security.Claims;
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
		[Inject] IUserGameWishlistService WishlistService { get; set; }
		[Inject] IUserWalletService WalletService { get; set; }
		[Inject] IPurchaseService PurchaseService { get; set; }
		[Inject] IUserGameLibraryService LibraryService { get; set; }
		[Inject] IEmailService MailService { get; set; }
		[Inject] IWalletTransactionHistoryService HistoryService { get; set; }
		[Inject] IUserService UserService { get; set; }

		[Inject] NotificationService Notice { get; set; }
		[Inject] AuthenticationStateProvider AuthProvider { get; set; }
		[Inject] private WishlistState WishlistState { get; set; } = default!;
		[Inject] NavigationManager NavManager { get; set; }
		List<GameViewModel> ViewModels { get; set; } = new List<GameViewModel>();
		List<GameData> GameDatas { get; set; }
		List<GameTypeData> GameTypeDatas { get; set; } = new();
		List<GameCompanyData> CompanyDatas { get; set; } = new();
		Dictionary<string, DiscountInformationData> DiscountDict = new();
		List<GameData> FeatureGameDatas { get; set; }
		GameViewModel SelectedGame { get; set; } = new();
		Table<GameViewModel> Table;
		GameDetail gameDetailRef;
		TransactionHistoryDetail transactionDetailRef = new TransactionHistoryDetail();
		Dictionary<string, string> DictUser = new();
		int width;
		int height;
		int totalFeature;

		bool loading;
		bool createVisible;
		bool isAdmin = false;
		bool isPurchaseModalVisible;

		string title;
		string currentUserId;
		string confirmText = "Thanh toán";
		string cancelText = "Hủy";
		string userName;
		string PaymentMethod = MethodPurchase.Banking.ToString();

		protected override async Task OnInitializedAsync()
		{
			try
			{
				var authState = await AuthProvider.GetAuthenticationStateAsync();
				var user = authState.User;
				isAdmin = user.IsInRole(UserRole.Admin.ToString());
				userName = user.Identity.Name;
				currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				width = ConfigTemplate.Width;
				height = ConfigTemplate.Height;
				await LoadListUserAsync();
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
					game.CurrentPrice = $"{(game.Price * (100 - game.CurrentSalePercent) / 100):N0} {game.Unit}";
					game.RecentlyInWishlist = await IsGameInWishlist(game.Id, currentUserId);
					game.IsPurchased = await IsGameInLibrary(game.Id, currentUserId);
					game.TotalSold = (await HistoryService.GetTotalGameRevenue(game.Id)).TotalSold;
					game.TotalRevenue = (await HistoryService.GetTotalGameRevenue(game.Id)).TotalRevenue;
				}
				int total = ViewModels.Where(c => c.RecentlyInWishlist == true).ToList().Count();
				WishlistState.SetCount(total);
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

		async Task AddOrRemoveGameWishlistAsync(string gameId)
		{
			try
			{
				// luồng check trong library
				var isOwned = await IsGameInLibrary(gameId, currentUserId);
				if (isOwned)
				{
					Notice.NotiWarning("Bạn đã mua game này nên không thể thêm vào wishlist");
					return;
				}
				// nếu chưa có ms tiếp tục
				var existData = await WishlistService.GetAllWithFilterAsync(new WishlistSearch
				{
					UserId = currentUserId,
					GameId = gameId
				});
				if (existData.Count == 0)
				{
					var newData = new UserGameWishlistData
					{
						Id = ObjectExtentions.GenerateGuid(),
						UserId = currentUserId,
						GameId = gameId,
						CreateDate = DateTime.Now,
					};
					var isSuccess = await WishlistService.AddToWishlistAsync(newData);
					if (isSuccess)
					{
						Notice.NotiSuccess("Thêm vào danh sách ưa thích thành công");
					}
					else
					{
						Notice.NotiError("Thêm thất bại , có lỗi xảy ra");
					}
				}
				else
				{
					var deleteData = existData.FirstOrDefault() ?? new UserGameWishlistData();
					var isComplete = await WishlistService.RemoveFromWishlistAsync(deleteData);
					if (isComplete)
					{
						Notice.NotiSuccess("Xóa khỏi danh sách ưa thích thành công");
					}
					else
					{
						Notice.NotiError("vẫn chưa xóa đc khỏi danh sách");
					}
				}
				await LoadDataAsync();
				StateHasChanged();
			}
			catch
			{

			}
		}

		async Task<bool> IsGameInWishlist(string gameId, string userId)
		{
			try
			{
				var isExist = await WishlistService.CheckExistGameInWishlistAsync(gameId, userId);
				return isExist;
			}
			catch
			{
				return false;
			}
		}

		async Task ViewDetailGameAsync(string gameId)
		{
			try
			{
				// mở ra màn xem các đánh giá của reviewer khác
				Notice.NotiWarning("Bận , sẽ làm func này sau");
				return;
			}
			catch
			{

			}
		}

		void PurchaseGameNow(GameViewModel game)
		{

			SelectedGame = game;
			isPurchaseModalVisible = true;
		}

		async Task ConfirmPurchaseAsync()
		{
			try
			{
				if (PaymentMethod == MethodPurchase.Banking.ToString())
				{
					Notice.NotiWarning("Cái này chưa tích hợp , troll thôi chọn cái Wallet đê dcm hehe");
					return;
				}
				int currentPrice = SelectedGame.Price * (100 - SelectedGame.CurrentSalePercent) / 100;
				var currentBalance = (await WalletService.GetAllWithFilterAsync(new UserWalletSearch
				{
					UserId = currentUserId
				})).FirstOrDefault()?.Balance ?? 0;
				if (currentBalance < currentPrice)
				{
					var lackMoney = (currentPrice - currentBalance).ToString("N0");
					Notice.NotiWarning($"Số dư tài khoản ví của bạn ko đủ thanh toán ," +
						$" vui lòng nạp thêm {lackMoney} đ để tiếp tục");
					return;
				}
				var result = await PurchaseService.ProcessPurchaseTransactionAsync(new PaymentGameInforData
				{
					UserId = currentUserId,
					GameId = SelectedGame.Id,
					PercentSale = SelectedGame.CurrentSalePercent,
					OriginalPrice = SelectedGame.Price,
					CurrentPrice = currentPrice,
					GameName = SelectedGame.Name,
					FullName = userName
				});
				if (!result.IsSuccess)
				{
					Notice.NotiError("Có lỗi xảy ra rồi , tất cả bị rollback");
					return;
				}

				//phần gửi mail 
				await MailService.SendTemplateMailAsync(
					  MailType.PurchaseSuccess,
					  new PurchaseGameReceiptData
					  {
						  FullName = result.FullName ?? "",
						  GameName = result.GameName ?? "",
						  UserName = result.FullName ?? "",
						  TransactionId = result.TransactionId ?? "",
						  PurchaseDate = result.PurchaseDate ?? "",
						  OriginalPrice = result.OriginalPrice ?? "",
						  DiscountPercent = result.DiscountPercent ?? "",
						  PurchasePrice = result.PurchasePrice ?? "",
						  Email = "hieuhooepu@gmail.com" // test mail này nhé
					  }
				);

				Notice.NotiSuccess("Mua game thành công ,vui lòng kiểm tra trong thư viện");
				await LoadDataAsync();
			}
			catch
			{

			}
			finally
			{
				PaymentMethod = MethodPurchase.Banking.ToString();
			}
		}

		async Task<bool> IsGameInLibrary(string gameId, string userId)
		{
			try
			{
				var isExist = await LibraryService.CheckExistGameInLibraryAsync(gameId, userId);
				return isExist;
			}
			catch
			{
				return false;
			}
		}

		async Task ViewHistoryAsync(string name, string id)
		{
			try
			{
				await transactionDetailRef.OpenDetailAsync(name, id, DictUser);
			}
			catch
			{

			}
		}

		async Task LoadListUserAsync()
		{
			try
			{
				var result = await UserService.GetAllWithFilterAsync(new UserSearch
				{
					Role = UserRole.Normal.ToString(),	
				}) ?? new List<UserData>();
				DictUser = result.ToDictionary(x => x.Id, x => x.UserName);
			}
			catch
			{

			}
		}

		async Task OpenVoteFormAsync(string userId, string gameId)
		{
			Notice.NotiWarning("Chưa làm");
		}
	}
}
