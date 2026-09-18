using AntDesign;
using AutoMapper;
using GameManagement.CoreConfig;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.Model.EditModel;
using GameManagement.Share.Model.ViewModel;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;
using static GameManagement.Share.Extension.EnumExtension;
using static GameManagement.Share.Extension.MessageEnumExtension;

namespace GameManagement.WebInterface.GameInformation
{
	public partial class GameRedeemCodeDetail : ComponentBase
	{
		[Inject] IMapper Mapper { get; set; }
		[Inject] IGameRedeemCodeService RedeemService { get; set; }
		[Inject] IGameService GameService { get; set; }
		[Inject] IUserService UserService { get; set; }


		[Inject] NotificationService NoticeService { get; set; }
		[Parameter] public EventCallback ReloadData { get; set; }
		GameRedeemCodeEditModel EditModel { get; set; } = new();
		List<SelectItem> RedeemOptions { get; set; } = new();
		List<GameData> GameDatas { get; set; } = new();

		InputWatcher inputWatcher;

		bool redeemVisible;
		bool saving;

		void ChangeTypeGenCode(string type)
		{
			if (type == GenerateRedeemType.ByHand.ToString())
			{
				EditModel.QuantityCode = null;
			}
			else
			{
				EditModel.Code = null;
			}
		}

		void CloseRedeemDetail()
		{
			redeemVisible = false;
			EditModel = new GameRedeemCodeEditModel();
		}

		public async Task OpenRedeemFormAsync()
		{
			try
			{
				await LoadGameDataAsync();
				RedeemOptions = Enum.GetValues(typeof(RedeemCodeType)).Cast<RedeemCodeType>()
				  .Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
				redeemVisible = true;
				await InvokeAsync(StateHasChanged);
			}
			catch
			{

			}
		}

		public async Task LoadEditModelAsync(GameRedeemCodeViewModel model)
		{
			try
			{
				var user = await UserService.GetAllWithFilterAsync(new UserSearch
				{
					Role = UserRole.Normal.ToString()
				}) ?? new List<UserData>();
				var userDict = user.ToDictionary(x => x.Id, x => x.UserName);
				EditModel = Mapper.Map<GameRedeemCodeEditModel>(model);
				if (EditModel.RedeemedBy != null && userDict.TryGetValue(EditModel.RedeemedBy, out var name))
				{
					EditModel.RedeemedName = name;
				}
				var remaining = EditModel.ExpiredDate - DateTime.Now;

				if (remaining.TotalSeconds <= 0)
				{
					EditModel.RemainingTime = "Đã hết hạn";
				}
				else
				{
					EditModel.RemainingTime = 	$"{remaining.Days} ngày {remaining.Hours} giờ";
				}
				EditModel.ReadOnly = true;
				redeemVisible = true;
			}
			catch
			{

			}
		}

		void ChangeRedeemType(string value)
		{
			if (value == RedeemCodeType.Money.ToString())
			{
				EditModel.GameId = null;
			}
			else
			{
				EditModel.Value = null;
			}
		}

		async Task SaveRedeemAsync()
		{
			try
			{
				saving = true;
				if (EditModel.Id.IsNullOrEmpty())
				{
					await CreateAsync();
				}
				else
				{
					await UpdateAsync();
				}
				await ReloadData.InvokeAsync();
			}
			catch
			{

			}
			finally
			{
				saving = false;
			}
		}

		string GenerateRedeemCode()
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

			var random = new Random();

			var suffix = new string(
				Enumerable.Range(0, 3)
					.Select(_ => chars[random.Next(chars.Length)])
					.ToArray());

			return $"{GlobalVariant.PrefixRedeemCode}{suffix}";
		}

