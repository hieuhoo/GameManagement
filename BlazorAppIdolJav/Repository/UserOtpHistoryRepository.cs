using AntDesign;
using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
	public class UserOtpHistoryRepository : Repository<UserOtpHistory>, IUserOtpHistoryRepository
	{
		private readonly ApplicationDbContext _context;
		public UserOtpHistoryRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<bool> AddOtpHistoryAsync(UserOtpHistory data)
		{
			await AddAsync(data);
			return true;
		}

		public async Task<UserOtpHistory> GetLatestOtpAsync(string email, string otpType)
		{
			return await _context.UserOtpHistory
				.Where(x =>
					x.Email == email &&
					x.OtpType == otpType)
				.OrderByDescending(x => x.CreateDate)
				.FirstOrDefaultAsync() ?? new UserOtpHistory();
		}

		public IQueryable<UserOtpHistory> GetQueryable()
		{
			return _context.UserOtpHistory.AsQueryable();
		}

		public async Task<bool> UpdateOtpHistoryAsync(UserOtpHistory data)
		{
			try
			{
				var existing = await _context.Set<UserOtpHistory>().FirstOrDefaultAsync(c => c.Id == data.Id);
				if (existing == null)
				{
					return false;
				}
				_context.Entry(existing).CurrentValues.SetValues(data);
				await _context.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
