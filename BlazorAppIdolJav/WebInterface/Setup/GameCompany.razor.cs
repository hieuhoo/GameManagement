using AntDesign;
using AntDesign.TableModels;
using AutoMapper;
using GameManagement.CoreConfig;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.Extension;
using GameManagement.Share.Model.EditModel;
using GameManagement.Share.Model.ViewModel;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;
using static GameManagement.Share.Extension.EnumExtension;
using static GameManagement.Share.Extension.MessageEnumExtension;

namespace GameManagement.WebInterface.Setup
{
    public partial class GameCompany : ComponentBase
    {
        [Inject] IMapper Mapper { get; set; }
        [Inject] IGameCompanyService CompanyService { get; set; }
        [Inject] NotificationService NoticeService { get; set; }
        List<GameCompanyViewModel> ViewModels { get; set; } = new();
        List<SelectItem> NationalOptions { get; set; } = new();
        List<SelectItem> StatusOptions { get; set; } = new();
        GameCompanyData SelectModel { get; set; }
        List<GameCompanyData> CompanyDatas { get; set; }

        string ScrollY => (ConfigTemplate.Height - 360).ToString();
        Table<GameCompanyViewModel> table;
        GameCompanyEditModel EditModel;
        InputWatcher inputWatcher;

        bool loading;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                EditModel = new GameCompanyEditModel();
                EditModel.ReadOnly = true;
                NationalOptions = Enum.GetValues(typeof(National)).Cast<National>()
                     .Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
                StatusOptions = Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>()
                     .Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
                await LoadingDataAsync();
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
                SelectModel = CompanyDatas.FirstOrDefault(c => c.Id == rowData.Data.Id) ?? new GameCompanyData();
                Mapper.Map(SelectModel, EditModel);
                EditModel.ReadOnly = true;
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
                EditModel = new GameCompanyEditModel();
                EditModel.ReadOnly = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void Edit()
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

        void Cancel()
        {
            try
            {
                EditModel.ReadOnly = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        void CancelChange()
        {
            try
            {
                EditModel = new();
                EditModel.ReadOnly = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task SaveAsync()
        {
            try
            {
                if (EditModel.Id.IsNotNullOrEmpty())
                {
                    await UpdateAsync();
                }
                else
                {
                    await CreateAsync();
                }
                await LoadingDataAsync();
                CancelChange();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task CreateAsync()
        {
            try
            {
                var errorMessageStore = EditModel.ValidateAll();
                if (!inputWatcher.Validate() || errorMessageStore?.Any() == true)
                {
                    if (errorMessageStore.Any())
                    {
                        inputWatcher.NotifyFieldChanged(errorMessageStore.First().Key, errorMessageStore);
                    }
                    NoticeService.NotiWarning(TypeAlert.InvalidData.GetDescription());
                    return;
                }
                EditModel.Id = ObjectExtentions.GenerateGuid();
                EditModel.CreateDate = DateTime.Now;
                var data = Mapper.Map<GameCompanyData>(EditModel);
                var isSucess = await CompanyService.SaveCompanyAsync(data);
                if (isSucess)
                {
                    NoticeService.NotiSuccess(OperationEnum.AddSuccessfully.GetDescription());
                }
                else
                {
                    NoticeService.NotiError(OperationEnum.AddFailed.GetDescription());
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        async Task UpdateAsync()
        {
            try
            {
                var errorMessageStore = EditModel.ValidateAll();
                if (!inputWatcher.Validate() || errorMessageStore?.Any() == true)
                {
                    if (errorMessageStore.Any())
                    {
                        inputWatcher.NotifyFieldChanged(errorMessageStore.First().Key, errorMessageStore);
                    }
                    NoticeService.NotiWarning(TypeAlert.InvalidData.GetDescription());
                    return;
                }
                var data = Mapper.Map<GameCompanyData>(EditModel);
                var isSucess = await CompanyService.UpdateCompanyAsync(data);
                if (isSucess)
                {
                    NoticeService.NotiSuccess(OperationEnum.UpdateSuccessfully.GetDescription());
                }
                else
                {
                    NoticeService.NotiError(OperationEnum.UpdateFailed.GetDescription());
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        async Task LoadingDataAsync()
        {
            try
            {
                CompanyDatas = await CompanyService.GetAllWithFilterAsync(new GameCompanySearch
                {

                }) ?? new List<GameCompanyData>();
                CompanyDatas = CompanyDatas.OrderByDescending(c => c.CreateDate).ToList();
                ViewModels = Mapper.Map<List<GameCompanyViewModel>>(CompanyDatas);
                int stt = 1;
                ViewModels.ForEach(c => c.Stt = stt++);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        async Task DeleteAsync(GameCompanyData data)
        {
            try
            {
                var isSuccess = await CompanyService.DeleteCompanyAsync(data);
                if (isSuccess)
                {
                    NoticeService.NotiSuccess(OperationEnum.DeleteSucessfully.GetDescription());
                    await LoadingDataAsync();
                }
                else
                {
                    NoticeService.NotiError(OperationEnum.DeleteFailed.GetDescription());
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
