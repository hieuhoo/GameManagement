using AntDesign;
using AutoMapper;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Service.IService;
using GameManagement.Services;
using GameManagement.Share.ClassData;
using GameManagement.Share.Extension;
using GameManagement.Share.Model.EditModel;
using GameManagement.SpecialComponent.ExtensionClass;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using static GameManagement.Share.Extension.EnumExtension;
using static GameManagement.Share.Extension.MessageEnumExtension;

namespace GameManagement.Components.Layout
{
    public partial class MainLayout
    {
        [Inject] IMapper Mapper { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] NotificationService NoticeService { get; set; }
        [Inject] AuthenticationStateProvider AuthProvider { get; set; }

        UserEditModel EditModel { get; set; } = new UserEditModel();
        UserData Data { get; set; } = new UserData();

        InputWatcher inputWatcher;

        bool loginVisible;
        bool registerVisible;
        bool isLoggedIn = false;
        bool error;
        string currentUser;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                EditModel = new UserEditModel();
                var authState = await AuthProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                isLoggedIn = user.Identity?.IsAuthenticated ?? false;
                currentUser = user.Identity?.Name;
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
                var data = Mapper.Map<UserData>(EditModel);
                var isLoginSuccess = await UserService.CheckUserLoginAsync(data);
                if (isLoginSuccess)
                {
                    Data = await UserService.GetUserInfoAsync(new UserSearch
                    {
                        UserName = EditModel.UserName
                    });
                    var role = Data.Role.Trim();
                    if (string.Equals(role, UserRole.Admin.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        currentUser = UserRole.Admin.GetDescription();
                    }
                    else
                    {
                        currentUser = Data.Name.Trim();
                    }
                    await ((CustomAuthenticationStateProvider)AuthProvider)
                            .MarkUserAsAuthenticated(EditModel.UserName);
                    isLoggedIn = true;
                    loginVisible = false;
                    StateHasChanged();
                }
                else
                {
                    NoticeService.NotiError("Sai tên đăng nhập hoặc mật khẩu");
                }
            }
            catch (Exception ex)
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
                    NoticeService.NotiWarning(TypeAlert.InvalidData.GetDescription());
                    return;
                }
                var isExist = await UserService.CheckExistUserInfoAsync(new UserData
                {
                    UserName = EditModel.UserName,
                    Email = EditModel.Email,
                });
                if (isExist)
                {
                    NoticeService.NotiWarning(AccountRegisterEnum.ExistEmailOrUserName.GetDescription());
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
                    NoticeService.NotiSuccess(AccountRegisterEnum.Success.GetDescription());
                }
                else
                {
                    NoticeService.NotiWarning(AccountRegisterEnum.Failed.GetDescription());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                EditModel = new UserEditModel();
                Data = new UserData();
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

        async Task LogoutAccount()
        {
            try
            {
                await ((CustomAuthenticationStateProvider)AuthProvider).MarkUserAsLoggedOut();
                isLoggedIn = false;
                EditModel = new UserEditModel();
                error = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        string DisplayUserNameImage(string userName)
        {
            if (userName.IsNullOrEmpty())
            {
                return String.Empty;
            }
            if (string.Equals(userName, UserRole.Admin.GetDescription(), StringComparison.OrdinalIgnoreCase))
            {
                return GlobalVariant.AdminShortName;
            }
            var parts = userName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Concat(parts.TakeLast(2).Select(p => p[0])).ToUpper();
        }
    }
}
