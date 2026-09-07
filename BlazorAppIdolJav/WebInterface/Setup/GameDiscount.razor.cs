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
    public partial class GameDiscount : ComponentBase
    {
        [Inject] IMapper Mapper { get; set; }
        [Inject] IGameService GameService { get; set; }
        [Inject] IGameDiscountService DiscountService { get; set; }

        [Inject] NotificationService NoticeService { get; set; }
        List<GameDiscountViewModel> ViewModels { get; set; } = new();
        GameDiscountData SelectModel { get; set; }
        List<GameDiscountData> DiscountDatas { get; set; } = new();
        List<SelectItem> StatusOptions { get; set; } = new();
        List<GameData> GameDatas { get; set; } = new();

        string ScrollY => (ConfigTemplate.Height - 360).ToString();

        Table<GameDiscountViewModel> table;
        GameDiscountEditModel EditModel;
        InputWatcher inputWatcher;

        bool loading;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                EditModel = new GameDiscountEditModel();
                EditModel.ReadOnly = true;
                StatusOptions = Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>()
                     .Select(v => new SelectItem(v.ToString(), v.GetDescription())).ToList();
                await LoadingGameDataAsync();
                await LoadingDiscountDataAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void OnRowClick(RowData<GameDiscountViewModel> rowData)
        {
            try
            {
                SelectModel = DiscountDatas.FirstOrDefault(c => c.Id == rowData.Data.Id) ?? new GameDiscountData();
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
                EditModel = new GameDiscountEditModel();
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
                await LoadingDiscountDataAsync();
                //CancelChange();
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
                var data = Mapper.Map<GameDiscountData>(EditModel);
                var isSucess = await DiscountService.SaveDiscountAsync(data);
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
                var data = Mapper.Map<GameDiscountData>(EditModel);
                var isSucess = await DiscountService.UpdateDiscountAsync(data);
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

        async Task LoadingDiscountDataAsync()
        {
            try
            {
                DiscountDatas = await DiscountService.GetAllWithFilterAsync(new DiscountSearch
                {

                }) ?? new List<GameDiscountData>();
                DiscountDatas = DiscountDatas.OrderByDescending(c => c.CreateDate).ToList();
                ViewModels = Mapper.Map<List<GameDiscountViewModel>>(DiscountDatas);
                int stt = 1;
                ViewModels.ForEach(c => c.Stt = stt++);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        async Task LoadingGameDataAsync()
        {
            try
            {
                GameDatas = await GameService.GetAllWithFilterAsync(new GameSearch
                {

                }) ?? new List<GameData>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        async Task DeleteAsync(GameDiscountData data)
        {
            try
            {
                var isSuccess = await DiscountService.DeleteDiscountAsync(data);
                if (isSuccess)
                {
                    NoticeService.NotiSuccess(OperationEnum.DeleteSucessfully.GetDescription());
                    await LoadingDiscountDataAsync();
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
