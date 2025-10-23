using AntDesign;
using AutoMapper;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using GameManagement.Share.Extension;
using GameManagement.Share.Model.EditModel;
using GameManagement.Share.Model.ViewModel;
using GameManagement.SpecialComponent;
using Microsoft.AspNetCore.Components;
using static GameManagement.Share.Extension.EnumExtension;
using static GameManagement.Share.Extension.MessageEnumExtension;

namespace GameManagement.WebInterface.GameInformation
{
    public partial class ListGame : ComponentBase
    {
        [Inject] IGameService GameService { get; set; }
        [Inject] IMapper Mapper { get; set; }
        [Inject] NotificationService Notice { get; set; }

        List<GameViewModel> ViewModels { get; set; }
        List<GameData> GameDatas { get; set; }

        int width;
        int height;
        Table<GameViewModel> Table;
        GameDetail gameDetailRef;

        bool loading;
        bool createVisible;
        string title;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                width = ConfigTemplate.Width;
                height = ConfigTemplate.Height;
                await GetGameDataAsync();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task GetGameDataAsync()
        {
            try
            {
                var result = await GameService.GetAllWithFilterAsync(new GameSearch
                {
                    Country = National.Japan.ToString()
                });
                GameDatas = result ?? new List<GameData>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task LoadDataAsync()
        {
            try
            {
                ViewModels = Mapper.Map<List<GameViewModel>>(GameDatas);
                int stt = 1;
                ViewModels.ForEach(c => c.Stt = stt++);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task UpdateAsync(GameViewModel model)
        {
            try
            {
                createVisible = true;
                title = "Thông tin game";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task DeleteAsync(GameViewModel model)
        {
            try
            {

            }
            catch
            {

            }
        }

        async Task OpenCreateAsync()
        {
            createVisible = true;
            title = "Thêm mới game";
        }

        public void ReSize(int size)
        {
            if (size == 12)
            {
                width = ConfigTemplate.Width;
            }
            else
            {
                width = ConfigTemplate.Width / 2;
            }
            StateHasChanged();
        }
    }
}
