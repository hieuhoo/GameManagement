using AntDesign;
using AutoMapper;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.Extension;
using GameManagement.Share.Model.ViewModel;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;

namespace GameManagement.WebInterface.GameInformation
{
    public partial class GameRedeemCode : ComponentBase
    {
        [Inject] IGameRedeemCodeService RedeemService { get; set; }
        [Inject] IMapper Mapper { get; set; }

        [Inject] NotificationService Notice { get; set; }

        List<GameViewModel> ViewModels { get; set; }
        List<GameRedeemCodeData> RedeemDatas { get; set; }

        Table<GameViewModel> Table;
        GameRedeemCodeDetail redeemDetailRef = new GameRedeemCodeDetail();

        bool loading;
        string title;


        protected override async Task OnInitializedAsync()
        {
            try
            {
                //width = ConfigTemplate.Width /2;
                //height = ConfigTemplate.Height;
            }
            catch (Exception)
            {
                throw;
            }
        }

    

        public async Task AddNewRedeemCode()
        {
            try
            {
                await redeemDetailRef.OpenRedeemForm();
            }
            catch (Exception)
            {

            }
        }
    }
}
