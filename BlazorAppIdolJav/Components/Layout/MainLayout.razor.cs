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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using static GameManagement.Share.Extension.EnumExtension;
using static GameManagement.Share.Extension.MessageEnumExtension;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.WebRequestMethods;

namespace GameManagement.Components.Layout
{
    public partial class MainLayout
    {
        [Inject] IMapper Mapper { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] IEmailService EmailService { get; set; }
        [Inject] IUserOtpHistoryService OtpService { get; set; }
        [Inject] IUserPasswordHistoryService PasswordService { get; set; }
        [Inject] IUserLockHistoryService HistoryLockService { get; set; }

		[Inject] NavigationManager NavigationManager { get; set; } = default!;
        [Inject] NotificationService NoticeService { get; set; }
        [Inject] AuthenticationStateProvider AuthProvider { get; set; }

        UserEditModel EditModel { get; set; } = new UserEditModel();
        UserData Data { get; set; } = new UserData();
        UserData EmailUserData { get; set; } = new UserData();

        InputWatcher inputWatcher;

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
        bool resetPassFormVisible = false;
        bool isErrorPass = false;
        bool isNotMatchPass = false;
        bool isAdmin = false;
        string currentUser;
        string emailReceiveOtp = string.Empty;
        string otpValid = string.Empty;
        string? redirectAfterLogin;
        int maxAttemptLogin = 3;
        int failedLoginCount;
        int lockTimeRelogin = 60;
        int minuteExpired = 3;
        string newPassword = string.Empty;
        string retypeNewPassword = string.Empty;
        //string userId = string.Empty;
        DateTime? lockoutUntil;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                EditModel = new UserEditModel();
                var authState = await AuthProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                //userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                isLoggedIn = user.Identity?.IsAuthenticated ?? false;
                currentUser = user.Identity?.Name;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ShowLoginModalAsync(string? redirectUrl = null)
        {
            redirectAfterLogin = redirectUrl;
            loginVisible = true;

            await InvokeAsync(StateHasChanged);
        }

