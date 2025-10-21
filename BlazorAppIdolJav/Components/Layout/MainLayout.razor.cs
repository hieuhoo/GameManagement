using AntDesign;
using AutoMapper;
using BlazorAppIdolJav.CoreConfig.Extensions;
using BlazorAppIdolJav.Service.IService;
using BlazorAppIdolJav.Share.ClassData;
using BlazorAppIdolJav.Share.Extension;
using BlazorAppIdolJav.Share.Model.EditModel;
using BlazorAppIdolJav.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using static BlazorAppIdolJav.Share.Extension.EnumExtension;
using static BlazorAppIdolJav.Share.Extension.MessageEnumExtension;

namespace BlazorAppIdolJav.Components.Layout
{
    public partial class MainLayout
    {
        [Inject] IMapper Mapper { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] NotificationService NoticeService { get; set; }
        UserEditModel EditModel { get; set; } = new UserEditModel();
        UserData Data { get; set; } = new UserData();

        InputWatcher inputWatcher;

        bool loginVisible;
        bool registerVisible;
        bool isLoggedIn = false;
        bool error;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                EditModel = new UserEditModel();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task HandleLoginAsync()
        {
            try
            {
                if (EditModel.UserName.IsNullOrEmpty() || EditModel.PassWord.IsNullOrEmpty())
                {
                    error = true;
                    return;
                }
                isLoggedIn = true;      
                loginVisible = false;
                StateHasChanged();
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }

        void RegisterAccount()
        {
            try
            {
                loginVisible = false;
                registerVisible = true;
                EditModel.IsRegister = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task HandleRegisterAsync()
        {
            try
            {
                var errorMessageStore = EditModel.ValidateAll();
                if (!inputWatcher.Validate() || errorMessageStore?.Any() == true || error)
                {
                    if (errorMessageStore.Any())
                    {
                        inputWatcher.NotifyFieldChanged(errorMessageStore.First().Key, errorMessageStore);
                    }
                    await NoticeService.Open(new NotificationConfig
                    {
                        Message = TypeAlert.ThongBao.GetDescription(),
                        Description = TypeAlert.InvalidData.GetDescription(),
                        NotificationType = NotificationType.Warning,
                        Placement = NotificationPlacement.TopRight
                    });
                    return;
                }
                var isExist = await UserService.CheckExistUserInfoAsync(new UserData
                {
                    UserName = EditModel.UserName,
                    Email = EditModel.Email,
                });
                if (isExist)
                {
                    await NoticeService.Open(new NotificationConfig
                    {
                        Message = TypeAlert.ThongBao.GetDescription(),
                        Description = AccountRegisterEnum.ExistEmailOrUserName.GetDescription(),
                        NotificationType = NotificationType.Warning,
                        Placement = NotificationPlacement.TopRight
                    });
                    return;
                }
                EditModel.Id = ObjectExtentions.GenerateGuid();
                EditModel.CreateDate = DateTime.Now;
                EditModel.Role = UserRole.Normal.ToString();
                EditModel.QuantityLoginCount = 0;
                Data = Mapper.Map<UserData>(EditModel);
                var result = await UserService.RegisterAccountAsync(Data);
                if (result)
                {
                    await NoticeService.Open(new NotificationConfig
                    {
                        Message = TypeAlert.ThongBao.GetDescription(),
                        Description = AccountRegisterEnum.Success.GetDescription(),
                        NotificationType = NotificationType.Success,
                        Placement = NotificationPlacement.TopRight
                    });
                }
                else
                {
                    await NoticeService.Open(new NotificationConfig
                    {
                        Message = TypeAlert.ThongBao.GetDescription(),
                        Description = AccountRegisterEnum.Failed.GetDescription(),
                        NotificationType = NotificationType.Warning,
                        Placement = NotificationPlacement.TopRight
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                EditModel = new UserEditModel();
                registerVisible = false;
            }
        }

        void CloseRegisterForm()
        {
            try
            {
                registerVisible = false;
                EditModel = new UserEditModel();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void BackToLogin()
        {
            try
            {
                registerVisible = false;
               loginVisible = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void ShowLoginModal()
        {
            loginVisible = true;
        }

        void LogoutAccount()
        {
            isLoggedIn = false;
            EditModel = new UserEditModel();
            error = false;
            StateHasChanged();
        }
    }
}
