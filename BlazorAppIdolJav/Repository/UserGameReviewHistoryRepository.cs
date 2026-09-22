using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
	public class UserGameReviewHistoryRepository : Repository<UserGameReviewHistory>, IUserGameReviewHistoryRepository
	{
		private readonly ApplicationDbContext _context;

		public UserGameReviewHistoryRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<List<UserGameReviewHistory>> GetAllWithFilterAsync(IQueryable<UserGameReviewHistory> query, ReviewHistorySearch search)
		{
			try
			{
				var result = await query.ToListAsync();
				return result;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public IQueryable<UserGameReviewHistory> GetQueryable()
		{
			return _context.UserGameReviewHistory.AsQueryable();
		}
	}
}
