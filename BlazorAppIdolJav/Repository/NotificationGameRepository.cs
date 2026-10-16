using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameManagement.Repository
{
	public class NotificationGameRepository : Repository<UserNotification>, INotificationGameRepository
	{
		private readonly ApplicationDbContext _context;
		public NotificationGameRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<bool> AddNotificationAsync(UserNotification noti)
		{
			try
			{
				await AddAsync(noti);
				return true;
			}
			catch
			{
				return false;
				throw;
			}
		}

		public async Task<List<UserNotification>> GetAllWithFilterAsync(IQueryable<UserNotification> query, NotiSearch search)
		{
			try
			{
				var result = await query.ToListAsync();
				return result;
			}
			catch
			{
				return new List<UserNotification>();
				throw;
			}
		}

		public IQueryable<UserNotification> GetQueryable()
		{
			return _context.UserNotification.AsQueryable();
		}

		public async Task<bool> UpdateNotificationAsync(UserNotification noti)
		{
			try
			{
				var existing = await _context.Set<UserNotification>().FirstOrDefaultAsync(c => c.Id == noti.Id);
				if (existing == null)
				{
					return false;
				}
				_context.Entry(existing).CurrentValues.SetValues(noti);
				await _context.SaveChangesAsync();
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
