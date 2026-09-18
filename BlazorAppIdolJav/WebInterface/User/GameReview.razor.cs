using AntDesign;
using AutoMapper;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.Model.EditModel;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using static GameManagement.Share.Extension.MessageEnumExtension;

namespace GameManagement.WebInterface.User
{
	public partial class GameReview : ComponentBase
	{
		[Inject] NotificationService NoticeService { get; set; }
		[Inject] IMapper Mapper { get; set; }
		[Inject] IUserGameReviewService ReviewService { get; set; }
		UserGameReviewEditModel EditModel { get; set; } = new UserGameReviewEditModel();
		InputWatcher inputWatcher { get; set; }

		bool isReviewVisible;
		string titleForm = string.Empty;
		string currentUser;
		string currentGame;

		async Task CreateReviewAsync()
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
				EditModel.Id = ObjectExtentions.GenerateGuid();
				EditModel.CreateDate = DateTime.Now;
				EditModel.UpdatedDate = DateTime.Now;
				EditModel.UserId = currentUser;
				EditModel.GameId = currentGame;
				var data = Mapper.Map<UserGameReviewData>(EditModel);
				var isSucess = await ReviewService.CreateReviewAsync(data);
				if (isSucess)
				{
					NoticeService.NotiSuccess("Bình luận của bạn đã được lưu");
					isReviewVisible = false;
					EditModel = new();
				}
				else
				{
					NoticeService.NotiError("Có lỗi xảy ra rồi đoán xem dcm lỗi gì");
				}
			}
			catch
			{

			}
		}

		async Task SubmitReviewAsync()
		{
			try
			{
				if (EditModel.Id.IsNullOrEmpty())
				{
					await CreateReviewAsync();
				}
				else
				{
					await UpdateReviewAsync();
				}
			}
			catch
			{

			}
		}

		async Task UpdateReviewAsync()
		{
			try
			{
				// chưa làm
			}
			catch
			{

			}
		}

		void CloseReviewModal()
		{
			EditModel = new();
			isReviewVisible = false;
		}

		public async Task OpenReviewFormAsync(string userId, string gameId, string name)
		{
			try
			{
				isReviewVisible = true;
				currentUser = userId;
				currentGame = gameId;
				titleForm = $"Đánh giá game : {name}";
				await InvokeAsync(StateHasChanged);
			}
			catch
			{

			}
		}

		void OnStarChanged(decimal value)
		{
			EditModel.StarNumber = value;
		}
	}
}
