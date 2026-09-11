using AntDesign;
using AntDesign.TableModels;
using AutoMapper;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.Extension;
using GameManagement.Share.Model.ViewModel;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using static GameManagement.Share.Extension.EnumExtension;
using static GameManagement.Share.Extension.MessageEnumExtension;

namespace GameManagement.WebInterface.GameInformation
{
	public partial class GameRedeemCode : ComponentBase
	{
		[Inject] IGameRedeemCodeService RedeemService { get; set; }
		[Inject] IGameService GameService { get; set; }

		[Inject] IMapper Mapper { get; set; }

		[Inject] NotificationService Notice { get; set; }

		List<GameRedeemCodeViewModel> ViewModels { get; set; }
		List<GameRedeemCodeData> RedeemDatas { get; set; } = new();
		List<GameData> GameDatas { get; set; } = new();

		Table<GameRedeemCodeViewModel> Table;
		GameRedeemCodeDetail redeemDetailRef = new GameRedeemCodeDetail();
		Dictionary<string, string> GameDict = new Dictionary<string, string>();
		bool loading;
		string title;


		protected override async Task OnInitializedAsync()
		{
			try
			{
				await GetGameAsync();
				await GetRedeemDataAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}



		public async Task AddNewRedeemCode()
		{
			try
			{
				await redeemDetailRef.OpenRedeemFormAsync();
			}
			catch (Exception)
			{

			}
		}

		void OnRowClick(RowData<GameRedeemCodeViewModel> rowData)
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

		async Task GetRedeemDataAsync()
		{
			try
			{
				RedeemDatas = (await RedeemService.GetAllWithFilterAsync(
					new GameRedeemSearch
					{

					}
				) ?? new List<GameRedeemCodeData>())
					.OrderByDescending(x => x.CreateDate)
					.ToList();
				ViewModels = Mapper.Map<List<GameRedeemCodeViewModel>>(RedeemDatas);
				int stt = 1;
				foreach (var item in ViewModels)
				{
					item.Stt = stt++;
					if (item.GameId != null && GameDict.TryGetValue(item.GameId, out var name))
					{
						item.GameName = name;
					}
				}
			}
			catch
			{

			}
		}

		async Task GetGameAsync()
		{
			try
			{
				GameDatas = await GameService.GetAllWithFilterAsync(new GameSearch
				{

				}) ?? new List<GameData>();
				GameDict = GameDatas.ToDictionary(x => x.Id, x => x.Name);
			}
			catch
			{

			}
		}

		async Task ViewDetailAsync(GameRedeemCodeViewModel model)
		{
			try
			{
				await redeemDetailRef.LoadEditModelAsync(model);
			}
			catch
			{

			}
		}

		async Task ChangeStatusAsync(GameRedeemCodeViewModel model)
		{
			try
			{
				var datas = await RedeemService.GetAllWithFilterAsync(new GameRedeemSearch
				{
					Id = model.Id
				});
				var redeemData = datas?.FirstOrDefault() ?? new GameRedeemCodeData();
				if (redeemData.Status == RedeemCodeStatus.Available.ToString())
				{
					redeemData.Status = RedeemCodeStatus.Lock.ToString(); //đổi từ hiệu lực -> khóa
				}
				else
				{
					if (redeemData.ExpiredDate < DateTime.Now)
					{
						Notice.NotiError("Đã quá hạn sử dụng mã, ko thể đổi trạng thái");
						return;
					}
					redeemData.Status = RedeemCodeStatus.Available.ToString();// cập nhạt lại thành hiệu lựcs
				}
				var isSuccess = await RedeemService.UpdateRedeemAsync(redeemData);
				if (isSuccess)
				{
					Notice.NotiSuccess(OperationEnum.UpdateSuccessfully.GetDescription());
					await GetRedeemDataAsync(); // update lại giao diện
				}
				else
				{
					Notice.NotiError(OperationEnum.UpdateFailed.GetDescription());
				}
			}
			catch
			{

			}
		}

		async Task DeleteRedeemAsync(GameRedeemCodeViewModel model)
		{
			try
			{
				var datas = await RedeemService.GetAllWithFilterAsync(new GameRedeemSearch
				{
					Id = model.Id
				});
				var redeemData = datas?.FirstOrDefault() ?? new GameRedeemCodeData();
				if (redeemData.Status == RedeemCodeStatus.Used.ToString())
				{
					Notice.NotiError("Không thể xóa mã đã được sử dụng");
					return;
				}
				var isSuccess = await RedeemService.DeleteRedeemAsync(redeemData);
				if (isSuccess)
				{
					Notice.NotiSuccess(OperationEnum.DeleteSucessfully.GetDescription());
					await GetRedeemDataAsync();
				}
				else
				{
					Notice.NotiError(OperationEnum.DeleteFailed.GetDescription());
				}
			}
			catch
			{

			}
		}
	}
}
