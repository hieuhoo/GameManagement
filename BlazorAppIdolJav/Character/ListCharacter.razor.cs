using Microsoft.AspNetCore.Components;
using BlazorAppIdolJav.Service.IService;
using static BlazorAppIdolJav.Share.Extension.EnumExtension;
using BlazorAppIdolJav.Share.ClassDB;
using BlazorAppIdolJav.CoreConfig.Extensions;

namespace BlazorAppIdolJav.Character
{
    public partial class ListCharacter : ComponentBase
    {
        [Inject] IActressService ActressService { get; set; }

        protected List<Actress> Character = new List<Actress>();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await GetActressDataAsync();
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
