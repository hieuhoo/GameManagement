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
using Org.BouncyCastle.Asn1.X509;
using System.Security.Claims;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.WebInterface.Management
{
	public partial class CommentManagement : ComponentBase
	{
		[Inject] IUserGameReviewService ReviewService { get; set; }
		[Inject] IGameService GameService { get; set; }
		[Inject] IUserService UserService { get; set; }

		[Inject] IUserGameReviewHistoryService ReviewHistoryService { get; set; }
		[Inject] IUserGameReviewReactionService ReactionService { get; set; }
		[Inject] IUserGameReviewReplyService ReplyService { get; set; }


		[Inject] IMapper Mapper { get; set; }
		[Inject] NotificationService Notice { get; set; }
		Dictionary<string, string> GameDict = new Dictionary<string, string>();
		Dictionary<string, string> UserDict = new Dictionary<string, string>();

		List<UserGameReviewViewModel> ViewModels { get; set; } = new List<UserGameReviewViewModel>();
		List<ReactionPersonInfoViewModel> ReactionViewModels { get; set; } = new List<ReactionPersonInfoViewModel>();
		List<UserGameReviewReplyViewModel> ReplyViewModels { get; set; } = new List<UserGameReviewReplyViewModel>();

		List<UserGameReviewData> ReviewDatas { get; set; } = new List<UserGameReviewData>();
		List<UserGameReviewReplyData> ReplyDatas { get; set; } = new List<UserGameReviewReplyData>();

		List<UserGameReviewHistoryData> RevHistoryDatas { get; set; } = new List<UserGameReviewHistoryData>();

		Table<UserGameReviewViewModel> Table;
		Table<ReactionPersonInfoViewModel> PersonTable;
		Table<UserGameReviewReplyViewModel> ReplyTable;



		bool loading;
		bool personLoading;
		bool replyLoading;
		bool showReplyList;
		bool showUserReaction;
		bool showHideCommentForm;
		bool isRequired;
		string titleText = string.Empty;
		string replyTextTitle = string.Empty;

		string cancelText = "Đóng";
		string hideText = "Ẩn";
		string? hideReason;
		string? selectedReviewId;
		string historyTitle;
		bool historyVisible;
		string commentPerson;
		protected override async Task OnInitializedAsync()
		{
			try
			{
				await LoadAllReplyAsync();
				await LoadAllGameAsync();
				await LoadAllUserAsync();
				await LoadAllCommentAsync();
			}
			catch
			{

			}
		}

		async Task LoadAllCommentAsync()
		{
			try
			{
				var data = await ReviewService.GetAllWithFilterAsync(new ReviewSearch
				{

				}) ?? new List<UserGameReviewData>();
				ReviewDatas = data;
				ViewModels = Mapper.Map<List<UserGameReviewViewModel>>(data);
				ViewModels = ViewModels.OrderByDescending(c => c.GameId).ToList();
				int stt = 1;
				foreach (var item in ViewModels)
				{
					item.Stt = stt++;
					item.TotalReaction = item.HeartCount + item.LikeCount + item.FunnyCount;
					if (item.GameId != null && GameDict.TryGetValue(item.GameId, out var name))
					{
						item.GameName = name;
					}
					item.ReplyCount = ReplyDatas.Where(c => c.ReviewId == item.Id)
												.Where(c => c.ParentId == null).ToList().Count;
				}
			}
			catch (Exception ex)
			{

			}
		}

		async Task LoadAllGameAsync()
		{
			try
			{
				var data = await GameService.GetAllWithFilterAsync(new GameSearch
				{

				}) ?? new List<GameData>();
				GameDict = data.ToDictionary(c => c.Id, c => c.Name);
			}
			catch (Exception ex)
			{

			}
		}

		async Task LoadAllUserAsync()
		{
			try
			{
				var data = await UserService.GetAllWithFilterAsync(new UserSearch
				{
					Role = UserRole.Normal.ToString()
				}) ?? new List<UserData>();
				UserDict = data.ToDictionary(c => c.Id, c => c.UserName);
			}
			catch (Exception ex)
			{

			}
		}

		void OnRowClick(RowData<UserGameReviewViewModel> rowData)
		{
			try
			{

			}
			catch
			{

			}
			StateHasChanged();
		}

		async Task OpenListUsersReaction(string type, string id)
		{
			try
			{
				if (type == TypeReaction.Like.ToString())
				{
					titleText = $"Danh sách người {TypeReaction.Like.GetDescription()}";
				}
				else if (type == TypeReaction.Funny.ToString())
				{
					titleText = $"Danh sách người {TypeReaction.Funny.GetDescription()}";
				}
				else
				{
					titleText = $"Danh sách người {TypeReaction.Heart.GetDescription()}";
				}
				var data = await ReactionService.GetAllWithFilterAsync(new PersonReactionSearch
				{
					ReactionType = type,
					ReviewId = id,
					ReplyId = null
				});
				ReactionViewModels = Mapper.Map<List<ReactionPersonInfoViewModel>>(data);
				int stt = 1;
				foreach (var item in ReactionViewModels)
				{
					item.Stt = stt++;
					if (item.UserId != null && UserDict.TryGetValue(item.UserId, out var name))
					{
						item.UserName = name;
					}
				}
				showUserReaction = true;
			}
			catch
			{

			}
		}

		void CloseFormUser()
		{
			showUserReaction = false;
		}

		void CloseFormReplyHistory()
		{
			showReplyList = false;
		}

		void CloseHideCommentFormUser()
		{
			showHideCommentForm = false;
		}

		async Task OpenPopupConfirmHideAsync(string reviewId, bool isHidden)
		{
			if (isHidden == true)
			{
				var data = ReviewDatas.FirstOrDefault(c => c.Id == reviewId) ?? new UserGameReviewData();
				data.IsHidden = false; // bật hiển thị lên
				data.HideReason = null;
				var isSuccess = await ReviewService.ChangeCommentHideStatusAsync(data);
				if (isSuccess)
				{
					Notice.NotiSuccess("Bình luận đã được hiển thị lại");
					await LoadAllCommentAsync();
				}
				else
				{
					Notice.NotiError("Thay đổi trạng thái thất bại , đoán đi lỗi gì dcm");
				}
			}
			else
			{
				selectedReviewId = reviewId;
				hideReason = null;
				showHideCommentForm = true;
			}

		}

		async Task ConfirmHideCommentAsync()
		{
			try
			{
				if (string.IsNullOrWhiteSpace(selectedReviewId))
					return;

				if (string.IsNullOrWhiteSpace(hideReason))
				{
					isRequired = true;
					showHideCommentForm = true;
					return;
				}

				var data = ReviewDatas.FirstOrDefault(c => c.Id == selectedReviewId);

				if (data == null)
				{
					Notice.NotiError("Không tìm thấy bình luận");
					return;
				}

				data.IsHidden = true;
				data.HideReason = hideReason;

				var isSuccess = await ReviewService.ChangeCommentHideStatusAsync(data);

				if (isSuccess)
				{
					showHideCommentForm = false;
					selectedReviewId = null;
					hideReason = null;
					isRequired = false;
					Notice.NotiSuccess("Bình luận này đã bị ẩn khỏi cộng đồng");
					await LoadAllCommentAsync();
				}
				else
				{
					Notice.NotiError("Thay đổi trạng thái thất bại, đoán lỗi đi dcm");
				}
			}
			catch
			{

			}
		}

		async Task ViewHistoryComment(string reviewId)
		{
			try
			{
				RevHistoryDatas = await ReviewHistoryService.GetAllWithFilterAsync(new ReviewHistorySearch
				{
					ReviewId = reviewId
				}) ?? new List<UserGameReviewHistoryData>();
				var userId = RevHistoryDatas.Select(c => c.UserId).Distinct().FirstOrDefault();
				if (userId != null && UserDict.TryGetValue(userId, out var name))
				{
					commentPerson = name;
				}
				RevHistoryDatas = RevHistoryDatas.OrderBy(c => c.CreateDate).ToList();
				historyTitle = "Lịch sử thay đổi bình luận";
				historyVisible = true;
			}
			catch
			{

			}
		}

		void CloseHistoryModal()
		{
			historyVisible = false;
		}

		async Task LoadAllReplyAsync()
		{
			try
			{
				ReplyDatas = await ReplyService.GetAllWithFilterAsync(new ReviewReplySearch
				{

				}) ?? new List<UserGameReviewReplyData>();
			}
			catch
			{

			}
		}

		async Task OpenReplyForCommentAsync(UserGameReviewViewModel view)
		{
			try
			{
				replyTextTitle = $"Phản hồi bình luận: {StringExtension.TruncateText(view.Comment)}";
				await GetReplyCommentAsync(view);
				showReplyList = true;
			}
			catch
			{

			}
		}

		async Task GetReplyCommentAsync(UserGameReviewViewModel model)
		{
			try
			{
				var data = await ReplyService.GetAllWithFilterAsync(new ReviewReplySearch
				{
					ReviewId = model.Id,
					IsShowWithoutChild = true
				});
				ReplyViewModels = Mapper.Map<List<UserGameReviewReplyViewModel>>(data);
				int stt = 1;
				foreach (var item in ReplyViewModels)
				{
					item.Stt = stt++;
					if (item.UserId != null && UserDict.TryGetValue(item.UserId, out var name))
					{
						item.UserName = name;
					}
				}
			}
			catch
			{

			}
		}
	}
}
