using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.Service.IService
{
	public interface IEmailService
	{
		public Task SendTemplateMailAsync(MailType type, Object data);
	}

	[DataContract]
	public class MailSearch
	{
		[DataMember(Order = 1)]
		public string Id { get; set; }

		[DataMember(Order = 2)]
		public virtual string UserSendId { get; set; }
		[DataMember(Order = 3)]
		public virtual string UserReceiptId { get; set; }
		[DataMember(Order = 4)]
		public virtual string Type { get; set; }

		public IQueryable<EmailInformation> CreateFilter(IQueryable<EmailInformation> filter)
		{
			if (Id.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.Id == Id);
			}
			if (UserSendId.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.UserSendId == UserSendId);
			}
			if (UserReceiptId.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.UserReceiptId == UserReceiptId);
			}
			return filter;
		}
	}
}
