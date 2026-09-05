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
using static System.Net.WebRequestMethods;

namespace GameManagement.Components.Layout
{
    public partial class MainLayout
    {
        [Inject] IMapper Mapper { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] IEmailService EmailService { get; set; }
        [Inject] IUserOtpHistoryService OtpService { get; set; }


        [Inject] NotificationService NoticeService { get; set; }
        [Inject] AuthenticationStateProvider AuthProvider { get; set; }

        UserEditModel EditModel { get; set; } = new UserEditModel();
        UserData Data { get; set; } = new UserData();
        UserData EmailUserData { get; set; } = new UserData();

        InputWatcher inputWatcher;
        StringExtension extension = new StringExtension();

        bool loginVisible;
        bool registerVisible;
        bool isLoggedIn = false;
        bool error;
        bool showForgetPassLink;
        bool forgotPasswordVisible = false;
        bool otpFormVisible = false;
        bool missingEmail;
        bool missingOtp;
        bool isLoggingIn = false;
        string currentUser;
        string emailReceiveOtp = string.Empty;
        string otpValid = string.Empty;

        int maxAttemptLogin = 3;
        int failedLoginCount;
        int lockTimeRelogin = 60;
        int minuteExpired = 3;

        DateTime? lockoutUntil;

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
            if (isLoggingIn)
                return;
            try
            {
                isLoggingIn = true;
                if (EditModel.UserName.IsNullOrEmpty() || EditModel.PassWord.IsNullOrEmpty())
                {
                    error = true;
                    return;
                }

                if (EditModel.UserName.IsNotNullOrEmpty() && EditModel.PassWord.IsNotNullOrEmpty())
                {
                    error = false;
                }

                if (lockoutUntil.HasValue && DateTime.UtcNow < lockoutUntil.Value)
                {
                    showForgetPassLink = true;
                    var remainingSeconds = (int)Math.Ceiling(
                        (lockoutUntil.Value - DateTime.UtcNow).TotalSeconds
                    );
                    NoticeService.NotiError(
                        $"Bạn đã đăng nhập không thành công quá {maxAttemptLogin} lần. " +
                        $"Vui lòng thử lại sau {remainingSeconds} giây."
                    );

                    return;
                }

                if (lockoutUntil.HasValue && DateTime.UtcNow > lockoutUntil.Value)
                {
                    failedLoginCount = 0;
                    lockoutUntil = null;
                }

                var data = Mapper.Map<UserData>(EditModel);
                var isLoginSuccess = await UserService.CheckUserLoginAsync(data);
                if (isLoginSuccess)
                {
                    failedLoginCount = 0;
                    lockoutUntil = null;
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
                    failedLoginCount++;
                    showForgetPassLink = true;
                    if (failedLoginCount > maxAttemptLogin)
                    {
                        lockoutUntil = DateTime.UtcNow.AddSeconds(lockTimeRelogin);
                        NoticeService.NotiError(
                            $"Bạn đã đăng nhập sai quá {maxAttemptLogin} lần. " +
                            $"Vui lòng thử lại sau {lockoutUntil} giây."
                        );
                    }
                    else
                    {
                        var remainingAttempt = maxAttemptLogin - failedLoginCount;
                        NoticeService.NotiError($"Sai tên đăng nhập hoặc mật khẩu." +
                                            $"Bạn còn lại {remainingAttempt} lần thử ");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                isLoggingIn = false;
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


        public async Task SendOtpAsync(string email)
        {
            try
            {
                if (emailReceiveOtp.IsNullOrEmpty())
                {
                    missingEmail = true;
                    return;
                }
                var existData = await UserService.CheckExistEmailAsync(email);

                if (existData != true)
                {
                    NoticeService.NotiError("Email ko tồn tại trong hệ thống");
                    return;
                }

                EmailUserData = await UserService.GetUserInfoAsync(new UserSearch
                {
                    Email = email
                });
                var userId = EmailUserData?.Id ?? "";
                var otpNumber = Random.Shared.Next(100000, 999999).ToString();

                var otpHistory = new UserOtpHistoryData
                {
                    Id = ObjectExtentions.GenerateGuid(),
                    UserId = userId,
                    Email = email,
                    OtpCode = otpNumber,
                    OtpType = TypeOTP.ResetPassword.ToString(),
                    OtpCodeHash = extension.GetCharacterHash(otpNumber),
                    CreateDate = DateTime.Now,
                    ExpiredDate = DateTime.Now.AddMinutes(minuteExpired),
                    IsUsed = false
                };
                await OtpService.AddOtpHistoryAsync(otpHistory);
                await EmailService.SendOtpAsync(email, otpNumber, minuteExpired);

                forgotPasswordVisible = false;
                otpFormVisible = true;
                NoticeService.NotiSuccess("Gửi OTP thành công.Vui lòng kiểm tra email để biết thêm chi tiết");
            }
            catch
            {

            }
        }

        void OpenResetpassForm()
        {
            try
            {
                forgotPasswordVisible = true;
                showForgetPassLink = false;
            }
            catch
            {

            }
        }

        void CloseResetpassForm()
        {
            forgotPasswordVisible = false;
        }

        void CloseOTPForm()
        {
            otpFormVisible = false;
        }

        public async Task VerifyOtpAndContinueAsync(string otpNumber, string email)
        {
            if (otpValid.IsNullOrEmpty())
            {
                missingOtp = true;
                return;
            }
            var latestOtp = await OtpService.GetLatestOtpAsync(
                email,
                TypeOTP.ResetPassword.ToString()
            );

            if (latestOtp == null)
            {
                NoticeService.NotiError("Không tìm thấy mã OTP");
                return;
            }

            if (otpNumber != latestOtp.OtpCode)
            {
                NoticeService.NotiError("Mã OTP không chính xác");
                return;
            }

            if (latestOtp.IsUsed)
            {
                NoticeService.NotiError("Mã OTP đã được sử dụng");
                return;
            }

            if (DateTime.Now > latestOtp.ExpiredDate)
            {
                NoticeService.NotiError("Mã OTP đã hết hạn");
                return;
            }

            // 6. Verify OTP
            //var isValid = BCrypt.Net.BCrypt.Verify(
            //    otp,
            //    otpHistory.OtpCodeHash
            //);

           
            latestOtp.IsUsed = true;

            //await OtpService.UpdateAsync(latestOtp);

            // 8. Đóng form OTP
            otpFormVisible = false;

            // 9. Mở form nhập mật khẩu mới
            //resetPasswordVisible = true;

            NoticeService.NotiSuccess("Xác thực OTP thành công");
        }

    }
}
