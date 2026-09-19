using AntDesign;
using AutoMapper;
using GameManagement.Service;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using GameManagement.Share.Model.ViewModel;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.WebInterface.User
{
	public partial class OtherPeopleReview : ComponentBase
	{
		[Inject] NavigationManager NavManager { get; set; }
		[Inject] NotificationService Notice { get; set; }
		[Inject] IUserGameReviewService ReviewService { get; set; }
		[Inject] IUserGameReviewReactionService ReactionService { get; set; }

		[Inject] IGameService GameService { get; set; }
		[Inject] IMapper Mapper { get; set; }

		List<UserGameReviewViewModel> ViewModels = new();
		IEnumerable<UserGameReviewViewModel> PagedReviews { get; set; } = Enumerable.Empty<UserGameReviewViewModel>();

		StatisticReviewGameData reviewStatisticData = new();

		bool isShowCommentVisible;
		bool reviewLoading;
		bool? displayPositiveComment;
		int currentPage;
		int pageSize = 4;
		int totalReviews;
		string reviewModalTitle = string.Empty;
		string currentGameId = string.Empty;

		string currentUserId = string.Empty;

		string CurrentFilter = ReviewFilter.All.ToString();

		protected override async Task OnInitializedAsync()
		{

		}


		private async Task LoadGameReviewsAsync()
		{
			try
			{
				reviewLoading = true;
				reviewStatisticData = await ReviewService.GetStatisticAboutGameAsync(currentGameId);
				var data = (await ReviewService.GetAllWithFilterAsync(new ReviewSearch
				{
					GameId = currentGameId,
					IsPositiveComment = displayPositiveComment
				}) ?? Enumerable.Empty<UserGameReviewData>())
					.OrderByDescending(c => c.StarNumber)
					.ToList();
				ViewModels = Mapper.Map<List<UserGameReviewViewModel>>(data);
				foreach (var item in ViewModels)
				{
					item.MyReaction = await ReactionService.CheckDisplayMyReactionAsync(currentUserId, item.Id);
				}
				PagedReviews = ViewModels
									.Skip((currentPage - 1) * pageSize)
									.Take(pageSize);
				totalReviews = ViewModels.Count;
			}
			catch
			{

			}
			finally
			{
				reviewLoading = false;
			}
		}


		Task OnPageChanged(
			PaginationEventArgs args)
		{
			currentPage = args.Page;

			PagedReviews = ViewModels
			.Skip((currentPage - 1) * pageSize)
			.Take(pageSize)
			.ToList();

			StateHasChanged();

			return Task.CompletedTask;
		}


		async Task OnFilterChanged(string value)
		{
			try
			{
				CurrentFilter = value;
				displayPositiveComment = null;
				if (value == ReviewFilter.Positive.ToString())
				{
					displayPositiveComment = true;
				}
				if (value == ReviewFilter.Negative.ToString())
				{
					displayPositiveComment = false;
				}
				currentPage = 1;
				await LoadGameReviewsAsync();
			}
			catch
			{

			}
		}


		async Task ChangeReactReviewAsync(
			string reviewId,
			string type)
		{
			try
			{
				var success = await ReactionService.ChangeReactionAsync(
											reviewId,
											currentUserId,
											type);
				if (!success)
				{
					Notice.NotiError("Có lỗi rồi, dm đoán đi cưng");
				}
				await LoadGameReviewsAsync();
			}
			catch
			{

			}
		}


		public async Task OpenReviewFormAsync(string gameId, string gameName, string currentUser)
		{
			reviewModalTitle = $"Danh sách đánh giá game: {gameName}";
			currentGameId = gameId;
			currentUserId = currentUser;
			await LoadGameReviewsAsync();
			isShowCommentVisible = true;

			await InvokeAsync(StateHasChanged);
		}


		//async Task EditReview(
		//	GameReviewViewModel review)
		//{
		//	// Load review vào EditModel
		//	// rồi mở Review Form Modal

		//	IsReviewFormVisible = true;

		//	await InvokeAsync(StateHasChanged);
		//}


		string GetUserInitial(string? userName, bool anonymous)
		{
			if (string.IsNullOrWhiteSpace(userName))
				return "?";
			var stringResult = anonymous == true ? "Người dùng ẩn danh" : userName
																			.Trim()
																			.Substring(0, 1)
																			.ToUpper();
			return stringResult;
		}
	}
}