		async Task CreateAsync()
		{
			try
			{
				var errorMessageStore = EditModel.ValidateAll();
				if (!inputWatcher.Validate() || errorMessageStore?.Any() == true)
				{
					if (errorMessageStore.Any())
					{
						inputWatcher.NotifyFieldChanged(errorMessageStore.First().Key, errorMessageStore);
					}
					NoticeService.NotiWarning(TypeAlert.InvalidData.GetDescription());
					return;
				}
				var prefix = GlobalVariant.PrefixRedeemCode;
				if (EditModel.GenerateType == GenerateRedeemType.ByHand.ToString())
				{
					EditModel.Code = EditModel.Code.Trim().ToUpper();
					var exist = await RedeemService.IsRedeemCodeExistsAsync(EditModel.Code);
					if (exist)
					{
						NoticeService.NotiError($"Mã nạp {EditModel.Code} đã tồn tại.Vui lòng tạo mã khác");
						return;
					}
					if (!Regex.IsMatch(
						EditModel.Code ?? "",
						$"^{Regex.Escape(prefix)}[A-Z0-9]{{3}}$"))
					{
						NoticeService.NotiWarning($"Mã nạp phải có dạng {prefix}XXX."); // thiết kế 3 kí tự cho nhanh
						return;
					}
					EditModel.Id = ObjectExtentions.GenerateGuid();
					EditModel.CreateDate = DateTime.Now;
					EditModel.CreateBy = "ADMIN"; //TEST
					EditModel.Status = RedeemCodeStatus.Available.ToString();
					var redeemData = Mapper.Map<GameRedeemCodeData>(EditModel);
					var isSuccess = await RedeemService.SaveRedeemAsync(redeemData);
					if (isSuccess)
					{
						NoticeService.NotiSuccess(OperationEnum.AddSuccessfully.GetDescription());
					}
					else
					{
						NoticeService.NotiError(OperationEnum.AddFailed.GetDescription());
					}
					EditModel = new GameRedeemCodeEditModel();
					redeemVisible = false;
					return;
				}
				else
				{
					var listRedeem = new List<GameRedeemCodeData>();
					var generatedCodes = new HashSet<string>(
								StringComparer.OrdinalIgnoreCase
							);
					var batchId = ObjectExtentions.GenerateGuid();
					for (int x = 0; x < EditModel.QuantityCode; x++)
					{
						var uniqueCode = await GenUniqueCodeAsync(generatedCodes);
						var item = new GameRedeemCodeData
						{
							Id = ObjectExtentions.GenerateGuid(),
							Code = uniqueCode,
							Value = EditModel.Value,
							CreateDate = DateTime.Now,
							CreateBy = "ADMIN",
							Status = RedeemCodeStatus.Available.ToString(),
							ExpiredDate = EditModel.ExpiredDate,
							BatchId = batchId,
							GenerateType = GenerateRedeemType.Auto.ToString(),
							RedeemType = EditModel.RedeemType,
							GameId = EditModel.GameId
						};
						listRedeem.Add(item);
					}
					var isSuccess = await RedeemService.SaveListRedeemAsync(listRedeem);
					if (isSuccess)
					{
						NoticeService.NotiSuccess(OperationEnum.AddSuccessfully.GetDescription());
					}
					else
					{
						NoticeService.NotiError(OperationEnum.AddFailed.GetDescription());
					}
					EditModel = new GameRedeemCodeEditModel();
					redeemVisible = false;
					return;
				}
			}
			catch
			{

			}
		}

		async Task UpdateAsync()
		{
			try
			{
				// chưa bt ngiệp vụ sửa sao đây :))
			}
			catch
			{

			}
		}

		async Task<string> GenUniqueCodeAsync(
				HashSet<string> currentCodes)
		{
			const int maxRetry = 30;

			for (int i = 0; i < maxRetry; i++)
			{
				var code = GenerateRedeemCode()
					.Trim()
					.ToUpper();

				// Check in batch hiện tại
				if (currentCodes.Contains(code))
				{
					continue; // trùng sẽ quay lại vòng for chạy tiếp
				}

				var exists = await RedeemService.IsRedeemCodeExistsAsync(code);

				if (exists)
				{
					continue; // trùng sẽ quay lại vòng for chạy tiếp
				}

				currentCodes.Add(code);

				return code;
			}

			throw new Exception(
				"Không thể tạo redeem code không trùng sau nhiều lần thử.");
		}

		async Task LoadGameDataAsync()
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
	}
}
