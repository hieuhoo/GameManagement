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
using static GameManagement.Share.Extension.EnumExtension;
using static GameManagement.Share.Extension.MessageEnumExtension;

namespace GameManagement.WebInterface.Setup
{
    public partial class GameType : ComponentBase
    {
        [Inject] IMapper Mapper { get; set; }
        [Inject] IGameTypeService GameTypeService { get; set; }
        [Inject] NotificationService NoticeService { get; set; }
        List<GameTypeViewModel> ViewModels { get; set; } = new();
        List<SelectItem> StatusOptions { get; set; } = new();
        GameTypeData SelectModel { get; set; }
        List<GameTypeData> TypeDatas { get; set; }

        string ScrollY => (ConfigTemplate.Height - 360).ToString();
        Table<GameTypeViewModel> table;
        GameTypeEditModel EditModel;
        InputWatcher inputWatcher;

        bool loading;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                EditModel = new GameTypeEditModel();
                EditModel.ReadOnly = true;
                StatusOptions = Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>()
                     .Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
                await LoadingDataAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void OnRowClick(RowData<GameTypeViewModel> rowData)
        {
            try
            {
                SelectModel = TypeDatas.FirstOrDefault(c => c.Id == rowData.Data.Id) ?? new GameTypeData();
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
                EditModel = new GameTypeEditModel();
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
                var data = Mapper.Map<GameTypeData>(EditModel);
                var isSucess = await GameTypeService.SaveGameTypeAsync(data);
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
                var data = Mapper.Map<GameTypeData>(EditModel);
                var isSucess = await GameTypeService.UpdateGameTypeAsync(data);
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
                TypeDatas = await GameTypeService.GetAllWithFilterAsync(new GameTypeSearch
                {

                }) ?? new List<GameTypeData>();
                TypeDatas = TypeDatas.OrderByDescending(c => c.CreateDate).ToList();
                ViewModels = Mapper.Map<List<GameTypeViewModel>>(TypeDatas);
                int stt = 1;
                ViewModels.ForEach(c => c.Stt = stt++);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        async Task DeleteAsync(GameTypeData data)
        {
            try
            {
                var isSuccess = await GameTypeService.DeleteGameTypeAsync(data);
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
