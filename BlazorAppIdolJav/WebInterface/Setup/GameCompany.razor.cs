using AntDesign;
using AntDesign.TableModels;
using AutoMapper;
using GameManagement.CoreConfig;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.Extension;
using GameManagement.Share.Model.EditModel;
using GameManagement.Share.Model.ViewModel;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.WebInterface.Setup
{
    public partial class GameCompany : ComponentBase
    {
        //[Inject] TableLocale TableLocale { get; set; }

        List<GameCompanyViewModel> ViewModels { get; set; } = new();
        List<SelectItem> NationalOptions { get; set; } = new();
        List<SelectItem> StatusOptions { get; set; } = new();

        GameCompanyData SelectModel { get; set; }

        Table<GameCompanyViewModel> table;
        GameCompanyEditModel EditModel;
        InputWatcher inputWatcher;

        bool loading;
        string ScrollY => (ConfigTemplate.Height - 360).ToString();

        protected override void OnInitialized()
        {
            try
            {
                EditModel = new GameCompanyEditModel();
                EditModel.ReadOnly = true;
                NationalOptions = Enum.GetValues(typeof(National)).Cast<National>()
                     .Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
                StatusOptions = Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>()
                     .Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void OnRowClick(RowData<GameCompanyViewModel> rowData)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            StateHasChanged();
        }

        void Add()
        {
            try
            {
                EditModel.ReadOnly = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void Edit()
        {

        }

        void Cancel()
        {

        }

        async Task SaveAsync()
        {

        }

        async Task DeleteAsync(GameCompanyData data)
        {

        }
    }
}
