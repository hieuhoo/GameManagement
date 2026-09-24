using AntDesign;
using AutoMapper;
using GameManagement.CoreConfig.Extensions;
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
		[Parameter] public Dictionary<string, string> DictUser { get; set; }
		[Inject] NavigationManager NavManager { get; set; }
		[Inject] NotificationService Notice { get; set; }
		[Inject] IUserGameReviewService ReviewService { get; set; }
		[Inject] IUserGameReviewReactionService ReactionService { get; set; }

		[Inject] IGameService GameService { get; set; }
		[Inject] IUserGameReviewReplyService ReplyService { get; set; }

		[Inject] IMapper Mapper { get; set; }

		List<UserGameReviewViewModel> ViewModels = new();
		IEnumerable<UserGameReviewViewModel> PagedReviews { get; set; } = Enumerable.Empty<UserGameReviewViewModel>();

		StatisticReviewGameData reviewStatisticData = new();
		List<UserGameReviewReplyData> ReviewRepDatas = new();

		GameReview reviewRef = new GameReview();
		HashSet<string> expandedReplies = new();
		HashSet<string> expandedReplyContent = new();
		HashSet<string> expandedAllReplies = new();
		List<string> Emojis =
			"😀 😃 😄 😁 😂 🤣 😊 😍 😎 😭 😡 👍 👎 ❤️ 🔥 👏 🙏 💪 🎉 🎮 🏆 ⭐ 💯 🚀 🤩 🥳 🤔 😱 😢 😆 😜 🤪"
			.Split(' ')
			.ToList();

		bool isShowCommentVisible;
		bool reviewLoading;
		bool? displayPositiveComment;
		int currentPage;
		int pageSize = 4;
		int totalReviews;
		int totalHiddenReviews;
		int totalDisplayReviews;
		string reviewModalTitle = string.Empty;
		string currentGameId = string.Empty;

		string currentUserId = string.Empty;

		string CurrentFilter = ReviewFilter.All.ToString();
		string NameOfGame = string.Empty;
		string? replyingToId;
		string replyContent;
		string replyToUserName;
		bool replyIsAnonymous = false;
		string? editingReplyId;
		string? editingReplyContent;

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
					if (item.UserId != null && DictUser.TryGetValue(item.UserId, out var name))
					{
						item.UserName = name;
					}
					item.ReactInfos = await ReactionService.GetListUsersReactAsync(item.Id);
					item.RepliesData = ReviewRepDatas.Where(c => c.ReviewId == item.Id)
													.Where(c => c.IsDeleted == false)
													.OrderByDescending(c => c.CreateDate)
													.ToList();
					foreach (var rep in item.RepliesData)
					{
						if (rep.UserId != null && DictUser.TryGetValue(rep.UserId, out var personName))
						{
							rep.UserName = personName;
						}
					}
				}
				totalReviews = ViewModels.Count;
				totalHiddenReviews = ViewModels.Count(c => c.IsHidden);
				totalDisplayReviews = totalReviews - totalHiddenReviews;
				// lấy những cái ko bị admin ẩn
				PagedReviews = ViewModels
									.Where(c => c.IsHidden == false)
									.Skip((currentPage - 1) * pageSize)
									.Take(pageSize);
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
			try
			{
				NameOfGame = gameName;
				reviewModalTitle = $"Danh sách đánh giá game: {gameName}";
				currentGameId = gameId;
				currentUserId = currentUser;
				await GetAllReplyCommentAsync();
				await LoadGameReviewsAsync();
				isShowCommentVisible = true;

				await InvokeAsync(StateHasChanged);
			}
			catch
			{

			}
		}


		async Task EditYourReviewAsync(string userId, string gameId)
		{
			try
			{
				await reviewRef.OpenReviewFormAsync(userId, gameId, NameOfGame);
			}
			catch
			{

			}
		}


		string GetUserInitial(string? userName, bool anonymous)
		{
			if (string.IsNullOrWhiteSpace(userName))
			{
				userName = "?";
			}
			var stringResult = anonymous == true ? "?" : userName
																			.Trim()
																			.Substring(0, 1)
																			.ToUpper();
			return stringResult;
		}


		async Task SubmitReplyAsync(string reviewId)
		{
			try
			{
				UserGameReviewReplyData data = new UserGameReviewReplyData
				{
					Id = ObjectExtentions.GenerateGuid(),
					ReviewId = reviewId,
					CreateDate = DateTime.Now,
					UpdatedDate = DateTime.Now,
					ReplyContent = replyContent,
					IsHidden = false,
					IsDeleted = false,
					IsAnonymous = replyIsAnonymous,
					UserId = currentUserId
				};
				var isDone = await ReplyService.AddReplyForCommentAsync(data);
				if (isDone)
				{
					Notice.NotiSuccess("Phản biện oke ,đã lưu db");
					// Tìm review hiện tại trên UI
					var review = ViewModels.FirstOrDefault(x => x.Id == reviewId);
					if (review != null)
					{
						// Nếu chưa có list reply thì tạo mới
						review.RepliesData ??= new List<UserGameReviewReplyData>();
						// Add reply mới vào UI ngay lập tức
						review.RepliesData.Add(data);
						review.RepliesData = review.RepliesData.OrderByDescending(c => c.CreateDate).ToList();
						// Mở danh sách reply
						if (!expandedReplies.Contains(review.Id))
						{
							expandedReplies.Add(review.Id);
						}
					}
					replyingToId = null;
					replyToUserName = null;
					replyContent = string.Empty;
					return;
				}
				else
				{
					Notice.NotiWarning("Có lỗi rồi dcm");
				}
			}
			catch
			{

			}
		}

		string GetDisplayUserName(string userName, bool isAnonymous)
		{
			return isAnonymous
				? "@Người dùng ẩn danh"
				: $"@{userName}";
		}

		void ToggleReplyBox(string replyId, bool isAnonymous, string userName)
		{
			// đang mở chính comment này => đóng
			if (replyingToId == replyId)
			{
				replyingToId = null;
				replyToUserName = null;
				replyContent = string.Empty;
				return;
			}

			// mở comment mới
			replyingToId = replyId;
			replyToUserName = userName;
			replyContent = string.Empty;
		}

		void HandleInputReply(ChangeEventArgs e)
		{
			replyContent = e.Value?.ToString();
		}

		string GetNameOfPersonReply()
		{
			if (currentUserId != null && DictUser.TryGetValue(currentUserId, out var name))
			{
				return name;
			}
			return string.Empty;
		}

		async Task GetAllReplyCommentAsync()
		{
			try
			{
				ReviewRepDatas = (await ReplyService.GetAllWithFilterAsync(new ReviewReplySearch
				{
					IsDeleted = false
				})) ?? new List<UserGameReviewReplyData>();
			}
			catch
			{

			}
		}

		void ToggleReplyList(string reviewId)
		{
			if (expandedReplies.Contains(reviewId))
			{
				expandedReplies.Remove(reviewId);
			}
			else
			{
				expandedReplies.Add(reviewId);
			}
		}

		string GetRelativeTime(DateTime date)
		{
			var diff = DateTime.Now - date;

			if (diff.TotalSeconds < 60)
			{
				return "Vừa xong";
			}

			if (diff.TotalMinutes < 60)
			{
				return $"{(int)diff.TotalMinutes} phút trước";
			}

			if (diff.TotalHours < 24)
			{
				return $"{(int)diff.TotalHours} giờ trước";
			}

			if (diff.TotalDays < 7)
			{
				return $"{(int)diff.TotalDays} ngày trước";
			}

			if (diff.TotalDays < 30)
			{
				return $"{(int)(diff.TotalDays / 7)} tuần trước";
			}

			if (diff.TotalDays < 365)
			{
				return $"{(int)(diff.TotalDays / 30)} tháng trước";
			}

			return $"{(int)(diff.TotalDays / 365)} năm trước";
		}

		void EditReply(UserGameReviewReplyData data)
		{
			editingReplyId = data.Id;
			editingReplyContent = data.ReplyContent;
		}

		async Task DeleteReplyAsync(UserGameReviewReplyData data)
		{
			try
			{
				if (data == null)
				{
					Notice.NotiWarning("Không tìm thấy phản hồi");
					return;
				}
				data.IsDeleted = true;
				data.UpdatedDate = DateTime.Now;
				var isDone = await ReplyService.UpdateReplyAsync(data);
				if (isDone)
				{
					var review = ViewModels
									 .FirstOrDefault(x => x.Id == data.ReviewId);
					if (review?.RepliesData != null)
					{
						var reply = review.RepliesData
							.FirstOrDefault(x => x.Id == data.Id);
						if (reply != null)
						{
							review.RepliesData.Remove(reply);
						}
					}
					StateHasChanged();
					return;
				}
				Notice.NotiWarning("Xóa phản hồi thất bại");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				Notice.NotiError("Có lỗi xảy ra");
			}
		}

		void CancelEditReply()
		{
			editingReplyId = null;

			editingReplyContent = null;
		}

		void CloseFormReview()
		{
			expandedAllReplies.Clear();
		}

		void AddEmoji(string emoji, bool isReplyEdit)
		{
			if (isReplyEdit == false)
			{
				replyContent += emoji;
			}
			else
			{
				editingReplyContent += emoji;
			}
		}

		void ToggleLongReplyContent(string replyId)
		{
			if (expandedReplyContent.Contains(replyId))
			{
				expandedReplyContent.Remove(replyId);
			}
			else
			{
				expandedReplyContent.Add(replyId);
			}

			StateHasChanged();
		}

		async Task SaveEditReplyAsync(UserGameReviewReplyData data)
		{
			try
			{
				if (string.IsNullOrEmpty(editingReplyContent))
					return;

				data.ReplyContent = editingReplyContent;
				data.UpdatedDate = DateTime.Now;

				await ReplyService.UpdateReplyAsync(data);

				editingReplyId = null;
				editingReplyContent = string.Empty;

				StateHasChanged();

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		void ToggleAllReplies(string reviewId)
		{
			if (expandedAllReplies.Contains(reviewId))
			{
				expandedAllReplies.Remove(reviewId);
			}
			else
			{
				expandedAllReplies.Add(reviewId);
			}

			StateHasChanged();
		}
	}
}
