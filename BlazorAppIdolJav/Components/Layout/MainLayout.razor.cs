using AntDesign;
using AutoMapper;
using GameManagement.Auth;
using GameManagement.Auth.Models;
using GameManagement.Components.State;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Service.IService;
using GameManagement.Services;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
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

namespace GameManagement.Components.Layout
{
	public partial class MainLayout
	{
		[Inject] IMapper Mapper { get; set; }
		[Inject] IUserService UserService { get; set; }
		[Inject] IAuthService AuthService { get; set; }
		[Inject] IEmailService EmailService { get; set; }
		[Inject] IUserOtpHistoryService OtpService { get; set; }
		[Inject] IUserPasswordHistoryService PasswordService { get; set; }
		[Inject] IUserLockHistoryService HistoryLockService { get; set; }
		[Inject] IUserGameWishlistService WishlistService { get; set; }
		[Inject] INotificationGameService NotiGameService { get; set; }
		[Inject] IGameService GameService { get; set; }


		[Inject] private WishlistState WishlistState { get; set; } = default!;
		[Inject] NavigationManager NavigationManager { get; set; } = default!;
		[Inject] NotificationService NoticeService { get; set; }
		[Inject] AuthenticationStateProvider AuthProvider { get; set; }

		UserEditModel EditModel { get; set; } = new UserEditModel();
		UserData Data { get; set; } = new UserData();
		UserData EmailUserData { get; set; } = new UserData();
		List<UserNotificationData> NotiList { get; set; } = new List<UserNotificationData>();
		Dictionary<string, UserData> DictUser = new Dictionary<string, UserData>();
		Dictionary<string, string> DictGame = new Dictionary<string, string>();

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
		bool isErrorPasswordLength = false;
		bool isErrorPasswordSpecial = false;
		string currentUser;
		string emailReceiveOtp = string.Empty;
		string otpValid = string.Empty;
		string? redirectAfterLogin;
		string currentUserId;
		int maxAttemptLogin = 3;
		int failedLoginCount;
		int lockTimeRelogin = 60;
		int minuteExpired = 3;
		int unreadNotificationCount;
		int visibleNotiCount = 5;
		int notiPageSize = 5;
		bool notificationVisible = false;
		string newPassword = string.Empty;
		string retypeNewPassword = string.Empty;


		protected override async Task OnInitializedAsync()
		{
			try
			{
				EditModel = new UserEditModel();
				var authState = await AuthProvider.GetAuthenticationStateAsync();
				var user = authState.User;
				currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				isLoggedIn = user.Identity?.IsAuthenticated ?? false;
				currentUser = user.Identity?.Name;
				isAdmin = user.IsInRole(UserRole.Admin.ToString());
				await GetListUserAsync();
				await GetListGameAsync();
				await GetNotificationForUserAsync(currentUserId ?? "");
				WishlistState.OnChange += OnWishlistChanged;
				if (!isAdmin)
				{
					var listData = await WishlistService.GetAllWithFilterAsync(new WishlistSearch
					{
						UserId = currentUserId
					});
					int count = listData.Count();
					WishlistState.SetCount(count);
				}
			}
			catch (Exception)
			{

			}
		}

		async Task GetListUserAsync()
		{
			try
			{
				var data = (await UserService.GetAllWithFilterAsync(new UserSearch
				{
					Role = UserRole.Normal.ToString()
				})) ?? new List<UserData>();
				DictUser = data.ToDictionary(x => x.Id, x => x);
			}
			catch
			{

			}
		}

		async Task GetListGameAsync()
		{
			try
			{
				var data = (await GameService.GetAllWithFilterAsync(new GameSearch
				{

				})) ?? new List<GameData>();
				DictGame = data.ToDictionary(x => x.Id, x => x.Name);
			}
			catch
			{

			}
		}

		async Task GetNotificationForUserAsync(string currentUserId)
		{
			try
			{
				NotiList = (await NotiGameService.GetAllWithFilterAsync(new NotiSearch
				{
					ReceiveUserId = currentUserId,
				})).OrderByDescending(c => c.CreateDate).ToList() ?? new List<UserNotificationData>();
				NotiList = NotiList.Where(c => c.ReceiveUserId != c.ActorUserId).ToList();
				unreadNotificationCount = NotiList.Where(c => c.IsRead == false).Count();
				await GetListUserAsync();
				await GetListGameAsync();
				foreach (var noti in NotiList)
				{
					noti.ActorUserName = noti.ActorDisplayName = string.Empty;
					if (DictUser.TryGetValue(noti.ActorUserId, out var dataUser))
					{
						noti.ActorUserName = dataUser.UserName;
						noti.ActorDisplayName = dataUser.Name;
					}
					if (DictGame.TryGetValue(noti.GameId, out var gameName))
					{
						noti.GameName = gameName;
					}
					noti.ContentView = BuildContentForNotification(noti);
				}
			}
			catch
			{

			}
		}

