using AntDesign;
using AutoMapper;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using GameManagement.Share.Extension;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Drawing;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.Service
{

	public class EmailService : IEmailService
	{
		private readonly IConfiguration _configuration;
		private readonly IWebHostEnvironment _environment;
		private readonly string senderEmail;
		private readonly string senderPassword;
		private readonly string smtpServer;
		private readonly string port;
		private readonly IEmailRepository _repo;
		readonly IMapper _mapper;

		public EmailService(IConfiguration configuration, IWebHostEnvironment environment,
				IMapper mapper,
				IEmailRepository repo)
		{
			_configuration = configuration;
			_environment = environment;
			senderEmail = _configuration["EmailSettings:Email"] ?? "test123@gmail.com";
			senderPassword = _configuration["EmailSettings:Password"] ?? "";
			smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "";
			port = _configuration["EmailSettings:Port"] ?? "";
			_mapper = mapper;
			_repo = repo;
		}

		//viết hàm tách logic gửi mail, truyền enum loại mail tương ứng
		public async Task SendTemplateMailAsync(MailType type, object data)
		{
			try
			{
				MimeMessage message;
				string userReceiptId = string.Empty;
				switch (type)
				{
					case MailType.OTP:
						message = await BuildOtpMailAsync(data);
						if (data is OtpMailData otpData)
						{
							userReceiptId = otpData.Name;
						}
						break;

					case MailType.RegisterAccountSuccess:
						message = await BuildRegisterSuccessMailAsync(data);
						if (data is RegisterSuccessMailData registerData)
						{
							userReceiptId = registerData.UserName;
						}
						break;

					case MailType.PurchaseSuccess:
						message = await BuildPurchaseReceiptMailAsync(data);
						if (data is PurchaseGameReceiptData purchaseData)
						{
							userReceiptId = purchaseData.UserName;
						}
						break;

					default:
						throw new ArgumentOutOfRangeException(
							nameof(type),
							type,
							"Mail type is not supported."
						);
				}

				await SendMailAsync(message);
				// luu mail vao db 
				await SaveMailAsync(new EmailInformationData
				{
					Id = ObjectExtentions.GenerateGuid(),
					CreateDate = DateTime.Now,
					MailType = type.ToString(),
					Subject = message.Subject ?? "",
					Content = (message.Body as TextPart)?.Text ?? "",
					MailSend = senderEmail,
					MailReceipt = message.To.Mailboxes.FirstOrDefault()?.Address ?? "",
					UserSendId = "ADMIN",
					UserReceiptId = userReceiptId,
					Status = "Success" // luôn gán = thành công
				});
			}
			catch (Exception ex)
			{
				// TODO: log lỗi
				throw;
			}
		}

		async Task SendMailAsync(MimeMessage message)
		{
			using var smtp = new SmtpClient();

			await smtp.ConnectAsync(
				smtpServer,
				port.ToInt(),
				SecureSocketOptions.StartTls
			);

			await smtp.AuthenticateAsync(
				senderEmail,
				senderPassword
			);

			await smtp.SendAsync(message);

			await smtp.DisconnectAsync(true);
		}

		async Task<MimeMessage> BuildOtpMailAsync(object data)
		{
			try
			{
				if (data is not OtpMailData otpData)
				{
					throw new ArgumentException(
						"Invalid data for OTP mail.",
						nameof(data)
					);
				}

				var message = new MimeMessage();

				message.From.Add(
					new MailboxAddress(
						"Thông báo cấp mã xác thực OTP",
						senderEmail
					)
				);

				message.To.Add(
					MailboxAddress.Parse(otpData.Email)
				);

				message.Subject =
					$"{otpData.Otp} là mã xác minh của bạn (Test version)";

				var templatePath = Path.Combine(
					_environment.ContentRootPath,
					"Templates",
					"OtpTemplate.html"
				);

				var html = await File.ReadAllTextAsync(templatePath);

				html = html
					.Replace("{{OTP}}", otpData.Otp)
					.Replace("{{EXPIRE_MINUTES}}", otpData.Minute.ToString())
					.Replace("{{NAME}}", otpData.Name);

				message.Body = new TextPart("html")
				{
					Text = html
				};

				return message;
			}
			catch
			{
				return new MimeMessage();
			}
		}

		async Task<MimeMessage> BuildRegisterSuccessMailAsync(object data)
		{
			try
			{
				if (data is not RegisterSuccessMailData successData)
				{
					throw new ArgumentException(
						"Invalid data for Register mail.",
						nameof(data)
					);
				}

				var message = new MimeMessage();

				message.From.Add(
					new MailboxAddress(
						"Thông báo đăng kí tài khoản thành công",
						senderEmail
					)
				);

				message.To.Add(
					MailboxAddress.Parse(successData.Email)
				);

				message.Subject =
					"Đăng kí account thành công (Test version)";

				var templatePath = Path.Combine(
					_environment.ContentRootPath,
					"Templates",
					"RegisterSuccessTemplate.html"
				);

				var html = await File.ReadAllTextAsync(templatePath);

				html = html
					.Replace("{{FULLNAME}}", successData.FullName)
					.Replace("{{USERNAME}}", successData.UserName)
					.Replace("{{REGISTRATIONDATE}}", successData.RegistrationDate.GetFormatDateWithMinute());

				message.Body = new TextPart("html")
				{
					Text = html
				};

				return message;
			}
			catch
			{
				return new MimeMessage();
			}
		}

		async Task<MimeMessage> BuildPurchaseReceiptMailAsync(object data)
		{
			try
			{
				if (data is not PurchaseGameReceiptData receiptData)
				{
					throw new ArgumentException(
						"Invalid data for receipt mail.",
						nameof(data)
					);
				}

				var message = new MimeMessage();

				message.From.Add(
					new MailboxAddress(
						"Thông báo nhận bill receipt (test)",
						senderEmail
					)
				);

				message.To.Add(
					MailboxAddress.Parse(receiptData.Email)
				);

				message.Subject = "Đây là receipt mua game (test version)";

				var templatePath = Path.Combine(
					_environment.ContentRootPath,
					"Templates",
					"GamePurchaseReceipt.html"
				);

				var html = await File.ReadAllTextAsync(templatePath);

				html = html
					.Replace("{{FULLNAME}}", receiptData.FullName)
					.Replace("{{USERNAME}}", receiptData.UserName)
					.Replace("{{GAMENAME}}", receiptData.GameName)
					.Replace("{{TRANSACTIONID}}", receiptData.TransactionId)
					.Replace("{{PURCHASEDATE}}", receiptData.PurchaseDate)
					.Replace("{{ORIGINALPRICE}}", receiptData.OriginalPrice)
					.Replace("{{DISCOUNTPERCENT}}", receiptData.DiscountPercent)
					.Replace("{{PURCHASEPRICE}}", receiptData.PurchasePrice);

				message.Body = new TextPart("html")
				{
					Text = html
				};

				return message;
			}
			catch
			{
				return new MimeMessage();
			}
		}

		public async Task<List<EmailInformationData>> GetAllWithFilterAsync(MailSearch search)
		{
			var filter = search.CreateFilter(_repo.GetQueryable());
			var result = await _repo.GetAllWithFilterAsync(filter, search);
			var data = _mapper.Map<List<EmailInformationData>>(result);
			return data;
		}

		public async Task<bool> SaveMailAsync(EmailInformationData data)
		{
			try
			{
				var mailInfor = _mapper.Map<EmailInformation>(data);
				var isSuccess = await _repo.SaveMailAsync(mailInfor);
				return isSuccess;
			}
			catch
			{
				throw;
			}
		}
	}
}
