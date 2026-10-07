using AntDesign;
using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
	public class EmailRepository : Repository<EmailInformation>, IEmailRepository
	{
		private readonly ApplicationDbContext _context;

		public EmailRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<List<EmailInformation>> GetAllWithFilterAsync(IQueryable<EmailInformation> query, MailSearch search)
		{
			try
			{
				var result = await query.ToListAsync();
				return result;
			}
			catch
			{
				return new List<EmailInformation>();
				throw;
			}
		}

		public IQueryable<EmailInformation> GetQueryable()
		{
			return _context.EmailInformation.AsQueryable();
		}

		public async Task<bool> SaveMailAsync(EmailInformation data)
		{
			try
			{
				await AddAsync(data);
				return true;
			}
			catch
			{
				return false;
				throw;
			}
		}
	}
}
