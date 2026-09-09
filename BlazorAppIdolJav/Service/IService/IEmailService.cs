namespace GameManagement.Service.IService
{
	public interface IEmailService
	{
		public Task SendOtpAsync(string email, string otpNumber, int minuteExpired, string name);
	}
}