        async Task HandleLoginAsync()
        {
            if (isLoggingIn)
                return;

            try
            {
                isLoggingIn = true;

                if (EditModel.UserName.IsNullOrEmpty() ||
                    EditModel.PassWord.IsNullOrEmpty())
                {
                    error = true;
                    return;
                }

                error = false;

                Data = await UserService.GetUserInfoAsync(new UserSearch
                {
                    UserName = EditModel.UserName
                });

                if (Data == null)
                {
                    showForgetPassLink = true;
                    NoticeService.NotiError("Sai tên đăng nhập hoặc mật khẩu.");
                    return;
                }

                if (string.Equals(
                    Data.Status,
                    AccountStatus.Lock.ToString(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    showForgetPassLink = false;
                    string reason = Data.LockReason.GetEnumDescription<AccountLockReason>();
                    NoticeService.NotiError(
                        $"Tài khoản đã bị {reason}. Vui lòng liên hệ quản trị viên để được mở khóa."
                    );
                    return;
                }
                // 4. Check username + password
                var loginData = Mapper.Map<UserData>(EditModel);

                var isLoginSuccess =
                    await UserService.CheckUserLoginAsync(loginData);

                if (isLoginSuccess)
                {
                    // Login thành công -> reset số lần đăng nhập sai
                    if (Data.FailedLoginCount > 0)
                    {
                        Data.FailedLoginCount = 0;
                        Data.UpdatedDate = DateTime.Now;

                        await UserService.UpdateAccountAsync(Data);
                    }

                    var role = Data.Role?.Trim();

                    if (string.Equals(
                        role,
                        UserRole.Admin.ToString(),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        currentUser = UserRole.Admin.GetDescription();
                        isAdmin = true;
                    }
                    else
                    {
                        currentUser = Data.Name.Trim();
                        isAdmin = false;
                    }

                    await ((CustomAuthenticationStateProvider)AuthProvider)
                        .MarkUserAsAuthenticated(EditModel.UserName);

                    isLoggedIn = true;
                    loginVisible = false;

                    StateHasChanged();
                }
                else
                {
                    // 6. Sai password
                    showForgetPassLink = true;

                    Data.FailedLoginCount++;
                    Data.UpdatedDate = DateTime.Now;

                    // 7. Sai đủ 3 lần -> Lock
                    if (Data.FailedLoginCount >= maxAttemptLogin)
                    {
                        showForgetPassLink = false;

                        Data.Status = AccountStatus.Lock.ToString();

                        Data.LockReason = AccountLockReason.ByIncorrectPassword.ToString();

                        Data.LockBy = LockPerson.System.ToString();

                        await UserService.UpdateAccountAsync(Data);
                        var lockHistory = Data.Id.FillLockHistoryData(
							AccountOperation.Lock,
							LockPerson.System,
							AccountLockReason.ByIncorrectPassword
						);
                        await HistoryLockService.AddLockHistoryAsync(lockHistory);
                        NoticeService.NotiError(
                            $"Bạn đã đăng nhập sai {maxAttemptLogin} lần. " +
                            "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên."
                        );

                        return;
                    }

                    // Chưa đủ 3 lần
                    await UserService.UpdateAccountAsync(Data);

                    var remainingAttempt =
                        maxAttemptLogin - Data.FailedLoginCount;

                    NoticeService.NotiError(
                        $"Sai tên đăng nhập hoặc mật khẩu. " +
                        $"Vui lòng ấn vào nút Quên mật khẩu hoặc" +
                        $" tài khoản của bạn sẽ bị khóa sau {remainingAttempt} lần nhập sai nữa."
                    );
                }
            }
            catch
            {
                throw;
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
                EditModel.Status = AccountStatus.Active.ToString();
                EditModel.FailedLoginCount = 0;
                Data = Mapper.Map<UserData>(EditModel);
                var result = await UserService.RegisterAccountAsync(Data);
                if (result)
                {
                    var history = new UserPasswordHistoryData
                    {
                        Id = ObjectExtentions.GenerateGuid(),
                        UserId = EditModel.Id,
                        CurrentPassword = EditModel.PassWord,
                        CurrentPasswordHash = StringExtension.GetCharacterHash(EditModel.PassWord),
                        CreateDate = DateTime.Now,
                        PreviousPassword = null
                    };
                    await PasswordService.AddPasswordHistoryAsync(history);
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

        async Task LogoutAccount()
        {
            try
            {
                await ((CustomAuthenticationStateProvider)AuthProvider).MarkUserAsLoggedOut();
                isLoggedIn = false;
                EditModel = new UserEditModel();
                error = false;
                NavigationManager.NavigateTo("/");
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
                    OtpCodeHash = StringExtension.GetCharacterHash(otpNumber),
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
            try
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

                await OtpService.UpdateOtpHistoryAsync(latestOtp);

                otpFormVisible = false;
                resetPassFormVisible = true;

                NoticeService.NotiSuccess("Xác thực OTP thành công");
            }
            catch
            {

            }
        }

        string GetGreetingByHour()
        {
            var hour = DateTime.Now.Hour;

            var greeting = hour switch
            {
                < 11 => "Chào buổi sáng",
                <= 12 => "Chào buổi trưa",
                <= 17 => "Chào buổi chiều",
                _ => "Chào buổi tối"
            };

            return $"{greeting}, {currentUser}";
        }

        void CloseFinalForm()
        {
            resetPassFormVisible = false;
        }

        void CloseLoginForm()
        {
            loginVisible = false;
            EditModel = new UserEditModel();
        }

        async Task ChangeNewPasswordAsync()
        {
            try
            {
                //không nhập
                if (newPassword.IsNullOrEmpty() || retypeNewPassword.IsNullOrEmpty())
                {
                    isErrorPass = true;
                    return;
                }
                // không khớp
                if (newPassword != retypeNewPassword)
                {
                    isNotMatchPass = true;
                    return;
                }

                var data = await UserService.GetUserInfoAsync(new UserSearch
                {
                    Email = emailReceiveOtp
                });

                //check trùng pass cũ 2 lần gần nhất
                int checkTime = 2;
                var checkResult = await PasswordService.CheckPasswordRecentlyUsedAsync(newPassword, data.Id, checkTime);
                if (checkResult)
                {
                    NoticeService.NotiError($"Mật khẩu mới trùng với {checkTime} mật khẩu gần nhất. " +
                                                $"Vui lòng nhập mật khẩu khác");
                    return;
                }

                // lưu 1 record vào bảng lịch sử
                var passwordData = new UserPasswordHistoryData
                {
                    Id = ObjectExtentions.GenerateGuid(),
                    UserId = data.Id,
                    CurrentPassword = newPassword,
                    PreviousPassword = data.PassWord,
                    CurrentPasswordHash = StringExtension.GetCharacterHash(newPassword),
                    CreateDate = DateTime.Now,
                };
                var result = await PasswordService.AddPasswordHistoryAsync(passwordData);
                if (result)
                {
                    data.PassWord = newPassword;
                    data.PasswordHash = StringExtension.GetCharacterHash(newPassword);
                    data.FailedLoginCount = 0;
                    await UserService.UpdateAccountAsync(data);
                    NoticeService.NotiSuccess("Đổi mật khẩu mới thành công");
                }
                else
                {
                    NoticeService.NotiError("Đổi mật khẩu mới thất bại, có lỗi xảy ra");
                }
                resetPassFormVisible = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
