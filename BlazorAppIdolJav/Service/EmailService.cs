namespace GameManagement.Service
{
	using GameManagement.Service.IService;
	using MailKit.Net.Smtp;
	using MailKit.Security;
	using MimeKit;

	public class EmailService : IEmailService
	{
		private readonly IConfiguration _configuration;
		private readonly IWebHostEnvironment _environment;
		public EmailService(IConfiguration configuration, IWebHostEnvironment environment)
		{
			_configuration = configuration;
			_environment = environment;
		}

		public async Task SendOtpAsync(string email, string otp, int minute, string name)
		{
			var senderEmail = _configuration["EmailSettings:Email"] ?? "test123@gmail.com";
			var senderPassword = _configuration["EmailSettings:Password"] ?? "";
			var smtpServer = _configuration["EmailSettings:SmtpServer"];
			var port = int.Parse(_configuration["EmailSettings:Port"]);

			var message = new MimeMessage();
			message.From.Add(
				new MailboxAddress(
					"Thông báo cấp mã xác thực OTP",
					senderEmail
				)
			);

			message.To.Add(
				MailboxAddress.Parse(email)
			);

            message.Subject = $"{otp} là mã xác minh của bạn (Test version)";

            var templatePath = Path.Combine(
				_environment.ContentRootPath,
				"Templates",
				"OtpTemplate.html"
			);

			var html = await File.ReadAllTextAsync(templatePath);

			html = html
				.Replace("{{OTP}}", otp)
				.Replace("{{EXPIRE_MINUTES}}", minute.ToString())
				.Replace("{{NAME}}", name);

			message.Body = new TextPart("html")
			{
				Text = html
			};
			using var smtp = new SmtpClient();

			await smtp.ConnectAsync(
				smtpServer,
				port,
				SecureSocketOptions.StartTls
			);

			await smtp.AuthenticateAsync(
				senderEmail,
				senderPassword
			);

			await smtp.SendAsync(message);

			await smtp.DisconnectAsync(true);
		}
	}
}
