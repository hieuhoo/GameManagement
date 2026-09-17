using AutoMapper;
using GameManagement.Service;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.Model.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace GameManagement.WebInterface.User
{
    public partial class UserGameWishlist : ComponentBase
    {
        [Inject] NavigationManager NavManager { get; set; }
        [Inject] AuthenticationStateProvider AuthProvider { get; set; }
        [Inject] IUserGameWishlistService WishlistService { get; set; }
        [Inject] IGameService GameService { get; set; }
        [Inject] IMapper Mapper { get; set; }

        public List<GameViewModel> ViewModels { get; set; } = new();
        List<GameData> GameDatas { get; set; } = new();
        List<GameData> WishlistGameDatas { get; set; } = new();

        string currentUserId;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var authState = await AuthProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await GetAllGameAsync();
                await GetWishlistGameDataAsync();
            }
            catch
            {

            }
        }

        async Task GetWishlistGameDataAsync()
        {
            try
            {
                var data = await WishlistService.GetAllWithFilterAsync(new WishlistSearch
                {
                    UserId = currentUserId
                }) ?? new List<UserGameWishlistData>();
                var listGameIds = data.Select(c => c.GameId).ToHashSet(); 

                WishlistGameDatas = GameDatas
                    .Where(x => listGameIds.Contains(x.Id))
                    .ToList(); //list cac game wlist
                ViewModels = Mapper.Map<List<GameViewModel>>(WishlistGameDatas);
                Console.Write(ViewModels);
            }
            catch
            {

            }
        }

        async Task GetAllGameAsync()
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
