using AntDesign;
using AntDesign.TableModels;
using AutoMapper;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.Model.ViewModel;
using Microsoft.AspNetCore.Components;
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

		[Inject] IMapper Mapper { get; set; }

		Dictionary<string, string> GameDict = new Dictionary<string, string>();
		Dictionary<string, string> UserDict = new Dictionary<string, string>();

		List<UserGameReviewViewModel> ViewModels { get; set; } = new List<UserGameReviewViewModel>();
		List<ReactionPersonInfoViewModel> ReactionViewModels { get; set; } = new List<ReactionPersonInfoViewModel>();

		Table<UserGameReviewViewModel> Table;
		Table<ReactionPersonInfoViewModel> PersonTable;


		bool loading;
		bool personLoading;

		bool showUserReaction;
		string titleText = string.Empty;
		string cancelText = "Đóng";
		protected override async Task OnInitializedAsync()
		{
			try
			{
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
					ReviewId = id
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
	}
}
