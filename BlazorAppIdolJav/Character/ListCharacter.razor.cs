using AntDesign;
using AutoMapper;
using BlazorAppIdolJav.CoreConfig.Extensions;
using BlazorAppIdolJav.Service.IService;
using BlazorAppIdolJav.Share.ClassData;
using BlazorAppIdolJav.Share.ClassDB;
using BlazorAppIdolJav.Share.Extension;
using BlazorAppIdolJav.Share.Model.EditModel;
using BlazorAppIdolJav.Share.Model.ViewModel;
using BlazorAppIdolJav.SpecialComponent;
using Microsoft.AspNetCore.Components;
using static BlazorAppIdolJav.Share.Extension.EnumExtension;
using static BlazorAppIdolJav.Share.Extension.MessageEnumExtension;

namespace BlazorAppIdolJav.Character
{
    public partial class ListCharacter : ComponentBase
    {
        [Inject] IActressService ActressService { get; set; }
        [Inject] IMapper Mapper { get; set; }
        [Inject] NotificationService Notice { get; set; }

        List<ActressViewModel> ViewModels { get; set; }
        List<ActressData> ActressDatas { get; set; }

        int width;
        int height;
        Table<ActressViewModel> Table;
        CharacterDetail characterDetailRef;

        bool loading;
        bool createVisible;
        string title;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                width = ConfigTemplate.Width;
                height = ConfigTemplate.Height;
                await GetActressDataAsync();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task GetActressDataAsync()
        {
            try
            {
                var result = await ActressService.GetAllWithFilterAsync(new ActressSearch
                {
                    Country = National.Japan.ToString()
                });
                ActressDatas = result ?? new List<ActressData>();
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
                ViewModels = Mapper.Map<List<ActressViewModel>>(ActressDatas);
                int stt = 1;
                ViewModels.ForEach(c => c.Stt = stt++);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task UpdateAsync(ActressViewModel model)
        {
            try
            {
                createVisible = true;
                title = "Thông tin diễn viên";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task DeleteAsync(ActressViewModel model)
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
            title = "Thêm mới diễn viên";
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
