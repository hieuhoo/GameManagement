using AntDesign;
using AutoMapper;
using GameManagement.Service.IService;
using GameManagement.Share.Model.ViewModel;
using Microsoft.AspNetCore.Components;

namespace GameManagement.WebInterface.GameInformation
{
	public partial class TransactionHistoryDetail : ComponentBase
	{
		[Inject] IWalletTransactionHistoryService HistoryService { get; set; }
		[Inject] IMapper Mapper { get; set; }
		List<WalletTransactionHistoryViewModel> ViewModels { get; set; } = new();
		Table<WalletTransactionHistoryViewModel> Table;

		bool transactionVisible;
		bool loading;

		string titleHistory = string.Empty;


		void CloseHistoryForm()
		{
			transactionVisible = false;
		}

		public async Task OpenDetailAsync(string gameName, string gameId, Dictionary<string, string> dict)
		{
			try
			{
				titleHistory = $"Lịch sử bán {gameName}";
				await LoadHistoryDetailAsync(gameId, dict);
				transactionVisible = true;
				await InvokeAsync(StateHasChanged);
			}
			catch
			{

			}
		}

		async Task LoadHistoryDetailAsync(string gameId, Dictionary<string, string> dict)
		{
			try
			{
				var data = await HistoryService.GetAllWithFilterAsync(new TransactionHistorySearch
				{
					RefId = gameId,
				});
				ViewModels = Mapper.Map<List<WalletTransactionHistoryViewModel>>(data);
				int stt = 1;
				foreach (var item in ViewModels)
				{
					item.Stt = stt++;
					if (item.UserId != null && dict.TryGetValue(item.UserId, out var name))
					{
						item.AccountName = name;
					}
				}
			}
			catch
			{

			}
		}
	}
}
