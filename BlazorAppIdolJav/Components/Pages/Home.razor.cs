
using GameManagement.Components.Layout;
using GameManagement.Service;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.Model.EditModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.Components.Pages
{
    public partial class Home : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        public AuthenticationStateProvider AuthProvider { get; set; } = default!;
        [Inject] IGameService GameService { get; set; }
        [Inject] IGameCompanyService GameCompanyService { get; set; }
        [Inject] IGameTypeService GameTypeService { get; set; }
        [Inject] IGameDiscountService GameDiscountService { get; set; }


        [CascadingParameter]
        public MainLayout MainLayout { get; set; } = default!;
        List<GameData> FeaturedGames { get; set; } = new List<GameData>();
        List<GameCompanyData> CompanyDatas { get; set; } = new List<GameCompanyData>();
        List<GameTypeData> TypeDatas { get; set; } = new List<GameTypeData>();

        Dictionary<string, string> CompanyDict = new Dictionary<string, string>();

        string? redirectAfterLogin;
        System.Threading.Timer? discountTimer;
        DateTime currentTime = DateTime.Now;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await GetGameCompanyAsync();
                await GetGameTypeAsync();
                await GetTopFeaturegameAsync();
                // Cập nhật thời gian mỗi 1 phút
                discountTimer = new System.Threading.Timer(
                    async _ =>
                    {
                        currentTime = DateTime.Now;

                        await InvokeAsync(StateHasChanged);
                    },
                    null,
                    TimeSpan.FromMinutes(1),
                    TimeSpan.FromMinutes(1));
            }
            catch (Exception)
            {

            }
        }

        async Task HandleBrowseGame()
        {
            try
            {
                var authState = await AuthProvider.GetAuthenticationStateAsync();

                if (authState.User.Identity?.IsAuthenticated == true)
                {
                    NavigationManager.NavigateTo("/danh-sach-game");
                    return;
                }

                await MainLayout.ShowLoginModalAsync("/danh-sach-game");
            }
            catch
            {

            }
        }

        async Task GetTopFeaturegameAsync()
        {
            try
            {
                var data = await GameService.GetAllWithFilterAsync(new GameSearch
                {
                    IsFeatured = true
                });
                FeaturedGames = data.OrderBy(x => x.OrderFeatured).ToList();
                foreach (var game in FeaturedGames)
                {
                    if (game.GameCompanyId != null &&
                        CompanyDict.TryGetValue(game.GameCompanyId, out var companyName))
                    {
                        game.CompanyName = companyName;
                    }
                    var discountResult = await GetCurrentDiscountAsync(game.Id);
                    game.DiscountPercent = discountResult.Item1;
                    game.DiscountEndDate = discountResult.Item2;
                    game.CurrentPrice = game.Price * (100  - game.DiscountPercent)/100;
                }
            }
            catch
            {

            }
        }

        async Task GetGameCompanyAsync()
        {
            try
            {
                var data = await GameCompanyService.GetAllWithFilterAsync(new GameCompanySearch
                {

                });
                CompanyDatas = data;
                CompanyDict = CompanyDatas.ToDictionary(x => x.Id, x => x.Name);
            }
            catch
            {

            }
        }

        async Task GetGameTypeAsync()
        {
            try
            {
                var data = await GameTypeService.GetAllWithFilterAsync(new GameTypeSearch
                {

                });
                TypeDatas = data;
            }
            catch
            {

            }
        }

        async Task<(int, DateTime)> GetCurrentDiscountAsync(string gameId)
        {
            try
            {
                var discounts = await GameDiscountService.GetAllWithFilterAsync(
                    new DiscountSearch
                    {
                        GameId = gameId
                    });

                var now = DateTime.Now;

                var currentDiscount = discounts
                    .Where(x =>
                        x.StartDate <= now &&
                        x.EndDate >= now)
                    .OrderByDescending(x => x.StartDate)
                    .FirstOrDefault() ?? new() ;
                return (currentDiscount?.DiscountPercent ?? 0 , currentDiscount.EndDate);
            }
            catch
            {
                return (0, DateTime.Now);
            }
        }

        string ConvertMoneyUnit (string baseUnit)
        {
            string desiredUnit = string.Empty;
            if (baseUnit == "VNĐ")
            {
                return "đ";
            }
            else
            {
                return baseUnit;
            }
        }

        string GetRemainingTime(DateTime endDate)
        {
            var remaining = endDate - currentTime;

            if (remaining.TotalSeconds <= 0)
            {
                return "đã kết thúc";
            }

            if (remaining.TotalDays >= 1)
            {
                return $"{remaining.Days} ngày {remaining.Hours} giờ nữa";
            }

            if (remaining.TotalHours >= 1)
            {
                return $"{remaining.Hours} giờ {remaining.Minutes} phút nữa";
            }

            return $"{remaining.Minutes} phút nữa";
        }

    }
}