		string BuildContentForNotification(UserNotificationData noti)
		{
			string content = string.Empty;
			if (noti.Type == TypeNotication.Comment.ToString())
			{
				content = $"{noti.ActorDisplayName} ({noti.ActorUserName}) đã bình luận về đánh giá game {noti.GameName} của bạn";
			}
			if (new[]
			{
				TypeNotication.ReactHeartComment.ToString(),
				TypeNotication.ReactFunnyComment.ToString(),
				TypeNotication.ReactLikeComment.ToString()
			}.Contains(noti.Type))
			{
				content = $"{noti.ActorDisplayName} ({noti.ActorUserName}) đã bày tỏ cảm xúc về đánh giá game {noti.GameName} của bạn";
			}
			if (noti.Type == TypeNotication.ReplyComment.ToString())
			{
				content = $"{noti.ActorDisplayName} ({noti.ActorUserName}) đã phản hồi về bình luận của bạn trong 1 đánh giá game {noti.GameName}";
			}
			if (noti.Type == TypeNotication.ReplyCommentInReview.ToString())
			{
				content = $"{noti.ActorDisplayName} ({noti.ActorUserName}) đã phản hồi một bình luận trong bài đánh giá game {noti.GameName} của bạn.";
			}
			return content;
		}

		void OnWishlistChanged()
		{
			InvokeAsync(StateHasChanged);
		}

		public void Dispose()
		{
			WishlistState.OnChange -= OnWishlistChanged;
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
				error = false;
				showForgetPassLink = false;

				var result = await AuthService.LoginAsync(new LoginRequest
				{
					UserName = EditModel.UserName,
					Password = EditModel.PassWord
				});

				if (!result.Succeeded || result.Tokens is null)
				{
					showForgetPassLink = true;
					NoticeService.NotiError(result.ErrorMessage);
					return;
				}

				var authProvider = (CustomAuthenticationStateProvider)AuthProvider;
				await authProvider.MarkUserAsAuthenticatedAsync(result.Tokens);
				currentUserId = result.Tokens.UserId;
				isLoggedIn = true;
				isAdmin = string.Equals(
					result.Tokens.Role,
					UserRole.Admin.ToString(),
					StringComparison.OrdinalIgnoreCase);

				currentUser = isAdmin
					? UserRole.Admin.GetDescription()
					: result.Tokens.Name;

				// load lấy ra số thông báo
				await GetNotificationForUserAsync(currentUserId);
				loginVisible = false;
				EditModel = new UserEditModel();

				if (!string.IsNullOrWhiteSpace(redirectAfterLogin))
				{
					NavigationManager.NavigateTo(redirectAfterLogin);
					redirectAfterLogin = null;
				}

				StateHasChanged();
			}
			catch (Exception ex)
			{
				NoticeService.NotiError($"Đăng nhập thất bại: {ex.Message}");
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
					var history = EditModel.Id.FillPasswordHistoryData(EditModel.PassWord);
					await PasswordService.AddPasswordHistoryAsync(history);
					await EmailService.SendTemplateMailAsync(
						MailType.RegisterAccountSuccess,
						new RegisterSuccessMailData
						{
							UserName = EditModel.UserName,
							Email = EditModel.Email ?? "",
							FullName = EditModel.Name,
							RegistrationDate = DateTime.Now,
						}
					);

					NoticeService.NotiSuccess(AccountRegisterEnum.SuccessAndCheckMail.GetDescription());
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
				await ((CustomAuthenticationStateProvider)AuthProvider)
					.MarkUserAsLoggedOutAsync();

				isLoggedIn = false;
				isAdmin = false;
				currentUser = string.Empty;
				EditModel = new UserEditModel();
				error = false;

				NavigationManager.NavigateTo("/");
				StateHasChanged();
			}
			catch (Exception ex)
			{
				NoticeService.NotiError($"Đăng xuất thất bại: {ex.Message}");
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
				var name = EmailUserData?.UserName ?? "";
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
				await EmailService.SendTemplateMailAsync(
					  MailType.OTP,
					  new OtpMailData
					  {
						  Name = name,
						  Email = email,
						  Otp = otpNumber,
						  Minute = minuteExpired
					  }
				  );

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
				isErrorPass = false;
				isNotMatchPass = false;
				isErrorPasswordLength = false;
				isErrorPasswordSpecial = false;

				//không nhập
				if (newPassword.IsNullOrEmpty() || retypeNewPassword.IsNullOrEmpty())
				{
					isErrorPass = true;
					return;
				}

				// Mật khẩu phải từ 3 đến 8 ký tự
				if (newPassword.Length < 3 || newPassword.Length > 8)
				{
					isErrorPasswordLength = true;
					return;
				}

				// Phải có ít nhất 1 ký tự @ hoặc _
				if (!newPassword.Contains("@") && !newPassword.Contains("_"))
				{
					isErrorPasswordSpecial = true;
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
				var passwordData = data.Id.FillPasswordHistoryData(newPassword, data.PassWord);
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

		async Task ToggleNotification()
		{
			notificationVisible = !notificationVisible;
			if (notificationVisible)
			{
				visibleNotiCount = 5;
				await GetNotificationForUserAsync(currentUserId ?? "");
			}
		}

		void ViewAllNotiAsync()
		{
			visibleNotiCount += notiPageSize;
		}
	}
}
