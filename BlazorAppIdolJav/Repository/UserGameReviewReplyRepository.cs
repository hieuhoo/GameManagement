using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameManagement.Repository
{
	public class UserGameReviewReplyRepository : Repository<UserGameReviewReply>, IUserGameReviewReplyRepository
	{
		private readonly ApplicationDbContext _context;
		public UserGameReviewReplyRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<bool> AddReplyForCommentAsync(UserGameReviewReply reply)
		{
			try
			{
				await AddAsync(reply);
				return true;
			}
			catch (Exception ex)
			{
				return false;
				throw;
			}
		}

		public async Task<List<UserGameReviewReply>> GetAllWithFilterAsync(IQueryable<UserGameReviewReply> query, ReviewReplySearch search)
		{
			try
			{
				var result = await query.ToListAsync();
				return result;
			}
			catch (Exception)
			{
				return new List<UserGameReviewReply>();
			}
		}

		public IQueryable<UserGameReviewReply> GetQueryable()
		{
			return _context.UserGameReviewReply.AsQueryable();
		}

		public async Task<bool> UpdateReplyAsync(UserGameReviewReply reply)
		{
			try
			{
				var existing = await _context.Set<UserGameReviewReply>().FirstOrDefaultAsync(c => c.Id == reply.Id);
				if (existing == null)
				{
					return false;
				}
				_context.Entry(existing).CurrentValues.SetValues(reply);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception)
			{
				return false;
				throw;
			}
		}
	}
}
