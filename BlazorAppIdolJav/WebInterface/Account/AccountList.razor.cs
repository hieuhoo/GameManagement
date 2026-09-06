using AntDesign;
using AntDesign.TableModels;
using AutoMapper;
using GameManagement.CoreConfig;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.Extension;
using GameManagement.Share.Model.EditModel;
using GameManagement.Share.Model.ViewModel;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using static GameManagement.Share.Extension.EnumExtension;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameManagement.WebInterface.Account
{
	public partial class AccountList : ComponentBase
	{
		[Inject] IMapper Mapper { get; set; }
		[Inject] IUserService UserService { get; set; }
		[Inject] IUserLockHistoryService HistoryLockService { get; set; }

		[Inject] NotificationService NoticeService { get; set; }
		[Inject] AuthenticationStateProvider AuthProvider { get; set; }
		List<AccountViewModel> ViewModels { get; set; } = new();
		UserData SelectModel { get; set; }
		List<UserData> AccountDatas { get; set; }
		UserData CurrentUser { get; set; }
		List<UserLockHistoryData> LockHistoryDatas { get; set; }
		Table<AccountViewModel> table;
		GameCompanyEditModel EditModel;
		InputWatcher inputWatcher;

		bool loading;
		bool historyVisible;
		protected override async Task OnInitializedAsync()
		{
			try
			{
				var authState = await AuthProvider.GetAuthenticationStateAsync();

				var userName = authState.User.Identity?.Name;

				if (!string.IsNullOrWhiteSpace(userName))
				{
					CurrentUser = await UserService.GetUserInfoAsync(
						new UserSearch
						{
							UserName = userName
						});
				}
				await LoadingDataAsync();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		async Task LoadingDataAsync()
		{
			try
			{
				AccountDatas = await UserService.GetAllWithFilterAsync(new UserSearch
				{

				}) ?? new List<UserData>();
				AccountDatas = AccountDatas.OrderByDescending(c => c.CreateDate).ToList();
				ViewModels = Mapper.Map<List<AccountViewModel>>(AccountDatas);
				int stt = 1;
				ViewModels.ForEach(c => c.Stt = stt++);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		void OnRowClick(RowData<AccountViewModel> rowData)
		{
			try
			{
				SelectModel = AccountDatas.FirstOrDefault(c => c.Id == rowData.Data.Id) ?? new UserData();
				//Mapper.Map(SelectModel, EditModel);
				//EditModel.ReadOnly = true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			StateHasChanged();
		}

		void ViewAccount(AccountViewModel data)
		{
		}

		async Task LockAccountAsync(AccountViewModel model)
		{
			try
			{
				var isAdmin = model.Role == UserRole.Admin.ToString();
				if (model.Id == CurrentUser.Id || isAdmin)
				{
					NoticeService.NotiWarning(
							isAdmin
							? "Không thể khóa tài khoản của ADMIN"
							: "Không thể khóa tài khoản của chính mình");
					return;
				}
				var data = await UserService.GetUserInfoAsync(new UserSearch
				{
					Id = model.Id
				}) ?? new UserData();

				data.Status = AccountStatus.Lock.ToString();
				data.LockBy = LockPerson.Admin.ToString();
				data.LockReason = AccountLockReason.ByAdmin.ToString();
				data.UpdatedDate = DateTime.Now;

				var result = await UserService.UpdateAccountAsync(data);
				if (result)
				{
					//logic add vào lịch sử khóa/mở
					var historyData = model.Id.FillLockHistoryData(
							AccountOperation.Lock,
							LockPerson.Admin,
							AccountLockReason.ByAdmin
						);
					await HistoryLockService.AddLockHistoryAsync(historyData);
					NoticeService.NotiSuccess("Khóa tài khoản thành công.User sẽ không thể đăng nhập đến khi được mở lại");
					await LoadingDataAsync();
				}
				else
				{
					NoticeService.NotiSuccess("Khóa tài khoản thất bại, đã có lỗi xảy ra");
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		async Task UnlockAccountAsync(AccountViewModel model)
		{
			try
			{
				var data = await UserService.GetUserInfoAsync(new UserSearch
				{
					Id = model.Id
				}) ?? new UserData();
				data.Status = AccountStatus.Active.ToString();
				data.LockBy = null;
				data.LockReason = null;
				data.UpdatedDate = DateTime.Now;
				data.FailedLoginCount = 0;

				var result = await UserService.UpdateAccountAsync(data);
				if (result)
				{
					//logic add vào lịch sử khóa/mở
					var historyData = model.Id.FillLockHistoryData(
						AccountOperation.Unlock,
						LockPerson.Admin);

					await HistoryLockService.AddLockHistoryAsync(historyData);
					NoticeService.NotiSuccess("Mở khóa tài khoản thành công");
					await LoadingDataAsync();
				}
				else
				{
					NoticeService.NotiError("Mở khóa tài khoản thất bại, có lỗi xảy ra");
				}
			}
			catch
			{

			}
		}

		void ChangePassword(AccountViewModel data)
		{
		}

		async Task ViewHistoryAccountAsync(AccountViewModel model)
		{
			try
			{

			}
			catch
			{

			}
		}

		void CloseHistoryModal()
		{
			historyVisible = false;
			SelectedAccount = null;
			LockHistoryDatas.Clear();
		}
	}
}
