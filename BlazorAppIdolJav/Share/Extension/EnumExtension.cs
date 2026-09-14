using System.ComponentModel.DataAnnotations;

namespace GameManagement.Share.Extension
{
	public class EnumExtension
	{
		public enum National
		{
			[Display(Name = "Nhật Bản")]
			Japan = 0,
			[Display(Name = "Mỹ")]
			America = 1,
			[Display(Name = "Đài Loan")]
			Taiwan,
			[Display(Name = "Việt Nam")]
			VietNam,
			[Display(Name = "Trung Quốc")]
			China,
			[Display(Name = "Ba Lan")]
			Poland,
			[Display(Name = "Anh Quốc")]
			England,
			[Display(Name = "Canada")]
			Canada,
		}

		public enum CharacterGender
		{
			[Display(Name = "Nam")]
			Male = 0,
			[Display(Name = "Nữ")]
			Female = 1,
			[Display(Name = "Khác")]
			Other = 2,
		}


		public enum UserRole
		{
			[Display(Name = "ADMIN")]
			Admin = 0,
			[Display(Name = "Bình thường")]
			Normal = 1,
		}

		public enum GameReleaseStatus
		{
			[Display(Name = "Chưa ra mắt")]
			UnReleased = 0,
			[Display(Name = "Đã ra mắt")]
			Released = 1,
		}

		public enum GameSaleStatus
		{
			[Display(Name = "Không bán")]
			NotForSale = 0,
			[Display(Name = "Đang bán")]
			OnSale = 1,
		}

		public enum GameActiveStatus
		{
			[Display(Name = "Không hoạt động")]
			Inactive = 0,

			[Display(Name = "Hoạt động")]
			Active = 1
		}

		public enum PlatformSystem
		{
			[Display(Name = "Window")]
			Window = 0,
			[Display(Name = "MacOS")]
			MacOS = 1,
		}

		public enum UnitMoneyEnum
		{
			[Display(Name = "VNĐ")]
			VNĐ = 0,
			[Display(Name = "USD")]
			USD = 1,
			[Display(Name = "HKD")] // HOngKong money
			HKD = 2,
			[Display(Name = "BAHT")] // Thailand money
			BAHT = 3,
		}

		public enum StatusEnum
		{
			[Display(Name = "Hiệu lực")]
			Active = 0,
			[Display(Name = "Không hiệu lực")]
			Disable = 1,
		}

		public enum GeneralGameSetup
		{
			[Display(Name = "Hãng game")]
			GameCompany = 0,
			[Display(Name = "Thể loại game")]
			GameType = 1,
			[Display(Name = "Giảm giá game")]
			GameDiscount = 2,
		}

		public enum SendOtpMethod // hiện tại chỉ hỗ trợ qua email
		{
			[Display(Name = "Qua email")]
			Email = 0,
			[Display(Name = "Qua tin nhắn")]
			SMS = 1,
		}

		public enum TypeOTP
		{
			[Display(Name = "Thay đổi mật khẩu")]
			ResetPassword = 0,
			[Display(Name = "Thay đổi email")]
			ResetEmail = 1
		}

		public enum AccountStatus
		{
			[Display(Name = "Hoạt động")]
			Active = 0,
			[Display(Name = "Ngừng hoạt động")]
			Inactive = 1,
			[Display(Name = "Khóa")]
			Lock = 2

		}

		public enum AccountLockReason
		{
			[Display(Name = "Khóa do vi phạm chính sách")]
			ByPolicyViolation = 0,
			[Display(Name = "Khóa do nhập sai mật khẩu nhiều lần")]
			ByIncorrectPassword = 1,
			[Display(Name = "Khóa do Admin ghét :))")]
			ByAdmin = 2,
		}

		public enum LockPerson
		{
			[Display(Name = "Admin")]
			Admin = 0,
			[Display(Name = "Hệ thống")]
			System = 1,
		}

		public enum AccountOperation
		{
			[Display(Name = "Khóa")]
			Lock = 0,
			[Display(Name = "Mở khóa")]
			Unlock = 1,
		}

		public enum MailType
		{
			[Display(Name = "Xác thực otp")]
			OTP = 0,
			[Display(Name = "Đăng kí tài khoản thành công")]
			RegisterAccountSuccess = 1,
			[Display(Name = "Mua game thành công (mock)")] // sẽ chưa có payment do là mock
			PurchaseSuccess = 2,
		}

		public enum GenerateRedeemType
		{
			[Display(Name = "Tạo một mã")]
			ByHand = 0,
			[Display(Name = "Tạo nhiều mã tự động")]
			Auto = 1,
		}

		public enum RedeemCodeStatus
		{
			[Display(Name = "Hiệu lực")]
			Available = 0,
			[Display(Name = "Đã sử dụng")]
			Used = 1,
			[Display(Name = "Khóa")]
			Lock = 2,
			[Display(Name = "Hết hạn")]
			Expired = 3,
		}

		public enum RedeemCodeType
		{
			[Display(Name = "Game trong thư viện")]
			Game = 0,
			[Display(Name = "Tiền trong ví")]
			Money = 1,
		}

		public enum WalletTransactionType
		{
			[Display(Name = "Quy đổi voucher redeem")]
			VoucherRedeem = 0,
			[Display(Name = "Thanh toán nạp tiền (mock)")]
			PurchaseMoney = 1,
			[Display(Name = "Mua game")]
			PurchaseGame = 2,
		}

		public enum WindowConfig
		{
			[Display(Name = "Window 10")]
			Win10 = 0,
			[Display(Name = "Window 11")]
			Win11 = 1,
			[Display(Name = "Window 12")]
			Win12 = 2,
		}

		public enum CPUConfig
		{
			[Display(Name = "Intel core i5")]
			IntelI5 = 0,
			[Display(Name = "Intel core i7")]
			IntelI7 = 1,
			[Display(Name = "Intel core i9")]
			IntelI9 = 2,
			[Display(Name = "Intel core ultra")]
			IntelUltra = 3,
			[Display(Name = "AMD ryzen 5")]
			Amd5 = 4,
			[Display(Name = "AMD ryzen 7")]
			Amd7 = 5,
			[Display(Name = "AMD ryzen 9")]
			Amd9 = 6,
		}
	}
}
