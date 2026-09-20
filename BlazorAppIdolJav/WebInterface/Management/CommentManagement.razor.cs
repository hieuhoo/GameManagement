using AntDesign;
using AntDesign.TableModels;
using AutoMapper;
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

        [Inject] IUserGameReviewHistoryService ReviewHistoryService { get; set; }
        [Inject] IMapper Mapper { get; set; }

        Dictionary<string, string> GameDict = new Dictionary<string, string>();
        List<UserGameReviewViewModel> ViewModels { get; set; } = new List<UserGameReviewViewModel>();
        Table<UserGameReviewViewModel> Table;

        bool loading;
        bool showUserReaction;
        string cancelText = "Đóng";
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await LoadAllGameAsync();
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

        async Task OpenListUsersReaction(string type)
        {
            try
            {
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
