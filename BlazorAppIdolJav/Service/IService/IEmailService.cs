using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.Service.IService
{
	public interface IEmailService
	{
		public Task SendTemplateMailAsync(MailType type, Object data);
	}
}
