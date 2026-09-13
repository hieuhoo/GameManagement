using AntDesign;
using AntDesign.TableModels;
using AutoMapper;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.Extension;
using GameManagement.Share.Model.ViewModel;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.WebInterface.User
{
	public partial class UserTransactionHistory : ComponentBase
	{
		[Inject] IMapper Mapper { get; set; }
		[Inject] IUserService UserService { get; set; }
		[Inject] IWalletTransactionHistoryService TransactionHistoryService { get; set; }

		[Inject] NotificationService NoticeService { get; set; }
		[Inject] AuthenticationStateProvider AuthProvider { get; set; }
		List<WalletTransactionHistoryViewModel> ViewModels { get; set; } = new();
		Table<WalletTransactionHistoryViewModel> table;
		InputWatcher inputWatcher;

		bool loading;
		string currentUserId;
		string displayName;

		protected override async Task OnInitializedAsync()
		{
			try
			{
				var authState = await AuthProvider.GetAuthenticationStateAsync();
				var user = authState.User;
				currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				displayName = user.Identity?.Name ?? "";
				await LoadHistoryDataAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

		async Task LoadHistoryDataAsync()
		{
			try
			{
				var result = await TransactionHistoryService.GetAllWithFilterAsync(new TransactionHistorySearch
				{
					UserId = currentUserId
				}) ?? new List<WalletTransactionHistoryData>();
				var data = result.ToList().OrderByDescending(c => c.CreateDate);
				ViewModels = Mapper.Map<List<WalletTransactionHistoryViewModel>>(data);
				int stt = 1;
				foreach (var item in ViewModels)
				{
					item.Stt = stt++;
					if (item.Type == WalletTransactionType.PurchaseGame.ToString())
					{
						item.ChangeMoney = $"- {item.Amount.ToString("N0")}"; 
					}
					else
					{
						item.ChangeMoney = $"+ {item.Amount.ToString("N0")}"; 
					}
				}
			}
			catch
			{

			}
		}

		void OnRowClick(RowData<WalletTransactionHistoryViewModel> rowData)
		{
			try
			{

			}
			catch (Exception ex)
			{
				throw ex;
			}
			StateHasChanged();
		}
	}
}
