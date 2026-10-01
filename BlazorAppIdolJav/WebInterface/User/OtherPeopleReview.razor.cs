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
		Dictionary<string, int> displayedChildReplyCount = new(); // khống chế hiển thị child

		bool isShowCommentVisible;
		bool reviewLoading;
		bool? displayPositiveComment;
		string? replyingReviewId;
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
		string replyContentLevel2;

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
													.Where(c => c.ParentId == null)
													//.Where(c => c.IsDeleted == false)
													.OrderByDescending(c => c.CreateDate)
													.ToList();
					foreach (var rep in item.RepliesData)
					{
						if (rep.UserId != null && DictUser.TryGetValue(rep.UserId, out var personName))
						{
							rep.UserName = personName;
						}
						rep.Children = ReviewRepDatas.Where(c => c.ReviewId == item.Id)
													.Where(c => c.ParentId == rep.Id)
													.Where(c => c.IsDeleted == false)
													.OrderByDescending(c => c.CreateDate)
													.ToList();
						rep.IsLikedByMe = (await ReactionService.CheckDisplayMyReactionAsync(currentUserId, rep.ReviewId, rep.Id) == TypeReaction.Like.ToString());
						if (rep.Children.Any())
						{
							foreach (var child in rep.Children)
							{
								if (child.UserId != null && DictUser.TryGetValue(child.UserId, out var replierLevel2))
								{
									child.UserName = replierLevel2;
								}
							}
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
					ParentId = replyingToId,
					CreateDate = DateTime.Now,
					UpdatedDate = DateTime.Now,
					ReplyContent = replyingToId == null
									? replyContent
									: replyContentLevel2,
					IsHidden = false,
					IsDeleted = false,
					IsAnonymous = replyIsAnonymous,
					UserId = currentUserId,
					LikeCount = 0
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
						review.RepliesData = review.RepliesData
											.Where(x => x.ParentId == null)
											.OrderByDescending(c => c.CreateDate).ToList();
						// show load lại tên người cmt luôn
						var latestReply = review.RepliesData.FirstOrDefault() ?? new UserGameReviewReplyData();
						if (latestReply.UserId != null && DictUser.TryGetValue(latestReply.UserId, out var name))
						{
							latestReply.UserName = name;
						}
						// nếu là reply level 2
						if (data.ParentId != null)
						{
							var parentReply = review.RepliesData
								.FirstOrDefault(x => x.Id == data.ParentId);

							if (parentReply != null)
							{
								parentReply.Children ??= new List<UserGameReviewReplyData>();

								parentReply.Children.Add(data);
								parentReply.Children = parentReply.Children.OrderByDescending(c => c.CreateDate).ToList();
								var latestChildRepply = parentReply.Children.FirstOrDefault() ?? new UserGameReviewReplyData();
								if (latestChildRepply.UserId != null && DictUser.TryGetValue(latestChildRepply.UserId, out var childName))
								{
									latestChildRepply.UserName = childName;
								}
							}
						}
						// Mở danh sách reply
						if (!expandedReplies.Contains(review.Id))
						{
							expandedReplies.Add(review.Id);
						}
					}
					replyingToId = null;
					replyToUserName = null;
					replyingReviewId = null;
					replyContent = string.Empty;
					replyContentLevel2 = string.Empty;
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

		void ToggleReplyBoxForOneReply(string replyId, bool isAnonymous, string userName)
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

		//hàm rep bình luạn gốc
		void ToggleReplyBoxForOriginalComment(
				string reviewId,
				bool isAnonymous,
				string userName)
		{
			// đang mở reply review này => đóng
			if (replyingReviewId == reviewId)
			{
				replyingReviewId = null;
				replyToUserName = null;
				replyContent = string.Empty;
				return;
			}

			// mở reply comment gốc
			replyingReviewId = reviewId;

			// reply review gốc
			replyingToId = null;
			replyToUserName = userName;
			replyContent = string.Empty;
		}

		void HandleInputReply(ChangeEventArgs e)
		{
			replyContent = e.Value?.ToString();
		}

		void HandleInputReplyNextLevel(ChangeEventArgs e)
		{
			replyContentLevel2 = e.Value?.ToString();
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
					//IsDeleted = falses
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

		void AddEmojiNextLevelReply(string emoji)
		{
			replyContentLevel2 += emoji;
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

		async Task ChangeReactionReplyLevel1Async(UserGameReviewReplyData data)
		{
			try
			{
				var isDone = await ReactionService
					.ChangeReactionForReplyAsync(
						data.ReviewId,
						data.Id,
						currentUserId);

				if (isDone)
				{
					// Check lại trạng thái like
					var myReaction =
						await ReactionService.CheckDisplayMyReactionAsync(
							currentUserId,
							data.ReviewId,
							data.Id);

					var isLiked = !myReaction.IsNullOrEmpty();
					// Cập nhật trạng thái mình đã like hay chưa
					data.IsLikedByMe = isLiked;
					// Cập nhật LikeCount theo trạng thái mới
					if (isLiked)
					{
						data.LikeCount++;
					}
					else
					{
						data.LikeCount = Math.Max(
							0,
							data.LikeCount - 1);
					}
					StateHasChanged();
				}
			}
			catch
			{

			}
		}

		string FormatCorrectReactionCount(int count)
		{
			if (count < 1000)
				return count.ToString();

			if (count < 1_000_000)
			{
				var value = count / 1000.0;

				return value % 1 == 0
					? $"{value:0}K"
					: $"{value:0.#}K";
			}

			if (count < 1_000_000_000)
			{
				var value = count / 1_000_000.0;

				return value % 1 == 0
					? $"{value:0}M"
					: $"{value:0.#}M";
			}

			var billion = count / 1_000_000_000.0;

			return billion % 1 == 0
				? $"{billion:0}B"
				: $"{billion:0.#}B";
		}

		void ShowMoreChildReplies(string replyId, int totalCount)
		{
			var currentCount = displayedChildReplyCount.TryGetValue(
				replyId,
				out var count)
				? count
				: 2;

			displayedChildReplyCount[replyId] =
				Math.Min(currentCount + 2, totalCount);
		}

		void HideChildReplies(string replyId)
		{
			displayedChildReplyCount[replyId] = 2;
		}
	}
}
