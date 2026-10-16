using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
	public interface IEmailRepository
	{
		IQueryable<EmailInformation> GetQueryable();
		Task<List<EmailInformation>> GetAllWithFilterAsync(IQueryable<EmailInformation> query, MailSearch search);
		Task<bool> SaveMailAsync(EmailInformation data);
	}
}
