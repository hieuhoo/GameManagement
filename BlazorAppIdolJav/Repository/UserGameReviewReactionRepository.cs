using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
	public class UserGameReviewReactionRepository : Repository<UserGameReviewReaction>, IUserGameReviewReactionRepository
	{
		private readonly ApplicationDbContext _context;

		public UserGameReviewReactionRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public IQueryable<UserGameReviewReaction> GetQueryable()
		{
			return _context.UserGameReviewReaction.AsQueryable();
		}
	}
}
